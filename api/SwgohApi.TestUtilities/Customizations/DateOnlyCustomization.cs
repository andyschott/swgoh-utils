using AutoFixture;

namespace SwgohApi.TestUtilities.Customizations;

public class DateOnlyCustomization : ICustomization
{
  public void Customize(IFixture fixture)
  {
    var intGenerator = fixture.Create<Generator<int>>();
    fixture.Register(() =>
    {
      using var enumerator = intGenerator.GetEnumerator();
      enumerator.MoveNext();

      var year = 2020 + enumerator.Current % 20;

      enumerator.MoveNext();
      var month = (enumerator.Current % 12) + 1;

      enumerator.MoveNext();
      var day = (enumerator.Current % 28) + 1;

      return new DateOnly(year, month, day);
    });
  }
}
