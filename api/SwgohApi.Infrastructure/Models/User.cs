namespace SwgohApi.Infrastructure.Models;

public class User
{
  public string Id { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public bool IsAdmin { get; set; }

  public List<EarnableShards> EarnableShards { get; set; } = [];
}
