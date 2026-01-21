using Inventory.Models.Product;
using Inventory.Services.Interfaces;
using Moq;

namespace Inventory.Tests.Mock;

public static class MemoryCacheMock
{
    public static IMemoryCacheService Create()
    {
        var mock = new Mock<IMemoryCacheService>();

        mock.Setup(x => x.GetAsync<IEnumerable<ProductResponse?>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<ProductResponse?>?)null);

        mock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
            .Returns(Task.CompletedTask);

        mock.Setup(x => x.RemoveAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        return mock.Object;
    }
}
