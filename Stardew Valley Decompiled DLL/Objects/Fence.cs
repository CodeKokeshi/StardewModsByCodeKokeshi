// Decompiled with JetBrains decompiler
// Type: StardewValley.Fence
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Fences;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Fence : Object
{
  public const int debrisPieces = 4;
  public static int fencePieceWidth = 16 /*0x10*/;
  public static int fencePieceHeight = 32 /*0x20*/;
  public const int gateClosedPosition = 0;
  public const int gateOpenedPosition = 88;
  public const int sourceRectForSoloGate = 17;
  public const int globalHealthMultiplier = 2;
  public const int N = 1000;
  public const int E = 100;
  public const int S = 500;
  public const int W = 10;
  /// <summary>The unqualified item ID for a wood fence.</summary>
  public const string woodFenceId = "322";
  /// <summary>The unqualified item ID for a stone fence.</summary>
  public const string stoneFenceId = "323";
  /// <summary>The unqualified item ID for an iron fence.</summary>
  public const string ironFenceId = "324";
  /// <summary>The unqualified item ID for a hardwood fence.</summary>
  public const string hardwoodFenceId = "298";
  /// <summary>The unqualified item ID for a fence gate.</summary>
  public const string gateId = "325";
  [XmlIgnore]
  public Lazy<Texture2D> fenceTexture;
  public static Dictionary<int, int> fenceDrawGuide;
  [XmlElement("health")]
  public readonly NetFloat health = new NetFloat();
  [XmlElement("maxHealth")]
  public readonly NetFloat maxHealth = new NetFloat();
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Item.ItemId" /> instead.</summary>
  [XmlElement("whichType")]
  public int? obsolete_whichType;
  [XmlElement("gatePosition")]
  public readonly NetInt gatePosition = new NetInt();
  public int gateMotion;
  [XmlElement("isGate")]
  public readonly NetBool isGate = new NetBool();
  [XmlIgnore]
  public readonly NetBool repairQueued = new NetBool();
  protected static Dictionary<string, FenceData> _FenceLookup;
  protected FenceData _data;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.maxHealth, "maxHealth").AddField((INetSerializable) this.gatePosition, "gatePosition").AddField((INetSerializable) this.isGate, "isGate").AddField((INetSerializable) this.repairQueued, "repairQueued");
    this.itemId.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) => this.OnIdChanged());
    this.isGate.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((field, oldValue, newValue) => this.OnIdChanged());
  }

  public Fence(Vector2 tileLocation, string itemId, bool isGate)
    : base(itemId, 1)
  {
    if (Fence.fenceDrawGuide == null)
      Fence.populateFenceDrawGuide();
    this.Type = "Crafting";
    this.isGate.Value = isGate;
    this.TileLocation = tileLocation;
    this.canBeSetDown.Value = true;
    this.canBeGrabbed.Value = true;
    this.price.Value = 1;
    this.ResetHealth((float) Game1.random.Next(-100, 101) / 100f);
    if (isGate)
      this.health.Value *= 2f;
    this.OnIdChanged();
  }

  public Fence()
    : this(Vector2.Zero, "322", false)
  {
  }

  public virtual void ResetHealth(float amount_adjustment)
  {
    FenceData data = this.GetData();
    float num = data != null ? (float) data.Health : 100f;
    if (this.isGate.Value)
      amount_adjustment = 0.0f;
    this.health.Value = num + amount_adjustment;
    this.health.Value *= 2f;
    this.maxHealth.Value = this.health.Value;
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.obsolete_whichType ?? 1)
    {
      case 2:
        this.ItemId = "323";
        break;
      case 3:
        this.ItemId = "324";
        break;
      case 4:
        this.ItemId = "325";
        break;
      case 5:
        this.ItemId = "298";
        break;
      default:
        this.ItemId = "322";
        break;
    }
    this.obsolete_whichType = new int?();
  }

  /// <summary>Reset the fence data and texture when the item ID changes (e.g. when the save is being loaded).</summary>
  protected virtual void OnIdChanged()
  {
    if (this.fenceTexture == null || this.fenceTexture.IsValueCreated)
      this.fenceTexture = new Lazy<Texture2D>(new Func<Texture2D>(this.loadFenceTexture));
    this._data = (FenceData) null;
  }

  public virtual void repair() => this.ResetHealth((float) Game1.random.Next(-100, 101) / 100f);

  public static void populateFenceDrawGuide()
  {
    Fence.fenceDrawGuide = new Dictionary<int, int>()
    {
      [0] = 5,
      [10] = 9,
      [100] = 10,
      [1000] = 3,
      [500] = 5,
      [1010] = 8,
      [1100] = 6,
      [1500] = 3,
      [600] = 0,
      [510] = 2,
      [110] = 7,
      [1600] = 0,
      [1610] = 4,
      [1510] = 2,
      [1110] = 7,
      [610] = 4
    };
  }

  public virtual void PerformRepairIfNecessary()
  {
    if (!Game1.IsMasterGame || !this.repairQueued.Value)
      return;
    this.ResetHealth(this.GetRepairHealthAdjustment());
    this.repairQueued.Value = false;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    this.PerformRepairIfNecessary();
    int newValue = this.gatePosition.Get() + this.gateMotion;
    if (newValue == 88)
    {
      switch (this.getDrawSum())
      {
        case 10:
        case 100:
        case 110:
        case 500:
        case 1000:
        case 1500:
          break;
        default:
          this.toggleGate(Game1.player, false);
          break;
      }
    }
    this.gatePosition.Set(newValue);
    if (newValue >= 88 || newValue <= 0)
      this.gateMotion = 0;
    this.heldObject.Get()?.updateWhenCurrentLocation(time);
  }

  public static Dictionary<string, FenceData> GetFenceLookup()
  {
    if (Fence._FenceLookup == null)
      Fence._LoadFenceData();
    return Fence._FenceLookup;
  }

  /// <summary>Get the fence's data from <c>Data/Fences</c>, if found.</summary>
  public FenceData GetData()
  {
    if (this._data == null)
      Fence.TryGetData(this.ItemId, out this._data);
    return this._data;
  }

  /// <summary>Try to get a fence's data from <c>Data/Fences</c>.</summary>
  /// <param name="itemId">The fence's unqualified item ID (i.e. the key in <c>Data/Fences</c>).</param>
  /// <param name="data">The fence data, if found.</param>
  /// <returns>Returns whether the fence data was found.</returns>
  public static bool TryGetData(string itemId, out FenceData data)
  {
    if (itemId != null)
      return Fence.GetFenceLookup().TryGetValue(itemId, out data);
    data = (FenceData) null;
    return false;
  }

  protected static void _LoadFenceData() => Fence._FenceLookup = DataLoader.Fences(Game1.content);

  public int getDrawSum()
  {
    GameLocation location = this.Location;
    if (location == null)
      return 0;
    int drawSum = 0;
    Vector2 key = this.tileLocation.Value;
    ++key.X;
    Object object1;
    if (location.objects.TryGetValue(key, out object1) && object1 is Fence fence1 && fence1.countsForDrawing(this.ItemId))
      drawSum += 100;
    key.X -= 2f;
    Object object2;
    if (location.objects.TryGetValue(key, out object2) && object2 is Fence fence2 && fence2.countsForDrawing(this.ItemId))
      drawSum += 10;
    ++key.X;
    ++key.Y;
    Object object3;
    if (location.objects.TryGetValue(key, out object3) && object3 is Fence fence3 && fence3.countsForDrawing(this.ItemId))
      drawSum += 500;
    key.Y -= 2f;
    Object object4;
    if (location.objects.TryGetValue(key, out object4) && object4 is Fence fence4 && fence4.countsForDrawing(this.ItemId))
      drawSum += 1000;
    return drawSum;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (!justCheckingForActivity && who != null)
    {
      Point tilePoint = who.TilePoint;
      Object object1;
      Object object2;
      Object object3;
      Object object4;
      if (location.objects.TryGetValue(new Vector2((float) tilePoint.X, (float) (tilePoint.Y - 1)), out object1) && location.objects.TryGetValue(new Vector2((float) tilePoint.X, (float) (tilePoint.Y + 1)), out object2) && location.objects.TryGetValue(new Vector2((float) (tilePoint.X + 1), (float) tilePoint.Y), out object3) && location.objects.TryGetValue(new Vector2((float) (tilePoint.X - 1), (float) tilePoint.Y), out object4) && !object1.isPassable() && !object2.isPassable() && !object4.isPassable() && !object3.isPassable())
        this.performToolAction((Tool) null);
    }
    if ((double) this.health.Value <= 1.0)
      return false;
    if (this.isGate.Value)
    {
      if (justCheckingForActivity || !this.isGate.Value)
        return true;
      this.toggleGate(who, this.gatePosition.Value == 0);
      return true;
    }
    if (justCheckingForActivity)
      return false;
    foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations(this.tileLocation.Value))
    {
      Object @object;
      if (location.objects.TryGetValue(adjacentTileLocation, out @object) && @object is Fence fence && fence.isGate.Value)
      {
        fence.checkForAction(who, false);
        return true;
      }
    }
    return (double) this.health.Value <= 0.0;
  }

  public virtual void toggleGate(bool open, bool is_toggling_counterpart = false, Farmer who = null)
  {
    if ((double) this.health.Value <= 1.0)
      return;
    GameLocation location = this.Location;
    if (location == null)
      return;
    int drawSum = this.getDrawSum();
    switch (drawSum)
    {
      case 10:
      case 100:
      case 110:
      case 500:
      case 1000:
      case 1500:
        who?.TemporaryPassableTiles.Add(new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
        if (open)
          this.gatePosition.Value = 88;
        else
          this.gatePosition.Value = 0;
        if (!is_toggling_counterpart && location != null)
        {
          location.playSound("doorClose");
          break;
        }
        break;
      default:
        who?.TemporaryPassableTiles.Add(new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/));
        this.gatePosition.Value = 0;
        break;
    }
    if (is_toggling_counterpart)
      return;
    switch (drawSum)
    {
      case 10:
        Vector2 key1 = this.tileLocation.Value + new Vector2(1f, 0.0f);
        Object object1;
        if (!location.objects.TryGetValue(key1, out object1) || !(object1 is Fence fence1) || !fence1.isGate.Value || fence1.getDrawSum() != 100)
          break;
        fence1.toggleGate(this.gatePosition.Value != 0, true, who);
        break;
      case 100:
        Vector2 key2 = this.tileLocation.Value + new Vector2(-1f, 0.0f);
        Object object2;
        if (!location.objects.TryGetValue(key2, out object2) || !(object2 is Fence fence2) || !fence2.isGate.Value || fence2.getDrawSum() != 10)
          break;
        fence2.toggleGate(this.gatePosition.Value != 0, true, who);
        break;
      case 500:
        Vector2 key3 = this.tileLocation.Value + new Vector2(0.0f, -1f);
        Object object3;
        if (!location.objects.TryGetValue(key3, out object3) || !(object3 is Fence fence3) || !fence3.isGate.Value || fence3.getDrawSum() != 1000)
          break;
        fence3.toggleGate(this.gatePosition.Value != 0, true, who);
        break;
      case 1000:
        Vector2 key4 = this.tileLocation.Value + new Vector2(0.0f, 1f);
        Object object4;
        if (!location.objects.TryGetValue(key4, out object4) || !(object4 is Fence fence4) || !fence4.isGate.Value || fence4.getDrawSum() != 500)
          break;
        fence4.toggleGate(this.gatePosition.Value != 0, true, who);
        break;
    }
  }

  public void toggleGate(Farmer who, bool open, bool is_toggling_counterpart = false)
  {
    this.toggleGate(open, is_toggling_counterpart, who);
  }

  public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
  {
    location.debris.Add(new Debris(this.ItemId, origin, destination));
  }

  public override bool performToolAction(Tool t)
  {
    GameLocation location = this.Location;
    if (this.heldObject.Value != null && t != null && !(t is MeleeWeapon) && t.isHeavyHitter())
    {
      Object @object = this.heldObject.Value;
      this.heldObject.Value.performRemoveAction();
      this.heldObject.Value = (Object) null;
      Game1.createItemDebris(@object.getOne(), this.TileLocation * 64f, -1);
      this.playNearbySoundAll("axchop");
    }
    else if (this.isGate.Value && (t is Axe || t is Pickaxe))
    {
      this.playNearbySoundAll("axchop");
      Game1.createObjectDebris("(O)325", (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.player.UniqueMultiplayerID, location);
      location.objects.Remove(this.tileLocation.Value);
      Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, 6, false);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextDouble() < 0.5, animationInterval: 50f));
    }
    else if (!this.isGate.Value && this.IsValidRemovalTool(t))
    {
      FenceData data = this.GetData();
      string audioName = data?.RemovalSound ?? data?.PlacementSound ?? "hammer";
      int debrisType = data != null ? data.RemovalDebrisType : 14;
      this.playNearbySoundAll(audioName);
      location.objects.Remove(this.tileLocation.Value);
      for (int index = 0; index < 4; ++index)
        location.temporarySprites.Add((TemporaryAnimatedSprite) new CosmeticDebris(this.fenceTexture.Value, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 32.0), (float) ((double) this.tileLocation.Y * 64.0 + 32.0)), (float) Game1.random.Next(-5, 5) / 100f, (float) Game1.random.Next(-64, 64 /*0x40*/) / 30f, (float) Game1.random.Next(-800, -100) / 100f, (int) (((double) this.tileLocation.Y + 1.0) * 64.0), new Rectangle(32 /*0x20*/ + Game1.random.Next(2) * 16 /*0x10*/ / 2, 96 /*0x60*/ + Game1.random.Next(2) * 16 /*0x10*/ / 2, 8, 8), Color.White, Game1.soundBank.GetCue("shiny4"), (LightSource) null, 0, 200));
      Game1.createRadialDebris(location, debrisType, (int) this.tileLocation.X, (int) this.tileLocation.Y, 6, false);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: Game1.random.NextBool(), animationInterval: 50f));
      if ((double) this.maxHealth.Value - (double) this.health.Value < 0.5)
        location.debris.Add(new Debris((Item) new Object(this.ItemId, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
    }
    return false;
  }

  /// <summary>Get whether a tool can be used to break this fence.</summary>
  /// <param name="tool">The tool instance to check.</param>
  public virtual bool IsValidRemovalTool(Tool tool)
  {
    if (tool == null)
      return !this.isGate.Value;
    FenceData data = this.GetData();
    List<string> removalToolIds = data?.RemovalToolIds;
    List<string> removalToolTypes = data?.RemovalToolTypes;
    bool flag = true;
    // ISSUE: explicit non-virtual call
    if (removalToolIds != null && __nonvirtual (removalToolIds.Count) > 0)
    {
      flag = false;
      string name = tool.Name;
      foreach (string str in removalToolIds)
      {
        if (name == str)
          return true;
      }
    }
    // ISSUE: explicit non-virtual call
    if (removalToolTypes != null && __nonvirtual (removalToolTypes.Count) > 0)
    {
      flag = false;
      string fullName = tool.GetType().FullName;
      foreach (string str in removalToolTypes)
      {
        if (fullName == str)
          return true;
      }
    }
    return flag;
  }

  /// <inheritdoc />
  public override bool minutesElapsed(int minutes)
  {
    if (!Game1.IsMasterGame)
      return false;
    this.PerformRepairIfNecessary();
    if (!Game1.IsBuildingConstructed("Gold Clock") || Game1.netWorldState.Value.goldenClocksTurnedOff.Value)
    {
      this.health.Value -= (float) minutes / 1440f;
      if ((double) this.health.Value <= -1.0 && (Game1.timeOfDay <= 610 || Game1.timeOfDay > 1800))
        return true;
    }
    return false;
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    base.actionOnPlayerEntry();
    if (this.heldObject.Value == null)
      return;
    this.heldObject.Value.TileLocation = this.tileLocation.Value;
    this.heldObject.Value.Location = this.Location;
    this.heldObject.Value.actionOnPlayerEntry();
    this.heldObject.Value.isOn.Value = true;
    this.heldObject.Value.initializeLightSource(this.tileLocation.Value);
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (dropInItem.HasTypeObject() && dropInItem.ItemId == "325")
    {
      if (probe)
        return false;
      if (!this.isGate.Value)
      {
        int drawSum = this.getDrawSum();
        switch (drawSum)
        {
          case 10:
          case 100:
          case 110:
          case 500:
          case 1000:
          case 1500:
            Vector2 key1 = new Vector2();
            if (drawSum <= 100)
            {
              if (drawSum != 10)
              {
                if (drawSum == 100)
                {
                  key1 = this.tileLocation.Value + new Vector2(-1f, 0.0f);
                  if (location.objects.GetValueOrDefault(key1) is Fence valueOrDefault && valueOrDefault.isGate.Value)
                  {
                    switch (valueOrDefault.getDrawSum())
                    {
                      case 10:
                      case 110:
                        break;
                      default:
                        return false;
                    }
                  }
                }
              }
              else
              {
                key1 = this.tileLocation.Value + new Vector2(1f, 0.0f);
                if (location.objects.GetValueOrDefault(key1) is Fence valueOrDefault && valueOrDefault.isGate.Value)
                {
                  switch (valueOrDefault.getDrawSum())
                  {
                    case 100:
                    case 110:
                      break;
                    default:
                      return false;
                  }
                }
              }
            }
            else if (drawSum != 500)
            {
              if (drawSum == 1000)
              {
                key1 = this.tileLocation.Value + new Vector2(0.0f, 1f);
                if (location.objects.GetValueOrDefault(key1) is Fence valueOrDefault && valueOrDefault.isGate.Value)
                {
                  switch (valueOrDefault.getDrawSum())
                  {
                    case 500:
                    case 1500:
                      break;
                    default:
                      return false;
                  }
                }
              }
            }
            else
            {
              key1 = this.tileLocation.Value + new Vector2(0.0f, -1f);
              if (location.objects.GetValueOrDefault(key1) is Fence valueOrDefault && valueOrDefault.isGate.Value)
              {
                switch (valueOrDefault.getDrawSum())
                {
                  case 1000:
                  case 1500:
                    break;
                  default:
                    return false;
                }
              }
            }
            Vector2[] vector2Array = new Vector2[4]
            {
              this.tileLocation.Value + new Vector2(1f, 0.0f),
              this.tileLocation.Value + new Vector2(-1f, 0.0f),
              this.tileLocation.Value + new Vector2(0.0f, -1f),
              this.tileLocation.Value + new Vector2(0.0f, 1f)
            };
            foreach (Vector2 key2 in vector2Array)
            {
              Object @object;
              if (!(key2 == key1) && location.objects.TryGetValue(key2, out @object) && @object is Fence fence && fence.isGate.Value && fence.Type == this.Type)
                return false;
            }
            if (this.heldObject.Value != null)
            {
              Object @object = this.heldObject.Value;
              this.heldObject.Value.performRemoveAction();
              this.heldObject.Value = (Object) null;
              Game1.createItemDebris(@object.getOne(), this.TileLocation * 64f, -1);
            }
            this.isGate.Value = true;
            FenceData data;
            if (Fence.TryGetData("325", out data))
              location.playSound(data.PlacementSound);
            return true;
        }
      }
    }
    else if (dropInItem.QualifiedItemId == "(O)93" && this.heldObject.Value == null && !this.isGate.Value)
    {
      if (!probe)
      {
        this.heldObject.Value = (Object) new Torch();
        location.playSound("axe");
        this.heldObject.Value.Location = this.Location;
        this.heldObject.Value.initializeLightSource(this.tileLocation.Value);
      }
      return true;
    }
    if ((double) this.health.Value > 1.0 || this.repairQueued.Value || !this.CanRepairWithThisItem(dropInItem))
      return base.performObjectDropInAction(dropInItem, probe, who, returnFalseIfItemConsumed);
    if (!probe)
    {
      string repairSound = this.GetRepairSound();
      if (!string.IsNullOrEmpty(repairSound))
        location.playSound(repairSound);
      this.repairQueued.Value = true;
    }
    return true;
  }

  public virtual float GetRepairHealthAdjustment()
  {
    FenceData data = this.GetData();
    return data == null ? 0.0f : Utility.RandomFloat(data.RepairHealthAdjustmentMinimum, data.RepairHealthAdjustmentMaximum);
  }

  public virtual string GetRepairSound() => this.GetData()?.PlacementSound ?? "";

  public virtual bool CanRepairWithThisItem(Item item)
  {
    return (double) this.health.Value <= 1.0 && item != null && item.QualifiedItemId == this.QualifiedItemId;
  }

  /// <inheritdoc />
  public override bool performDropDownAction(Farmer who) => false;

  public virtual Texture2D loadFenceTexture()
  {
    if (this.ItemId == "325")
      this.isGate.Value = true;
    FenceData data = this.GetData();
    return data == null ? ItemRegistry.RequireTypeDefinition(this.TypeDefinitionId).GetErrorTexture() : Game1.content.Load<Texture2D>(data.Texture);
  }

  public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
    spriteBatch.Draw(this.fenceTexture.Value, objectPosition - new Vector2(0.0f, 64f), new Rectangle?(new Rectangle(5 * Fence.fencePieceWidth % this.fenceTexture.Value.Bounds.Width, 5 * Fence.fencePieceWidth / this.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (f.StandingPixel.Y + 1) / 10000f);
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scale,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    location.Y -= 64f * scale;
    int drawSum = this.getDrawSum();
    int tilePosition = Fence.fenceDrawGuide[drawSum];
    if (this.isGate.Value)
    {
      if (drawSum != 110)
      {
        if (drawSum == 1500)
        {
          spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(6f, 6f), new Rectangle?(new Rectangle(112 /*0x70*/, 512 /*0x0200*/, 16 /*0x10*/, 64 /*0x40*/)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
          return;
        }
      }
      else
      {
        spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(6f, 6f), new Rectangle?(new Rectangle(0, 512 /*0x0200*/, 88, 24)), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        return;
      }
    }
    spriteBatch.Draw(this.fenceTexture.Value, location + new Vector2(32f, 32f) * scale, new Rectangle?(Game1.getArbitrarySourceRect(this.fenceTexture.Value, 64 /*0x40*/, 128 /*0x80*/, tilePosition)), color * transparency, 0.0f, new Vector2(32f, 32f) * scale, scale, SpriteEffects.None, layerDepth);
  }

  public bool countsForDrawing(string otherItemId)
  {
    if ((double) this.health.Value <= 1.0 && !this.repairQueued.Value || this.isGate.Value)
      return false;
    return otherItemId == this.ItemId || otherItemId == "325";
  }

  public override bool isPassable() => this.isGate.Value && this.gatePosition.Value >= 88;

  public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
  {
    int num = 1;
    FenceData data = this.GetData();
    if (data == null)
    {
      IItemDataDefinition itemDataDefinition = ItemRegistry.RequireTypeDefinition(this.TypeDefinitionId);
      b.Draw(itemDataDefinition.GetErrorTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f)), new Rectangle?(itemDataDefinition.GetErrorSourceRect()), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-09f);
    }
    else
    {
      if ((double) this.health.Value > 1.0 || this.repairQueued.Value)
      {
        int drawSum = this.getDrawSum();
        num = Fence.fenceDrawGuide[drawSum];
        if (this.isGate.Value)
        {
          Vector2 vector2 = new Vector2(0.0f, 0.0f);
          switch (drawSum)
          {
            case 10:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ - 16 /*0x10*/), (float) (y * 64 /*0x40*/ - 128 /*0x80*/))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 24 : 0, 192 /*0xC0*/, 24, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + 1) / 10000f);
              return;
            case 100:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ - 16 /*0x10*/), (float) (y * 64 /*0x40*/ - 128 /*0x80*/))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 24 : 0, 240 /*0xF0*/, 24, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + 1) / 10000f);
              return;
            case 110:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ - 16 /*0x10*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 24 : 0, 128 /*0x80*/, 24, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 32 /*0x20*/ + 1) / 10000f);
              return;
            case 500:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ + 20), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 20))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 24 : 0, 320, 24, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 96 /*0x60*/ - 1) / 10000f);
              return;
            case 1000:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ + 20), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 20))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 24 : 0, 288, 24, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ - 32 /*0x20*/ + 2) / 10000f);
              return;
            case 1500:
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ + 20), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 20))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 16 /*0x10*/ : 0, 160 /*0xA0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ - 32 /*0x20*/ + 2) / 10000f);
              b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, vector2 + new Vector2((float) (x * 64 /*0x40*/ + 20), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 44))), new Rectangle?(new Rectangle(this.gatePosition.Value == 88 ? 16 /*0x10*/ : 0, 176 /*0xB0*/, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 96 /*0x60*/ - 1) / 10000f);
              return;
            default:
              num = 17;
              break;
          }
        }
        else if (this.heldObject.Value != null)
        {
          Vector2 vector2_1 = Vector2.Zero + data.HeldObjectDrawOffset;
          switch (drawSum)
          {
            case 10:
              vector2_1.X = data.RightEndHeldObjectDrawX;
              break;
            case 100:
              vector2_1.X = data.LeftEndHeldObjectDrawX;
              break;
          }
          Vector2 vector2_2 = vector2_1 * 4f;
          this.heldObject.Value.draw(b, x * 64 /*0x40*/ + (int) vector2_2.X, y * 64 /*0x40*/ + (int) vector2_2.Y, (float) (y * 64 /*0x40*/ + 64 /*0x40*/) / 10000f, 1f);
        }
      }
      b.Draw(this.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/))), new Rectangle?(new Rectangle(num * Fence.fencePieceWidth % this.fenceTexture.Value.Bounds.Width, num * Fence.fencePieceWidth / this.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (y * 64 /*0x40*/ + 32 /*0x20*/) / 10000f);
    }
  }
}
