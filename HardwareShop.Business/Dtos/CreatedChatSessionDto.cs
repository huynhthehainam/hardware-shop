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

        public bool IsCreated { get; set; }
        public Guid CreatedUserGuid { get; set; }
        public PageData<ChatMessageDto> Messages { get; set; } = PageData<ChatMessageDto>.EmptyPageData();
    }
}
