using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Infrastructure.Persistence.Repositories;

public sealed class HouseholdRepository : Repository<Household>, IHouseholdRepository
{
    public HouseholdRepository(AppDbContext context) : base(context) { }

    public Task<bool> JoinCodeExistsAsync(string code, CancellationToken cancellationToken = default)
        => Set.AnyAsync(h => h.AdultJoinCode == code || h.ChildJoinCode == code, cancellationToken);

    public async Task<Household?> GetByJoinCodeAsync(string code, CancellationToken cancellationToken = default)
        => await Set.FirstOrDefaultAsync(h => h.AdultJoinCode == code || h.ChildJoinCode == code, cancellationToken);
}
