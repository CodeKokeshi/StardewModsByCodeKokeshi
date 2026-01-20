// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.LocationWarpRoute
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Pathfinding;

/// <summary>A possible path through location names that NPCs can take while pathfinding.</summary>
public class LocationWarpRoute
{
  /// <summary>The sequential location names that an NPC can pathfind through.</summary>
  public readonly string[] LocationNames;
  /// <summary>If set, this route can only be used by NPCs of the given gender.</summary>
  public readonly Gender? OnlyGender;

  /// <summary>Construct an instance.</summary>
  /// <param name="locationNames">The sequential location names that an NPC can pathfind through.</param>
  /// <param name="onlyGender">If set, this route can only be used by NPCs of the given gender.</param>
  public LocationWarpRoute(string[] locationNames, Gender? onlyGender)
  {
    this.LocationNames = locationNames;
    this.OnlyGender = onlyGender;
  }
}
