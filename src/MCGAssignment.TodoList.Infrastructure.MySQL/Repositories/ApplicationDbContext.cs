using MCGAssignment.TodoList.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MySql.EntityFrameworkCore.Extensions;

namespace MCGAssignment.TodoList.Infrastructure.MySQL.Repositories;

public class ApplicationDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }

    public DbSet<LogEntity> Logs { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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

            entity.HasOne(e => e)
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