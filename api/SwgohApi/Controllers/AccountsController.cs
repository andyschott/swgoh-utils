using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SwgohApi.Pages;
using SwgohApi.Services;
using SwgohApi.ViewModels;

namespace SwgohApi.Controllers;

public class AccountsController : Controller
{
  private readonly IAuthService _authService;

  public AccountsController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpGet]
  public IActionResult Login()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Login(LoginViewModel model)
  {
    if (!ModelState.IsValid)
    {
      return View(model);
    }

    var principle = await _authService.Login(model.Email, model.Password);
    if (principle is null)
    {
      ModelState.AddModelError(string.Empty, "Invalid login attempt");
      return View(model);
    }

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
      principle);
    return RedirectToAction("Index", "Home");
  }

  [HttpGet]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Index", "Home");
  }
}
