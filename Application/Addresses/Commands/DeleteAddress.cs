using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Addresses.Commands;

public sealed record DeleteAddressCommand(int Id) : IRequest<bool>;

public sealed class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteAddressCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FindAsync(new object[] { request.Id }, cancellationToken);
        if (address is null)
        {
            return false;
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
