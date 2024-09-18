using Microsoft.EntityFrameworkCore;
using FridgeAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the FridgeContext to use SQLite
builder.Services.AddDbContext<FridgeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS services to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()    // Allows requests from any origin
                   .AllowAnyMethod()    // Allows all HTTP methods (GET, POST, etc.)
                   .AllowAnyHeader();   // Allows any headers
        });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for external API calls (OpenAI)
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS for the app
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();

