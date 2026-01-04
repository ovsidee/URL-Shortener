using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using URLShortener.Data;
using URLShortener.Models;
using URLShortener.Services;

namespace URLShortener.Tests;

public class UrlApiServiceTests
{
    [Fact]
    public async Task AddUrlAsync_ShouldAddUrlToDatabase_ShouldSucceed()
    {
        // --arrange
        // fake in-Memory Database 
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_AddUrl") 
            .Options;

        // create fake dbContext
        await using var context = new AppDbContext(options);

        // fake userManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // when ask for "GetUserAsync", return a fake user "TestUser", because in AddUrlAsync we use GetUserAsync method
        var fakeUser = new IdentityUser { UserName = "TestUser" };
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(fakeUser);

        // fake service that we are testing
        var service = new UrlApiService(context, userManagerMock.Object);

        // --action
        var result = await service
            .AddUrlAsync("https://www.test.com", new ClaimsPrincipal(), CancellationToken.None);

        // --assert
        Assert.NotNull(result);
        Assert.Equal("https://www.test.com", result.OriginalUrl);
        Assert.Equal("TestUser", result.CreatedBy);
        Assert.Equal(6, result.ShortCode.Length); // Ensure code is 6 chars
        
        // check if present in db
        var savedUrl = await context
            .ShortUrls
            .FirstOrDefaultAsync();
        
        Assert.NotNull(savedUrl);
        Assert.Equal("https://www.test.com", savedUrl.OriginalURL);
    }
    
    [Fact]
    public async Task AddUrlAsync_DuplicateUrl_ShouldThrowException()
    {
        // --arrange
        // fake in-Memory Database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_AddDuplicate")
            .Options;

        await using var context = new AppDbContext(options);

        // seed existing data manually
        context.ShortUrls.Add(new ShortUrl
        {
            OriginalURL = "https://www.duplicate.com",
            CreatedBy = "SomeUser",
            ShortCode = "123456",
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // fake userManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        var fakeUser = new IdentityUser { UserName = "TestUser" };
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(fakeUser);

        var service = new UrlApiService(context, userManagerMock.Object);

        // --action & assert
        // expect an error because "https://www.duplicate.com" exists
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service
                .AddUrlAsync("https://www.duplicate.com", new ClaimsPrincipal(), CancellationToken.None)
        );
    }

    [Fact]
    public async Task DeleteUrlAsync_OwnerDeletingOwnUrl_ShouldSucceed()
    {
        // --arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteOwn")
            .Options;

        await using var context = new AppDbContext(options);

        // seed a url created by "OwnerUser"
        var urlId = 1;
        context.ShortUrls.Add(new ShortUrl
        {
            Id = urlId,
            OriginalURL = "https://www.google.com",
            CreatedBy = "OwnerUser",
            ShortCode = "ABCDEF",
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // fake userManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // fake user is the Owner
        var fakeUser = new IdentityUser { UserName = "OwnerUser" };
        
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(fakeUser);
        
        // mock Role check (not admin)
        userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, "Admin"))
            .ReturnsAsync(false);

        var service = new UrlApiService(context, userManagerMock.Object);

        // --action
        var result = await service
            .DeleteUrlAsync(urlId, new ClaimsPrincipal(), CancellationToken.None);

        // --assert
        Assert.True(result);

        // check if actually deleted from db
        var deletedUrl = await context.ShortUrls.FindAsync(urlId);
        Assert.Null(deletedUrl);
    }

    [Fact]
    public async Task DeleteUrlAsync_UserDeletingOthersUrl_ShouldThrowUnauthorized()
    {
        // --arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteOther")
            .Options;

        await using var context = new AppDbContext(options);

        // seed a url created by "DifferentUser"
        var urlId = 2;
        context.ShortUrls.Add(new ShortUrl
        {
            Id = urlId,
            OriginalURL = "https://www.yahoo.com",
            CreatedBy = "DifferentUser", 
            ShortCode = "XYZ123",
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // fake userManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // fake user is "HackerUser" (Not the owner)
        var fakeUser = new IdentityUser { UserName = "HackerUser" };

        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(fakeUser);

        // mock Role check (not admin)
        userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, "Admin"))
            .ReturnsAsync(false);

        var service = new UrlApiService(context, userManagerMock.Object);

        // --action & assert
        // Should fail because HackerUser != DifferentUser
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () =>
            await service
                .DeleteUrlAsync(urlId, new ClaimsPrincipal(), CancellationToken.None)
        );
    }
    
    [Fact]
    public async Task DeleteUrlAsync_AdminDeletingOthersUrl_ShouldSucceed()
    {
        // --arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_DeleteAdmin")
            .Options;

        await using var context = new AppDbContext(options);

        // seed a url created by "RegularUser"
        var urlId = 3;
        context.ShortUrls.Add(new ShortUrl
        {
            Id = urlId,
            OriginalURL = "https://www.bing.com",
            CreatedBy = "RegularUser",
            ShortCode = "ADMIN1",
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // fake userManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // fake user is "SuperAdmin" (Not owner, but is Admin)
        var fakeUser = new IdentityUser { UserName = "SuperAdmin" };

        //mock user privileges
        userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(fakeUser);
        
        userManagerMock.Setup(x => x.IsInRoleAsync(fakeUser, "Admin"))
            .ReturnsAsync(true);

        var service = new UrlApiService(context, userManagerMock.Object);

        // --action
        var result = await service
            .DeleteUrlAsync(urlId, new ClaimsPrincipal(), CancellationToken.None);

        // --assert
        Assert.True(result);
        
        // verify deletion
        var deletedUrl = await context
            .ShortUrls
            .FindAsync(urlId);
        
        Assert.Null(deletedUrl);
    }
}