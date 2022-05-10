using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos.UserDtos;

public class RegisterUserDto
{
    [Required(ErrorMessage = "The name field is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "The email field is required.")]
    [EmailAddress(ErrorMessage = "The email field is not valid.")]
    public string Email { get; set; }

    // public string Password { get; set; }
}
