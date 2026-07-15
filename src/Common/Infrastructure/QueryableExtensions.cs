using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PAS.Common.Infrastructure;

public static class QueryableExtensions {

    /// <summary>
    /// Filters the source IQueryable based on a condition. If the condition is true, 
    /// applies the provided predicate; otherwise, returns the original source.
    /// </summary>
    public static IQueryable<TEntity> WhereIf<TEntity>(
        this IQueryable<TEntity> source,
        bool condition,
        Expression<Func<TEntity, bool>> predicate
    ) {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Filters the source IQueryable based on a search term applied to a specified string property of the entity.
    /// Considers the presence of an asterisk (*) in the search term for pattern matching using EF.Functions.Like.
    /// </summary>
    public static IQueryable<TEntity> WhereSearch<TEntity>(
        this IQueryable<TEntity> source,
        Expression<Func<TEntity, string>> propertySelector,
        string? searchTerm
    ) {
        if (searchTerm == null) return source;

        // If the term contains an asterisk (*), we will use EF.Functions.Like for pattern matching
        if (searchTerm.Contains('*')) {
            string likeExpression = searchTerm.Replace('*', '%');

            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.Like),
                [typeof(DbFunctions), typeof(string), typeof(string)]
            )!;

            var efFunctions = Expression.Constant(EF.Functions);
            var likeCall = Expression.Call(likeMethod, efFunctions, propertySelector.Body, Expression.Constant(likeExpression));

            var lambda = Expression.Lambda<Func<TEntity, bool>>(likeCall, propertySelector.Parameters);
            return source.Where(lambda);
        }

        // If the term does not contain an asterisk, we will use a simple equality check
        var equalsCall = Expression.Equal(propertySelector.Body, Expression.Constant(searchTerm));
        var equalsLambda = Expression.Lambda<Func<TEntity, bool>>(equalsCall, propertySelector.Parameters);
        return source.Where(equalsLambda);
    }
}
