
using DGII.Api.Data;
using DGII.Api.Interfaces;
using DGII.Api.Middleware;
using DGII.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
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

        try
        {
            await seeder.SeedAsync();
        }
        catch (Exception exception)
        {
            app.Logger.LogCritical(exception, "Development data seeding failed during application startup.");
            throw;
        }
    }
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseDefaultFiles();   
app.UseStaticFiles();    
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;

