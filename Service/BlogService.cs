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
            AuthorName = article.Author.Name,
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
            AuthorName = a.Author.Name,
            Title = a.Title,
            Content = a.Content
        }).ToListAsync();
    }
    public async Task CreateAsync(NewspaperDto dto)
    {
        var article = new Article
        {
             AuthorId = dto.AuthorId,
             Content = dto.Content,
             CreatedAt = DateTime.Now,
             Title = dto.Title,


        };
        db.Articles.Add(article);
        await db.SaveChangesAsync();
    }
    public async Task UpdateAsync(NewspaperDto dto)
    {
        await db.Articles.Where(a => a.Id == dto.ArticleId).ExecuteUpdateAsync(
            setters =>
                setters.SetProperty(a => a.Content, dto.Content)
                        .SetProperty(a => a.Title, dto.Title)
                        .SetProperty(a => a.AuthorId, dto.AuthorId)
                        .SetProperty(a => a.UpdatedAt, DateTime.Now));
    }
    public async Task DeleteAsync(int id)
    {
        await db.Articles.Where(a => a.Id == id).ExecuteDeleteAsync();
    }
}