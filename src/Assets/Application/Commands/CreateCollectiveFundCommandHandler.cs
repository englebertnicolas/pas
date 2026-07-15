using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Commands;

public class CreateCollectiveFundCommandHandler(IFundRepository fundRepository) {

    public async Task<CreateCollectiveFundCommandResult> Handle(CreateCollectiveFundCommand request) {
        var isin = Isin.Create(request.Isin);
        var currencyId = CurrencyId.Create(request.Currency);

        var navs = (IEnumerable<FundNav>?)null;
        if (request.Nav != null)
            navs = [FundNav.Create(request.Nav.Date, request.Nav.Value)];

        var fund = Fund.CreateCollective(FundStatus.Active, request.Name, isin, currencyId, navs);
        fundRepository.Add(fund);

        return new CreateCollectiveFundCommandResult(fund.Id);
    }
}

public record CreateCollectiveFundCommandResult(long Id);
