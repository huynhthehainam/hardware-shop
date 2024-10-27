using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Infrastructure.ModelConfigurations
{
    public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        public void Configure(EntityTypeBuilder<ChatSession> e)
        {
            e.HasKey(chatSession => chatSession.Id);
            e.HasOne(chatSession => chatSession.Asset).WithMany(asset => asset.ChatSessions)
                .HasForeignKey(chatSession => chatSession.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}