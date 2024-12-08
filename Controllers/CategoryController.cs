using Blogue.Data;
using Blogue.Extensions;
using Blogue.Models;
using Blogue.ViewModels;
using Blogue.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blogue.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("v1/categories")]
    public IActionResult GetAsync(
        [FromServices] IMemoryCache cache,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var categories = cache.GetOrCreate("CategoriesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetCategories(context);
            });

            return Ok(new ResultViewModel<List<Category>>(categories));
        }
        catch 
        {
            return StatusCode(500, new ResultViewModel<List<Category>>("05XE01 - Falha interna no servidor"));
        }
    }

    private List<Category> GetCategories(BlogDataContext context)
    {
        return context.Category.ToList();
    }

    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Não foi possível encontrar a categoria"));
            }

            return Ok(new ResultViewModel<Category>(category));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE02 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync(
        [FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
        
        try
        {
            var category = new Category
            {
                Id = 0,
                Name = model.Name,
                Slug = model.Slug.ToLower()
            };
                
            await context.Category.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}",new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE05 - Não foi posível incluir a categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("05X06 - Falha interna no servidor"));
        }
    }

    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] EditorCategoryViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Não foi possível encontrar a categoria"));
            }

            category.Name = model.Name;
            category.Slug = model.Slug;

            context.Category.Update(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE07 - Não foi possível atualizar categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE08 - Falha interna no servidor"));
        }
    }

    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new ResultViewModel<Category>("Não foi possível encontrar a categoria"));
            }

            context.Category.Remove(category);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE09 - Não foi possível apagar categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("05XE10 - Falha interna no servidor"));
        }
    }

}