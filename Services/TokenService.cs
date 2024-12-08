using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blogue.Extensions;
using Blogue.Models;
using Microsoft.IdentityModel.Tokens;

namespace Blogue.Services;
//gerar token
public class TokenService
{
    public string GenerateToken(User user)
    {
        var manipuladorDeToken = new JwtSecurityTokenHandler();
        var meuToken = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        var claims = user.GetClaims();
        var descricaoDoToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(meuToken), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = manipuladorDeToken.CreateToken(descricaoDoToken);
        return manipuladorDeToken.WriteToken(token);
    }
}
