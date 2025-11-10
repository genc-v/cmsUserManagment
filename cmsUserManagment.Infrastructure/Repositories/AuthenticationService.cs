using System.Text.Json;

using cms.Domain.Entities;

using cmsUserManagment.Application.Common.ErrorCodes;
using cmsUserManagment.Application.DTO;
using cmsUserManagment.Application.Interfaces;
using cmsUserManagment.Infrastructure.Persistance;

using Google.Authenticator;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace cmsUserManagment.Infrastructure.Repositories;

public class AuthenticationService(IDistributedCache cache, AppDbContext dbContext, IJwtTokenProvider jwtTokenProvider)
    : IAuthenticationService
{
    private readonly IDistributedCache _cache = cache;
    private readonly IJwtTokenProvider _jwtTokenProvider = jwtTokenProvider;
    private readonly AppDbContext _dbContext = dbContext;

    public object? getRightToken(User user)
    {
        if (user.TwoFactorSecret != null || !user.TwoFactorSecret.IsNullOrEmpty())
        {
            var token = _jwtTokenProvider.GenerateToken(user.Email, user.Id.ToString(), user.IsAdmin);
            return token;
        }

        var twoFactorCode = new TwoFactorAuthCodes
        {
            UserId = user.Id,
        };

        _dbContext.TwoFactorAuthCodes.Add(twoFactorCode);
        _dbContext.SaveChanges();

        return twoFactorCode.Id.ToString();
    }

    public object? Login(string email, string password)
    {
        var key = $"email:{email}";

        string? cachedUser = _cache.GetString(key);
        if (cachedUser != null)
        {
            User? userObj = JsonSerializer.Deserialize<User>(cachedUser);
            if (userObj != null)
            {
                return getRightToken(userObj);
            }
        }

        var user = _dbContext.Users.FirstOrDefault(e => e.Email == email && e.Password == password);

        if (user == null) throw new GeneralErrorCodes(GeneralErrorCodes.notFound.code, GeneralErrorCodes.notFound.message);

        return getRightToken(user);
    }

    public async Task<bool> Register(RegisterUser user)
    {
        var key = $"email:{user.Email}";
        var newUser = new User
        {
            Email = user.Email,
            Username = user.Username,
            Password = user.Password,
            TwoFactorSecret = user.TwoFactorSecret,
            IsAdmin = false,
            Id = default
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(newUser));
        return true;
    }

    public string RefreshToken(string token)
    {
        throw new NotImplementedException();
    }

    public void Logout(string token)
    {
        throw new NotImplementedException();
    }
    public string twoFactorAuthentication(Guid loginId, string code)
    {

        TwoFactorAuthCodes? token = _dbContext.TwoFactorAuthCodes.FirstOrDefault(e => e.Id == loginId);
        if (token != null && token.Expires > DateTime.Now) throw new AuthErrorCodes(AuthErrorCodes.tokenNotFound.code, AuthErrorCodes.tokenNotFound.message);

        var user = _dbContext.Users.FirstOrDefault(e => e.Id == token.UserId);
        if (user == null) throw new GeneralErrorCodes(GeneralErrorCodes.notFound.code, GeneralErrorCodes.notFound.message);


        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
        bool result = tfa.ValidateTwoFactorPIN(user.TwoFactorSecret, code);

        if (!result) throw new AuthErrorCodes(AuthErrorCodes.notCorrectCode.code, AuthErrorCodes.notCorrectCode.message);
        _dbContext.TwoFactorAuthCodes.Remove(token);
        _dbContext.SaveChanges();

        var jwtToken = _jwtTokenProvider.GenerateToken(user.Email, user.Id.ToString(), user.IsAdmin);

        return jwtToken;
    }
}
