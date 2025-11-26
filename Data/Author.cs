namespace Data;

public class Author
{
    public int id;
    public string name = string.Empty;
    public string bio = string.Empty;

    public List<Article> Articles { get; set; } = new();
}
