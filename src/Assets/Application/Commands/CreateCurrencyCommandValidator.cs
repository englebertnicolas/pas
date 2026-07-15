using FluentValidation;
using PAS.Assets.Domain.CurrencyAggregate;

namespace PAS.Assets.Application.Commands;

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand> {
    private readonly ICurrencyRepository currencyRepository;

    public CreateCurrencyCommandValidator(ICurrencyRepository currencyRepository) {
        this.currencyRepository = currencyRepository;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(BeAnUniqueIdAsync).WithMessage("Currency identifier already in use");
    }

    private async Task<bool> BeAnUniqueIdAsync(string id, CancellationToken cancellationToken) {
        return !await currencyRepository.ExistsAsync(CurrencyId.Create(id), cancellationToken);
    }
}
