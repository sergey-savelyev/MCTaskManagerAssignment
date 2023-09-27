using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace MCGAssignment.TodoList.Models;

public class TodoListContext : DbContext
{
    public DbSet<TaskEntity> Task { get; set; }

    public DbSet<LogEntity> LogEntry { get; set; }

    public TodoListContext(DbContextOptions<TodoListContext> options) : base(options)
    {
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

            entity.Property(e => e.CreateDate).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Ignore);
        });

        modelBuilder.Entity<LogEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.EntityType);
        });
    }
}