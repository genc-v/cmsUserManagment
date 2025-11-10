using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cms.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; init; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }

    public string? TwoFactorSecret { get; set; }
    public bool IsAdmin { get; set; } = false;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<TwoFactorAuthCodes> TwoFactorAuthCodes { get; set; } = new List<TwoFactorAuthCodes>();
}
