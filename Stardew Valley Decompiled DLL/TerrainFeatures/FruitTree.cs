// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.FruitTree
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.FruitTrees;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class FruitTree : TerrainFeature
{
  /// <summary>The asset name for the default fruit tree tilesheet.</summary>
  public const string DefaultTextureName = "TileSheets\\fruitTrees";
  public const float shakeRate = 0.0157079641f;
  public const float shakeDecayRate = 0.00306796166f;
  public const int minWoodDebrisForFallenTree = 12;
  public const int minWoodDebrisForStump = 5;
  public const int startingHealth = 10;
  public const int leafFallRate = 3;
  public const int DaysUntilMaturity = 28;
  public const int maxFruitsOnTrees = 3;
  public const int seedStage = 0;
  public const int sproutStage = 1;
  public const int saplingStage = 2;
  public const int bushStage = 3;
  public const int treeStage = 4;
  /// <summary>The texture from which to draw the tree sprites.</summary>
  [XmlIgnore]
  public Texture2D texture;
  [XmlElement("growthStage")]
  public readonly NetInt growthStage = new NetInt();
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.TerrainFeatures.FruitTree.treeId" /> instead.</summary>
  [XmlElement("treeType")]
  public string obsolete_treeType;
  /// <summary>The unique identifier for the underlying fruit tree data.</summary>
  [XmlElement("treeId")]
  public readonly NetString treeId = new NetString();
  /// <summary>The number of days until the fruit tree becomes full-grown.</summary>
  /// <remarks>The fruit tree is a seed at <see cref="F:StardewValley.TerrainFeatures.FruitTree.DaysUntilMaturity" /> and becomes full-grown at 0 or below.</remarks>
  [XmlElement("daysUntilMature")]
  public readonly NetInt daysUntilMature = new NetInt(28);
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.TerrainFeatures.FruitTree.fruit" /> instead.</summary>
  [XmlElement("fruitsOnTree")]
  public int? obsolete_fruitsOnTree;
  [XmlElement("fruit")]
  public readonly NetList<Item, NetRef<Item>> fruit = new NetList<Item, NetRef<Item>>();
  [XmlElement("struckByLightningCountdown")]
  public readonly NetInt struckByLightningCountdown = new NetInt();
  [XmlElement("health")]
  public readonly NetFloat health = new NetFloat(10f);
  [XmlElement("flipped")]
  public readonly NetBool flipped = new NetBool();
  [XmlElement("stump")]
  public readonly NetBool stump = new NetBool();
  /// <summary>Whether the tree is planted on a stone tile in the greenhouse.</summary>
  [XmlElement("greenHouseTileTree")]
  public readonly NetBool greenHouseTileTree = new NetBool();
  [XmlIgnore]
  public readonly NetBool shakeLeft = new NetBool();
  [XmlIgnore]
  public readonly NetBool falling = new NetBool();
  [XmlIgnore]
  public bool destroy;
  [XmlIgnore]
  public float shakeRotation;
  [XmlIgnore]
  public float maxShake;
  [XmlIgnore]
  public float alpha = 1f;
  private List<Leaf> leaves = new List<Leaf>();
  [XmlIgnore]
  public readonly NetLong lastPlayerToHit = new NetLong();
  [XmlIgnore]
  public float shakeTimer;
  [XmlElement("growthRate")]
  public readonly NetInt growthRate = new NetInt(1);

  /// <summary>The asset name loaded for <see cref="F:StardewValley.TerrainFeatures.FruitTree.texture" />.</summary>
  [XmlIgnore]
  public string textureName { get; private set; }

  /// <inheritdoc cref="F:StardewValley.TerrainFeatures.FruitTree.greenHouseTileTree" />
  [XmlIgnore]
  public bool GreenHouseTileTree
  {
    get => this.greenHouseTileTree.Value;
    set => this.greenHouseTileTree.Value = value;
  }

  public FruitTree()
    : this((string) null)
  {
  }

  public FruitTree(string id, int growthStage = 0)
    : base(true)
  {
    this.treeId.Value = id;
    this.growthStage.Value = growthStage;
    this.daysUntilMature.Value = FruitTree.GrowthStageToDaysUntilMature(growthStage);
    this.flipped.Value = Game1.random.NextBool();
    this.loadSprite();
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.growthStage, "this.growthStage").AddField((INetSerializable) this.treeId, "treeId").AddField((INetSerializable) this.daysUntilMature, "daysUntilMature").AddField((INetSerializable) this.fruit, "fruit").AddField((INetSerializable) this.struckByLightningCountdown, "struckByLightningCountdown").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.flipped, "flipped").AddField((INetSerializable) this.stump, "stump").AddField((INetSerializable) this.greenHouseTileTree, "greenHouseTileTree").AddField((INetSerializable) this.shakeLeft, "shakeLeft").AddField((INetSerializable) this.falling, "falling").AddField((INetSerializable) this.lastPlayerToHit, "lastPlayerToHit").AddField((INetSerializable) this.growthRate, "growthRate");
    this.treeId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((_param1, _param2, _param3) => this.loadSprite());
  }

  public int GetSpriteRowNumber()
  {
    FruitTreeData data = this.GetData();
    return data == null ? 0 : data.TextureSpriteRow;
  }

  public override void loadSprite()
  {
    string assetName = this.GetData()?.Texture ?? "TileSheets\\fruitTrees";
    if (this.texture != null && !(this.textureName != assetName))
      return;
    try
    {
      this.texture = Game1.content.Load<Texture2D>(assetName);
      this.textureName = assetName;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Fruit tree '{this.treeId.Value}' failed to load spritesheet '{assetName}'.", ex);
    }
  }

  public override bool isActionable() => true;

  /// <summary>Get whether the tree is in a location which ignores seasons (like the greenhouse or Ginger Island).</summary>
  public bool IgnoresSeasonsHere() => this.Location?.SeedsIgnoreSeasonsHere() ?? false;

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  public override Rectangle getRenderBounds()
  {
    Vector2 tile = this.Tile;
    return this.stump.Value || this.growthStage.Value < 4 ? new Rectangle((int) ((double) tile.X - 0.0) * 64 /*0x40*/, (int) ((double) tile.Y - 1.0) * 64 /*0x40*/, 64 /*0x40*/, 128 /*0x80*/) : new Rectangle((int) ((double) tile.X - 1.0) * 64 /*0x40*/, (int) ((double) tile.Y - 5.0) * 64 /*0x40*/, 192 /*0xC0*/, 448);
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    if ((double) this.maxShake == 0.0 && !this.stump.Value && this.growthStage.Value >= 3 && !this.IsWinterTreeHere())
      location.playSound("leafrustle");
    this.shake(tileLocation, false);
    return true;
  }

  public override bool tickUpdate(GameTime time)
  {
    if (this.destroy)
      return true;
    GameLocation location = this.Location;
    Vector2 tile = this.Tile;
    this.alpha = Math.Min(1f, this.alpha + 0.05f);
    if ((double) this.shakeTimer > 0.0)
      this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
    if (this.growthStage.Value >= 4 && !this.falling.Value && !this.stump.Value && Game1.player.GetBoundingBox().Intersects(new Rectangle(64 /*0x40*/ * ((int) tile.X - 1), 64 /*0x40*/ * ((int) tile.Y - 4), 192 /*0xC0*/, 224 /*0xE0*/)))
      this.alpha = Math.Max(0.4f, this.alpha - 0.09f);
    if (!this.falling.Value)
    {
      if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0 && this.leaves.Count <= 0 && (double) this.health.Value <= 0.0)
        return true;
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft.Value)
        {
          this.shakeRotation -= this.growthStage.Value >= 4 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
          if ((double) this.shakeRotation <= -(double) this.maxShake)
            this.shakeLeft.Value = false;
        }
        else
        {
          this.shakeRotation += this.growthStage.Value >= 4 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
          if ((double) this.shakeRotation >= (double) this.maxShake)
            this.shakeLeft.Value = true;
        }
      }
      if ((double) this.maxShake > 0.0)
        this.maxShake = Math.Max(0.0f, this.maxShake - (this.growthStage.Value >= 4 ? 0.00102265389f : 0.00306796166f));
      if (this.struckByLightningCountdown.Value > 0 && Game1.random.NextDouble() < 0.01)
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(tile.X * 64f + (float) Game1.random.Next(-64, 96 /*0x60*/), (float) ((double) tile.Y * 64.0 - 192.0) + (float) Game1.random.Next(-64, 128 /*0x80*/)), false, 1f / 500f, Color.Gray)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -0.5f),
          interval = 99999f,
          layerDepth = 1f,
          scale = 2f,
          scaleChange = 0.01f
        });
    }
    else
    {
      this.shakeRotation += this.shakeLeft.Value ? (float) -((double) this.maxShake * (double) this.maxShake) : this.maxShake * this.maxShake;
      this.maxShake += 0.00153398083f;
      if (Game1.random.NextDouble() < 0.01 && !this.IsWinterTreeHere())
        location.localSound("leafrustle");
      if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0)
      {
        this.falling.Value = false;
        this.maxShake = 0.0f;
        location.localSound("treethud");
        int num = Game1.random.Next(90, 120);
        for (int index = 0; index < num; ++index)
          this.leaves.Add(new Leaf(new Vector2((float) (Game1.random.Next((int) ((double) tile.X * 64.0), (int) ((double) tile.X * 64.0 + 192.0)) + (this.shakeLeft.Value ? -320 : 256 /*0x0100*/)), (float) ((double) tile.Y * 64.0 - 64.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(10, 40) / 10f));
        Farmer farmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
        Game1.createRadialDebris(location, 12, (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, (int) ((farmer.professions.Contains(12) ? 1.25 : 1.0) * 12.0), true);
        Game1.createRadialDebris(location, 12, (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, (int) ((farmer.professions.Contains(12) ? 1.25 : 1.0) * 12.0), false);
        if (Game1.IsMultiplayer)
          Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tile.X * 1000.0, (double) tile.Y);
        if (Game1.IsMultiplayer)
          Game1.createMultipleObjectDebris("(O)92", (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, 10, this.lastPlayerToHit.Value, location);
        else
          Game1.createMultipleObjectDebris("(O)92", (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, 10, location);
        if ((double) this.health.Value <= 0.0)
          this.health.Value = -100f;
      }
    }
    for (int index = this.leaves.Count - 1; index >= 0; --index)
    {
      Leaf leaf = this.leaves[index];
      leaf.position.Y -= leaf.yVelocity - 3f;
      leaf.yVelocity = Math.Max(0.0f, leaf.yVelocity - 0.01f);
      leaf.rotation += leaf.rotationRate;
      if ((double) leaf.position.Y >= (double) tile.Y * 64.0 + 64.0)
        this.leaves.RemoveAt(index);
    }
    return false;
  }

  /// <summary>Get the quality of fruit currently produced by the tree, matching one of the constants like <see cref="F:StardewValley.Object.highQuality" />.</summary>
  public int GetQuality()
  {
    if (this.struckByLightningCountdown.Value > 0 || this.daysUntilMature.Value >= 0)
      return 0;
    switch (this.daysUntilMature.Value / -112)
    {
      case 0:
        return 0;
      case 1:
        return 1;
      case 2:
        return 2;
      default:
        return 4;
    }
  }

  public virtual void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
  {
    if ((double) this.maxShake == 0.0 | doEvenIfStillShaking && this.growthStage.Value >= 3 && !this.stump.Value)
    {
      Vector2 standingPosition = Game1.player.getStandingPosition();
      this.shakeLeft.Value = (double) standingPosition.X > ((double) tileLocation.X + 0.5) * 64.0 || (double) Game1.player.Tile.X == (double) tileLocation.X && Game1.random.NextBool();
      this.maxShake = this.growthStage.Value >= 4 ? (float) Math.PI / 128f : (float) Math.PI / 64f;
      if (this.growthStage.Value >= 4)
      {
        if (Game1.random.NextDouble() < 0.66 && !this.IsWinterTreeHere())
        {
          int num = Game1.random.Next(1, 6);
          for (int index = 0; index < num; ++index)
            this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0 - 64.0), (int) ((double) tileLocation.X * 64.0 + 128.0)), (float) Game1.random.Next((int) ((double) tileLocation.Y * 64.0 - 256.0), (int) ((double) tileLocation.Y * 64.0 - 192.0))), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(5) / 10f));
        }
        int quality = this.GetQuality();
        TerrainFeature terrainFeature;
        if (!this.Location.terrainFeatures.TryGetValue(tileLocation, out terrainFeature) || !terrainFeature.Equals((object) this))
          return;
        for (int index = 0; index < this.fruit.Count; ++index)
        {
          Vector2 vector2 = new Vector2(0.0f, 0.0f);
          switch (index)
          {
            case 0:
              vector2.X = -64f;
              break;
            case 1:
              vector2.X = 64f;
              vector2.Y = -32f;
              break;
            case 2:
              vector2.Y = 32f;
              break;
          }
          Debris debris;
          if (this.struckByLightningCountdown.Value <= 0)
          {
            Item obj = this.fruit[index];
            this.fruit[index] = (Item) null;
            Vector2 debrisOrigin = new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2;
            Vector2 targetLocation = standingPosition;
            debris = new Debris(obj, debrisOrigin, targetLocation)
            {
              itemQuality = quality
            };
          }
          else
            debris = new Debris(382.ToString(), new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2, standingPosition)
            {
              itemQuality = quality
            };
          debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
          debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
          this.Location.debris.Add(debris);
        }
        this.fruit.Clear();
      }
      else
      {
        if (Game1.random.NextDouble() >= 0.66 || this.IsWinterTreeHere())
          return;
        int num = Game1.random.Next(1, 3);
        for (int index = 0; index < num; ++index)
          this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 48.0)), (float) ((double) tileLocation.Y * 64.0 - 96.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(30) / 10f));
      }
    }
    else
    {
      if (!this.stump.Value)
        return;
      this.shakeTimer = 100f;
    }
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => (double) this.health.Value <= -99.0;

  public static bool IsTooCloseToAnotherTree(
    Vector2 tileLocation,
    GameLocation environment,
    bool fruitTreesOnly = false)
  {
    Vector2 key = new Vector2();
    for (int index1 = (int) tileLocation.X - 2; index1 <= (int) tileLocation.X + 2; ++index1)
    {
      for (int index2 = (int) tileLocation.Y - 2; index2 <= (int) tileLocation.Y + 2; ++index2)
      {
        key.X = (float) index1;
        key.Y = (float) index2;
        TerrainFeature terrainFeature;
        if (environment.terrainFeatures.TryGetValue(key, out terrainFeature) && (terrainFeature is FruitTree || !fruitTreesOnly && terrainFeature is Tree))
          return true;
      }
    }
    return false;
  }

  /// <summary>Get whether a fruit tree is unable to grow due to nearby objects, terrain features, etc.</summary>
  /// <param name="tileLocation">The tile position containing the fruit tree.</param>
  /// <param name="environment">The location containing the fruit tree.</param>
  public static bool IsGrowthBlocked(Vector2 tileLocation, GameLocation environment)
  {
    foreach (Vector2 surroundingTileLocations in Utility.getSurroundingTileLocationsArray(tileLocation))
    {
      if (environment.IsTileOccupiedBy(surroundingTileLocations, CollisionMask.Objects))
      {
        string qualifiedItemId = environment.objects.GetValueOrDefault(surroundingTileLocations)?.QualifiedItemId;
        if (!(qualifiedItemId == "(O)590") && !(qualifiedItemId == "(O)SeedSpot"))
          return true;
      }
      if (environment.IsTileOccupiedBy(surroundingTileLocations, CollisionMask.TerrainFeatures))
      {
        switch (environment.terrainFeatures.GetValueOrDefault(surroundingTileLocations))
        {
          case HoeDirt hoeDirt:
            if (hoeDirt.crop != null)
              return true;
            break;
          case Grass _:
            break;
          default:
            return true;
        }
      }
      if (environment.IsTileOccupiedBy(surroundingTileLocations, CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.Furniture | CollisionMask.LocationSpecific))
        return true;
    }
    return false;
  }

  /// <summary>Get the fruit tree's data from <see cref="F:StardewValley.Game1.fruitTreeData" />, if found.</summary>
  public FruitTreeData GetData()
  {
    FruitTreeData data;
    return !FruitTree.TryGetData(this.treeId.Value, out data) ? (FruitTreeData) null : data;
  }

  /// <summary>Try to get a fruit tree's data from <see cref="F:StardewValley.Game1.fruitTreeData" />.</summary>
  /// <param name="id">The tree type ID (i.e. the key in <see cref="F:StardewValley.Game1.fruitTreeData" />).</param>
  /// <param name="data">The fruit tree data, if found.</param>
  /// <returns>Returns whether the fruit tree data was found.</returns>
  public static bool TryGetData(string id, out FruitTreeData data)
  {
    if (id != null)
      return Game1.fruitTreeData.TryGetValue(id, out data);
    data = (FruitTreeData) null;
    return false;
  }

  /// <summary>Get the translated display name for this tree, like 'Cherry' or 'Mango'.</summary>
  public string GetDisplayName()
  {
    return TokenParser.ParseText(this.GetData()?.DisplayName) ?? ItemRegistry.GetErrorItemName();
  }

  public override void dayUpdate()
  {
    GameLocation location = this.Location;
    if ((double) this.health.Value <= -99.0)
      this.destroy = true;
    if (this.struckByLightningCountdown.Value > 0)
    {
      --this.struckByLightningCountdown.Value;
      if (this.struckByLightningCountdown.Value <= 0)
        this.fruit.Clear();
    }
    bool flag = FruitTree.IsGrowthBlocked(this.Tile, location);
    if (!flag || this.daysUntilMature.Value <= 0)
    {
      if (this.daysUntilMature.Value > 28)
        this.daysUntilMature.Value = 28;
      if (this.growthRate.Value > 1)
      {
        int num = this.growthRate.Value;
      }
      this.daysUntilMature.Value -= this.growthRate.Value;
      this.growthStage.Value = FruitTree.DaysUntilMatureToGrowthStage(this.daysUntilMature.Value);
    }
    else if (flag && this.growthStage.Value != 4)
    {
      string str = this.GetData()?.DisplayName ?? this.GetDisplayName();
      Game1.multiplayer.broadcastGlobalMessage("Strings\\UI:FruitTree_Warning", true, (GameLocation) null, str);
    }
    if (this.stump.Value)
      this.fruit.Clear();
    else
      this.TryAddFruit();
  }

  /// <summary>Get the maximum <see cref="F:StardewValley.TerrainFeatures.FruitTree.daysUntilMature" /> value which would match a given growth stage.</summary>
  /// <param name="growthStage">The growth stage (matching a constant like <see cref="F:StardewValley.TerrainFeatures.FruitTree.treeStage" />).</param>
  public static int GrowthStageToDaysUntilMature(int growthStage)
  {
    if (growthStage > 4)
      growthStage = 4;
    switch (growthStage)
    {
      case 1:
        return 21;
      case 2:
        return 14;
      case 3:
        return 7;
      case 4:
        return 0;
      default:
        return 28;
    }
  }

  /// <summary>Get the growth stage (matching a constant like <see cref="F:StardewValley.TerrainFeatures.FruitTree.treeStage" />) for a given <see cref="F:StardewValley.TerrainFeatures.FruitTree.daysUntilMature" /> value.</summary>
  /// <param name="daysUntilMature">The <see cref="F:StardewValley.TerrainFeatures.FruitTree.daysUntilMature" /> value.</param>
  public static int DaysUntilMatureToGrowthStage(int daysUntilMature)
  {
    for (int growthStage = 4; growthStage >= 0; --growthStage)
    {
      if (daysUntilMature <= FruitTree.GrowthStageToDaysUntilMature(growthStage))
        return growthStage;
    }
    return 0;
  }

  /// <summary>Try to add a fruit to the tree.</summary>
  public bool TryAddFruit()
  {
    if (!this.stump.Value && this.growthStage.Value >= 4 && (this.IsInSeasonHere() || this.struckByLightningCountdown.Value > 0 && !this.IsWinterTreeHere()) && this.fruit.Count < 3)
    {
      FruitTreeData data = this.GetData();
      if (data?.Fruit != null)
      {
        foreach (FruitTreeFruitData drop in data.Fruit)
        {
          Item fruit = this.TryCreateFruit(drop);
          if (fruit != null)
          {
            this.fruit.Add(fruit);
            return true;
          }
        }
      }
    }
    return false;
  }

  /// <summary>Create a fruit item if its fields match.</summary>
  /// <param name="drop">The fruit data.</param>
  /// <returns>Returns the produced item (if any), else <c>null</c>.</returns>
  private Item TryCreateFruit(FruitTreeFruitData drop)
  {
    if (!Game1.random.NextBool(drop.Chance))
      return (Item) null;
    if (drop.Condition != null && !GameStateQuery.CheckConditions(drop.Condition, this.Location, ignoreQueryKeys: this.IgnoresSeasonsHere() ? GameStateQuery.SeasonQueryKeys : (HashSet<string>) null))
      return (Item) null;
    Season? season = drop.Season;
    if (season.HasValue && !this.IgnoresSeasonsHere())
    {
      season = drop.Season;
      Season seasonForLocation = Game1.GetSeasonForLocation(this.Location);
      if (!(season.GetValueOrDefault() == seasonForLocation & season.HasValue))
        return (Item) null;
    }
    Item fruit = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, new ItemQueryContext(this.Location, (Farmer) null, (Random) null, $"fruit tree '{this.treeId.Value}' > fruit '{drop.Id}'"), logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Fruit tree '{this.treeId.Value}' failed parsing item query '{query}' for fruit '{drop.Id}': {error}")));
    if (fruit != null)
      fruit.Quality = this.GetQuality();
    return fruit;
  }

  /// <summary>Get whether the fruit tree is in winter mode now (e.g. with no leaves).</summary>
  public virtual bool IsWinterTreeHere()
  {
    return !this.IgnoresSeasonsHere() && Game1.GetSeasonForLocation(this.Location) == Season.Winter;
  }

  /// <summary>Get whether the fruit tree can produce fruit now.</summary>
  public virtual bool IsInSeasonHere()
  {
    if (this.IgnoresSeasonsHere())
      return true;
    List<Season> seasons = this.GetData()?.Seasons;
    // ISSUE: explicit non-virtual call
    if (seasons != null && __nonvirtual (seasons.Count) > 0)
    {
      Season seasonForLocation = Game1.GetSeasonForLocation(this.Location);
      foreach (Season season in seasons)
      {
        if (seasonForLocation == season)
          return true;
      }
    }
    return false;
  }

  /// <summary>Get the season for which to show a fruit tree sprite (which isn't necessarily the season for which it produces fruit).</summary>
  public virtual Season GetCosmeticSeason()
  {
    return !this.IgnoresSeasonsHere() ? this.Location.GetSeason() : Season.Summer;
  }

  /// <inheritdoc />
  public override bool seasonUpdate(bool onLoad)
  {
    if (!this.IsInSeasonHere() && !onLoad)
      this.fruit.Clear();
    return false;
  }

  public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
  {
    if ((double) this.health.Value <= -99.0 || t is MeleeWeapon)
      return false;
    GameLocation location = this.Location;
    if (this.growthStage.Value >= 4)
    {
      if (t is Axe)
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        location.debris.Add(new Debris(12, Game1.random.Next(t.upgradeLevel.Value * 2, t.upgradeLevel.Value * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position, 0));
        this.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
        int quality = this.GetQuality();
        TerrainFeature terrainFeature;
        if (location.terrainFeatures.TryGetValue(tileLocation, out terrainFeature) && terrainFeature.Equals((object) this))
        {
          for (int index = 0; index < this.fruit.Count; ++index)
          {
            Vector2 vector2 = new Vector2(0.0f, 0.0f);
            switch (index)
            {
              case 0:
                vector2.X = -64f;
                break;
              case 1:
                vector2.X = 64f;
                vector2.Y = -32f;
                break;
              case 2:
                vector2.Y = 32f;
                break;
            }
            Debris debris;
            if (this.struckByLightningCountdown.Value <= 0)
            {
              Item obj = this.fruit[index];
              this.fruit[index] = (Item) null;
              Vector2 debrisOrigin = new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2;
              Vector2 standingPosition = Game1.player.getStandingPosition();
              debris = new Debris(obj, debrisOrigin, standingPosition)
              {
                itemQuality = quality
              };
            }
            else
              debris = new Debris(382.ToString(), new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2, Game1.player.getStandingPosition())
              {
                itemQuality = quality
              };
            debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
            debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
            location.debris.Add(debris);
          }
          this.fruit.Clear();
        }
      }
      else if (explosion <= 0)
        return false;
      this.shake(tileLocation, true);
      float num;
      if (explosion > 0)
      {
        num = (float) explosion;
      }
      else
      {
        if (t == null)
          return false;
        switch (t.upgradeLevel.Value)
        {
          case 0:
            num = 1f;
            break;
          case 1:
            num = 1.25f;
            break;
          case 2:
            num = 1.67f;
            break;
          case 3:
            num = 2.5f;
            break;
          case 4:
            num = 5f;
            break;
          default:
            num = (float) (t.upgradeLevel.Value + 1);
            break;
        }
      }
      this.health.Value -= num;
      if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num / 5.0)
      {
        Debris debris = new Debris("388", new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
        debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
        debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
        location.debris.Add(debris);
      }
      if ((double) this.health.Value <= 0.0)
      {
        if (!this.stump.Value)
        {
          location.playSound("treecrack", new Vector2?(tileLocation));
          this.stump.Value = true;
          this.health.Value = 5f;
          this.falling.Value = true;
          if (t?.getLastFarmerToUse() == null)
            this.shakeLeft.Value = true;
          else
            this.shakeLeft.Value = (double) t.getLastFarmerToUse().StandingPixel.X > ((double) tileLocation.X + 0.5) * 64.0;
        }
        else
        {
          this.health.Value = -100f;
          Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(30, 40), false);
          if (Game1.IsMultiplayer)
            Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 2000.0, (double) tileLocation.Y);
          if (t?.getLastFarmerToUse() == null)
          {
            Game1.createMultipleObjectDebris("(O)92", (int) tileLocation.X, (int) tileLocation.Y, 2, location);
          }
          else
          {
            Farmer farmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
            if (Game1.IsMultiplayer)
            {
              Game1.createMultipleObjectDebris("(O)92", (int) tileLocation.X, (int) tileLocation.Y, 1, this.lastPlayerToHit.Value, location);
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, farmer.professions.Contains(12) ? 5 : 4, true);
            }
            else
            {
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((farmer.professions.Contains(12) ? 1.25 : 1.0) * 5.0), true);
              Game1.createMultipleObjectDebris("(O)92", (int) tileLocation.X, (int) tileLocation.Y, 1, location);
            }
          }
          if (this.treeId.Value != null)
            Game1.createItemDebris(ItemRegistry.Create("(O)" + this.treeId.Value, quality: this.GetQuality()), tileLocation * 64f, 2, location);
        }
      }
    }
    else if (this.growthStage.Value >= 3)
    {
      if (t != null && t.Name.Contains("Ax"))
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        location.playSound("leafrustle", new Vector2?(tileLocation));
        location.debris.Add(new Debris(12, Game1.random.Next(t.upgradeLevel.Value * 2, t.upgradeLevel.Value * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().getStandingPosition(), 0));
      }
      else if (explosion <= 0)
        return false;
      this.shake(tileLocation, true);
      float num = 1f;
      Random random = !Game1.IsMultiplayer ? Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0, (double) Game1.stats.DaysPlayed, (double) this.health.Value) : Game1.recentMultiplayerRandom;
      if (explosion > 0)
      {
        num = (float) explosion;
      }
      else
      {
        switch (t.upgradeLevel.Value)
        {
          case 0:
            num = 2f;
            break;
          case 1:
            num = 2.5f;
            break;
          case 2:
            num = 3.34f;
            break;
          case 3:
            num = 5f;
            break;
          case 4:
            num = 10f;
            break;
        }
      }
      int numberOfChunks = 0;
      while (t != null && random.NextDouble() < (double) num * 0.08 + (double) t.getLastFarmerToUse().ForagingLevel / 200.0)
        ++numberOfChunks;
      this.health.Value -= num;
      if (numberOfChunks > 0)
        Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, numberOfChunks, location);
      if ((double) this.health.Value <= 0.0)
      {
        if (this.treeId.Value != null)
          Game1.createItemDebris(ItemRegistry.Create("(O)" + this.treeId.Value), tileLocation * 64f, 2, location);
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(20, 30), false);
        return true;
      }
    }
    else if (this.growthStage.Value >= 1)
    {
      if (explosion > 0)
        return true;
      if (t != null && t.Name.Contains("Axe"))
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
      }
      if (t is Axe || t is Pickaxe || t is Hoe || t is MeleeWeapon)
      {
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
        if (t.Name.Contains("Axe") && Game1.recentMultiplayerRandom.NextDouble() < (double) t.getLastFarmerToUse().ForagingLevel / 10.0)
          Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, 1, location);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
        if (this.treeId.Value != null)
          Game1.createItemDebris(ItemRegistry.Create("(O)" + this.treeId.Value), tileLocation * 64f, 2, location);
        return true;
      }
    }
    else
    {
      if (explosion > 0)
        return true;
      if (t.Name.Contains("Axe") || t.Name.Contains("Pick") || t.Name.Contains("Hoe"))
      {
        location.playSound("woodyHit", new Vector2?(tileLocation));
        location.playSound("axchop", new Vector2?(tileLocation));
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
        if (this.treeId.Value != null)
          Game1.createItemDebris(ItemRegistry.Create("(O)" + this.treeId.Value), tileLocation * 64f, 2, location);
        return true;
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
    if (this.growthStage.Value < 4)
    {
      Rectangle rectangle;
      switch (this.growthStage.Value)
      {
        case 0:
          rectangle = new Rectangle(128 /*0x80*/, 512 /*0x0200*/, 64 /*0x40*/, 64 /*0x40*/);
          break;
        case 1:
          rectangle = new Rectangle(0, 512 /*0x0200*/, 64 /*0x40*/, 64 /*0x40*/);
          break;
        case 2:
          rectangle = new Rectangle(64 /*0x40*/, 512 /*0x0200*/, 64 /*0x40*/, 64 /*0x40*/);
          break;
        default:
          rectangle = new Rectangle(0, 384, 64 /*0x40*/, 128 /*0x80*/);
          break;
      }
      spriteBatch.Draw(this.texture, positionOnScreen - new Vector2(0.0f, (float) rectangle.Height * scale), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + (double) rectangle.Height * (double) scale) / 20000.0));
    }
    else
    {
      if (!this.falling.Value)
        spriteBatch.Draw(this.texture, positionOnScreen + new Vector2(0.0f, -64f * scale), new Rectangle?(new Rectangle(128 /*0x80*/, 384, 64 /*0x40*/, 128 /*0x80*/)), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
      if (this.stump.Value && !this.falling.Value)
        return;
      spriteBatch.Draw(this.texture, positionOnScreen + new Vector2(-64f * scale, -320f * scale), new Rectangle?(new Rectangle(0, 0, 192 /*0xC0*/, 384)), Color.White, this.shakeRotation, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale) / 20000.0));
    }
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    int indexForLocation = Game1.GetSeasonIndexForLocation(this.Location);
    int spriteRowNumber = this.GetSpriteRowNumber();
    Vector2 tile = this.Tile;
    Rectangle boundingBox = this.getBoundingBox();
    if (this.greenHouseTileTree.Value)
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tile.X * 64f, tile.Y * 64f)), new Rectangle?(new Rectangle(669, 1957, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
    if (this.growthStage.Value < 4)
    {
      Vector2 vector2 = new Vector2((float) Math.Max(-8.0, Math.Min(64.0, Math.Sin((double) tile.X * 200.0 / (2.0 * Math.PI)) * -16.0)), (float) Math.Max(-8.0, Math.Min(64.0, Math.Sin((double) tile.X * 200.0 / (2.0 * Math.PI)) * -16.0))) / 2f;
      Rectangle rectangle;
      switch (this.growthStage.Value)
      {
        case 0:
          rectangle = new Rectangle(0, spriteRowNumber * 5 * 16 /*0x10*/, 48 /*0x30*/, 80 /*0x50*/);
          break;
        case 1:
          rectangle = new Rectangle(48 /*0x30*/, spriteRowNumber * 5 * 16 /*0x10*/, 48 /*0x30*/, 80 /*0x50*/);
          break;
        case 2:
          rectangle = new Rectangle(96 /*0x60*/, spriteRowNumber * 5 * 16 /*0x10*/, 48 /*0x30*/, 80 /*0x50*/);
          break;
        default:
          rectangle = new Rectangle(144 /*0x90*/, spriteRowNumber * 5 * 16 /*0x10*/, 48 /*0x30*/, 80 /*0x50*/);
          break;
      }
      spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0) + vector2.X, (float) ((double) tile.Y * 64.0 - (double) rectangle.Height + 128.0) + vector2.Y)), new Rectangle?(rectangle), Color.White, this.shakeRotation, new Vector2(24f, 80f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) boundingBox.Bottom / 10000.0 - (double) tile.X / 1000000.0));
    }
    else
    {
      if (!this.stump.Value || this.falling.Value)
      {
        Season cosmeticSeason = this.GetCosmeticSeason();
        if (!this.falling.Value)
          spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle((12 + (int) cosmeticSeason * 3) * 16 /*0x10*/, spriteRowNumber * 5 * 16 /*0x10*/ + 64 /*0x40*/, 48 /*0x30*/, 16 /*0x10*/)), this.struckByLightningCountdown.Value > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, 0.0f, new Vector2(24f, 16f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-07f);
        spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle((12 + (int) cosmeticSeason * 3) * 16 /*0x10*/, spriteRowNumber * 5 * 16 /*0x10*/, 48 /*0x30*/, 64 /*0x40*/)), this.struckByLightningCountdown.Value > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, this.shakeRotation, new Vector2(24f, 80f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) boundingBox.Bottom / 10000.0 + 1.0 / 1000.0 - (double) tile.X / 1000000.0));
      }
      if ((double) this.health.Value >= 1.0 || !this.falling.Value && (double) this.health.Value > -99.0)
        spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 2.0 : 0.0)), (float) ((double) tile.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle(384, spriteRowNumber * 5 * 16 /*0x10*/ + 48 /*0x30*/, 48 /*0x30*/, 32 /*0x20*/)), this.struckByLightningCountdown.Value > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, 0.0f, new Vector2(24f, 32f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, !this.stump.Value || this.falling.Value ? (float) ((double) boundingBox.Bottom / 10000.0 - 1.0 / 1000.0 - (double) tile.X / 1000000.0) : (float) boundingBox.Bottom / 10000f);
      for (int index = 0; index < this.fruit.Count; ++index)
      {
        ParsedItemData parsedItemData = this.struckByLightningCountdown.Value > 0 ? ItemRegistry.GetDataOrErrorItem("(O)382") : ItemRegistry.GetDataOrErrorItem(this.fruit[index].QualifiedItemId);
        Texture2D texture = parsedItemData.GetTexture();
        Rectangle sourceRect = parsedItemData.GetSourceRect();
        switch (index)
        {
          case 0:
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 - 64.0 + (double) tile.X * 200.0 % 64.0 / 2.0), (float) ((double) tile.Y * 64.0 - 192.0 - (double) tile.X % 64.0 / 3.0))), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) boundingBox.Bottom / 10000.0 + 1.0 / 500.0 - (double) tile.X / 1000000.0));
            break;
          case 1:
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 - 256.0 + (double) tile.X * 232.0 % 64.0 / 3.0))), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) boundingBox.Bottom / 10000.0 + 1.0 / 500.0 - (double) tile.X / 1000000.0));
            break;
          case 2:
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + (double) tile.X * 200.0 % 64.0 / 3.0), (float) ((double) tile.Y * 64.0 - 160.0 + (double) tile.X * 200.0 % 64.0 / 3.0))), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, (float) ((double) boundingBox.Bottom / 10000.0 + 1.0 / 500.0 - (double) tile.X / 1000000.0));
            break;
        }
      }
    }
    foreach (Leaf leaf in this.leaves)
      spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, leaf.position), new Rectangle?(new Rectangle((24 + indexForLocation) * 16 /*0x10*/, spriteRowNumber * 5 * 16 /*0x10*/, 8, 8)), Color.White, leaf.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) boundingBox.Bottom / 10000.0 + 0.0099999997764825821));
  }
}
