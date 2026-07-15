using FluentValidation;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Commands;

public class CreateCollectiveFundCommandValidator : AbstractValidator<CreateCollectiveFundCommand> {
    private readonly IFundRepository fundRepository;
    private readonly ICurrencyRepository currencyRepository;

    public CreateCollectiveFundCommandValidator(IFundRepository fundRepository, ICurrencyRepository currencyRepository) {
        this.fundRepository = fundRepository;
        this.currencyRepository = currencyRepository;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MustAsync(BeAnUniqueNameAsync).WithMessage("Fund name already in use");

        RuleFor(x => x.Isin)
            .NotEmpty()
            .MustAsync(BeAnUniqueIsinAsync).WithMessage("Fund ISIN already in use");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .MustAsync(CurrencyExistsAsync).WithMessage(x => $"Currency '{x.Currency}' not found");
    }

    private async Task<bool> BeAnUniqueNameAsync(string name, CancellationToken cancellationToken) {
        return !await fundRepository.NameExistsAsync(name, cancellationToken);
    }

    private async Task<bool> BeAnUniqueIsinAsync(string isin, CancellationToken cancellationToken) {
        return !await fundRepository.IsinExistsAsync(Isin.Create(isin), cancellationToken);
    }

    private async Task<bool> CurrencyExistsAsync(string currencyId, CancellationToken cancellationToken) {
        return await currencyRepository.ExistsAsync(CurrencyId.Create(currencyId), cancellationToken);
    }
}
