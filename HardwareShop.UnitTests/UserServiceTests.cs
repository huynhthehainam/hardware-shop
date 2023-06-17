using HardwareShop.Business.Implementations;
using HardwareShop.Business.Services;
using HardwareShop.Core.Implementations;
using HardwareShop.Core.Models;
using HardwareShop.Core.Services;
using HardwareShop.Dal.Models;
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
    private const string dumpAccessToken = "dumpToken";
    private const string dumpRefreshToken = "dumpRefreshToken";
    private const string dumpSessionId = "sessid";
    public UserServiceTests()
    {

        Mock<DbContext> mockDb = new Mock<DbContext>();
        var dumpHashedPassword = "hashedPassword";
        var mockUsers = new List<User>() { new User() { Username = "admin", HashedPassword = dumpHashedPassword } };
        mockDb.Setup(e => e.Set<User>()).ReturnsDbSet(mockUsers);
        var db = mockDb.Object;
        var cacheOptions = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
        var distributedCache = mockDistributedCache.Object;
        Mock<IJwtService> mockJwtService = new Mock<IJwtService>();
        mockJwtService.Setup(e => e.GenerateTokens(It.Is<CacheUser>(e => e.Username == "admin"))).Returns(new LoginResponse(
dumpAccessToken, dumpRefreshToken, dumpSessionId
        ));
        var jwtService = mockJwtService.Object;
        Mock<ICurrentUserService> mockCurrentUserService = new();

        ICurrentUserService currentUserService = mockCurrentUserService.Object;

        Mock<ILanguageService> mockLanguageService = new Mock<ILanguageService>();
        ILanguageService languageService = mockLanguageService.Object;

        IResponseResultBuilder responseResultBuilder = new ResponseResultBuilder(languageService);
        Mock<IHashingPasswordService> mockHashingPasswordService = new Mock<IHashingPasswordService>();
        mockHashingPasswordService.Setup(e => e.Verify("123", dumpHashedPassword)).Returns(true);
        IHashingPasswordService hashingPasswordService = mockHashingPasswordService.Object;
        IShopService shopService = new Mock<IShopService>().Object;
        userService = new UserService(db, jwtService, currentUserService, responseResultBuilder, languageService, hashingPasswordService, shopService, distributedCache);
    }
    [Theory]
    [InlineData("admin", "123")]
    public async Task Test1(string username, string password)
    {
        var result = await userService.LoginAsync(username, password);
        Assert.NotNull(result);
        Assert.Equal(result.AccessToken, dumpAccessToken);
        Assert.Equal(result.SessionId, dumpSessionId);

    }
}