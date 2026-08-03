using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.ViewModels;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Controllers;

public class ShipsController : Controller
{
  private readonly IEarnableRepository<InternalShip> _shipRepository;
  private readonly IMapper<InternalShip, Ship> _mapper;

  public ShipsController(IEarnableRepository<InternalShip> shipRepository,
    IMapper<InternalShip, Ship> mapper)
  {
    _shipRepository = shipRepository;
    _mapper = mapper;
  }

  public async Task<IActionResult> Index()
  {
    var internalShips = await _shipRepository.GetEarnables();
    var ships = internalShips.Select(_mapper.MapTo)
      .OrderBy(ship => ship.Name);
    var model = new ShipsViewModel
    {
      Ships = new ShipsTableViewModel(ships)
    };
    return View(model);
  }
}
