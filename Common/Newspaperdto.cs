using Data;
namespace Common;
public record NewspaperDto
{
    public int ArticleId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
};
