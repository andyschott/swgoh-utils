using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class MarqueesTableViewModel : ITableViewModel
{
  private readonly Dictionary<string, MarqueeDate> _marquees;

  private const string NameColumn = "Name";
  private const string IntroductionDateColumn = "Introduction Date";
  private const string MarqueeEventDateColumn = "Marquee Event Date";
  private const string ShipmentDateColumn = "Shipment Date";
  private const string FarmDate =  "Farm Date";
  private const string AccelerationDate = "Acceleration Date";

  public MarqueesTableViewModel(IEnumerable<MarqueeDate> marquees)
  {
    _marquees = marquees.ToDictionary(marquee => marquee.Name);
  }

  public string Title => "Marquees";

  public IEnumerable<string> Columns { get; } =
  [
    NameColumn,
    IntroductionDateColumn,
    MarqueeEventDateColumn,
    ShipmentDateColumn,
    FarmDate,
    AccelerationDate
  ];

  public IEnumerable<string> Items => _marquees.Keys;

  public string GetText(string id, string column)
  {
    var marquee = _marquees[id];
    return column switch
    {
      NameColumn => marquee.Name,
      IntroductionDateColumn => FormatDate(marquee.IntroductionDate),
      MarqueeEventDateColumn => FormatDate(marquee.MarqueeEventDate),
      ShipmentDateColumn => FormatDate(marquee.ShipmentDate),
      FarmDate => FormatDate(marquee.FarmDate),
      AccelerationDate => FormatDate(marquee.AccelerationDate),
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  private static string FormatDate(DateOnly? date)
  {
    if (date is null)
    {
      return string.Empty;
    }

    return date.Value.ToString("d");
  }
}
