﻿namespace HardwareShop.Application.Models
{
    public class PageData<T>
    {
        public T[] Items { get; set; }
        public int TotalRecords { get; set; }
        public PageData(T[] items, int count)
        {
            Items = items;
            TotalRecords = count;
        }
        public static PageData<T> EmptyPageData()
        {
            return new PageData<T>(Array.Empty<T>(), 0);
        }
    }
    public static class PageDataExtensions
    {
        public static PageData<T> ConvertToOtherPageData<T, TFrom>(this PageData<TFrom> pageData, Func<TFrom, T> selector)
        {
            T[] newItems = pageData.Items.Select(selector).ToArray();
            return new PageData<T>(newItems, pageData.TotalRecords);
        }

    }
}