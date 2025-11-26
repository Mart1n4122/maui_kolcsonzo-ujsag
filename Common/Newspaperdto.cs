using Data;
namespace Common;
public record NewspaperDto
{
    public int ArticleId { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int AuthId;
    public string Name = string.Empty;
    public string Bio = string.Empty;

    public List<Article> Articles { get; set; } = new();
};
