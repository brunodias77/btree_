using Shared.Domain.Abstractions;

namespace Users.Domain.Aggregates.Tokens;

public class EmailConfirmationToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string IdentityToken { get; private set; } = null!;
    public string ShortCode { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    private EmailConfirmationToken() : base() { }

    public EmailConfirmationToken(Guid userId, string identityToken, string shortCode, DateTime expiresAt) 
        : base(Guid.NewGuid())
    {
        UserId = userId;
        IdentityToken = identityToken;
        ShortCode = shortCode;
        ExpiresAt = expiresAt;
        IsUsed = false;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }
}
