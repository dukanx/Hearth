using System.Linq.Expressions;
using Hearth.Domain.Common;

namespace Hearth.Application.Common.Interfaces;

// Generička apstrakcija nad pristupom podacima za jedan agregat.
// Vraća domenske entitete; commit se radi kroz IUnitOfWork.SaveChangesAsync.
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    void Add(T entity);

    void Update(T entity);

    void Remove(T entity);
}
