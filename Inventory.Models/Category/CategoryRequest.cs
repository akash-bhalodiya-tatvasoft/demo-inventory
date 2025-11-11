using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Category;

public class CategoryRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }

}
