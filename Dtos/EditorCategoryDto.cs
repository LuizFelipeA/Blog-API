using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos;

public class EditorCategoryDto
{
    [Required]
    [StringLength(
        40,
        MinimumLength = 3,
        ErrorMessage = "This field must have between 3 and 40 characters")]
    public string Name { get; set; }

    [Required]
    public string Slug { get; set; }
}