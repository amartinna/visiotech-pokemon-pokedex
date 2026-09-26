using Microsoft.EntityFrameworkCore;
using Visiotech.Pokemon.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Registrar el contexto de SQLite apuntando a un archivo local de almacenamiento
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=pokedex.db"));

var app = builder.Build();

// Comando senior: Asegura la inicialización y el sembrado automático de la DB al arrancar
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
