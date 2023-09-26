using Microsoft.EntityFrameworkCore;

namespace MCTaskManagerAssignment.Models;

public class TodoListContext : DbContext
{
    public DbSet<TaskEntity> Task { get; set; }

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

            entity.HasOne(e => e.RootTask)
                .WithOne()
                .HasForeignKey<TaskEntity>(e => e.RootTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}