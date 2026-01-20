// Decompiled with JetBrains decompiler
// Type: StardewValley.Enchantments.HaymakerEnchantment
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;

#nullable disable
namespace StardewValley.Enchantments;

public class HaymakerEnchantment : BaseWeaponEnchantment
{
  public override string GetName() => "Haymaker";

  protected override void _OnCutWeed(Vector2 tile_location, GameLocation location, Farmer who)
  {
    base._OnCutWeed(tile_location, location, who);
    if (Game1.random.NextBool())
      Game1.createItemDebris(ItemRegistry.Create("(O)771"), new Vector2((float) ((double) tile_location.X * 64.0 + 32.0), (float) ((double) tile_location.Y * 64.0 + 32.0)), -1);
    if (Game1.random.NextDouble() >= 0.33)
      return;
    if (GameLocation.StoreHayInAnySilo(1, location) == 0)
    {
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16 /*0x10*/, 16 /*0x10*/), 750f, 1, 0, who.Position - new Vector2(0.0f, 128f), false, false, who.Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0.0f, 0.0f)
      {
        motion = {
          Y = -1f
        },
        layerDepth = (float) (1.0 - (double) Game1.random.Next(100) / 10000.0),
        delayBeforeAnimationStart = Game1.random.Next(350)
      });
      Game1.addHUDMessage(HUDMessage.ForItemGained(ItemRegistry.Create("(O)178"), 1));
    }
    else
      Game1.createItemDebris(ItemRegistry.Create("(O)178").getOne(), new Vector2((float) ((double) tile_location.X * 64.0 + 32.0), (float) ((double) tile_location.Y * 64.0 + 32.0)), -1);
  }
}
