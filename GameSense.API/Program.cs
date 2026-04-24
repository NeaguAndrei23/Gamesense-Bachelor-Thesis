using GameSense.API.Data;
using Microsoft.EntityFrameworkCore;

// Create a builder object for initialization, passing along arguments given when the app started
var builder = WebApplication.CreateBuilder(args);

// Register controllers so ASP.NET can route HTTP requests to them
builder.Services.AddControllers();

// Register the database context using dependency injection, connecting to the SQLite file
builder.Services.AddDbContext<GameSenseDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build the application
var app = builder.Build();

// Redirect HTTP reqs to HTTPS
app.UseHttpsRedirection();

// Map incoming reqs to the correct controller methods based on URL, searching for the [ApiController] marked files
app.MapControllers(); 

// Start the application and listen for incoming requests
app.Run();