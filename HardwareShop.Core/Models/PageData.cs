using HardwareShop.Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Models
{
    public class PageData<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRecords { get; set; } = 0;
        public PageData() { }
        public static PageData<T> ConvertFromOtherPageData<TFrom>(PageData<TFrom> pageData, Func<TFrom, T> selector)
        {
            List<T> newItems = pageData.Items.Select(selector).ToList();
            return new PageData<T> { Items = newItems, TotalRecords = pageData.TotalRecords };
        }
    }
}
