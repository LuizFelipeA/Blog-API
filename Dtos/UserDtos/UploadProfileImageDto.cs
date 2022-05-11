using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos.UserDtos;

public class UploadProfileImageDto
{
    [Required(ErrorMessage = "This field is required.")]
    public string Base64Image { get; set; }
}