using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using WebApi.Helpers;
using WebApi.HostedServices;
using WebApi.Models.Tasks;
using WebApi.Seed;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<DataContext>();
    services.AddCors();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params (e.g. User update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // configure DI for application services
    services.AddScoped<IEventService, EventService>();
    services.AddScoped<IUserService, UserService>();

    // services.AddQuartz(q =>
    // {
    //     // base quartz scheduler, job and trigger configuration
    // });

    // // ASP.NET Core hosting
    // services.AddQuartzServer(options =>
    // {
    //     // when shutting down we want jobs to complete gracefully
    //     options.WaitForJobsToComplete = true;
    // });

    services.AddControllersWithViews()
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

    services.AddHostedService<QuartzHostedService>();
    services.AddSingleton<IJobFactory, SingletonJobFactory>();
    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

    services.AddSingleton<ScrapingJob>();
    // services.AddSingleton(new MyJobs(type:typeof(ScrapingJob), expression:"0/30 0/1 * 1/1 * ? * ")); // every 30 sec
    // services.AddSingleton(new MyJobs(type:typeof(TestJob), expression:"0/30 0/1 0 * * ? *")); // At minute 0
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DataContext mainDbContext = services.GetRequiredService<DataContext>();

    if (!await DefaultSeeds.CheckIsSeeded(mainDbContext))
    {
        await DefaultSeeds.SeedAsync(mainDbContext);
    }
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapControllers();
}

app.Run();