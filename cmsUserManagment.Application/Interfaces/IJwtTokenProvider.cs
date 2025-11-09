using cms.Domain.Entities;

namespace cmsAuth.Application.Interfaces;

public interface IJwtTokenProvider
{
    string GenerateToken(string email, string id);
}