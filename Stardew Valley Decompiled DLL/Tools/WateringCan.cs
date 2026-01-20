// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.WateringCan
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class WateringCan : Tool
{
  [XmlElement("isBottomless")]
  public readonly NetBool isBottomless = new NetBool();
  [XmlIgnore]
  protected bool _emptyCanPlayed;
  [XmlIgnore]
  public int waterCanMax = 40;
  private readonly NetInt waterLeft = new NetInt(40);

  public int WaterLeft
  {
    get => !this.IsBottomless ? this.waterLeft.Value : this.waterCanMax;
    set => this.waterLeft.Value = value;
  }

  public bool IsBottomless
  {
    get => this.isBottomless.Value;
    set => this.isBottomless.Value = value;
  }

  public WateringCan()
    : base("Watering Can", 0, 273, 296, false)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.isBottomless, "isBottomless").AddField((INetSerializable) this.waterLeft, "waterLeft");
    this.upgradeLevel.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((_param1, _param2, _param3) => this.OnUpgradeLevelChanged());
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.UpgradeLevel)
    {
      case 0:
        this.ItemId = nameof (WateringCan);
        break;
      case 1:
        this.ItemId = "CopperWateringCan";
        break;
      case 2:
        this.ItemId = "SteelWateringCan";
        break;
      case 3:
        this.ItemId = "GoldWateringCan";
        break;
      case 4:
        this.ItemId = "IridiumWateringCan";
        break;
      default:
        this.ItemId = nameof (WateringCan);
        break;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new WateringCan();

  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is WateringCan wateringCan))
      return;
    this.WaterLeft = wateringCan.WaterLeft;
    this.IsBottomless = wateringCan.IsBottomless;
  }

  /// <summary>Update the tool state when <see cref="F:StardewValley.Tool.upgradeLevel" /> changes.</summary>
  protected virtual void OnUpgradeLevelChanged()
  {
    switch (this.upgradeLevel.Value)
    {
      case 0:
        this.waterCanMax = 40;
        break;
      case 1:
        this.waterCanMax = 55;
        break;
      case 2:
        this.waterCanMax = 70;
        break;
      case 3:
        this.waterCanMax = 85;
        break;
      default:
        this.waterCanMax = 100;
        break;
    }
    this.waterLeft.Value = this.waterCanMax;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    base.drawInMenu(spriteBatch, location + (Game1.player.hasWateringCanEnchantment ? new Vector2(0.0f, -4f) : new Vector2(0.0f, -12f)), scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
    if (drawStackNumber == StackDrawType.Hide || Game1.player.hasWateringCanEnchantment)
      return;
    spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(4f, 44f), new Rectangle?(new Rectangle(297, 420, 14, 5)), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.0001f);
    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int) location.X + 8, (int) location.Y + 64 /*0x40*/ - 16 /*0x10*/, (int) ((double) this.WaterLeft / (double) this.waterCanMax * 48.0), 8), this.IsBottomless ? Color.BlueViolet * 1f * transparency : Color.DodgerBlue * 0.7f * transparency);
  }

  public override string getDescription()
  {
    return Game1.parseText(this.description + (Game1.player.hasWateringCanEnchantment ? Environment.NewLine + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan_enchant") : ""), Game1.smallFont, this.getDescriptionWidth());
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    power = who.toolPower.Value;
    who.stopJittering();
    List<Vector2> vector2List = this.tilesAffected(new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/)), power, who);
    if (Game1.currentLocation.CanRefillWateringCanOnTile(x / 64 /*0x40*/, y / 64 /*0x40*/))
    {
      who.jitterStrength = 0.5f;
      this.WaterLeft = this.waterCanMax;
      if (!this.PlayUseSounds)
        return;
      who.playNearbySoundAll("slosh");
      DelayedAction.playSoundAfterDelay("glug", 250, location, new Vector2?(who.Tile));
    }
    else if (this.WaterLeft > 0 || who.hasWateringCanEnchantment)
    {
      if (!this.isEfficient.Value)
        who.Stamina -= (float) (2 * (power + 1)) - (float) who.FarmingLevel * 0.1f;
      int num = 0;
      foreach (Vector2 vector2 in vector2List)
      {
        TerrainFeature terrainFeature;
        if (location.terrainFeatures.TryGetValue(vector2, out terrainFeature))
          terrainFeature.performToolAction((Tool) this, 0, vector2);
        StardewValley.Object @object;
        if (location.objects.TryGetValue(vector2, out @object))
          @object.performToolAction((Tool) this);
        location.performToolAction((Tool) this, (int) vector2.X, (int) vector2.Y);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(13, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 10, Game1.random.NextBool(), 70f, sourceRectWidth: 64 /*0x40*/, layerDepth: (float) (((double) vector2.Y * 64.0 + 32.0) / 10000.0 - 0.0099999997764825821))
        {
          delayBeforeAnimationStart = 200 + num * 10
        });
        ++num;
      }
      if (!this.IsBottomless)
        this.WaterLeft -= power + 1;
      Vector2 vector2_1 = new Vector2((float) ((double) who.Position.X - 32.0 - 4.0), (float) ((double) who.Position.Y - 16.0 - 4.0));
      switch (who.FacingDirection)
      {
        case 0:
          vector2_1 = Vector2.Zero;
          break;
        case 1:
          vector2_1.X += 136f;
          break;
        case 2:
          vector2_1.X += 72f;
          vector2_1.Y += 44f;
          break;
      }
      if (vector2_1.Equals(Vector2.Zero))
        return;
      Rectangle boundingBox = who.GetBoundingBox();
      for (int index = 0; index < 30; ++index)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("", new Rectangle(0, 0, 1, 1), 999f, 1, 999, vector2_1 + new Vector2((float) (Game1.random.Next(-3, 0) * 4), (float) (Game1.random.Next(2) * 4)), false, false, (float) (boundingBox.Bottom + 32 /*0x20*/) / 10000f, 0.04f, Game1.random.Choose<Color>(Color.DeepSkyBlue, Color.LightBlue), 4f, 0.0f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = index * 15,
          motion = new Vector2((float) Game1.random.Next(-10, 11) / 100f, 0.5f),
          acceleration = new Vector2(0.0f, 0.1f)
        });
    }
    else
    {
      if (this._emptyCanPlayed)
        return;
      this._emptyCanPlayed = true;
      who.doEmote(4);
      if (who != Game1.player)
        return;
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335"));
    }
  }

  public override bool CanUseOnStandingTile() => true;

  public override void tickUpdate(GameTime time, Farmer who)
  {
    base.tickUpdate(time, who);
    if (who.IsLocalPlayer)
    {
      if (!Game1.areAllOfTheseKeysUp(Game1.input.GetKeyboardState(), Game1.options.useToolButton) || Game1.input.GetMouseState().LeftButton != ButtonState.Released || !Game1.input.GetGamePadState().IsButtonUp(Buttons.X))
        return;
      this._emptyCanPlayed = false;
    }
    else
      this._emptyCanPlayed = false;
  }
}
