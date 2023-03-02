using HardwareShop.Core.Bases;
using HardwareShop.Core.Services;

namespace HardwareShop.Core.Models
{
    public class SortingModel
    {
        public string? SortFieldName { get; set; }
        public bool? IsSortAscending { get; set; }

        public List<QueryOrder<T>> ToSearchQueries<T>() where T : EntityBase
        {
            if (SortFieldName == null || IsSortAscending == null) return new List<QueryOrder<T>>();
            var properties = typeof(T).GetProperties();
            var selectedProperty = properties.FirstOrDefault(e => e.Name.ToLower() == SortFieldName.ToLower() && (e.PropertyType.IsPrimitive || e.PropertyType == typeof(string)));

            if (selectedProperty == null)
            {
                return new List<QueryOrder<T>>();
            }
            return new List<QueryOrder<T>>()
            {
                new QueryOrder<T>(e=>selectedProperty.GetValue(e), IsSortAscending ?? false)
            };
        }
    }
}