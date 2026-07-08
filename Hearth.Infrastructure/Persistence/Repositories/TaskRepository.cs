using Hearth.Application.Common.Interfaces;
using Hearth.Domain.Entities;

namespace Hearth.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository : Repository<HouseholdTask>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }
}
