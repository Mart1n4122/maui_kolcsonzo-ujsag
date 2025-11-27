using Common;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class BlogService(BlogDbContext db) : IBlogService
{

}