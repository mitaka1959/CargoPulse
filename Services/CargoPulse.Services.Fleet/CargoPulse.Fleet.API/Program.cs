using CargoPulse.Fleet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FleetDatabase"),
        b => b.MigrationsAssembly("CargoPulse.Fleet.Infrastructure")));

builder.Services.AddScoped<CargoPulse.Fleet.Infrastructure.Persistence.Seeder.FleetDataSeeder>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<CargoPulse.Fleet.Infrastructure.Persistence.Seeder.FleetDataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "A fatal error occurred while seeding the database! Is your kubectl port-forward tunnel running?");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();


