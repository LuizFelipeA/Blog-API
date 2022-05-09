using System.Net;
using Blog.Data;
using Blog.Dtos;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CategoryController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllAsync(
            [FromServices] BlogDataContext context)
            => Ok(await context.Categories.ToListAsync());

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int? id,
            [FromServices] BlogDataContext context)
        {
            if(id is null)
                return NotFound();

            var category = await context.Categories.FirstOrDefaultAsync();
            
            if(category is null)
                return StatusCode(
                    (int)HttpStatusCode.BadRequest,
                    new ResultDto<Category>("Something went wrong!"));

            var response = new ResultDto<Category>(category);

            return Ok(response);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateCategoryDto categoryRequest,
            [FromServices] BlogDataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var category = new Category
            {
                Name = categoryRequest.Name,
                Slug = categoryRequest.Slug
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"/categories/{category.Id}", category);
        }

        [HttpPut("categories/{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int? id,
            [FromBody] Category categoryRequest,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category is null)
                return NotFound();

            category.Name = categoryRequest.Name;
            category.Slug = categoryRequest.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpDelete("categories/{id:int}")]
        public async Task<IActionResult> RemoveAsync(
            [FromRoute] int? id,
            [FromBody] Category categoryRequest,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category is null)
                return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
    }
}