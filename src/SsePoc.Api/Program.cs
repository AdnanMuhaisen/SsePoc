using SsePoc.Api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .UseSnakeCaseNamingConvention()
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapOpenApi();
app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();