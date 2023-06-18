using System.Linq.Expressions;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Core.Extensions
{
    public static class DbExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> query, SearchQuery<T>? searchQuery) where T : EntityBase
        {
            if (searchQuery == null) return query;
            var searchExpression = searchQuery.BuildSearchExpression();
            return query.Where(searchExpression);
        }
        public static PageData<T> GetPageData<T>(this IQueryable<T> query, PagingModel pagingModel, OrderQuery<T>[]? orders = null) where T : EntityBase
        {
            return query.GetPageDataAsync(pagingModel, orders).Result;
        }
        public static async Task<PageData<T>> GetPageDataAsync<T>(this IQueryable<T> query, PagingModel pagingModel, OrderQuery<T>[]? orders = null) where T : EntityBase
        {
            int? pageIndex = pagingModel.PageIndex;
            int? pageSize = pagingModel.PageSize;
            var count = await query.CountAsync();
            if (orders == null)
            {
                if (!pageIndex.HasValue || !pageSize.HasValue)
                {
                    return new PageData<T>(query.ToArray(), count);
                }

                return new PageData<T>(query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToArray(), count);
            }
            IOrderedEnumerable<T>? orderedData = null;
            for (int i = 0; i < orders.Length; i++)
            {
                OrderQuery<T> order = orders[i];
                orderedData = i == 0
                    ? order.IsAscending ? query.OrderBy(order.Order) : query.OrderByDescending(order.Order)
                    : order.IsAscending ? orderedData!.ThenBy(order.Order) : orderedData!.ThenByDescending(order.Order);
            }
            if (!pageIndex.HasValue || !pageSize.HasValue)
            {
                return new PageData<T>(orderedData != null ? orderedData.ToArray() : query.ToArray(), count);


            }
            return new PageData<T>(orderedData != null ? orderedData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToArray() : query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToArray(), count);
        }

        public static bool SoftDelete<T>(this DbContext db, T entity) where T : EntityBase, ISoftDeletable
        {
            entity.IsDeleted = true;
            db.Entry(entity).State = EntityState.Modified;
            _ = db.SaveChanges();
            return true;
        }
        public static CreateOrUpdateResponse<T> CreateOrUpdate<T>(this DbContext db, T entity, Expression<Func<T, object>> searchSelector, Expression<Func<T, object>> updateSelector) where T : EntityBase
        {
            var dbSet = db.Set<T>();
            System.Reflection.PropertyInfo[] searchProperties = searchSelector.Body.Type.GetProperties();
            System.Reflection.PropertyInfo[] entityProperties = typeof(T).GetProperties();
            T? item = null;

            ParameterExpression parameterExpression = searchSelector.Parameters[0];
            Expression expression = parameterExpression;

            Expression? body = null;
            foreach (System.Reflection.PropertyInfo property in searchProperties)
            {
                System.Reflection.PropertyInfo? existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                if (existedProperty != null)
                {
                    ConstantExpression valueExpression = Expression.Constant(existedProperty.GetValue(entity));
                    body = body == null
                        ? Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression)
                        : (Expression)Expression.AndAlso(body, Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression));
                }
            }
            if (body == null)
            {
                dbSet.Add(entity);
                db.SaveChanges();
                return new CreateOrUpdateResponse<T>(false, entity);
            }
            else
            {
                Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
                item = dbSet.Where(existQuery).FirstOrDefault();
                if (item != null)
                {
                    // Parse item
                    System.Reflection.PropertyInfo[] updateProperties = updateSelector.Body.Type.GetProperties();
                    foreach (System.Reflection.PropertyInfo property in updateProperties)
                    {
                        System.Reflection.PropertyInfo? existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                        if (existedProperty != null)
                        {
                            object? value = existedProperty.GetValue(entity);
                            existedProperty.SetValue(item, value);
                        }
                    }
                    dbSet.Update(item);
                    db.SaveChanges();
                    return new CreateOrUpdateResponse<T>(true, entity);
                }
                else
                {
                    dbSet.Add(entity);
                    db.SaveChanges();
                    return new CreateOrUpdateResponse<T>(false, entity);
                }
            }
        }


        public static CreateIfNotExistResponse<T> CreateIfNotExists<T>(this DbContext db, T entity, Expression<Func<T, object>> selector) where T : EntityBase
        {
            System.Reflection.PropertyInfo[] properties = selector.Body.Type.GetProperties();
            System.Reflection.PropertyInfo[] entityProperties = typeof(T).GetProperties();

            ParameterExpression parameterExpression = selector.Parameters[0];
            Expression expression = parameterExpression;

            Expression? body = null;
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                System.Reflection.PropertyInfo? existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                if (existedProperty != null)
                {
                    ConstantExpression valueExpression = Expression.Constant(existedProperty.GetValue(entity));
                    body = body == null
                        ? Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression)
                        : (Expression)Expression.AndAlso(body, Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression));
                }
            }
            if (body == null)
            {
                db.Add(entity);
                db.SaveChanges();
                return new CreateIfNotExistResponse<T>(false, entity);
            }

            Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
            T? existingEntity = db.Set<T>().Where(existQuery).FirstOrDefault();
            if (existingEntity == null)
            {
                db.Set<T>().Add(entity);
                db.SaveChanges();
                return new CreateIfNotExistResponse<T>(false, entity);
            }

            return new CreateIfNotExistResponse<T>(true, existingEntity); ;
        }


    }
}