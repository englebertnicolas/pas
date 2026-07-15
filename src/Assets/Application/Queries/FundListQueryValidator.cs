using FluentValidation;

namespace PAS.Assets.Application.Queries;

public class FundListQueryValidator : AbstractValidator<FundListQuery> {

    public FundListQueryValidator() {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);
    }
}
