using SwgohApi.Infrastructure.Postgres;
using SwgohApi.Infrastructure.Utilities;
using SwgohApi.Users;

const string CorsPolicy = "AllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")
  .Get<string[]>();
if (allowedOrigins is null)
{
  throw new Exception("AllowedOrigins configuration is missing");
}

builder.Services.AddCors(options =>
{
  options.AddPolicy(CorsPolicy, policy =>
  {
    policy.WithOrigins(allowedOrigins);
  });
});

var postgresConfig = builder.Configuration.GetSection("Postgres")
  .Get<PostgresConfiguration>();
if (postgresConfig is null)
{
  throw new Exception("Postgres configuration is missing");
}
builder.Services.AddPostgres(postgresConfig)
  .AddUtilityServices();

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(CorsPolicy);
app.MapUserEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
