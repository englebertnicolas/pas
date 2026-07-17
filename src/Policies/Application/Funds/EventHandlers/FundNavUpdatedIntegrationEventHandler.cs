using Microsoft.Extensions.Logging;
using PAS.Assets.Contracts.Events;

namespace PAS.Policies.Application.Funds.EventHandlers;

public static class FundNavUpdatedIntegrationEventHandler {

    public static Task HandleAsync(FundNavUpdatedIntegrationEvent integrationEvent, ILogger<FundNavUpdatedIntegrationEvent> logger) {
        logger.LogInformation("Received FundNavUpdatedIntegrationEvent for FundId: {FundId}, NavDate: {NavDate}, NavOldValue: {NavOldValue}, NavNewValue: {NavNewValue}",
            integrationEvent.FundId, integrationEvent.NavDate, integrationEvent.NavOldValue, integrationEvent.NavNewValue);

        return Task.CompletedTask;
    }
}
