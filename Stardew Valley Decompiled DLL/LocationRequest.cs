// Decompiled with JetBrains decompiler
// Type: StardewValley.LocationRequest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley;

public class LocationRequest
{
  public string Name;
  public bool IsStructure;
  public GameLocation Location;

  public event LocationRequest.Callback OnLoad;

  public event LocationRequest.Callback OnWarp;

  public LocationRequest(string name, bool isStructure, GameLocation location)
  {
    this.Name = name;
    this.IsStructure = isStructure;
    this.Location = location;
  }

  public void Loaded(GameLocation location)
  {
    LocationRequest.Callback onLoad = this.OnLoad;
    if (onLoad == null)
      return;
    onLoad();
  }

  public void Warped(GameLocation location)
  {
    LocationRequest.Callback onWarp = this.OnWarp;
    if (onWarp != null)
      onWarp();
    Game1.player.ridingMineElevator = false;
    Game1.player.mount?.SyncPositionToRider();
    Game1.player.ClearCachedPosition();
    Game1.forceSnapOnNextViewportUpdate = true;
  }

  public bool IsRequestFor(GameLocation location)
  {
    if (!this.IsStructure && location.Name == this.Name)
      return true;
    return location.NameOrUniqueName == this.Name && location.isStructure.Value;
  }

  public override string ToString()
  {
    return $"LocationRequest({this.Name}, {this.IsStructure.ToString()})";
  }

  public delegate void Callback();
}
