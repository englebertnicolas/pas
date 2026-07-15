using FluentValidation;

namespace PAS.Assets.Application.Funds.Commands;

public class AddOrUpdateFundNavCommandValidator : AbstractValidator<AddOrUpdateFundNavCommand> {

    public AddOrUpdateFundNavCommandValidator() {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.NavDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Fund NAV date cannot be in the future.");

        RuleFor(x => x.NavValue)
            .GreaterThan(0).WithMessage("Fund NAV should be greater than zero.");
    }
}
