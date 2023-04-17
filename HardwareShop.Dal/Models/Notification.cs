using System.Text.Json;
using HardwareShop.Core.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HardwareShop.Dal.Models
{
    public class Notification : EntityBase, ITrackingDate
    {
        public Notification()
        {
        }

        public Notification(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? Variant { get; set; }

        private User? user;
        public User? User
        {
            get => lazyLoader is not null ? lazyLoader.Load(this, ref user) : user;
            set => user = value;
        }

        public string? Translation { get; set; }
        public JsonDocument? TranslationParams { get; set; }
        public bool IsDismissed { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
    }
}