using Shared.Application.Models;

namespace Users.Application.Services;

public interface IRefreshTokenService
{
    Task<Result<(string AccessToken, string RefreshToken)>> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}
