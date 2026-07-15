using FluentValidation;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.Application.Funds.Commands;

public class CreateCollectiveFundCommandValidator : AbstractValidator<CreateCollectiveFundCommand> {
    private readonly IFundRepository fundRepository;

    public CreateCollectiveFundCommandValidator(IFundRepository fundRepository) {
        this.fundRepository = fundRepository;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MustAsync(BeAnUniqueName).WithMessage("Fund name already in use");

        RuleFor(x => x.Isin)
            .NotEmpty()
            .MustAsync(BeAnUniqueIsin).WithMessage("Fund ISIN already in use");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);
    }

    private async Task<bool> BeAnUniqueName(string name, CancellationToken cancellationToken) {
        return !await fundRepository.NameExistsAsync(name, cancellationToken);
    }

    private async Task<bool> BeAnUniqueIsin(string isin, CancellationToken cancellationToken) {
        return !await fundRepository.IsinExistsAsync(Isin.Create(isin), cancellationToken);
    }
}
