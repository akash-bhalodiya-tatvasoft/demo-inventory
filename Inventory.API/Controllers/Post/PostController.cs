using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Post;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
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
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }
        var post = await _postService.GetByIdAsync(convertedId);
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
            ApiResponse<string>.SuccessResponse(EncryptionHelper.EncryptId(id.ToString()), StatusCodes.Status201Created)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, PostRequest request)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }

        var result = await _postService.UpdateAsync(convertedId, request);
        if (result == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<int>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<string>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var decryptedId = EncryptionHelper.DecryptId(id);
        if (!int.TryParse(decryptedId, out int convertedId))
        {
            throw new Exception("Invalid id");
        }

        var result = await _postService.DeleteAsync(convertedId);
        if (result == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<int>.Failure(StatusCodes.Status500InternalServerError)
            );
        }
        return StatusCode(
            StatusCodes.Status200OK,
            ApiResponse<string>.SuccessResponse(id, StatusCodes.Status200OK)
        );
    }
}
