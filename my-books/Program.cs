using Microsoft.EntityFrameworkCore;
using my_books.Data;
using my_books.Data.Services;
using my_books.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<BooksService>();
builder.Services.AddTransient<AuthorService>();
builder.Services.AddTransient<PublisherService>();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;

    //config.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("custom-version-header");
    //config.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.MediaTypeApiVersionReader("custom-version-media");
});   
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//AppDbIntializer.Seed(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Serve the Swagger UI at application root ("/") so the browser opens to a page
        options.RoutePrefix = string.Empty;
        // Explicitly point to the Swashbuckle-generated JSON to avoid conflicts with MapOpenApi's endpoints
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "my-books API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Error handling middleware
app.ConfigureBuiltInExceptionMiddleware();
//app.ConfigureCustomExceptionMiddleware();

app.MapControllers();

app.Run();
