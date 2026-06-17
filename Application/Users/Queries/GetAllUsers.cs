using MiPruebaTecnica.Application.Common;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Users.Queries;

public sealed record GetAllUsersQuery(bool? IsActive) : IRequest<List<UserDto>>;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly AppDbContext _context;

    public GetAllUsersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        return await query
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive))
            .ToListAsync(cancellationToken);
    }
}
