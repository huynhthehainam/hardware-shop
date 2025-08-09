

using Grpc.Core;
using HardwareShop.Application.Services;
using HardwareShop.WebApi.Protos;
using Microsoft.AspNetCore.Authorization;

namespace HardwareShop.WebApi.GrpcServices
{
    public sealed class UserGrpcService : HardwareShop.WebApi.Protos.HardwareShopGrpcService.HardwareShopGrpcServiceBase
    {
        private readonly ICurrentUserService currentUserService;
        public UserGrpcService(ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }
        [Authorize]
        public override Task<UserGrpcModel> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
        {
            var isAdmin = currentUserService.IsSystemAdmin();

            return Task<UserGrpcModel>.FromResult(new UserGrpcModel { Email = "test@mail.com" });
        }
        public override async Task TestStream(GetUserInfoRequest request, IServerStreamWriter<UserGrpcModel> responseStream, ServerCallContext context)
        {
            var rand = new Random();
            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested && i < 20)
            {
                i += 1;
                var index = rand.Next();
                await responseStream.WriteAsync(new UserGrpcModel()
                {
                    Email = $"test{index % 10}@mail.com"
                });
            }
            Console.WriteLine("Close stream");
        }
    }
}