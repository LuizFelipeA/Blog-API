using Blog.Dtos.CategoryDtos;
using Blog.Models;
using Ether.Outcomes;

namespace Blog.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IOutcome<Category>> CreateAsync(EditorCategoryDto categoryRequest);

    Task<IOutcome<IEnumerable<Category>>> GetAllAsync(int page, int pageSize);

    Task<IOutcome<Category>> GetByIdAsync(int id);

    Task<IOutcome<Category>> UpdateAsync(int id, EditorCategoryDto categoryRequest);

    Task<IOutcome> RemoveAsync(int id);
}
