using FluentValidation;
using PAS.Assets.Domain.CurrencyAggregate;

namespace PAS.Assets.Application.Commands;

public record CreateCurrencyCommand(
    string Id,
    string EnglishName,
    string? Symbol = null
) {
    public record Result(string Id);

    // Command validator (auto discovered by Wolverine)
    public class Validator : AbstractValidator<CreateCurrencyCommand> {
        public Validator(ICurrencyRepository currencyRepository) {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.EnglishName)
                .NotEmpty();

            RuleFor(x => x.Id)
               .NotEmpty()
               .MustAsync(async (id, ct) => !await currencyRepository.ExistsAsync(CurrencyId.Create(id), ct))
               .WithMessage("Currency identifier already in use");
        }
    }

    // Command handler (auto discovered by Wolverine)
    public static class Handler {
        public static Result Handle(CreateCurrencyCommand request, ICurrencyRepository currencyRepository) {
            var id = CurrencyId.Create(request.Id);
            var symbol = CurrencySymbol.CreateOrNull(request.Symbol);
            var currency = Currency.Create(id, request.EnglishName, symbol);

            currencyRepository.Add(currency);
            return new(currency.Id.Value);
        }
    }
}
