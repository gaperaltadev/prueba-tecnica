using System.Security.Cryptography;
using System.Text;
using MiPruebaTecnica.Infrastructure.Data;
using MiPruebaTecnica.Domain;

namespace MiPruebaTecnica.Application.Users.Commands;

public sealed record CreateUserCommand(string Name, string Email, string Password) : IRequest<int>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(AppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .Matches(@"^[^@\s]+@[^@\s\.]+\.[^@\s\.]+$").WithMessage("El formato del email no es válido.")
            .MustAsync(async (email, cancellation) => !await context.Users.AnyAsync(u => u.Email == email, cancellation))
            .WithMessage("El email ya se encuentra registrado.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.");
    }
}

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly AppDbContext _context;

    public CreateUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = HashPassword(request.Password),
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}