using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Endpoints;

public static class EarnableEndpoints
{
  public static WebApplication MapEarnableEndpoints(this WebApplication app)
  {
    app.MapGroup("/characters")
      .RequireAuthorization()
      .MapEndpoints<InternalCharacter, Character>();

    app.MapGroup("/ship")
      .RequireAuthorization()
      .MapEndpoints<InternalShip, Ship>();

    return app;
  }

  private static RouteGroupBuilder MapEndpoints<TInternal, T>(this RouteGroupBuilder builder)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    builder.MapGet(string.Empty, GetEarnables<TInternal, T>)
      .AllowAnonymous();

    return builder;
  }

  public static async Task<Results<Ok<IEnumerable<T>>, ProblemHttpResult>> GetEarnables<TInternal, T>(
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> mapper)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    var earnables =  await earnableRepository.GetEarnables();
    return TypedResults.Ok(earnables.Select(mapper.MapTo));
  }
}
