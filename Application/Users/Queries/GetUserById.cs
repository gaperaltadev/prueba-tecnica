using MiPruebaTecnica.Application.Common;
using MiPruebaTecnica.Infrastructure.Data;

namespace MiPruebaTecnica.Application.Users.Queries;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly AppDbContext _context;

    public GetUserByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
