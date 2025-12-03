using Common;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class BlogService(BlogDbContext db) : IBlogService
{
    public async Task<NewspaperDto> GetAsync(int id)
    {
        var article = await db.Articles.SingleAsync(a => a.Id == id);

        return new NewspaperDto
        {
            ArticleId = article.Id,
            AuthorId = article.AuthorId,
            AuthorName = article.Author.name,
            Title = article.Title,
            Content = article.Content,
        };
    }
    public async Task<List<NewspaperDto>> ListAllAsync()
    {
        return await db.Articles.Select(a => new NewspaperDto
        {
            ArticleId = a.Id,
            AuthorId = a.AuthorId,
            AuthorName = a.Author.name,
            Title = a.Title,
            Content = a.Content
        }).ToListAsync();
    }
    public async Task CreateAsync(NewspaperDto dto)
    {

    }
    public async Task UpdateAsync(NewspaperDto dto)
    {

    }
    public async Task DeleteAsync(int id)
    {

    }
}