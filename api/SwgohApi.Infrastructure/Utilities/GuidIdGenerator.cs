namespace SwgohApi.Infrastructure.Utilities;

public class GuidIdGenerator: IIdGenerator
{
  public string CreateId() => Guid.NewGuid().ToString();
}
