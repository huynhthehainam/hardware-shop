using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;

namespace HardwareShop.UnitTests;

public class UserServiceTests
{
    private readonly IUserService userService;
    public UserServiceTests()
    {
        IOptions<JwtConfiguration> jwtOptions = Options.Create<JwtConfiguration>(new JwtConfiguration()
        {
            SecretKey = "Q5gNy2zerr0iO3vLFZcRO4uGR7aRYjDl",
            ExpiredDuration = 60
        });
        Mock<DbContext> mockDb = new Mock<DbContext>();
        var mockUsers = new List<User>() { new User() { Username = "admin", HashedPassword = "hashedPassword" } };
        mockDb.Setup(e => e.Set<User>()).ReturnsDbSet(mockUsers);
        var db = mockDb.Object;
        var cacheOptions = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        IDistributedCache distributedCache = new MemoryDistributedCache(cacheOptions);
        IJwtService jwtService = new JwtService(jwtOptions, distributedCache);
        Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
        ICurrentUserService currentUserService = new CurrentUserService(jwtService, httpContextAccessor.Object);
        ILanguageService languageService = new LanguageService(httpContextAccessor.Object);
        IResponseResultBuilder responseResultBuilder = new ResponseResultBuilder(languageService);
        Mock<IHashingPasswordService> mockHashingPasswordService = new Mock<IHashingPasswordService>();
        mockHashingPasswordService.Setup(e => e.Verify("123", "hashedPassword")).Returns(true);
        IHashingPasswordService hashingPasswordService = mockHashingPasswordService.Object;
        IShopService shopService = new Mock<IShopService>().Object;
        userService = new UserService(db, jwtService, currentUserService, responseResultBuilder, languageService, hashingPasswordService, shopService, distributedCache);
    }
    [Theory]
    [InlineData("admin", "123")]
    [InlineData("staff", "123")]
    public async Task Test1(string username, string password)
    {
        var result = await userService.LoginAsync(username, password);
        Assert.NotNull(result);
        Assert.Equal(result.AccessToken, "asfasf");
    }
}