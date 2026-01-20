// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.ResourceClump
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

[XmlInclude(typeof (GiantCrop))]
public class ResourceClump : TerrainFeature
{
  public const int greenRainBush1Index = 44;
  public const int greenRainBush2Index = 46;
  public const int stumpIndex = 600;
  public const int hollowLogIndex = 602;
  public const int meteoriteIndex = 622;
  public const int boulderIndex = 672;
  public const int mineRock1Index = 752;
  public const int mineRock2Index = 754;
  public const int mineRock3Index = 756;
  public const int mineRock4Index = 758;
  public const int quarryBoulderIndex = 148;
  [XmlElement("width")]
  public readonly NetInt width = new NetInt();
  [XmlElement("height")]
  public readonly NetInt height = new NetInt();
  [XmlElement("parentSheetIndex")]
  public readonly NetInt parentSheetIndex = new NetInt();
  [XmlElement("textureName")]
  public readonly NetString textureName = new NetString();
  [XmlElement("health")]
  public readonly NetFloat health = new NetFloat();
  /// <summary>The backing field for <see cref="P:StardewValley.TerrainFeatures.ResourceClump.Tile" />.</summary>
  [XmlElement("tile")]
  public readonly NetVector2 netTile = new NetVector2();
  [XmlIgnore]
  public float shakeTimer;
  private Texture2D texture;
  private int lastToolHitTicker;

  /// <inheritdoc />
  [XmlIgnore]
  public override Vector2 Tile
  {
    get => this.netTile.Value;
    set => this.netTile.Value = value;
  }

  public ResourceClump()
    : base(true)
  {
  }

