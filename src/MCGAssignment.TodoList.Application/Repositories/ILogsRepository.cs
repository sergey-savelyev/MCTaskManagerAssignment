using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Application.Repositories;

public interface ILogsRepository : IRepository<LogEntity>
{
    Task<IEnumerable<LogEntity>> GetLogBatchByEntityTypeAsync<TEntity>(int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntity>> GetLogBatchByEntityAsync(Guid entityId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);
}