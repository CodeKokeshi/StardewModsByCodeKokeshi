// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.HoeDirt
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class HoeDirt : TerrainFeature
{
  public const float defaultShakeRate = 0.03926991f;
  public const float maximumShake = 0.3926991f;
  public const float shakeDecayRate = 0.0104719754f;
  public const byte N = 1;
  public const byte E = 2;
  public const byte S = 4;
  public const byte W = 8;
  public const byte Cardinals = 15;
  public static readonly Vector2 N_Offset = new Vector2(0.0f, -1f);
  public static readonly Vector2 E_Offset = new Vector2(1f, 0.0f);
  public static readonly Vector2 S_Offset = new Vector2(0.0f, 1f);
  public static readonly Vector2 W_Offset = new Vector2(-1f, 0.0f);
  public const float paddyGrowBonus = 0.25f;
  public const int dry = 0;
  public const int watered = 1;
  public const int invisible = 2;
  public const string fertilizerLowQualityID = "368";
  public const string fertilizerHighQualityID = "369";
  public const string waterRetentionSoilID = "370";
  public const string waterRetentionSoilQualityID = "371";
  public const string speedGroID = "465";
  public const string superSpeedGroID = "466";
  public const string hyperSpeedGroID = "918";
  public const string fertilizerDeluxeQualityID = "919";
  public const string waterRetentionSoilDeluxeID = "920";
  public const string fertilizerLowQualityQID = "(O)368";
  public const string fertilizerHighQualityQID = "(O)369";
  public const string waterRetentionSoilQID = "(O)370";
  public const string waterRetentionSoilQualityQID = "(O)371";
  public const string speedGroQID = "(O)465";
  public const string superSpeedGroQID = "(O)466";
  public const string hyperSpeedGroQID = "(O)918";
  public const string fertilizerDeluxeQualityQID = "(O)919";
  public const string waterRetentionSoilDeluxeQID = "(O)920";
  public static Texture2D lightTexture;
  public static Texture2D darkTexture;
  public static Texture2D snowTexture;
  private readonly NetRef<Crop> netCrop = new NetRef<Crop>();
  public static Dictionary<byte, int> drawGuide;
  [XmlElement("state")]
  public readonly NetInt state = new NetInt();
  /// <summary>The qualified or unqualified item ID of the fertilizer applied to this dirt, if any.</summary>
  /// <remarks>See also the helper methods like <see cref="M:StardewValley.TerrainFeatures.HoeDirt.HasFertilizer" />, <see cref="M:StardewValley.TerrainFeatures.HoeDirt.CanApplyFertilizer(System.String)" />, <see cref="M:StardewValley.TerrainFeatures.HoeDirt.GetFertilizerSpeedBoost" />, etc.</remarks>
  [XmlElement("fertilizer")]
  public readonly NetString fertilizer = new NetString();
  private bool shakeLeft;
  private float shakeRotation;
  private float maxShake;
  private float shakeRate;
  [XmlElement("c")]
  private readonly NetColor c = new NetColor(Color.White);
  private List<Action<GameLocation, Vector2>> queuedActions = new List<Action<GameLocation, Vector2>>();
  private byte neighborMask;
  private byte wateredNeighborMask;
  [XmlIgnore]
  public NetInt nearWaterForPaddy = new NetInt(-1);
  private byte drawSum;
  private int sourceRectPosition;
  private int wateredRectPosition;
  private Texture2D texture;
  private static readonly HoeDirt.NeighborLoc[] _offsets = new HoeDirt.NeighborLoc[4]
  {
    new HoeDirt.NeighborLoc(HoeDirt.N_Offset, (byte) 1, (byte) 4),
    new HoeDirt.NeighborLoc(HoeDirt.S_Offset, (byte) 4, (byte) 1),
    new HoeDirt.NeighborLoc(HoeDirt.E_Offset, (byte) 2, (byte) 8),
    new HoeDirt.NeighborLoc(HoeDirt.W_Offset, (byte) 8, (byte) 2)
  };
  private List<HoeDirt.Neighbor> _neighbors = new List<HoeDirt.Neighbor>();

  /// <inheritdoc />
  [XmlIgnore]
  public override GameLocation Location
  {
    get => base.Location;
    set
    {
      base.Location = value;
      if (this.netCrop.Value == null)
        return;
      this.netCrop.Value.currentLocation = value;
    }
  }

  /// <inheritdoc />
  public override Vector2 Tile
  {
    get => base.Tile;
    set
    {
      base.Tile = value;
      if (this.netCrop.Value == null)
        return;
      this.netCrop.Value.tilePosition = value;
    }
  }

  public Crop crop
  {
    get => this.netCrop.Value;
    set => this.netCrop.Value = value;
  }

  /// <summary>The pot containing this dirt, if applicable.</summary>
  [XmlIgnore]
  public IndoorPot Pot { get; set; }

  public HoeDirt()
    : base(true)
  {
    this.loadSprite();
    if (HoeDirt.drawGuide == null)
      HoeDirt.populateDrawGuide();
    this.initialize(Game1.currentLocation);
  }

  public HoeDirt(int startingState, GameLocation location = null)
    : this()
  {
    this.state.Value = startingState;
    this.Location = location ?? Game1.currentLocation;
    if (location == null)
      return;
    this.initialize(location);
  }

  public HoeDirt(int startingState, Crop crop)
    : this()
  {
    this.state.Value = startingState;
    this.crop = crop;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.netCrop, "netCrop").AddField((INetSerializable) this.state, "state").AddField((INetSerializable) this.fertilizer, "fertilizer").AddField((INetSerializable) this.c, "c").AddField((INetSerializable) this.nearWaterForPaddy, "nearWaterForPaddy");
    this.state.fieldChangeVisibleEvent += (FieldChange<NetInt, int>) ((x, y, z) => this.OnAdded(this.Location, this.Tile));
    this.netCrop.fieldChangeVisibleEvent += (FieldChange<NetRef<Crop>, Crop>) ((x, y, z) =>
    {
      this.nearWaterForPaddy.Value = -1;
      this.updateNeighbors();
      if (this.netCrop.Value == null)
        return;
      this.netCrop.Value.Dirt = this;
      this.netCrop.Value.currentLocation = this.Location;
      this.netCrop.Value.updateDrawMath(this.Tile);
    });
    this.nearWaterForPaddy.Interpolated(false, false);
    this.netCrop.Interpolated(false, false);
    this.netCrop.OnConflictResolve += (NetRefBase<Crop, NetRef<Crop>>.ConflictResolveEvent) ((rejected, accepted) =>
    {
      if (!Game1.IsMasterGame || rejected == null || rejected.netSeedIndex.Value == null)
        return;
      this.queuedActions.Add((Action<GameLocation, Vector2>) ((gLocation, tileLocation) =>
      {
        Vector2 vector2 = tileLocation * 64f;
        gLocation.debris.Add(new Debris(rejected.netSeedIndex.Value, vector2, vector2));
      }));
      this.NeedsUpdate = true;
    });
  }

  private void initialize(GameLocation location)
  {
    if (location == null)
      location = Game1.currentLocation;
    if (location == null)
      return;
    if (location is MineShaft mineShaft)
    {
      int mineArea = mineShaft.getMineArea();
      if (mineShaft.GetAdditionalDifficulty() > 0)
      {
        if (mineArea != 0 && mineArea != 10)
          return;
        this.c.Value = new Color(80 /*0x50*/, 100, 140) * 0.5f;
      }
      else
      {
        if (mineArea != 80 /*0x50*/)
          return;
        this.c.Value = Color.MediumPurple * 0.4f;
      }
    }
    else if (location.GetSeason() == Season.Fall && location.IsOutdoors && !(location is Beach))
    {
      this.c.Value = new Color(250, 210, 240 /*0xF0*/);
    }
    else
    {
      if (!(location is VolcanoDungeon))
        return;
      this.c.Value = Color.MediumPurple * 0.7f;
    }
  }

  public float getShakeRotation() => this.shakeRotation;

  public float getMaxShake() => this.maxShake;

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X * 64.0), (int) ((double) tile.Y * 64.0), 64 /*0x40*/, 64 /*0x40*/);
  }

  public override void doCollisionAction(
    Rectangle positionOfCollider,
    int speedOfCollision,
    Vector2 tileLocation,
    Character who)
  {
    if (this.crop != null && this.crop.currentPhase.Value != 0 && speedOfCollision > 0 && (double) this.maxShake == 0.0 && positionOfCollider.Intersects(this.getBoundingBox()) && Utility.isOnScreen(Utility.Vector2ToPoint(tileLocation), 64 /*0x40*/, this.Location))
    {
      if (!(who is FarmAnimal))
        Grass.PlayGrassSound();
      this.shake((float) (0.39269909262657166 / (double) Math.Min(1f, 5f / (float) speedOfCollision) - (speedOfCollision > 2 ? (double) this.crop.currentPhase.Value * 3.1415927410125732 / 64.0 : 0.0)), (float) Math.PI / 80f / Math.Min(1f, 5f / (float) speedOfCollision), (double) positionOfCollider.Center.X > (double) tileLocation.X * 64.0 + 32.0);
    }
    if (this.crop == null || this.crop.currentPhase.Value == 0 || !(who is Farmer farmer) || !farmer.running)
      return;
    if (farmer.stats.Get("Book_Grass") > 0U)
      farmer.temporarySpeedBuff = -0.33f;
    else
      farmer.temporarySpeedBuff = -1f;
  }

  public void shake(float shake, float rate, bool left)
  {
    if (this.crop != null)
    {
      this.maxShake = shake * (this.crop.raisedSeeds.Value ? 0.6f : 1.5f);
      this.shakeRate = rate * 0.5f;
      this.shakeRotation = 0.0f;
      this.shakeLeft = left;
    }
    this.NeedsUpdate = true;
  }

  /// <summary>Whether this dirt contains a crop which needs water to grow further. To check whether it is watered, see <see cref="M:StardewValley.TerrainFeatures.HoeDirt.isWatered" />.</summary>
  public bool needsWatering()
  {
    if (this.crop == null || this.readyForHarvest() && !this.crop.RegrowsAfterHarvest())
      return false;
    CropData data = this.crop.GetData();
    return data == null || data.NeedsWatering;
  }

  /// <summary>Whether this dirt is watered.</summary>
  /// <remarks>See also <see cref="M:StardewValley.TerrainFeatures.HoeDirt.needsWatering" />.</remarks>
  public bool isWatered() => this.state.Value == 1;

  public static void populateDrawGuide()
  {
    HoeDirt.drawGuide = new Dictionary<byte, int>()
    {
      [(byte) 0] = 0,
      [(byte) 8] = 15,
      [(byte) 2] = 13,
      [(byte) 1] = 12,
      [(byte) 4] = 4,
      [(byte) 9] = 11,
      [(byte) 3] = 9,
      [(byte) 5] = 8,
      [(byte) 6] = 1,
      [(byte) 12] = 3,
      [(byte) 10] = 14,
      [(byte) 7] = 5,
      [(byte) 15] = 6,
      [(byte) 13] = 7,
      [(byte) 11] = 10,
      [(byte) 14] = 2
    };
  }

  public override void loadSprite()
  {
    if (HoeDirt.lightTexture == null)
    {
      try
      {
        HoeDirt.lightTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirt");
      }
      catch (Exception ex)
      {
      }
    }
    if (HoeDirt.darkTexture == null)
    {
      try
      {
        HoeDirt.darkTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtDark");
      }
      catch (Exception ex)
      {
      }
    }
    if (HoeDirt.snowTexture == null)
    {
      try
      {
        HoeDirt.snowTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtSnow");
      }
      catch (Exception ex)
      {
      }
    }
    this.nearWaterForPaddy.Value = -1;
    this.crop?.updateDrawMath(this.Tile);
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null)
  {
    return this.crop == null || !this.crop.raisedSeeds.Value || c is JunimoHarvester;
  }

  public bool readyForHarvest()
  {
    if (this.crop == null || this.crop.fullyGrown.Value && this.crop.dayOfCurrentPhase.Value > 0 || this.crop.currentPhase.Value < this.crop.phaseDays.Count - 1 || this.crop.dead.Value)
      return false;
    return !this.crop.forageCrop.Value || this.crop.whichForageCrop.Value != "2";
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    if (this.crop == null)
      return false;
    bool flag = this.crop.currentPhase.Value >= this.crop.phaseDays.Count - 1 && (!this.crop.fullyGrown.Value || this.crop.dayOfCurrentPhase.Value <= 0);
    HarvestMethod harvestMethod = this.crop.GetHarvestMethod();
    if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.isScythe() && Game1.player.CurrentTool.ItemId == "66")
      harvestMethod = HarvestMethod.Scythe;
    switch (harvestMethod)
    {
      case HarvestMethod.Grab:
        if (this.crop.harvest((int) tileLocation.X, (int) tileLocation.Y, this))
        {
          GameLocation location = this.Location;
          if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
            Game1.player.team.RequestLimitedNutDrops("IslandFarming", location, (int) tileLocation.X * 64 /*0x40*/, (int) tileLocation.Y * 64 /*0x40*/, 5);
          this.destroyCrop(false);
          return true;
        }
        break;
      case HarvestMethod.Scythe:
        if (this.readyForHarvest())
        {
          Tool currentTool = Game1.player.CurrentTool;
          if ((currentTool != null ? (currentTool.isScythe() ? 1 : 0) : 0) != 0)
          {
            Game1.player.CanMove = false;
            Game1.player.UsingTool = true;
            Game1.player.canReleaseTool = true;
            Game1.player.Halt();
            try
            {
              Game1.player.CurrentTool.beginUsing(Game1.currentLocation, (int) Game1.player.lastClick.X, (int) Game1.player.lastClick.Y, Game1.player);
            }
            catch (Exception ex)
            {
            }
            ((MeleeWeapon) Game1.player.CurrentTool).setFarmerAnimating(Game1.player);
            break;
          }
          if (Game1.didPlayerJustClickAtAll(true))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13915"));
            break;
          }
          break;
        }
        break;
    }
    return flag;
  }

  public bool plant(string itemId, Farmer who, bool isFertilizer)
  {
    GameLocation location = this.Location;
    if (isFertilizer)
    {
      if (!this.CanApplyFertilizer(itemId))
        return false;
      this.fertilizer.Value = ItemRegistry.QualifyItemId(itemId) ?? itemId;
      this.applySpeedIncreases(who);
      location.playSound("dirtyHit");
      return true;
    }
    Season season = location.GetSeason();
    Point point = Utility.Vector2ToPoint(this.Tile);
    itemId = Crop.ResolveSeedId(itemId, location);
    CropData data;
    if (!Crop.TryGetData(itemId, out data) || data.Seasons.Count == 0)
      return false;
    StardewValley.Object @object;
    bool isGardenPot = location.objects.TryGetValue(this.Tile, out @object) && @object is IndoorPot;
    bool flag = isGardenPot && !location.IsOutdoors;
    string deniedMessage;
    if (!who.currentLocation.CheckItemPlantRules(itemId, isGardenPot, flag || ((int) location.GetData()?.CanPlantHere ?? (location.IsFarm ? 1 : 0)) != 0, out deniedMessage))
    {
      if (Game1.didPlayerJustClickAtAll(true))
      {
        if (deniedMessage == null && location.NameOrUniqueName != "Farm")
        {
          Farm farm = Game1.getFarm();
          if (farm.CheckItemPlantRules(itemId, isGardenPot, (bool?) farm.GetData()?.CanPlantHere ?? true, out string _))
            deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919");
        }
        if (deniedMessage == null)
          deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13925");
        Game1.showRedMessage(deniedMessage);
      }
      return false;
    }
    if (!flag && !who.currentLocation.CanPlantSeedsHere(itemId, point.X, point.Y, isGardenPot, out deniedMessage))
    {
      if (Game1.didPlayerJustClickAtAll(true))
      {
        if (deniedMessage == null)
          deniedMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13925");
        Game1.showRedMessage(deniedMessage);
      }
      return false;
    }
    if (!flag && !location.SeedsIgnoreSeasonsHere())
    {
      bool? nullable1 = data.Seasons?.Contains(season);
      if (!nullable1.HasValue || !nullable1.GetValueOrDefault())
      {
        if (Game1.didPlayerJustClickAtAll(true))
        {
          bool? nullable2 = data.Seasons?.Contains(season);
          string path = !nullable2.HasValue || nullable2.GetValueOrDefault() ? "Strings\\StringsFromCSFiles:HoeDirt.cs.13925" : "Strings\\StringsFromCSFiles:HoeDirt.cs.13924";
          Game1.showRedMessage(Game1.content.LoadString(path));
        }
        return false;
      }
    }
    this.crop = new Crop(itemId, point.X, point.Y, this.Location);
    if (this.crop.raisedSeeds.Value)
      location.playSound("stoneStep");
    location.playSound("dirtyHit");
    ++Game1.stats.SeedsSown;
    this.applySpeedIncreases(who);
    this.nearWaterForPaddy.Value = -1;
    if (this.hasPaddyCrop() && this.paddyWaterCheck())
    {
      this.state.Value = 1;
      this.updateNeighbors();
    }
    return true;
  }

  public void applySpeedIncreases(Farmer who)
  {
    if (this.crop == null)
      return;
    bool flag = this.Location != null && this.paddyWaterCheck();
    float fertilizerSpeedBoost = this.GetFertilizerSpeedBoost();
    if ((((double) fertilizerSpeedBoost != 0.0 ? 1 : (who.professions.Contains(5) ? 1 : 0)) | (flag ? 1 : 0)) == 0)
      return;
    this.crop.ResetPhaseDays();
    int num1 = 0;
    for (int index = 0; index < this.crop.phaseDays.Count - 1; ++index)
      num1 += this.crop.phaseDays[index];
    float num2 = fertilizerSpeedBoost;
    if (flag)
      num2 += 0.25f;
    if (who.professions.Contains(5))
      num2 += 0.1f;
    int num3 = (int) Math.Ceiling((double) num1 * (double) num2);
    for (int index1 = 0; num3 > 0 && index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < this.crop.phaseDays.Count; ++index2)
      {
        if ((index2 > 0 || this.crop.phaseDays[index2] > 1) && this.crop.phaseDays[index2] != 99999 && this.crop.phaseDays[index2] > 0)
        {
          this.crop.phaseDays[index2]--;
          --num3;
        }
        if (num3 <= 0)
          break;
      }
    }
  }

  public void destroyCrop(bool showAnimation)
  {
    GameLocation location = this.Location;
    if (this.crop != null & showAnimation && location != null)
    {
      Vector2 tile = this.Tile;
      if (this.crop.currentPhase.Value < 1 && !this.crop.dead.Value)
      {
        Game1.multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite(12, tile * 64f, Color.White));
        location.playSound("dirtyHit", new Vector2?(tile));
      }
      else
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, tile * 64f, this.crop.dead.Value ? new Color(207, 193, 43) : Color.ForestGreen));
    }
    this.crop = (Crop) null;
    this.nearWaterForPaddy.Value = -1;
    if (location == null)
      return;
    this.updateNeighbors();
  }

  public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
  {
    GameLocation location = this.Location;
    switch (t)
    {
      case null:
        if (damage > 0 && this.crop != null)
        {
          if (damage == 50)
          {
            this.crop.Kill();
            goto label_31;
          }
          this.destroyCrop(true);
          goto label_31;
        }
        goto label_31;
      case Hoe _:
        if (this.crop != null && this.crop.hitWithHoe((int) tileLocation.X, (int) tileLocation.Y, location, this))
        {
          if (this.crop.forageCrop.Value && this.crop.whichForageCrop.Value == "2" && t.getLastFarmerToUse() != null)
            t.getLastFarmerToUse().gainExperience(2, 7);
          this.destroyCrop(true);
          break;
        }
        break;
      case Pickaxe _:
        if (this.crop == null)
          return true;
        goto default;
      case WateringCan _:
        if (this.crop == null || !this.crop.forageCrop.Value || this.crop.whichForageCrop.Value != "2")
        {
          this.state.Value = 1;
          break;
        }
        break;
      default:
        if (t.isScythe())
        {
          Crop crop = this.crop;
          if ((crop != null ? (crop.GetHarvestMethod() == HarvestMethod.Scythe ? 1 : 0) : 0) != 0 || this.crop != null && t.ItemId == "66")
          {
            if (this.crop.indexOfHarvest.Value == "771" && t.hasEnchantmentOfType<HaymakerEnchantment>())
            {
              for (int index = 0; index < 2; ++index)
                Game1.createItemDebris(ItemRegistry.Create("(O)771"), new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), -1);
            }
            if (this.crop.harvest((int) tileLocation.X, (int) tileLocation.Y, this, isForcedScytheHarvest: true))
            {
              if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
                Game1.player.team.RequestLimitedNutDrops("IslandFarming", location, (int) tileLocation.X * 64 /*0x40*/, (int) tileLocation.Y * 64 /*0x40*/, 5);
              this.destroyCrop(true);
            }
          }
          if (this.crop != null && this.crop.dead.Value)
            this.destroyCrop(true);
          StardewValley.Object forage;
          if (this.crop == null && t.ItemId == "66" && location.objects.TryGetValue(tileLocation, out forage) && forage.isForage())
          {
            Farmer who = t.getLastFarmerToUse() ?? Game1.player;
            forage.Quality = location.GetHarvestSpawnedObjectQuality(who, forage.isForage(), forage.TileLocation);
            Vector2 pixelOrigin = new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0));
            Game1.createItemDebris((Item) forage, pixelOrigin, -1);
            location.OnHarvestedForage(who, forage);
            location.objects.Remove(tileLocation);
            if (who.professions.Contains(13) && Game1.random.NextDouble() < 0.2)
            {
              StardewValley.Object one = (StardewValley.Object) forage.getOne();
              one.Quality = location.GetHarvestSpawnedObjectQuality(who, one.isForage(), one.TileLocation);
              Game1.createItemDebris((Item) one, pixelOrigin, -1);
              location.OnHarvestedForage(who, one);
              break;
            }
            break;
          }
          break;
        }
        if (t.isHeavyHitter() && !(t is MeleeWeapon) && this.crop != null)
        {
          this.destroyCrop(true);
          break;
        }
        break;
    }
    this.shake((float) Math.PI / 32f, (float) Math.PI / 40f, (double) tileLocation.X * 64.0 < (double) Game1.player.Position.X);
