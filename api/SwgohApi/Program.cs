using System.Net;
using Microsoft.AspNetCore.Identity;
using SwgohApi.Infrastructure.Models;
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

builder.Services.AddSingleton<IPasswordHasher<User>>(_ =>
  new PasswordHasher<User>());

var postgresConfig = builder.Configuration.GetSection("Postgres")
  .Get<PostgresConfiguration>();
if (postgresConfig is null)
{
  throw new Exception("Postgres configuration is missing");
}
builder.Services.AddPostgres(postgresConfig)
  .AddUtilityServices();

builder.Services.AddOpenApi();

var allowCreatingUsers = builder.Configuration.GetSection("AllowCreatingUsers")
  .Get<bool>();

var app = builder.Build();

app.UseCors(CorsPolicy);

app.UseExceptionHandler(exceptionApp =>
{
  exceptionApp.Run(async context =>
  {
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

    await context.Response.WriteAsJsonAsync(new
    {
      Error = "Internal Server Error"
    });
  });
});

app.MapUserEndpoints(allowCreatingUsers);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
