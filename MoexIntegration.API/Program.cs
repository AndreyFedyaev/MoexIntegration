using MoexIntegration.API.BackgroundJobs;
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

builder.Services.AddHttpClient<IMoexApiRequest, MoexApiRequest>(client =>
{
    client.BaseAddress = new Uri("https://iss.moex.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IMoexApiRequest, MoexApiRequest>();
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
    var GetMoexPricesDataKey = new JobKey(nameof(GetMoexPricesData));
    var GetMoexPricesWeekDataKey = new JobKey(nameof(GetMoexPricesWeekData));
    var GetMoexPricesMonthDataKey = new JobKey(nameof(GetMoexPricesMonthData));
    var GetMoexPricesYearDataKey = new JobKey(nameof(GetMoexPricesYearData));

    var now = DateBuilder.FutureDate(5, IntervalUnit.Second);

    configure
        .AddJob<GetAllMoexData>(GetAllMoexDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetAllMoexDataKey)
             .StartAt(now)
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(12)
                        .RepeatForever()));

    configure
        .AddJob<GetMoexPricesData>(GetMoexPricesDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesDataKey)
            .StartAt(now.AddMinutes(1))
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInMinutes(10)
                        .RepeatForever()));

    configure
        .AddJob<GetMoexPricesWeekData>(GetMoexPricesWeekDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesWeekDataKey)
            .StartAt(now.AddMinutes(3))
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(1)
                        .RepeatForever()));

    configure
        .AddJob<GetMoexPricesMonthData>(GetMoexPricesMonthDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesMonthDataKey)
            .StartAt(now.AddMinutes(6))
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(5)
                        .RepeatForever()));

    configure
        .AddJob<GetMoexPricesYearData>(GetMoexPricesYearDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesYearDataKey)
            .StartAt(now.AddMinutes(10))
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(10)
                        .RepeatForever()));
       
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
