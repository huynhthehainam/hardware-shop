﻿using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

        public UnitCategory(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        private ICollection<Unit>? units;
        public ICollection<Unit>? Units
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref units) : units;
            set => units = value;
        }

    }
}