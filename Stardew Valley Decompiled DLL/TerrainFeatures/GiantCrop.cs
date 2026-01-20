// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.GiantCrop
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
using StardewValley.GameData.GiantCrops;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class GiantCrop : ResourceClump
{
  /// <summary>A cache of giant crops by small-crop-ID for <see cref="M:StardewValley.TerrainFeatures.GiantCrop.GetGiantCropsFor(System.String)" />.</summary>
  private static readonly Dictionary<string, List<KeyValuePair<string, GiantCropData>>> CacheByCropId = new Dictionary<string, List<KeyValuePair<string, GiantCropData>>>();
  /// <summary>The <see cref="F:StardewValley.Game1.ticks" /> value when the <see cref="F:StardewValley.TerrainFeatures.GiantCrop.CacheByCropId" /> was last reset.</summary>
  private static int CacheTick;
  /// <summary>The backing field for <see cref="P:StardewValley.TerrainFeatures.GiantCrop.Id" />.</summary>
  [XmlElement("id")]
  public readonly NetString netId = new NetString();

  /// <summary>A unique ID for this giant crop matching its entry in <c>Data/GiantCrops</c>.</summary>
  [XmlIgnore]
  public string Id
  {
    get
    {
      if (this.netId.Value == null)
        this.netId.Value = this.GetIdFromLegacySpriteIndex(this.parentSheetIndex.Value);
      return this.netId.Value;
    }
    set => this.netId.Value = value;
  }

  /// <summary>Construct an empty instance.</summary>
  public GiantCrop()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="id">A unique ID for this giant crop matching its entry in <c>Data/GiantCrops</c>.</param>
  /// <param name="tile">The top-left tile position for the giant crop.</param>
  public GiantCrop(string id, Vector2 tile)
    : this()
  {
    this.Tile = tile;
    this.Id = id;
    GiantCropData data = this.GetData();
    this.width.Value = data != null ? data.TileSize.X : 3;
    this.height.Value = data != null ? data.TileSize.Y : 3;
    this.health.Value = data != null ? (float) data.Health : 3f;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.netId, "netId");
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    GiantCropData data = this.GetData();
    if (data != null)
    {
      Texture2D texture = Game1.content.Load<Texture2D>(data.Texture);
      spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, tile * 64f - new Vector2((double) this.shakeTimer > 0.0 ? (float) (Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 2.0) : 0.0f, 64f)), new Rectangle?(new Rectangle(data.TexturePosition.X, data.TexturePosition.Y, 16 /*0x10*/ * data.TileSize.X, 16 /*0x10*/ * (data.TileSize.Y + 1))), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y + (double) data.TileSize.Y) * 64.0 / 10000.0));
    }
    else
    {
      IItemDataDefinition itemDataDefinition = ItemRegistry.RequireTypeDefinition("(O)");
      spriteBatch.Draw(itemDataDefinition.GetErrorTexture(), Game1.GlobalToLocal(Game1.viewport, tile * 64f - new Vector2((double) this.shakeTimer > 0.0 ? (float) (Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 2.0) : 0.0f, 64f)), new Rectangle?(itemDataDefinition.GetErrorSourceRect()), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) tile.Y + 2.0) * 64.0 / 10000.0));
    }
  }

  public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
  {
    if (!(t is Axe))
      return false;
    GameLocation location = this.Location;
    Farmer targetFarmer = t.getLastFarmerToUse() ?? Game1.player;
    int val2 = t.upgradeLevel.Value / 2 + 1;
    float healthDeducted = Math.Min(this.health.Value, (float) val2);
    GiantCropData data = this.GetData();
    Random random1;
    if (!Game1.IsMultiplayer)
      random1 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed, (double) tileLocation.X * 7.0, (double) tileLocation.Y * 11.0);
    else
      Game1.recentMultiplayerRandom = random1 = Utility.CreateRandom((double) tileLocation.X * 1000.0, (double) tileLocation.Y);
    Random random2 = random1;
    location.playSound("axchop", new Vector2?(tileLocation));
    Game1.createRadialDebris(Game1.currentLocation, 12, (int) tileLocation.X + this.width.Value / 2, (int) tileLocation.Y + this.height.Value / 2, random2.Next(4, 9), false);
    if ((double) this.shakeTimer <= 0.0)
    {
      this.shakeTimer = 100f;
      this.NeedsUpdate = true;
    }
    if (t.hasEnchantmentOfType<ShavingEnchantment>() && random2.NextBool((float) val2 / 5f) && data?.HarvestItems != null)
    {
      foreach (GiantCropHarvestItemData harvestItem in data.HarvestItems)
      {
        Item drop = this.TryGetDrop(harvestItem, random2, targetFarmer, true, healthDeducted);
        if (drop != null)
        {
          if (this.Id.Equals("QiFruit"))
          {
            if (!Game1.player.team.SpecialOrderActive("QiChallenge2"))
              break;
          }
          Debris debris = new Debris(drop, new Vector2((float) (((double) tileLocation.X + (double) (this.width.Value / 2)) * 64.0), (float) (((double) tileLocation.Y + (double) (this.height.Value / 2)) * 64.0)), Game1.player.getStandingPosition());
          debris.Chunks[0].xVelocity.Value += (float) random2.Next(-10, 11) / 10f;
          debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 128.0);
          location.debris.Add(debris);
        }
      }
    }
    this.health.Value -= (float) val2;
    if ((double) this.health.Value > 0.0)
      return false;
    t.getLastFarmerToUse().gainExperience(5, 50 * ((t.getLastFarmerToUse().luckLevel.Value + 1) / 2));
    if (location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()))
    {
      StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
      if (unseenSecretNote != null)
        Game1.createItemDebris((Item) unseenSecretNote, tileLocation * 64f, -1, location);
    }
    if (data?.HarvestItems != null)
    {
      foreach (GiantCropHarvestItemData harvestItem in data.HarvestItems)
      {
        Item drop = this.TryGetDrop(harvestItem, random2, targetFarmer, false, healthDeducted);
        if (drop != null)
        {
          if (this.Id.Equals("QiFruit") && !Game1.player.team.SpecialOrderActive("QiChallenge2"))
          {
            if (!Game1.player.mailReceived.Contains("GiantQiFruitMessage"))
            {
              Game1.player.mailReceived.Add("GiantQiFruitMessage");
              Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\1_6_Strings:GiantQiFruitMessage"), new Color(100, 50, (int) byte.MaxValue));
            }
            Game1.createMultipleItemDebris(ItemRegistry.Create("(O)MysteryBox"), new Vector2((float) ((int) tileLocation.X + this.width.Value / 2), (float) ((int) tileLocation.Y + this.width.Value / 2)) * 64f, -2, location);
          }
          else
          {
            Game1.createMultipleItemDebris(drop, new Vector2((float) ((int) tileLocation.X + this.width.Value / 2), (float) ((int) tileLocation.Y + this.width.Value / 2)) * 64f, -2, location);
            Game1.setRichPresence("giantcrop", (object) drop.Name);
          }
        }
      }
    }
    Game1.createRadialDebris(Game1.currentLocation, 12, (int) tileLocation.X + this.width.Value / 2, (int) tileLocation.Y + this.width.Value / 2, random2.Next(4, 9), false);
    location.playSound("stumpCrack", new Vector2?(tileLocation));
    for (int x = 0; x < this.width.Value; ++x)
    {
      for (int y = 0; y < this.height.Value; ++y)
      {
        float animationInterval = Utility.RandomFloat(80f, 110f);
        if (this.width.Value >= 2 && this.height.Value >= 2 && (x == 0 || x == this.width.Value - 2) && (y == 0 || y == this.height.Value - 2))
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2((float) x + 0.5f, (float) y + 0.5f)) * 64f, Color.White, animationInterval: 70f));
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, (tileLocation + new Vector2((float) x, (float) y)) * 64f, Color.White, animationInterval: animationInterval));
      }
    }
    return true;
  }

  /// <summary>Get the giant crop's data from <c>Data/GiantCrops</c>, if found.</summary>
  public GiantCropData GetData()
  {
    GiantCropData data;
    return !GiantCrop.TryGetData(this.Id, out data) ? (GiantCropData) null : data;
  }

  /// <summary>Try to get a giant crop's data from <c>Data/GiantCrops</c>.</summary>
  /// <param name="id">The giant crop ID (i.e. the key in <c>Data/GiantCrops</c>).</param>
  /// <param name="data">The giant crop data, if found.</param>
  /// <returns>Returns whether the giant crop data was found.</returns>
  public static bool TryGetData(string id, out GiantCropData data)
  {
    if (id != null)
      return DataLoader.GiantCrops(Game1.content).TryGetValue(id, out data);
    data = (GiantCropData) null;
    return false;
  }

  /// <summary>Get the giant crops that can grow from a given crop ID.</summary>
  /// <param name="cropId">The qualified or unqualified item ID for the crop's harvest item.</param>
  public static IReadOnlyList<KeyValuePair<string, GiantCropData>> GetGiantCropsFor(string cropId)
  {
    cropId = ItemRegistry.QualifyItemId(cropId);
    if (cropId != null)
    {
      GiantCrop.RebuildCropIdCacheIfNeeded();
      List<KeyValuePair<string, GiantCropData>> giantCropsFor;
      if (GiantCrop.CacheByCropId.TryGetValue(cropId, out giantCropsFor))
        return (IReadOnlyList<KeyValuePair<string, GiantCropData>>) giantCropsFor;
    }
    return (IReadOnlyList<KeyValuePair<string, GiantCropData>>) LegacyShims.EmptyArray<KeyValuePair<string, GiantCropData>>();
  }

  /// <summary>Rebuild the <see cref="F:StardewValley.TerrainFeatures.GiantCrop.CacheByCropId" /> cache, if it was generated before the current tick.</summary>
  /// <param name="forceRebuild">Whether to force rebuilding the cache even if it was generated in the current tick.</param>
  /// <returns>Returns whether the cache was rebuilt.</returns>
  public static bool RebuildCropIdCacheIfNeeded(bool forceRebuild = false)
  {
    if (!forceRebuild && GiantCrop.CacheTick == Game1.ticks)
      return false;
    GiantCrop.CacheTick = Game1.ticks;
    GiantCrop.CacheByCropId.Clear();
    foreach (KeyValuePair<string, GiantCropData> giantCrop in DataLoader.GiantCrops(Game1.content))
    {
      string key = ItemRegistry.QualifyItemId(giantCrop.Value.FromItemId);
      if (key != null)
      {
        List<KeyValuePair<string, GiantCropData>> keyValuePairList;
        if (!GiantCrop.CacheByCropId.TryGetValue(key, out keyValuePairList))
          GiantCrop.CacheByCropId[key] = keyValuePairList = new List<KeyValuePair<string, GiantCropData>>();
        keyValuePairList.Add(giantCrop);
      }
    }
    return true;
  }

  /// <summary>Get a dropped item if its fields match.</summary>
  /// <param name="drop">The drop data.</param>
  /// <param name="r">The RNG to use for random checks.</param>
  /// <param name="targetFarmer">The player interacting with the giant crop.</param>
  /// <param name="isShaving">Whether the item is being dropped for the Shaving enchantment (true), instead of because the giant crop was broken (false).</param>
  /// <param name="healthDeducted">The health points deducted by the tool hit.</param>
  /// <returns>Returns the produced item (if any), else <c>null</c>.</returns>
  public Item TryGetDrop(
    GiantCropHarvestItemData drop,
    Random r,
    Farmer targetFarmer,
    bool isShaving,
    float healthDeducted)
  {
    if (!r.NextBool(drop.Chance))
      return (Item) null;
    if (drop.Condition != null && !GameStateQuery.CheckConditions(drop.Condition, this.Location, targetFarmer, random: r))
      return (Item) null;
    bool? shavingEnchantment = drop.ForShavingEnchantment;
    if (shavingEnchantment.HasValue)
    {
      shavingEnchantment = drop.ForShavingEnchantment;
      bool flag = isShaving;
      if (!(shavingEnchantment.GetValueOrDefault() == flag & shavingEnchantment.HasValue))
        return (Item) null;
    }
    Item drop1 = ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) drop, new ItemQueryContext(this.Location, targetFarmer, r, $"giant crop {this.Id} > harvest item '{drop.Id}'"), logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Giant crop '{this.Id}' failed parsing item query '{query}' for harvest item '{drop.Id}': {error}")));
    if (isShaving)
      this.AdjustStackSizeWhenShaving(drop1, drop.ScaledMinStackWhenShaving, drop.ScaledMaxStackWhenShaving, healthDeducted, r);
    return drop1;
  }

  /// <summary>Adjust the item's stack size for the scaled min/max values, if set.</summary>
  /// <param name="item">The item whose stack size to adjust.</param>
  /// <param name="min">The minimum stack size to apply, scaled to the <paramref name="healthDeducted" />.</param>
  /// <param name="max">The maximum stack size to apply, scaled to the <paramref name="healthDeducted" />.</param>
  /// <param name="healthDeducted">The health points deducted by the tool hit.</param>
  /// <param name="random">The RNG to use when randomizing the stack size.</param>
  private void AdjustStackSizeWhenShaving(
    Item item,
    int? min,
    int? max,
    float healthDeducted,
    Random random)
  {
    if (item == null || !min.HasValue && !max.HasValue)
      return;
    float? nullable1;
    if (min.HasValue)
    {
      ref int? local = ref min;
      int? nullable2 = min;
      float? nullable3 = nullable2.HasValue ? new float?((float) nullable2.GetValueOrDefault()) : new float?();
      float num1 = healthDeducted;
      nullable1 = nullable3.HasValue ? new float?(nullable3.GetValueOrDefault() * num1) : new float?();
      int num2 = (int) nullable1.Value;
      local = new int?(num2);
    }
    if (max.HasValue)
    {
      ref int? local = ref max;
      int? nullable4 = max;
      float? nullable5;
      if (!nullable4.HasValue)
      {
        nullable1 = new float?();
        nullable5 = nullable1;
      }
      else
        nullable5 = new float?((float) nullable4.GetValueOrDefault());
      float? nullable6 = nullable5;
      float num3 = healthDeducted;
      float? nullable7;
      if (!nullable6.HasValue)
      {
        nullable1 = new float?();
        nullable7 = nullable1;
      }
      else
        nullable7 = new float?(nullable6.GetValueOrDefault() * num3);
      nullable1 = nullable7;
      int num4 = (int) nullable1.Value;
      local = new int?(num4);
    }
    if (min.HasValue && max.HasValue)
    {
      item.Stack = random.Next(min.Value, max.Value + 1);
    }
    else
    {
      int stack1 = item.Stack;
      int? nullable8 = min;
      int valueOrDefault1 = nullable8.GetValueOrDefault();
      if (stack1 < valueOrDefault1 & nullable8.HasValue)
      {
        item.Stack = min.Value;
      }
      else
      {
        int stack2 = item.Stack;
        nullable8 = max;
        int valueOrDefault2 = nullable8.GetValueOrDefault();
        if (!(stack2 > valueOrDefault2 & nullable8.HasValue))
          return;
        item.Stack = max.Value;
      }
    }
  }

  /// <summary>Get the giant crop ID which matches a pre-1.6 parent sheet index.</summary>
  /// <param name="spriteIndex">The parent sheet index.</param>
  private string GetIdFromLegacySpriteIndex(int spriteIndex)
  {
    switch (spriteIndex)
    {
      case 190:
        return "Cauliflower";
      case 254:
        return "Melon";
      case 276:
        return "Pumpkin";
      default:
        return spriteIndex.ToString();
    }
  }
}
