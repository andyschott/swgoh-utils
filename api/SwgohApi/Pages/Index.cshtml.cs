using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SwgohApi.Pages;

public class IndexModel : PageModel
{
  public string Message { get; set; } = "World";

  public void OnGet()
  {
  }
}
