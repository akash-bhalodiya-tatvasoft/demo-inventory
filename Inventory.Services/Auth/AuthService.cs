using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Inventory.Common.Enums;
using Inventory.Context;
using Inventory.Models.Auth;
using Inventory.Models.Entities;
using Inventory.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            return false;

        var encryptedPassword = GenerateEncryptedPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = encryptedPassword,
            Role = (int)GlobalEnum.UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !user.IsActive)
            return null;

        var hashedInput = GenerateEncryptedPassword(request.Password);
        if (hashedInput != user.PasswordHash)
            return null;

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return GenerateJwtToken(user);
    }

    public async Task<int?> UpdateUserStatusAsync(int userId, int modifiedBy, bool isActive)
    {
        var result = await _context.Database
        .SqlQueryRaw<int>(
            @"SELECT public.fn_make_user_inactive({0}, {1}, {2});",
            userId, modifiedBy, isActive)
        .ToListAsync();

        return result.FirstOrDefault();
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JWTToken:SecretKey"]);
        var identity = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.Role, Enum.GetName(typeof(GlobalEnum.UserRole), user.Role)),
                        new Claim(ClaimTypes.Email, user.Email)
                        });

        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _config["JWTToken:Issuer"],
            Subject = identity,
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateEncryptedPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hashedPassword).ToLowerInvariant();
    }
}
