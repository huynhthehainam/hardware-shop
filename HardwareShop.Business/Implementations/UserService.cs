using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal;
using HardwareShop.Dal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Business.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserAsset> userAssetRepository;
        private readonly IJwtService jwtService;
        private readonly ICurrentUserService currentUserService;
        public UserService(IRepository<User> userRepository, IJwtService jwtService, ICurrentUserService currentUserService, IRepository<UserAsset> userAssetRepository)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
            this.userAssetRepository = userAssetRepository;
        }

        public async Task<CreatedUserDto> CreateUserAsync(string username, string password)
        {
            return new CreatedUserDto { Id = 1 };
        }

        private async Task<IAssetTable?> getUserAvatarByUserId(int userId)
        {
            var user = await userRepository.GetItemByQueryAsync(e => e.Id == userId);
            if (user == null) return null;

            var userAsset = await userAssetRepository.GetItemByQueryAsync(e => e.UserId == user.Id
&& e.AssetType == UserAssetConstants.AvatarAssetType);
            return userAsset;
        }
        public Task<IAssetTable?> GetCurrentUserAvatarAsync()
        {
            var userId = currentUserService.GetUserId();
            return getUserAvatarByUserId(userId);
        }

        public async Task<List<UserDto>> GetUserDtosAsync()
        {
            PageData<User> userPageData = await userRepository.GetPageDataByQueryAsync(new PagingModel { PageIndex = 0, PageSize = 5 }, e => true, new List<QueryOrder<User>>() { new QueryOrder<User>(e => e.Username, true), new QueryOrder<User>(e => e.HashedPassword, false) });
            return userPageData.Items.Select(e => new UserDto() { Id = e.Id }).ToList();
        }

        public async Task<LoginResponse?> Login(string username, string password)
        {
            var user = await userRepository.GetItemByQueryAsync(e => e.Username == username);
            if (user == null) return null;
            var cacheUser = new CacheUser()
            {
                Id = user.Id,
                Username = user.Username ?? "",
                Role = user.Role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Settings = user.InterfaceSettings,
            };
            var response = jwtService.GenerateTokens(cacheUser);
            return response;
        }
    }
}
