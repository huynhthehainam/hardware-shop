using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ChatMessageConfiguration : ModelConfigurationBase<ChatMessage>
    {
        public ChatMessageConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = cm =>
            {
                cm.HasKey(e => e.Id);
                cm.HasOne(e => e.Member).WithMany(e => e.Messages).HasForeignKey(e => new { e.SessionId, e.UserId }).OnDelete(DeleteBehavior.Cascade);
                cm.HasOne(e => e.User).WithMany(e => e.Messages).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                cm.HasOne(e => e.Session).WithMany(e => e.Messages).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}