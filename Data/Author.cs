namespace Data;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Bio { get; set; }

    public List<Article> Articles { get; set; } = new();
}
