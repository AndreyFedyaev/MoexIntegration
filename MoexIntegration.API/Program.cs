using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Application.Handling.Handlers;
using MoexIntegration.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MoexGetDataRequestHandler).Assembly);
});
builder.Services.AddControllers();

builder.Services.AddScoped<IMoexApiRequest, MoexApiRequest>();

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
