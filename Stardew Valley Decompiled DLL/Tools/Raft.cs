// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Raft
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Tools;

public class Raft : Tool
{
  public Raft()
    : base(nameof (Raft), 0, 1, 1, false)
  {
    this.InstantUse = true;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Raft();

  protected override string loadDisplayName()
  {
    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Raft.cs.14204");
  }

  protected override string loadDescription()
  {
    return Game1.content.LoadString("Strings\\StringsFromCSFiles:Raft.cs.14205");
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    if (!who.isRafting && location.isWaterTile(x / 64 /*0x40*/, y / 64 /*0x40*/))
    {
      who.isRafting = true;
      Rectangle position = new Rectangle(x - 32 /*0x20*/, y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/);
      if (location.isCollidingPosition(position, Game1.viewport, (Character) who))
      {
        who.isRafting = false;
        return;
      }
      who.xVelocity = who.FacingDirection == 1 ? 3f : (who.FacingDirection == 3 ? -3f : 0.0f);
      who.yVelocity = who.FacingDirection == 2 ? 3f : (who.FacingDirection == 0 ? -3f : 0.0f);
      who.Position = new Vector2((float) (x - 32 /*0x20*/), (float) (y - 32 /*0x20*/ - 32 /*0x20*/ - (y < who.StandingPixel.Y ? 64 /*0x40*/ : 0)));
      if (this.PlayUseSounds)
        who.playNearbySoundAll("dropItemInWater");
    }
    this.CurrentParentTileIndex = this.IndexOfMenuItemView;
  }
}
