using NSubstitute;
using PAS.Assets.Application.Funds.Commands;
using PAS.Assets.Domain.FundAggregate;

namespace PAS.Assets.UnitTests.Application;

public class CreateCollectiveFundCommandTests {
    private readonly IFundRepository fundRepositoryMock;
    private readonly CreateCollectiveFundCommandHandler handler;

    public CreateCollectiveFundCommandTests() {
        fundRepositoryMock = Substitute.For<IFundRepository>();
        handler = new CreateCollectiveFundCommandHandler(fundRepositoryMock);
    }

    [Fact]
    public async Task Handle_WithCorrectData() {
        // Arrange
        var name = "Global Equity Fund";
        var isin = "BE1234567890";
        var currency = "EUR";

        // Act
        await handler.Handle(new(name, isin, currency));

        // Assert
        fundRepositoryMock.Received(1).Add(
            Arg.Is<Fund>(f =>
                f.Name == name &&
                f.Isin.Value == isin &&
                f.Currency.Code == currency &&
                f.Status == FundStatus.Active
            )
        );
    }
}
