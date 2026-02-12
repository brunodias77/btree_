using BuildingBlocks.Application.Models;
using System.Security.Claims;

namespace Users.Application.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    (string Token, string TokenHash) GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
