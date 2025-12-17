using System.Data;
using Inventory.Context;
using Inventory.Models.Entities;
using Inventory.Models.Product;
using Inventory.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Inventory.Services;

public class ProductService : IProductService
{
    private readonly ADODbContext _adoDbContext;
    private readonly IMemoryCacheService _memoryCacheService;

    public ProductService(ADODbContext adoDbContext, IMemoryCacheService memoryCacheService)
    {
        _adoDbContext = adoDbContext;
        _memoryCacheService = memoryCacheService;
    }

    public async Task<IEnumerable<ProductResponse?>> GetAllAsync(string search)
    {

        var cachedProduct = await _memoryCacheService.GetAsync<IEnumerable<ProductResponse?>>("Product");

        if (cachedProduct == null)
        {
            search = search?.ToLower();

            string query = @"SELECT p.*, c.""Name"" as ""CategoryName"" FROM ""Products"" AS p INNER JOIN ""Categories"" AS c ON p.""CategoryId"" = c.""Id"" WHERE LOWER(p.""Name"") LIKE @Search OR LOWER(p.""Description"") LIKE @Search OR LOWER(c.""Name"") LIKE @Search";
            var parameters = new Dictionary<string, object>
        {
            { "Search", $"%{search}%" },
        };


            var product = await _adoDbContext.ExecuteQueryGetList<ProductResponse>(query, parameters);
            await _memoryCacheService.SetAsync("Product", product, TimeSpan.FromMinutes(60 * 24 * 90));
            return product;
        }
        else
        {
            return cachedProduct;
        }
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        string query = @"SELECT p.*, c.""Name"" as ""CategoryName"" FROM ""Products"" AS p INNER JOIN ""Categories"" AS c ON p.""CategoryId"" = c.""Id"" WHERE p.""Id"" = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "Id", id },
        };

        var product = await _adoDbContext.ExecuteQueryGetObject<ProductResponse>(query, parameters);
        return product;
    }

    public async Task<int> CreateAsync(ProductRequest product, int? userId)
    {
        string query = @"
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

        await _memoryCacheService.RemoveAsync("Product");

        return createdProductId;
    }

    public async Task<bool> UpdateAsync(int id, ProductRequest product, int? userId)
    {
        var existingProduct = await GetByIdAsync(id);

        if (existingProduct == null)
        {
            return false;
        }

        string query = @"
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

        await _memoryCacheService.RemoveAsync("Product");

        return productId > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        string query = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id RETURNING ""Id""";
        var parameters = new Dictionary<string, object>
        {
            { "Id", id },
        };

        var result = await _adoDbContext.ExecuteQuery<int>(query, parameters);

        await _memoryCacheService.RemoveAsync("Product");

        return result > 0;
    }

    public async Task AddOfferAsync(int productId, ProductOfferRequest productOfferRequest, int? modifiedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            { "p_product_id", productId },
            { "p_discounted_price", productOfferRequest?.DiscountedPrice },
            { "p_discount_end_on", productOfferRequest?.DiscountEndOn },
            { "p_modified_by", modifiedBy }
        };

        await _adoDbContext.ExecuteProcedure("public.sp_add_product_offer", parameters);

        await _memoryCacheService.RemoveAsync("Product");
    }
}
