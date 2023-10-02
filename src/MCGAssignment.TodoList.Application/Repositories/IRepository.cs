namespace MCGAssignment.TodoList.Application.Repositories;

public interface IRepository<TEntity>
{
    Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> DeleteAsync(Guid id, CancellationToken cancellationToken);
}