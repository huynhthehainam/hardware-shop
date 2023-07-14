using HardwareShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Dtos
{
    public sealed class CreatedChatSessionDto : ChatContactDto
    {
        public List<int> AffectedUserIds { get; set; } = new List<int>();
        public bool IsCreated { get; set; }
        public int CreatedUserId { get; set; }
        public PageData<ChatMessageDto> Messages { get; set; } = PageData<ChatMessageDto>.EmptyPageData();
    }
}
