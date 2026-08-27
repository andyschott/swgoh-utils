using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.ViewModels;
using EarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using FarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;

namespace SwgohApi.Controllers;

public class CharactersForUserController : Controller
{
  private readonly IUserRepository _userRepository;
  private readonly ICharacterRepository _repository;
  private readonly IMapper<InternalCharacter, Character> _mapper;

  public CharactersForUserController(IUserRepository userRepository,
    ICharacterRepository repository,
    IMapper<InternalCharacter, Character> mapper)
  {
    _userRepository = userRepository;
    _repository = repository;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> Index()
  {
    if (User.Identity?.IsAuthenticated != true)
    {
      // TODO: handle error
      return View();
    }

    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
      // TODO: handle error
      return View();
    }

    var user = await _userRepository.GetUserById(userId);
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
          FarmingStatus = FarmingStatus.Backlog,
          Shards = 0,
          User = user,
          UserId = userId
        });
      }

      return _mapper.MapTo(internalCharacter);
    }).ToArray();

    var model = new UserEarnablesViewModel<Character>
    {
      Earnables = new UserCharactersTableViewModel(characters)
    };
    return View(model);
  }
}
