using SsePoc.Api.Infrastructure.Data;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .UseSnakeCaseNamingConvention()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton(_ =>
{
    var options = new UnboundedChannelOptions
    {
        SingleReader = false,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    };

    return Channel.CreateUnbounded<Product>(options);
});

var app = builder.Build();

app.MapOpenApi();
app.MapControllers();

app.UseHttpsRedirection();
app.UseCors(config => config
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

await app.RunAsync();