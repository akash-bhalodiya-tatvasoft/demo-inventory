using System.Net.Http.Json;
using Inventory.Context;
using Inventory.Models.Post;
using Inventory.Services.Interfaces;

namespace Inventory.Services.Implementations;

public class PostService : IPostService
{
    private readonly HttpClient _httpClient;

    public PostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<PostResponse>> GetAllAsync()
    {
        try
        {
            var url = "posts";
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<PostResponse>>(url);
            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<PostResponse?> GetByIdAsync(int id)
    {
        try
        {
            var url = $"posts/{id}";
            var result = await _httpClient.GetFromJsonAsync<PostResponse>(url);
            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<int?> CreateAsync(PostRequest request)
    {
        try
        {
            var url = "posts";
            var response = await _httpClient.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<PostResponse>();
            return created?.Id ?? 0;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool?> UpdateAsync(int id, PostRequest request)
    {
        try
        {
            var url = $"posts/{id}";
            var response = await _httpClient.PutAsJsonAsync(url, request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        try
        {
            var url = $"posts/{id}";
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
