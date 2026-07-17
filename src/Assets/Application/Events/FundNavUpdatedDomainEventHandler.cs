using PAS.Assets.Contracts.Events;
using PAS.Assets.Domain.Events;

namespace PAS.Assets.Application.Events;

public static class FundNavUpdatedDomainEventHandler {

    public static FundNavUpdatedIntegrationEvent Handle(FundNavUpdatedDomainEvent domainEvent) {
        // Raise an integration event to notify other services about the NAV update
        return new FundNavUpdatedIntegrationEvent(
           domainEvent.FundId,
           domainEvent.FundIsin,
           domainEvent.FundCurrency,
           domainEvent.NavDate,
           domainEvent.NavOldValue,
           domainEvent.NavNewValue
       );
    }
}
