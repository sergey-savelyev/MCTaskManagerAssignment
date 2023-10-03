using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MCGAssignment.TodoList.Core.Entities;
using MCGAssignment.TodoList.Core.Enums;

namespace MCGAssignment.TodoList.Infrastructure.DynamoDB.Extensions;

public static class MappingExtensions
{
    public static Document ToDocument(this LogEntity entity)
    {
        var document = new Document
        {
            ["Id"] = entity.Id.ToString(),
            ["EntityId"] = entity.EntityId.ToString(),
            ["EntityType"] = entity.EntityType,
            ["Action"] = (byte)entity.Action,
            ["TimestampMsec"] = entity.TimestampMsec,
            ["Payload"] = entity.Payload
        };

        return document;
    }

    public static Document ToDocument(this TaskEntity entity)
    {
        var document = new Document
        {
            ["Id"] = entity.Id.ToString(),
            ["RootTaskId"] = entity.RootTaskId?.ToString() ?? "null",
            ["Summary"] = entity.Summary,
            ["Description"] = entity.Description,
            ["Priority"] = (byte)entity.Priority,
            ["Status"] = (byte)entity.Status,
            ["CreateDate"] = entity.CreateDate,
            ["DueDate"] = entity.DueDate
        };

        return document;
    }

    public static TaskEntity ToTaskEntity(this Document document)
    {
        var entity = new TaskEntity
        {
            Id = Guid.Parse(document["Id"].AsString()),
            RootTaskId = document["RootTaskId"].AsString() is null || document["RootTaskId"].AsString() == "null" ? null : Guid.Parse(document["RootTaskId"].AsString()),
            Summary = document["Summary"].AsString(),
            Description = document.ContainsKey("Description") ? document["Description"].AsString() : null,
            Priority = (TaskPriority)document["Priority"].AsByte(),
            Status = (Core.Enums.TaskStatus)document["Status"].AsByte(),
            CreateDate = document["CreateDate"].AsDateTime(),
            DueDate = document["DueDate"].AsDateTime()
        };

        return entity;
    }

    public static TaskEntity ToTaskEntity(this Dictionary<string, AttributeValue> item)
    {
        var entity = new TaskEntity
        {
            Id = Guid.Parse(item["Id"].S),
            RootTaskId = item["RootTaskId"]?.S is null || item["RootTaskId"].S == "null" ? null : Guid.Parse(item["RootTaskId"].S),
            Summary = item["Summary"].S,
            Description = item.ContainsKey("Description") ? item["Description"].S : null,
            Priority = Enum.Parse<TaskPriority>(item["Priority"].N),
            Status = Enum.Parse<Core.Enums.TaskStatus>(item["Status"].N),
            CreateDate = DateTime.Parse(item["CreateDate"].S),
            DueDate = DateTime.Parse(item["DueDate"].S)
        };

        return entity;
    }

    public static TaskSearchEntity ToTaskSearchEntity(this Dictionary<string, AttributeValue> item)
    {
        var entity = new TaskSearchEntity
        (
            Guid.Parse(item["Id"].S),
            item["Summary"].S,
            item.ContainsKey("Description") ? item["Description"].S : null
        );

        return entity;
    }

    public static LogEntity ToLogEntity(this Document document)
    {
        var entity = new LogEntity
        {
            Id = Guid.Parse(document["Id"].AsString()),
            EntityId = Guid.Parse(document["EntityId"].AsString()),
            EntityType = document["EntityType"].AsString(),
            Action = (TaskAction)document["Action"].AsByte(),
            TimestampMsec = document["TimestampMsec"].AsLong(),
            Payload = document.ContainsKey("Payload") ? document["Payload"].AsString() : null
        };

        return entity;
    }

    public static LogEntity ToLogEntity(this Dictionary<string, AttributeValue> item)
    {
        var entity = new LogEntity
        {
            Id = Guid.Parse(item["Id"].S),
            EntityId = Guid.Parse(item["EntityId"].S),
            EntityType = item["EntityType"].S,
            Action = Enum.Parse<TaskAction>(item["Action"].N),
            TimestampMsec = long.Parse(item["TimestampMsec"].N),
            Payload = item.ContainsKey("Payload") ? item["Payload"].S : null
        };

        return entity;
    }
}