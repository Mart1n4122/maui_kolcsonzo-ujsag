namespace OnlineNewspaper.Database
{
    public class Article
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
