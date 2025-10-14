using MoexIntegration.Core.Abstractions;
using MoexIntegration.Core.Application.Handling.Handlers;
using MoexIntegration.Infrastructure.Http;
using MoexIntegration.Infrastructure.Redis;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MoexGetDataRequestHandler).Assembly);
});
builder.Services.AddControllers();

builder.Services.AddScoped<IMoexApiRequest, MoexApiRequest>();
builder.Services.AddSingleton<ICacheService, RedisService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://fineva.ru",
            "https://www.fineva.ru",
            "http://127.0.0.1:5500",
            "http://localhost:5500"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();   // важно, чтобы не было mixed content

app.UseRouting();

app.UseCors();

app.MapControllers();

app.MapGet("", () => "v.0.0.5");

app.Run();
