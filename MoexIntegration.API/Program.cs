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

//cors для тестирования фронта
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://127.0.0.1:5500",
            "http://127.0.0.1:5501",
            "http://localhost:5500",
            "http://localhost:5501"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();

    });
});

var app = builder.Build();

//cors для тестирования фронта
app.UseCors();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
