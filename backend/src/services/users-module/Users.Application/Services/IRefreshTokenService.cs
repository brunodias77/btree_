
using BuildingBlocks.Application.Models;

namespace Users.Application.Services;

/// <summary>
/// Serviço para gerenciamento de Refresh Tokens.
/// </summary>
public interface IRefreshTokenService
{
    Task<Result<(string AccessToken, string RefreshToken)>> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}
