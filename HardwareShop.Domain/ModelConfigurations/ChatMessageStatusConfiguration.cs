using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class ChatMessageStatusConfiguration : IEntityTypeConfiguration<ChatMessageStatus>
    {
      

        public void Configure(EntityTypeBuilder<ChatMessageStatus> mt)
        {
            mt.HasKey(e => new { e.MessageId, e.SessionId, e.UserId });
            mt.HasOne(e => e.Member).WithMany(e => e.MessageStatuses).HasForeignKey(e => new { e.SessionId, e.UserId }).OnDelete(DeleteBehavior.Cascade);
            mt.HasOne(e => e.User).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            mt.HasOne(e => e.Session).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
            mt.HasOne(e => e.Message).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.MessageId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}