namespace PAS.Assets.Domain.FundAggregate;

public record Currency : ValueObject {
    public string Code { get; private set; }

    private Currency(string code) {
        Code = code;
    }

    public static Currency Create(string code) {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Invalid currency code.", "Currency");

        var cleanedCode = code.Trim().ToUpper();
        if (cleanedCode.Length != 3)
            throw new DomainException("Currency code must be exactly 3 characters long.", "Currency");

        return new Currency(cleanedCode);
    }
}
