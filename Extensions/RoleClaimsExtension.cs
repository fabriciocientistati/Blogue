using System.Security.Claims;
using Blogue.Models;

namespace Blogue.Extensions;

public static class RoleClaimsExtension
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        var result = new List<Claim>
        {
            new(ClaimTypes.Name, value: user.Email)
        };
        result.AddRange(
            user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Slug))
            );
        return result;
    }
}