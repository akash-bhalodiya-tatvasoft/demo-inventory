using Inventory.Context;
using Inventory.Models.Category;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync(string search)
    {
        search = search?.ToLower();
        return await _context.Categories.Where(c => string.IsNullOrEmpty(search) || (c.Name != null && c.Name.ToLower().Contains(search)) || (c.Description != null && c.Description.ToLower().Contains(search)))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return null;

        return category;
    }

    public async Task<int> CreateAsync(CategoryRequest request, int? userId)
    {
        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int id, CategoryRequest request, int? userId)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        category.Name = request.Name;
        category.Description = request.Description;
        category.ModifiedAt = DateTime.UtcNow;
        if (request.IsActive.HasValue)
            category.IsActive = request.IsActive.Value;
        category.ModifiedBy = userId;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
