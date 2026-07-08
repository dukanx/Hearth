using System.Linq.Expressions;
using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Hearth.Infrastructure.Persistence.Repositories;

// Generička EF Core implementacija. Deli AppDbContext (scoped) sa UnitOfWork-om,
// pa Add/Update/Remove samo menjaju change-tracker; upis radi SaveChangesAsync.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;

    protected DbSet<T> Set => Context.Set<T>();

    public Repository(AppDbContext context) => Context = context;

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await Set.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await Set.Where(predicate).ToListAsync(cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await Set.AnyAsync(predicate, cancellationToken);

    public void Add(T entity) => Set.Add(entity);

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);
}
