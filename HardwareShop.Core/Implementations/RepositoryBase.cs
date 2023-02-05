using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    public class RepositoryBase<T> : IRepository<T> where T : EntityBase
    {
        private readonly DbContext db;
        public RepositoryBase(DbContext db)
        {
            this.db = db;
        }
        private DbSet<T> dbSet => db.Set<T>();
        public async Task<T> CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
            return true;

        }
        public async Task<bool> DeleteSoftlyAsync<T1>(T1 entity) where T1 : T, ISoftDeletable
        {

            entity.IsDeleted = true;
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRangeByQueryAsync(Expression<Func<T, bool>> expression)
        {
            var entities = dbSet.Where(expression);
            db.RemoveRange(entities);
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteByQueryAsync(Expression<Func<T, bool>> expression)
        {
            var entity = dbSet.FirstOrDefault(expression);
            if (entity == null)
                return false;
            return await DeleteAsync(entity);
        }

        public async Task<List<T>> GetDataByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        }

        public async Task<PageData<T>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, bool>> expression, SearchQuery<T>? searchQuery, List<QueryOrder<T>>? orders)
        {
            var pageIndex = pagingModel.PageIndex;
            var pageSize = pagingModel.PageSize;

            var filteredDbSet = dbSet.Where(expression);
            if (searchQuery != null)
            {
                var searchExpression = searchQuery.BuildSearchExpression();
                filteredDbSet = filteredDbSet.Where(searchExpression);
            }

            var count = await filteredDbSet.CountAsync();


            IQueryable<T> data = filteredDbSet;
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                data = data.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T>? orderedData = null;
                    for (var i = 0; i < orders.Count; i++)
                    {
                        var order = orders[i];
                        if (i == 0)
                        {
                            if (order.IsAscending)
                            {
                                orderedData = data.OrderBy(order.Order);
                            }
                            else
                            {
                                orderedData = data.OrderByDescending(order.Order);
                            }
                        }
                        else
                        {
                            if (order.IsAscending)
                            {
                                orderedData = orderedData!.ThenBy(order.Order);
                            }
                            else
                            {
                                orderedData = orderedData!.ThenByDescending(order.Order);
                            }
                        }
                    }
                    var finalData = orderedData!.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    var finalData = data.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
            }
            else
            {
                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T>? orderedData = null;
                    for (var i = 0; i < orders.Count; i++)
                    {
                        var order = orders[i];
                        if (i == 0)
                        {
                            if (order.IsAscending)
                            {
                                orderedData = data.OrderBy(order.Order);
                            }
                            else
                            {
                                orderedData = data.OrderByDescending(order.Order);
                            }
                        }
                        else
                        {
                            if (order.IsAscending)
                            {
                                orderedData = orderedData!.ThenBy(order.Order);
                            }
                            else
                            {
                                orderedData = orderedData!.ThenByDescending(order.Order);
                            }
                        }
                    }
                    var finalData = orderedData!.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    var finalData = data.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> GetItemByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.FirstOrDefaultAsync(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.AnyAsync(expression);
        }

        public async Task<T?> CreateIfNotExists(T entity, Expression<Func<T, object>> selector)
        {
            var properties = selector.Body.Type.GetProperties();
            var entityProperties = typeof(T).GetProperties();

            ParameterExpression parameterExpression = selector.Parameters[0];
            Expression expression = (Expression)parameterExpression;

            Expression? body = null;
            foreach (var property in properties)
            {
                var existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                if (existedProperty != null)
                {
                    ConstantExpression valueExpression = Expression.Constant(existedProperty.GetValue(entity));
                    if (body == null)
                    {

                        body = Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression);
                    }
                    else
                    {
                        body = Expression.AndAlso(body, Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression));
                    }
                }
            }
            if (body == null)
            {
                return await CreateAsync(entity);
            }

            Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
            var exist = dbSet.Where(existQuery).Any();
            if (!exist)
            {
                return await CreateAsync(entity);
            }

            return null;
        }

        public async Task<T> CreateOrUpdate(T entity, Expression<Func<T, object>> searchSelector, Expression<Func<T, object>> updateSelector)
        {
            var searchProperties = searchSelector.Body.Type.GetProperties();
            var entityProperties = typeof(T).GetProperties();
            T? item = null;

            ParameterExpression parameterExpression = searchSelector.Parameters[0];
            Expression expression = (Expression)parameterExpression;

            Expression? body = null;
            foreach (var property in searchProperties)
            {
                var existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                if (existedProperty != null)
                {
                    ConstantExpression valueExpression = Expression.Constant(existedProperty.GetValue(entity));
                    if (body == null)
                    {

                        body = Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression);
                    }
                    else
                    {
                        body = Expression.AndAlso(body, Expression.Equal(Expression.Property(parameterExpression, property.Name), valueExpression));
                    }
                }
            }
            if (body == null)
            {
                return await CreateAsync(entity);
            }
            else
            {
                Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
                item = dbSet.Where(existQuery).FirstOrDefault();
                if (item != null)
                {
                    // Parse item
                    var updateProperties = updateSelector.Body.Type.GetProperties();
                    foreach (var property in updateProperties)
                    {
                        var existedProperty = entityProperties.Where(e => e.Name == property.Name && e.PropertyType.FullName == property.PropertyType.FullName).FirstOrDefault();
                        if (existedProperty != null)
                        {
                            var value = existedProperty.GetValue(entity);
                            existedProperty.SetValue(item, value);
                        }
                    }

                    return await UpdateAsync(item);
                }
                else
                {
                    return await CreateAsync(entity);
                }
            }


        }
    }
}
