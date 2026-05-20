using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SwgohApi.Auth;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.Services;
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
  new PasswordHasher<User>())
  .AddSingleton(TimeProvider.System);

var postgresConfig = builder.Configuration.GetSection("Postgres")
  .Get<PostgresConfiguration>();
if (postgresConfig is null)
{
  throw new Exception("Postgres configuration is missing");
}
builder.Services.AddPostgres(postgresConfig)
  .AddUtilityServices();

builder.Services.AddOptions<JwtOptions>()
  .Bind(builder.Configuration.GetSection("Jwt"))
  .ValidateDataAnnotations()
  .ValidateOnStart();

builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    var jwtOptions = builder.Configuration.GetSection("Jwt")
      .Get<JwtOptions>()
      ?? throw new Exception("Jwt configuration is missing");
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateIssuerSigningKey = true,
      ValidateLifetime = true,
      ValidIssuer = jwtOptions.Issuer,
      ValidAudience = jwtOptions.Audience,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
      ClockSkew = TimeSpan.FromSeconds(30)
    };
  });
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var allowCreatingUsers = builder.Configuration.GetSection("AllowCreatingUsers")
  .Get<bool>();

var app = builder.Build();

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

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
app.MapAuthEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
