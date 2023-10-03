using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Application.Repositories;

public interface ILogsRepository : IRepository<LogEntity>
{
    Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityTypeAsync(string entityType, object continuationToken, int take, bool descending, CancellationToken cancellationToken);

    Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityAsync(Guid entityId, object continuationToken, int take, bool descending, CancellationToken cancellationToken);
}