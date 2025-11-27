using Common;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class BlogService(BlogDbContext db) : IBlogService
{
    public async Task<NewspaperDto> GetAsync(int id)
    {
        return null;
    }
    public async Task<NewspaperDto> ListAllAsync()
    {
        return null;
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