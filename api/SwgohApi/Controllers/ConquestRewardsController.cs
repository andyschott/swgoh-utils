using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.ViewModels;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Controllers;

public class ConquestRewardsController : Controller
{
  private readonly IConquestRewardRepository _repository;
  private readonly IMapper<InternalConquestReward, ConquestRewardDate> _mapper;

  public ConquestRewardsController(IConquestRewardRepository repository,
    IMapper<InternalConquestReward, ConquestRewardDate> mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IActionResult> Index()
  {
    var internalConquestRewards = await _repository.GetConquestRewards();
    var conquestRewards = internalConquestRewards.Select(_mapper.MapTo)
      .OrderByDescending(cr => cr.InitialUnlockDate);
    var model = new ConquestRewardsViewModel
    {
      ConquestRewards = new ConquestRewardsTableViewModel(conquestRewards)
    };
    return View(model);
  }
}