label_31:
    return false;
  }

  public bool canPlantThisSeedHere(string itemId, bool isFertilizer = false)
  {
    if (isFertilizer)
      return this.CanApplyFertilizer(itemId);
    if (this.crop == null)
    {
      Season season = this.Location.GetSeason();
      itemId = Crop.ResolveSeedId(itemId, this.Location);
      CropData data;
      if (Crop.TryGetData(itemId, out data) && data.Seasons.Count != 0)
      {
        if (!Game1.currentLocation.IsOutdoors || Game1.currentLocation.SeedsIgnoreSeasonsHere() || data.Seasons.Contains(season))
          return !data.IsRaised || !Utility.doesRectangleIntersectTile(Game1.player.GetBoundingBox(), (int) this.Tile.X, (int) this.Tile.Y);
        if (itemId == "309" || itemId == "310" || itemId == "311")
          return true;
        if (Game1.didPlayerJustClickAtAll() && !Game1.doesHUDMessageExist(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13924")))
        {
          Game1.playSound("cancel");
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13924"));
        }
      }
    }
    return false;
  }

  public override void performPlayerEntryAction()
  {
    base.performPlayerEntryAction();
    this.crop?.updateDrawMath(this.Tile);
  }

  public override bool tickUpdate(GameTime time)
  {
    foreach (Action<GameLocation, Vector2> queuedAction in this.queuedActions)
      queuedAction(this.Location, this.Tile);
    this.queuedActions.Clear();
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
      this.maxShake = Math.Max(0.0f, this.maxShake - (float) Math.PI / 300f);
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
    return this.state.Value == 2 && this.crop == null;
  }

  /// <summary>Get whether this dirt contains a crop which should be planted near water.</summary>
  public bool hasPaddyCrop() => this.crop != null && this.crop.isPaddyCrop();

  /// <summary>Get whether this is a paddy crop planted near water, so it should be watered automatically.</summary>
  /// <param name="forceUpdate">Whether to recheck the surrounding map area instead of using the cached value.</param>
  public bool paddyWaterCheck(bool forceUpdate = false)
  {
    if (!forceUpdate && this.nearWaterForPaddy.Value >= 0)
      return this.nearWaterForPaddy.Value == 1;
    if (!this.hasPaddyCrop())
    {
      this.nearWaterForPaddy.Value = 0;
      return false;
    }
    Vector2 tile = this.Tile;
    if (this.Location.getObjectAtTile((int) tile.X, (int) tile.Y) is IndoorPot)
    {
      this.nearWaterForPaddy.Value = 0;
      return false;
    }
    int num = 3;
    for (int index1 = -num; index1 <= num; ++index1)
    {
      for (int index2 = -num; index2 <= num; ++index2)
      {
        if (this.Location.isWaterTile((int) ((double) tile.X + (double) index1), (int) ((double) tile.Y + (double) index2)))
        {
          this.nearWaterForPaddy.Value = 1;
          return true;
        }
      }
    }
    this.nearWaterForPaddy.Value = 0;
    return false;
  }

  public override void dayUpdate()
  {
    GameLocation location = this.Location;
    int num = !this.hasPaddyCrop() ? 0 : (this.paddyWaterCheck(true) ? 1 : 0);
    if (num != 0 && this.state.Value == 0)
      this.state.Value = 1;
    if (this.crop != null)
    {
      this.crop.newDay(this.state.Value);
      if (location.isOutdoors.Value && location.GetSeason() == Season.Winter && this.crop != null && !this.crop.isWildSeedCrop() && !this.crop.IsInSeason(location))
        this.destroyCrop(false);
    }
    if (num == 0 && !Game1.random.NextBool(this.GetFertilizerWaterRetentionChance()))
      this.state.Value = 0;
    if (!location.IsGreenhouse)
      return;
    this.c.Value = Color.White;
  }

  /// <inheritdoc />
  public override bool seasonUpdate(bool onLoad)
  {
    GameLocation location = this.Location;
    if (!onLoad && !location.SeedsIgnoreSeasonsHere() && (this.crop == null || this.crop.dead.Value || !this.crop.IsInSeason(location)))
      this.fertilizer.Value = (string) null;
    if (location.GetSeason() == Season.Fall && !location.IsGreenhouse)
      this.c.Value = new Color(250, 210, 240 /*0xF0*/);
    else
      this.c.Value = Color.White;
    this.texture = (Texture2D) null;
    return false;
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 positionOnScreen,
    Vector2 tileLocation,
    float scale,
    float layerDepth)
  {
    byte key1 = 0;
    Vector2 key2 = tileLocation;
    ++key2.X;
    Farm farm = Game1.getFarm();
    TerrainFeature terrainFeature1;
    if (farm.terrainFeatures.TryGetValue(key2, out terrainFeature1) && terrainFeature1 is HoeDirt)
      key1 += (byte) 2;
    key2.X -= 2f;
    TerrainFeature terrainFeature2;
    if (farm.terrainFeatures.TryGetValue(key2, out terrainFeature2) && terrainFeature2 is HoeDirt)
      key1 += (byte) 8;
    ++key2.X;
    ++key2.Y;
    TerrainFeature terrainFeature3;
    if (Game1.currentLocation.terrainFeatures.TryGetValue(key2, out terrainFeature3) && terrainFeature3 is HoeDirt)
      key1 += (byte) 4;
    key2.Y -= 2f;
    TerrainFeature terrainFeature4;
    if (farm.terrainFeatures.TryGetValue(key2, out terrainFeature4) && terrainFeature4 is HoeDirt)
      ++key1;
    int num = HoeDirt.drawGuide[key1];
    spriteBatch.Draw(HoeDirt.lightTexture, positionOnScreen, new Rectangle?(new Rectangle(num % 4 * 64 /*0x40*/, num / 4 * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + positionOnScreen.Y / 20000f);
    this.crop?.drawInMenu(spriteBatch, positionOnScreen + new Vector2(64f * scale, 64f * scale), Color.White, 0.0f, scale, layerDepth + (float) (((double) positionOnScreen.Y + 64.0 * (double) scale) / 20000.0));
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    this.DrawOptimized(spriteBatch, spriteBatch, spriteBatch);
  }

  public void DrawOptimized(SpriteBatch dirt_batch, SpriteBatch fert_batch, SpriteBatch crop_batch)
  {
    int num = this.state.Value;
    Vector2 tile = this.Tile;
    if (num != 2 && (dirt_batch != null || fert_batch != null))
    {
      if (dirt_batch != null && this.texture == null)
      {
        this.texture = Game1.currentLocation.Name.Equals("Mountain") || Game1.currentLocation.Name.Equals("Mine") || Game1.currentLocation is MineShaft currentLocation1 && currentLocation1.shouldShowDarkHoeDirt() || Game1.currentLocation is VolcanoDungeon ? HoeDirt.darkTexture : HoeDirt.lightTexture;
        if (Game1.currentLocation.GetSeason() == Season.Winter && !Game1.currentLocation.SeedsIgnoreSeasonsHere() && !(Game1.currentLocation is MineShaft) || Game1.currentLocation is MineShaft currentLocation2 && currentLocation2.shouldUseSnowTextureHoeDirt())
          this.texture = HoeDirt.snowTexture;
      }
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, tile * 64f);
      if (dirt_batch != null)
      {
        dirt_batch.Draw(this.texture, local, new Rectangle?(new Rectangle(this.sourceRectPosition % 4 * 16 /*0x10*/, this.sourceRectPosition / 4 * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), this.c.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
        if (num == 1)
          dirt_batch.Draw(this.texture, local, new Rectangle?(new Rectangle(this.wateredRectPosition % 4 * 16 /*0x10*/ + (this.paddyWaterCheck() ? 128 /*0x80*/ : 64 /*0x40*/), this.wateredRectPosition / 4 * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), this.c.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1.2E-08f);
      }
      if (fert_batch != null && this.HasFertilizer())
        fert_batch.Draw(Game1.mouseCursors, local, new Rectangle?(this.GetFertilizerSourceRect()), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1.9E-08f);
    }
    if (this.crop == null || crop_batch == null)
      return;
    this.crop.draw(crop_batch, tile, num != 1 || this.crop.currentPhase.Value != 0 || !this.crop.shouldDrawDarkWhenWatered() ? Color.White : new Color(180, 100, 200) * 1f, this.shakeRotation);
  }

  /// <summary>Get whether the dirt has any fertilizer applied.</summary>
  public virtual bool HasFertilizer()
  {
    return this.fertilizer.Value != null && this.fertilizer.Value != "0";
  }

  /// <summary>Get whether a player can apply the given fertilizer to this dirt.</summary>
  /// <param name="fertilizerId">The fertilizer item ID.</param>
  public virtual bool CanApplyFertilizer(string fertilizerId)
  {
    return this.CheckApplyFertilizerRules(fertilizerId) == HoeDirtFertilizerApplyStatus.Okay;
  }

  /// <summary>Get a status which indicates whether fertilizer can be applied to this dirt, and the reason it can't if applicable.</summary>
  /// <param name="fertilizerId">The fertilizer item ID.</param>
  public virtual HoeDirtFertilizerApplyStatus CheckApplyFertilizerRules(string fertilizerId)
  {
    if (this.HasFertilizer())
    {
      fertilizerId = ItemRegistry.QualifyItemId(fertilizerId);
      return !(fertilizerId == ItemRegistry.QualifyItemId(this.fertilizer.Value)) ? HoeDirtFertilizerApplyStatus.HasAnotherFertilizer : HoeDirtFertilizerApplyStatus.HasThisFertilizer;
    }
    return this.crop != null && this.crop.currentPhase.Value != 0 && (fertilizerId == "(O)368" || fertilizerId == "(O)369") ? HoeDirtFertilizerApplyStatus.CropAlreadySprouted : HoeDirtFertilizerApplyStatus.Okay;
  }

  /// <summary>Get the crop growth speed boost from fertilizers applied to this dirt.</summary>
  public virtual float GetFertilizerSpeedBoost()
  {
    switch (this.fertilizer.Value)
    {
      case "465":
      case "(O)465":
        return 0.1f;
      case "466":
      case "(O)466":
        return 0.25f;
      case "918":
      case "(O)918":
        return 0.33f;
      default:
        return 0.0f;
    }
  }

  /// <summary>Get the water retention chance from fertilizers applied to this dirt, as a value between 0 (no change) and 1 (100% chance of staying watered).</summary>
  public virtual float GetFertilizerWaterRetentionChance()
  {
    switch (this.fertilizer.Value)
    {
      case "370":
      case "(O)370":
        return 0.33f;
      case "371":
      case "(O)371":
        return 0.66f;
      case "920":
      case "(O)920":
        return 1f;
      default:
        return 0.0f;
    }
  }

  /// <summary>Get the quality boost level from fertilizers applied to this dirt, which influences the chance of producing a higher-quality crop.</summary>
  /// <remarks>See <see cref="M:StardewValley.Crop.harvest(System.Int32,System.Int32,StardewValley.TerrainFeatures.HoeDirt,StardewValley.Characters.JunimoHarvester,System.Boolean)" /> for the quality boost logic.</remarks>
  public virtual int GetFertilizerQualityBoostLevel()
  {
    switch (this.fertilizer.Value)
    {
      case "368":
      case "(O)368":
        return 1;
      case "369":
      case "(O)369":
        return 2;
      case "919":
      case "(O)919":
        return 3;
      default:
        return 0;
    }
  }

  /// <summary>Get the pixel area within the dirt spritesheet to draw for any fertilizer applied to this dirt.</summary>
  public virtual Rectangle GetFertilizerSourceRect()
  {
    string str = this.fertilizer.Value;
    int num;
    if (str != null)
    {
      switch (str.Length)
      {
        case 3:
          switch (str[2])
          {
            case '0':
              switch (str)
              {
                case "370":
                  goto label_17;
                case "920":
                  goto label_19;
                default:
                  goto label_24;
              }
            case '1':
              if (str == "371")
                goto label_18;
              goto label_24;
            case '5':
              if (str == "465")
                goto label_20;
              goto label_24;
            case '6':
              if (str == "466")
                goto label_21;
              goto label_24;
            case '8':
              if (str == "918")
                goto label_22;
              goto label_24;
            case '9':
              switch (str)
              {
                case "369":
                  break;
                case "919":
                  goto label_23;
                default:
                  goto label_24;
              }
              break;
            default:
              goto label_24;
          }
        case 6:
          switch (str[5])
          {
            case '0':
              switch (str)
              {
                case "(O)370":
                  goto label_17;
                case "(O)920":
                  goto label_19;
                default:
                  goto label_24;
              }
            case '1':
              if (str == "(O)371")
                goto label_18;
              goto label_24;
            case '5':
              if (str == "(O)465")
                goto label_20;
              goto label_24;
            case '6':
              if (str == "(O)466")
                goto label_21;
              goto label_24;
            case '8':
              if (str == "(O)918")
                goto label_22;
              goto label_24;
            case '9':
              switch (str)
              {
                case "(O)369":
                  break;
                case "(O)919":
                  goto label_23;
                default:
                  goto label_24;
              }
              break;
            default:
              goto label_24;
          }
        default:
          goto label_24;
      }
      num = 1;
      goto label_25;
label_17:
      num = 3;
      goto label_25;
label_18:
      num = 4;
      goto label_25;
label_19:
      num = 5;
      goto label_25;
label_20:
      num = 6;
      goto label_25;
label_21:
      num = 7;
      goto label_25;
label_22:
      num = 8;
      goto label_25;
label_23:
      num = 2;
      goto label_25;
    }
label_24:
    num = 0;
label_25:
    return new Rectangle(173 + num / 3 * 16 /*0x10*/, 462 + num % 3 * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
  }

  private List<HoeDirt.Neighbor> gatherNeighbors()
  {
    List<HoeDirt.Neighbor> neighbors = this._neighbors;
    neighbors.Clear();
    if (this.Pot == null)
    {
      GameLocation location = this.Location;
      Vector2 tile = this.Tile;
      NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = location.terrainFeatures;
      foreach (HoeDirt.NeighborLoc offset in HoeDirt._offsets)
      {
        Vector2 key = tile + offset.Offset;
        TerrainFeature terrainFeature;
        if (terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature is HoeDirt a && a.state.Value != 2)
        {
          HoeDirt.Neighbor neighbor = new HoeDirt.Neighbor(a, offset.Direction, offset.InvDirection);
          neighbors.Add(neighbor);
        }
      }
    }
    return neighbors;
  }

  public void updateNeighbors()
  {
    if (this.Location == null)
      return;
    List<HoeDirt.Neighbor> neighborList = this.gatherNeighbors();
    this.neighborMask = (byte) 0;
    this.wateredNeighborMask = (byte) 0;
    foreach (HoeDirt.Neighbor neighbor in neighborList)
    {
      this.neighborMask |= neighbor.direction;
      if (this.state.Value != 2)
        neighbor.feature.OnNeighborAdded(neighbor.invDirection, this.state.Value);
      if (this.isWatered() && neighbor.feature.isWatered())
      {
        if (neighbor.feature.paddyWaterCheck() == this.paddyWaterCheck())
        {
          this.wateredNeighborMask |= neighbor.direction;
          neighbor.feature.wateredNeighborMask |= neighbor.invDirection;
        }
        else
          neighbor.feature.wateredNeighborMask &= ~neighbor.invDirection;
      }
      neighbor.feature.UpdateDrawSums();
    }
    this.UpdateDrawSums();
  }

  public void OnAdded(GameLocation loc, Vector2 tilePos)
  {
    this.Location = loc;
    this.Tile = tilePos;
    this.updateNeighbors();
  }

  public void OnRemoved()
  {
    if (this.Location == null)
      return;
    List<HoeDirt.Neighbor> neighborList = this.gatherNeighbors();
    this.neighborMask = (byte) 0;
    this.wateredNeighborMask = (byte) 0;
    foreach (HoeDirt.Neighbor neighbor in neighborList)
    {
      neighbor.feature.OnNeighborRemoved(neighbor.invDirection);
      if (this.isWatered())
        neighbor.feature.wateredNeighborMask &= ~neighbor.invDirection;
      neighbor.feature.UpdateDrawSums();
    }
    this.UpdateDrawSums();
  }

  public virtual void UpdateDrawSums()
  {
    this.drawSum = (byte) ((uint) this.neighborMask & 15U);
    this.sourceRectPosition = HoeDirt.drawGuide[this.drawSum];
    this.wateredRectPosition = HoeDirt.drawGuide[this.wateredNeighborMask];
  }

  /// <summary>Called when a neighbor is added or changed.</summary>
  /// <param name="direction">The direction from this dirt to the one which changed.</param>
  /// <param name="neighborState">The water state for the neighbor which changed.</param>
  public void OnNeighborAdded(byte direction, int neighborState)
  {
    this.neighborMask |= direction;
    if (neighborState == 1)
      this.wateredNeighborMask |= direction;
    else
      this.wateredNeighborMask &= ~direction;
  }

  /// <summary>Called when a neighbor is removed.</summary>
  /// <param name="direction">The direction from this dirt to the one which was removed.</param>
  public void OnNeighborRemoved(byte direction)
  {
    this.neighborMask &= ~direction;
    this.wateredNeighborMask &= ~direction;
  }

  private struct NeighborLoc(Vector2 a, byte b, byte c)
  {
    public readonly Vector2 Offset = a;
    public readonly byte Direction = b;
    public readonly byte InvDirection = c;
  }

  private struct Neighbor(HoeDirt a, byte b, byte c)
  {
    public readonly HoeDirt feature = a;
    public readonly byte direction = b;
    public readonly byte invDirection = c;
  }
}
