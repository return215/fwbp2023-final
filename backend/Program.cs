// configure service with dbcontext, spa static files, cors, auth, and mvc
using backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("YourConnectionString")));

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://example.com"));
});

builder.Services.AddAuthentication();
builder.Services.AddControllersWithViews();






app.MapGet("/", () => "Hello World!");

app.Run();
