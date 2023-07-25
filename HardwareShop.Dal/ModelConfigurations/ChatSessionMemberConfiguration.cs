using HardwareShop.Core.Bases;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ChatSessionMemberConfiguration : ModelConfigurationBase<ChatSessionMember>
    {
        public ChatSessionMemberConfiguration(ModelBuilder modelBuilder) : base(modelBuilder)
        {
            buildAction = c =>
            {
                c.HasKey(e => new { e.SessionId, e.UserId });
                c.HasOne(e => e.Session).WithMany(e => e.Members).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
                c.HasOne(e => e.User).WithMany(e => e.ChatSessionMembers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            };
        }
    }
}