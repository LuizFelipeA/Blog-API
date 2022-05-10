using System.Security.Claims;
using Blog.Models;

namespace Blog.Extensions;

public static class RoleClaimsExtension
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        var result = new List<Claim>
        {
            new (type: ClaimTypes.Name, value: user.Email)
        };

        result.AddRange(
            user.Roles.Select(role => new Claim(type: ClaimTypes.Role, role.Slug)));

        return result;
    }
}