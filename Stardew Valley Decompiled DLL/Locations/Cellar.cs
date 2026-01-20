// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Cellar
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using System;

#nullable disable
namespace StardewValley.Locations;

public class Cellar : GameLocation
{
  public Cellar()
  {
  }

  public Cellar(string mapPath, string name)
    : base(mapPath, name)
  {
    this.setUpAgingBoards();
  }

  public void setUpAgingBoards()
  {
    for (int x = 6; x < 17; ++x)
    {
      Vector2 vector2 = new Vector2((float) x, 8f);
      if (!this.objects.ContainsKey(vector2))
        this.objects.Add(vector2, (StardewValley.Object) new Cask(vector2));
      vector2 = new Vector2((float) x, 10f);
      if (!this.objects.ContainsKey(vector2))
        this.objects.Add(vector2, (StardewValley.Object) new Cask(vector2));
      vector2 = new Vector2((float) x, 12f);
      if (!this.objects.ContainsKey(vector2))
        this.objects.Add(vector2, (StardewValley.Object) new Cask(vector2));
    }
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    string target = "Farmhouse";
    bool targetFound = false;
    Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
    {
      if (!(location is Cabin cabin2) || !(cabin2.GetCellarName() == this.Name))
        return true;
      target = cabin2.NameOrUniqueName;
      targetFound = true;
      return false;
    }));
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) this.warps)
      warp.TargetName = target;
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    b.Draw(Game1.staminaRect, new Rectangle(-Game1.viewport.X, -Game1.viewport.Y - 256 /*0x0100*/, 512 /*0x0200*/, 256 /*0x0100*/), Color.Black);
    base.drawAboveAlwaysFrontLayer(b);
  }
}
