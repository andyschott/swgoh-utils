using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.ViewModels;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Controllers;

public class MarqueesController : Controller
{
  private readonly IMarqueeRepository _marqueeRepository;
  private readonly IMapper<InternalMarquee, MarqueeDate> _marqueeDateMapper;

  public MarqueesController(IMarqueeRepository marqueeRepository,
    IMapper<InternalMarquee, MarqueeDate> marqueeDateMapper)
  {
    _marqueeRepository = marqueeRepository;
    _marqueeDateMapper = marqueeDateMapper;
  }

  public async Task<IActionResult> Index()
  {
    var internalMarquees = await _marqueeRepository.GetMarquees();
    var marquees = internalMarquees.Select(_marqueeDateMapper.MapTo)
      .OrderByDescending(marquee => marquee.MarqueeEventDate);
    var model = new MarqueesViewModel
    {
      Marquees = new MarqueesTableViewModel(marquees)
    };
    return View(model);
  }
}
