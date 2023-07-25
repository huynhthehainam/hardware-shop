
using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ChatSessionConfiguration : ModelConfigurationBase<ChatSession>
    {
        public ChatSessionConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = e =>
            {
                e.HasKey(e => e.Id);
                e.HasOne(e => e.Asset).WithMany(e => e.ChatSessions).HasForeignKey(e => e.AssetId).OnDelete(DeleteBehavior.Cascade);
                
            };
        }
    }
}