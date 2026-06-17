using MiPruebaTecnica.Domain;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Currencies.Commands;

public sealed record CreateCurrencyCommand(string Code, string Name, decimal RateToBase) : IRequest<int>;

public sealed class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator(AppDbContext context)
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.RateToBase).GreaterThan(0);
        RuleFor(x => x.Code)
            .MustAsync(async (code, cancellation) => !await context.Currencies.AnyAsync(c => c.Code == code, cancellation))
            .WithMessage("Currency code must be unique.");
    }
}

public sealed class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, int>
{
    private readonly AppDbContext _context;

    public CreateCurrencyCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency
        {
            Code = request.Code,
            Name = request.Name,
            RateToBase = request.RateToBase
        };

        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync(cancellationToken);
        return currency.Id;
    }
}
