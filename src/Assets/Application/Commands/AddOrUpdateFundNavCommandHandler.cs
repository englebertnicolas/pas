using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Commands;

public class AddOrUpdateFundNavCommandHandler(IFundRepository fundRepository) {

    public async Task Handle(AddOrUpdateFundNavCommand request, CancellationToken cancellationToken) {
        var fund = await fundRepository.GetByIdWithRecentNavsAsync(request.FundId, request.NavDate, cancellationToken)
            ?? throw new NotFoundException("Fund", request.FundId);

        fund.AddOrUpdateNav(request.NavDate, request.NavValue);

        // On ne publie pas d'integration event FundNavUpdatedIntegrationEvent ici,
        // car il est publié par FundNavUpdatedDomainEventHandler.
    }
}
