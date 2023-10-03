using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MCGAssignment.TodoList.Core.Entities;

namespace MCGAssignment.TodoList.Infrastructure.DynamoDB;

public class DatabaseInitializer
{
    private IAmazonDynamoDB _dynamoDBClient;

    public DatabaseInitializer(IAmazonDynamoDB dynamoDBClient)
    {
        _dynamoDBClient = dynamoDBClient;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var tables = await _dynamoDBClient.ListTablesAsync(cancellationToken);
        if (tables.TableNames.Contains(Constants.TasksTableName) && tables.TableNames.Contains(Constants.LogsTableName))
        {
            return;
        }

        await CreateTasksTableAsync(cancellationToken);
        await CreateLogsTableAsync(cancellationToken);
    }

    private async Task CreateTasksTableAsync(CancellationToken cancellationToken)
    {
        var request = new CreateTableRequest
        {
            TableName = Constants.TasksTableName,
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            },
            AttributeDefinitions = new()
            {
                new(nameof(TaskEntity.RootTaskId), ScalarAttributeType.S),
                new(nameof(TaskEntity.Id), ScalarAttributeType.S),
                new(nameof(TaskEntity.Summary), ScalarAttributeType.S),
                new(nameof(TaskEntity.CreateDate), ScalarAttributeType.S),
                new(nameof(TaskEntity.DueDate), ScalarAttributeType.S),
                new(nameof(TaskEntity.Priority), ScalarAttributeType.N),
                new(nameof(TaskEntity.Status), ScalarAttributeType.N),
            },
            KeySchema = new()
            {
                new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                new(nameof(TaskEntity.Id), KeyType.RANGE)
            },
            LocalSecondaryIndexes = new()
            {
                new LocalSecondaryIndex
                {
                    IndexName = Constants.TasksLocalSummaryIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                        new(nameof(TaskEntity.Summary), KeyType.RANGE)
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
                new LocalSecondaryIndex
                {
                    IndexName = Constants.TasksLocalCreateDateIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                        new(nameof(TaskEntity.CreateDate), KeyType.RANGE)
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
                new LocalSecondaryIndex
                {
                    IndexName = Constants.TasksLocalDueDateIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                        new(nameof(TaskEntity.DueDate), KeyType.RANGE)
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
                new LocalSecondaryIndex
                {
                    IndexName = Constants.TasksLocalPriorityIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                        new(nameof(TaskEntity.Priority), KeyType.RANGE)
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
                new LocalSecondaryIndex
                {
                    IndexName = Constants.TasksLocalStatusIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.RootTaskId), KeyType.HASH),
                        new(nameof(TaskEntity.Status), KeyType.RANGE)
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
            },
            GlobalSecondaryIndexes = new()
            {
                new GlobalSecondaryIndex
                {
                    IndexName = Constants.TasksGlobalIdIndexName,
                    KeySchema = new()
                    {
                        new(nameof(TaskEntity.Id), KeyType.HASH)
                    },
                    ProvisionedThroughput = new()
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                }
            },
        };

        await _dynamoDBClient.CreateTableAsync(request, cancellationToken);
        await WaitUntilTableReady(Constants.TasksTableName);
    }

    private async Task CreateLogsTableAsync(CancellationToken cancellationToken)
    {
        var request = new CreateTableRequest
        {
            TableName = Constants.LogsTableName,
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            },
            AttributeDefinitions = new()
            {
                new(nameof(LogEntity.Id), ScalarAttributeType.S),
                new(nameof(LogEntity.TimestampMsec), ScalarAttributeType.N),
                new(nameof(LogEntity.EntityId), ScalarAttributeType.S),
                new(nameof(LogEntity.EntityType), ScalarAttributeType.S)
            },
            KeySchema = new()
            {
                new(nameof(LogEntity.Id), KeyType.HASH),
                new(nameof(LogEntity.TimestampMsec), KeyType.RANGE)
            },
            GlobalSecondaryIndexes = new()
            {
                new GlobalSecondaryIndex
                {
                    IndexName = Constants.LogsGlobalEntityIdIndexName,
                    KeySchema = new()
                    {
                        new(nameof(LogEntity.EntityId), KeyType.HASH),
                        new(nameof(LogEntity.TimestampMsec), KeyType.RANGE)
                    },
                    ProvisionedThroughput = new()
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                },
                new GlobalSecondaryIndex
                {
                    IndexName = Constants.LogsGlobalEntityTypeNameIndexName,
                    KeySchema = new()
                    {
                        new(nameof(LogEntity.EntityType), KeyType.HASH),
                        new(nameof(LogEntity.TimestampMsec), KeyType.RANGE)
                    },
                    ProvisionedThroughput = new()
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    Projection = new()
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                }
            }
        };

        await _dynamoDBClient.CreateTableAsync(request, cancellationToken);
        await WaitUntilTableReady(Constants.LogsTableName);
    }

    private async Task WaitUntilTableReady(string tableName)
    {
        string? status = null;
        // Let us wait until table is created. Call DescribeTable.
        do
        {
            Thread.Sleep(5000); // Wait 5 seconds.
            try
            {
                var response = await _dynamoDBClient.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });

                status = response.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                // DescribeTable is eventually consistent. So you might
                // get resource not found. So we handle the potential exception.
            }
        } while (status != "ACTIVE");
    }
}