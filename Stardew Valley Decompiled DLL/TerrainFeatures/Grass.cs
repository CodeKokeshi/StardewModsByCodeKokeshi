// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Grass
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Netcode.Validation;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

[XmlInclude(typeof (CosmeticPlant))]
[NotImplicitNetField]
public class Grass : TerrainFeature
{
  public const float defaultShakeRate = 0.03926991f;
  public const float maximumShake = 0.3926991f;
  public const float shakeDecayRate = 0.008975979f;
  public const byte springGrass = 1;
  public const byte caveGrass = 2;
  public const byte frostGrass = 3;
  public const byte lavaGrass = 4;
  public const byte caveGrass2 = 5;
  public const byte cobweb = 6;
  public const byte blueGrass = 7;
  /// <summary>The backing field for <see cref="M:StardewValley.TerrainFeatures.Grass.PlayGrassSound" />.</summary>
  public static ICue grassSound;
  [XmlElement("grassType")]
  public readonly NetByte grassType = new NetByte();
  private bool shakeLeft;
  protected float shakeRotation;
  protected float maxShake;
  protected float shakeRate;
  [XmlElement("numberOfWeeds")]
  public readonly NetInt numberOfWeeds = new NetInt();
  [XmlElement("grassSourceOffset")]
  public readonly NetInt grassSourceOffset = new NetInt();
  private int grassBladeHealth = 1;
  [XmlIgnore]
  public Lazy<Texture2D> texture;
  private int[] whichWeed = new int[4];
  private int[] offset1 = new int[4];
  private int[] offset2 = new int[4];
  private int[] offset3 = new int[4];
  private int[] offset4 = new int[4];
  private bool[] flip = new bool[4];
  private double[] shakeRandom = new double[4];

  public Grass()
    : base(true)
  {
    this.texture = new Lazy<Texture2D>((Func<Texture2D>) (() => Game1.content.Load<Texture2D>(this.textureName())));
  }

