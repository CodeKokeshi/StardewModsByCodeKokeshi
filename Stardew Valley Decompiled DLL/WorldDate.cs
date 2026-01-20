// Decompiled with JetBrains decompiler
// Type: StardewValley.WorldDate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

/// <summary>An in-game calendar date.</summary>
public class WorldDate : INetObject<NetFields>
{
  /// <summary>The number of months in a year.</summary>
  public const int MonthsPerYear = 4;
  /// <summary>The number of days per month.</summary>
  public const int DaysPerMonth = 28;
  /// <summary>The number of days per year.</summary>
  public const int DaysPerYear = 112 /*0x70*/;
  /// <summary>The backing field for <see cref="P:StardewValley.WorldDate.Year" />.</summary>
  private readonly NetInt year = new NetInt(1);
  /// <summary>The backing field for <see cref="P:StardewValley.WorldDate.Season" />.</summary>
  private readonly NetEnum<Season> season = new NetEnum<Season>(Season.Spring);
  /// <summary>The backing field for <see cref="P:StardewValley.WorldDate.DayOfMonth" />.</summary>
  private readonly NetInt dayOfMonth = new NetInt(1);

  /// <summary>The calendar year.</summary>
  public int Year
  {
    get => this.year.Value;
    set => this.year.Value = value;
  }

  /// <summary>The index of the calendar season (where 0 is spring, 1 is summer, 2 is fall, and 3 is winter).</summary>
  [XmlIgnore]
  public int SeasonIndex => (int) this.season.Value;

  /// <summary>The calendar day of month.</summary>
  public int DayOfMonth
  {
    get => this.dayOfMonth.Value;
    set => this.dayOfMonth.Value = value;
  }

  /// <summary>The day of week.</summary>
  public DayOfWeek DayOfWeek => WorldDate.GetDayOfWeekFor(this.DayOfMonth);

  /// <summary>The calendar season.</summary>
  [XmlIgnore]
  public Season Season
  {
    get => this.season.Value;
    set => this.season.Value = value;
  }

  /// <summary>The unique key for the calendar season (one of <c>spring</c>, <c>summer</c>, <c>fall</c>, or <c>winter</c>).</summary>
  [XmlElement("Season")]
  public string SeasonKey
  {
    get => Utility.getSeasonKey(this.season.Value);
    set
    {
      Season parsed;
      if (!Utility.TryParseEnum<Season>(value, out parsed))
        throw new ArgumentException($"Can't parse '{value}' as a season key.", nameof (value));
      this.season.Value = parsed;
    }
  }

  /// <summary>The number of days since the game began (starting at 1 for the first day of spring in Y1).</summary>
  [XmlIgnore]
  public int TotalDays
  {
    get => WorldDate.GetDaysPlayed(this.Year, this.Season, this.DayOfMonth);
    set
    {
      int num = value / 28;
      this.DayOfMonth = value % 28 + 1;
      this.Season = (Season) (num % 4);
      this.Year = num / 4 + 1;
    }
  }

  /// <summary>The number of weeks since the game began (starting at 1 for the first day of spring in Y1).</summary>
  public int TotalWeeks => this.TotalDays / 7;

  /// <summary>The number of Sundays since the game began (starting at 1 for the first day of spring in Y1).</summary>
  public int TotalSundayWeeks => (this.TotalDays + 1) / 7;

  public NetFields NetFields { get; } = new NetFields(nameof (WorldDate));

