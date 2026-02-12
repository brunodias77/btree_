using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Shared.IntegrationEvents.Users;

public sealed record UserRegisteredEvent : IntegrationEvent
{
    /// <summary>
    /// ID do usuário criado.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Email do usuário.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    public required string Name { get; init; }
}