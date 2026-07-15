using FluentValidation;

namespace PAS.Assets.Application.Queries;

public class CurrencyListQueryValidator : AbstractValidator<CurrencyListQuery> {

    public CurrencyListQueryValidator() {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);
    }
}
