using FluentValidation;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Commands;

public record AddOrUpdateFundNavCommand(
    long FundId,
    DateTime NavDate,
    double NavValue
) {
    // Command validator (auto discovered by Wolverine)
    public class Validator : AbstractValidator<AddOrUpdateFundNavCommand> {
        public Validator() {
            RuleFor(x => x.FundId)
                .GreaterThan(0);
        }
    }

    // Command handler (auto discovered by Wolverine)
    public static class Handler {
        public static async Task HandleAsync(AddOrUpdateFundNavCommand request, IFundRepository fundRepository, CancellationToken ct) {
            var fund = await fundRepository.GetByIdWithRecentNavsAsync(request.FundId, request.NavDate, ct)
                ?? throw new NotFoundException("Fund", request.FundId);

            fund.AddOrUpdateNav(request.NavDate, request.NavValue);

            // We do not publish integration event FundNavUpdatedIntegrationEvent here,
            // because it is published by FundNavUpdatedDomainEventHandler.
        }
    }
}
