using System.Linq.Expressions;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Core.Implementations
{
    public class RepositoryBase<T> : IRepository<T> where T : EntityBase
    {
        private readonly DbContext db;
        public RepositoryBase(DbContext db)
        {
            this.db = db;
        }
        public DbSet<T> DbSet => db.Set<T>();
        public async Task<T> CreateAsync(T entity)
        {
            _ = await DbSet.AddAsync(entity);
            _ = await db.SaveChangesAsync();
            return entity;
        }


        public async Task<bool> DeleteAsync(T entity)
        {
            _ = DbSet.Remove(entity);
            _ = await db.SaveChangesAsync();
            return true;

        }
        public async Task<bool> DeleteSoftlyAsync<T1>(T1 entity) where T1 : T, ISoftDeletable
        {

            entity.IsDeleted = true;
            db.Entry(entity).State = EntityState.Modified;
            _ = await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRangeByQueryAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> entities = DbSet.Where(expression);
            db.RemoveRange(entities);
            _ = await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteByQueryAsync(Expression<Func<T, bool>> expression)
        {
            T? entity = DbSet.FirstOrDefault(expression);
            return entity != null && await DeleteAsync(entity);
        }

        public async Task<List<T>> GetDataByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public async Task<PageData<T1>> GetDtoPageDataByQueryAsync<T1>(PagingModel pagingModel, Expression<Func<T, bool>> expression, Func<T, T1> convertor, SearchQuery<T>? searchQuery = null, List<QueryOrder<T>>? orders = null) where T1 : class
        {
            var entityPageData = await GetPageDataByQueryAsync(pagingModel, expression, searchQuery, orders);
            return PageData<T1>.ConvertFromOtherPageData(entityPageData, convertor);
        }
        public async Task<PageData<T>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, bool>> expression, SearchQuery<T>? searchQuery, List<QueryOrder<T>>? orders)
        {
            int? pageIndex = pagingModel.PageIndex;
            int? pageSize = pagingModel.PageSize;

            IQueryable<T> filteredDbSet = DbSet.Where(expression);
            if (searchQuery != null)
            {
                Expression<Func<T, bool>> searchExpression = searchQuery.BuildSearchExpression();
                filteredDbSet = filteredDbSet.Where(searchExpression);
            }

            int count = await filteredDbSet.CountAsync();


            IQueryable<T> data = filteredDbSet;
            if (pageIndex.HasValue && pageSize.HasValue)
            {

                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T>? orderedData = null;
                    for (int i = 0; i < orders.Count; i++)
                    {
                        QueryOrder<T> order = orders[i];
                        orderedData = i == 0
                            ? order.IsAscending ? data.OrderBy(order.Order) : data.OrderByDescending(order.Order)
                            : order.IsAscending ? orderedData!.ThenBy(order.Order) : orderedData!.ThenByDescending(order.Order);
                    }
                    List<T> finalData = orderedData!.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    List<T> finalData = data.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
            }
            else
            {
                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T>? orderedData = null;
                    for (int i = 0; i < orders.Count; i++)
                    {
                        QueryOrder<T> order = orders[i];
                        orderedData = i == 0
                            ? order.IsAscending ? data.OrderBy(order.Order) : data.OrderByDescending(order.Order)
                            : order.IsAscending ? orderedData!.ThenBy(order.Order) : orderedData!.ThenByDescending(order.Order);
                    }
                    List<T> finalData = orderedData!.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    List<T> finalData = data.ToList();
                    return new PageData<T> { Items = finalData, TotalRecords = count };
                }
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _ = DbSet.Update(entity);
            db.Entry(entity).State = EntityState.Modified;
            _ = await db.SaveChangesAsync();
            return entity;
        }

        public async Task<T?> GetItemByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return typeof(ITrackingDate).IsAssignableFrom(typeof(T))
                ? await DbSet.OrderByDescending(e => ((ITrackingDate)e).CreatedDate).FirstOrDefaultAsync(expression)
                : await DbSet.FirstOrDefaultAsync(expression);

        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.AnyAsync(expression);
        }

        public async Task<CreateIfNotExistResponse<T>> CreateIfNotExistsAsync(T entity, Expression<Func<T, object>> selector)
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
                entity = await CreateAsync(entity);
                return new CreateIfNotExistResponse<T>(false, entity);
            }

            Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
            T? existingEntity = DbSet.Where(existQuery).FirstOrDefault();
            if (existingEntity == null)
            {
                entity = await CreateAsync(entity);
                return new CreateIfNotExistResponse<T>(false, entity);
            }

            return new CreateIfNotExistResponse<T>(true, existingEntity); ;
        }

        public virtual async Task<CreateOrUpdateResponse<T>> CreateOrUpdateAsync(T entity, Expression<Func<T, object>> searchSelector, Expression<Func<T, object>> updateSelector)
        {
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
                entity = await CreateAsync(entity);
                return new CreateOrUpdateResponse<T>(false, entity);
            }
            else
            {
                Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
                item = DbSet.Where(existQuery).FirstOrDefault();
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
                    entity = await UpdateAsync(item);
                    return new CreateOrUpdateResponse<T>(true, entity);
                }
                else
                {
                    entity = await CreateAsync(entity);
                    return new CreateOrUpdateResponse<T>(false, entity);
                }
            }


        }

        public DbSet<T> GetDbSet()
        {
            return DbSet;
        }
    }
}
