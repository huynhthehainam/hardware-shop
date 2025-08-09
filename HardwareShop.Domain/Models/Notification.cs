using System.Text.Json;
using HardwareShop.Domain.Abstracts;
using HardwareShop.Domain.Extensions;
using HardwareShop.Domain.Interfaces;

namespace HardwareShop.Domain.Models
{
    public class Notification : EntityBase, ITrackingDate
    {
        public Notification()
        {
        }

        public Notification(Action<object, string?> lazyLoader) : base(lazyLoader)
        {
        }
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? Variant { get; set; }

        private User? user;
        public User? User
        {
            get => lazyLoader?.Load(this, ref user);
            set => user = value;
        }

        public string? Translation { get; set; }
        public String? TranslationParams { get; set; }
        public bool IsDismissed { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
    }
}