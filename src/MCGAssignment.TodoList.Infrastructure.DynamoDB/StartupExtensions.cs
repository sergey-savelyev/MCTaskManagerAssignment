using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Application.Services;
using MCGAssignment.TodoList.Infrastructure.DynamoDB;
using MCGAssignment.TodoList.Infrastructure.DynamoDB.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MCGAssignment.TodoList.Infrastructure;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<ITaskActionLogService, TaskActionLogService>();
        Console.WriteLine("DynamoDB:ServiceURL: " + configuration["DynamoDB:ServiceURL"]);
        Console.WriteLine("DynamoDB:RegionEndpoint: " + configuration["DynamoDB:Region"]);
        services.AddSingleton<IAmazonDynamoDB>(sp =>
        {
            var clientConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            };

            return new AmazonDynamoDBClient(clientConfig);
        });

        services.AddSingleton<ITasksRepository, TasksRepository>();
        services.AddSingleton<ILogsRepository, LogsRepository>();

        return services;
    }

    public static void EnsureCreated(this IServiceProvider services)
    {
        var dynamoDBClient = services.GetRequiredService<IAmazonDynamoDB>();
        var databaseInitializer = new DatabaseInitializer(dynamoDBClient);
        databaseInitializer.InitializeAsync(default).Wait();
    }
}