
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Application.Models
{
    public class SortingModel
    {
        public string? SortFieldName { get; set; }
        public bool? IsSortAscending { get; set; }

        public OrderQuery<T>[] ToOrderQueries<T>() where T : EntityBase
        {
            if (SortFieldName == null || IsSortAscending == null) return Array.Empty<OrderQuery<T>>();
            var properties = typeof(T).GetProperties();
            var allowedTypes = new Type[] { typeof(string), typeof(DateTime) };
            var selectedProperty = properties.FirstOrDefault(e => e.Name.ToLower() == SortFieldName.ToLower() && (e.PropertyType.IsPrimitive || allowedTypes.Contains(e.PropertyType)));

            if (selectedProperty == null)
            {
                return Array.Empty<OrderQuery<T>>();
            }
            return new OrderQuery<T>[]
            {
                new OrderQuery<T>(e=>selectedProperty.GetValue(e), IsSortAscending ?? false)
            };
        }
    }
}