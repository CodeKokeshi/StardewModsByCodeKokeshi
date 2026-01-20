// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Sewer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Sewer : GameLocation
{
  public const float steamZoom = 4f;
  public const float steamYMotionPerMillisecond = 0.1f;
  private Texture2D steamAnimation;
  private Vector2 steamPosition;
  private Color steamColor = new Color(200, (int) byte.MaxValue, 200);

  public Sewer()
  {
  }

  public Sewer(string map, string name)
    : base(map, name)
  {
    this.waterColor.Value = Color.LimeGreen;
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    for (float x = -1000f * Game1.options.zoomLevel + this.steamPosition.X; (double) x < (double) Game1.graphics.GraphicsDevice.Viewport.Width + 256.0; x += 256f)
    {
      for (float y = this.steamPosition.Y - 256f; (double) y < (double) (Game1.graphics.GraphicsDevice.Viewport.Height + 128 /*0x80*/); y += 256f)
        b.Draw(this.steamAnimation, new Vector2(x, y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/)), this.steamColor * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.steamPosition.Y -= (float) time.ElapsedGameTime.Milliseconds * 0.1f;
    this.steamPosition.Y %= -256f;
    this.steamPosition -= Game1.getMostRecentViewportMotion();
    if (Game1.random.NextDouble() >= 0.001)
      return;
    this.localSound("cavedrip");
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "st"))
    {
      case 21:
        Game1.warpFarmer("Town", 35, 97, 2);
        DelayedAction.playSoundAfterDelay("stairsdown", 250);
        return true;
      case 84:
        Utility.TryOpenShopMenu("ShadowShop", (string) null);
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    this.waterColor.Value = Color.LimeGreen * 0.75f;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.steamPosition = new Vector2(0.0f, 0.0f);
    this.steamAnimation = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\steamAnimation");
    Game1.ambientLight = new Color(250, 140, 160 /*0xA0*/);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (Game1.getCharacterFromName("Krobus").isMarried())
    {
      this.setMapTile(31 /*0x1F*/, 17, 84, "Buildings", "st");
      this.setMapTile(31 /*0x1F*/, 16 /*0x10*/, 1, "Front", "st");
    }
    else
    {
      this.removeMapTile(31 /*0x1F*/, 17, "Buildings");
      this.removeMapTile(31 /*0x1F*/, 16 /*0x10*/, "Front");
    }
  }
}
