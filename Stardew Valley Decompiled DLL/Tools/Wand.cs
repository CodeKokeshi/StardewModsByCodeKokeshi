// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Wand
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Locations;

#nullable disable
namespace StardewValley.Tools;

public class Wand : Tool
{
  public Wand()
    : base("Return Scepter", 0, 2, 2, false)
  {
    this.InstantUse = true;
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId() => this.ItemId = "ReturnScepter";

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Wand();

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    if (who.bathingClothes.Value || !who.IsLocalPlayer || who.onBridge.Value)
      return;
    this.indexOfMenuItemView.Value = 2;
    this.CurrentParentTileIndex = 2;
    for (int index = 0; index < 12; ++index)
      Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.position.X - 256 /*0x0100*/, (int) who.position.X + 192 /*0xC0*/), (float) Game1.random.Next((int) who.position.Y - 256 /*0x0100*/, (int) who.position.Y + 192 /*0xC0*/)), false, Game1.random.NextBool()));
    if (this.PlayUseSounds)
      who.playNearbySoundAll("wand");
    Game1.displayFarmer = false;
    who.temporarilyInvincible = true;
    who.temporaryInvincibilityTimer = -2000;
    who.Halt();
    who.faceDirection(2);
    who.CanMove = false;
    who.freezePause = 2000;
    Game1.flashAlpha = 1f;
    DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.wandWarpForReal), 1000);
    Rectangle boundingBox = who.GetBoundingBox();
    new Rectangle(boundingBox.X, boundingBox.Y, 64 /*0x40*/, 64 /*0x40*/).Inflate(192 /*0xC0*/, 192 /*0xC0*/);
    int num = 0;
    Point tilePoint = who.TilePoint;
    for (int x1 = tilePoint.X + 8; x1 >= tilePoint.X - 8; --x1)
    {
      Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2((float) x1, (float) tilePoint.Y) * 64f, Color.White, animationInterval: 50f)
      {
        layerDepth = 1f,
        delayBeforeAnimationStart = num * 25,
        motion = new Vector2(-0.25f, 0.0f)
      });
      ++num;
    }
    this.CurrentParentTileIndex = this.IndexOfMenuItemView;
  }

  /// <inheritdoc />
  public override bool actionWhenPurchased(string shopId)
  {
    Game1.player.mailReceived.Add("ReturnScepter");
    return base.actionWhenPurchased(shopId);
  }

  private void wandWarpForReal()
  {
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
    if (homeOfFarmer == null)
      return;
    Point frontDoorSpot = homeOfFarmer.getFrontDoorSpot();
    Game1.warpFarmer("Farm", frontDoorSpot.X, frontDoorSpot.Y, false);
    Game1.fadeToBlackAlpha = 0.99f;
    Game1.screenGlow = false;
    this.lastUser.temporarilyInvincible = false;
    this.lastUser.temporaryInvincibilityTimer = 0;
    Game1.displayFarmer = true;
    this.lastUser.CanMove = true;
  }
}
