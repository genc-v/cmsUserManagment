using cmsUserManagment.Application.DTO;
using cmsUserManagment.Application.Interfaces;

using Google.Authenticator;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cmsUserManagment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public AuthController(IAuthenticationService authenticationService, IJwtTokenProvider jwtTokenProvider)
    {
        _authenticationService = authenticationService;
        _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser user)
    {
        var result = await _authenticationService.Register(user);
        return Ok(result);
    }

    [HttpGet("login")]
    public IActionResult Login(string email, string password)
    {
        var user = _authenticationService.Login(email, password);
        var token = _jwtTokenProvider.GenerateToken(email, "1231231", false);
        return Ok(user);
    }
    [HttpGet]
    public SetupCode Get()
    {

        string key = "teest";
        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

        SetupCode setupInfo = tfa.GenerateSetupCode("cms", "test@text.com", key, false);

        return setupInfo;
    }

    [HttpGet("testing")]
    public IActionResult Testing(string manualKey)
    {
        string key = "teest";
        TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
        bool result = tfa.ValidateTwoFactorPIN(key, manualKey);

        if (!result) return BadRequest("not working");
        return Ok("it works");
    }
}
