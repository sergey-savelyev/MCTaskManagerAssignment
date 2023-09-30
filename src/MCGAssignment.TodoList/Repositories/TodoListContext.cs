using MCGAssignment.TodoList.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MySql.EntityFrameworkCore.Extensions;

namespace MCGAssignment.TodoList.Repositories;

public class TodoListContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }

    public DbSet<LogEntity> Logs { get; set; }

    public TodoListContext(DbContextOptions<TodoListContext> options) : base(options)
    {
    }

    public async Task<IEnumerable<Guid>> GetAllSubtaskIdsRecursivelyAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var allSubtaskIds = await Database.SqlQueryRaw<Guid>(
            @$"with recursive cte (Id, RootTaskId) as (
                select     Id, 
                            RootTaskId 
                from       todolist.Task 
                where      RootTaskId = ""{taskId}""
                union all
                select     t.Id, 
                            t.RootTaskId 
                from       todolist.Task t 
                inner join cte
                        on t.RootTaskId = cte.Id 
            ) 
            select cte.Id from cte;"
        ).ToListAsync(cancellationToken);

        return allSubtaskIds;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Summary).IsRequired();
            entity.HasIndex(e => e.RootTaskId).IsUnique(false);

            // Optimizations for better tasks search
            entity.HasIndex(e => e.Summary).IsFullText();
            entity.HasIndex(e => e.Description).IsFullText();

            entity.HasOne(e => e.RootTask)
                .WithOne()
                .HasForeignKey<TaskEntity>(e => e.RootTaskId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.CreateDate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        });

        modelBuilder.Entity<LogEntity>(logEntity =>
        {
            logEntity.HasKey(e => e.Id);
            logEntity.HasIndex(e => e.EntityId);
        });
    }
}