using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Application.Repositories;

public interface ILogsRepository : IRepository<LogEntity>
{
    Task<IEnumerable<LogEntity>> GetLogBatchByEntityTypeAsync(string entityType, object continuationToken, int take, bool descending, CancellationToken cancellationToken);

    Task<IEnumerable<LogEntity>> GetLogBatchByEntityAsync(Guid entityId, object continuationToken, int take, bool descending, CancellationToken cancellationToken);
}