using MiPruebaTecnica.Application.Common;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Currencies.CurrencyConversion;

public sealed record ConvertCurrencyQuery(string FromCurrencyCode, string ToCurrencyCode, decimal Amount) : IRequest<ConversionResultDto>;

public sealed class ConvertCurrencyQueryValidator : AbstractValidator<ConvertCurrencyQuery>
{
    public ConvertCurrencyQueryValidator()
    {
        RuleFor(x => x.FromCurrencyCode).NotEmpty();
        RuleFor(x => x.ToCurrencyCode).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class ConvertCurrencyQueryHandler : IRequestHandler<ConvertCurrencyQuery, ConversionResultDto>
{
    private readonly AppDbContext _context;

    public ConvertCurrencyQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConversionResultDto> Handle(ConvertCurrencyQuery request, CancellationToken cancellationToken)
    {
        var fromCurrency = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == request.FromCurrencyCode, cancellationToken);

        var toCurrency = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == request.ToCurrencyCode, cancellationToken);

        if (fromCurrency is null || toCurrency is null)
        {
            throw new InvalidOperationException("Currency not found.");
        }

        var amountInBase = request.Amount * fromCurrency.RateToBase;
        var convertedAmount = amountInBase / toCurrency.RateToBase;

        return new ConversionResultDto(
            FromCurrency: fromCurrency.Code,
            ToCurrency: toCurrency.Code,
            OriginalAmount: request.Amount,
            ConvertedAmount: convertedAmount);
    }
}
