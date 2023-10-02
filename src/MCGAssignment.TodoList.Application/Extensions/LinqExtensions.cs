using System.Linq.Expressions;

namespace MCGAssignment.TodoList.Application.Extensions;

public static class LinqExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression<Func<T, object?>> property, bool descending) => descending
        ? source.OrderByDescending(property)
        : source.OrderBy(property);
}