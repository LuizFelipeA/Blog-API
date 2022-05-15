using Ether.Outcomes;

namespace Blog.Shared.Repositories;

public interface IRepository<TEntity>
{
    Task<IOutcome<TEntity>> CreateAsync(TEntity requestModel);

    Task<IOutcome<IEnumerable<TEntity>>> GetAllAsync(int page, int pageSize);
}
