using PAS.Assets.Domain.CurrencyAggregate;

namespace PAS.Assets.Application.Commands;

public class CreateCurrencyCommandHandler(ICurrencyRepository currencyRepository) {

    public async Task<CreateCurrencyCommandResult> Handle(CreateCurrencyCommand request) {
        var id = CurrencyId.Create(request.Id);
        var symbol = CurrencySymbol.CreateOrNull(request.Symbol);
        var currency = Currency.Create(id, request.EnglishName, symbol);
        currencyRepository.Add(currency);

        return new CreateCurrencyCommandResult(id.Value);
    }
}

public record CreateCurrencyCommandResult(string Id);
