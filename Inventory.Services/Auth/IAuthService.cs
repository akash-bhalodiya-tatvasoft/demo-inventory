using Inventory.Models.Auth;

namespace Inventory.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<string?> LoginAsync(LoginRequest request);
}
