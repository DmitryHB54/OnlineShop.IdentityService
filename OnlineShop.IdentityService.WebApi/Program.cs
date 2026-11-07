using OnlineShop.IdentityService.Application;
using OnlineShop.IdentityService.Infrastructure;
using OnlineShop.IdentityService.Infrastructure.Persistence.Seeders;
using OnlineShop.IdentityService.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicaton();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddGlobalExtensionHandling();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddRabbitMq();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseExceptionHandler();
app.UseSwaggerDocumentation(app.Environment);

app.UseHttpsRedirection();
app.UseAuthentication();   
app.UseAuthorization();

app.MapControllers();

await RoleSeeder.SeedAsync(app.Services);
await UserSeeder.SeedAsync(app.Services, app.Configuration);

app.Run();

public partial class Program { }