  /// <summary>Construct an instance.</summary>
  public WorldDate()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.year, nameof (year)).AddField((INetSerializable) this.season, nameof (season)).AddField((INetSerializable) this.dayOfMonth, nameof (dayOfMonth));
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="other">The date to copy.</param>
  public WorldDate(WorldDate other)
    : this()
  {
    this.Year = other.Year;
    this.Season = other.Season;
    this.DayOfMonth = other.DayOfMonth;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="year">The calendar year.</param>
  /// <param name="season">The calendar season.</param>
  /// <param name="dayOfMonth">The calendar day of month.</param>
  public WorldDate(int year, Season season, int dayOfMonth)
    : this()
  {
    this.Year = year;
    this.Season = season;
    this.DayOfMonth = dayOfMonth;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="year">The calendar year.</param>
  /// <param name="seasonKey">The unique key for the calendar season (one of <c>spring</c>, <c>summer</c>, <c>fall</c>, or <c>winter</c>).</param>
  /// <param name="dayOfMonth">The calendar day of month.</param>
  public WorldDate(int year, string seasonKey, int dayOfMonth)
    : this()
  {
    this.Year = year;
    this.SeasonKey = seasonKey;
    this.DayOfMonth = dayOfMonth;
  }

  /// <summary>Get a translated display text for the calendar date.</summary>
  public string Localize()
  {
    return Utility.getDateStringFor(this.DayOfMonth, this.SeasonIndex, this.Year);
  }

  /// <summary>Get a non-translated string representation for debug purposes.</summary>
  public override string ToString()
  {
    return $"Year {this.Year}, {this.SeasonKey} {this.DayOfMonth}, {this.DayOfWeek}";
  }

  /// <inheritdoc />
  public override bool Equals(object obj)
  {
    WorldDate worldDate = obj as WorldDate;
    return (object) worldDate != null && this.TotalDays == worldDate.TotalDays;
  }

  /// <inheritdoc />
  public override int GetHashCode() => this.TotalDays;

  /// <summary>Get whether two dates are equal.</summary>
  /// <param name="a">The first date to check.</param>
  /// <param name="b">The second date to check.</param>
  public static bool operator ==(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return totalDays1.GetValueOrDefault() == totalDays2.GetValueOrDefault() & totalDays1.HasValue == totalDays2.HasValue;
  }

  /// <summary>Get whether two dates are not equal.</summary>
  /// <param name="a">The first date to check.</param>
  /// <param name="b">The second date to check.</param>
  public static bool operator !=(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return !(totalDays1.GetValueOrDefault() == totalDays2.GetValueOrDefault() & totalDays1.HasValue == totalDays2.HasValue);
  }

  /// <summary>Get whether one date precedes another.</summary>
  /// <param name="a">The left date to check.</param>
  /// <param name="b">The right date to check.</param>
  public static bool operator <(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return totalDays1.GetValueOrDefault() < totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
  }

  /// <summary>Get whether one date postdates another.</summary>
  /// <param name="a">The left date to check.</param>
  /// <param name="b">The right date to check.</param>
  public static bool operator >(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return totalDays1.GetValueOrDefault() > totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
  }

  /// <summary>Get whether one date precedes or is equal to another.</summary>
  /// <param name="a">The left date to check.</param>
  /// <param name="b">The right date to check.</param>
  public static bool operator <=(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return totalDays1.GetValueOrDefault() <= totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
  }

  /// <summary>Get whether one date postdates or is equal to another.</summary>
  /// <param name="a">The left date to check.</param>
  /// <param name="b">The right date to check.</param>
  public static bool operator >=(WorldDate a, WorldDate b)
  {
    int? totalDays1 = a?.TotalDays;
    int? totalDays2 = b?.TotalDays;
    return totalDays1.GetValueOrDefault() >= totalDays2.GetValueOrDefault() & totalDays1.HasValue & totalDays2.HasValue;
  }

  /// <summary>Get the day of week for a day number.</summary>
  /// <param name="dayOfMonth">The day of month, between 1 and 28.</param>
  public static DayOfWeek GetDayOfWeekFor(int dayOfMonth) => (DayOfWeek) (dayOfMonth % 7);

  /// <summary>Get the current in-game date.</summary>
  public static WorldDate Now() => new WorldDate(Game1.year, Game1.season, Game1.dayOfMonth);

  /// <summary>Get the in-game date for a number of days played.</summary>
  /// <param name="daysPlayed">The number of days since the game began (starting at 1 for the first day of spring in Y1).</param>
  public static WorldDate ForDaysPlayed(int daysPlayed)
  {
    return new WorldDate() { TotalDays = daysPlayed };
  }

  /// <summary>Get the number of days since the game began (starting at 1 for the first day of spring in Y1).</summary>
  /// <param name="year">The calendar year.</param>
  /// <param name="season">The calendar season.</param>
  /// <param name="dayOfMonth">The calendar day of month.</param>
  public static int GetDaysPlayed(int year, Season season, int dayOfMonth)
  {
    return (int) ((year - 1) * 4 + season) * 28 + (dayOfMonth - 1);
  }

  /// <summary>Get the day of week from a string value, if valid.</summary>
  /// <param name="day">The numeric day of month (between 1 and 28), short English day name (like 'Mon'), or full English day name (like 'Monday').</param>
  /// <param name="dayOfWeek">The parsed day of week, if valid.</param>
  /// <returns>Returns whether the day of week was successfully parsed.</returns>
  public static bool TryGetDayOfWeekFor(string day, out DayOfWeek dayOfWeek)
  {
    int result;
    if (int.TryParse(day, out result))
    {
      dayOfWeek = WorldDate.GetDayOfWeekFor(result);
      return true;
    }
    string lower = day?.ToLower();
    if (lower != null)
    {
      switch (lower.Length)
      {
        case 3:
          switch (lower[1])
          {
            case 'a':
              if (lower == "sat")
                goto label_25;
              goto label_27;
            case 'e':
              if (lower == "wed")
                goto label_22;
              goto label_27;
            case 'h':
              if (lower == "thu")
                goto label_23;
              goto label_27;
            case 'o':
              if (lower == "mon")
                break;
              goto label_27;
            case 'r':
              if (lower == "fri")
                goto label_24;
              goto label_27;
            case 'u':
              switch (lower)
              {
                case "tue":
                  goto label_21;
                case "sun":
                  goto label_26;
                default:
                  goto label_27;
              }
            default:
              goto label_27;
          }
          break;
        case 6:
          switch (lower[0])
          {
            case 'f':
              if (lower == "friday")
                goto label_24;
              goto label_27;
            case 'm':
              if (lower == "monday")
                break;
              goto label_27;
            case 's':
              if (lower == "sunday")
                goto label_26;
              goto label_27;
            default:
              goto label_27;
          }
          break;
        case 7:
          if (lower == "tuesday")
            goto label_21;
          goto label_27;
        case 8:
          switch (lower[0])
          {
            case 's':
              if (lower == "saturday")
                goto label_25;
              goto label_27;
            case 't':
              if (lower == "thursday")
                goto label_23;
              goto label_27;
            default:
              goto label_27;
          }
        case 9:
          if (lower == "wednesday")
            goto label_22;
          goto label_27;
        default:
          goto label_27;
      }
      dayOfWeek = DayOfWeek.Monday;
      return true;
label_21:
      dayOfWeek = DayOfWeek.Tuesday;
      return true;
label_22:
      dayOfWeek = DayOfWeek.Wednesday;
      return true;
label_23:
      dayOfWeek = DayOfWeek.Thursday;
      return true;
label_24:
      dayOfWeek = DayOfWeek.Friday;
      return true;
label_25:
      dayOfWeek = DayOfWeek.Saturday;
      return true;
label_26:
      dayOfWeek = DayOfWeek.Sunday;
      return true;
    }
label_27:
    dayOfWeek = DayOfWeek.Sunday;
    return false;
  }
}
