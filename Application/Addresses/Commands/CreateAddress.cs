using MiPruebaTecnica.Domain;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Addresses.Commands;

public sealed record CreateAddressCommand(int UserId, string Street, string City, string Country, string? ZipCode) : IRequest<int>;

public sealed class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator(AppDbContext context)
    {
        RuleFor(x => x.UserId).GreaterThan(0)
            .MustAsync(async (userId, cancellation) => await context.Users.AnyAsync(u => u.Id == userId, cancellation))
            .WithMessage("User must exist.");

        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}

public sealed class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, int>
{
    private readonly AppDbContext _context;

    public CreateAddressCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new Address
        {
            UserId = request.UserId,
            Street = request.Street,
            City = request.City,
            Country = request.Country,
            ZipCode = request.ZipCode
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
        return address.Id;
    }
}
