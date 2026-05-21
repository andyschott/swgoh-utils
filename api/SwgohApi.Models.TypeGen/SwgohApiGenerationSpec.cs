using TypeGen.Core.SpecGeneration;

namespace SwgohApi.Models.TypeGen;

public class SwgohApiGenerationSpec : GenerationSpec
{
  public override void OnBeforeGeneration(OnBeforeGenerationArgs args)
  {
    var assembly = typeof(SwgohApi.Models.Users.UserDto).Assembly;
    var types = assembly.GetTypes();

    foreach (var type in types)
    {
      AddInterface(type);
    }

    base.OnBeforeGeneration(args);
  }
}
