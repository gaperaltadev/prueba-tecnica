using MiPruebaTecnica.Application.Users.Commands;
using MiPruebaTecnica.Application.Users.Queries;

namespace MiPruebaTecnica.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users", async (CreateUserCommand command, IMediator mediator, IValidator<CreateUserCommand> validator) =>
        {
            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            var id = await mediator.Send(command);
            return Results.Created($"/users/{id}", new { id });
        });

        app.MapGet("/users", async (bool? isActive, IMediator mediator) =>
        {
            var users = await mediator.Send(new GetAllUsersQuery(isActive));
            return Results.Ok(users);
        });

        app.MapGet("/users/{id:int}", async (int id, IMediator mediator) =>
        {
            var user = await mediator.Send(new GetUserByIdQuery(id));
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        app.MapPut("/users/{id:int}", async (int id, UpdateUserRequest request, IMediator mediator, IValidator<UpdateUserCommand> validator) =>
        {
            var command = new UpdateUserCommand(id, request.Name, request.Email, request.IsActive);

            var validation = await validator.ValidateAsync(command);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            var updated = await mediator.Send(command);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        app.MapDelete("/users/{id:int}", async (int id, IMediator mediator) =>
        {
            var deleted = await mediator.Send(new DeleteUserCommand(id));
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}