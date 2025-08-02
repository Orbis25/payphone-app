using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Payphone.Application.Dtos.Users;
using Payphone.Application.Options;
using Payphone.Application.Services.Users;
using Payphone.Domain.Models;
using Xunit;

namespace Payphone.Tests;

public class UserServiceTest
{
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IOptions<JwtOption>> _jwtOptionsMock = new();


    public UserServiceTest()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock =
            new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            userClaimsPrincipalFactoryMock.Object,
            null, null, null, null
        );
        _jwtOptionsMock.Setup(x => x.Value).Returns(new JwtOption
        {
            Key = "testkeytestkeytestkeytestkeyxasxasxasxasxasxasxasxasxasxasxasxasxasxasxas121231231212123131321231231231231231",
            Issuer = "testissuer",
            Audience = "testaudience",
            DefaultUser = "default@payphone.com",
            DefaultPassword = "Default123!"
        });
    }


    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        var user = new User { Id = "1", Email = "test@payphone.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, It.IsAny<string>(), false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var service = new UserService(_loggerMock.Object, _userManagerMock.Object, _signInManagerMock.Object,
            _jwtOptionsMock.Object);

        var result = await service.LoginAsync(new LoginDto { Email = "test@payphone.com", Password = "password" });

        Assert.NotNull(result);
        Assert.Equal("1", result.Data.UserId);
        Assert.False(string.IsNullOrEmpty(result.Data.Jwt));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
    {
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        var service = new UserService(_loggerMock.Object, _userManagerMock.Object, _signInManagerMock.Object,
            _jwtOptionsMock.Object);

        var result = await service.LoginAsync(new LoginDto { Email = "notfound@payphone.com", Password = "password" });

        Assert.Null(result.Data);
        Assert.Equal("invalid email/password", result.Message);
    }

    [Fact]
    public async Task SeedAsync_ShouldCreateDefaultUser_WhenNoUsersExist()
    {
        _userManagerMock.Setup(x => x.Users).Returns(new List<User>().AsQueryable());
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var service = new UserService(_loggerMock.Object, _userManagerMock.Object, _signInManagerMock.Object,
            _jwtOptionsMock.Object);

        await service.SeedAsync();

        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SeedAsync_ShouldNotCreateUser_WhenUsersExist()
    {
        _userManagerMock.Setup(x => x.Users).Returns(new List<User> { new User() }.AsQueryable());

        var service = new UserService(_loggerMock.Object, _userManagerMock.Object, _signInManagerMock.Object,
            _jwtOptionsMock.Object);

        await service.SeedAsync();

        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }
}