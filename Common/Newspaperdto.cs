using Data;
namespace Common;
public record NewspaperDto
{
    public int ArticleId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
