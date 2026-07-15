using FluentValidation;

namespace PAS.Assets.Application.Queries;

public class FundQueryValidator : AbstractValidator<FundQuery> {

    public FundQueryValidator() {
        RuleFor(x => x)
            .Must(x => (x.Id != null ? 1 : 0)
                     + (!string.IsNullOrEmpty(x.Name) ? 1 : 0)
                     + (!string.IsNullOrEmpty(x.Isin) ? 1 : 0) == 1)
            .WithMessage("One and only one of the following must be supplied: Id, Name, or Isin.");

        RuleFor(x => x.Isin)
            .Length(12)
            .When(x => !string.IsNullOrEmpty(x.Isin));
    }
}
