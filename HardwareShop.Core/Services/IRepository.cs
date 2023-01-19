using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Services
{

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
        Task<PageData<T>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, Boolean>> expression, List<QueryOrder<T>>? orders);
        Task<List<T>> GetDataByQueryAsync(Expression<Func<T, Boolean>> expression);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByQueryAsync(Expression<Func<T, Boolean>> expression);
    }
}
