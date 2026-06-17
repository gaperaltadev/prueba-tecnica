using MiPruebaTecnica.Application.Common;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Addresses.Queries;

public sealed record GetAddressesByUserQuery(int UserId) : IRequest<List<AddressDto>>;

public sealed class GetAddressesByUserQueryValidator : AbstractValidator<GetAddressesByUserQuery>
{
    public GetAddressesByUserQueryValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public sealed class GetAddressesByUserQueryHandler : IRequestHandler<GetAddressesByUserQuery, List<AddressDto>>
{
    private readonly AppDbContext _context;

    public GetAddressesByUserQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AddressDto>> Handle(GetAddressesByUserQuery request, CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .Select(a => new AddressDto(a.Id, a.UserId, a.Street, a.City, a.Country, a.ZipCode))
            .ToListAsync(cancellationToken);
    }
}