  public ResourceClump(
    int parentSheetIndex,
    int width,
    int height,
    Vector2 tile,
    int? health = null,
    string textureName = null)
    : this()
  {
    this.width.Value = width;
    this.height.Value = height;
    this.parentSheetIndex.Value = parentSheetIndex;
    this.Tile = tile;
    this.textureName.Value = textureName;
    this.health.Value = (float) (health ?? this.GetDefaultHealth(parentSheetIndex));
    this.loadSprite();
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.width, "width").AddField((INetSerializable) this.height, "height").AddField((INetSerializable) this.parentSheetIndex, "parentSheetIndex").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.netTile, "netTile").AddField((INetSerializable) this.textureName, "textureName");
  }

  protected virtual int GetDefaultHealth(int parentSheetIndex)
  {
    switch (parentSheetIndex)
    {
      case 148:
      case 602:
      case 622:
        return 20;
      case 600:
      case 672:
        return 10;
      case 752:
      case 754:
      case 756:
      case 758:
        return 8;
      default:
        return 1;
    }
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => false;

  /// <summary>Get whether this is a green rain bush.</summary>
  public bool IsGreenRainBush()
  {
    return this.parentSheetIndex.Value == 44 || this.parentSheetIndex.Value == 46;
  }

  public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
  {
    if (t == null || this.lastToolHitTicker == t.swingTicker)
      return false;
    this.lastToolHitTicker = t.swingTicker;
    float num = Math.Max(1f, (float) (t.upgradeLevel.Value + 1) * 0.75f);
    GameLocation location = this.Location;
    int debrisType = 12;
    switch (this.parentSheetIndex.Value)
    {
      case 148:
      case 622:
        switch (t)
        {
          case Pickaxe _ when t.upgradeLevel.Value < 3:
            location.playSound("clubhit", new Vector2?(tileLocation));
            location.playSound("clank", new Vector2?(tileLocation));
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13952"));
            Game1.player.jitterStrength = 1f;
            return false;
          case Pickaxe _:
            location.playSound("hammer", new Vector2?(tileLocation));
            debrisType = 14;
            break;
          default:
            return false;
        }
        break;
      case 600:
        switch (t)
        {
          case Axe _ when t.upgradeLevel.Value < 1:
            location.playSound("axe", new Vector2?(tileLocation));
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13945"));
            Game1.player.jitterStrength = 1f;
            return false;
          case Axe _:
            location.playSound("axchop", new Vector2?(tileLocation));
            break;
          default:
            return false;
        }
        break;
      case 602:
        switch (t)
        {
          case Axe _ when t.upgradeLevel.Value < 2:
            location.playSound("axe", new Vector2?(tileLocation));
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13948"));
            Game1.player.jitterStrength = 1f;
            return false;
          case Axe _:
            location.playSound("axchop", new Vector2?(tileLocation));
            break;
          default:
            return false;
        }
        break;
      case 672:
        switch (t)
        {
          case Pickaxe _ when t.upgradeLevel.Value < 2:
            location.playSound("clubhit", new Vector2?(tileLocation));
            location.playSound("clank", new Vector2?(tileLocation));
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13956"));
            Game1.player.jitterStrength = 1f;
            return false;
          case Pickaxe _:
            location.playSound("hammer", new Vector2?(tileLocation));
            debrisType = 14;
            break;
          default:
            return false;
        }
        break;
      case 752:
      case 754:
      case 756:
      case 758:
        if (!(t is Pickaxe))
          return false;
        location.playSound("hammer", new Vector2?(tileLocation));
        debrisType = 14;
        this.shakeTimer = 500f;
        this.NeedsUpdate = true;
        break;
      default:
        if (this.IsGreenRainBush())
        {
          location.playSound((double) this.health.Value - (double) num <= 0.0 ? "cut" : "weed_cut", new Vector2?(tileLocation));
          this.shakeTimer = 500f;
          debrisType = 36;
          break;
        }
        break;
    }
    this.health.Value -= num;
    if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num / 12.0 && (this.parentSheetIndex.Value == 602 || this.parentSheetIndex.Value == 600))
    {
      Debris debris = new Debris(709, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
      debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
      debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
      location.debris.Add(debris);
    }
    Game1.createRadialDebris(Game1.currentLocation, debrisType, (int) tileLocation.X + Game1.random.Next(this.width.Value / 2 + 1), (int) tileLocation.Y + Game1.random.Next(this.height.Value / 2 + 1), Game1.random.Next(4, 9), false);
    if ((double) this.health.Value <= 0.0)
      return this.destroy(t, location, tileLocation);
    this.shakeTimer = 100f;
    this.NeedsUpdate = true;
    return false;
  }

  public bool destroy(Tool t, GameLocation location, Vector2 tileLocation)
  {
    if (t != null && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < 0.05)
    {
      StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
      if (unseenSecretNote != null)
        Game1.createItemDebris((Item) unseenSecretNote, tileLocation * 64f + new Vector2((float) (this.width.Value / 2), (float) (this.height.Value / 2)) * 64f, -1, location);
    }
    switch (this.parentSheetIndex.Value)
    {
      case 148:
      case 672:
      case 752:
      case 754:
      case 756:
      case 758:
        if (t == null)
          return false;
        int num = this.parentSheetIndex.Value == 672 ? 15 : 10;
        if (Game1.IsMultiplayer)
        {
          Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 1000.0, (double) tileLocation.Y);
          Game1.createMultipleObjectDebris("(O)390", (int) tileLocation.X, (int) tileLocation.Y, num, t.getLastFarmerToUse().UniqueMultiplayerID);
        }
        else
          Game1.createRadialDebris(Game1.currentLocation, 390, (int) tileLocation.X, (int) tileLocation.Y, num, false, item: true);
        location.playSound("boulderBreak", new Vector2?(tileLocation));
        Game1.createRadialDebris(Game1.currentLocation, 32 /*0x20*/, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 12), false);
        Color color = Color.White;
        switch (this.parentSheetIndex.Value)
        {
          case 752:
            color = new Color(188, 119, 98);
            break;
          case 754:
            color = new Color(168, 120, 95);
            break;
          case 756:
          case 758:
            color = new Color(67, 189, 238);
            break;
        }
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(48 /*0x30*/, tileLocation * 64f, color, 5, animationInterval: 180f, sourceRectWidth: 128 /*0x80*/, sourceRectHeight: 128 /*0x80*/)
        {
          alphaFade = 0.01f
        });
        return true;
      case 600:
      case 602:
        if (t == null)
          return false;
        if (t.getLastFarmerToUse() == Game1.player)
          ++Game1.stats.StumpsChopped;
        t.getLastFarmerToUse().gainExperience(2, 25);
        int amount = this.parentSheetIndex.Value == 602 ? 8 : 2;
        Random random1;
        if (Game1.IsMultiplayer)
        {
          Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 1000.0, (double) tileLocation.Y);
          random1 = Game1.recentMultiplayerRandom;
        }
        else
          random1 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
        if (t.getLastFarmerToUse().professions.Contains(12))
        {
          if (amount == 8)
            amount = 10;
          else if (random1.NextBool())
            ++amount;
        }
        Item obj = ItemRegistry.Create("(O)709", amount);
        if (Game1.IsMultiplayer)
          Game1.createMultipleItemDebris(obj, tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        else
          Game1.createMultipleItemDebris(obj, tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        location.playSound("stumpCrack", new Vector2?(tileLocation));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(23, tileLocation * 64f, Color.White, 4, animationInterval: 140f, sourceRectWidth: 128 /*0x80*/, sourceRectHeight: 128 /*0x80*/));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(385, 1522, (int) sbyte.MaxValue, 79), 2000f, 1, 1, tileLocation * 64f + new Vector2(0.0f, 49f), false, false, 1E-05f, 0.016f, Color.White, 1f, 0.0f, 0.0f, 0.0f));
        Game1.createRadialDebris(Game1.currentLocation, 34, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(4, 9), false);
        if (random1.NextDouble() < 0.1)
          Game1.createMultipleObjectDebris("(O)292", (int) tileLocation.X, (int) tileLocation.Y, 1);
        if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          Game1.createObjectDebris("(O)890", (int) tileLocation.X, (int) tileLocation.Y, (int) tileLocation.Y, location: location);
        return true;
      case 622:
        if (t == null)
          return false;
        if (Game1.IsMultiplayer)
        {
          Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 1000.0, (double) tileLocation.Y);
          Random random2 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tileLocation.X, (double) tileLocation.Y * 983728.0);
          Game1.createMultipleObjectDebris("(O)386", (int) tileLocation.X, (int) tileLocation.Y, 10, t.getLastFarmerToUse().UniqueMultiplayerID);
          Game1.createMultipleObjectDebris("(O)390", (int) tileLocation.X, (int) tileLocation.Y, 8, t.getLastFarmerToUse().UniqueMultiplayerID);
          Game1.createMultipleObjectDebris("(O)749", (int) tileLocation.X, (int) tileLocation.Y, 2, t.getLastFarmerToUse().UniqueMultiplayerID);
          if (random2.NextDouble() < 0.25)
            Game1.createMultipleItemDebris(ItemRegistry.Create("(O)74"), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        }
        else
        {
          Game1.createMultipleItemDebris(ItemRegistry.Create("(O)386", 10), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
          Game1.createMultipleItemDebris(ItemRegistry.Create("(O)390", 8), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
          Game1.createMultipleObjectDebris("(O)749", (int) tileLocation.X, (int) tileLocation.Y, 2);
          if (Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tileLocation.X, (double) tileLocation.Y * 983728.0).NextDouble() < 0.25)
            Game1.createMultipleItemDebris(ItemRegistry.Create("(O)74"), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        }
        location.playSound("boulderBreak", new Vector2?(tileLocation));
        Game1.createRadialDebris(Game1.currentLocation, 32 /*0x20*/, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(6, 12), false);
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f, Color.White));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 0.0f)) * 64f, Color.White, animationInterval: 110f));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(1f, 1f)) * 64f, Color.White, flipped: true, animationInterval: 80f));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2(0.0f, 1f)) * 64f, Color.White, animationInterval: 90f));
        Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(5, tileLocation * 64f + new Vector2(32f, 32f), Color.White, animationInterval: 70f));
        return true;
      default:
        if (!this.IsGreenRainBush())
          return false;
        Color green = Color.Green;
        for (int x = 0; x < 2; ++x)
        {
          for (int y = 0; y < 2; ++y)
          {
            Vector2 vector2 = tileLocation + new Vector2((float) x, (float) y);
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, vector2 * 64f, green));
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, vector2 * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), green * 0.75f)
            {
              scale = 0.75f,
              flipped = true
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, vector2 * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), green * 0.75f)
            {
              scale = 0.75f,
              delayBeforeAnimationStart = 50
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, vector2 * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), green * 0.75f)
            {
              scale = 0.75f,
              flipped = true,
              delayBeforeAnimationStart = 100
            });
          }
        }
        t?.getLastFarmerToUse().gainExperience(2, 15);
        Random random3 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
        Game1.createMultipleItemDebris(ItemRegistry.Create("(O)Moss", random3.Next(2, 4)), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        Game1.createMultipleItemDebris(ItemRegistry.Create("(O)771", random3.Next(2, 4)), tileLocation * 64f + new Vector2((float) this.width.Value / 3f, (float) this.height.Value / 3f) * 64f, -1, Game1.currentLocation);
        if (random3.NextDouble() < 0.05)
          Game1.createMultipleItemDebris(ItemRegistry.Create("(O)MossySeed"), tileLocation * 64f + new Vector2((float) this.width.Value / 4f, (float) this.height.Value / 4f) * 64f, -1, Game1.currentLocation);
        return true;
    }
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, this.width.Value * 64 /*0x40*/, this.height.Value * 64 /*0x40*/);
  }

  public bool occupiesTile(int x, int y)
  {
    Vector2 tile = this.Tile;
    return (double) x >= (double) tile.X && (double) x - (double) tile.X < (double) this.width.Value && (double) y >= (double) tile.Y && (double) y - (double) tile.Y < (double) this.height.Value;
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    if (this.texture == null)
      this.loadSprite();
    Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(this.texture, this.parentSheetIndex.Value, 16 /*0x10*/, 16 /*0x10*/) with
    {
      Width = this.width.Value * 16 /*0x10*/,
      Height = this.height.Value * 16 /*0x10*/
    };
    Vector2 tile = this.Tile;
    Vector2 globalPosition = tile * 64f;
    if ((double) this.shakeTimer > 0.0)
      globalPosition.X += (float) Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 4f;
    spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(standardTileSheet), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y + 1.0) * 64.0 / 10000.0 + (double) tile.X / 100000.0));
  }

  public override void loadSprite()
  {
    this.texture = this.textureName.Value != null ? Game1.content.Load<Texture2D>(this.textureName.Value) : Game1.objectSpriteSheet;
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    if (!Game1.didPlayerJustRightClick(true))
    {
      Game1.haltAfterCheck = false;
      return false;
    }
    switch (this.parentSheetIndex.Value)
    {
      case 602:
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13962")));
        return true;
      case 622:
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13964")));
        return true;
      case 672:
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceClump.cs.13963")));
        return true;
      default:
        return false;
    }
  }

  public override bool tickUpdate(GameTime time)
  {
    if ((double) this.shakeTimer > 0.0)
      this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
    else
      this.NeedsUpdate = false;
    return false;
  }
}
