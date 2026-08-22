using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwgohApi.Models.TerritoryBattles;
using SwgohApi.ViewModels;

namespace SwgohApi.Controllers;

public class RiseOfTheEmpireRewardsController : Controller
{
  private readonly RiseOfTheEmpireRewards[] _rewards;

  public RiseOfTheEmpireRewardsController(IOptions<RiseOfTheEmpire> rote)
  {
    _rewards = rote.Value.Rewards
      .OrderByDescending(reward => reward.Stars)
      .ToArray();
  }

  public IActionResult Index()
  {
    var model = new RiseOfTheEmpireRewardsViewModel
    {
      Rewards = new RoteRewardsTableViewModel(_rewards)
    };
    return View(model);
  }
}
