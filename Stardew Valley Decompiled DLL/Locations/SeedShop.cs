// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.SeedShop
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.Locations;

public class SeedShop : ShopLocation
{
  public SeedShop()
  {
  }

  public SeedShop(string map, string name)
    : base(map, name)
  {
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (Game1.player.maxItems.Value == 12)
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(456f, 1088f)), new Rectangle?(new Rectangle((int) byte.MaxValue, 1436, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1232f);
    else if (Game1.player.maxItems.Value < 36)
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(456f, 1088f)), new Rectangle?(new Rectangle(267, 1436, 12, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1232f);
    else
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Rectangle(452, 1184, 112 /*0x70*/, 20)), new Rectangle?(new Rectangle(258, 1449, 1, 1)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1232f);
  }
}
