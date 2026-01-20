// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.CosmeticPlant
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class CosmeticPlant : Grass
{
  [XmlElement("flipped")]
  public readonly NetBool flipped = new NetBool();
  [XmlElement("xOffset")]
  private readonly NetInt xOffset = new NetInt();
  [XmlElement("yOffset")]
  private readonly NetInt yOffset = new NetInt();

  public CosmeticPlant()
  {
  }

  public CosmeticPlant(int which)
    : base(which, 1)
  {
    this.flipped.Value = Game1.random.NextBool();
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.flipped, "flipped").AddField((INetSerializable) this.xOffset, "xOffset").AddField((INetSerializable) this.yOffset, "yOffset");
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X * 64.0 + 16.0), (int) (((double) tile.Y + 1.0) * 64.0 - 8.0 - 4.0), 8, 8);
  }

  public override string textureName() => "TerrainFeatures\\upperCavePlants";

  public override void loadSprite()
  {
    this.xOffset.Value = Game1.random.Next(-2, 3) * 4;
    this.yOffset.Value = Game1.random.Next(-2, 1) * 4;
  }

  public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    if (t is MeleeWeapon meleeWeapon && meleeWeapon.type.Value != 2 || explosion > 0)
    {
      this.shake(3f * (float) Math.PI / 32f, (float) Math.PI / 40f, Game1.random.NextBool());
      int num = explosion > 0 ? Math.Max(1, explosion + 2 - Game1.random.Next(2)) : (t.upgradeLevel.Value == 3 ? 3 : t.upgradeLevel.Value + 1);
      Game1.createRadialDebris(location, this.textureName(), new Rectangle((int) this.grassType.Value * 16 /*0x10*/, 6, 7, 6), (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 14));
      this.numberOfWeeds.Value -= num;
      if (this.numberOfWeeds.Value <= 0)
      {
        Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0, (double) Game1.CurrentMineLevel, (double) Game1.player.timesReachedMineBottom);
        if (random.NextDouble() < 0.005)
          Game1.createObjectDebris("(O)114", (int) tileLocation.X, (int) tileLocation.Y, -1, 0, 1f, location);
        else if (random.NextDouble() < 0.01)
          Game1.createDebris(4, (int) tileLocation.X, (int) tileLocation.Y, random.Next(1, 2), location);
        else if (random.NextDouble() < 0.02)
          Game1.createDebris(92, (int) tileLocation.X, (int) tileLocation.Y, random.Next(2, 4), location);
        return true;
      }
    }
    return false;
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f) + new Vector2((float) (32 /*0x20*/ + this.xOffset.Value), (float) (60 + this.yOffset.Value))), new Rectangle?(new Rectangle((int) this.grassType.Value * 16 /*0x10*/, 0, 16 /*0x10*/, 24)), Color.White, this.shakeRotation, new Vector2(8f, 23f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) (this.getBoundingBox().Y - 4) + (double) tile.X / 900.0 + 0.0099999997764825821) / 10000.0));
  }
}
