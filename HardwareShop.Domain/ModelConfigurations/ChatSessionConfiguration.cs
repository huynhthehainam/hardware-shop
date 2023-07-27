using HardwareShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Domain.ModelConfigurations
{
    public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
      

        public void Configure(EntityTypeBuilder<ChatSession> e)
        {
            e.HasKey(e => e.Id);
                e.HasOne(e => e.Asset).WithMany(e => e.ChatSessions).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}