using FakeRestuarantAPI.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiConfiguration();
builder.Services.AddFilterConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigureMiddleware();

app.Run();
