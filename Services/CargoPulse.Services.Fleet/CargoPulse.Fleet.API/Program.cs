using CargoPulse.Fleet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FleetDatabase"),
        b => b.MigrationsAssembly("CargoPulse.Fleet.Infrastructure")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();


