// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Club
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

#nullable disable
namespace StardewValley.Locations;

public class Club : GameLocation
{
  public static int timesPlayedCalicoJack;
  public static int timesPlayedSlots;
  private string coinBuffer;

  public Club()
  {
  }

  public Club(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.lightGlows.Clear();
    this.coinBuffer = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ? "　　" : "  ";
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
    if (Game1.random.NextDouble() >= 0.002)
      return;
    this.localSound("boop");
  }

  public override void drawOverlays(SpriteBatch b)
  {
    if (Game1.currentMinigame == null)
    {
      SpriteText.drawStringWithScrollBackground(b, this.coinBuffer + Game1.player.clubCoins.ToString(), 64 /*0x40*/, 16 /*0x10*/);
      Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(68f, 20f), new Rectangle(211, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 1f);
    }
    base.drawOverlays(b);
  }
}
