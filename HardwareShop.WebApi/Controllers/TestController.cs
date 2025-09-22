

using HardwareShop.Application.Models;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Abstracts;
using HardwareShop.WebApi.Commands;
using HardwareShop.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HardwareShop.WebApi.Controllers
{
    public sealed class TestController : AuthorizedApiControllerBase
    {
        private readonly IChatService chatService;
        private readonly ITestService testService;
        public TestController(IChatService chatService, ITestService testService, IResponseResultBuilder responseResultBuilder, ICurrentUserService currentUserService) : base(responseResultBuilder, currentUserService)
        {
            this.chatService = chatService;
            this.testService = testService;
        }
        [HttpPost("TestWriteBack")]
        public async Task<IActionResult> TestWriteBack([FromBody] TestWriteBackCommand command)
        {
            await testService.TestWriteBackAsync();
            responseResultBuilder.SetData(new
            {
                Message = "Write-back test completed"
            });
            return responseResultBuilder.Build();
        }
        [HttpGet("TestEntity")]
        public async Task<IActionResult> TestEntity()
        {
            var data = await testService.TestEntityAsync();
            responseResultBuilder.SetData(new
            {
                Count = data
            });
            return responseResultBuilder.Build();
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
            var messageId = await chatService.CreateChatMessageAsync(command.ChatId, command.Content);
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