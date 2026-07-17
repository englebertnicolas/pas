namespace PAS.Assets.Domain.FundAggregate;

public class FundDomainService(/*IFundRepository fundRepository*/) {
    // Add domain service methods here
}

// Sample implementation of a domain service rule (that would be called by the application layer)
//public class FundDomainService(IFundRepository fundRepository, ICurrencyRepository currencyRepository) {
//    public async Task VerifyFundRiskExposureAsync(Fund fund, string currencyCode) {
//        var currency = await currencyRepository.GetCurrencyDetailsAsync(currencyCode);
//        if (currency.IsHighVolatility && fund.IsConservative()){
//            throw new DomainException("Conservative funds cannot be exposed to high-volatility currencies.");
//        }
//    }
//}
