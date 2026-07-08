using Hearth.Domain.Entities;

namespace Hearth.Application.Common.Interfaces;

public interface IHouseholdRepository : IRepository<Household>
{
    // Kod je jedinstven preko OBE kolone (adult + child) svih domaćinstava.
    Task<bool> JoinCodeExistsAsync(string code, CancellationToken cancellationToken = default);

    Task<Household?> GetByJoinCodeAsync(string code, CancellationToken cancellationToken = default);
}
