using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Domain.Events;

public record FundStatusChangedToClosedDomainEvent(
    Fund Fund
) : IDomainEvent;
