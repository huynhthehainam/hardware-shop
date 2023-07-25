
using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ChatMessageStatusConfiguration : ModelConfigurationBase<ChatMessageStatus>
    {
        public ChatMessageStatusConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = mt =>
            {
                mt.HasKey(e => new { e.MessageId, e.SessionId, e.UserId });
                mt.HasOne(e => e.Member).WithMany(e => e.MessageStatuses).HasForeignKey(e => new { e.SessionId, e.UserId }).OnDelete(DeleteBehavior.Cascade);
                mt.HasOne(e => e.User).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                mt.HasOne(e => e.Session).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
                mt.HasOne(e => e.Message).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.MessageId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}