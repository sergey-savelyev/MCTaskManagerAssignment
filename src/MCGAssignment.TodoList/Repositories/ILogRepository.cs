using MCGAssignment.TodoList.Models;

namespace MCGAssignment.TodoList.Repositories;

public interface ILogRepository
{
    Task<LogEntity> GetLogEntryAsync(string entryId, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntity>> GetLogEntryBatchAsync<TEntity>(int skip, int take, string orderby, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntity>> GetLogEntryBatchByEntityIdAsync(string entityId, int skip, int take, string orderBy, bool descending, CancellationToken cancellationToken);

    Task CreateLogEntryAsync(LogEntity entity, CancellationToken cancellationToken);
}