  public Grass(int which, int numberOfWeeds)
    : this()
  {
    this.grassType.Value = (byte) which;
    this.loadSprite();
    this.numberOfWeeds.Value = numberOfWeeds;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.grassType, "grassType").AddField((INetSerializable) this.numberOfWeeds, "numberOfWeeds").AddField((INetSerializable) this.grassSourceOffset, "grassSourceOffset");
  }

  /// <summary>Play the sound of walking through grass, if it's not already playing.</summary>
  public static void PlayGrassSound()
  {
    ICue grassSound = Grass.grassSound;
    if ((grassSound != null ? (!grassSound.IsPlaying ? 1 : 0) : 1) == 0)
      return;
    Game1.playSound("grassyStep", out Grass.grassSound);
  }

  public virtual string textureName() => "TerrainFeatures\\grass";

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => true;

  public override void loadSprite()
  {
    try
    {
      switch (this.grassType.Value)
      {
        case 1:
          switch (Game1.GetSeasonForLocation(this.Location))
          {
            case Season.Spring:
              this.grassSourceOffset.Value = 0;
              return;
            case Season.Summer:
              this.grassSourceOffset.Value = 20;
              return;
            case Season.Fall:
              this.grassSourceOffset.Value = 40;
              return;
            case Season.Winter:
              this.grassSourceOffset.Value = this.Location == null || !this.Location.IsOutdoors ? 0 : 80 /*0x50*/;
              return;
            default:
              return;
          }
        case 2:
          this.grassSourceOffset.Value = 60;
          break;
        case 3:
          this.grassSourceOffset.Value = 80 /*0x50*/;
          break;
        case 4:
          this.grassSourceOffset.Value = 100;
          break;
        case 7:
          switch (Game1.GetSeasonForLocation(this.Location))
          {
            case Season.Spring:
              this.grassSourceOffset.Value = 160 /*0xA0*/;
              return;
            case Season.Summer:
              this.grassSourceOffset.Value = 180;
              return;
            case Season.Fall:
              this.grassSourceOffset.Value = 200;
              return;
            case Season.Winter:
              this.grassSourceOffset.Value = this.Location == null || !this.Location.IsOutdoors ? 160 /*0xA0*/ : 220;
              return;
            default:
              return;
          }
        default:
          this.grassSourceOffset.Value = ((int) this.grassType.Value + 1) * 20;
          break;
      }
    }
    catch
    {
    }
  }

  public override void OnAddedToLocation(GameLocation location, Vector2 tile)
  {
    base.OnAddedToLocation(location, tile);
    this.loadSprite();
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X * 64.0), (int) ((double) tile.Y * 64.0), 64 /*0x40*/, 64 /*0x40*/);
  }

  public override Rectangle getRenderBounds()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X * 64.0) - 32 /*0x20*/, (int) ((double) tile.Y * 64.0) - 32 /*0x20*/, 128 /*0x80*/, 112 /*0x70*/);
  }

  public override void doCollisionAction(
    Rectangle positionOfCollider,
    int speedOfCollision,
    Vector2 tileLocation,
    Character who)
  {
    GameLocation location = this.Location;
    if (location != Game1.currentLocation)
      return;
    if (speedOfCollision > 0 && (double) this.maxShake == 0.0 && positionOfCollider.Intersects(this.getBoundingBox()))
    {
      if (!(who is FarmAnimal) && Utility.isOnScreen(new Point((int) tileLocation.X, (int) tileLocation.Y), 2, location))
        Grass.PlayGrassSound();
      this.shake(0.3926991f / Math.Min(1f, 5f / (float) speedOfCollision), (float) Math.PI / 80f / Math.Min(1f, 5f / (float) speedOfCollision), (double) positionOfCollider.Center.X > (double) tileLocation.X * 64.0 + 32.0);
    }
    if (who is Farmer && Game1.player.CurrentTool is MeleeWeapon currentTool && currentTool.isOnSpecial && currentTool.type.Value == 0 && (double) Math.Abs(this.shakeRotation) < 1.0 / 1000.0 && this.performToolAction(Game1.player.CurrentTool, -1, tileLocation))
      Game1.currentLocation.terrainFeatures.Remove(tileLocation);
    if (!(who is Farmer farmer))
      return;
    farmer.temporarySpeedBuff = farmer.stats.Get("Book_Grass") <= 0U ? -1f : -0.33f;
    if (this.grassType.Value != (byte) 6)
      return;
    farmer.temporarySpeedBuff = -3f;
  }

  public bool reduceBy(int number, bool showDebris)
  {
    this.grassBladeHealth -= number;
    if (this.grassBladeHealth > 0)
      return true;
    int num;
    if (this.grassType.Value == (byte) 7)
    {
      num = 1 + this.grassBladeHealth / -2;
      this.grassBladeHealth = 2 - this.grassBladeHealth % 2;
    }
    else
    {
      this.grassBladeHealth = 1;
      num = number;
    }
    this.numberOfWeeds.Value -= num;
    if (showDebris)
    {
      Vector2 tile = this.Tile;
      Game1.createRadialDebris(Game1.currentLocation, this.textureName(), new Rectangle(2, 8 + this.grassSourceOffset.Value, 8, 8), 1, (int) (((double) tile.X + 1.0) * 64.0), ((int) tile.Y + 1) * 64 /*0x40*/, Game1.random.Next(2, 5), (int) tile.Y + 1, Color.White, 4f);
      Game1.createRadialDebris(Game1.currentLocation, this.textureName(), new Rectangle(2, 8 + this.grassSourceOffset.Value, 8, 8), 1, (int) (((double) tile.X + 1.1000000238418579) * 64.0), (int) (((double) tile.Y + 1.1000000238418579) * 64.0), Game1.random.Next(2, 5), (int) tile.Y + 1, Color.White, 4f);
      Game1.createRadialDebris(Game1.currentLocation, this.textureName(), new Rectangle(2, 8 + this.grassSourceOffset.Value, 8, 8), 1, (int) (((double) tile.X + 0.89999997615814209) * 64.0), (int) (((double) tile.Y + 1.1000000238418579) * 64.0), Game1.random.Next(2, 5), (int) tile.Y + 1, Color.White, 4f);
      this.createDestroySprites(Game1.currentLocation, tile);
    }
    return this.numberOfWeeds.Value <= 0;
  }

  protected void shake(float shake, float rate, bool left)
  {
    this.maxShake = shake;
    this.shakeRate = rate;
    this.shakeRotation = 0.0f;
    this.shakeLeft = left;
    this.NeedsUpdate = true;
  }

  public override void performPlayerEntryAction()
  {
    base.performPlayerEntryAction();
    if (this.shakeRandom[0] != 0.0)
      return;
    this.setUpRandom();
  }

  public override bool tickUpdate(GameTime time)
  {
    if (this.shakeRandom[0] == 0.0)
      this.setUpRandom();
    if ((double) this.maxShake > 0.0)
    {
      if (this.shakeLeft)
      {
        this.shakeRotation -= this.shakeRate;
        if ((double) Math.Abs(this.shakeRotation) >= (double) this.maxShake)
          this.shakeLeft = false;
      }
      else
      {
        this.shakeRotation += this.shakeRate;
        if ((double) this.shakeRotation >= (double) this.maxShake)
        {
          this.shakeLeft = true;
          this.shakeRotation -= this.shakeRate;
        }
      }
      this.maxShake = Math.Max(0.0f, this.maxShake - (float) Math.PI / 350f);
    }
    else
    {
      this.shakeRotation /= 2f;
      if ((double) this.shakeRotation <= 0.0099999997764825821)
      {
        this.NeedsUpdate = false;
        this.shakeRotation = 0.0f;
      }
    }
    return false;
  }

  public override void dayUpdate()
  {
    GameLocation location = this.Location;
    if ((this.grassType.Value == (byte) 1 || this.grassType.Value == (byte) 7) && (location.GetSeason() != Season.Winter || location.HasMapPropertyWithValue("AllowGrassGrowInWinter")) && this.numberOfWeeds.Value < 4)
      this.numberOfWeeds.Value = Utility.Clamp(this.numberOfWeeds.Value + Game1.random.Next(1, 4), 0, 4);
    this.setUpRandom();
    if (this.grassType.Value == (byte) 7)
      this.grassBladeHealth = 2;
    else
      this.grassBladeHealth = 1;
  }

  public void setUpRandom()
  {
    Vector2 tile = this.Tile;
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed / 28.0, (double) tile.X * 7.0, (double) tile.Y * 11.0);
    bool? nullable = this.Location?.hasTileAt((int) tile.X, (int) tile.Y, "Front");
    bool flag = nullable.HasValue && nullable.GetValueOrDefault();
    for (int index = 0; index < 4; ++index)
    {
      this.whichWeed[index] = random.Next(3);
      this.offset1[index] = random.Next(-2, 3);
      this.offset2[index] = random.Next(-2, 3) + (flag ? -7 : 0);
      this.offset3[index] = random.Next(-2, 3);
      this.offset4[index] = random.Next(-2, 3) + (flag ? -7 : 0);
      this.flip[index] = random.NextBool();
      this.shakeRandom[index] = random.NextDouble();
    }
  }

  /// <inheritdoc />
  public override bool seasonUpdate(bool onLoad)
  {
    if (this.grassType.Value == (byte) 1 || this.grassType.Value == (byte) 7)
    {
      if (this.Location.IsOutdoors && this.Location.IsWinterHere() && this.Location.HasMapPropertyWithValue("AllowGrassSurviveInWinter") && this.Location.getMapProperty("AllowGrassSurviveInWinter").StartsWithIgnoreCase("f") && !onLoad)
        return true;
      this.loadSprite();
    }
    return false;
  }

  public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
  {
    GameLocation location = this.Location ?? Game1.currentLocation;
    if ((!(t is MeleeWeapon meleeWeapon) || meleeWeapon.type.Value == 2) && explosion <= 0)
      return false;
    if (meleeWeapon != null && meleeWeapon.type.Value != 1)
      DelayedAction.playSoundAfterDelay("daggerswipe", 50, location, new Vector2?(tileLocation));
    else
      location.playSound("swordswipe", new Vector2?(tileLocation));
    this.shake(3f * (float) Math.PI / 32f, (float) Math.PI / 40f, Game1.random.NextBool());
    int num = explosion > 0 ? Math.Max(1, explosion + 2 - Game1.recentMultiplayerRandom.Next(2)) : 1;
    if (meleeWeapon != null && t.ItemId == "53")
      num = 2;
    else if (meleeWeapon != null && t.ItemId == "66")
      num = 4;
    if (this.grassType.Value == (byte) 6 && Game1.random.NextBool())
      num = 0;
    this.numberOfWeeds.Value -= num;
    this.createDestroySprites(location, tileLocation);
    return this.TryDropItemsOnCut(t);
  }

  private void createDestroySprites(GameLocation location, Vector2 tileLocation)
  {
    Color color;
    switch (this.grassType.Value)
    {
      case 1:
        switch (location.GetSeason())
        {
          case Season.Spring:
            color = new Color(60, 180, 58);
            break;
          case Season.Summer:
            color = new Color(110, 190, 24);
            break;
          case Season.Fall:
            color = new Color(219, 102, 58);
            break;
          case Season.Winter:
            color = new Color(63 /*0x3F*/, 167, 156);
            break;
          default:
            color = Color.Green;
            break;
        }
        break;
      case 2:
        color = new Color(148, 146, 71);
        break;
      case 3:
        color = new Color(216, 240 /*0xF0*/, (int) byte.MaxValue);
        break;
      case 4:
        color = new Color(165, 93, 58);
        break;
      case 6:
        color = Color.White * 0.6f;
        break;
      case 7:
        switch (location.GetSeason())
        {
          case Season.Spring:
          case Season.Summer:
            color = new Color(0, 178, 174);
            break;
          case Season.Fall:
            color = new Color(129, 80 /*0x50*/, 148);
            break;
          case Season.Winter:
            color = new Color(40, 125, 178);
            break;
          default:
            color = Color.Green;
            break;
        }
        break;
      default:
        color = Color.Green;
        break;
    }
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(28, tileLocation * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), color, flipped: Game1.random.NextBool(), animationInterval: (float) Game1.random.Next(60, 100)));
  }

  /// <summary>Drop an item when this grass is cut, if any.</summary>
  /// <param name="tool">The tool used to cut the grass.</param>
  /// <param name="addAnimation">Whether to show animations for the cut grass.</param>
  public bool TryDropItemsOnCut(Tool tool, bool addAnimation = true)
  {
    Vector2 tile = this.Tile;
    GameLocation location = this.Location;
    if (this.numberOfWeeds.Value > 0)
      return false;
    if (this.grassType.Value != (byte) 1 && this.grassType.Value != (byte) 7)
    {
      Random random = Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tile.X * 1000.0, (double) tile.Y * 11.0, (double) Game1.CurrentMineLevel, (double) Game1.player.timesReachedMineBottom);
      if (random.NextDouble() < 0.005)
        Game1.createObjectDebris("(O)114", (int) tile.X, (int) tile.Y, -1, 0, 1f, location);
      else if (random.NextDouble() < 0.01)
        Game1.createDebris(4, (int) tile.X, (int) tile.Y, random.Next(1, 2), location);
      else if (random.NextDouble() < 0.02)
        Game1.createObjectDebris("(O)92", (int) tile.X, (int) tile.Y, (long) random.Next(2, 4), location);
    }
    else if (tool != null && tool.isScythe())
    {
      Farmer farmer = tool.getLastFarmerToUse() ?? Game1.player;
      Random random = Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tile.X * 1000.0, (double) tile.Y * 11.0);
      double num = tool.ItemId == "66" ? 1.0 : (tool.ItemId == "53" ? 0.75 : 0.5);
      if (farmer.currentLocation.IsWinterHere())
        num *= 0.33;
      if (random.NextDouble() < num)
      {
        int count = this.grassType.Value == (byte) 7 ? 2 : 1;
        if (GameLocation.StoreHayInAnySilo(count, this.Location) == 0)
        {
          if (addAnimation)
          {
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16 /*0x10*/, 16 /*0x10*/), 750f, 1, 0, farmer.Position - new Vector2(0.0f, 128f), false, false, farmer.Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0.0f, 0.0f)
            {
              motion = {
                Y = (float) ((double) Game1.random.Next(-10, 11) / 100.0 - 3.0)
              },
              acceleration = {
                Y = (float) (0.070000000298023224 + (double) Game1.random.Next(-10, 11) / 1000.0)
              }
            };
            temporaryAnimatedSprite.motion.X = (float) Game1.random.Next(-20, 21) / 10f;
            temporaryAnimatedSprite.layerDepth = (float) (1.0 - (double) Game1.random.Next(100) / 10000.0);
            temporaryAnimatedSprite.delayBeforeAnimationStart = Game1.random.Next(150);
            Game1.multiplayer.broadcastSprites(this.Location, temporaryAnimatedSprite);
          }
          Game1.addHUDMessage(HUDMessage.ForItemGained(ItemRegistry.Create("(O)178"), count));
        }
      }
    }
    return true;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 positionOnScreen,
    Vector2 tileLocation,
    float scale,
    float layerDepth)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed / 28.0, (double) positionOnScreen.X * 7.0, (double) positionOnScreen.Y * 11.0);
    for (int index = 0; index < this.numberOfWeeds.Value; ++index)
    {
      int num = random.Next(3);
      Vector2 position = index != 4 ? tileLocation * 64f + new Vector2((float) (index % 2 * 64 /*0x40*/ / 2 + random.Next(-2, 2) * 4 - 4) + 30f, (float) (index / 2 * 64 /*0x40*/ / 2 + random.Next(-2, 2) * 4 + 40)) : tileLocation * 64f + new Vector2((float) (16 /*0x10*/ + random.Next(-2, 2) * 4 - 4) + 30f, (float) (16 /*0x10*/ + random.Next(-2, 2) * 4 + 40));
      spriteBatch.Draw(this.texture.Value, position, new Rectangle?(new Rectangle(num * 15, this.grassSourceOffset.Value, 15, 20)), Color.White, this.shakeRotation / (float) (random.NextDouble() + 1.0), Vector2.Zero, scale, SpriteEffects.None, layerDepth + (float) ((32.0 * (double) scale + 300.0) / 20000.0));
    }
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    for (int index = 0; index < this.numberOfWeeds.Value; ++index)
    {
      Vector2 globalPosition = index != 4 ? tile * 64f + new Vector2((float) (index % 2 * 64 /*0x40*/ / 2 + this.offset3[index] * 4 - 4) + 30f, (float) (index / 2 * 64 /*0x40*/ / 2 + this.offset4[index] * 4 + 40)) : tile * 64f + new Vector2((float) (16 /*0x10*/ + this.offset1[index] * 4 - 4) + 30f, (float) (16 /*0x10*/ + this.offset2[index] * 4 + 40));
      spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(new Rectangle(this.whichWeed[index] * 15, this.grassSourceOffset.Value, 15, 20)), Color.White, this.shakeRotation / (float) (this.shakeRandom[index] + 1.0), new Vector2(7.5f, 17.5f), 4f, this.flip[index] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) globalPosition.Y + 16.0 - 20.0) / 10000.0 + (double) globalPosition.X / 10000000.0));
    }
  }
}
