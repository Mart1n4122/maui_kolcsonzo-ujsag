using Common;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class ToDoService(NewsPaperDbContext db) : IToDoService
{
}