namespace MCGAssignment.TodoList.Infrastructure.DynamoDB;

public static class Constants
{
    public const string TasksTableName = "Tasks";
    public const string LogsTableName = "Logs";

    public const string TasksGlobalIdIndexName = "G_IdIndex";
    public const string TasksLocalSummaryIndexName = "L_SummaryIndex";
    public const string TasksLocalCreateDateIndexName = "L_CreateDateIndex";
    public const string TasksLocalDueDateIndexName = "L_DueDateIndex";
    public const string TasksLocalPriorityIndexName = "L_PriorityIndex";
    public const string TasksLocalStatusIndexName = "L_StatusIndex";

    public const string LogsGlobalEntityIdIndexName = "G_EntityIdIndex";
    public const string LogsGlobalEntityTypeNameIndexName = "G_EntityTypeNameIndex";
}