using Microsoft.AspNetCore.Mvc;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.Services;
using SwgohApi.ViewModels;
using EarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;

namespace SwgohApi.Controllers;

public class UserController : Controller
{
  private readonly ICharacterRepository _repository;
  private readonly IMapper<InternalCharacter, Character> _mapper;
  private readonly IComparer<Earnable> _userEarnableComparer;

  public UserController(ICharacterRepository repository,
    IMapper<InternalCharacter, Character> mapper,
    [FromKeyedServices(KeyedServiceNames.UserCharacterComparer)] IComparer<Earnable> userEarnableComparer)
  {
    _repository = repository;
    _mapper = mapper;
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

    var internalCharacters = await _repository.GetEarnablesForUser(user);
    var characters = internalCharacters.Select(internalCharacter =>
    {
      if (internalCharacter.CurrentEarnableShards is null)
      {
        internalCharacter.EarnableShards.Add(new EarnableShards
        {
          Id = string.Empty,
          Character = internalCharacter,
          CharacterId = internalCharacter.Id,
          Ship = null,
          ShipId = null,
          FarmingStatus = InternalFarmingStatus.Backlog,
          Shards = 0,
          User = user,
          UserId = user.Id
        });
      }

      return _mapper.MapTo(internalCharacter);
    }).Order(_userEarnableComparer)
    .Cast<Character>()
    .ToArray();

    var model = new UserEarnablesViewModel<Character>
    {
      Earnables = new UserCharactersTableViewModel(characters)
    };
    return View(model);
  }
}
