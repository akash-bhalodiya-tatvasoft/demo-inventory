using Inventory.Models.Category;
using Inventory.Models.Entities;
using Inventory.Services;
using Inventory.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventory.Tests.Services;

public class CategoryServiceTests
{
    private CategoryService CreateService(string dbName)
    {
        var context = DbContextHelper.CreateContext(dbName);
        return new CategoryService(context);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Category()
    {
        var service = CreateService(nameof(CreateAsync_Should_Create_Category));
        var request = new CategoryRequest
        {
            Name = "Electronics",
            Description = "Electronic items"
        };

        var categoryId = await service.CreateAsync(request, userId: 1);

        Assert.True(categoryId > 0);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Category_When_Exists()
    {
        var service = CreateService(nameof(GetByIdAsync_Should_Return_Category_When_Exists));

        var id = await service.CreateAsync(new CategoryRequest
        {
            Name = "Books"
        }, null);

        var category = await service.GetByIdAsync(id);

        Assert.NotNull(category);
        Assert.Equal("Books", category!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        var service = CreateService(nameof(GetByIdAsync_Should_Return_Null_When_Not_Exists));

        var category = await service.GetByIdAsync(999);

        Assert.Null(category);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_By_Search()
    {
        var service = CreateService(nameof(GetAllAsync_Should_Filter_By_Search));

        await service.CreateAsync(new CategoryRequest { Name = "Electronics" }, null);
        await service.CreateAsync(new CategoryRequest { Name = "Books" }, null);

        var result = await service.GetAllAsync("elect");

        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Category_When_Exists()
    {
        var service = CreateService(nameof(UpdateAsync_Should_Update_Category_When_Exists));

        var id = await service.CreateAsync(new CategoryRequest
        {
            Name = "Old Name"
        }, null);

        var updateRequest = new CategoryRequest
        {
            Name = "New Name",
            Description = "Updated",
            IsActive = false
        };

        var result = await service.UpdateAsync(id, updateRequest, userId: 2);
        var updated = await service.GetByIdAsync(id);

        Assert.True(result);
        Assert.Equal("New Name", updated!.Name);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_When_Not_Exists()
    {
        var service = CreateService(nameof(UpdateAsync_Should_Return_False_When_Not_Exists));

        var result = await service.UpdateAsync(999, new CategoryRequest
        {
            Name = "Test"
        }, null);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Category_When_Exists()
    {
        var service = CreateService(nameof(DeleteAsync_Should_Remove_Category_When_Exists));

        var id = await service.CreateAsync(new CategoryRequest
        {
            Name = "To Delete"
        }, null);

        var result = await service.DeleteAsync(id);
        var deleted = await service.GetByIdAsync(id);

        Assert.True(result);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Not_Exists()
    {
        var service = CreateService(nameof(DeleteAsync_Should_Return_False_When_Not_Exists));

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}
