// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Mine
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Locations;

public class Mine : GameLocation
{
  public Mine()
  {
  }

  public Mine(string map, string name)
    : base(map, name)
  {
    Vector2 boulderPosition = this.GetBoulderPosition();
    this.objects.Add(boulderPosition, new Object(boulderPosition, "78"));
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    MineShaft.mushroomLevelsGeneratedToday.Clear();
  }

  /// <summary>Get the tile position for the boulder which initially blocks access to the dwarf.</summary>
  public Vector2 GetBoulderPosition() => new Vector2(27f, 8f);
}
