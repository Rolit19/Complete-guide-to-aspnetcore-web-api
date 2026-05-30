var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.MapControllers();

app.Run();
