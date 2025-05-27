using UserServiceAPI.Repository;
using UserServiceAPI.Services;
using UserServiceAPI.Seeding;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServiceMongo, UserServiceMongo>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Seed users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var userService = services.GetRequiredService<IUserServiceMongo>();
    await UserSeeder.SeedUsersAsync(userService, logger);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();