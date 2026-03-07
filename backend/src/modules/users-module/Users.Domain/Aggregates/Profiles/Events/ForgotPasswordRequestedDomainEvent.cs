using Shared.Domain.Events;

namespace Users.Domain.Aggregates.Profiles.Events;

public class ForgotPasswordRequestedDomainEvent : DomainEventBase
{
    public override string AggregateType => nameof(Profile);
    public override Guid AggregateId { get; }
    public override string Module => "users";

    public Guid UserId { get; }
    public string Email { get; }
    public string ResetToken { get; }
    public string ResetCode { get; }
    public DateTime ExpirationDate { get; }

    public ForgotPasswordRequestedDomainEvent(Guid aggregateId, Guid userId, string email, string resetToken, string resetCode, DateTime expirationDate)
    {
        AggregateId = aggregateId;
        UserId = userId;
        Email = email;
        ResetToken = resetToken;
        ResetCode = resetCode;
        ExpirationDate = expirationDate;
    }
}
