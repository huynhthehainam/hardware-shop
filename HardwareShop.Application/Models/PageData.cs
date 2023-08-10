using System.Collections;

namespace HardwareShop.Application.Models
{
    public class PageData<T> : IEnumerable<T>
    {
        private T[] items;
        public int TotalRecords { get; set; }
        public PageData(T[] items, int count)
        {
            this.items = items;
            TotalRecords = count;
        }
        public static PageData<T> EmptyPageData()
        {
            return new PageData<T>(Array.Empty<T>(), 0);
        }
        public T[] ToArray()
        {
            return items;
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
        public void SetItems(T[] items)
        {
            this.items = items;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    public static class PageDataExtensions
    {
        public static PageData<T> ConvertToOtherPageData<T, TFrom>(this PageData<TFrom> pageData, Func<TFrom, T> selector)
        {
            IList<int> aa = new List<int>();
            aa.ToArray();
            T[] newItems = pageData.Select(selector).ToArray();
            return new PageData<T>(newItems, pageData.TotalRecords);
        }

    }
}
