using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Application.Services;
using MCGAssignment.TodoList.Infrastructure.MySQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCGAssignment.TodoList.Infrastructure;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskActionLogService, TaskActionLogService>();

        services.AddDbContext<ApplicationDbContext>(options => 
        {
            options.UseLazyLoadingProxies().UseMySQL(configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Failed to initialize: connection string cannot be null"));
        });

        services.AddScoped<ITasksRepository, TasksRepository>();
        services.AddScoped<ILogsRepository, LogsRepository>();

        return services;
    }

    public static void EnsureCreated(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
}