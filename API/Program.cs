using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;


// Ez a sor felel az alap web szerver létrehozásáért.
// Builder pattern-t használ, ahol a konfigurációs beállításokat a builder objektumon keresztül adjuk meg.
var builder = WebApplication.CreateBuilder(args);

// Elkezdjük a buildert felparaméterezni a szükséges szolgáltatásokkal és konfigurációkkal.

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

// Authorization policy-k hozzáadása, ezekre tudunk majd hivatkozni RequireAuthorization metódusokban.
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin", policy => policy.RequireRole("Admin"))
  .AddPolicy("user", policy => policy.RequireRole("User"));

// Ez a tesztekben használhatósághoz kell, hogy a teszt framework elérje az endpointokat
builder.Services.AddEndpointsApiExplorer();

// Configure the Database connection
var connectionString = builder.Configuration.GetConnectionString("NewsPaperDbContext");
builder.Services.AddDbContext<NewsPaperDbContext>(options =>
  options.UseSqlServer(connectionString));

// accountEndpoints.MapIdentityApi<IdentityUser>(); - ezekhez az endpointokhoz szükséges service-eket adja hozzá a DI-hoz
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NewsPaperDbContext>();

// Configure the DI container
builder.Services.AddTransient<IToDoService, ToDoService>();

// Authorization szolgáltatás hozzáadása a DI konténerhez
builder.Services.AddAuthorization();

// CORS beállítások hozzáadása a DI konténerhez, CORS a böngészõbõl érkezõ kérések kezeléséhez szükséges.
var allowSpecificOrigins = "_allowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});

//A fentebb megadott konfigurációk alapján létrehozzuk a web alkalmazás objektumot.
var app = builder.Build();

// CORS middleware hozzáadása az alkalmazás middleware pipeline-jához
app.UseCors(allowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger middleware hozzáadása a pipeline-hoz fejlesztõi környezetben
    app.UseOpenApi();
    app.UseSwaggerUi();
}


// User kezeléshez szükséges endpointok hozzáadása
var accountEndpoints = app.MapGroup("Account").WithTags("Account");
accountEndpoints.MapIdentityApi<IdentityUser>();


// ToDo endpointok hozzáadása
var toDoEndpoints = app.MapGroup("ToDo").WithTags("ToDo");
toDoEndpoints.MapGet("get/{id:int}", async (int id, IToDoService service) =>
{
    return await service.GetAsync(id);
}).RequireAuthorization();

toDoEndpoints.MapGet("list", async (
    [FromQuery(Name = "isReady")] bool? isReady,
    IToDoService service) =>
{
    return await service.ListAllAsync(isReady);
}).RequireAuthorization();

toDoEndpoints.MapPost("create", async (Newspa model, IToDoService service) =>
{
    await service.CreateAsync(model);

    return Results.Created();
}).RequireAuthorization("admin");

toDoEndpoints.MapPut("update", async (ToDo model, IToDoService service) =>
{
    await service.UpdateAsync(model);

    return Results.Ok();
}).RequireAuthorization();

toDoEndpoints.MapDelete("delete/{id:int}", async (int id, IToDoService service) =>
{
    await service.DeleteAsync(id);

    return Results.Ok();
}).RequireAuthorization();

// Auto migration
using var scope = app.Services.CreateScope();
using var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
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

// Az alkalmazás elindítása, innentõl kezdve képes fogadni a bejövõ HTTP kéréseket.
app.Run();

//Ez a tesztekben használhatósághoz kell, hogy publikusan elérhetõ legyen a Program osztály
public partial class Program;