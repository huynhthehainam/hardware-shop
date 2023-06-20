

using Grpc.Core;
using HardwareShop.Core.Services;
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
            var aa = request.Id;
            return Task<UserGrpcModel>.FromResult(new UserGrpcModel { Email = "test@mail.com" });
        }
    }
}