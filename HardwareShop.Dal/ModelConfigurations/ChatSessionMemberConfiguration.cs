using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Dal.ModelConfigurations
{
    public sealed class ChatSessionMemberConfiguration : IEntityTypeConfiguration<ChatSessionMember>
    {


        public void Configure(EntityTypeBuilder<ChatSessionMember> c)
        {
            c.HasKey(e => new { e.SessionId, e.UserId });
            c.HasOne(e => e.Session).WithMany(e => e.Members).HasForeignKey(e => e.SessionId).OnDelete(DeleteBehavior.Cascade);
            c.HasOne(e => e.User).WithMany(e => e.ChatSessionMembers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}