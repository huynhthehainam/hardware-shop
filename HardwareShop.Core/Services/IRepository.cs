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

    public class QueryOrder<T, T1> where T : EntityBase<T1> where T1 : struct
    {
        public Func<T, Object?> Order { get; set; }
        public bool IsAscending { get; set; }
        public QueryOrder(Func<T, object?> order, bool isAscending)
        {
            Order = order;
            IsAscending = isAscending;
        }
    }
    public interface IRepository<T, T1> where T : EntityBase<T1> where T1 : struct
    {
        Task<PageData<T, T1>> GetPageDataByQueryAsync(PagingModel pagingModel, Expression<Func<T, Boolean>> expression, List<QueryOrder<T, T1>>? orders);
        Task<List<T>> GetDataByQueryAsync(Expression<Func<T, Boolean>> expression);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByQueryAsync(Expression<Func<T, Boolean>> expression);
    }
}
