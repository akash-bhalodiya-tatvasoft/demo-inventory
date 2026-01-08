using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Inventory.Common.Helpers;

public static class FileHelper
{
    public static async Task<(string fileName, string fileUrl)> SaveFileAsync(IFormFile file, string[] allowedExtensions, string directoryName)
    {
        var ext = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(ext))
            throw new Exception("Invalid file format");

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directoryName);

        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var fileName = $"{DateTime.Now:yyyyMMddHHmmss}{ext}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (fileName, $"/{directoryName}/{fileName}");
    }

    public static async Task<string?> GetBase64ImageAsync(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return null;

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            imageUrl.TrimStart('/')
        );

        if (!File.Exists(filePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(filePath);
        return Convert.ToBase64String(bytes);
    }

    public static void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return;

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            fileUrl.TrimStart('/')
        );

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}