using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Tests;

public class AccountServiceTests
{
    // helper method to mock the SignInManager dependencies
    private Mock<SignInManager<IdentityUser>> CreateSignInManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        
        return new Mock<SignInManager<IdentityUser>>(
            userManagerMock.Object, 
            contextAccessorMock.Object, 
            claimsFactoryMock.Object, 
            null, null, null, null);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnTrue()
    {
        // --arrange
        var signInManagerMock = CreateSignInManagerMock();
        
        // setup PasswordSignInAsync to return "Success"
        signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        var service = new AccountService(signInManagerMock.Object);
        var model = new LoginViewModel { Email = "test@test.com", Password = "Password123!" };

        // --action
        var result = await service
            .LoginAsync(model, CancellationToken.None);

        // --assert
        Assert.True(result);
        
        // verify method was called with correct params
        signInManagerMock.Verify(x => x.PasswordSignInAsync(
            model.Email, 
            model.Password, 
            false, // isPersistent
            false), // lockoutOnFailure
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ShouldReturnFalse()
    {
        // --arrange
        var signInManagerMock = CreateSignInManagerMock();

        // setup PasswordSignInAsync to return "Failed"
        signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>(), 
                It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        var service = new AccountService(signInManagerMock.Object);
        var model = new LoginViewModel { Email = "wrong@test.com", Password = "WrongPassword" };

        // --action
        var result = await service.LoginAsync(model, CancellationToken.None);

        // --assert
        Assert.False(result);
    }

    [Fact]
    public async Task LogoutAsync_ShouldCallSignOut()
    {
        // --arrange
        var signInManagerMock = CreateSignInManagerMock();
        
        // setup SignOutAsync
        signInManagerMock.Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        var service = new AccountService(signInManagerMock.Object);

        // --action
        await service.LogoutAsync(CancellationToken.None);

        // --assert
        signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }
}