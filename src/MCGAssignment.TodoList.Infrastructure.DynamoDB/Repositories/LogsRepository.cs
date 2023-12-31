using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MCGAssignment.TodoList.Application.Repositories;
using MCGAssignment.TodoList.Core.Entities;
using MCGAssignment.TodoList.Infrastructure.DynamoDB.Extensions;

namespace MCGAssignment.TodoList.Infrastructure.DynamoDB.Repositories;

public class LogsRepository : ILogsRepository
{
    private readonly Table _table;
    private readonly IAmazonDynamoDB _dynamoDBClient;

    public LogsRepository(IAmazonDynamoDB dynamoDBClient)
    {
        _table = Table.LoadTable(dynamoDBClient, Constants.LogsTableName);
        _dynamoDBClient = dynamoDBClient;
    }

    public async Task<LogEntity> AddAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        await _table.PutItemAsync(entity.ToDocument(), cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _table.DeleteItemAsync(id, cancellationToken);
    }

    public async Task<LogEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var document = await _table.GetItemAsync(id, cancellationToken);

        return document.ToLogEntity();
    }

    public async Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityAsync(Guid entityId, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.LogsTableName,
            IndexName = Constants.LogsGlobalEntityIdIndexName,
            ScanIndexForward = !descending,
            KeyConditionExpression = "EntityId = :entityId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":entityId", new AttributeValue { S = entityId.ToString() } }
            },
            Limit = take
        };

        if (continuationToken is not null && continuationToken is Dictionary<string, AttributeValue> continuationTokenParsed)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToLogEntity());

        return (Entities: entities, ContinuationToken: response.LastEvaluatedKey);
    }

    public async Task<(IEnumerable<LogEntity> Entities, object ContinuationToken)> GetLogBatchByEntityTypeAsync(string entityType, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        QueryRequest queryRequest = new QueryRequest
        {
            TableName = Constants.LogsTableName,
            IndexName = Constants.LogsGlobalEntityTypeNameIndexName,
            ScanIndexForward = !descending,
            KeyConditionExpression = "EntityType = :entityType",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":entityType", new AttributeValue { S = entityType } }
            },
            Limit = take,
        };

        if (continuationToken is not null && continuationToken is Dictionary<string, AttributeValue> continuationTokenParsed)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToLogEntity());

        return (Entities: entities, ContinuationToken: response.LastEvaluatedKey);
    }

    public Task<LogEntity> UpdateAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Logs are immutable");
    }
}