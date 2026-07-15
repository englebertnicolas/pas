using PAS.Assets.Contracts.Events;

namespace PAS.Policies.Application.Funds.EventHandlers;

public static class FundNavUpdatedIntegrationEventHandler {

    public static Task HandleAsync(FundNavUpdatedIntegrationEvent integrationEvent) {
        //TODO: Voir si l'ajout d'une VNI nécessite un recalcul de certaines polices...

        return Task.CompletedTask;
    }
}
