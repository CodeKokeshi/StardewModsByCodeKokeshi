// Decompiled with JetBrains decompiler
// Type: StardewValley.Shed
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;

#nullable disable
namespace StardewValley;

public class Shed : DecoratableLocation
{
  private bool isRobinUpgrading;

  public Shed()
  {
  }

  public Shed(string m, string name)
    : base(m, name)
  {
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.isDarkOut((GameLocation) this))
      Game1.ambientLight = new Color(180, 180, 0);
    Building underConstruction = Game1.GetBuildingUnderConstruction();
    this.isRobinUpgrading = underConstruction != null && underConstruction.HasIndoorsName(this.NameOrUniqueName);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (!this.isRobinUpgrading)
      return;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(64f, 64f)), new Rectangle?(new Rectangle(90, 0, 33, 6)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01546f);
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(64f, 84f)), new Rectangle?(new Rectangle(90, 0, 33, 31 /*0x1F*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0153600005f);
  }
}
