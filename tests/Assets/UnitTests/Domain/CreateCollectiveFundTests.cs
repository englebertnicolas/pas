using FluentAssertions;
using PAS.Assets.Domain.CurrencyAggregate;
using PAS.Assets.Domain.FundAggregate;
using PAS.Common.Domain;

namespace PAS.Assets.UnitTests.Domain;

public class CreateCollectiveFundTests {

    [Fact]
    public void Create_WithCorrectData() {
        // Arrange
        var name = "Global Equity Fund";
        var isin = "BE1234567890";
        var currency = "EUR";

        // Act
        var fund = Fund.CreateCollective(FundStatus.Active, name, Isin.Create(isin), CurrencyId.Create(currency));

        // Assert
        fund.Name.Should().Be(name);
        fund.Isin.Value.Should().Be(isin);
        fund.CurrencyId.Value.Should().Be(currency);
    }

    [Fact]
    public void Create_WithInvalidIsin_ShouldThrowException() {
        // Arrange
        var invalidIsin = "BE123456789";

        // Act
        var act = () => Isin.Create(invalidIsin);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*must be exactly 12 characters long*");
    }
}
