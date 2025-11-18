using Inventory.Models.Post;

namespace Inventory.Services.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostResponse>> GetAllAsync();
    Task<PostResponse?> GetByIdAsync(int id);
    Task<int?> CreateAsync(PostRequest request);
    Task<bool?> UpdateAsync(int id, PostRequest request);
    Task<bool?> DeleteAsync(int id);
}
