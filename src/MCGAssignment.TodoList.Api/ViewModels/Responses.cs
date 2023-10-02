using MCGAssignment.TodoList.Application.DataTransferObjects;

namespace MCGAssignment.TodoList.Api.ViewModels
{
    public record BatchResponse<TView>
    {
        public required List<TView> Entities { get; init; } 
        public required object ContinuationToken { get; init; }
    }

    public record TaskSearchResponse : BatchResponse<TaskSearchView> { }

    public record TaskResponse : BatchResponse<TaskViewDetailed> { }
}
