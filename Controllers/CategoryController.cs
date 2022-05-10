using System.Net;
using Blog.Data;
using Blog.Dtos;
using Blog.Extensions;
using Blog.Models;
using Ether.Outcomes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Blog.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CategoryController : HomeController
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllAsync(
            [FromServices] BlogDataContext context)
        {
            var categories = await context.Categories.ToListAsync();

            if(categories is null)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: "Something went wrong.");

            return SuccessResponse<List<Category>>(
                statusCode: (int)HttpStatusCode.OK,
                value: categories);
            
        }

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int? id,
            [FromServices] BlogDataContext context)
        {
            if(id is null)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: "Category not found. Please, enter a valid category.");

            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            
            if(category is null)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: "Something went wrong!");

            return SuccessResponse<Category>(
                statusCode: (int)HttpStatusCode.OK,
                value: category);
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateAsync(
            [FromBody] EditorCategoryDto categoryRequest,
            [FromServices] BlogDataContext context)
        {
            if(!ModelState.IsValid)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    messages: ModelState.GetErrors());

            var category = new Category
            {
                Name = categoryRequest.Name,
                Slug = categoryRequest.Slug
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return SuccessResponse<Category>(
                statusCode: (int)HttpStatusCode.OK,
                value: category,
                message: $"/categories/{category.Id}");
        }

        [HttpPut("categories/{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int? id,
            [FromBody] EditorCategoryDto categoryRequest,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category is null)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: "Category not found. Please, enter a valid category.");

            category.Name = categoryRequest.Name;
            category.Slug = categoryRequest.Slug;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return SuccessResponse<Category>(
                statusCode: (int)HttpStatusCode.OK,
                value: category);
        }

        [HttpDelete("categories/{id:int}")]
        public async Task<IActionResult> RemoveAsync(
            [FromRoute] int? id,
            [FromBody] Category categoryRequest,
            [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category is null)
                return FailureResponse(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: "Category not found. Please, enter a valid category.");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return SuccessResponse(
                statusCode: (int)HttpStatusCode.OK,
                message: "Category deleted.");
        }
    }
}