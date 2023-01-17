using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Implementations
{
    public class RepositoryBase<T, T1> : IRepository<T, T1> where T : EntityBase<T1> where T1 : struct
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

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByQueryAsync(Expression<Func<T, bool>> expression)
        {
            var entities = dbSet.Where(expression);
            db.RemoveRange(entities);
            await db.SaveChangesAsync();
        }

        public async Task<List<T>> GetDataByQueryAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        }

        public async Task<PageData<T, T1>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, bool>> expression, List<QueryOrder<T, T1>>? orders)
        {
            var pageIndex = pagingModel.PageIndex;
            var pageSize = pagingModel.PageSize;
            var count = await dbSet.CountAsync(expression);
            IQueryable<T> data = dbSet.Where(expression);
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                data = data.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T> orderedData = null;
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
                    return new PageData<T, T1> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    var finalData = data.ToList();
                    return new PageData<T, T1> { Items = finalData, TotalRecords = count };
                }
            }
            else
            {
                if (orders is not null && orders.Count > 0)
                {
                    IOrderedEnumerable<T> orderedData = null;
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
                    return new PageData<T, T1> { Items = finalData, TotalRecords = count };
                }
                else
                {
                    var finalData = data.ToList();
                    return new PageData<T, T1> { Items = finalData, TotalRecords = count };
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
    }
}
