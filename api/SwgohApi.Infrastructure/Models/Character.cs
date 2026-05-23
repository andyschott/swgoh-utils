namespace SwgohApi.Infrastructure.Models;

public class Character : Earnable
{
  public override EarnableType Type => EarnableType.Character;

  public bool IsAccelerated { get; set; }
}
