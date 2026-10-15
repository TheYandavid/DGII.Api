
using DGII.Api.Data;
using DGII.Api.Interfaces;
using DGII.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<DGIIContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<IContribuyenteService, ContribuyenteService>();
builder.Services.AddScoped<IComprobanteFiscalService, ComprobanteFiscalService>();
builder.Services.AddScoped<IDevelopmentDataSeeder, DevelopmentDataSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DGII API v1");
    });

    if (app.Configuration.GetValue<bool>("SeedData:Enabled"))
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider
            .GetRequiredService<IDevelopmentDataSeeder>();

        await seeder.SeedAsync();
    }
}

app.UseHttpsRedirection();
app.UseDefaultFiles();   
app.UseStaticFiles();    
app.UseAuthorization();
app.MapControllers();

app.Run();

