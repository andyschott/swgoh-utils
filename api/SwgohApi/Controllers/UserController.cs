using Microsoft.AspNetCore.Mvc;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Mapping;
using SwgohApi.Services;
using SwgohApi.ViewModels;
using Character = SwgohApi.Models.Earnables.Character;
using Earnable = SwgohApi.Models.Earnables.Earnable;
using Ship = SwgohApi.Models.Earnables.Ship;
using EarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Controllers;

public class UserController : Controller
{
  private readonly ICharacterRepository _characterRepository;
  private readonly IMapper<InternalCharacter, Character> _characterMapper;
  private readonly IShipRepository _shipRepository;
  private readonly IMapper<InternalShip, Ship> _shipMapper;
  private readonly IComparer<Earnable> _userEarnableComparer;

  public UserController(ICharacterRepository characterRepository,
    IMapper<InternalCharacter, Character> characterMapper,
    IShipRepository shipRepository,
    IMapper<InternalShip, Ship> shipMapper,
    [FromKeyedServices(KeyedServiceNames.UserCharacterComparer)] IComparer<Earnable> userEarnableComparer)
  {
    _characterRepository = characterRepository;
    _characterMapper = characterMapper;
    _shipRepository = shipRepository;
    _shipMapper = shipMapper;
    _userEarnableComparer = userEarnableComparer;
  }

  [HttpGet]
  public async Task<IActionResult> CharactersIndex()
  {
    var user = HttpContext.RequestingUser;
    if (user is null)
    {
      // TODO: handle error
      return View();
    }

    var model = await CreateViewModel(user,
      _characterRepository,
      _characterMapper,
      characters => new UserCharactersTableViewModel(characters));
    return View(model);
  }

  [HttpGet]
  public async Task<IActionResult> ShipsIndex()
  {
    var user = HttpContext.RequestingUser;
    if (user is null)
    {
      // TODO: handle error
      return View();
    }

    var model = await CreateViewModel(user,
      _shipRepository,
      _shipMapper,
      ships => new UserShipsTableViewModel(ships));
    return View(model);
  }

  private async Task<UserEarnablesViewModel<T>> CreateViewModel<T, TInternal>(
    User user,
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> mapper,
    Func<IEnumerable<T>, UserEarnablesTableViewModel<T>> createTableViewModel)
  where T : Earnable
  where TInternal : InternalEarnable
  {
    var internalEarnables = await earnableRepository.GetEarnablesForUser(user);
    var earnables = internalEarnables.Select(internalEarnable =>
      {
        if (internalEarnable.CurrentEarnableShards is null)
        {
          internalEarnable.EarnableShards.Add(new EarnableShards
          {
            Id = string.Empty,
            Character = null,
            CharacterId = null,
            Ship = null,
            ShipId = null,
            FarmingStatus = InternalFarmingStatus.Backlog,
            Shards = 0,
            User = user,
            UserId = user.Id
          });
        }

        return mapper.MapTo(internalEarnable);
      }).Order(_userEarnableComparer)
      .Cast<T>()
      .ToArray();

    return new UserEarnablesViewModel<T>
    {
      Earnables = createTableViewModel(earnables)
    };
  }
}
