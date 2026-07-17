using NSubstitute;
using PAS.Assets.Application.Commands;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.UnitTests.Application;

public class CreateFundCommandTests {
    private readonly IFundRepository fundRepositoryMock;

    public CreateFundCommandTests() {
        fundRepositoryMock = Substitute.For<IFundRepository>();
    }

    [Fact]
    public void Handle_WithCorrectData() {
        // Arrange
        var name = "Global Equity Fund";
        var isin = "BE1234567890";
        var currency = "EUR";

        // Act
        CreateCollectiveFundCommand.Handler.Handle(new(name, isin, currency), fundRepositoryMock);

        // Assert
        fundRepositoryMock.Received(1).Add(
            Arg.Is<Fund>(f =>
                f.Name == name &&
                f.Isin.Value == isin &&
                f.CurrencyId.Value == currency &&
                f.Status == FundStatus.Active
            )
        );
    }
}
