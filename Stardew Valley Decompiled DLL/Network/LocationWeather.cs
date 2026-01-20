// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LocationWeather
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.GameData;
using StardewValley.GameData.LocationContexts;
using System;

#nullable disable
namespace StardewValley.Network;

public class LocationWeather : INetObject<NetFields>
{
  public readonly NetString weatherForTomorrow = new NetString();
  public readonly NetString weather = new NetString();
  public readonly NetBool isRaining = new NetBool();
  public readonly NetBool isSnowing = new NetBool();
  public readonly NetBool isLightning = new NetBool();
  public readonly NetBool isDebrisWeather = new NetBool();
  public readonly NetBool isGreenRain = new NetBool();
  public readonly NetInt monthlyNonRainyDayCount = new NetInt();

  public NetFields NetFields { get; } = new NetFields(nameof (LocationWeather));

  public string WeatherForTomorrow
  {
    get => this.weatherForTomorrow.Value;
    set => this.weatherForTomorrow.Value = value;
  }

  public string Weather
  {
    get => this.weather.Value;
    set => this.weather.Value = value;
  }

  public bool IsRaining
  {
    get => this.isRaining.Value;
    set => this.isRaining.Value = value;
  }

  public bool IsSnowing
  {
    get => this.isSnowing.Value;
    set => this.isSnowing.Value = value;
  }

  public bool IsLightning
  {
    get => this.isLightning.Value;
    set => this.isLightning.Value = value;
  }

  public bool IsDebrisWeather
  {
    get => this.isDebrisWeather.Value;
    set => this.isDebrisWeather.Value = value;
  }

  public bool IsGreenRain
  {
    get => this.isGreenRain.Value;
    set
    {
      this.isGreenRain.Value = value;
      if (!value)
        return;
      this.IsRaining = true;
    }
  }

  public LocationWeather()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.weatherForTomorrow, nameof (weatherForTomorrow)).AddField((INetSerializable) this.weather, nameof (weather)).AddField((INetSerializable) this.isRaining, nameof (isRaining)).AddField((INetSerializable) this.isSnowing, nameof (isSnowing)).AddField((INetSerializable) this.isLightning, nameof (isLightning)).AddField((INetSerializable) this.isDebrisWeather, nameof (isDebrisWeather)).AddField((INetSerializable) this.isGreenRain, nameof (isGreenRain)).AddField((INetSerializable) this.monthlyNonRainyDayCount, nameof (monthlyNonRainyDayCount));
  }

  public void InitializeDayWeather()
  {
    this.Weather = this.WeatherForTomorrow;
    this.IsRaining = false;
    this.IsSnowing = false;
    this.IsLightning = false;
    this.IsDebrisWeather = false;
    this.IsGreenRain = false;
  }

  public void UpdateDailyWeather(string locationContextId, LocationContextData data, Random random)
  {
    this.InitializeDayWeather();
    string id = this.WeatherForTomorrow;
    switch (id)
    {
      case "Rain":
        this.IsRaining = true;
        break;
      case "GreenRain":
        this.IsGreenRain = true;
        break;
      case "Storm":
        this.IsRaining = true;
        this.IsLightning = true;
        break;
      case "Wind":
        this.IsDebrisWeather = true;
        break;
      case "Snow":
        this.IsSnowing = true;
        break;
    }
    this.WeatherForTomorrow = "Sun";
    WorldDate worldDate = new WorldDate(Game1.Date);
    ++worldDate.TotalDays;
    if (Utility.isFestivalDay(worldDate.DayOfMonth, worldDate.Season, locationContextId))
      this.WeatherForTomorrow = "Festival";
    else if (Utility.TryGetPassiveFestivalDataForDay(worldDate.DayOfMonth, worldDate.Season, locationContextId, out id, out PassiveFestivalData _))
    {
      this.WeatherForTomorrow = "Sun";
    }
    else
    {
      foreach (WeatherCondition weatherCondition in data.WeatherConditions)
      {
        if (GameStateQuery.CheckConditions(weatherCondition.Condition, random: random))
        {
          this.WeatherForTomorrow = weatherCondition.Weather;
          break;
        }
      }
    }
  }

  public void CopyFrom(LocationWeather other)
  {
    this.Weather = other.Weather;
    this.IsRaining = other.IsRaining;
    this.IsSnowing = other.IsSnowing;
    this.IsLightning = other.IsLightning;
    this.IsDebrisWeather = other.IsDebrisWeather;
    this.IsGreenRain = other.IsGreenRain;
    this.WeatherForTomorrow = other.WeatherForTomorrow;
    this.monthlyNonRainyDayCount.Value = other.monthlyNonRainyDayCount.Value;
    if (this.Weather != null)
      return;
    if (this.IsLightning)
      this.Weather = "Storm";
    else if (this.IsRaining)
      this.Weather = "Rain";
    else if (this.IsSnowing)
      this.Weather = "Snow";
    else if (this.IsDebrisWeather)
      this.Weather = "Wind";
    else
      this.Weather = "Sun";
  }
}
