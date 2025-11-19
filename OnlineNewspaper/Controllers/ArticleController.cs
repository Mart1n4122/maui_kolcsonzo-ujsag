using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineNewspaper.Data;
using OnlineNewspaper.Database;
using System.Runtime.InteropServices;

namespace OnlineNewspaper.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly BlogDbContext _db;
        public ArticlesController(BlogDbContext db) => _db = db;

        // GET: api/articles
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var list = await _db.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/articles/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var article = await _db.Articles.Include(a => a.Author).FirstOrDefaultAsync(a => a.Id == id);
            return article is null ? NotFound() : Ok(article);
        }

        // POST: api/articles
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] Article payload)
        {
            payload.CreatedAt = DateTime.UtcNow;
            _db.Articles.Add(payload);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = payload.Id }, payload);
        }

        // PUT: api/articles/5
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] Article payload)
        {
            var existing = await _db.Articles.FindAsync(id);
            if (existing is null) return NotFound();

            existing.Title = payload.Title;
            existing.Content = payload.Content;
            existing.AuthorId = payload.AuthorId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/articles/5
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Articles.FindAsync(id);
            if (existing is null) return NotFound();

            _db.Articles.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}