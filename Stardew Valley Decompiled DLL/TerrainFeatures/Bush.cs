// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Bush
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class Bush : LargeTerrainFeature
{
  public const float shakeRate = 0.0157079641f;
  public const float shakeDecayRate = 0.00306796166f;
  public const int smallBush = 0;
  public const int mediumBush = 1;
  public const int largeBush = 2;
  public const int greenTeaBush = 3;
  public const int walnutBush = 4;
  public const int daysToMatureGreenTeaBush = 20;
  /// <summary>The type of bush, usually matching a constant like <see cref="F:StardewValley.TerrainFeatures.Bush.smallBush" />.</summary>
  [XmlElement("size")]
  public readonly NetInt size = new NetInt();
  [XmlElement("datePlanted")]
  public readonly NetInt datePlanted = new NetInt();
  [XmlElement("tileSheetOffset")]
  public readonly NetInt tileSheetOffset = new NetInt();
  public float health;
  [XmlElement("flipped")]
  public readonly NetBool flipped = new NetBool();
  /// <summary>Whether this is a cosmetic bush which produces no berries.</summary>
  [XmlElement("townBush")]
  public readonly NetBool townBush = new NetBool();
  /// <summary>Whether this bush is planted in a garden pot.</summary>
  public readonly NetBool inPot = new NetBool();
  [XmlElement("drawShadow")]
  public readonly NetBool drawShadow = new NetBool(true);
  private bool shakeLeft;
  private float shakeRotation;
  private float maxShake;
  [XmlIgnore]
  public float shakeTimer;
  [XmlIgnore]
  public readonly NetRectangle sourceRect = new NetRectangle();
  [XmlIgnore]
  public NetMutex uniqueSpawnMutex = new NetMutex();
  public static Lazy<Texture2D> texture = new Lazy<Texture2D>((Func<Texture2D>) (() => Game1.content.Load<Texture2D>("TileSheets\\bushes")));
  public static Rectangle shadowSourceRect = new Rectangle(663, 1011, 41, 30);
  private float yDrawOffset;

  public Bush()
    : base(true)
  {
  }

  public Bush(Vector2 tileLocation, int size, GameLocation location, int datePlantedOverride = -1)
    : this()
  {
    this.Tile = tileLocation;
    this.size.Value = size;
    this.Location = location;
    this.townBush.Value = location is Town && (size == 0 || size == 1 || size == 2) && (double) tileLocation.X % 5.0 != 0.0;
    if (location.map.RequireLayer("Front").Tiles[(int) tileLocation.X, (int) tileLocation.Y] != null)
      this.drawShadow.Value = false;
    this.datePlanted.Value = datePlantedOverride == -1 ? (int) Game1.stats.DaysPlayed : datePlantedOverride;
    switch (size)
    {
      case 3:
        this.drawShadow.Value = false;
        break;
      case 4:
        this.tileSheetOffset.Value = 1;
        break;
    }
    GameLocation location1 = this.Location;
    this.Location = location;
    this.loadSprite();
    this.Location = location1;
    this.flipped.Value = Game1.random.NextBool();
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.size, "size").AddField((INetSerializable) this.tileSheetOffset, "tileSheetOffset").AddField((INetSerializable) this.flipped, "flipped").AddField((INetSerializable) this.townBush, "townBush").AddField((INetSerializable) this.drawShadow, "drawShadow").AddField((INetSerializable) this.sourceRect, "sourceRect").AddField((INetSerializable) this.datePlanted, "datePlanted").AddField((INetSerializable) this.inPot, "inPot").AddField((INetSerializable) this.uniqueSpawnMutex.NetFields, "uniqueSpawnMutex.NetFields");
  }

  public int getAge() => (int) Game1.stats.DaysPlayed - this.datePlanted.Value;

  public void setUpSourceRect()
  {
    Season cosmeticSeason = this.GetCosmeticSeason();
    int num1 = (int) cosmeticSeason;
    switch (this.size.Value)
    {
      case 0:
        this.sourceRect.Value = new Rectangle(num1 * 16 /*0x10*/ * 2 + this.tileSheetOffset.Value * 16 /*0x10*/, 224 /*0xE0*/, 16 /*0x10*/, 32 /*0x20*/);
        break;
      case 1:
        if (this.townBush.Value)
        {
          this.sourceRect.Value = new Rectangle(num1 * 16 /*0x10*/ * 2, 96 /*0x60*/, 32 /*0x20*/, 32 /*0x20*/);
          break;
        }
        int num2 = num1 * 16 /*0x10*/ * 4 + this.tileSheetOffset.Value * 16 /*0x10*/ * 2;
        this.sourceRect.Value = new Rectangle(num2 % Bush.texture.Value.Bounds.Width, num2 / Bush.texture.Value.Bounds.Width * 3 * 16 /*0x10*/, 32 /*0x20*/, 48 /*0x30*/);
        break;
      case 2:
        if (this.townBush.Value && (cosmeticSeason == Season.Spring || cosmeticSeason == Season.Summer))
        {
          this.sourceRect.Value = new Rectangle(48 /*0x30*/, 176 /*0xB0*/, 48 /*0x30*/, 48 /*0x30*/);
          break;
        }
        switch (cosmeticSeason)
        {
          case Season.Spring:
          case Season.Summer:
            this.sourceRect.Value = new Rectangle(0, 128 /*0x80*/, 48 /*0x30*/, 48 /*0x30*/);
            return;
          case Season.Fall:
            this.sourceRect.Value = new Rectangle(48 /*0x30*/, 128 /*0x80*/, 48 /*0x30*/, 48 /*0x30*/);
            return;
          case Season.Winter:
            this.sourceRect.Value = new Rectangle(0, 176 /*0xB0*/, 48 /*0x30*/, 48 /*0x30*/);
            return;
          default:
            return;
        }
      case 3:
        int age = this.getAge();
        switch (cosmeticSeason)
        {
          case Season.Spring:
            this.sourceRect.Value = new Rectangle(Math.Min(2, age / 10) * 16 /*0x10*/ + this.tileSheetOffset.Value * 16 /*0x10*/, 256 /*0x0100*/, 16 /*0x10*/, 32 /*0x20*/);
            return;
          case Season.Summer:
            this.sourceRect.Value = new Rectangle(64 /*0x40*/ + Math.Min(2, age / 10) * 16 /*0x10*/ + this.tileSheetOffset.Value * 16 /*0x10*/, 256 /*0x0100*/, 16 /*0x10*/, 32 /*0x20*/);
            return;
          case Season.Fall:
            this.sourceRect.Value = new Rectangle(Math.Min(2, age / 10) * 16 /*0x10*/ + this.tileSheetOffset.Value * 16 /*0x10*/, 288, 16 /*0x10*/, 32 /*0x20*/);
            return;
          case Season.Winter:
            this.sourceRect.Value = new Rectangle(64 /*0x40*/ + Math.Min(2, age / 10) * 16 /*0x10*/ + this.tileSheetOffset.Value * 16 /*0x10*/, 288, 16 /*0x10*/, 32 /*0x20*/);
            return;
          default:
            return;
        }
      case 4:
        this.sourceRect.Value = new Rectangle(this.tileSheetOffset.Value * 32 /*0x20*/, 320, 32 /*0x20*/, 32 /*0x20*/);
        break;
    }
  }

  /// <summary>Get whether this bush has berries or fruit to harvest.</summary>
  public bool readyForHarvest() => this.tileSheetOffset.Value == 1;

  /// <summary>Get the season for which to show a bush sprite (which isn't necessarily the season for which it produces fruit or leaves).</summary>
  public virtual Season GetCosmeticSeason()
  {
    if (this.size.Value == 1)
      return this.Location.GetSeason();
    return !this.IsSheltered() ? this.Location.GetSeason() : Season.Spring;
  }

  /// <summary>Whether this bush is in a greenhouse or indoor pot.</summary>
  public bool IsSheltered()
  {
    if (this.Location != null && this.Location.SeedsIgnoreSeasonsHere())
      return true;
    return this.inPot.Value && !this.Location.IsOutdoors;
  }

  /// <summary>Get whether this bush is in season to produce items, regardless of whether it has any currently.</summary>
  public bool inBloom()
  {
    if (this.size.Value == 4)
      return this.readyForHarvest();
    GameLocation location = this.Location;
    Season season = location != null ? location.GetSeason() : Game1.season;
    int dayOfMonth = Game1.dayOfMonth;
    if (this.size.Value == 3)
    {
      bool flag = this.getAge() >= 20 && dayOfMonth >= 22 && (season != Season.Winter || this.IsSheltered());
      if (flag && this.Location != null && this.Location.IsFarm)
      {
        foreach (Farmer allFarmer in Game1.getAllFarmers())
          allFarmer.autoGenerateActiveDialogueEvent("cropMatured_815");
      }
      return flag;
    }
    switch (season)
    {
      case Season.Spring:
        return dayOfMonth > 14 && dayOfMonth < 19;
      case Season.Fall:
        return dayOfMonth > 7 && dayOfMonth < 12;
      default:
        return false;
    }
  }

  public override bool isActionable() => true;

  public override void loadSprite()
  {
    Vector2 tile = this.Tile;
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, (double) tile.X, (double) tile.Y * 777.0);
    double num = random.NextDouble() < 0.5 ? 0.0 : (double) random.Next(6) / 100.0;
    if (this.size.Value != 4)
    {
      if (this.size.Value == 1 && !this.readyForHarvest() && random.NextDouble() < 0.2 + num && this.inBloom())
        this.tileSheetOffset.Value = 1;
      else if (Game1.GetSeasonForLocation(this.Location) != Season.Summer && !this.inBloom())
        this.tileSheetOffset.Value = 0;
    }
    if (this.size.Value == 3)
      this.tileSheetOffset.Value = this.inBloom() ? 1 : 0;
    this.setUpSourceRect();
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    switch (this.size.Value)
    {
      case 0:
      case 3:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
      case 1:
      case 4:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 128 /*0x80*/, 64 /*0x40*/);
      case 2:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 192 /*0xC0*/, 64 /*0x40*/);
      default:
        return Rectangle.Empty;
    }
  }

  public override Rectangle getRenderBounds()
  {
    Vector2 tile = this.Tile;
    switch (this.size.Value)
    {
      case 0:
      case 3:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) ((double) tile.Y - 1.0) * 64 /*0x40*/, 64 /*0x40*/, 160 /*0xA0*/);
      case 1:
      case 4:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) ((double) tile.Y - 2.0) * 64 /*0x40*/, 128 /*0x80*/, 256 /*0x0100*/);
      case 2:
        return new Rectangle((int) tile.X * 64 /*0x40*/, (int) ((double) tile.Y - 2.0) * 64 /*0x40*/, 192 /*0xC0*/, 256 /*0x0100*/);
      default:
        return Rectangle.Empty;
    }
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    GameLocation location1 = this.Location;
    this.NeedsUpdate = true;
    if (Game1.didPlayerJustRightClick(true))
      this.shakeTimer = 0.0f;
    if ((double) this.shakeTimer <= 0.0)
    {
      Season season = location1.GetSeason();
      if ((double) this.maxShake == 0.0 && (this.size.Value != 3 || season != Season.Winter || this.IsSheltered()))
        location1.localSound("leafrustle");
      GameLocation location2 = this.Location;
      this.Location = location1;
      this.shake(tileLocation, false);
      this.Location = location2;
      this.shakeTimer = 500f;
    }
    return true;
  }

  public override bool tickUpdate(GameTime time)
  {
    if ((double) this.shakeTimer > 0.0)
      this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
    if (this.size.Value == 4)
      this.uniqueSpawnMutex.Update(this.Location);
    if ((double) this.maxShake > 0.0)
    {
      if (this.shakeLeft)
      {
        this.shakeRotation -= (float) Math.PI / 200f;
        if ((double) this.shakeRotation <= -(double) this.maxShake)
          this.shakeLeft = false;
      }
      else
      {
        this.shakeRotation += (float) Math.PI / 200f;
        if ((double) this.shakeRotation >= (double) this.maxShake)
          this.shakeLeft = true;
      }
      this.maxShake = Math.Max(0.0f, this.maxShake - 0.00306796166f);
    }
    if ((double) this.shakeTimer <= 0.0 && this.size.Value != 4 && (double) this.maxShake <= 0.0)
      this.NeedsUpdate = false;
    return false;
  }

  public void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
  {
    if (!((double) this.maxShake == 0.0 | doEvenIfStillShaking))
      return;
    this.shakeLeft = (double) Game1.player.Tile.X > (double) tileLocation.X || (double) Game1.player.Tile.X == (double) tileLocation.X && Game1.random.NextBool();
    this.maxShake = (float) Math.PI / 128f;
    this.NeedsUpdate = true;
    if (!this.townBush.Value && this.readyForHarvest() && this.inBloom())
    {
      string shakeOff = this.GetShakeOffItem();
      if (shakeOff == null)
        return;
      this.tileSheetOffset.Value = 0;
      this.setUpSourceRect();
      switch (this.size.Value)
      {
        case 3:
          Game1.createObjectDebris(shakeOff, (int) tileLocation.X, (int) tileLocation.Y);
          break;
        case 4:
          this.uniqueSpawnMutex.RequestLock((Action) (() =>
          {
            Game1.player.team.MarkCollectedNut($"Bush_{this.Location.Name}_{tileLocation.X.ToString()}_{tileLocation.Y.ToString()}");
            Item obj = ItemRegistry.Create(shakeOff);
            Rectangle boundingBox = this.getBoundingBox();
            double x = (double) boundingBox.Center.X;
            boundingBox = this.getBoundingBox();
            double y = (double) (boundingBox.Bottom - 2);
            Vector2 pixelOrigin = new Vector2((float) x, (float) y);
            GameLocation location = this.Location;
            boundingBox = this.getBoundingBox();
            int bottom = boundingBox.Bottom;
            Game1.createItemDebris(obj, pixelOrigin, 0, location, bottom);
          }));
          break;
        default:
          int howMuch = Utility.CreateRandom((double) tileLocation.X, (double) tileLocation.Y * 5000.0, (double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed).Next(1, 2) + Game1.player.ForagingLevel / 4;
          for (int index = 0; index < howMuch; ++index)
          {
            Item obj = ItemRegistry.Create(shakeOff);
            if (Game1.player.professions.Contains(16 /*0x10*/))
              obj.Quality = 4;
            Game1.createItemDebris(obj, Utility.PointToVector2(this.getBoundingBox().Center), Game1.random.Next(1, 4));
          }
          Game1.player.gainExperience(2, howMuch);
          break;
      }
      if (this.size.Value == 3)
        return;
      DelayedAction.playSoundAfterDelay("leafrustle", 100);
    }
    else if ((double) tileLocation.X == 20.0 && (double) tileLocation.Y == 8.0 && Game1.dayOfMonth == 28 && Game1.timeOfDay == 1200 && !Game1.player.mailReceived.Contains("junimoPlush"))
    {
      Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(F)1733"), new ItemGrabMenu.behaviorOnItemSelect(this.junimoPlushCallback));
    }
    else
    {
      if (!(Game1.currentLocation is Town currentLocation))
        return;
      if ((double) tileLocation.X == 28.0 && (double) tileLocation.Y == 14.0 && Game1.player.eventsSeen.Contains("520702") && !Game1.player.hasMagnifyingGlass)
      {
        currentLocation.initiateMagnifyingGlassGet();
      }
      else
      {
        if ((double) tileLocation.X != 47.0 || (double) tileLocation.Y != 100.0 || !Game1.player.secretNotesSeen.Contains(21) || Game1.timeOfDay != 2440 || !Game1.player.mailReceived.Add("secretNote21_done"))
          return;
        currentLocation.initiateMarnieLewisBush();
      }
    }
  }

  /// <summary>Get the qualified or unqualified item ID to produce when the bush is shaken, assuming it's in bloom.</summary>
  public string GetShakeOffItem()
  {
    switch (this.size.Value)
    {
      case 3:
        return "(O)815";
      case 4:
        return "(O)73";
      default:
        switch (this.Location.GetSeason())
        {
          case Season.Spring:
            return "(O)296";
          case Season.Fall:
            return "(O)410";
          default:
            return (string) null;
        }
    }
  }

  public void junimoPlushCallback(Item item, Farmer who)
  {
    if (!(item?.QualifiedItemId == "(F)1733") || who == null)
      return;
    who.mailReceived.Add("junimoPlush");
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => c is JunimoHarvester;

  public override void dayUpdate()
  {
    GameLocation location = this.Location;
    this.NeedsUpdate = true;
    Season season = location.GetSeason();
    if (this.size.Value == 4)
      return;
    Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, (double) this.Tile.X, (double) this.Tile.Y * 777.0);
    double num = random.NextDouble() < 0.5 ? 0.0 : (double) random.Next(6) / 100.0;
    if (this.size.Value == 1 && !this.readyForHarvest() && random.NextDouble() < 0.2 + num && this.inBloom())
      this.tileSheetOffset.Value = 1;
    else if (season != Season.Summer && !this.inBloom())
      this.tileSheetOffset.Value = 0;
    if (this.size.Value == 3)
      this.tileSheetOffset.Value = this.inBloom() ? 1 : 0;
    this.setUpSourceRect();
    Vector2 tile = this.Tile;
    if ((double) tile.X == 6.0 && (double) tile.Y == 7.0 && location.Name == "Sunroom")
      return;
    this.health = 0.0f;
  }

  /// <inheritdoc />
  public override bool seasonUpdate(bool onLoad)
  {
    if (this.size.Value == 4 || Game1.IsMultiplayer && !Game1.IsServer)
      return false;
    Season season = this.Location.GetSeason();
    this.tileSheetOffset.Value = this.size.Value != 1 || season != Season.Summer || !Game1.random.NextBool() ? 0 : 1;
    this.loadSprite();
    return false;
  }

  public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    this.NeedsUpdate = true;
    if (this.size.Value == 4)
      return false;
    if (explosion > 0)
    {
      this.shake(tileLocation, true);
      return false;
    }
    if (this.size.Value == 3 && t is MeleeWeapon meleeWeapon && meleeWeapon.ItemId == "66")
      this.shake(tileLocation, true);
    else if (t is Axe axe && this.isDestroyable())
    {
      location.playSound("leafrustle", new Vector2?(tileLocation));
      this.shake(tileLocation, true);
      if (axe.upgradeLevel.Value >= 1 || this.size.Value == 3)
      {
        this.health -= this.size.Value == 3 ? 0.5f : (float) axe.upgradeLevel.Value / 5f;
        if ((double) this.health <= -1.0)
        {
          location.playSound("treethud", new Vector2?(tileLocation));
          DelayedAction.playSoundAfterDelay("leafrustle", 100, location, new Vector2?(tileLocation));
          Color color = Color.Green;
          Season season = location.GetSeason();
          if (!this.IsSheltered())
          {
            switch (season)
            {
              case Season.Spring:
                color = Color.Green;
                break;
              case Season.Summer:
                color = Color.ForestGreen;
                break;
              case Season.Fall:
                color = Color.IndianRed;
                break;
              case Season.Winter:
                color = Color.Cyan;
                break;
            }
          }
          if (location.Name == "Sunroom")
          {
            foreach (NPC character in location.characters)
            {
              character.jump();
              character.doEmote(12);
            }
          }
          for (int index1 = 0; index1 <= this.getEffectiveSize(); ++index1)
          {
            for (int index2 = 0; index2 < 12; ++index2)
            {
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(355, 1200 + (season.Equals((object) "fall") ? 16 /*0x10*/ : (season.Equals((object) "winter") ? -16 : 0)), 16 /*0x10*/, 16 /*0x10*/), Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(0.0f, (float) Game1.random.Next(64 /*0x40*/)), false, 0.01f, color)
              {
                motion = new Vector2((float) Game1.random.Next(-10, 11) / 10f, (float) -Game1.random.Next(5, 7)),
                acceleration = new Vector2(0.0f, (float) Game1.random.Next(13, 17) / 100f),
                accelerationChange = new Vector2(0.0f, -1f / 1000f),
                scale = 4f,
                layerDepth = (float) (((double) tileLocation.Y + 1.0) * 64.0 / 10000.0),
                animationLength = 11,
                totalNumberOfLoops = 99,
                interval = (float) Game1.random.Next(20, 90),
                delayBeforeAnimationStart = (index1 + 1) * index2 * 20
              });
              if (index2 % 6 == 0)
              {
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(32f, (float) Game1.random.Next(32 /*0x20*/, 64 /*0x40*/)), color));
                Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random) - new Vector2(32f, (float) Game1.random.Next(32 /*0x20*/, 64 /*0x40*/)), Color.White));
              }
            }
          }
          if (this.size.Value == 3)
            Game1.createItemDebris(ItemRegistry.Create("(O)251"), tileLocation * 64f, 2, location);
          return true;
        }
        location.playSound("axchop", new Vector2?(tileLocation));
      }
    }
    return false;
  }

  public bool isDestroyable()
  {
    if (this.size.Value == 3)
      return true;
    if (this.Location is Farm)
    {
      Vector2 tile = this.Tile;
      switch (Game1.whichFarm)
      {
        case 1:
          return new Rectangle(32 /*0x20*/, 11, 11, 25).Contains((int) tile.X, (int) tile.Y);
        case 2:
          return (double) tile.X == 13.0 && (double) tile.Y == 35.0 || (double) tile.X == 37.0 && (double) tile.Y == 9.0 || new Rectangle(43, 11, 34, 50).Contains((int) tile.X, (int) tile.Y);
        case 3:
          return new Rectangle(24, 56, 10, 8).Contains((int) tile.X, (int) tile.Y);
        case 6:
          return new Rectangle(20, 44, 36, 44).Contains((int) tile.X, (int) tile.Y);
      }
    }
    return false;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 positionOnScreen,
    Vector2 tileLocation,
    float scale,
    float layerDepth)
  {
    layerDepth += positionOnScreen.X / 100000f;
    spriteBatch.Draw(Bush.texture.Value, positionOnScreen + new Vector2(0.0f, -64f * scale), new Rectangle?(new Rectangle(32 /*0x20*/, 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
  }

  public override void performPlayerEntryAction()
  {
    base.performPlayerEntryAction();
    Season season = this.Location.GetSeason();
    if (season != Season.Winter && !this.Location.IsRainingHere() && Game1.isDarkOut(this.Location) && Game1.random.NextBool(season == Season.Summer ? 0.08 : 0.04))
      AmbientLocationSounds.addSound(this.Tile, 3);
    NetRectangle sourceRect = this.sourceRect;
    if ((sourceRect != null ? (sourceRect.X < 0 ? 1 : 0) : 0) == 0)
      return;
    this.setUpSourceRect();
  }

  private int getEffectiveSize()
  {
    switch (this.size.Value)
    {
      case 3:
        return 0;
      case 4:
        return 1;
      default:
        return this.size.Value;
    }
  }

  public void draw(SpriteBatch spriteBatch, float yDrawOffset)
  {
    this.yDrawOffset = yDrawOffset;
    this.draw(spriteBatch);
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    if (this.drawShadow.Value)
    {
      if (this.getEffectiveSize() > 0)
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (((double) tile.X + (this.getEffectiveSize() == 1 ? 0.5 : 1.0)) * 64.0 - 51.0), (float) ((double) tile.Y * 64.0 - 16.0) + this.yDrawOffset)), new Rectangle?(Bush.shadowSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
      else
        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 + 64.0 - 4.0) + this.yDrawOffset)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 1E-06f);
    }
    spriteBatch.Draw(Bush.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f + (float) ((this.getEffectiveSize() + 1) * 64 /*0x40*/ / 2), (float) (((double) tile.Y + 1.0) * 64.0 - (this.getEffectiveSize() <= 0 || this.townBush.Value && this.getEffectiveSize() == 1 || this.size.Value == 4 ? 0.0 : 64.0)) + this.yDrawOffset)), new Rectangle?(this.sourceRect.Value), Color.White, this.shakeRotation, new Vector2((float) ((this.getEffectiveSize() + 1) * 16 /*0x10*/ / 2), 32f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) (this.getBoundingBox().Center.Y + 48 /*0x30*/) / 10000.0 - (double) tile.X / 1000000.0));
  }
}
