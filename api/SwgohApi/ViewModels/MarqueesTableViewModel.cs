using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class MarqueesTableViewModel : DatesTableViewModel
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

  public override string Title => "Marquees";

  public override IEnumerable<string> Columns { get; } =
  [
    NameColumn,
    IntroductionDateColumn,
    MarqueeEventDateColumn,
    ShipmentDateColumn,
    FarmDate,
    AccelerationDate
  ];

  public override IEnumerable<string> Items => _marquees.Keys;

  public override string DefaultSortColumn => IntroductionDateColumn;
  public override bool DefaultSortAscending => false;

  public override string GetText(string id, string column)
  {
    var marquee = _marquees[id];
    return column switch
    {
      NameColumn => marquee.Name,
      _ => FormatDate(GetDateForColumn(marquee, column)),
    };
  }

  public string GetDataType(string column)
  {
    return column switch
    {
      NameColumn => "text",
      IntroductionDateColumn => "date",
      MarqueeEventDateColumn => "date",
      ShipmentDateColumn => "date",
      FarmDate => "date",
      AccelerationDate => "date",
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  protected override DateOnly? GetDateForColumn(string id, string column)
  {
    var marquee = _marquees[id];
    return GetDateForColumn(marquee, column);
  }

  private static DateOnly? GetDateForColumn(MarqueeDate marquee, string column)
  {
    return column switch
    {
      NameColumn => null,
      IntroductionDateColumn => marquee.IntroductionDate,
      MarqueeEventDateColumn => marquee.MarqueeEventDate,
      ShipmentDateColumn => marquee.ShipmentDate,
      FarmDate => marquee.FarmDate,
      AccelerationDate => marquee.AccelerationDate,
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
