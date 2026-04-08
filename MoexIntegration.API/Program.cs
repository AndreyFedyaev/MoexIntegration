using MoexIntegration.API.BackgroundJobs;
using MoexIntegration.Core.Abstractions;
using MoexIntegration.Infrastructure.Http;
using MoexIntegration.Infrastructure.Redis;
using Quartz;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MoexIntegration.Core.AssemblyMarker).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IMoexApiRequest, MoexApiRequest>(client =>
{
    client.BaseAddress = new Uri("https://iss.moex.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var configuration = builder.Configuration;

    var config = new ConfigurationOptions
    {
        EndPoints = { $"{configuration["Redis:Address"]}:{configuration["Redis:Port"]}" },
        Password = configuration["Redis:Password"],
        ConnectRetry = 3,
        ConnectTimeout = 5000,
        AbortOnConnectFail = false,
        KeepAlive = 60,
        DefaultDatabase = 0
    };

    return ConnectionMultiplexer.Connect(config);
});
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
    var GetMoexPricesDayDataKey = new JobKey(nameof(GetMoexPricesDayData));
    var GetMoexPricesWeekDataKey = new JobKey(nameof(GetMoexPricesWeekData));
    var GetMoexPricesMonthDataKey = new JobKey(nameof(GetMoexPricesMonthData));
    var GetMoexPricesYearDataKey = new JobKey(nameof(GetMoexPricesYearData));

    var now = DateBuilder.FutureDate(5, IntervalUnit.Second);

    //обнвление списка активов
    configure
        .AddJob<GetAllMoexData>(GetAllMoexDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetAllMoexDataKey)
             .StartAt(now)  //стартует сразу
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(12)    //интервал срабатывания - 12 часов
                        .RepeatForever()));

    //обновление цен за день
    configure
        .AddJob<GetMoexPricesDayData>(GetMoexPricesDayDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesDayDataKey)
            .StartAt(now.AddSeconds(10)) //стартует через 1 минуту
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInMinutes(5)  //интервал срабатывания - 5 минут
                        .RepeatForever()));

    //обновление цен за неделю
    configure
        .AddJob<GetMoexPricesWeekData>(GetMoexPricesWeekDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesWeekDataKey)
            .StartAt(now.AddMinutes(3)) //стартует через 3 минуты
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInMinutes(30)  //интервал срабатывания - 30 минут
                        .RepeatForever()));

    //обновление цен за месяц
    configure
        .AddJob<GetMoexPricesMonthData>(GetMoexPricesMonthDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesMonthDataKey)
            .StartAt(now.AddMinutes(5)) //стартует через 5 минут
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(1)     //интервал срабатывания - 1 час
                        .RepeatForever()));

    //обновление цен за год
    configure
        .AddJob<GetMoexPricesYearData>(GetMoexPricesYearDataKey)
        .AddTrigger(
            trigger => trigger.ForJob(GetMoexPricesYearDataKey)
            .StartAt(now.AddMinutes(7)) //стартует через 7 минут
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInHours(5)    //интервал срабатывания - 5 часов
                        .RepeatForever()));
});
builder.Services.AddQuartzHostedService();

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();   // важно, чтобы не было mixed content

app.UseRouting();

app.UseCors();

app.MapControllers();

app.MapGet("", () => "v.0.0.6");

app.Run();
