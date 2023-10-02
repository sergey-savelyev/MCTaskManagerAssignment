using System.Text.Json;
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
        await _table.DeleteItemAsync(id, cancellationToken);
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
        var entity = documents.Items.Select(x => x.ToTaskEntity()).FirstOrDefault();

        return entity ?? throw new EntityNotFoundException(id);
    }

    public async Task<(IEnumerable<TaskEntity> Entities, object ContinuationToken)> GetRootTaskBatchAsync(int take, object continuationToken, string sortBy, bool descending, CancellationToken cancellationToken)
    {
        var continuationTokenParsed = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(continuationToken?.ToString() ?? "");
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
            KeyConditionExpression = "RootTaskId = :rootTaskId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rootTaskId", new AttributeValue { NULL = true } }
            },
            ScanIndexForward = !descending,
            Limit = take
        };

        if (continuationTokenParsed is not null)
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
        var continuationTokenParsed = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(continuationToken?.ToString() ?? "");

        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            IndexName = Constants.TasksLocalSummaryIndexName,
            KeyConditionExpression = "RootTaskId = :rootTaskId and contains(Summary, :keyPhrase)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rootTaskId", new AttributeValue { NULL = true } },
                { ":keyPhrase", new AttributeValue { S = keyPhrase } }
            },
            Limit = take,
            AttributesToGet = new List<string> { "Id", "Summary", "Description" }
        };

        if (continuationTokenParsed is not null)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToTaskSearchEntity());

        return (entities, response.LastEvaluatedKey);
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity entity, CancellationToken cancellationToken)
    {
        var taskDocument = new Document();
        taskDocument["Id"] = entity.Id;

        taskDocument["Summary"] = entity.Summary;
        taskDocument["Description"] = entity.Description;
        taskDocument["Priority"] = entity.Priority.ToString();
        taskDocument["Status"] = entity.Status.ToString();
        taskDocument["DueDate"] = entity.DueDate;

        var config = new UpdateItemOperationConfig
        {
            ReturnValues = ReturnValues.AllNewAttributes
        };

        var updated = await _table.UpdateItemAsync(taskDocument, config, cancellationToken);

        return updated.ToTaskEntity();
    }

    public async Task UpdateTaskRootAsync(Guid taskId, Guid? newRootId, CancellationToken cancellationToken)
    {
        var taskDocument = new Document();
        taskDocument["Id"] = taskId;
        taskDocument["RootTaskId"] = newRootId?.ToString();

        await _table.UpdateItemAsync(taskDocument, cancellationToken);
    }

    private async Task<IEnumerable<Guid>> GetSubtaskIdsAsync(Guid taskId, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.TasksTableName,
            KeyConditionExpression = "RootTaskId = :rootTaskId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":rootTaskId", new AttributeValue { S = taskId.ToString() } }
            },
            AttributesToGet = new List<string> { "Id" }
        };

        var documents = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var ids = documents.Items.Select(x => x["Id"].S).Select(Guid.Parse);

        return ids;
    }
}