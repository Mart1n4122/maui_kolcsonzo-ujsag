namespace Data;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<Article> Articles { get; set; } = new();
}

