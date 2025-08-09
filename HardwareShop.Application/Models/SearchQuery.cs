

using System.Linq.Expressions;
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Application.Models
{
    public class SearchQuery<T> where T : EntityBase
    {
        private readonly string search;
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
}