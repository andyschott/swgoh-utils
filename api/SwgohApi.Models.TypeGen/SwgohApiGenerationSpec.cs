using TypeGen.Core.SpecGeneration;

namespace SwgohApi.Models.TypeGen;

public class SwgohApiGenerationSpec : GenerationSpec
{
  public override void OnBeforeGeneration(OnBeforeGenerationArgs args)
  {
    var assembly = typeof(Users.UserDto).Assembly;
    var types = assembly.GetTypes();

    foreach (var type in types)
    {
      if (type.IsEnum)
      {
        AddEnum(type);
      }
      else
      {
        AddInterface(type);
      }
    }

    base.OnBeforeGeneration(args);
  }
}
