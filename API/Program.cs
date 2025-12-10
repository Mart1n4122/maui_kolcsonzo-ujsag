using Common;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

// Ez a sor felel az alap web szerver létrehozásáért.
var builder = WebApplication.CreateBuilder(args);

// Swaggerhez, azon belül is az authentikációhoz szükséges beállítások
builder.Services.AddOpenApiDocument(config =>
{
    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Type 'Bearer {your JWT token}' into the field below."
    });

    config.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

// Authorization policy-k hozzáadása
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin", policy => policy.RequireRole("Admin"))
  .AddPolicy("user", policy => policy.RequireRole("User"));

// Tesztekhez szükséges endpoint explorer
builder.Services.AddEndpointsApiExplorer();

// Configure the Database connection
var connectionString = builder.Configuration.GetConnectionString("NewsPaperDbContext");
builder.Services.AddDbContext<BlogDbContext>(options =>
  options.UseSqlServer(connectionString));

// Identity endpointokhoz szükséges service-ek
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>();

// DI container konfiguráció
builder.Services.AddTransient<IBlogService, BlogService>();

// Authorization szolgáltatás
builder.Services.AddAuthorization();

// CORS beállítások
var allowSpecificOrigins = "_allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Web alkalmazás objektum létrehozása
var app = builder.Build();

// CORS middleware
app.UseCors(allowSpecificOrigins);

// HTTP request pipeline konfiguráció
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

// User kezelés endpointok
var accountEndpoints = app.MapGroup("Account").WithTags("Account");
accountEndpoints.MapIdentityApi<IdentityUser>();

// Blog endpointok
var blogEndpoints = app.MapGroup("Blog").WithTags("Blog");
blogEndpoints.MapGet("get/{id:int}", async (
    int id, IBlogService service) =>
{
    return await service.GetAsync(id);
});

blogEndpoints.MapGet("list", async (
    [FromQuery(Name = "isReady")] bool? isReady,
    IBlogService service) =>
{
    return await service.ListAllAsync();
});

blogEndpoints.MapPost("create", async (
    NewspaperDto dto,
    IBlogService service) =>
{
    await service.CreateAsync(dto);

    return Results.Created();
}).RequireAuthorization("admin");

blogEndpoints.MapPut("update", async (
    NewspaperDto dto,
    IBlogService service) =>
{
    await service.UpdateAsync(dto);

    return Results.Ok();
}).RequireAuthorization("admin");

blogEndpoints.MapDelete("delete/{id:int}", async (
    int id, IBlogService service) =>
{
    await service.DeleteAsync(id);

    return Results.Ok();
}).RequireAuthorization("admin");

// Auto migration
using var scope = app.Services.CreateScope();
using var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
var migrations = await dbContext.Database.GetPendingMigrationsAsync();
if (migrations.Any())
    await dbContext.Database.MigrateAsync();

var roles = dbContext.Set<IdentityRole>();
if (!await roles.AnyAsync(role => role.Name == "Admin"))
{
    roles.Add(new IdentityRole { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" });
    roles.Add(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });
    roles.Add(new IdentityRole { Name = "User", NormalizedName = "USER" });

    await dbContext.SaveChangesAsync();
}

// Az alkalmazás elindítása
app.Run();

// Tesztekhez szükséges
public partial class Program;
