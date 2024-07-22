using System.Linq.Expressions;

namespace GenGui_CrystalStar.Code.DatabaseModels.Commands.Helpers;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                      Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }
}