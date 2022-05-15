using System.Net;
using Blog.Data;
using Blog.Dtos.CategoryDtos;
using Blog.Extensions;
using Blog.Models;
using Blog.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers;

[ApiController]
[Route("api/category/")]
public class CategoryController : HomeController
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategoriesAsync(
        [FromServices] IMemoryCache cache,
        [FromQuery] int? page = 0,
        [FromQuery] int? pageSize = 25)
    {
        var categories = await _categoryRepository.GetAllAsync((int)page, (int)pageSize);

        if(!categories.Success)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest, "Something went wrong.");

        return SuccessResponse<IEnumerable<Category>>(
            statusCode: (int)HttpStatusCode.OK,
            value: categories.Value);
    }

    [HttpGet("category/{id:int}")]
    public async Task<IActionResult> GetCategoryByIdAsync(
        [FromRoute] int? id,
        [FromServices] BlogDataContext context)
    {
        if (id is null)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.NotFound,
                message: "Category not found. Please, enter a valid category.");

        var category = await _categoryRepository.GetByIdAsync((int)id);

        if(!category.Success)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "Something went wrong!");

        return SuccessResponse<Category>(
            statusCode: (int)HttpStatusCode.OK,
            value: category.Value);
    }

    [HttpPost("create-category")]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] EditorCategoryDto categoryRequest)
    {
        if (!ModelState.IsValid)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                messages: ModelState.GetErrors());

        var category = await _categoryRepository.CreateAsync(categoryRequest);

        if(!category.Success)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "Something went wrong.");

        return SuccessResponse<Category>(
            statusCode: (int)HttpStatusCode.OK,
            value: category.Value);
    }

    [HttpPut("update-category/{id:int}")]
    public async Task<IActionResult> UpdateCategoryAsync(
        [FromRoute] int? id,
        [FromBody] EditorCategoryDto categoryRequest)
    {
        if(id is null || !ModelState.IsValid)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "Please, fill in all the requested information.");

        var category = await _categoryRepository.UpdateAsync((int)id, categoryRequest);

        if(!category.Success)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.NotFound,
                message: "Something went wrong.");

        return SuccessResponse<Category>(
            statusCode: (int)HttpStatusCode.OK,
            value: category.Value);
    }

    [HttpDelete("remove-category/{id:int}")]
    public async Task<IActionResult> RemoveCategoryAsync([FromRoute] int? id)
    {
        if(id is null || !ModelState.IsValid)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "Please, fill in all the requested information.");

        var category = await _categoryRepository.RemoveAsync((int)id);

        if (!category.Success)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "Something went wrong.");

        return SuccessResponse(
            statusCode: (int)HttpStatusCode.OK,
            message: "Category deleted.");
    }
}