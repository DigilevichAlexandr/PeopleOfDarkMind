using Microsoft.EntityFrameworkCore;
using PeopleOfDarkMind.Application;
using PeopleOfDarkMind.Infrastructure;
using PeopleOfDarkMind.Infrastructure.Persistence;
using PeopleOfDarkMind.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var corsOrigins = new List<string> { "http://localhost:5173", "http://localhost:3000" };
var frontendUrl = builder.Configuration["Frontend:Url"];
if (!string.IsNullOrWhiteSpace(frontendUrl))
    corsOrigins.Add(frontendUrl.TrimEnd('/'));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(corsOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    if (conn?.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) == true)
    {
        db.Database.EnsureCreated();
        try { db.Database.ExecuteSqlRaw("ALTER TABLE Characters ADD COLUMN PendingEventId TEXT NULL"); }
        catch { /* already exists */ }
    }
    else
        db.Database.Migrate();
    GameDataSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

app.Run();
