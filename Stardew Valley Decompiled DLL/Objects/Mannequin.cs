// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Mannequin
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Delegates;
using StardewValley.GameData;
using StardewValley.Internal;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Objects;

/// <summary>A mannequin which can be placed in the world and used to store and display clothing.</summary>
public class Mannequin : StardewValley.Object
{
  protected string _description;
  protected MannequinData _data;
  public string displayNameOverride;
  public readonly NetMutex changeMutex = new NetMutex();
  public readonly NetRef<Hat> hat = new NetRef<Hat>();
  public readonly NetRef<Clothing> shirt = new NetRef<Clothing>();
  public readonly NetRef<Clothing> pants = new NetRef<Clothing>();
  public readonly NetRef<Boots> boots = new NetRef<Boots>();
  public readonly NetDirection facing = new NetDirection();
  public readonly NetBool swappedWithFarmerTonight = new NetBool();
  private Farmer renderCache;
  internal int eyeTimer;

  public Mannequin()
  {
  }

  public Mannequin(string itemId)
    : this()
  {
    this.ItemId = itemId;
    this.name = itemId;
    this.ParentSheetIndex = ItemRegistry.GetDataOrErrorItem(itemId).SpriteIndex;
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
    this.setIndoors.Value = true;
    this.setOutdoors.Value = true;
    this.Type = "interactive";
    this.facing.Value = 2;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.changeMutex.NetFields, "changeMutex.NetFields").AddField((INetSerializable) this.hat, "hat").AddField((INetSerializable) this.shirt, "shirt").AddField((INetSerializable) this.pants, "pants").AddField((INetSerializable) this.boots, "boots").AddField((INetSerializable) this.facing, "facing").AddField((INetSerializable) this.swappedWithFarmerTonight, "swappedWithFarmerTonight");
    this.hat.fieldChangeVisibleEvent += new FieldChange<NetRef<Hat>, Hat>(this.OnMannequinUpdated<NetRef<Hat>, Hat>);
    this.shirt.fieldChangeVisibleEvent += new FieldChange<NetRef<Clothing>, Clothing>(this.OnMannequinUpdated<NetRef<Clothing>, Clothing>);
    this.pants.fieldChangeVisibleEvent += new FieldChange<NetRef<Clothing>, Clothing>(this.OnMannequinUpdated<NetRef<Clothing>, Clothing>);
    this.boots.fieldChangeVisibleEvent += new FieldChange<NetRef<Boots>, Boots>(this.OnMannequinUpdated<NetRef<Boots>, Boots>);
  }

  private void OnMannequinUpdated<TNetField, TValue>(
    TNetField field,
    TValue oldValue,
    TValue newValue)
  {
    this.renderCache = (Farmer) null;
  }

  protected internal MannequinData GetMannequinData()
  {
    if (this._data == null)
      this._data = DataLoader.Mannequins(Game1.content).GetValueOrDefault<string, MannequinData>(this.ItemId);
    return this._data;
  }

  protected override string loadDisplayName()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.ItemId);
    return this.displayNameOverride == null ? dataOrErrorItem.DisplayName : this.displayNameOverride;
  }

  public override string TypeDefinitionId { get; } = "(M)";

  public override string getDescription()
  {
    if (this._description == null)
      this._description = Game1.parseText(TokenParser.ParseText(ItemRegistry.GetDataOrErrorItem(this.ItemId).Description), Game1.smallFont, this.getDescriptionWidth());
    return this._description;
  }

  public override bool isPlaceable() => true;

  /// <inheritdoc />
  public override bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return base.ForEachItem(handler, getPath) && ForEachItemHelper.ApplyToField<Hat>(this.hat, handler, getPath) && ForEachItemHelper.ApplyToField<Clothing>(this.shirt, handler, getPath) && ForEachItemHelper.ApplyToField<Clothing>(this.pants, handler, getPath) && ForEachItemHelper.ApplyToField<Boots>(this.boots, handler, getPath);
  }

  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    Vector2 key = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    Mannequin one = this.getOne() as Mannequin;
    location.Objects.Add(key, (StardewValley.Object) one);
    location.playSound("woodyStep");
    return true;
  }

  private void emitGhost()
  {
    this.Location.temporarySprites.Add(new TemporaryAnimatedSprite(this.GetMannequinData().Texture, new Rectangle(Game1.random.NextDouble() < 0.5 ? 0 : 64 /*0x40*/, 64 /*0x40*/, 16 /*0x10*/, 32 /*0x20*/), this.TileLocation * 64f + new Vector2(0.0f, -1f) * 64f, false, 0.004f, Color.White)
    {
      scale = 4f,
      layerDepth = 1f,
      motion = new Vector2((float) (7 + Game1.random.Next(-1, 6)), (float) (Game1.random.Next(-1, 5) - 8)),
      acceleration = new Vector2((float) ((double) Game1.random.Next(10) / 100.0 - 0.40000000596046448), 0.0f),
      animationLength = 4,
      totalNumberOfLoops = 99,
      interval = 80f,
      scaleChangeChange = 0.01f
    });
    this.Location.playSound("cursed_mannequin");
  }

  /// <inheritdoc />
  public override bool minutesElapsed(int minutes)
  {
    if (Game1.random.NextDouble() < 0.001 && this.GetMannequinData().Cursed)
    {
      if (Game1.timeOfDay > Game1.getTrulyDarkTime(this.Location) && Game1.random.NextDouble() < 0.1)
        this.emitGhost();
      else if (Game1.random.NextDouble() < 0.66)
      {
        if (Game1.random.NextDouble() < 0.5)
        {
          foreach (Character farmer in this.Location.farmers)
          {
            this.facing.Value = Utility.GetOppositeFacingDirection(Utility.getDirectionFromChange(this.TileLocation, farmer.Tile));
            this.renderCache = (Farmer) null;
          }
        }
        else
          this.eyeTimer = 2500;
      }
      else
      {
        this.Location.playSound("cursed_mannequin");
        this.shakeTimer = Game1.random.Next(500, 4000);
      }
    }
    return base.minutesElapsed(minutes);
  }

  public override void actionOnPlayerEntry()
  {
    if (Game1.random.NextDouble() < 0.001 && this.GetMannequinData().Cursed)
      this.shakeTimer = Game1.random.Next(500, 1000);
    base.actionOnPlayerEntry();
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    if (Game1.IsMasterGame && this.GetMannequinData().Cursed && this.Location != null && (this.Location is FarmHouse || this.Location is IslandFarmHouse || this.Location is Shed))
    {
      if (Game1.random.NextDouble() < 0.05)
      {
        Vector2 oldTile = this.TileLocation;
        Utility.spawnObjectAround(this.TileLocation, (StardewValley.Object) this, this.Location, false, (Action<StardewValley.Object>) (x =>
        {
          if (this.TileLocation.Equals(oldTile))
            return;
          this.Location.objects.Remove(oldTile);
        }));
      }
      else if (this.swappedWithFarmerTonight.Value)
        this.swappedWithFarmerTonight.Value = false;
      else if (Game1.random.NextDouble() < 0.005)
      {
        if (this.Location.farmers.Count <= 0)
          return;
        using (FarmerCollection.Enumerator enumerator = this.Location.farmers.GetEnumerator())
        {
          if (!enumerator.MoveNext())
            return;
          Farmer current = enumerator.Current;
          Vector2 oldTile = this.TileLocation;
          Vector2 tileLocation = current.mostRecentBed / 64f;
          tileLocation.X = (float) (int) tileLocation.X;
          tileLocation.Y = (float) (int) tileLocation.Y;
          if (!Utility.spawnObjectAround(tileLocation, (StardewValley.Object) this, this.Location, false, (Action<StardewValley.Object>) (x =>
          {
            if (this.TileLocation.Equals(oldTile))
              return;
            this.Location.objects.Remove(oldTile);
          })))
            return;
          this.facing.Value = Utility.GetOppositeFacingDirection(Utility.getDirectionFromChange(this.TileLocation, current.Tile));
          this.renderCache = (Farmer) null;
          this.eyeTimer = 2000;
        }
      }
      else if (Game1.random.NextDouble() < 0.001)
      {
        DecoratableLocation location = this.Location as DecoratableLocation;
        string floorId = location.GetFloorID((int) this.TileLocation.X, (int) this.TileLocation.Y);
        string which_room = (string) null;
        for (int y = (int) this.TileLocation.Y; y > 0; --y)
        {
          which_room = location.GetWallpaperID((int) this.TileLocation.X, y);
          if (which_room != null)
            break;
        }
        if (floorId != null)
          location.SetFloor("MoreFloors:6", floorId);
        if (which_room != null)
          location.SetWallpaper("MoreWalls:21", which_room);
        this.shakeTimer = 10000;
      }
      else
      {
        if (Game1.random.NextDouble() >= 0.02)
          return;
        DecoratableLocation location = this.Location as DecoratableLocation;
        if (Game1.random.NextDouble() < 0.33)
        {
          for (int index = 0; index < 30; ++index)
          {
            int x = Game1.random.Next(2, this.Location.Map.Layers[0].LayerWidth - 2);
            for (int y = 1; y < this.Location.Map.Layers[0].LayerHeight; ++y)
            {
              Vector2 vector2 = new Vector2((float) x, (float) y);
              if (this.Location.isTileLocationOpen(vector2) && this.Location.isTilePlaceable(vector2) && !location.isTileOnWall(x, y) && !this.Location.IsTileOccupiedBy(vector2))
              {
                this.facing.Value = 2;
                this.renderCache = (Farmer) null;
                this.Location.objects.Remove(this.TileLocation);
                this.TileLocation = vector2;
                this.Location.objects.Add(this.TileLocation, (StardewValley.Object) this);
                return;
              }
            }
          }
        }
        else
        {
          int num1;
          int num2;
          int num3;
          if (Game1.random.NextDouble() < 0.5)
          {
            num1 = 1;
            num2 = this.Location.Map.Layers[0].LayerWidth - 1;
            num3 = 1;
          }
          else
          {
            num1 = this.Location.Map.Layers[0].LayerWidth - 1;
            num2 = 1;
            num3 = -1;
          }
          for (int index = 0; index < 30; ++index)
          {
            int y = Game1.random.Next(2, this.Location.Map.Layers[0].LayerHeight - 2);
            for (int x = num1; x != num2; x += num3)
            {
              Vector2 vector2 = new Vector2((float) x, (float) y);
              if (this.Location.isTileLocationOpen(vector2) && this.Location.isTilePlaceable(vector2) && !location.isTileOnWall(x, y) && !this.Location.IsTileOccupiedBy(vector2))
              {
                this.facing.Value = num3 == 1 ? 1 : 3;
                this.renderCache = (Farmer) null;
                this.Location.objects.Remove(this.TileLocation);
                this.TileLocation = vector2;
                this.Location.objects.Add(this.TileLocation, (StardewValley.Object) this);
                return;
              }
            }
          }
        }
      }
    }
    else
    {
      if (!Game1.IsMasterGame || !(this.Location is SeedShop) || (double) this.TileLocation.X <= 33.0 || (double) this.TileLocation.Y <= 14.0)
        return;
      if (this.ItemId.Equals("CursedMannequinMale"))
        this.ItemId = "MannequinMale";
      else if (this.ItemId.Equals("CursedMannequinFemale"))
        this.ItemId = "MannequinFemale";
      this.ResetParentSheetIndex();
      this.renderCache = (Farmer) null;
      this._data = (MannequinData) null;
    }
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    base.updateWhenCurrentLocation(time);
    this.changeMutex.Update(this.Location);
    if (this.eyeTimer <= 0)
      return;
    this.eyeTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
  }

  public override bool performToolAction(Tool t)
  {
    if (t == null || t is MeleeWeapon || !t.isHeavyHitter())
      return false;
    if (this.hat.Value != null || this.shirt.Value != null || this.pants.Value != null || this.boots.Value != null)
    {
      if (this.hat.Value != null)
      {
        this.DropItem(Utility.PerformSpecialItemGrabReplacement((Item) this.hat.Value));
        this.hat.Value = (Hat) null;
      }
      else if (this.shirt.Value != null)
      {
        this.DropItem(Utility.PerformSpecialItemGrabReplacement((Item) this.shirt.Value));
        this.shirt.Value = (Clothing) null;
      }
      else if (this.pants.Value != null)
      {
        this.DropItem(Utility.PerformSpecialItemGrabReplacement((Item) this.pants.Value));
        this.pants.Value = (Clothing) null;
      }
      else if (this.boots.Value != null)
      {
        this.DropItem(Utility.PerformSpecialItemGrabReplacement((Item) this.boots.Value));
        this.boots.Value = (Boots) null;
      }
      this.Location.playSound("hammer");
      this.shakeTimer = 100;
      return false;
    }
    this.Location.objects.Remove(this.TileLocation);
    this.Location.playSound("hammer");
    this.DropItem((Item) new Mannequin(this.ItemId));
    return false;
  }

  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (who.CurrentItem is Hat || who.CurrentItem is Clothing || who.CurrentItem is Boots)
      return false;
    if (justCheckingForActivity)
      return true;
    if (this.hat.Value == null && this.shirt.Value == null && this.pants.Value == null && this.boots.Value == null)
    {
      this.facing.Value = (this.facing.Value + 1) % 4;
      this.renderCache = (Farmer) null;
      Game1.playSound("shwip");
    }
    else
    {
      this.changeMutex.RequestLock((Action) (() =>
      {
        this.hat.Value = who.Equip<Hat>(this.hat.Value, who.hat);
        this.shirt.Value = who.Equip<Clothing>(this.shirt.Value, who.shirtItem);
        this.pants.Value = who.Equip<Clothing>(this.pants.Value, who.pantsItem);
        this.boots.Value = who.Equip<Boots>(this.boots.Value, who.boots);
        this.changeMutex.ReleaseLock();
      }));
      Game1.playSound("coin");
    }
    if (this.GetMannequinData().Cursed && Game1.random.NextDouble() < 0.001)
      this.emitGhost();
    return true;
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    switch (dropInItem)
    {
      case Hat hat:
        if (!probe)
        {
          this.DropItem((Item) this.hat.Value);
          this.hat.Value = (Hat) hat.getOne();
          break;
        }
        break;
      case Clothing clothing:
        if (!probe)
        {
          if (clothing.clothesType.Value == Clothing.ClothesType.SHIRT)
          {
            this.DropItem((Item) this.shirt.Value);
            this.shirt.Value = (Clothing) clothing.getOne();
            break;
          }
          this.DropItem((Item) this.pants.Value);
          this.pants.Value = (Clothing) clothing.getOne();
          break;
        }
        break;
      case Boots boots:
        if (!probe)
        {
          this.DropItem((Item) this.boots.Value);
          this.boots.Value = (Boots) boots.getOne();
          break;
        }
        break;
      default:
        return false;
    }
    if (!probe)
      Game1.playSound("dirtyHit");
    return true;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    base.draw(spriteBatch, x, y, alpha);
    if (this.eyeTimer > 0 && this.facing.Value != 0)
    {
      float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1.1E-05f;
      Vector2 local = Game1.GlobalToLocal(new Vector2((float) x, (float) y) * 64f + new Vector2(20f, -40f));
      switch (this.facing.Value)
      {
        case 1:
          local.X += 12f;
          break;
        case 3:
          local.X += 4f;
          break;
      }
      if (this.facing.Value != 2)
        local.Y -= 4f;
      spriteBatch.Draw(Game1.mouseCursors_1_6, local, new Rectangle?(new Rectangle(377 + 5 * (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1620.0 / 60.0), 330, 5 + (this.facing.Value != 2 ? -3 : 0), 3)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
    }
    float num = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (float) x * 1E-05f;
    Farmer farmerForRendering = this.GetFarmerForRendering();
    farmerForRendering.position.Value = new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 4 + (this.GetMannequinData().DisplaysClothingAsMale ? 20 : 16 /*0x10*/)));
    if (this.shakeTimer > 0)
    {
      NetPosition position = farmerForRendering.position;
      position.Value = position.Value + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    }
    farmerForRendering.FarmerRenderer.draw(spriteBatch, farmerForRendering.FarmerSprite, farmerForRendering.FarmerSprite.SourceRect, farmerForRendering.getLocalPosition(Game1.viewport), new Vector2(0.0f, (float) farmerForRendering.GetBoundingBox().Height), num + 0.0001f, Color.White, 0.0f, farmerForRendering);
    FarmerRenderer.FarmerSpriteLayers layer = FarmerRenderer.FarmerSpriteLayers.Arms;
    if (farmerForRendering.facingDirection.Value == 0)
      layer = FarmerRenderer.FarmerSpriteLayers.ArmsUp;
    if (farmerForRendering.FarmerSprite.CurrentAnimationFrame.armOffset <= 0)
      return;
    Rectangle sourceRect = farmerForRendering.FarmerSprite.SourceRect;
    sourceRect.Offset(farmerForRendering.FarmerSprite.CurrentAnimationFrame.armOffset * 16 /*0x10*/ - 288, 0);
    spriteBatch.Draw(farmerForRendering.FarmerRenderer.baseTexture, farmerForRendering.getLocalPosition(Game1.viewport) + new Vector2(0.0f, (float) farmerForRendering.GetBoundingBox().Height) + farmerForRendering.FarmerRenderer.positionOffset + farmerForRendering.armOffset, new Rectangle?(sourceRect), Color.White, 0.0f, new Vector2(0.0f, (float) farmerForRendering.GetBoundingBox().Height), 4f * this.scale, farmerForRendering.FarmerSprite.CurrentAnimationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, FarmerRenderer.GetLayerDepth(num + 0.0001f, layer));
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Mannequin(this.ItemId);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Mannequin mannequin))
      return;
    this.hat.Value = mannequin.hat.Value?.getOne() as Hat;
    this.shirt.Value = mannequin.shirt.Value?.getOne() as Clothing;
    this.pants.Value = mannequin.pants.Value?.getOne() as Clothing;
    this.boots.Value = mannequin.boots.Value?.getOne() as Boots;
  }

  private void DropItem(Item item)
  {
    if (item == null)
      return;
    Vector2 debrisOrigin = new Vector2((float) (((double) this.TileLocation.X + 0.5) * 64.0), (float) (((double) this.TileLocation.Y + 0.5) * 64.0));
    this.Location.debris.Add(new Debris(item, debrisOrigin));
  }

  private Farmer GetFarmerForRendering()
  {
    this.renderCache = this.renderCache ?? CreateInstance();
    return this.renderCache;

    Farmer CreateInstance()
    {
      MannequinData mannequinData = this.GetMannequinData();
      Farmer instance = new Farmer();
      instance.changeGender(mannequinData.DisplaysClothingAsMale);
      instance.faceDirection(this.facing.Value);
      instance.changeHairColor(Color.Transparent);
      instance.skin.Set(instance.FarmerRenderer.recolorSkin(-12345));
      instance.hat.Value = this.hat.Value;
      instance.shirtItem.Value = this.shirt.Value;
      if (this.shirt.Value != null)
        instance.changeShirt("-1");
      instance.pantsItem.Value = this.pants.Value;
      if (this.pants.Value != null)
        instance.changePantStyle("-1");
      instance.boots.Value = this.boots.Value;
      if (this.boots.Value != null)
        instance.changeShoeColor(this.boots.Value.GetBootsColorString());
      instance.FarmerRenderer.textureName.Value = mannequinData.FarmerTexture;
      instance.FarmerSprite.PauseForSingleAnimation = true;
      instance.currentEyes = 0;
      return instance;
    }
  }
}
