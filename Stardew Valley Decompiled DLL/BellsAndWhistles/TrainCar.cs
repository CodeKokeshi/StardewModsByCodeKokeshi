// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.TrainCar
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class TrainCar : INetObject<NetFields>
{
  public const int spotsForTopFeatures = 6;
  public const double chanceForTopFeature = 0.2;
  public const int engine = 3;
  public const int passengerCar = 2;
  public const int coalCar = 1;
  public const int plainCar = 0;
  public const int coal = 0;
  public const int metal = 1;
  public const int wood = 2;
  public const int compartments = 3;
  public const int grass = 4;
  public const int hay = 5;
  public const int bricks = 6;
  public const int rocks = 7;
  public const int packages = 8;
  public const int presents = 9;
  public readonly NetInt frontDecal = new NetInt();
  public readonly NetInt carType = new NetInt();
  public readonly NetInt resourceType = new NetInt();
  public readonly NetInt loaded = new NetInt();
  public readonly NetArray<int, NetInt> topFeatures = new NetArray<int, NetInt>(6);
  public readonly NetBool alternateCar = new NetBool();
  public readonly NetColor color = new NetColor();

  public NetFields NetFields { get; } = new NetFields(nameof (TrainCar));

  [Obsolete("This constructor is for deserialization and shouldn't be called directly.")]
  public TrainCar() => this.initNetFields();

  public TrainCar(
    Random random,
    int carType,
    int frontDecal,
    Color color,
    int resourceType = 0,
    int loaded = 0)
    : this()
  {
    this.carType.Value = carType;
    this.frontDecal.Value = frontDecal;
    this.color.Value = color;
    this.resourceType.Value = resourceType;
    this.loaded.Value = loaded;
    if (carType != 0 && carType != 1)
      this.color.Value = Color.White;
    if (carType != 0)
    {
      if (carType != 2 || !random.NextBool())
        return;
      this.alternateCar.Value = true;
    }
    else
    {
      if (color.Equals(Color.DimGray))
        return;
      for (int index = 0; index < this.topFeatures.Count; ++index)
        this.topFeatures[index] = random.NextDouble() >= 0.2 ? -1 : random.Next(2);
    }
  }

  private void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.frontDecal, "frontDecal").AddField((INetSerializable) this.carType, "carType").AddField((INetSerializable) this.resourceType, "resourceType").AddField((INetSerializable) this.loaded, "loaded").AddField((INetSerializable) this.topFeatures, "topFeatures").AddField((INetSerializable) this.alternateCar, "alternateCar").AddField((INetSerializable) this.color, "color");
  }

  public void draw(
    SpriteBatch b,
    Vector2 globalPosition,
    float wheelRotation,
    GameLocation location)
  {
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(new Rectangle(192 /*0xC0*/ + this.carType.Value * 128 /*0x80*/, 512 /*0x0200*/ - (this.alternateCar.Value ? 64 /*0x40*/ : 0), 128 /*0x80*/, 57)), this.color.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 256.0) / 10000.0));
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(0.0f, 228f)), new Rectangle?(new Rectangle(192 /*0xC0*/ + this.carType.Value * 128 /*0x80*/, 569, 128 /*0x80*/, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 256.0) / 10000.0));
    switch (this.carType.Value)
    {
      case 0:
        for (int index = 0; index < this.topFeatures.Count; index += 64 /*0x40*/)
        {
          if (this.topFeatures[index] != -1)
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2((float) (64 /*0x40*/ + index), 20f)), new Rectangle?(new Rectangle(192 /*0xC0*/, 608 + this.topFeatures[index] * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
        }
        this.DrawFrontDecal(b, globalPosition);
        break;
      case 1:
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(new Rectangle(448 + this.resourceType.Value * 128 /*0x80*/ % 256 /*0x0100*/, 576 + this.resourceType.Value / 2 * 32 /*0x20*/, 128 /*0x80*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
        if (this.loaded.Value > 0 && Game1.random.NextDouble() < 0.02 && (double) globalPosition.X > 320.0 && (double) globalPosition.X < (double) (location.map.DisplayWidth - 256 /*0x0100*/))
        {
          --this.loaded.Value;
          string id = (string) null;
          switch (this.resourceType.Value)
          {
            case 0:
              id = "(O)382";
              break;
            case 1:
              id = (int) this.color.R > (int) this.color.G ? "(O)378" : ((int) this.color.G > (int) this.color.B ? "(O)380" : ((int) this.color.B > (int) this.color.R ? "(O)384" : "(O)378"));
              break;
            case 2:
              id = Game1.random.NextDouble() < 0.05 ? "(O)709" : "(O)388";
              break;
            case 6:
              id = "(O)390";
              break;
            case 7:
              id = location.IsWinterHere() ? "(O)536" : (Game1.stats.DaysPlayed <= 120U || (int) this.color.R <= (int) this.color.G ? "(O)535" : "(O)537");
              break;
            case 9:
              if (Utility.tryRollMysteryBox(0.02))
              {
                id = "(O)MysteryBox";
                break;
              }
              break;
          }
          if (id != null)
            Game1.createObjectDebris(id, (int) globalPosition.X / 64 /*0x40*/ + 2, (int) globalPosition.Y / 64 /*0x40*/, (int) ((double) globalPosition.Y + 320.0));
          if (Game1.random.NextDouble() < 0.01)
            Game1.createItemDebris(ItemRegistry.Create("(B)806"), new Vector2((float) ((int) globalPosition.X + 128 /*0x80*/), (float) (int) globalPosition.Y), (int) ((double) globalPosition.Y + 320.0));
        }
        this.DrawFrontDecal(b, globalPosition);
        break;
      case 3:
        Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(72f, 208f));
        Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(316f, 208f));
        b.Draw(Game1.mouseCursors, local1, new Rectangle?(new Rectangle(192 /*0xC0*/, 576, 20, 20)), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(228f, 208f)), new Rectangle?(new Rectangle(192 /*0xC0*/, 576, 20, 20)), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
        b.Draw(Game1.mouseCursors, local2, new Rectangle?(new Rectangle(192 /*0xC0*/, 576, 20, 20)), Color.White, wheelRotation, new Vector2(10f, 10f), 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
        int x1 = (int) ((double) local1.X + 4.0 + 24.0 * Math.Cos((double) wheelRotation));
        int y1 = (int) ((double) local1.Y + 4.0 + 24.0 * Math.Sin((double) wheelRotation));
        int x2 = (int) ((double) local2.X + 4.0 + 24.0 * Math.Cos((double) wheelRotation));
        int y2 = (int) ((double) local2.Y + 4.0 + 24.0 * Math.Sin((double) wheelRotation));
        Utility.drawLineWithScreenCoordinates(x1, y1, x2, y2, b, new Color(112 /*0x70*/, 98, 92), (float) (((double) globalPosition.Y + 264.0) / 10000.0));
        Utility.drawLineWithScreenCoordinates(x1, y1 + 2, x2, y2 + 2, b, new Color(112 /*0x70*/, 98, 92), (float) (((double) globalPosition.Y + 264.0) / 10000.0));
        Utility.drawLineWithScreenCoordinates(x1, y1 + 4, x2, y2 + 4, b, new Color(53, 46, 43), (float) (((double) globalPosition.Y + 264.0) / 10000.0));
        Utility.drawLineWithScreenCoordinates(x1, y1 + 6, x2, y2 + 6, b, new Color(53, 46, 43), (float) (((double) globalPosition.Y + 264.0) / 10000.0));
        b.Draw(Game1.mouseCursors, new Vector2((float) (x1 - 8), (float) (y1 - 8)), new Rectangle?(new Rectangle(192 /*0xC0*/, 640, 24, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 268.0) / 10000.0));
        b.Draw(Game1.mouseCursors, new Vector2((float) (x2 - 8), (float) (y2 - 8)), new Rectangle?(new Rectangle(192 /*0xC0*/, 640, 24, 24)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 268.0) / 10000.0));
        break;
    }
  }

  private void DrawFrontDecal(SpriteBatch b, Vector2 globalPosition)
  {
    if (this.frontDecal.Value == 35)
    {
      b.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(192f, 92f)), new Rectangle?(new Rectangle(480, 480, 32 /*0x20*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
    }
    else
    {
      if (this.frontDecal.Value == -1 || this.frontDecal.Value >= 35)
        return;
      b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition + new Vector2(192f, 92f)), new Rectangle?(new Rectangle(224 /*0xE0*/ + this.frontDecal.Value * 32 /*0x20*/ % 224 /*0xE0*/, 576 + this.frontDecal.Value * 32 /*0x20*/ / 224 /*0xE0*/ * 32 /*0x20*/, 32 /*0x20*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) globalPosition.Y + 260.0) / 10000.0));
    }
  }
}
