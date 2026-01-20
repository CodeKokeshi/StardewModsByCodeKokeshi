// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Tree
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.WildTrees;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class Tree : TerrainFeature
{
  /// <remarks>The backing field for <see cref="M:StardewValley.TerrainFeatures.Tree.GetWildTreeDataDictionary" />.</remarks>
  protected static Dictionary<string, WildTreeData> _WildTreeData;
  /// <summary>The backing field for <see cref="M:StardewValley.TerrainFeatures.Tree.GetWildTreeSeedLookup" />.</summary>
  protected static Dictionary<string, List<string>> _WildTreeSeedLookup;
  public const float chanceForDailySeed = 0.05f;
  public const float shakeRate = 0.0157079641f;
  public const float shakeDecayRate = 0.00306796166f;
  public const int minWoodDebrisForFallenTree = 12;
  public const int minWoodDebrisForStump = 5;
  public const int startingHealth = 10;
  public const int leafFallRate = 3;
  public const int stageForMossGrowth = 14;
  /// <summary>The oak tree type ID in <c>Data/WildTrees</c>.</summary>
  public const string bushyTree = "1";
  /// <summary>The maple tree type ID in <c>Data/WildTrees</c>.</summary>
  public const string leafyTree = "2";
  /// <summary>The pine tree type ID in <c>Data/WildTrees</c>.</summary>
  public const string pineTree = "3";
  public const string winterTree1 = "4";
  public const string winterTree2 = "5";
  /// <summary>The palm tree type ID (valley variant) in <c>Data/WildTrees</c>.</summary>
  public const string palmTree = "6";
  /// <summary>The mushroom tree type ID in <c>Data/WildTrees</c>.</summary>
  public const string mushroomTree = "7";
  /// <summary>The mahogany tree type ID in <c>Data/WildTrees</c>.</summary>
  public const string mahoganyTree = "8";
  /// <summary>The palm tree type ID (Ginger Island variant) in <c>Data/WildTrees</c>.</summary>
  public const string palmTree2 = "9";
  public const string greenRainTreeBushy = "10";
  public const string greenRainTreeLeafy = "11";
  public const string greenRainTreeFern = "12";
  public const string mysticTree = "13";
  public const int seedStage = 0;
  public const int sproutStage = 1;
  public const int saplingStage = 2;
  public const int bushStage = 3;
  public const int treeStage = 5;
  /// <summary>The texture for the displayed tree sprites.</summary>
  [XmlIgnore]
  public Lazy<Texture2D> texture;
  /// <summary>The current season for the location containing the tree.</summary>
  protected Season? localSeason;
  [XmlElement("growthStage")]
  public readonly NetInt growthStage = new NetInt();
  [XmlElement("treeType")]
  public readonly NetString treeType = new NetString();
  [XmlElement("health")]
  public readonly NetFloat health = new NetFloat();
  [XmlElement("flipped")]
  public readonly NetBool flipped = new NetBool();
  [XmlElement("stump")]
  public readonly NetBool stump = new NetBool();
  [XmlElement("tapped")]
  public readonly NetBool tapped = new NetBool();
  [XmlElement("hasSeed")]
  public readonly NetBool hasSeed = new NetBool();
  [XmlElement("hasMoss")]
  public readonly NetBool hasMoss = new NetBool();
  [XmlElement("isTemporaryGreenRainTree")]
  public readonly NetBool isTemporaryGreenRainTree = new NetBool();
  [XmlIgnore]
  public readonly NetBool wasShakenToday = new NetBool();
  [XmlElement("fertilized")]
  public readonly NetBool fertilized = new NetBool();
  [XmlIgnore]
  public readonly NetBool shakeLeft = new NetBool().Interpolated(false, false);
  [XmlIgnore]
  public readonly NetBool falling = new NetBool();
  [XmlIgnore]
  public readonly NetBool destroy = new NetBool();
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
  [XmlElement("stopGrowingMoss")]
  public readonly NetBool stopGrowingMoss = new NetBool();
  public static Microsoft.Xna.Framework.Rectangle treeTopSourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 48 /*0x30*/, 96 /*0x60*/);
  public static Microsoft.Xna.Framework.Rectangle stumpSourceRect = new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/);
  public static Microsoft.Xna.Framework.Rectangle shadowSourceRect = new Microsoft.Xna.Framework.Rectangle(663, 1011, 41, 30);

  /// <summary>The asset name for the texture loaded by <see cref="F:StardewValley.TerrainFeatures.Tree.texture" />, if applicable.</summary>
  [XmlIgnore]
  public string TextureName { get; private set; }

  public Tree()
    : base(true)
  {
    this.resetTexture();
  }

  public Tree(string id, int growthStage, bool isGreenRainTemporaryTree = false)
    : this()
  {
    this.growthStage.Value = growthStage;
    this.isTemporaryGreenRainTree.Value = isGreenRainTemporaryTree;
    this.treeType.Value = id;
    if (this.treeType.Value == "4")
      this.treeType.Value = "1";
    if (this.treeType.Value == "5")
      this.treeType.Value = "2";
    this.flipped.Value = Game1.random.NextBool();
    this.health.Value = 10f;
  }

  public Tree(string id)
    : this()
  {
    this.treeType.Value = id;
    if (this.treeType.Value == "4")
      this.treeType.Value = "1";
    if (this.treeType.Value == "5")
      this.treeType.Value = "2";
    this.flipped.Value = Game1.random.NextBool();
    this.health.Value = 10f;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.growthStage, "growthStage").AddField((INetSerializable) this.treeType, "treeType").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.flipped, "flipped").AddField((INetSerializable) this.stump, "stump").AddField((INetSerializable) this.tapped, "tapped").AddField((INetSerializable) this.hasSeed, "hasSeed").AddField((INetSerializable) this.fertilized, "fertilized").AddField((INetSerializable) this.shakeLeft, "shakeLeft").AddField((INetSerializable) this.falling, "falling").AddField((INetSerializable) this.destroy, "destroy").AddField((INetSerializable) this.lastPlayerToHit, "lastPlayerToHit").AddField((INetSerializable) this.wasShakenToday, "wasShakenToday").AddField((INetSerializable) this.hasMoss, "hasMoss").AddField((INetSerializable) this.isTemporaryGreenRainTree, "isTemporaryGreenRainTree").AddField((INetSerializable) this.stopGrowingMoss, "stopGrowingMoss");
    this.treeType.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((a, b, c) => this.CheckForNewTexture());
  }

  /// <summary>Get the wild tree data from <c>Data/WildTrees</c>.</summary>
  /// <remarks>This is a specialized method; most code should use <see cref="M:StardewValley.TerrainFeatures.Tree.GetData" /> or <see cref="M:StardewValley.TerrainFeatures.Tree.TryGetData(System.String,StardewValley.GameData.WildTrees.WildTreeData@)" /> instead.</remarks>
  public static Dictionary<string, WildTreeData> GetWildTreeDataDictionary()
  {
    if (Tree._WildTreeData == null)
      Tree._LoadWildTreeData();
    return Tree._WildTreeData;
  }

  /// <summary>Get tree types indexed by their qualified and unqualified seed item IDs.</summary>
  public static Dictionary<string, List<string>> GetWildTreeSeedLookup()
  {
    if (Tree._WildTreeSeedLookup == null)
      Tree._LoadWildTreeData();
    return Tree._WildTreeSeedLookup;
  }

  /// <summary>Load the raw wild tree data from <c>Data/WildTrees</c>.</summary>
  /// <remarks>This generally shouldn't be called directly; most code should use <see cref="M:StardewValley.TerrainFeatures.Tree.GetWildTreeDataDictionary" /> or <see cref="M:StardewValley.TerrainFeatures.Tree.GetWildTreeSeedLookup" /> instead.</remarks>
  protected static void _LoadWildTreeData()
  {
    Tree._WildTreeData = DataLoader.WildTrees(Game1.content);
    Tree._WildTreeSeedLookup = new Dictionary<string, List<string>>();
    foreach (KeyValuePair<string, WildTreeData> keyValuePair in Tree._WildTreeData)
    {
      string key = keyValuePair.Key;
      WildTreeData wildTreeData = keyValuePair.Value;
      if (wildTreeData.SeedPlantable && !string.IsNullOrWhiteSpace(wildTreeData.SeedItemId))
      {
        ItemMetadata itemMetadata = ItemRegistry.ResolveMetadata(wildTreeData.SeedItemId);
        if (itemMetadata != null)
        {
          List<string> stringList;
          if (!Tree._WildTreeSeedLookup.TryGetValue(itemMetadata.QualifiedItemId, out stringList))
            Tree._WildTreeSeedLookup[itemMetadata.QualifiedItemId] = stringList = new List<string>();
          stringList.Add(key);
          if (!Tree._WildTreeSeedLookup.TryGetValue(itemMetadata.LocalItemId, out stringList))
            Tree._WildTreeSeedLookup[itemMetadata.LocalItemId] = stringList = new List<string>();
          stringList.Add(key);
        }
      }
    }
  }

  /// <summary>Get the next tree that will sprout when planting a seed item.</summary>
  /// <param name="itemId">The seed's qualified or unqualified item ID.</param>
  public static string ResolveTreeTypeFromSeed(string itemId)
  {
    ItemMetadata metadata = ItemRegistry.GetMetadata(itemId);
    List<string> options;
    return metadata?.TypeIdentifier == "(O)" && Tree.GetWildTreeSeedLookup().TryGetValue(metadata.LocalItemId, out options) ? Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (Game1.stats.Get("wildtreesplanted") + 1U)).ChooseFrom<string>((IList<string>) options) : (string) null;
  }

  /// <summary>Reset the cached wild tree data, so it's reloaded on the next request.</summary>
  internal static void ClearCache()
  {
    Tree._WildTreeData = (Dictionary<string, WildTreeData>) null;
    Tree._WildTreeSeedLookup = (Dictionary<string, List<string>>) null;
  }

  /// <summary>Reload the tree texture based on <see cref="F:StardewValley.GameData.WildTrees.WildTreeData.Textures" /> if a different texture would be selected now.</summary>
  public void CheckForNewTexture()
  {
    if (!this.texture.IsValueCreated)
      return;
    string str = this.ChooseTexture();
    if (str == null || !(str != this.TextureName))
      return;
    this.resetTexture();
  }

  /// <summary>Reset the tree texture, so it'll be reselected and reloaded next time it's accessed.</summary>
  public void resetTexture()
  {
    // ISSUE: method pointer
    this.texture = new Lazy<Texture2D>(new Func<Texture2D>((object) this, __methodptr(\u003CresetTexture\u003Eg__LoadTexture\u007C65_0)));
  }

  /// <summary>Get the tree's data from <c>Data/WildTrees</c>, if found.</summary>
  public WildTreeData GetData()
  {
    WildTreeData data;
    return !Tree.TryGetData(this.treeType.Value, out data) ? (WildTreeData) null : data;
  }

  /// <summary>Try to get a tree's data from <c>Data/WildTrees</c>.</summary>
  /// <param name="id">The tree type ID (i.e. the key in <c>Data/WildTrees</c>).</param>
  /// <param name="data">The tree data, if found.</param>
  /// <returns>Returns whether the tree data was found.</returns>
  public static bool TryGetData(string id, out WildTreeData data)
  {
    if (id != null)
      return Tree.GetWildTreeDataDictionary().TryGetValue(id, out data);
    data = (WildTreeData) null;
    return false;
  }

  /// <summary>Choose an applicable texture from <see cref="F:StardewValley.GameData.WildTrees.WildTreeData.Textures" />.</summary>
  protected string ChooseTexture()
  {
    WildTreeData data = this.GetData();
    if (data != null)
    {
      int? count = data.Textures?.Count;
      int num = 0;
      if (count.GetValueOrDefault() > num & count.HasValue)
      {
        Season? season1;
        foreach (WildTreeTextureData texture in data.Textures)
        {
          if (this.Location != null && this.Location.IsGreenhouse && texture.Season.HasValue)
          {
            season1 = texture.Season;
            Season season2 = Season.Spring;
            if (season1.GetValueOrDefault() == season2 & season1.HasValue)
              return texture.Texture;
          }
          else
          {
            if (texture.Season.HasValue)
            {
              season1 = texture.Season;
              Season? localSeason = this.localSeason;
              if (!(season1.GetValueOrDefault() == localSeason.GetValueOrDefault() & season1.HasValue == localSeason.HasValue))
                continue;
            }
            if (texture.Condition == null || GameStateQuery.CheckConditions(texture.Condition, this.Location))
              return texture.Texture;
          }
        }
        return data.Textures[0].Texture;
      }
    }
    return (string) null;
  }

  public override Microsoft.Xna.Framework.Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  public override Microsoft.Xna.Framework.Rectangle getRenderBounds()
  {
    Vector2 tile = this.Tile;
    return this.stump.Value || this.growthStage.Value < 5 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) tile.X - 0.0) * 64 /*0x40*/, (int) ((double) tile.Y - 1.0) * 64 /*0x40*/, 64 /*0x40*/, 128 /*0x80*/) : new Microsoft.Xna.Framework.Rectangle((int) ((double) tile.X - 1.0) * 64 /*0x40*/, (int) ((double) tile.Y - 5.0) * 64 /*0x40*/, 192 /*0xC0*/, 448);
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    if (!this.tapped.Value)
    {
      if ((double) this.maxShake == 0.0 && !this.stump.Value && this.growthStage.Value >= 3 && this.IsLeafy())
        location.localSound("leafrustle");
      this.shake(tileLocation, false);
    }
    return Game1.player.ActiveObject == null || !Game1.player.ActiveObject.canBePlacedHere(location, tileLocation, CollisionMask.All, false);
  }

  private int extraWoodCalculator(Vector2 tileLocation)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
    int num = 0;
    if (random.NextDouble() < Game1.player.DailyLuck)
      ++num;
    if (random.NextDouble() < (double) Game1.player.ForagingLevel / 12.5)
      ++num;
    if (random.NextDouble() < (double) Game1.player.ForagingLevel / 12.5)
      ++num;
    if (random.NextDouble() < (double) Game1.player.LuckLevel / 25.0)
      ++num;
    if (this.treeType.Value == "3")
      ++num;
    return num;
  }

  public override bool tickUpdate(GameTime time)
  {
    GameLocation location = this.Location;
    if (!this.localSeason.HasValue)
    {
      this.setSeason();
      this.CheckForNewTexture();
    }
    if ((double) this.shakeTimer > 0.0)
      this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
    if (this.destroy.Value)
      return true;
    this.alpha = Math.Min(1f, this.alpha + 0.05f);
    Vector2 tile = this.Tile;
    if (this.growthStage.Value >= 5 && !this.falling.Value && !this.stump.Value && Game1.player.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(64 /*0x40*/ * ((int) tile.X - 1), 64 /*0x40*/ * ((int) tile.Y - 5), 192 /*0xC0*/, 288)))
      this.alpha = Math.Max(0.4f, this.alpha - 0.09f);
    if (!this.falling.Value)
    {
      if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0 && this.leaves.Count <= 0 && (double) this.health.Value <= 0.0)
        return true;
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft.Value)
        {
          this.shakeRotation -= this.growthStage.Value >= 5 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
          if ((double) this.shakeRotation <= -(double) this.maxShake)
            this.shakeLeft.Value = false;
        }
        else
        {
          this.shakeRotation += this.growthStage.Value >= 5 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
          if ((double) this.shakeRotation >= (double) this.maxShake)
            this.shakeLeft.Value = true;
        }
      }
      if ((double) this.maxShake > 0.0)
        this.maxShake = Math.Max(0.0f, this.maxShake - (this.growthStage.Value >= 5 ? 0.00102265389f : 0.00306796166f));
    }
    else
    {
      this.shakeRotation += this.shakeLeft.Value ? (float) -((double) this.maxShake * (double) this.maxShake) : this.maxShake * this.maxShake;
      this.maxShake += 0.00153398083f;
      WildTreeData data = this.GetData();
      if (data != null && Game1.random.NextDouble() < 0.01 && this.IsLeafy())
        location.localSound("leafrustle");
      if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0)
      {
        this.falling.Value = false;
        this.maxShake = 0.0f;
        if (data != null)
        {
          location.localSound("treethud");
          if (this.IsLeafy())
          {
            int num = Game1.random.Next(90, 120);
            for (int index = 0; index < num; ++index)
              this.leaves.Add(new Leaf(new Vector2((float) (Game1.random.Next((int) ((double) tile.X * 64.0), (int) ((double) tile.X * 64.0 + 192.0)) + (this.shakeLeft.Value ? -320 : 256 /*0x0100*/)), (float) ((double) tile.Y * 64.0 - 64.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(10, 40) / 10f));
          }
          Random random;
          if (Game1.IsMultiplayer)
          {
            Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tile.X * 1000.0, (double) tile.Y);
            random = Game1.recentMultiplayerRandom;
          }
          else
            random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tile.X * 7.0, (double) tile.Y * 11.0);
          Farmer targetFarmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
          if (data.DropWoodOnChop)
          {
            int numberOfChunks = (int) ((targetFarmer.professions.Contains(12) ? 1.25 : 1.0) * (double) (12 + this.extraWoodCalculator(tile)));
            if (targetFarmer.stats.Get("Book_Woodcutting") > 0U && random.NextDouble() < 0.05)
              numberOfChunks *= 2;
            Game1.createRadialDebris(location, 12, (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, numberOfChunks, true);
            Game1.createRadialDebris(location, 12, (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, (int) ((targetFarmer.professions.Contains(12) ? 1.25 : 1.0) * (double) (12 + this.extraWoodCalculator(tile))), false);
          }
          if (data.DropWoodOnChop)
            Game1.createMultipleObjectDebris("(O)92", (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, 5, this.lastPlayerToHit.Value, location);
          int number = 0;
          if (data.DropHardwoodOnLumberChop)
          {
            while (targetFarmer.professions.Contains(14) && random.NextBool())
              ++number;
          }
          List<WildTreeChopItemData> chopItems = data.ChopItems;
          // ISSUE: explicit non-virtual call
          if ((chopItems != null ? (__nonvirtual (chopItems.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            bool flag = false;
            foreach (WildTreeChopItemData chopItem in data.ChopItems)
            {
              Item drop = this.TryGetDrop((WildTreeItemData) chopItem, random, targetFarmer, "ChopItems", isStump: new bool?(false));
              if (drop != null)
              {
                if (chopItem.ItemId == "709")
                {
                  number += drop.Stack;
                  flag = true;
                }
                else
                  Game1.createMultipleItemDebris(drop, new Vector2(tile.X + (this.shakeLeft.Value ? -4f : 4f), tile.Y) * 64f, -2, location);
              }
            }
            if (flag && targetFarmer.professions.Contains(14))
              number += (int) ((double) number * 0.25 + 0.89999997615814209);
          }
          if (number > 0)
            Game1.createMultipleObjectDebris("(O)709", (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, number, this.lastPlayerToHit.Value, location);
          float seedOnChopChance = data.SeedOnChopChance;
          if (targetFarmer.getEffectiveSkillLevel(2) >= 1 && data != null && data.SeedItemId != null && random.NextDouble() < (double) seedOnChopChance)
            Game1.createMultipleObjectDebris(data.SeedItemId, (int) tile.X + (this.shakeLeft.Value ? -4 : 4), (int) tile.Y, random.Next(1, 3), this.lastPlayerToHit.Value, location);
        }
        if ((double) this.health.Value == -100.0)
          return true;
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

  /// <summary>Get a dropped item if its fields match.</summary>
  /// <param name="drop">The drop data.</param>
  /// <param name="r">The RNG to use for random checks.</param>
  /// <param name="targetFarmer">The player interacting with the tree.</param>
  /// <param name="fieldName">The field name to show in error messages if the drop is invalid.</param>
  /// <param name="formatItemId">Format the selected item ID before it's resolved.</param>
  /// <param name="isStump">Whether the tree is a stump, or <c>null</c> to use <see cref="F:StardewValley.TerrainFeatures.Tree.stump" />.</param>
  /// <returns>Returns the produced item (if any), else <c>null</c>.</returns>
  public Item TryGetDrop(
    WildTreeItemData drop,
    Random r,
    Farmer targetFarmer,
    string fieldName,
    Func<string, string> formatItemId = null,
    bool? isStump = null)
  {
    if (!r.NextBool(drop.Chance))
      return (Item) null;
    Season? season1 = drop.Season;
    if (season1.HasValue)
    {
      season1 = drop.Season;
      Season season2 = this.Location.GetSeason();
      if (!(season1.GetValueOrDefault() == season2 & season1.HasValue))
        return (Item) null;
    }
    if (drop.Condition != null && !GameStateQuery.CheckConditions(drop.Condition, this.Location, targetFarmer, random: r))
      return (Item) null;
    if (drop is WildTreeChopItemData treeChopItemData)
    {
      int size = this.growthStage.Value;
      int num = (int) isStump ?? (this.stump.Value ? 1 : 0);
      if (!treeChopItemData.IsValidForGrowthStage(size, num != 0))
        return (Item) null;
    }
    return ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, new ItemQueryContext(this.Location, targetFarmer, r, $"wild tree '{this.treeType.Value}' > {fieldName} entry '{drop.Id}'"), formatItemId: formatItemId, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Wild tree '{this.treeType.Value}' failed parsing item query '{query}' for {fieldName} entry '{drop.Id}': {error}")));
  }

  public void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
  {
    GameLocation location = this.Location;
    WildTreeData data = this.GetData();
    if ((double) this.maxShake == 0.0 | doEvenIfStillShaking && this.growthStage.Value >= 3 && !this.stump.Value)
    {
      this.shakeLeft.Value = (double) Game1.player.StandingPixel.X > ((double) tileLocation.X + 0.5) * 64.0 || (double) Game1.player.Tile.X == (double) tileLocation.X && Game1.random.NextBool();
      this.maxShake = this.growthStage.Value >= 5 ? (float) Math.PI / 128f : (float) Math.PI / 64f;
      if (this.growthStage.Value >= 5)
      {
        if (this.IsLeafy())
        {
          if (Game1.random.NextDouble() < 0.66)
          {
            int num = Game1.random.Next(1, 6);
            for (int index = 0; index < num; ++index)
              this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0 - 64.0), (int) ((double) tileLocation.X * 64.0 + 128.0)), (float) Game1.random.Next((int) ((double) tileLocation.Y * 64.0 - 256.0), (int) ((double) tileLocation.Y * 64.0 - 192.0))), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(5) / 10f));
          }
          if (Game1.random.NextDouble() < 0.01)
          {
            Season? localSeason = this.localSeason;
            Season season = Season.Spring;
            if (localSeason.GetValueOrDefault() == season & localSeason.HasValue || this.localSeason.GetValueOrDefault() == Season.Summer)
            {
              bool islandButterfly = this.Location.InIslandContext();
              while (Game1.random.NextDouble() < 0.8)
                location.addCritter((Critter) new Butterfly(location, new Vector2(tileLocation.X + (float) Game1.random.Next(1, 3), tileLocation.Y - 2f + (float) Game1.random.Next(-1, 2)), islandButterfly));
            }
          }
        }
        if (this.hasSeed.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1))
        {
          bool flag = true;
          if (data != null)
          {
            int? count = data.SeedDropItems?.Count;
            int num = 0;
            if (count.GetValueOrDefault() > num & count.HasValue)
            {
              foreach (WildTreeSeedDropItemData seedDropItem in data.SeedDropItems)
              {
                Item drop = this.TryGetDrop((WildTreeItemData) seedDropItem, Game1.random, Game1.player, "SeedDropItems");
                if (drop != null)
                {
                  if (Game1.player.professions.Contains(16 /*0x10*/) && drop.HasContextTag("forage_item"))
                    drop.Quality = 4;
                  Game1.createItemDebris(drop, new Vector2(tileLocation.X * 64f, (float) (((double) tileLocation.Y - 3.0) * 64.0)), -1, location, Game1.player.StandingPixel.Y);
                  if (!seedDropItem.ContinueOnDrop)
                  {
                    flag = false;
                    break;
                  }
                }
              }
            }
          }
          if (flag && data != null)
          {
            Item obj = ItemRegistry.Create(data.SeedItemId);
            if (Game1.player.professions.Contains(16 /*0x10*/) && obj.HasContextTag("forage_item"))
              obj.Quality = 4;
            Game1.createItemDebris(obj, new Vector2(tileLocation.X * 64f, (float) (((double) tileLocation.Y - 3.0) * 64.0)), -1, location, Game1.player.StandingPixel.Y);
          }
          if (Utility.tryRollMysteryBox(0.03))
            Game1.createItemDebris(ItemRegistry.Create(Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32 /*0x20*/);
          Utility.trySpawnRareObject(Game1.player, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, this.Location, 2.0, groundLevel: Game1.player.StandingPixel.Y - 32 /*0x20*/);
          if (Game1.random.NextBool() && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            Game1.createObjectDebris("(O)890", (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64 /*0x40*/, location: location);
          this.hasSeed.Value = false;
        }
        if (this.wasShakenToday.Value)
          return;
        this.wasShakenToday.Value = true;
        if (data?.ShakeItems == null)
          return;
        foreach (WildTreeItemData shakeItem in data.ShakeItems)
        {
          Item drop = this.TryGetDrop(shakeItem, Game1.random, Game1.player, "ShakeItems");
          if (drop != null)
            Game1.createItemDebris(drop, tileLocation * 64f, -2, this.Location);
        }
      }
      else
      {
        if (Game1.random.NextDouble() >= 0.66)
          return;
        int num = Game1.random.Next(1, 3);
        for (int index = 0; index < num; ++index)
          this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 48.0)), (float) ((double) tileLocation.Y * 64.0 - 32.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(30) / 10f));
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
  public override bool isPassable(Character c = null)
  {
    return (double) this.health.Value <= -99.0 || this.growthStage.Value == 0;
  }

  /// <summary>Get the maximum size the tree can grow in its current position.</summary>
  /// <param name="ignoreSeason">Whether to assume the tree is in-season.</param>
  public virtual int GetMaxSizeHere(bool ignoreSeason = false)
  {
    GameLocation location = this.Location;
    Vector2 tile = this.Tile;
    if (this.GetData() == null)
      return this.growthStage.Value;
    if (location.IsNoSpawnTile(tile, nameof (Tree)) && !location.doesEitherTileOrTileIndexPropertyEqual((int) tile.X, (int) tile.Y, "CanPlantTrees", "Back", "T"))
      return this.growthStage.Value;
    if (!ignoreSeason && !this.IsInSeason())
      return this.growthStage.Value;
    if (this.growthStage.Value == 0 && location.objects.ContainsKey(tile))
      return 0;
    return this.IsGrowthBlockedByNearbyTree() ? 4 : 15;
  }

  /// <summary>Get whether this tree is in-season for its current location, so it can grow if applicable.</summary>
  public bool IsInSeason()
  {
    return this.localSeason.GetValueOrDefault() != Season.Winter || this.fertilized.Value || this.Location.SeedsIgnoreSeasonsHere() || (this.GetData()?.GrowsInWinter ?? false);
  }

  /// <summary>Get whether growth is blocked because it's too close to another fully-grown tree.</summary>
  public bool IsGrowthBlockedByNearbyTree()
  {
    GameLocation location = this.Location;
    Vector2 tile = this.Tile;
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) (((double) tile.X - 1.0) * 64.0), (int) (((double) tile.Y - 1.0) * 64.0), 192 /*0xC0*/, 192 /*0xC0*/);
    foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
    {
      if (pair.Key != tile && pair.Value is Tree tree && tree.growthStage.Value >= 5 && tree.getBoundingBox().Intersects(rectangle))
        return true;
    }
    return false;
  }

  public void onGreenRainDay(bool undo = false)
  {
    if (undo)
    {
      if (!this.isTemporaryGreenRainTree.Value)
        return;
      this.isTemporaryGreenRainTree.Value = false;
      if (this.treeType.Value == "10")
        this.treeType.Value = "1";
      else
        this.treeType.Value = "2";
      this.resetTexture();
    }
    else
    {
      if (this.Location == null || !this.Location.IsOutdoors)
        return;
      if (this.growthStage.Value < 5)
      {
        if (this.growthStage.Value == 0 && (Game1.random.NextDouble() < 0.5 || this.Location == null || this.Location.objects.ContainsKey(this.Tile)))
          return;
        this.growthStage.Value = 4;
        for (int index = 0; index < 3; ++index)
          this.dayUpdate();
      }
      bool? growsMoss = this.GetData()?.GrowsMoss;
      if (growsMoss.HasValue && growsMoss.GetValueOrDefault() && Game1.random.NextBool())
        this.hasMoss.Value = true;
      if (!(this.treeType.Value == "1") && !(this.treeType.Value == "2") || this.growthStage.Value < 5 || !Game1.random.NextBool(0.75))
        return;
      this.isTemporaryGreenRainTree.Value = true;
      if (this.treeType.Value == "1")
        this.treeType.Value = "10";
      else
        this.treeType.Value = "11";
      this.resetTexture();
    }
  }

  public override void dayUpdate()
  {
    GameLocation location1 = this.Location;
    if (!Game1.IsFall && !Game1.IsWinter)
    {
      GameLocation location2 = this.Location;
      if ((location2 != null ? (location2.IsGreenRainingHere() ? 1 : 0) : 0) == 0 && this.isTemporaryGreenRainTree.Value)
      {
        this.isTemporaryGreenRainTree.Value = false;
        if (this.treeType.Value == "10")
          this.treeType.Value = "1";
        else
          this.treeType.Value = "2";
        this.resetTexture();
      }
    }
    this.wasShakenToday.Value = false;
    this.setSeason();
    this.CheckForNewTexture();
    WildTreeData data = this.GetData();
    Vector2 tile = this.Tile;
    if ((double) this.health.Value <= -100.0)
      this.destroy.Value = true;
    if (this.tapped.Value)
    {
      StardewValley.Object objectAtTile = location1.getObjectAtTile((int) tile.X, (int) tile.Y);
      if (objectAtTile == null || !objectAtTile.IsTapper())
        this.tapped.Value = false;
      else if (objectAtTile.IsTapper() && objectAtTile.heldObject.Value == null)
        this.UpdateTapperProduct(objectAtTile);
    }
    if (this.GetMaxSizeHere() > this.growthStage.Value)
    {
      float chance1 = data != null ? data.GrowthChance : 0.2f;
      float chance2 = data != null ? data.FertilizedGrowthChance : 1f;
      if (Game1.random.NextBool(chance1) || this.fertilized.Value && Game1.random.NextBool(chance2))
        ++this.growthStage.Value;
    }
    if (this.localSeason.GetValueOrDefault() == Season.Winter && data != null && data.IsStumpDuringWinter && !this.Location.SeedsIgnoreSeasonsHere())
      this.stump.Value = true;
    else if (data != null && data.IsStumpDuringWinter && Game1.dayOfMonth <= 1 && Game1.IsSpring)
    {
      this.stump.Value = false;
      this.health.Value = 10f;
      this.shakeRotation = 0.0f;
    }
    if (this.growthStage.Value >= 5 && !this.stump.Value && location1 is Farm && Game1.random.NextBool(data != null ? data.SeedSpreadChance : 0.15f))
    {
      int num1 = Game1.random.Next(-3, 4) + (int) tile.X;
      int num2 = Game1.random.Next(-3, 4) + (int) tile.Y;
      Vector2 vector2 = new Vector2((float) num1, (float) num2);
      if (!location1.IsNoSpawnTile(vector2, nameof (Tree)) && location1.isTileLocationOpen(new xTile.Dimensions.Location(num1, num2)) && !location1.IsTileOccupiedBy(vector2) && !location1.isWaterTile(num1, num2) && location1.isTileOnMap(vector2))
        location1.terrainFeatures.Add(vector2, (TerrainFeature) new Tree(this.treeType.Value, 0));
    }
    if (this.isTemporaryGreenRainTree.Value && location1.IsGreenhouse && (this.localSeason.GetValueOrDefault() == Season.Winter || this.localSeason.GetValueOrDefault() == Season.Fall))
      this.hasSeed.Value = false;
    else
      this.hasSeed.Value = data != null && data.SeedItemId != null && this.growthStage.Value >= 5 && Game1.random.NextBool(data.SeedOnShakeChance);
    bool flag = this.growthStage.Value >= 5 && !Game1.IsWinter && (this.treeType.Value == "10" || this.treeType.Value == "11") && !this.isTemporaryGreenRainTree.Value;
    if (this.growthStage.Value >= 5 && !Game1.IsWinter && !flag)
    {
      for (int x = (int) tile.X - 2; (double) x <= (double) tile.X + 2.0; ++x)
      {
        for (int y = (int) tile.Y - 2; (double) y <= (double) tile.Y + 2.0; ++y)
        {
          if (this.Location.terrainFeatures.GetValueOrDefault(new Vector2((float) x, (float) y)) is Tree valueOrDefault && valueOrDefault.growthStage.Value >= 5 && (valueOrDefault.treeType.Value == "10" || valueOrDefault.treeType.Value == "11") && !valueOrDefault.isTemporaryGreenRainTree.Value && valueOrDefault.hasMoss.Value)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          break;
      }
    }
    float chance = Game1.isRaining ? 0.2f : 0.1f;
    if (flag && Game1.random.NextDouble() < 0.5)
      ++this.growthStage.Value;
    if (Game1.IsSummer && !Game1.isGreenRain && !Game1.isRaining)
      chance = 0.033f;
    if (flag && Game1.random.NextDouble() < 0.5)
      chance += 0.1f;
    if (this.stopGrowingMoss.Value)
      this.hasMoss.Value = false;
    else if (!location1.IsGreenhouse && (this.localSeason.GetValueOrDefault() == Season.Winter || this.stump.Value))
    {
      this.hasMoss.Value = false;
    }
    else
    {
      bool? growsMoss = data?.GrowsMoss;
      if (!growsMoss.HasValue || !growsMoss.GetValueOrDefault() || this.growthStage.Value < 14 || this.stump.Value || !Game1.random.NextBool(chance))
        return;
      this.hasMoss.Value = true;
    }
  }

  public override void performPlayerEntryAction()
  {
    base.performPlayerEntryAction();
    this.setSeason();
    this.CheckForNewTexture();
  }

  /// <inheritdoc />
  public override bool seasonUpdate(bool onLoad)
  {
    if (!onLoad && Game1.IsFall && Game1.random.NextDouble() < 0.05 && !this.tapped.Value && (this.treeType.Value == "1" || this.treeType.Value == "2") && this.growthStage.Value >= 5 && this.Location != null && !(this.Location is Town) && !this.Location.IsGreenhouse)
    {
      this.treeType.Value = this.treeType.Value == "1" ? "10" : "11";
      this.isTemporaryGreenRainTree.Value = true;
      this.resetTexture();
    }
    if (this.tapped.Value && this.Location != null)
    {
      StardewValley.Object objectAtTile = this.Location.getObjectAtTile((int) this.Tile.X, (int) this.Tile.Y);
      if (objectAtTile != null && objectAtTile.IsTapper())
        this.UpdateTapperProduct(objectAtTile, onlyPerformRemovals: true);
    }
    this.loadSprite();
    return false;
  }

  public override bool isActionable() => !this.tapped.Value && this.growthStage.Value >= 3;

  public virtual bool IsLeafy()
  {
    WildTreeData data = this.GetData();
    if (data == null || !data.IsLeafy || !data.IsLeafyInWinter && this.Location.IsWinterHere())
      return false;
    return data.IsLeafyInFall || !this.Location.IsFallHere();
  }

  /// <summary>Get the color of the cosmetic wood chips when chopping the tree.</summary>
  public Color? GetChopDebrisColor() => this.GetChopDebrisColor(this.GetData());

  /// <summary>Get the color of the cosmetic wood chips when chopping the tree.</summary>
  /// <param name="data">The wild tree data to read.</param>
  public Color? GetChopDebrisColor(WildTreeData data)
  {
    string debrisColor = data?.DebrisColor;
    if (debrisColor == null)
      return new Color?();
    int result;
    return !int.TryParse(debrisColor, out result) ? Utility.StringToColor(debrisColor) : new Color?(Debris.getColorForDebris(result));
  }

  public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation)
  {
    GameLocation location = this.Location ?? Game1.currentLocation;
    if (explosion > 0)
      this.tapped.Value = false;
    if ((double) this.health.Value <= -99.0)
      return false;
    if (this.growthStage.Value >= 5)
    {
      if (this.hasMoss.Value)
      {
        Item mossItem = Tree.CreateMossItem();
        if (t?.getLastFarmerToUse() != null)
          t.getLastFarmerToUse().gainExperience(2, mossItem.Stack);
        this.hasMoss.Value = false;
        Game1.createMultipleItemDebris(mossItem, new Vector2(tileLocation.X, tileLocation.Y - 1f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32 /*0x20*/);
        int num = (int) Game1.stats.Increment("mossHarvested");
        this.shake(tileLocation, true);
        this.growthStage.Value = 12 - mossItem.Stack;
        Game1.playSound("moss_cut");
        for (int index = 0; index < 6; ++index)
          location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\debris", new Microsoft.Xna.Framework.Rectangle(Game1.random.Choose<int>(16 /*0x10*/, 0), 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), new Vector2((float) ((double) tileLocation.X + Game1.random.NextDouble() - 0.15000000596046448), tileLocation.Y - 1f + (float) Game1.random.NextDouble()) * 64f, false, 0.025f, Color.Green)
          {
            drawAboveAlwaysFront = true,
            motion = new Vector2((float) Game1.random.Next(-10, 11) / 10f, -4f),
            acceleration = new Vector2(0.0f, (float) (0.30000001192092896 + (double) Game1.random.Next(-10, 11) / 200.0)),
            animationLength = 1,
            interval = 1000f,
            sourceRectStartingPos = new Vector2(0.0f, 96f),
            alpha = 1f,
            layerDepth = 1f,
            scale = 4f
          });
      }
      if (this.tapped.Value)
        return false;
      if (t is Axe)
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        this.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
        location.debris.Add(new Debris(12, Game1.random.Next(1, 3), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position, 0, this.GetChopDebrisColor()));
        if (location is Town && (double) tileLocation.X < 100.0 && !this.isTemporaryGreenRainTree.Value)
        {
          switch (location.getTileIndexAt((int) tileLocation.X, (int) tileLocation.Y, "Paths"))
          {
            case 9:
            case 10:
            case 11:
              this.shake(tileLocation, true);
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:TownTreeWarning"));
              return false;
          }
        }
        if (!this.stump.Value && t.getLastFarmerToUse() != null && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < 0.005)
        {
          StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
          if (unseenSecretNote != null)
            Game1.createItemDebris((Item) unseenSecretNote, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32 /*0x20*/);
        }
        else if (!this.stump.Value && t.getLastFarmerToUse() != null && Utility.tryRollMysteryBox(0.005))
          Game1.createItemDebris(ItemRegistry.Create(t.getLastFarmerToUse().stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32 /*0x20*/);
        else if (!this.stump.Value && t.getLastFarmerToUse() != null && t.getLastFarmerToUse().stats.Get("TreesChopped") > 20U && Game1.random.NextDouble() < 0.0003 + (t.getLastFarmerToUse().mailReceived.Contains("GotWoodcuttingBook") ? 0.0007 : (double) t.getLastFarmerToUse().stats.Get("TreesChopped") * 1E-05))
        {
          Game1.createItemDebris(ItemRegistry.Create("(O)Book_Woodcutting"), new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.StandingPixel.Y - 32 /*0x20*/);
          t.getLastFarmerToUse().mailReceived.Add("GotWoodcuttingBook");
        }
        else if (!this.stump.Value)
          Utility.trySpawnRareObject(Game1.player, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, this.Location, 0.33, groundLevel: Game1.player.StandingPixel.Y - 32 /*0x20*/);
      }
      else if (explosion <= 0)
        return false;
      this.shake(tileLocation, true);
      float num1;
      if (explosion > 0)
      {
        num1 = (float) explosion;
        if (location is Town && (double) tileLocation.X < 100.0)
          return false;
      }
      else
      {
        if (t == null)
          return false;
        switch (t.upgradeLevel.Value)
        {
          case 0:
            num1 = 1f;
            break;
          case 1:
            num1 = 1.25f;
            break;
          case 2:
            num1 = 1.67f;
            break;
          case 3:
            num1 = 2.5f;
            break;
          case 4:
            num1 = 5f;
            break;
          default:
            num1 = (float) (t.upgradeLevel.Value + 1);
            break;
        }
      }
      if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num1 / 5.0)
      {
        Debris debris;
        switch (this.treeType.Value)
        {
          case "12":
            debris = new Debris("(O)259", new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
            break;
          case "7":
            debris = new Debris("(O)420", new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
            break;
          case "8":
            debris = new Debris("(O)709", new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
            break;
          default:
            debris = new Debris("388", new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), Game1.player.getStandingPosition());
            break;
        }
        debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
        debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
        location.debris.Add(debris);
      }
      this.health.Value -= num1;
      if ((double) this.health.Value <= 0.0 && this.performTreeFall(t, explosion, tileLocation))
        return true;
    }
    else if (this.growthStage.Value >= 3)
    {
      if (t != null && t.Name.Contains("Ax"))
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        if (this.IsLeafy())
          location.playSound("leafrustle");
        location.debris.Add(new Debris(12, Game1.random.Next(t.upgradeLevel.Value * 2, t.upgradeLevel.Value * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), Utility.PointToVector2(t.getLastFarmerToUse().StandingPixel), 0));
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
          default:
            num = (float) (10 + (t.upgradeLevel.Value - 4));
            break;
        }
      }
      this.health.Value -= num;
      if ((double) this.health.Value <= 0.0)
      {
        this.performBushDestroy(tileLocation);
        return true;
      }
    }
    else if (this.growthStage.Value >= 1)
    {
      if (explosion > 0)
      {
        location.playSound("cut");
        return true;
      }
      if (t != null && t.Name.Contains("Axe"))
      {
        location.playSound("axchop", new Vector2?(tileLocation));
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
      }
      if (t is Axe || t is Pickaxe || t is Hoe || t is MeleeWeapon)
      {
        location.playSound("cut");
        this.performSproutDestroy(t, tileLocation);
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
        this.performSeedDestroy(t, tileLocation);
        return true;
      }
    }
    return false;
  }

  public static Item CreateMossItem()
  {
    return ItemRegistry.Create("(O)Moss", Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (Game1.stats.Get("mossHarvested") * 50U)).Next(1, 3));
  }

  public bool fertilize()
  {
    GameLocation location = this.Location;
    if (this.growthStage.Value >= 5)
    {
      Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:TreeFertilizer1");
      location.playSound("cancel");
      return false;
    }
    if (this.fertilized.Value)
    {
      Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:TreeFertilizer2");
      location.playSound("cancel");
      return false;
    }
    this.fertilized.Value = true;
    location.playSound("dirtyHit");
    return true;
  }

  public bool instantDestroy(Vector2 tileLocation)
  {
    if (this.growthStage.Value >= 5)
      return this.performTreeFall((Tool) null, 0, tileLocation);
    if (this.growthStage.Value >= 3)
    {
      this.performBushDestroy(tileLocation);
      return true;
    }
    if (this.growthStage.Value >= 1)
    {
      this.performSproutDestroy((Tool) null, tileLocation);
      return true;
    }
    this.performSeedDestroy((Tool) null, tileLocation);
    return true;
  }

  protected void performSeedDestroy(Tool t, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
    WildTreeData data = this.GetData();
    if (data == null || data.SeedItemId == null)
      return;
    Farmer farmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
    if (this.lastPlayerToHit.Value != 0L && farmer.getEffectiveSkillLevel(2) >= 1)
    {
      Game1.createMultipleObjectDebris(data.SeedItemId, (int) tileLocation.X, (int) tileLocation.Y, 1, t.getLastFarmerToUse().UniqueMultiplayerID, location);
    }
    else
    {
      if (Game1.player.getEffectiveSkillLevel(2) < 1)
        return;
      Game1.createMultipleObjectDebris(data.SeedItemId, (int) tileLocation.X, (int) tileLocation.Y, 1, t == null ? Game1.player.UniqueMultiplayerID : t.getLastFarmerToUse().UniqueMultiplayerID, location);
    }
  }

  /// <summary>Update the attached tapper's held output.</summary>
  /// <param name="tapper">The attached tapper instance.</param>
  /// <param name="previousOutput">The previous item produced by the tapper, if any.</param>
  public void UpdateTapperProduct(StardewValley.Object tapper, StardewValley.Object previousOutput = null, bool onlyPerformRemovals = false)
  {
    if (tapper == null)
      return;
    WildTreeData data = this.GetData();
    if (data == null)
      return;
    float timeMultiplier = 1f;
    foreach (string contextTag in tapper.GetContextTags())
    {
      float result;
      if (contextTag.StartsWithIgnoreCase("tapper_multiplier_") && float.TryParse(contextTag.Substring("tapper_multiplier_".Length), out result))
      {
        timeMultiplier = 1f / result;
        break;
      }
    }
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, 73137.0, (double) this.Tile.X * 9.0, (double) this.Tile.Y * 13.0);
    StardewValley.Object output;
    int minutesUntilReady;
    if (!this.TryGetTapperOutput(data.TapItems, previousOutput?.ItemId, random, timeMultiplier, out output, out minutesUntilReady) || onlyPerformRemovals && output != null)
      return;
    tapper.heldObject.Value = output;
    tapper.minutesUntilReady.Value = minutesUntilReady;
  }

  /// <summary>Get a valid item that can be produced by the tree's current tapper.</summary>
  /// <param name="tapItems">The tap item data to choose from.</param>
  /// <param name="previousItemId">The previous item ID that was produced.</param>
  /// <param name="r">The RNG with which to randomize.</param>
  /// <param name="timeMultiplier">A multiplier to apply to the minutes until ready.</param>
  /// <param name="output">The possible tapper output.</param>
  /// <param name="minutesUntilReady">The number of minutes until the tapper would produce the output.</param>
  protected bool TryGetTapperOutput(
    List<WildTreeTapItemData> tapItems,
    string previousItemId,
    Random r,
    float timeMultiplier,
    out StardewValley.Object output,
    out int minutesUntilReady)
  {
    if (tapItems != null)
    {
      previousItemId = previousItemId != null ? ItemRegistry.QualifyItemId(previousItemId) : (string) null;
      foreach (WildTreeTapItemData tapItem in tapItems)
      {
        if (GameStateQuery.CheckConditions(tapItem.Condition, this.Location))
        {
          if (tapItem.PreviousItemId != null)
          {
            bool flag = false;
            foreach (string itemId in tapItem.PreviousItemId)
            {
              flag = string.IsNullOrEmpty(itemId) ? previousItemId == null : previousItemId.EqualsIgnoreCase(ItemRegistry.QualifyItemId(itemId));
              if (flag)
                break;
            }
            if (!flag)
              continue;
          }
          Season? season = tapItem.Season;
          if (season.HasValue)
          {
            season = tapItem.Season;
            Season? localSeason = this.localSeason;
            if (!(season.GetValueOrDefault() == localSeason.GetValueOrDefault() & season.HasValue == localSeason.HasValue))
              continue;
          }
          Farmer targetFarmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
          Item drop = this.TryGetDrop((WildTreeItemData) tapItem, r, targetFarmer, "TapItems", (Func<string, string>) (id => id.Replace("PREVIOUS_OUTPUT_ID", previousItemId)));
          if (drop != null)
          {
            if (!(drop is StardewValley.Object @object))
            {
              Game1.log.Warn($"Wild tree '{this.treeType.Value}' can't produce item '{drop.ItemId}': must be an object-type item.");
            }
            else
            {
              int num = (int) Utility.ApplyQuantityModifiers((float) tapItem.DaysUntilReady, (IList<QuantityModifier>) tapItem.DaysUntilReadyModifiers, tapItem.DaysUntilReadyModifierMode, this.Location, Game1.player);
              output = @object;
              minutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int) Math.Max(1.0, Math.Floor((double) num * (double) timeMultiplier)));
              return true;
            }
          }
        }
      }
      if (previousItemId != null)
        return this.TryGetTapperOutput(tapItems, (string) null, r, timeMultiplier, out output, out minutesUntilReady);
    }
    output = (StardewValley.Object) null;
    minutesUntilReady = 0;
    return false;
  }

  protected void performSproutDestroy(Tool t, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
    if (t != null && t.Name.Contains("Axe") && Game1.recentMultiplayerRandom.NextDouble() < (double) t.getLastFarmerToUse().ForagingLevel / 10.0)
      Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, 1);
    Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
  }

  protected void performBushDestroy(Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    WildTreeData data = this.GetData();
    if (data == null)
      return;
    Farmer targetFarmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
    Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(20, 30), false, color: this.GetChopDebrisColor(data));
    if (data.DropWoodOnChop || data.DropHardwoodOnLumberChop)
      Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((targetFarmer.professions.Contains(12) ? 1.25 : 1.0) * 4.0), location);
    List<WildTreeChopItemData> chopItems = data.ChopItems;
    // ISSUE: explicit non-virtual call
    if ((chopItems != null ? (__nonvirtual (chopItems.Count) > 0 ? 1 : 0) : 0) == 0)
      return;
    Random r;
    if (Game1.IsMultiplayer)
    {
      Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 1000.0, (double) tileLocation.Y);
      r = Game1.recentMultiplayerRandom;
    }
    else
      r = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
    foreach (WildTreeItemData chopItem in data.ChopItems)
    {
      Item drop = this.TryGetDrop(chopItem, r, targetFarmer, "ChopItems");
      if (drop != null)
        Game1.createMultipleItemDebris(drop, tileLocation * 64f, -2, location);
    }
  }

  protected bool performTreeFall(Tool t, int explosion, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    WildTreeData data = this.GetData();
    this.Location.objects.Remove(this.Tile);
    this.tapped.Value = false;
    if (!this.stump.Value)
    {
      if (t != null || explosion > 0)
        location.playSound("treecrack");
      this.stump.Value = true;
      this.health.Value = 5f;
      this.falling.Value = true;
      if (t != null && t.getLastFarmerToUse().IsLocalPlayer)
      {
        t?.getLastFarmerToUse().gainExperience(2, 14);
        if (t?.getLastFarmerToUse() == null)
          this.shakeLeft.Value = true;
        else
          this.shakeLeft.Value = (double) t.getLastFarmerToUse().StandingPixel.X > ((double) tileLocation.X + 0.5) * 64.0;
        int num = (int) t.getLastFarmerToUse().stats.Increment("TreesChopped", 1);
      }
    }
    else
    {
      if (t != null && (double) this.health.Value != -100.0 && t.getLastFarmerToUse().IsLocalPlayer && t != null)
        t.getLastFarmerToUse().gainExperience(2, 2);
      this.health.Value = -100f;
      if (data != null)
      {
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(30, 40), false, color: this.GetChopDebrisColor(data));
        Random r;
        if (Game1.IsMultiplayer)
        {
          Game1.recentMultiplayerRandom = Utility.CreateRandom((double) tileLocation.X * 2000.0, (double) tileLocation.Y);
          r = Game1.recentMultiplayerRandom;
        }
        else
          r = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
        if (t?.getLastFarmerToUse() == null)
        {
          if (location.Equals(Game1.currentLocation))
          {
            Game1.createMultipleObjectDebris("(O)92", (int) tileLocation.X, (int) tileLocation.Y, 2, location);
          }
          else
          {
            for (int index = 0; index < 2; ++index)
              Game1.createItemDebris(ItemRegistry.Create("(O)92"), tileLocation * 64f, 2, location);
          }
        }
        else
        {
          Farmer targetFarmer = Game1.GetPlayer(this.lastPlayerToHit.Value) ?? Game1.MasterPlayer;
          if (Game1.IsMultiplayer)
          {
            if (data.DropWoodOnChop)
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((targetFarmer.professions.Contains(12) ? 1.25 : 1.0) * 4.0), true);
            List<WildTreeChopItemData> chopItems = data.ChopItems;
            // ISSUE: explicit non-virtual call
            if ((chopItems != null ? (__nonvirtual (chopItems.Count) > 0 ? 1 : 0) : 0) != 0)
            {
              foreach (WildTreeItemData chopItem in data.ChopItems)
              {
                Item drop = this.TryGetDrop(chopItem, r, targetFarmer, "ChopItems");
                if (drop != null)
                {
                  if (drop.QualifiedItemId == "(O)420" && (double) tileLocation.X % 7.0 == 0.0)
                    drop = ItemRegistry.Create("(O)422", drop.Stack, drop.Quality);
                  Game1.createMultipleItemDebris(drop, tileLocation * 64f, -2, location);
                }
              }
            }
          }
          else
          {
            if (data.DropWoodOnChop)
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((targetFarmer.professions.Contains(12) ? 1.25 : 1.0) * (double) (5 + this.extraWoodCalculator(tileLocation))), true);
            List<WildTreeChopItemData> chopItems = data.ChopItems;
            // ISSUE: explicit non-virtual call
            if ((chopItems != null ? (__nonvirtual (chopItems.Count) > 0 ? 1 : 0) : 0) != 0)
            {
              foreach (WildTreeItemData chopItem in data.ChopItems)
              {
                Item drop = this.TryGetDrop(chopItem, r, targetFarmer, "ChopItems");
                if (drop != null)
                {
                  if (drop.QualifiedItemId == "(O)420" && (double) tileLocation.X % 7.0 == 0.0)
                    drop = ItemRegistry.Create("(O)422", drop.Stack, drop.Quality);
                  Game1.createMultipleItemDebris(drop, tileLocation * 64f, -2, location);
                }
              }
            }
          }
        }
        if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          Game1.createObjectDebris("(O)890", (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64 /*0x40*/, location: location);
        location.playSound("treethud");
      }
      if (!this.falling.Value)
        return true;
    }
    return false;
  }

  /// <summary>Update the tree's season for the location it's planted in.</summary>
  protected void setSeason()
  {
    GameLocation location = this.Location;
    int num;
    switch (location)
    {
      case Desert _:
      case MineShaft _:
        num = 0;
        break;
      default:
        num = (int) Game1.GetSeasonForLocation(location);
        break;
    }
    this.localSeason = new Season?((Season) num);
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 positionOnScreen,
    Vector2 tileLocation,
    float scale,
    float layerDepth)
  {
    layerDepth += positionOnScreen.X / 100000f;
    if (this.growthStage.Value < 5)
    {
      Microsoft.Xna.Framework.Rectangle rectangle;
      switch (this.growthStage.Value)
      {
        case 0:
          rectangle = new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
          break;
        case 1:
          rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
          break;
        case 2:
          rectangle = new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
          break;
        default:
          rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/);
          break;
      }
      spriteBatch.Draw(this.texture.Value, positionOnScreen - new Vector2(0.0f, (float) rectangle.Height * scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + (double) rectangle.Height * (double) scale) / 20000.0));
    }
    else
    {
      if (!this.falling.Value)
        spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(0.0f, -64f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
      if (this.stump.Value && !this.falling.Value)
        return;
      spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(-64f * scale, -320f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 48 /*0x30*/, 96 /*0x60*/)), Color.White, this.shakeRotation, Vector2.Zero, scale, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale) / 20000.0));
    }
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    if (this.isTemporarilyInvisible)
      return;
    Vector2 tile = this.Tile;
    float bottom = (float) this.getBoundingBox().Bottom;
    WildTreeData data;
    if (this.texture.Value == null || !Tree.TryGetData(this.treeType.Value, out data))
    {
      IItemDataDefinition itemDataDefinition = ItemRegistry.RequireTypeDefinition("(O)");
      spriteBatch.Draw(itemDataDefinition.GetErrorTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 3.0 : 0.0)), tile.Y * 64f)), new Microsoft.Xna.Framework.Rectangle?(itemDataDefinition.GetErrorSourceRect()), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) bottom + 1.0) / 10000.0));
    }
    else
    {
      if (this.growthStage.Value < 5)
      {
        Microsoft.Xna.Framework.Rectangle rectangle;
        switch (this.growthStage.Value)
        {
          case 0:
            rectangle = new Microsoft.Xna.Framework.Rectangle(32 /*0x20*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
            break;
          case 1:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
            break;
          case 2:
            rectangle = new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/);
            break;
          default:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96 /*0x60*/, 16 /*0x10*/, 32 /*0x20*/);
            break;
        }
        spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 - (double) (rectangle.Height * 4 - 64 /*0x40*/) + (this.growthStage.Value >= 3 ? 128.0 : 64.0)))), new Microsoft.Xna.Framework.Rectangle?(rectangle), this.fertilized.Value ? Color.HotPink : Color.White, this.shakeRotation, new Vector2(8f, this.growthStage.Value >= 3 ? 32f : 16f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.growthStage.Value == 0 ? 0.0001f : bottom / 10000f);
      }
      else
      {
        if (!this.stump.Value || this.falling.Value)
        {
          if (this.IsLeafy())
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 - 51.0), (float) ((double) tile.Y * 64.0 - 16.0))), new Microsoft.Xna.Framework.Rectangle?(Tree.shadowSourceRect), Color.White * (1.57079637f - Math.Abs(this.shakeRotation)), 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
          else
            spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 - 51.0), (float) ((double) tile.Y * 64.0 - 16.0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(469, 298, 42, 31 /*0x1F*/)), Color.White * (1.57079637f - Math.Abs(this.shakeRotation)), 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
          Microsoft.Xna.Framework.Rectangle treeTopSourceRect = Tree.treeTopSourceRect;
          int num = !data.UseAlternateSpriteWhenSeedReady || !this.hasSeed.Value ? (!data.UseAlternateSpriteWhenNotShaken ? 0 : (!this.wasShakenToday.Value ? 1 : 0)) : 1;
          treeTopSourceRect.X = num == 0 ? 0 : 48 /*0x30*/;
          if (this.hasMoss.Value)
            treeTopSourceRect.X = 96 /*0x60*/;
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + 32.0), (float) ((double) tile.Y * 64.0 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(treeTopSourceRect), Color.White * this.alpha, this.shakeRotation, new Vector2(24f, 96f), 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) bottom + 2.0) / 10000.0 - (double) tile.X / 1000000.0));
        }
        Microsoft.Xna.Framework.Rectangle stumpSourceRect = Tree.stumpSourceRect;
        if (this.hasMoss.Value)
          stumpSourceRect.X += 96 /*0x60*/;
        if ((double) this.health.Value >= 1.0 || !this.falling.Value && (double) this.health.Value > -99.0)
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 3.0 : 0.0)), (float) ((double) tile.Y * 64.0 - 64.0))), new Microsoft.Xna.Framework.Rectangle?(stumpSourceRect), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, bottom / 10000f);
        if (this.stump.Value && (double) this.health.Value < 4.0 && (double) this.health.Value > -99.0)
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tile.X * 64.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 3.0 : 0.0)), tile.Y * 64f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Math.Min(2, (int) (3.0 - (double) this.health.Value)) * 16 /*0x10*/, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) bottom + 1.0) / 10000.0));
      }
      foreach (Leaf leaf in this.leaves)
        spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, leaf.position), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/ + leaf.type % 2 * 8, 112 /*0x70*/ + leaf.type / 2 * 8, 8, 8)), Color.White, leaf.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) bottom / 10000.0 + 0.0099999997764825821));
    }
  }
}
