using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using System.Linq.Expressions;

namespace HardwareShop.Core.Services
{
    public class SearchQuery<T> where T : EntityBase
    {
        private string search;
        public Expression<Func<T, object>> selector;
        public SearchQuery(string search, Expression<Func<T, object>> expression)
        {
            this.search = search;
            this.selector = expression;
        }
        public Expression<Func<T, bool>> BuildSearchExpression()
        {
            var properties = selector.Body.Type.GetProperties();
            var entityProperties = typeof(T).GetProperties();

            ParameterExpression parameterExpression = selector.Parameters[0];
            Expression expression = (Expression)parameterExpression;

            Expression? body = null;

            foreach (var property in properties)
            {
                var existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName && e.PropertyType.FullName == "System.String").FirstOrDefault();
                if (existedProperty != null)
                {
                    ConstantExpression valueExpression = Expression.Constant(search.ToLower());
                    var likeExpression = Expression.Equal(
                        Expression.Call(Expression.Call(
                        Expression.Property(parameterExpression, property.Name), typeof(string).GetMethods().First(e => e.Name == "ToLower")), typeof(string).GetMethods().First(e => e.Name == "Contains"), new Expression[] { valueExpression }), Expression.Constant(true));
                    if (body == null)
                    {

                        body = likeExpression;
                    }
                    else
                    {
                        body = Expression.OrElse(body, likeExpression);
                    }
                }
            }

            if (body == null)
            {
                return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameterExpression);
            }

            return Expression.Lambda<Func<T, bool>>(body, parameterExpression);
        }
    }
    public class QueryOrder<T> where T : EntityBase
    {
        public Func<T, object?> Order { get; set; }
        public bool IsAscending { get; set; }
        public QueryOrder(Func<T, object?> order, bool isAscending)
        {
            Order = order;
            IsAscending = isAscending;
        }
    }


    public interface IRepository<T> where T : EntityBase
    {
        Task<PageData<T>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, bool>> expression, SearchQuery<T>? searchQuery = null, List<QueryOrder<T>>? orders = null);
        Task<List<T>> GetDataByQueryAsync(Expression<Func<T, Boolean>> expression);
        Task<bool> DeleteByQueryAsync(Expression<Func<T, Boolean>> expression);
        Task<T?> GetItemByQueryAsync(Expression<Func<T, bool>> expression);
        Task<T> CreateAsync(T entity);
        Task<bool> DeleteSoftlyAsync<T1>(T1 entity) where T1 : T, ISoftDeletable;
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteRangeByQueryAsync(Expression<Func<T, Boolean>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<T?> CreateIfNotExistsAsync(T entity, Expression<Func<T, object>> selector);
        Task<T> CreateOrUpdateAsync(T entity, Expression<Func<T, object>> searchSelector, Expression<Func<T, object>> updateSelector);
    }
}
