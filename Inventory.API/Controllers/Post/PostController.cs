using Inventory.Common.Responses;
using Inventory.Models.Post;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var posts = await _postService.GetAllAsync();
        if (posts == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<IEnumerable<PostResponse>>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<IEnumerable<PostResponse>>.SuccessResponse(posts, StatusCodes.Status200OK)
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<PostResponse>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<PostResponse>.SuccessResponse(post, StatusCodes.Status200OK)
        );
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(PostRequest request)
    {
        var id = await _postService.CreateAsync(request);
        if (id == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<int>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.SuccessResponse(id ?? 0, StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, PostRequest request)
    {
        var result = await _postService.UpdateAsync(id, request);
        if (result == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<int>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _postService.DeleteAsync(id);
        if (result == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<int>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<int>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }
}
