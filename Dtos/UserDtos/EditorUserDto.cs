using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos.UserDtos
{
    public class EditorUserDto
    {
        public string? Name { get; set; }

        public string? Email { get; set; }
    }
}