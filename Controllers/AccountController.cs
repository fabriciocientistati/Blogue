using System.Data.Common;
using System.Text.RegularExpressions;
using Blogue.Data;
using Blogue.Extensions;
using Blogue.Models;
using Blogue.Services;
using Blogue.ViewModels;
using Blogue.ViewModels.Accounts;
using Blogue.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity3.Password;

namespace Blogue.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts")]
    public async Task<IActionResult> Account(
        [FromBody] RegisterViewModel model, 
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        
        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25, includeSpecialChars: true, upperCase: false);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.User.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(
                user.Name, 
                user.Email, 
                subject: "Bem vindo a Primo Móveis", 
                body: $"Sua senha é <strong>{password}</strong>");
            
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email, password //enviar a senha por E-mail depois
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }
    
    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context
            .User
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"ˆdata:imageV[a-z]+;base64,")
            .Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }

        var user = await context
            .User.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

        user.Image = $"https://localhost:0000/images/{fileName}";

        try
        {
            context.User.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResultViewModel<string>("05X05 - Falha interna no servidor"));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
    }
}