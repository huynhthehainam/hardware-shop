using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {

        public void Configure(EntityTypeBuilder<ChatMessage> cm)
        {
            cm.HasKey(e => e.Id);
            cm.HasOne(e => e.Member).WithMany(e => e.Messages).HasForeignKey(e => new { e.SessionId, e.UserId }).OnDelete(DeleteBehavior.Cascade);
            cm.HasOne(e => e.User).WithMany(e => e.Messages).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            cm.HasOne(e => e.Session).WithMany(e => e.Messages).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}