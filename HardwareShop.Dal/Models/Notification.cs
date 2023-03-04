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
        public bool IsDismissed { get; set; } = false;
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public static void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(e =>
                {
                    e.HasKey(e => e.Id);
                    e.HasOne(e => e.User).WithMany(e => e.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}