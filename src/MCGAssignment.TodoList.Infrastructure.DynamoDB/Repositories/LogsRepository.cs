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
    private const string TableName = "Logs";
    private const string EntityIdIndexName = "EntityIdIndex";
    private const string EntityTypeNameIndexName = "EntityTypeNameIndex";

    private readonly Table _table;
    private readonly IAmazonDynamoDB _dynamoDBClient;

    public LogsRepository(IAmazonDynamoDB dynamoDBClient)
    {
        _table = Table.LoadTable(dynamoDBClient, TableName);
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

    public async Task<IEnumerable<LogEntity>> GetLogBatchByEntityAsync(Guid entityId, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        var continuationTokenParsed = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(continuationToken?.ToString() ?? "");

        QueryRequest queryRequest = new QueryRequest
        {
            TableName = TableName,
            IndexName = EntityIdIndexName,
            ScanIndexForward = !descending,
            KeyConditionExpression = "EntityId = :entityId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":entityId", new AttributeValue { S = entityId.ToString() } }
            },
            Limit = take
        };

        if (continuationTokenParsed is not null)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToLogEntity());

        return entities;
    }

    public async Task<IEnumerable<LogEntity>> GetLogBatchByEntityTypeAsync(string entityType, object continuationToken, int take, bool descending, CancellationToken cancellationToken)
    {
        var continuationTokenParsed = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(continuationToken?.ToString() ?? "");

        QueryRequest queryRequest = new QueryRequest
        {
            TableName = TableName,
            IndexName = EntityTypeNameIndexName,
            ScanIndexForward = !descending,
            KeyConditionExpression = "EntityType = :entityType",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":entityType", new AttributeValue { S = entityType } }
            },
            Limit = take,
        };

        if (continuationTokenParsed is not null)
        {
            queryRequest.ExclusiveStartKey = continuationTokenParsed;
        }

        var response = await _dynamoDBClient.QueryAsync(queryRequest, cancellationToken);
        var entities = response.Items.Select(x => x.ToLogEntity());

        return entities;
    }

    public Task<LogEntity> UpdateAsync(LogEntity entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Logs are immutable");
    }
}