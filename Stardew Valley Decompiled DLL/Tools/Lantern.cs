// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Lantern
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class Lantern : Tool
{
  public const float baseRadius = 10f;
  public const int millisecondsPerFuelUnit = 6000;
  public const int maxFuel = 100;
  public int fuelLeft;
  private int fuelTimer;
  public bool on;
  [XmlIgnore]
  public string lightSourceId;

  public Lantern()
    : base(nameof (Lantern), 0, 74, 74, false)
  {
    this.InstantUse = true;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Lantern();

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    this.on = !this.on;
    this.CurrentParentTileIndex = this.IndexOfMenuItemView;
    Utility.removeLightSource(this.lightSourceId);
    if (!this.on)
      return;
    this.lightSourceId = this.GenerateLightSourceId(who);
    Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 1, new Vector2(who.Position.X + 21f, who.Position.Y + 64f), (float) (2.5 + (double) this.fuelLeft / 100.0 * 10.0 * 0.75), new Color(0, 131, (int) byte.MaxValue)));
  }

  public override void tickUpdate(GameTime time, Farmer who)
  {
    if (this.on && this.fuelLeft > 0 && Game1.drawLighting)
    {
      this.fuelTimer += time.ElapsedGameTime.Milliseconds;
      if (this.fuelTimer > 6000)
      {
        --this.fuelLeft;
        this.fuelTimer = 0;
      }
      Vector2 position = new Vector2(who.Position.X + 21f, who.Position.Y + 64f);
      LightSource lightSource;
      if (Game1.currentLightSources.TryGetValue(this.lightSourceId, out lightSource))
      {
        lightSource.position.Value = position;
      }
      else
      {
        this.lightSourceId = this.GenerateLightSourceId(who);
        Game1.currentLightSources.Add(new LightSource(this.lightSourceId, 1, position, (float) (2.5 + (double) this.fuelLeft / 100.0 * 10.0 * 0.75), new Color(0, 131, (int) byte.MaxValue)));
      }
    }
    if (!this.on || this.fuelLeft > 0)
      return;
    Utility.removeLightSource(this.GenerateLightSourceId(who));
  }
}
