using DeliveryApp.Api.Adapters.BackgroundJobs;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Infrastructure.Http;
using MoexIntegration.Infrastructure.Redis;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MoexIntegration.Core.AssemblyMarker).Assembly);
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

// CRON Jobs
builder.Services.AddQuartz(configure =>
{
    var GetAllMoexDataKey = new JobKey(nameof(GetAllMoexData));
    configure
        .AddJob<GetAllMoexData>(GetAllMoexDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetAllMoexDataKey)
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(12)
                        .RepeatForever()));
       
    configure.UseMicrosoftDependencyInjectionJobFactory();
});
builder.Services.AddQuartzHostedService();

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

app.MapGet("", () => "v.0.0.6");

app.Run();
