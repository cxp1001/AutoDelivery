using System.Linq.Expressions;

namespace AutoDelivery.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, false);
        }
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, desc);
        }
        static class QueryableHelper<T>
        {
            private static Dictionary<string, LambdaExpression> cache = new Dictionary<string, LambdaExpression>();
            public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool desc)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                var res = desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
                return res;
            }
            private static LambdaExpression GetLambdaExpression(string propertyName)
            {
                if (cache.ContainsKey(propertyName)) return cache[propertyName];
                var param = Expression.Parameter(typeof(T));
                var body = Expression.Property(param, propertyName);
                var keySelector = Expression.Lambda(body, param);
                cache[propertyName] = keySelector;
                return keySelector;
            }
        }
    }
}