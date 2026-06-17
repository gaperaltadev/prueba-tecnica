using MiPruebaTecnica.Application.Addresses.Commands;
using MiPruebaTecnica.Application.Addresses.Queries;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this WebApplication app)
    {
        app.MapPost("/users/{userId}/addresses", async (int userId, CreateAddressCommand command, IMediator mediator, IValidator<CreateAddressCommand> validator, AppDbContext context) =>
        {
            if (userId != command.UserId)
            {
                return Results.BadRequest();
            }

            if (!await context.Users.AnyAsync(u => u.Id == userId))
            {
                return Results.NotFound();
            }

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            var id = await mediator.Send(command);
            return Results.Created($"/users/{userId}/addresses/{id}", new { id });
        });

        app.MapGet("/users/{userId}/addresses", async (int userId, IMediator mediator, AppDbContext context) =>
        {
            if (!await context.Users.AnyAsync(u => u.Id == userId))
            {
                return Results.NotFound();
            }

            var addresses = await mediator.Send(new GetAddressesByUserQuery(userId));
            return Results.Ok(addresses);
        });

        app.MapPut("/addresses/{id}", async (int id, UpdateAddressCommand command, IMediator mediator, IValidator<UpdateAddressCommand> validator) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            var updated = await mediator.Send(command);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        app.MapDelete("/addresses/{id}", async (int id, IMediator mediator) =>
        {
            var deleted = await mediator.Send(new DeleteAddressCommand(id));
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}