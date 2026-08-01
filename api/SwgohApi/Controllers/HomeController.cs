using Microsoft.AspNetCore.Mvc;

namespace SwgohApi.Controllers;

public class HomeController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
