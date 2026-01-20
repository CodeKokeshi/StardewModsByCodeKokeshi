// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.IndoorPot
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class IndoorPot : StardewValley.Object
{
  [XmlElement("hoeDirt")]
  public readonly NetRef<HoeDirt> hoeDirt = new NetRef<HoeDirt>();
  [XmlElement("bush")]
  public readonly NetRef<Bush> bush = new NetRef<Bush>();
  [XmlIgnore]
  public readonly NetBool bushLoadDirty = new NetBool(true);

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  [XmlIgnore]
  public override GameLocation Location
  {
    get => base.Location;
    set
    {
      if (this.hoeDirt.Value != null)
      {
        this.hoeDirt.Value.Location = value;
        this.hoeDirt.Value.Pot = this;
      }
      if (this.bush.Value != null)
        this.bush.Value.Location = value;
      base.Location = value;
    }
  }

  /// <inheritdoc />
  public override Vector2 TileLocation
  {
    get => base.TileLocation;
    set
    {
      if (this.hoeDirt.Value != null)
        this.hoeDirt.Value.Tile = value;
      if (this.bush.Value != null)
        this.bush.Value.Tile = value;
      base.TileLocation = value;
    }
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.hoeDirt, "hoeDirt").AddField((INetSerializable) this.bush, "bush").AddField((INetSerializable) this.bushLoadDirty, "bushLoadDirty");
    this.bush.fieldChangeEvent += (FieldChange<NetRef<Bush>, Bush>) ((field, value, newValue) =>
    {
      if (newValue == null)
        return;
      newValue.Location = this.Location;
      newValue.inPot.Value = true;
    });
  }

  public IndoorPot()
  {
  }

  public IndoorPot(Vector2 tileLocation)
    : base(tileLocation, "62")
  {
    GameLocation currentLocation = Game1.currentLocation;
    this.Location = currentLocation;
    this.hoeDirt.Value = new HoeDirt(0, currentLocation);
    if (!currentLocation.IsRainingHere() || !currentLocation.isOutdoors.Value)
      return;
    this.Water();
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    this.hoeDirt.Value.dayUpdate();
    this.showNextIndex.Value = this.hoeDirt.Value.isWatered();
    GameLocation location = this.Location;
    if (location.isOutdoors.Value && location.IsRainingHere())
      this.Water();
    if (this.heldObject.Value != null)
      this.readyForHarvest.Value = true;
    this.bush.Value?.dayUpdate();
  }

  /// <summary>Water the dirt in this garden pot.</summary>
  public void Water()
  {
    this.hoeDirt.Value.state.Value = 1;
    this.showNextIndex.Value = true;
  }

  /// <summary>Get whether an item type can be planted in indoor pots, regardless of whether the pot has room currently.</summary>
  /// <param name="item">The item to check.</param>
  public bool IsPlantableItem(Item item)
  {
    if (item.HasTypeObject())
    {
      string qualifiedItemId = item.QualifiedItemId;
      if (qualifiedItemId == "(O)499" || qualifiedItemId == "(O)805")
        return false;
      if (item.Category == -19)
        return true;
      string key = Crop.ResolveSeedId(item.ItemId, this.Location);
      if (Game1.cropData.ContainsKey(key) || item is StardewValley.Object @object && @object.IsTeaSapling())
        return true;
    }
    return false;
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    if (who != null && dropInItem != null && this.bush.Value == null)
    {
      if (this.hoeDirt.Value.canPlantThisSeedHere(dropInItem.ItemId, dropInItem.Category == -19))
      {
        if (dropInItem.QualifiedItemId == "(O)805")
        {
          if (!probe)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
          return false;
        }
        return probe || this.hoeDirt.Value.plant(dropInItem.ItemId, who, dropInItem.Category == -19);
      }
      if (this.hoeDirt.Value.crop == null && dropInItem.QualifiedItemId == "(O)251")
      {
        if (!probe)
        {
          NetRef<Bush> bush1 = this.bush;
          Bush bush2 = new Bush(this.tileLocation.Value, 3, this.Location);
          bush2.inPot.Value = true;
          bush1.Value = bush2;
          if (!this.Location.IsOutdoors)
          {
            this.bush.Value.loadSprite();
            Game1.playSound("coin");
          }
        }
        return true;
      }
    }
    return false;
  }

  public override bool performToolAction(Tool t)
  {
    if (t != null)
    {
      this.hoeDirt.Value.performToolAction(t, -1, this.tileLocation.Value);
      if (this.bush.Value != null)
      {
        if (this.bush.Value.performToolAction(t, -1, this.tileLocation.Value))
          this.bush.Value = (Bush) null;
        return false;
      }
    }
    if (this.hoeDirt.Value.isWatered())
      this.Water();
    return base.performToolAction(t);
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (who != null)
    {
      if (justCheckingForActivity)
      {
        if (this.hoeDirt.Value.readyForHarvest() || this.heldObject.Value != null)
          return true;
        return this.bush.Value != null && this.bush.Value.inBloom();
      }
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      if (this.heldObject.Value != null)
      {
        StardewValley.Object forage = this.heldObject.Value;
        int quality = forage.Quality;
        forage.Quality = this.Location.GetHarvestSpawnedObjectQuality(who, forage.isForage(), this.TileLocation);
        int num = who.addItemToInventoryBool((Item) forage) ? 1 : 0;
        if (num != 0)
        {
          this.heldObject.Value = (StardewValley.Object) null;
          this.readyForHarvest.Value = false;
          Game1.playSound("coin");
          this.Location.OnHarvestedForage(who, forage);
          return num != 0;
        }
        this.heldObject.Value.Quality = quality;
        return num != 0;
      }
      bool flag = this.hoeDirt.Value.performUseAction(this.tileLocation.Value);
      if (flag)
        return flag;
      Crop crop = this.hoeDirt.Value.crop;
      if ((crop != null ? (crop.currentPhase.Value > 0 ? 1 : 0) : 0) != 0 && (double) this.hoeDirt.Value.getMaxShake() == 0.0)
      {
        this.hoeDirt.Value.shake((float) Math.PI / 32f, 0.06283186f, Game1.random.NextBool());
        DelayedAction.playSoundAfterDelay("leafrustle", Game1.random.Next(100));
      }
      this.bush.Value?.performUseAction(this.tileLocation.Value);
    }
    return false;
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    base.actionOnPlayerEntry();
    this.hoeDirt.Value?.performPlayerEntryAction();
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    base.updateWhenCurrentLocation(time);
    if (this.Location == null)
      return;
    this.hoeDirt.Value.tickUpdate(time);
    this.bush.Value?.tickUpdate(time);
    if (!this.bushLoadDirty.Value)
      return;
    this.bush.Value?.loadSprite();
    this.bushLoadDirty.Value = false;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    Vector2 vector2 = this.getScale() * 4f;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
    Rectangle destinationRectangle = new Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), destinationRectangle, new Rectangle?(dataOrErrorItem.GetSourceRect(this.showNextIndex.Value ? 1 : 0)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1E-05f);
    if (this.hoeDirt.Value.HasFertilizer())
    {
      Rectangle fertilizerSourceRect = this.hoeDirt.Value.GetFertilizerSourceRect() with
      {
        Width = 13,
        Height = 13
      };
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) this.tileLocation.X * 64.0 + 4.0), (float) ((double) this.tileLocation.Y * 64.0 - 12.0))), new Rectangle?(fertilizerSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y + 0.64999997615814209) * 64.0 / 10000.0 + (double) x * 9.9999997473787516E-06));
    }
    this.hoeDirt.Value.crop?.drawWithOffset(spriteBatch, this.tileLocation.Value, !this.hoeDirt.Value.isWatered() || this.hoeDirt.Value.crop.currentPhase.Value != 0 || this.hoeDirt.Value.crop.raisedSeeds.Value ? Color.White : new Color(180, 100, 200) * 1f, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 8f));
    this.heldObject.Value?.draw(spriteBatch, x * 64 /*0x40*/, y * 64 /*0x40*/ - 48 /*0x30*/, (float) (((double) this.tileLocation.Y + 0.6600000262260437) * 64.0 / 10000.0 + (double) x * 9.9999997473787516E-06), 1f);
    this.bush.Value?.draw(spriteBatch, -24f);
  }
}
