using Inventory.Models.Auth;
using Inventory.Models.Entities;
using Inventory.Services;
using Inventory.Services.Interfaces;
using Inventory.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Inventory.Tests.Services;

public class AuthServiceTests
{
    private AuthService CreateService(string dbName, out Mock<IEmailService> emailMock)
    {
        var context = DbContextHelper.CreateContext(dbName);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JWTToken:SecretKey", "Tsdajlkndsafhiehriwgntijnjndfgsdgest" },
                { "JWTToken:Issuer", "http://localhost:5115" }
            })
            .Build();

        emailMock = new Mock<IEmailService>();

        return new AuthService(context, config, emailMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User()
    {
        var service = CreateService(nameof(RegisterAsync_Should_Create_User), out _);

        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        var result = await service.RegisterAsync(request);

        Assert.True(result);
    }

    [Fact]
    public async Task RegisterAsync_Should_Return_False_When_Email_Exists()
    {
        var service = CreateService(nameof(RegisterAsync_Should_Return_False_When_Email_Exists), out _);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        var result = await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_Should_Send_Otp_When_Credentials_Are_Valid()
    {
        var service = CreateService(nameof(LoginAsync_Should_Send_Otp_When_Credentials_Are_Valid), out var emailMock);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        };

        var result = await service.LoginAsync(loginRequest);

        Assert.True(result);
        emailMock.Verify(x => x.SendOtpEmailAsync("test@test.com", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_False_When_Password_Invalid()
    {
        var service = CreateService(nameof(LoginAsync_Should_Return_False_When_Password_Invalid), out _);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "wrong"
        });

        Assert.False(result);
    }

    [Fact]
    public async Task VerifyOtpAsync_Should_Return_Tokens_When_Otp_Valid()
    {
        var service = CreateService(nameof(VerifyOtpAsync_Should_Return_Tokens_When_Otp_Valid), out _);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        await service.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        });

        var context = DbContextHelper.CreateContext(nameof(VerifyOtpAsync_Should_Return_Tokens_When_Otp_Valid));
        var user = await context.Users.FirstAsync();

        var response = await service.VerifyOtpAsync(user.Email, user.OtpCode!);

        Assert.NotNull(response);
        Assert.NotNull(response!.AccessToken);
        Assert.NotNull(response.RefreshToken);
    }

    [Fact]
    public async Task VerifyOtpAsync_Should_Lock_User_After_3_Failures()
    {
        var service = CreateService(nameof(VerifyOtpAsync_Should_Lock_User_After_3_Failures), out _);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        await service.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        });

        await service.VerifyOtpAsync("test@test.com", "111111");
        await service.VerifyOtpAsync("test@test.com", "111111");
        await service.VerifyOtpAsync("test@test.com", "111111");

        var context = DbContextHelper.CreateContext(nameof(VerifyOtpAsync_Should_Lock_User_After_3_Failures));
        var user = await context.Users.FirstAsync();

        Assert.NotNull(user.LockoutUntil);
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Tokens()
    {
        var service = CreateService(nameof(RefreshTokenAsync_Should_Return_New_Tokens), out _);

        await service.RegisterAsync(new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        });

        await service.LoginAsync(new LoginRequest
        {
            Email = "test@test.com",
            Password = "password"
        });

        var context = DbContextHelper.CreateContext(nameof(RefreshTokenAsync_Should_Return_New_Tokens));
        var user = await context.Users.FirstAsync();

        var verify = await service.VerifyOtpAsync(user.Email, user.OtpCode!);

        var refreshResponse = await service.RefreshTokenAsync(verify!.RefreshToken!);

        Assert.NotNull(refreshResponse);
        Assert.NotEqual(verify.RefreshToken, refreshResponse!.RefreshToken);
    }
}