using Microsoft.EntityFrameworkCore;

namespace URLShortener.Tests;
using Data;
using Models;
using Services;

public class AboutServiceTests
{
    [Fact]
    public async Task GetAboutContentAsync_NoContentExists_ShouldCreateDefaultContent()
    {
        // --arrange
        // fake in-Memory Database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_About_Default")
            .Options;

        await using var context = new AppDbContext(options);
        var service = new AboutService(context);

        // --action
        var result = await service
            .GetAboutContentAsync(CancellationToken.None);

        // --assert
        Assert.NotNull(result);
        Assert.Contains("It creates a unique 6-character code from a set of 62 characters (A-Z, a-z, 0-9), that gives over 56 billion possible combinations", result.Description); // Verify default text part
        
        // check if saved to db
        var savedContent = await context
            .AboutContents
            .FirstOrDefaultAsync();
        
        Assert.NotNull(savedContent);
        Assert.Equal(result.Description, savedContent.Description);
    }

    [Fact]
    public async Task GetAboutContentAsync_ContentExists_ShouldReturnExistingContent()
    {
        // --arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_About_Existing")
            .Options;

        await using var context = new AppDbContext(options);
        
        // seed existing content
        context.AboutContents.Add(new AboutContent
        {
            Description = "Existing Description"
        });
        await context.SaveChangesAsync();

        var service = new AboutService(context);

        // --action
        var result = await service
            .GetAboutContentAsync(CancellationToken.None);

        // --assert
        Assert.NotNull(result);
        Assert.Equal("Existing Description", result.Description);
    }

    [Fact]
    public async Task UpdateAboutContentAsync_ContentExists_ShouldUpdateDescription()
    {
        // --arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_About_Update")
            .Options;

        await using var context = new AppDbContext(options);

        // seed existing content
        context.AboutContents.Add(new AboutContent
        {
            Description = "Old Description"
        });
        await context.SaveChangesAsync();

        var service = new AboutService(context);

        // --action
        await service
            .UpdateAboutContentAsync("New Updated Description", CancellationToken.None);

        // --assert
        var updatedContent = await context.AboutContents.FirstOrDefaultAsync();
        Assert.NotNull(updatedContent);
        Assert.Equal("New Updated Description", updatedContent.Description);
    }
}