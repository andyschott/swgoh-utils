using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;

namespace SwgohApi.Controllers;

public class CharactersController : Controller
{
  private readonly IEarnableRepository<InternalCharacter> _characterRepository;
  private readonly IMapper<InternalCharacter, Character> _mapper;

  public CharactersController(IEarnableRepository<InternalCharacter> characterRepository,
    IMapper<InternalCharacter, Character> mapper)
  {
    _characterRepository = characterRepository;
    _mapper = mapper;
  }

  public async Task<IActionResult> Index()
  {
    var internalCharacters = await _characterRepository.GetEarnables();
    var characters = internalCharacters.Select(_mapper.MapTo);
    var model = new CharactersViewModel
    {
      Characters = [.. characters]
    };
    return View(model);
  }
}
