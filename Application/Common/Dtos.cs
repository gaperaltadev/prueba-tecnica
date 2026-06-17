namespace MiPruebaTecnica.Application.Common;

public sealed record UserDto(int Id, string Name, string Email, bool IsActive);

public sealed record AddressDto(int Id, int UserId, string Street, string City, string Country, string? ZipCode);

public sealed record CurrencyDto(int Id, string Code, string Name, decimal RateToBase);

public sealed record ConversionResultDto(string FromCurrency, string ToCurrency, decimal OriginalAmount, decimal ConvertedAmount);
