using FluentValidation;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using DomainIsin = PAS.Assets.Domain.FundAggregate.Isin;

namespace PAS.Assets.Application.Commands;

public record CreateCollectiveFundCommand(
    string Name,
    string Isin,
    string Currency,
    CreateCollectiveFundCommand.NavInput? Nav = null
) {
    public record NavInput(DateTime Date, double Value);
    public record Result(long Id);

    // Command validator (auto discovered by Wolverine)
    public class Validator : AbstractValidator<CreateCollectiveFundCommand> {
        public Validator(IFundRepository fundRepository, ICurrencyRepository currencyRepository) {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (name, ct) => !await fundRepository.NameExistsAsync(name, ct))
                .WithMessage("Fund name already in use");

            RuleFor(x => x.Isin)
                .NotEmpty()
                .MustAsync(async (isin, ct) => !await fundRepository.IsinExistsAsync(DomainIsin.Create(isin), ct))
                .WithMessage("Fund ISIN already in use");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3)
                .MustAsync((id, ct) => currencyRepository.ExistsAsync(CurrencyId.Create(id), ct))
                .WithMessage(x => $"Currency '{x.Currency}' not found");
        }
    }

    // Command handler (auto discovered by Wolverine)
    public static class Handler {
        public static Result Handle(CreateCollectiveFundCommand request, IFundRepository fundRepository) {
            var isin = DomainIsin.Create(request.Isin);
            var currencyId = CurrencyId.Create(request.Currency);

            var navs = (IEnumerable<FundNav>?)null;
            if (request.Nav != null)
                navs = [FundNav.Create(request.Nav.Date, request.Nav.Value)];

            var fund = Fund.CreateCollectiveFund(FundStatus.Active, request.Name, isin, currencyId, navs);

            fundRepository.Add(fund);
            return new(fund.Id);
        }
    }
}
