using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PokemonApi;
using PokemonApi.Data;
using PokemonApi.Interfaces;
using PokemonApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
// Application database context
builder.Services.AddHealthChecks().AddDbContextCheck<DataContext>();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    SeedData(app);
}

void SeedData(IHost webApp)
{
    var scopedFactory = webApp.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory?.CreateScope();
    var service = scope?.ServiceProvider.GetService<Seed>();
    service?.SeedDataContext();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
