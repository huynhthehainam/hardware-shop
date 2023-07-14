

using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.WebApi.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class TestController : AuthorizedApiControllerBase
    {
        private readonly IChatService chatService;
        public TestController(IChatService chatService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.chatService = chatService;
        }
        [HttpGet("Contracts")]
        public async Task<IActionResult> GetContacts()
        {
            var contracts = await chatService.GetContactsOfCurrentUserAsync();
            if (contracts != null)
            {
                responseResultBuilder.SetData(contracts);
            }
            return responseResultBuilder.Build();
        }
        [HttpPost("CreateContact")]
        public async Task<IActionResult> CreateChatSession([FromBody] CreateChatSessionCommand command)
        {
            var chatId = await chatService.CreateChatSessionAsync(command.UserIds);
            if (chatId != null)
            {
                responseResultBuilder.SetData(chatId);
            }
            return responseResultBuilder.Build();
        }
        [HttpPost("CreateMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateChatMessageCommand command)
        {
            var messageId = await chatService.CreateChatMessage(command.ChatId, command.Content);
            if (messageId != null) responseResultBuilder.SetData(messageId);
            return responseResultBuilder.Build();
        }
        [HttpGet("ChatMessages")]
        public async Task<IActionResult> GetChatMessage([FromQuery] PagingModel pagingModel)
        {
            var messages = await chatService.GetMessagesAsync(1, pagingModel);
            if (messages != null)
            {
                responseResultBuilder.SetPageData(messages);
            }
            return responseResultBuilder.Build();
        }
    }
}