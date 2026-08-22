using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwgohApi.Configuration;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.Services;
using SwgohApi.Infrastructure.Utilities;
using SwgohApi.Mapping;
using SwgohApi.Middleware;
using SwgohApi.Models.TerritoryBattles;

const string CorsPolicy = "AllowedOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("roteRewards.json", false);

var roteSection = builder.Configuration.GetSection("RiseOfTheEmpire");
builder.Services.Configure<RiseOfTheEmpire>(roteSection);

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins")
  .Get<string[]>();
if (allowedOrigins is null)
{
  throw new Exception("AllowedOrigins configuration is missing");
}

builder.Services.Configure<UserEndpointsConfiguration>(builder.Configuration.GetSection("UserEndpoints"));

builder.Services.AddCors(options =>
{
  options.AddPolicy(CorsPolicy, policy =>
  {
    policy.WithOrigins(allowedOrigins)
      .AllowAnyHeader()
      .AllowAnyMethod();
  });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IPasswordHasher<User>>(_ =>
  new PasswordHasher<User>())
  .AddSingleton(TimeProvider.System)
  .AddValidation();

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

builder.Services.AddSingleton<ITokenService, JwtTokenService>()
  .AddScoped<IAuthService, AuthService>()
  .AddMappers();

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

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(CorsPolicy);
app.UseStaticFiles();

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

app.UseMiddleware<RequestingUserMiddleware>();

app.MapUserEndpoints()
  .MapAuthEndpoints()
  .MapEarnableEndpoints()
  .MapMarqueeEndpoints()
  .MapConquestRewardEndpoints()
  .MapEarnableShardsEndpoints();
app.MapControllerRoute("default",
  "{controller=Home}/{action=Index}/{id?}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var rote = app.Services.GetService<IOptions<RiseOfTheEmpire>>();

app.Run();
