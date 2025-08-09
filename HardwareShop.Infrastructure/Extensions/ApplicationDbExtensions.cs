

using HardwareShop.Application.Models;
using HardwareShop.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Infrastructure.Extensions;
public static class ApplicationDbExtensions
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
}