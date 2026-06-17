using MiPruebaTecnica.Application.Currencies.Commands;
using MiPruebaTecnica.Application.Currencies.CurrencyConversion;
using MiPruebaTecnica.Application.Common;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Endpoints;

public static class CurrencyEndpoints
{
    public static void MapCurrencyEndpoints(this WebApplication app)
    {
        app.MapGet("/currencies", async (AppDbContext context) =>
        {
            var currencies = await context.Currencies
                .AsNoTracking()
                .Select(c => new CurrencyDto(c.Id, c.Code, c.Name, c.RateToBase))
                .ToListAsync();

            return Results.Ok(currencies);
        });

        app.MapPost("/currencies", async (CreateCurrencyCommand command, IMediator mediator, IValidator<CreateCurrencyCommand> validator) =>
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            var id = await mediator.Send(command);
            return Results.Created($"/currencies/{id}", new { id });
        });

        app.MapPost("/currency/convert", async (ConvertCurrencyQuery query, IMediator mediator, IValidator<ConvertCurrencyQuery> validator) =>
        {
            var validation = await validator.ValidateAsync(query);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            try
            {
                var conversion = await mediator.Send(query);
                return Results.Ok(conversion);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        });
    }
}
