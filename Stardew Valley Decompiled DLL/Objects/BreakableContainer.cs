// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.BreakableContainer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Constants;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class BreakableContainer : StardewValley.Object
{
  public const string barrelId = "118";
  public const string frostBarrelId = "120";
  public const string darkBarrelId = "122";
  public const string desertBarrelId = "124";
  public const string volcanoBarrelId = "174";
  public const string waterBarrelId = "262";
  [XmlElement("debris")]
  private readonly NetInt debris = new NetInt();
  private new int shakeTimer;
  [XmlElement("health")]
  private readonly NetInt health = new NetInt();
  [XmlElement("hitSound")]
  private readonly NetString hitSound = new NetString();
  [XmlElement("breakSound")]
  private readonly NetString breakSound = new NetString();
  [XmlElement("breakDebrisSource")]
  private readonly NetRectangle breakDebrisSource = new NetRectangle();
  [XmlElement("breakDebrisSource2")]
  private readonly NetRectangle breakDebrisSource2 = new NetRectangle();

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.debris, "debris").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.hitSound, "hitSound").AddField((INetSerializable) this.breakSound, "breakSound").AddField((INetSerializable) this.breakDebrisSource, "breakDebrisSource").AddField((INetSerializable) this.breakDebrisSource2, "breakDebrisSource2");
  }

  public BreakableContainer()
  {
  }

  public BreakableContainer(
    Vector2 tile,
    string itemId,
    int health = 3,
    int debrisType = 12,
    string hitSound = "woodWhack",
    string breakSound = "barrelBreak")
    : base(tile, itemId)
  {
    this.health.Value = health;
    this.debris.Value = debrisType;
    this.hitSound.Value = hitSound;
    this.breakSound.Value = breakSound;
    this.breakDebrisSource.Value = new Rectangle(598, 1275, 13, 4);
    this.breakDebrisSource2.Value = new Rectangle(611, 1275, 10, 4);
  }

  /// <summary>Get a barrel to place in the mines or Skull Cavern.</summary>
  /// <param name="tile">The tile position at which it'll be placed.</param>
  /// <param name="mine">The mine level.</param>
  public static BreakableContainer GetBarrelForMines(Vector2 tile, MineShaft mine)
  {
    int mineArea = mine.getMineArea();
    string itemId;
    if (mine.GetAdditionalDifficulty() > 0)
    {
      itemId = mineArea != 0 && mineArea != 10 || mine.isDarkArea() ? "118" : "262";
    }
    else
    {
      switch (mineArea)
      {
        case 40:
          itemId = "120";
          break;
        case 80 /*0x50*/:
          itemId = "122";
          break;
        case 121:
          itemId = "124";
          break;
        default:
          itemId = "118";
          break;
      }
    }
    BreakableContainer barrelForMines = new BreakableContainer(tile, itemId);
    if (Game1.random.NextBool())
      barrelForMines.showNextIndex.Value = true;
    return barrelForMines;
  }

  /// <summary>Get a barrel to place in the Volcano Dungeon.</summary>
  /// <param name="tile">The tile position at which it'll be placed.</param>
  public static BreakableContainer GetBarrelForVolcanoDungeon(Vector2 tile)
  {
    BreakableContainer forVolcanoDungeon = new BreakableContainer(tile, "174", 4, 14, "clank", "boulderBreak");
    if (Game1.random.NextBool())
      forVolcanoDungeon.showNextIndex.Value = true;
    return forVolcanoDungeon;
  }

  public override bool performToolAction(Tool t)
  {
    GameLocation location = this.Location;
    if (location == null || t == null || !t.isHeavyHitter())
      return false;
    --this.health.Value;
    if (t is MeleeWeapon meleeWeapon && meleeWeapon.type.Value == 2)
      --this.health.Value;
    if (this.health.Value <= 0)
    {
      if (!string.IsNullOrEmpty(this.breakSound.Value))
        this.playNearbySoundAll(this.breakSound.Value);
      this.releaseContents(t.getLastFarmerToUse());
      location.objects.Remove(this.tileLocation.Value);
      int num = Game1.random.Next(4, 12);
      Color chipColor = this.GetChipColor();
      for (int index = 0; index < num; ++index)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextBool() ? this.breakDebrisSource.Value : this.breakDebrisSource2.Value, 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 32f), false, Game1.random.NextBool(), (float) (((double) this.tileLocation.Y * 64.0 + 32.0) / 10000.0), 0.01f, chipColor, 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 64.0))
        {
          motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, (float) Game1.random.Next(-10, -7)),
          acceleration = new Vector2(0.0f, 0.3f)
        });
    }
    else if (!string.IsNullOrEmpty(this.hitSound.Value))
    {
      this.shakeTimer = 300;
      this.playNearbySoundAll(this.hitSound.Value);
      Color? color = this.ItemId == "120" ? new Color?(Color.White) : new Color?();
      Game1.createRadialDebris(location, this.debris.Value, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 7), false, color: color);
    }
    return false;
  }

  public override bool onExplosion(Farmer who)
  {
    if (who == null)
      who = Game1.player;
    GameLocation location = this.Location;
    if (location == null)
      return true;
    this.releaseContents(who);
    int num = Game1.random.Next(4, 12);
    Color chipColor = this.GetChipColor();
    for (int index = 0; index < num; ++index)
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.random.NextBool() ? this.breakDebrisSource.Value : this.breakDebrisSource2.Value, 999f, 1, 0, this.tileLocation.Value * 64f + new Vector2(32f, 32f), false, Game1.random.NextBool(), (float) (((double) this.tileLocation.Y * 64.0 + 32.0) / 10000.0), 0.01f, chipColor, 4f, 0.0f, (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 8.0), (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 64.0))
      {
        motion = new Vector2((float) Game1.random.Next(-30, 31 /*0x1F*/) / 10f, (float) Game1.random.Next(-10, -7)),
        acceleration = new Vector2(0.0f, 0.3f)
      });
    return true;
  }

  /// <summary>Get the color of cosmetic chip debris when breaking this container.</summary>
  public Color GetChipColor()
  {
    switch (this.ItemId)
    {
      case "120":
        return Color.White;
      case "122":
        return new Color(109, 122, 80 /*0x50*/);
      case "174":
        return new Color(107, 76, 83);
      default:
        return new Color(130, 80 /*0x50*/, 30);
    }
  }

  public void releaseContents(Farmer who)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    Random random = Utility.CreateRandom((double) this.tileLocation.X, (double) this.tileLocation.Y * 10000.0, (double) Game1.stats.DaysPlayed, location is MineShaft mineShaft1 ? (double) mineShaft1.mineLevel : 0.0);
    int x = (int) this.tileLocation.X;
    int y = (int) this.tileLocation.Y;
    int level = -1;
    int num1 = 0;
    if (location is MineShaft mineShaft2)
    {
      level = mineShaft2.mineLevel;
      if (mineShaft2.isContainerPlatform(x, y))
        mineShaft2.updateMineLevelData(0, -1);
      num1 = mineShaft2.GetAdditionalDifficulty();
    }
    if (random.NextDouble() < 0.2)
    {
      if (random.NextDouble() >= 0.1)
        return;
      Game1.createMultipleItemDebris(Utility.getRaccoonSeedForCurrentTimeOfYear(who, random), new Vector2((float) x, (float) y) * 64f + new Vector2(32f), -1, location);
    }
    else
    {
      if (location is MineShaft mineShaft3)
      {
        if (mineShaft3.mineLevel > 120 && !mineShaft3.isSideBranch())
        {
          int num2 = mineShaft3.mineLevel - 121;
          if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0)
          {
            float chance = (float) (num2 + Game1.player.team.calicoEggSkullCavernRating.Value * 2) * (3f / 1000f);
            if ((double) chance > 0.33000001311302185)
              chance = 0.33f;
            if (random.NextBool(chance))
              Game1.createMultipleObjectDebris("CalicoEgg", x, y, random.Next(1, 4), who.UniqueMultiplayerID, location);
          }
        }
        int num3 = mineShaft3.mineLevel;
        if (mineShaft3.mineLevel == 77377)
          num3 = 5000;
        Trinket.TrySpawnTrinket(location, (Monster) null, new Vector2((float) x, (float) y) * 64f + new Vector2(32f), 1.0 + (double) num3 * 0.001);
      }
      if (random.NextDouble() <= 0.05 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
        Game1.createMultipleObjectDebris("(O)890", x, y, random.Next(1, 3), who.UniqueMultiplayerID, location);
      if (Utility.tryRollMysteryBox(0.0081 + Game1.player.team.AverageDailyLuck() / 15.0, random))
        Game1.createItemDebris(ItemRegistry.Create(Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2((float) x, (float) y) * 64f + new Vector2(32f), -1, location);
      Utility.trySpawnRareObject(who, new Vector2((float) x, (float) y) * 64f, location, 1.5, random: random);
      if (num1 > 0)
      {
        if (random.NextDouble() < 0.15)
          return;
        if (random.NextDouble() < 0.008)
          Game1.createMultipleObjectDebris("(O)858", x, y, 1, location);
        if (random.NextDouble() < 0.01)
          Game1.createItemDebris(ItemRegistry.Create("(BC)71"), new Vector2((float) x, (float) y) * 64f + new Vector2(32f), 0);
        if (random.NextDouble() < 0.01)
          Game1.createMultipleObjectDebris(random.Choose<string>("(O)918", "(O)919", "(O)920"), x, y, 1, location);
        if (random.NextDouble() < 0.01)
          Game1.createMultipleObjectDebris("(O)386", x, y, random.Next(1, 4), location);
        switch (random.Next(17))
        {
          case 0:
            Game1.createMultipleObjectDebris("(O)382", x, y, random.Next(1, 3), location);
            break;
          case 1:
            Game1.createMultipleObjectDebris("(O)380", x, y, random.Next(1, 4), location);
            break;
          case 2:
            Game1.createMultipleObjectDebris("(O)62", x, y, 1, location);
            break;
          case 3:
            Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
            break;
          case 4:
            Game1.createMultipleObjectDebris("(O)80", x, y, random.Next(2, 3), location);
            break;
          case 5:
            Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? "(O)84" : random.Choose<string>("(O)92", "(O)370"), x, y, random.Choose<int>(2, 3), location);
            break;
          case 6:
            Game1.createMultipleObjectDebris("(O)70", x, y, 1, location);
            break;
          case 7:
            Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
            break;
          case 8:
            Game1.createMultipleObjectDebris("(O)" + random.Next(218, 245).ToString(), x, y, 1, location);
            break;
          case 9:
            Game1.createMultipleObjectDebris(Game1.whichFarm == 6 ? "(O)920" : "(O)749", x, y, 1, location);
            break;
          case 10:
            Game1.createMultipleObjectDebris("(O)286", x, y, 1, location);
            break;
          case 11:
            Game1.createMultipleObjectDebris("(O)378", x, y, random.Next(1, 4), location);
            break;
          case 12:
            Game1.createMultipleObjectDebris("(O)384", x, y, random.Next(1, 4), location);
            break;
          case 13:
            Game1.createMultipleObjectDebris("(O)287", x, y, 1, location);
            break;
        }
      }
      else
      {
        switch (this.ItemId)
        {
          case "118":
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(9))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)382", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)378", x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris("(O)388", x, y, random.Next(2, 3), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? "(O)80" : random.Choose<string>("(O)92", "(O)370"), x, y, random.Choose<int>(2, 3), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris("(O)388", x, y, random.Next(2, 6), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
                    return;
                  case 8:
                    Game1.createMultipleObjectDebris("(O)770", x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)535", x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(5))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)66", x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris("(O)68", x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris("(O)709", x, y, 1, location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris("(O)535", x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                default:
                  return;
              }
            }
          case "120":
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(9))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)382", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)380", x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)378", x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris("(O)388", x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? "(O)84" : random.Choose<string>("(O)92", "(O)371"), x, y, random.Choose<int>(2, 3), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 4), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
                    return;
                  case 8:
                    Game1.createMultipleObjectDebris("(O)770", x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)536", x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(5))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)62", x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris("(O)70", x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris("(O)709", x, y, random.Next(1, 4), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris("(O)536", x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                default:
                  return;
              }
            }
          case "124":
          case "122":
            if (random.NextDouble() < 0.65)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(8))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)382", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)384", x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)380", x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris("(O)378", x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris("(O)390", x, y, random.Next(2, 6), location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris("(O)388", x, y, random.Next(2, 6), location);
                    return;
                  case 7:
                    Game1.createMultipleObjectDebris("(O)881", x, y, random.Next(2, 6), location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(4))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)537", x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris(who.timesReachedMineBottom > 0 ? "(O)82" : "(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  default:
                    return;
                }
              }
            }
            else
            {
              if (random.NextDouble() >= 0.4)
                break;
              switch (random.Next(6))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)60", x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris("(O)64", x, y, 1, location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris("(O)709", x, y, random.Next(1, 4), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris("(O)749", x, y, 1, location);
                  return;
                case 4:
                  Game1.createItemDebris(MineShaft.getSpecialItemForThisMineLevel(level, x, y), new Vector2((float) x, (float) y) * 64f + new Vector2(32f, 32f), random.Next(4), location);
                  return;
                case 5:
                  Game1.createMultipleObjectDebris("(O)688", x, y, 1, location);
                  return;
                default:
                  return;
              }
            }
          case "174":
            if (random.NextDouble() < 0.1)
              Game1.player.team.RequestLimitedNutDrops("VolcanoBarrel", location, x * 64 /*0x40*/, y * 64 /*0x40*/, 5);
            if (location is VolcanoDungeon volcanoDungeon && volcanoDungeon.level.Value == 5 && x == 34)
            {
              Item obj = ItemRegistry.Create("(O)851");
              obj.Quality = 2;
              Game1.createItemDebris(obj, new Vector2((float) x, (float) y) * 64f, 1);
              break;
            }
            if (random.NextDouble() < 0.75)
            {
              if (random.NextDouble() < 0.8)
              {
                switch (random.Next(7))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)382", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)384", x, y, random.Next(1, 4), location);
                    return;
                  case 2:
                    location.characters.Add((NPC) new DwarvishSentry(new Vector2((float) x, (float) y) * 64f));
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)380", x, y, random.Next(2, 6), location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris("(O)378", x, y, random.Next(2, 6), location);
                    return;
                  case 5:
                    Game1.createMultipleObjectDebris("66", x, y, 1, location);
                    return;
                  case 6:
                    Game1.createMultipleObjectDebris("(O)709", x, y, random.Next(2, 6), location);
                    return;
                  default:
                    return;
                }
              }
              else
              {
                switch (random.Next(5))
                {
                  case 0:
                    Game1.createMultipleObjectDebris("(O)78", x, y, random.Next(1, 3), location);
                    return;
                  case 1:
                    Game1.createMultipleObjectDebris("(O)749", x, y, random.Next(1, 3), location);
                    return;
                  case 2:
                    Game1.createMultipleObjectDebris("(O)60", x, y, 1, location);
                    return;
                  case 3:
                    Game1.createMultipleObjectDebris("(O)64", x, y, 1, location);
                    return;
                  case 4:
                    Game1.createMultipleObjectDebris("(O)68", x, y, 1, location);
                    return;
                  default:
                    return;
                }
              }
            }
            else if (random.NextDouble() < 0.4)
            {
              switch (random.Next(9))
              {
                case 0:
                  Game1.createMultipleObjectDebris("(O)72", x, y, 1, location);
                  return;
                case 1:
                  Game1.createMultipleObjectDebris("(O)831", x, y, random.Next(1, 4), location);
                  return;
                case 2:
                  Game1.createMultipleObjectDebris("(O)833", x, y, random.Next(1, 3), location);
                  return;
                case 3:
                  Game1.createMultipleObjectDebris("(O)749", x, y, 1, location);
                  return;
                case 4:
                  Game1.createMultipleObjectDebris("(O)386", x, y, 1, location);
                  return;
                case 5:
                  Game1.createMultipleObjectDebris("(O)848", x, y, 1, location);
                  return;
                case 6:
                  Game1.createMultipleObjectDebris("(O)856", x, y, 1, location);
                  return;
                case 7:
                  Game1.createMultipleObjectDebris("(O)886", x, y, 1, location);
                  return;
                case 8:
                  Game1.createMultipleObjectDebris("(O)688", x, y, 1, location);
                  return;
                default:
                  return;
              }
            }
            else
            {
              location.characters.Add((NPC) new DwarvishSentry(new Vector2((float) x, (float) y) * 64f));
              break;
            }
        }
      }
    }
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.shakeTimer <= 0)
      return;
    this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    Vector2 vector2 = this.getScale() * 4f;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
    Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0), (int) ((double) local.Y - (double) vector2.Y / 2.0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
    if (this.shakeTimer > 0)
    {
      int num = this.shakeTimer / 100 + 1;
      destinationRectangle.X += Game1.random.Next(-num, num + 1);
      destinationRectangle.Y += Game1.random.Next(-num, num + 1);
    }
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), destinationRectangle, new Rectangle?(dataOrErrorItem.GetSourceRect(this.showNextIndex.Value ? 1 : 0)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 1) / 10000f));
  }
}
