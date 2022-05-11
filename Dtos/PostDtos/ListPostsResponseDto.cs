namespace Blog.Dtos.PostDtos;

public class ListPostsResponseDto
{
    public ListPostsResponseDto(
        int total,
        int page,
        int pageSize,
        IEnumerable<ListPostsDto> posts)
    {
        Total = total;
        Page = page;
        PageSize = pageSize;
        Posts = posts;
    }

    public int Total { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public IEnumerable<ListPostsDto> Posts { get; set; }
}