namespace SwgohApi.Infrastructure.Models;

public record User
{
  public User(string id,
    string email,
    string password,
    bool isAdmin = false)
  {
    Id = id;
    Email = email;
    Password = password;
    IsAdmin = isAdmin;
  }

  public string Id { get; init; }
  public string Email { get; set; }
  public string Password { get; set; }
  public bool IsAdmin { get; set; }
}
