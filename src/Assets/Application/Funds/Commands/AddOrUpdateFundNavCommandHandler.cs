using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Funds.Commands;

public class AddOrUpdateFundNavCommandHandler(IFundRepository fundRepository) {

    public async Task Handle(AddOrUpdateFundNavCommand request, CancellationToken cancellationToken) {
        var isin = Isin.Create(request.Isin);
        var fund = await fundRepository.GetByIsinWithRecentNavsAsync(isin, request.NavDate, cancellationToken)
            ?? throw new NotFoundException("Fund", request.Isin);

        fund.AddOrUpdateNav(request.NavDate, request.NavValue);

        // On ne publie pas d'integration event FundNavUpdatedIntegrationEvent ici,
        // car il est publié par FundNavUpdatedDomainEventHandler.
    }
}
