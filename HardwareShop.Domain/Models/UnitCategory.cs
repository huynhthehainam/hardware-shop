using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;

namespace HardwareShop.Domain.Models
{
    public static class UnitCategoryConstants
    {
        public const string CurrencyCategoryName = "Currency";
    }
    public sealed class UnitCategory : EntityBase
    {
        public UnitCategory()
        {
        }

        public UnitCategory(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        private ICollection<Unit>? units;
        public ICollection<Unit>? Units
        {
            get => lazyLoader?.Load(this, ref units);
            set => units = value;
        }

    }
}
