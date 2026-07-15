using PAS.Assets.Contracts.Events;
using PAS.Assets.Domain.Events;

namespace PAS.Assets.Application.Events;

public static class FundNavUpdatedDomainEventHandler {

    public static FundNavUpdatedIntegrationEvent Handle(FundNavUpdatedDomainEvent domainEvent) {
        return new FundNavUpdatedIntegrationEvent(
           domainEvent.FundIsin,
           domainEvent.FundCurrency,
           domainEvent.NavDate,
           domainEvent.NavOldValue,
           domainEvent.NavNewValue
       );
    }
}
