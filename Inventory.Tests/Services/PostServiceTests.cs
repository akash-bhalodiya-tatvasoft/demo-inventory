using Inventory.Models.Post;
using Inventory.Services;
using Inventory.Tests.Mock;
using Xunit;

namespace Inventory.Tests.Services;

public class PostServiceTests
{
    [Fact]
    public async Task GetAllAsync_Returns_Posts()
    {
        var apiResponse = new[]
        {
            new APIPostResponse
            {
                Id = 1,
                UserId = 10,
                Title = "Test Title",
                Body = "Test Body"
            }
        };

        var httpClient = HttpClientMock.Create(apiResponse);
        var service = new PostService(httpClient);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
        var post = Assert.Single(result);
        Assert.Equal("Test Title", post.Title);
        Assert.Equal("Test Body", post.Body);
        Assert.NotEmpty(post.EncryptedId);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Post()
    {
        var apiResponse = new APIPostResponse
        {
            Id = 2,
            UserId = 20,
            Title = "Single Post",
            Body = "Post Body"
        };

        var httpClient = HttpClientMock.Create(apiResponse);
        var service = new PostService(httpClient);

        var result = await service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Single Post", result!.Title);
        Assert.NotEmpty(result.EncryptedUserId);
    }

    [Fact]
    public async Task CreateAsync_Returns_New_Id()
    {
        var request = new PostRequest
        {
            EncryptedUserId = "encrypted",
            Title = "New Post",
            Body = "New Body"
        };

        var apiResponse = new PostResponse
        {
            Title = "New Post",
            Body = "New Body"
        };

        var httpClient = HttpClientMock.Create(apiResponse);
        var service = new PostService(httpClient);

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_Returns_True_On_Success()
    {
        var request = new PostRequest
        {
            EncryptedUserId = "encrypted",
            Title = "Updated",
            Body = "Updated Body"
        };

        var httpClient = HttpClientMock.Create(new { });
        var service = new PostService(httpClient);

        var result = await service.UpdateAsync(1, request);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_On_Success()
    {
        var httpClient = HttpClientMock.Create(new { });
        var service = new PostService(httpClient);

        var result = await service.DeleteAsync(1);

        Assert.True(result);
    }
}
