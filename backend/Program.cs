using backend.Services;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add database and auth services to the container.
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("YourConnectionString")));
var supaUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
var supaKey = Environment.GetEnvironmentVariable("SUPABASE_KEY");
var supaOpts = new SupabaseOptions {
    AutoRefreshToken = true,
};
builder.Services.AddScoped(provider => new Client(supaUrl, supaKey, supaOpts));
builder.Services.AddScoped(provider => new AuthStateProvider(
    provider.GetRequiredService<Client>(),
    provider.GetRequiredService<ILogger<AuthStateProvider>>()
));
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped(provider => new AuthService(
    provider.GetRequiredService<Client>(),
    provider.GetRequiredService<AuthStateProvider>(),
    provider.GetRequiredService<ILogger<AuthService>>()
));

// Add frontend services to the container.

builder.Services.AddSpaStaticFiles(configuration => {
    configuration.RootPath = "wwwroot";
});
builder.Services.AddCors(options => {
    options.AddPolicy("FrontendCorsPolicy",
        builder => {
            builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:5001");
        }
    );
});

// Add API services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc(
    option => option.EnableEndpointRouting = false
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("FrontendCorsPolicy");
app.UseMvc();
app.UseRouting();
app.MapControllers();
app.UseSpaStaticFiles();
app.UseSpa(configuration: builder =>
{
    if (app.Environment.IsDevelopment())
    {
        builder.UseProxyToSpaDevelopmentServer("http://localhost:5173");
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }
});
// app.MapGet("/", () => "Hello World!");
app.Run();
