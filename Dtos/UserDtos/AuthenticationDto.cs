using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos.UserDtos;

public class AuthenticationDto
{
    [Required(ErrorMessage = "The email field is required.")]
    [EmailAddress(ErrorMessage = "The email field is not valid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The password Field is required.")]
    public string Password { get; set; }
}