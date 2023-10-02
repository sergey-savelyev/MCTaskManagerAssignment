using Amazon.DynamoDBv2;
using MCGAssignment.TodoList.Application.Services;
using MCGAssignment.TodoList.Infrastructure.DynamoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCGAssignment.TodoList.Infrastructure;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<ITaskActionLogService, TaskActionLogService>();
        services.AddAWSService<IAmazonDynamoDB>();

        return services;
    }

    public static void EnsureCreated(this IServiceProvider services)
    {
        var dynamoDBClient = services.GetRequiredService<IAmazonDynamoDB>();
        var databaseInitializer = new DatabaseInitializer(dynamoDBClient);
        databaseInitializer.InitializeAsync(default).Wait();
    }
}