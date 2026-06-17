using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Users.Commands;

// 1. EL REQUISITO PÚBLICO: Name y Email ahora aceptan null (? ) en el JSON de Swagger
public sealed record UpdateUserRequest(string? Name, string? Email, bool IsActive);

// El Command interno que procesa la operación
public sealed record UpdateUserCommand(int Id, string? Name, string? Email, bool IsActive) : IRequest<bool>;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(AppDbContext context)
    {
        RuleFor(x => x.Email)
            .Matches(@"^[^@\s]+@[^@\s\.]+\.[^@\s\.]+$").WithMessage("El formato del email no es válido.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Email)
            .MustAsync(async (request, email, cancellation) =>
                !await context.Users.AnyAsync(u => u.Email == email && u.Id != request.Id, cancellation))
            .WithMessage("El email ya se encuentra registrado por otro usuario.")
            .When(x => !string.IsNullOrEmpty(x.Email));
            

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede estar vacío si se incluye en la petición.")
            .When(x => x.Name != null);
    }
}

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user is null)
        {
            return false;
        }

        user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        
        user.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}