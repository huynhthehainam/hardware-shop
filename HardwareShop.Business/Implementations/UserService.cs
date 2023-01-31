using HardwareShop.Business.Dtos;
using HardwareShop.Business.Services;
using HardwareShop.Core.Bases;
using HardwareShop.Core.Helpers;
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
        private readonly IResponseResultBuilder responseResultBuilder;
        private readonly ICurrentUserService currentUserService;
        private readonly ILanguageService languageService;
        public UserService(IRepository<User> userRepository, IJwtService jwtService, ICurrentUserService currentUserService, IRepository<UserAsset> userAssetRepository, IResponseResultBuilder responseResultBuilder, ILanguageService languageService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
            this.currentUserService = currentUserService;
            this.userAssetRepository = userAssetRepository;
            this.responseResultBuilder = responseResultBuilder;
            this.languageService = languageService;
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

        public async Task<LoginDto?> Login(string username, string password)
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
            };
            var tokens = jwtService.GenerateTokens(cacheUser);
            if (tokens == null) return null;
            var userShop = user.UserShop;


            return new LoginDto(tokens.AccessToken, new LoginUserDto(user.Role, new LoginUserDataDto(
                languageService.GenerateFullName(user.FirstName, user.LastName), user.Email, user.InterfaceSettings), userShop == null ? null : new LoginShopDto(
                    userShop.Shop != null ? (userShop.Shop.Name ?? "") : "", userShop.Role)));
        }
    }
}
