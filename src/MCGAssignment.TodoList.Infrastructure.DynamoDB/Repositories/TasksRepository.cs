using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MCGAssignment.TodoList.Application.Exceptions;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using MCGAssignment.TodoList.Infrastructure.DynamoDB.Extensions;

namespace MCGAssignment.TodoList.Infrastructure.DynamoDB.Repositories;

public class TasksRepository : ITasksRepository
{
    private readonly Table _table;
    private readonly IAmazonDynamoDB _dynamoDBClient;

    public TasksRepository(IAmazonDynamoDB dynamoDBClient)
    {
        _table = Table.LoadTable(dynamoDBClient, Constants.TasksTableName);
        _dynamoDBClient = dynamoDBClient;
    }

    public async Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken cancellationToken)
    {
        await _table.PutItemAsync(entity.ToDocument(), cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await GetByIdIndexAsync(id, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(id);
        }

        var subtasks = await GetSubtaskIdsAsync(id, cancellationToken);
        foreach (var subtaskId in subtasks)
        {
            await UpdateTaskRootAsync(subtaskId, null, cancellationToken);
        }

        await _table.DeleteItemAsync(item.RootTaskId?.ToString() ?? "null", id, cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetAllSubtaskIdsRecursivelyAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var ids = await GetSubtaskIdsAsync(taskId, cancellationToken);
        if (ids is null || !ids.Any())
        {
            return new List<Guid>();
        }

        foreach (var id in ids)
        {
            var subIds = await GetAllSubtaskIdsRecursivelyAsync(id, cancellationToken);
            ids = ids.Concat(subIds);
        }

        return ids;
    }

    public async Task<TaskEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetByIdIndexAsync(id, cancellationToken);

        // Not the best way to do this, costs 2 RCU, need to reconsider in future
        if (entity.RootTaskId is not null)
        {
            entity.RootTask = await GetByIdIndexAsync(entity.RootTaskId.Value, cancellationToken);
        }

        return entity;
    }

    public async Task<(IEnumerable<TaskEntity> Entities, object ContinuationToken)> GetRootTaskBatchAsync(int take, object continuationToken, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var sortIndexName = sortBy switch
        {
            "Summary" => Constants.TasksLocalSummaryIndexName,
            "CreateDate" => Constants.TasksLocalCreateDateIndexName,
            "DueDate" => Constants.TasksLocalDueDateIndexName,
            "Priority" => Constants.TasksLocalPriorityIndexName,
            "Status" => Constants.TasksLocalStatusIndexName,
            _ => throw new ArgumentException($"Invalid sort by value: {sortBy}")
        };
        
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            IndexName = sortIndexName,
            KeyConditionExpression = "RootTaskId = :null",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":null", new AttributeValue { S = "null" } }
            },
            ScanIndexForward = !descending,
            Limit = take
        };

        if (continuationToken is not null && continuationToken is Dictionary<string, AttributeValue> continuationTokenParsed)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToTaskEntity());

        return (entities, response.LastEvaluatedKey);
    }

    public async Task<IEnumerable<TaskEntity>> GetSubTasksAsync(Guid taskId, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            KeyConditionExpression = "RootTaskId = :rootTaskId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rootTaskId", new AttributeValue { S = taskId.ToString() } }
            }
        };

        var documents = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = documents.Items.Select(x => x.ToTaskEntity());

        return entities;
    }

    public async Task<(IEnumerable<TaskSearchEntity> Entities, object ContinuationToken)> SearchTasksAsync(string keyPhrase, int take, object continuationToken, CancellationToken cancellationToken)
    {
        var request = new ScanRequest
        {
            TableName = Constants.TasksTableName,
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#sm", "Summary" },
                { "#ds", "Description" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":keyPhrase", new AttributeValue { S = keyPhrase } },
            },
            FilterExpression = "contains(#sm, :keyPhrase) or contains(#ds, :keyPhrase)",
            ProjectionExpression = "Id, Summary, Description",
            Limit = take
        };

        if (continuationToken is not null && continuationToken is Dictionary<string, AttributeValue> continuationTokenParsed)
        {
            request.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.ScanAsync(request, cancellationToken);
        var entities = response.Items.Select(x => x.ToTaskSearchEntity());

        return (entities, response.LastEvaluatedKey);
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken)
    {
        var entityOld = await GetByIdIndexAsync(entity.Id, cancellationToken);
        if (entityOld is null)
        {
            throw new EntityNotFoundException(entity.Id);
        }

        var request = new UpdateItemRequest
        {
            AttributeUpdates = new()
            {
                ["Summary"] = new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { S = entity.Summary }
                },
                ["Description"] = new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { S = entity.Description }
                },
                ["Priority"] = new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { N = ((byte)entity.Priority).ToString() }
                },
                ["Status"] = new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { N = ((byte)entity.Status).ToString() }
                },
                ["DueDate"] = new AttributeValueUpdate
                {
                    Action = AttributeAction.PUT,
                    Value = new AttributeValue { S = entity.DueDate.ToString() }
                }
            },
            TableName = Constants.TasksTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = entity.Id.ToString() } },
                { "RootTaskId", new AttributeValue { S = entityOld.RootTaskId?.ToString() ?? "null" } }
            },
            ReturnValues = ReturnValue.NONE,
        };

        var response = await _dynamoDBClient.UpdateItemAsync(request, cancellationToken);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return entity;
        }

        throw new Exception("Failed to update task");
    }

    public async Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken)
    {
        var taskDocument = new Document();
        taskDocument["Id"] = taskId;
        taskDocument["RootTaskId"] = newRootId?.ToString() ?? "null";

        var item = await GetByIdIndexAsync(taskId, cancellationToken);
        if (item is null)
        {
            throw new EntityNotFoundException(taskId);
        }

        var oldRootTaskId = item.RootTaskId?.ToString() ?? "null";
        await _table.DeleteItemAsync(oldRootTaskId, taskId, cancellationToken);
        var newItem = new TaskEntity
        {
            Id = item.Id,
            RootTaskId = newRootId,
            Summary = item.Summary,
            Description = item.Description,
            CreateDate = item.CreateDate,
            DueDate = item.DueDate,
            Priority = item.Priority,
            Status = item.Status
        };

        await AddAsync(newItem, cancellationToken);
    }

    // Same here. I need only Id's, but AttributeToGet doesn't work with KeyConditionExpression
    // Reconsider index projections???
    private async Task<IEnumerable<Guid>> GetSubtaskIdsAsync(Guid taskId, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            KeyConditionExpression = "RootTaskId = :rootTaskId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rootTaskId", new AttributeValue { S = taskId.ToString() } }
            }
        };

        var documents = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var ids = documents.Items.Select(x => x["Id"].S).Select(Guid.Parse);

        return ids;
    }

    private async Task<TaskEntity> GetByIdIndexAsync(Guid id, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            IndexName = Constants.TasksGlobalIdIndexName,
            KeyConditionExpression = "Id = :id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":id", new AttributeValue { S = id.ToString() } }
            },
            Limit = 1
        };

        var documents = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entity = documents.Items.Select(x => x.ToTaskEntity()).FirstOrDefault() ?? throw new EntityNotFoundException(id);

        return entity;
    }
}