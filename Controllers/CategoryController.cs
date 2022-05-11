using System.Net;
using Blog.Data;
using Blog.Dtos.CategoryDtos;
using Blog.Extensions;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers;

[ApiController]
[Route("api/category/")]
public class CategoryController : HomeController
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllAsync(
        [FromServices] IMemoryCache cache,
        [FromServices] BlogDataContext context,
        [FromQuery] int? page = 0,
        [FromQuery] int? pageSize = 25)
    {
        if(page is null || pageSize is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "The page and page size fields is required.");

        var categories = cache.GetOrCreate(
            key: "CategoriesCache",
            factory: entry => 
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                
                return GetCategoriesAsync(context: context);
            });

        // var categories = await context
        //     .Categories
        //     .AsNoTracking()
        //     .Skip((int)page * (int)pageSize)
        //     .Take((int)pageSize)
        //     .ToListAsync();

        if(categories is null)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "There is no categories.");

        return SuccessResponse<List<Category>>(
            statusCode: (int)HttpStatusCode.OK,
            value: categories);
    }

    private List<Category> GetCategoriesAsync(
        [FromServices] BlogDataContext context)
            => context.Categories.ToList();
    

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