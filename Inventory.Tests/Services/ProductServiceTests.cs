using Inventory.Common.Helpers;
using Inventory.Context;
using Inventory.Models.Product;
using Inventory.Services;
using Inventory.Services.Interfaces;
using Inventory.Tests.Mock;
using Moq;
using Xunit;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_Returns_Products_From_Cache()
    {
        var cachedProducts = new List<ProductResponse?>
        {
            new ProductResponse
            {
                Id = 1,
                Name = "Laptop",
                Price = 1000,
                Quantity = 5
            }
        };

        var dbMock = new Mock<IADODbContext>();

        var cacheMock = new Mock<IMemoryCacheService>();
        cacheMock
            .Setup(x => x.GetAsync<IEnumerable<ProductResponse?>>("Product"))
            .ReturnsAsync(cachedProducts);

        var service = new ProductService(dbMock.Object, cacheMock.Object);

        var result = await service.GetAllAsync("lap");

        var product = Assert.Single(result);
        Assert.Equal("Laptop", product!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_Product()
    {
        var dbMock = new Mock<IADODbContext>();
        var cacheMock = new Mock<IMemoryCacheService>();

        dbMock
            .Setup(x => x.ExecuteQueryGetObject<ProductResponse>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(new ProductResponse
            {
                Id = 10,
                Name = "Phone",
                Price = 500
            });

        var service = new ProductService(dbMock.Object, cacheMock.Object);

        var product = await service.GetByIdAsync(10);

        Assert.NotNull(product);
        Assert.Equal("Phone", product!.Name);
    }

    [Fact]
    public async Task CreateAsync_Returns_New_Product_Id()
    {
        var dbMock = new Mock<IADODbContext>();
        var cacheMock = new Mock<IMemoryCacheService>();

        dbMock
            .Setup(x => x.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(100);

        var service = new ProductService(dbMock.Object, cacheMock.Object);

        var request = new ProductRequest
        {
            Name = "Tablet",
            Price = 300,
            Quantity = 10,
            EnvryptedCategoryId = EncryptionHelper.EncryptId("1")
        };

        var id = await service.CreateAsync(request, 1);

        Assert.Equal(100, id);
    }

    [Fact]
    public async Task DeleteAsync_Returns_True_When_Deleted()
    {
        var dbMock = new Mock<IADODbContext>();
        var cacheMock = new Mock<IMemoryCacheService>();

        dbMock
            .Setup(x => x.ExecuteQueryGetObject<ProductResponse>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(new ProductResponse { Id = 1 });

        dbMock
            .Setup(x => x.ExecuteQuery<int>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .ReturnsAsync(1);

        var service = new ProductService(dbMock.Object, cacheMock.Object);

        var result = await service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task AddOfferAsync_Executes_Procedure()
    {
        var dbMock = new Mock<IADODbContext>();
        var cacheMock = new Mock<IMemoryCacheService>();

        dbMock
            .Setup(x => x.ExecuteProcedure(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
            .Returns(Task.CompletedTask);

        var service = new ProductService(dbMock.Object, cacheMock.Object);

        await service.AddOfferAsync(1, new ProductOfferRequest
        {
            DiscountedPrice = 50,
            DiscountEndOn = DateOnly.FromDateTime(DateTime.Today)
        }, 1);

        dbMock.Verify(x =>
            x.ExecuteProcedure(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()),
            Times.Once);
    }
}
