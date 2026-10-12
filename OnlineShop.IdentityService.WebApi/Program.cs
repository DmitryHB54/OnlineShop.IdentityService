using OnlineShop.IdentityService.Application;
using OnlineShop.IdentityService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicaton();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.SwaggerEndpoint("swagger/v1/swagger/json", "Identity API V1");
        cfg.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }