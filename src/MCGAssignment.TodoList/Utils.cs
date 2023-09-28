namespace MCGAssignment.TodoList;

public static class Utils
{
    public static void ThrowIfNotGuid(this string? str)
    {
        if (str is not null && !Guid.TryParse(str, out _))
        {
            throw new ArgumentException("Guid or null string expected");
        }
    }
}