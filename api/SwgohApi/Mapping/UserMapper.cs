using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Users;

namespace SwgohApi.Mapping;

public class UserMapper : IMapper<User, UserDto>
{
  public UserDto MapTo(User source)
  {
    return new UserDto(source.Id,
      source.Email,
      source.IsAdmin);
  }

  public User MapFrom(UserDto destination)
  {
    throw new NotImplementedException();
  }
}
