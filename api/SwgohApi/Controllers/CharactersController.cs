using Microsoft.AspNetCore.Mvc;

namespace SwgohApi.Controllers;

public class CharactersController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
