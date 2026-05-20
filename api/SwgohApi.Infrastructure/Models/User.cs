namespace SwgohApi.Infrastructure.Models;

public record User
{
  public User(string id,
    string email,
    string password)
  {
    Id = id;
    Email = email;
    Password = password;
  }

  public string Id { get; init; }
  public string Email { get; set; }
  public string Password { get; set; }
}
