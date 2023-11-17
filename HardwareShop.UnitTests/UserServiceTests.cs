using HardwareShop.Application.Services;
using Moq;

namespace HardwareShop.UnitTests;

public class UserServiceTests
{
    private readonly IUserService userService;
    private const string dumpAccessToken = "dumpToken";
    private const string dumpRefreshToken = "dumpRefreshToken";
    private const string dumpSessionId = "sessionId";
    public UserServiceTests()
    {

        //         Mock<DbContext> mockDb = new Mock<DbContext>();
        //         var dumpHashedPassword = "hashedPassword";
        //         var mockUsers = new List<User>() { new User() { Username = "admin", HashedPassword = dumpHashedPassword } };
        //         mockDb.Setup(e => e.Set<User>()).ReturnsDbSet(mockUsers);
        //         var db = mockDb.Object;
        //         var cacheOptions = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        //         Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
        //         var distributedCache = mockDistributedCache.Object;
        //         Mock<IJwtService> mockJwtService = new Mock<IJwtService>();
        //         mockJwtService.Setup(e => e.GenerateTokens(It.Is<CacheUser>(e => e.Username == "admin"))).Returns(new LoginResponse(
        // dumpAccessToken, dumpRefreshToken, dumpSessionId
        //         ));
        //         var jwtService = mockJwtService.Object;
        //         Mock<ICurrentUserService> mockCurrentUserService = new();

        //         ICurrentUserService currentUserService = mockCurrentUserService.Object;

        //         Mock<ILanguageService> mockLanguageService = new Mock<ILanguageService>();
        //         ILanguageService languageService = mockLanguageService.Object;

        //         IResponseResultBuilder responseResultBuilder = new ResponseResultBuilder(languageService);
        //         Mock<IHashingPasswordService> mockHashingPasswordService = new Mock<IHashingPasswordService>();
        //         mockHashingPasswordService.Setup(e => e.Verify("123", dumpHashedPassword)).Returns(true);
        //         IHashingPasswordService hashingPasswordService = mockHashingPasswordService.Object;
        //         IShopService shopService = new Mock<IShopService>().Object;
        // userService = new UserService(db, jwtService, currentUserService, responseResultBuilder, languageService, hashingPasswordService, shopService, distributedCache);
        userService = new Mock<IUserService>().Object;
    }
    [Theory]
    [InlineData("admin", "123")]
    public async Task LoginUser_InvalidUsername_ReturnLoginResponse(string username, string password)
    {
        var result = await userService.LoginAsync(username, password);
        Assert.NotNull(result);
        Assert.Equal(dumpAccessToken, result.AccessToken);
        Assert.Equal(dumpSessionId, result.SessionId);

    }
}