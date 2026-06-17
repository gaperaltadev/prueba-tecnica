using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Addresses.Commands;

public sealed record UpdateAddressCommand(int Id, string Street, string City, string Country, string? ZipCode) : IRequest<bool>;

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}

public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateAddressCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (address is null)
        {
            return false;
        }

        address.Street = request.Street;
        address.City = request.City;
        address.Country = request.Country;
        address.ZipCode = request.ZipCode;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
