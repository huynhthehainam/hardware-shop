
using System.Linq.Expressions;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;

namespace HardwareShop.Dal.Extensions
{
    public enum CreateOrUpdateAssetStatus
    {
        Created,
        Updated,
        Invalid
    }
    public sealed class CreateOrUpdateAssetResponse<T> where T : AssetEntityBase
    {
        public CreateOrUpdateAssetStatus Status { get; set; } = CreateOrUpdateAssetStatus.Invalid;
        public T Entity { get; set; }
        public CreateOrUpdateAssetResponse(CreateOrUpdateAssetStatus status, T entity)
        {
            this.Status = status;
            this.Entity = entity;
        }
    }
    public static class AssetEntityBaseRepositoryExtensions
    {
        public static async Task<CreateOrUpdateAssetResponse<T>> CreateOrUpdateAssetAsync<T>(this IRepository<T> repository, T entity, Expression<Func<T, object>> searchSelector, Expression<Func<T, object>> updateSelector) where T : AssetEntityBase
        {
            var asset = entity.Asset;
            if (asset == null)
            {
                return new CreateOrUpdateAssetResponse<T>(CreateOrUpdateAssetStatus.Invalid, entity);
            }
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

                entity = await repository.CreateAsync(entity);
                return new CreateOrUpdateAssetResponse<T>(CreateOrUpdateAssetStatus.Created, entity);
            }
            else
            {
                Expression<Func<T, bool>> existQuery = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
                item = repository.GetDbSet().Where(existQuery).FirstOrDefault();
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
                    item.Asset = asset;
                    entity = await repository.UpdateAsync(item);
                    return new CreateOrUpdateAssetResponse<T>(CreateOrUpdateAssetStatus.Updated, entity);
                }
                else
                {
                    entity = await repository.CreateAsync(entity);
                    return new CreateOrUpdateAssetResponse<T>(CreateOrUpdateAssetStatus.Created, entity);
                }
            }

        }
    }
}