using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.RefreshToken;

public interface IRefreshTokenUseCase : IUseCase<RefreshTokenInput, RefreshTokenOutput>
{
}
