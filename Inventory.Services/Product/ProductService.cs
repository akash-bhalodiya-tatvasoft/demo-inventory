using System.Data;
using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.Product;
using Inventory.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Inventory.Services.Implementations;

public class ProductService : IProductService
{
    private readonly ADODbContext _adoDbContext;

    public ProductService(ADODbContext adoDbContext)
    {
        _adoDbContext = adoDbContext;
    }

    public async Task<IEnumerable<Product?>> GetAllAsync()
    {
        const string query = @"SELECT * FROM ""Products""";

        var product = await _adoDbContext.ExecuteQueryGetList<Product>(query, null);
        return product;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string query = @"SELECT * FROM ""Products"" WHERE ""Id"" = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "Id", id },
        };

        var product = await _adoDbContext.ExecuteQueryGetObject<Product>(query, parameters);
        return product;
    }

    public async Task<int> CreateAsync(ProductRequest product, int? userId)
    {
        const string query = @"
            INSERT INTO ""Products"" 
                (""Name"", ""Description"", ""Price"", ""Quantity"", ""DiscountedPrice"", ""DiscountEndOn"", ""CategoryId"", ""CreatedAt"", ""CreatedBy"")
            VALUES 
                (@Name, @Description, @Price, @Quantity, @DiscountedPrice, @DiscountEndOn, @CategoryId, @CreatedAt, @CreatedBy)
            RETURNING ""Id"";
        ";

        var parameters = new Dictionary<string, object>
        {
            { "Name", product.Name },
            { "Description", product.Description },
            { "Price", product.Price },
            { "Quantity", product.Quantity },
            { "DiscountedPrice", product.DiscountedPrice },
            { "DiscountEndOn", product.DiscountEndOn ?? (object)DBNull.Value },
            { "CategoryId", product.CategoryId },
            { "CreatedAt", DateTime.UtcNow },
            { "CreatedBy", userId ?? (object)DBNull.Value },
        };

        var createdProductId = await _adoDbContext.ExecuteQuery<int>(query, parameters);
        return createdProductId;
    }

    public async Task<bool> UpdateAsync(int id, ProductRequest product, int? userId)
    {
        var existingProduct = await GetByIdAsync(id);

        if (existingProduct == null)
        {
            return false;
        }

        const string query = @"
            UPDATE ""Products"" SET
                ""Name"" = @Name,
                ""Description"" = @Description,
                ""Price"" = @Price,
                ""Quantity"" = @Quantity,
                ""DiscountedPrice"" = @DiscountedPrice,
                ""DiscountEndOn"" = @DiscountEndOn,
                ""CategoryId"" = @CategoryId,
                ""ModifiedAt"" = @ModifiedAt,
                ""ModifiedBy"" = @ModifiedBy
            WHERE ""Id"" = @Id RETURNING ""Id"";
        ";

        var parameters = new Dictionary<string, object>
        {
            { "Id", id },
            { "Name", product.Name },
            { "Description", product.Description },
            { "Price", product.Price },
            { "Quantity", product.Quantity },
            { "DiscountedPrice", product.DiscountedPrice },
            { "DiscountEndOn", product.DiscountEndOn ?? (object)DBNull.Value },
            { "CategoryId", product.CategoryId },
            { "ModifiedAt", DateTime.UtcNow },
            { "ModifiedBy", userId ?? (object)DBNull.Value },
        };

        var productId = await _adoDbContext.ExecuteQuery<int>(query, parameters);
        return productId > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id RETURNING ""Id""";
        var parameters = new Dictionary<string, object>
        {
            { "Id", id },
        };

        var result = await _adoDbContext.ExecuteQuery<int>(query, parameters);
        return result > 0;
    }
}
