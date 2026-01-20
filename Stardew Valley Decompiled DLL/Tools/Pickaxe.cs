// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Pickaxe
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System;

#nullable disable
namespace StardewValley.Tools;

public class Pickaxe : Tool
{
  public const int hitMargin = 8;
  public const int BoulderStrength = 4;
  private int boulderTileX;
  private int boulderTileY;
  private int hitsToBoulder;
  public NetInt additionalPower = new NetInt(0);

  public Pickaxe()
    : base(nameof (Pickaxe), 0, 105, 131, false)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.additionalPower, "additionalPower");
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.UpgradeLevel)
    {
      case 0:
        this.ItemId = nameof (Pickaxe);
        break;
      case 1:
        this.ItemId = "CopperPickaxe";
        break;
      case 2:
        this.ItemId = "SteelPickaxe";
        break;
      case 3:
        this.ItemId = "GoldPickaxe";
        break;
      case 4:
        this.ItemId = "IridiumPickaxe";
        break;
      default:
        this.ItemId = nameof (Pickaxe);
        break;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Pickaxe();

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Pickaxe pickaxe))
      return;
    this.additionalPower.Value = pickaxe.additionalPower.Value;
  }

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    this.Update(who.FacingDirection, 0, who);
    who.EndUsingTool();
    return true;
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    power = who.toolPower.Value;
    if (!this.isEfficient.Value)
      who.Stamina -= (float) (2 * (power + 1)) - (float) who.MiningLevel * 0.1f;
    Utility.clampToTile(new Vector2((float) x, (float) y));
    int num1 = x / 64 /*0x40*/;
    int num2 = y / 64 /*0x40*/;
    Vector2 vector2 = new Vector2((float) num1, (float) num2);
    if (location.performToolAction((Tool) this, num1, num2))
      return;
    StardewValley.Object @object;
    location.Objects.TryGetValue(vector2, out @object);
    if (@object == null)
    {
      if (who.FacingDirection == 0 || who.FacingDirection == 2)
      {
        num1 = (x - 8) / 64 /*0x40*/;
        location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
        if (@object == null)
        {
          num1 = (x + 8) / 64 /*0x40*/;
          location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
        }
      }
      else
      {
        num2 = (y + 8) / 64 /*0x40*/;
        location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
        if (@object == null)
        {
          num2 = (y - 8) / 64 /*0x40*/;
          location.Objects.TryGetValue(new Vector2((float) num1, (float) num2), out @object);
        }
      }
      x = num1 * 64 /*0x40*/;
      y = num2 * 64 /*0x40*/;
      TerrainFeature terrainFeature;
      if (location.terrainFeatures.TryGetValue(vector2, out terrainFeature) && terrainFeature.performToolAction((Tool) this, 0, vector2))
        location.terrainFeatures.Remove(vector2);
    }
    vector2 = new Vector2((float) num1, (float) num2);
    if (@object != null)
    {
      if (@object.IsBreakableStone())
      {
        if (this.PlayUseSounds)
          location.playSound("hammer", new Vector2?(vector2));
        if (@object.MinutesUntilReady > 0)
        {
          int num3 = Math.Max(1, this.upgradeLevel.Value + 1) + this.additionalPower.Value;
          @object.minutesUntilReady.Value -= num3;
          @object.shakeTimer = 200;
          if (@object.MinutesUntilReady > 0)
          {
            Game1.createRadialDebris(Game1.currentLocation, 14, num1, num2, Game1.random.Next(2, 5), false);
            return;
          }
        }
        TemporaryAnimatedSprite temporaryAnimatedSprite1;
        if ((!(ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).TextureName == "Maps\\springobjects") || @object.ParentSheetIndex >= 200 || Game1.objectData.ContainsKey((@object.ParentSheetIndex + 1).ToString()) ? 0 : (@object.QualifiedItemId != "(O)25" ? 1 : 0)) == 0)
        {
          temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(47, new Vector2((float) (num1 * 64 /*0x40*/), (float) (num2 * 64 /*0x40*/)), Color.Gray, 10, animationInterval: 80f);
        }
        else
        {
          temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(@object.ParentSheetIndex + 1, 300f, 1, 2, new Vector2((float) (x - x % 64 /*0x40*/), (float) (y - y % 64 /*0x40*/)), true, @object.flipped.Value);
          temporaryAnimatedSprite1.alphaFade = 0.01f;
        }
        TemporaryAnimatedSprite temporaryAnimatedSprite2 = temporaryAnimatedSprite1;
        Game1.multiplayer.broadcastSprites(location, temporaryAnimatedSprite2);
        Game1.createRadialDebris(location, 14, num1, num2, Game1.random.Next(2, 5), false);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(46, new Vector2((float) (num1 * 64 /*0x40*/), (float) (num2 * 64 /*0x40*/)), Color.White, 10, animationInterval: 80f)
        {
          motion = new Vector2(0.0f, -0.6f),
          acceleration = new Vector2(0.0f, 1f / 500f),
          alphaFade = 0.015f
        });
        location.OnStoneDestroyed(@object.ItemId, num1, num2, this.getLastFarmerToUse());
        if (who != null && who.stats.Get("Book_Diamonds") > 0U && Game1.random.NextDouble() < 0.0066)
        {
          Game1.createObjectDebris("(O)72", num1, num2, who.UniqueMultiplayerID, location);
          if (who.professions.Contains(19) && Game1.random.NextBool())
            Game1.createObjectDebris("(O)72", num1, num2, who.UniqueMultiplayerID, location);
        }
        if (@object.MinutesUntilReady > 0)
          return;
        @object.performRemoveAction();
        location.Objects.Remove(new Vector2((float) num1, (float) num2));
        if (this.PlayUseSounds)
          location.playSound("stoneCrack", new Vector2?(vector2));
        ++Game1.stats.RocksCrushed;
      }
      else if (@object.Name.Contains("Boulder"))
      {
        if (this.PlayUseSounds)
          location.playSound("hammer", new Vector2?(vector2));
        if (this.UpgradeLevel < 2)
        {
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194")));
        }
        else
        {
          if (num1 == this.boulderTileX && num2 == this.boulderTileY)
          {
            this.hitsToBoulder += power + 1;
            @object.shakeTimer = 190;
          }
          else
          {
            this.hitsToBoulder = 0;
            this.boulderTileX = num1;
            this.boulderTileY = num2;
          }
          if (this.hitsToBoulder < 4)
            return;
          location.removeObject(vector2, false);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float) (64.0 * (double) vector2.X - 32.0), (float) (64.0 * ((double) vector2.Y - 1.0))), Color.Gray, flipped: Game1.random.NextBool(), animationInterval: 50f)
          {
            delayBeforeAnimationStart = 0
          });
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float) (64.0 * (double) vector2.X + 32.0), (float) (64.0 * ((double) vector2.Y - 1.0))), Color.Gray, flipped: Game1.random.NextBool(), animationInterval: 50f)
          {
            delayBeforeAnimationStart = 200
          });
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector2.X, (float) (64.0 * ((double) vector2.Y - 1.0) - 32.0)), Color.Gray, flipped: Game1.random.NextBool(), animationInterval: 50f)
          {
            delayBeforeAnimationStart = 400
          });
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector2.X, (float) (64.0 * (double) vector2.Y - 32.0)), Color.Gray, flipped: Game1.random.NextBool(), animationInterval: 50f)
          {
            delayBeforeAnimationStart = 600
          });
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * vector2.X, 64f * vector2.Y), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f, sourceRectHeight: 128 /*0x80*/));
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float) (64.0 * (double) vector2.X + 32.0), 64f * vector2.Y), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f, sourceRectHeight: 128 /*0x80*/)
          {
            delayBeforeAnimationStart = 250
          });
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float) (64.0 * (double) vector2.X - 32.0), 64f * vector2.Y), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f, sourceRectHeight: 128 /*0x80*/)
          {
            delayBeforeAnimationStart = 500
          });
          if (!this.PlayUseSounds)
            return;
          location.playSound("boulderBreak", new Vector2?(vector2));
        }
      }
      else
      {
        if (!@object.performToolAction((Tool) this))
          return;
        @object.performRemoveAction();
        if (@object.Type == "Crafting" && @object.fragility.Value != 2)
          Game1.currentLocation.debris.Add(new Debris(@object.QualifiedItemId, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel)));
        Game1.currentLocation.Objects.Remove(vector2);
      }
    }
    else
    {
      if (this.PlayUseSounds)
        location.playSound("woodyHit", new Vector2?(vector2));
      if (location.doesTileHaveProperty(num1, num2, "Diggable", "Back") == null)
        return;
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2((float) (num1 * 64 /*0x40*/), (float) (num2 * 64 /*0x40*/)), Color.White, animationInterval: 80f)
      {
        alphaFade = 0.015f
      });
    }
  }
}
