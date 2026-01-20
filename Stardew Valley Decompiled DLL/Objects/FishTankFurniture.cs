// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.FishTankFurniture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class FishTankFurniture : StorageFurniture
{
  public const int TANK_DEPTH = 10;
  public const int FLOOR_DECORATION_OFFSET = 4;
  public const int TANK_SORT_REGION = 20;
  [XmlIgnore]
  public List<Vector4> bubbles = new List<Vector4>();
  [XmlIgnore]
  public List<TankFish> tankFish = new List<TankFish>();
  [XmlIgnore]
  public NetEvent0 refreshFishEvent = new NetEvent0();
  [XmlIgnore]
  public bool fishDirty = true;
  [XmlIgnore]
  private Texture2D _aquariumTexture;
  [XmlIgnore]
  public List<KeyValuePair<Rectangle, Vector2>?> floorDecorations = new List<KeyValuePair<Rectangle, Vector2>?>();
  [XmlIgnore]
  public List<Vector2> decorationSlots = new List<Vector2>();
  [XmlIgnore]
  public List<int> floorDecorationIndices = new List<int>();
  public NetInt generationSeed = new NetInt();
  [XmlIgnore]
  public Item localDepositedItem;
  [XmlIgnore]
  protected int _currentDecorationIndex;
  protected Dictionary<Item, TankFish> _fishLookup = new Dictionary<Item, TankFish>();

  public FishTankFurniture() => this.generationSeed.Value = Game1.random.Next();

  public FishTankFurniture(string itemId, Vector2 tile, int initialRotations)
    : base(itemId, tile, initialRotations)
  {
    this.generationSeed.Value = Game1.random.Next();
  }

  public FishTankFurniture(string itemId, Vector2 tile)
    : base(itemId, tile)
  {
    this.generationSeed.Value = Game1.random.Next();
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntryOrPlacement(GameLocation environment, bool dropDown)
  {
    base.actionOnPlayerEntryOrPlacement(environment, dropDown);
    this.ResetFish();
    this.UpdateFish();
  }

  public virtual void ResetFish()
  {
    this.bubbles.Clear();
    this.tankFish.Clear();
    this._fishLookup.Clear();
    this.UpdateFish();
  }

  public Texture2D GetAquariumTexture()
  {
    if (this._aquariumTexture == null)
      this._aquariumTexture = Game1.content.Load<Texture2D>("LooseSprites\\AquariumFish");
    return this._aquariumTexture;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.generationSeed, "generationSeed").AddField((INetSerializable) this.refreshFishEvent, "refreshFishEvent");
    this.refreshFishEvent.onEvent += new NetEvent0.Event(this.UpdateDecorAndFish);
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new FishTankFurniture(this.ItemId, this.tileLocation.Value);
  }

  public virtual int GetCapacityForCategory(FishTankFurniture.FishTankCategories category)
  {
    int num = 0;
    if (this.QualifiedItemId.Equals("(F)JungleTank"))
      ++num;
    switch (category)
    {
      case FishTankFurniture.FishTankCategories.Swim:
        return this.getTilesWide() - 1;
      case FishTankFurniture.FishTankCategories.Ground:
        return this.getTilesWide() - 1 + num;
      case FishTankFurniture.FishTankCategories.Decoration:
        return this.getTilesWide() <= 2 ? 1 : -1;
      default:
        return 0;
    }
  }

  public FishTankFurniture.FishTankCategories GetCategoryFromItem(Item item)
  {
    Dictionary<string, string> aquariumData = this.GetAquariumData();
    if (!this.CanBeDeposited(item))
      return FishTankFurniture.FishTankCategories.None;
    if (item.QualifiedItemId == "(TR)FrogEgg")
      return FishTankFurniture.FishTankCategories.Ground;
    string str1;
    if (!aquariumData.TryGetValue(item.ItemId, out str1))
      return FishTankFurniture.FishTankCategories.Decoration;
    string str2 = ArgUtility.Get(str1.Split('/'), 1);
    return str2 == "crawl" || str2 == "ground" || str2 == "front_crawl" || str2 == "static" ? FishTankFurniture.FishTankCategories.Ground : FishTankFurniture.FishTankCategories.Swim;
  }

  public bool HasRoomForThisItem(Item item)
  {
    if (!this.CanBeDeposited(item))
      return false;
    FishTankFurniture.FishTankCategories categoryFromItem = this.GetCategoryFromItem(item);
    int num1 = this.GetCapacityForCategory(categoryFromItem);
    if (item is Hat)
      num1 = 999;
    if (num1 < 0)
    {
      foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
      {
        if (heldItem != null && heldItem.QualifiedItemId == item.QualifiedItemId)
          return false;
      }
      return true;
    }
    int num2 = 0;
    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
    {
      if (heldItem != null)
      {
        if (this.GetCategoryFromItem(heldItem) == categoryFromItem)
          ++num2;
        if (num2 >= num1)
          return false;
      }
    }
    return true;
  }

  public override string GetShopMenuContext() => "FishTank";

  public override void ShowMenu() => this.ShowShopMenu();

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (justCheckingForActivity || this.mutex.IsLocked())
      return true;
    if ((who.ActiveObject != null || who.CurrentItem is Hat || who.CurrentItem?.QualifiedItemId == "(TR)FrogEgg") && this.localDepositedItem == null && this.CanBeDeposited(who.CurrentItem))
    {
      if (!this.HasRoomForThisItem(who.CurrentItem))
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishTank_Full"));
        return true;
      }
      this.localDepositedItem = who.CurrentItem.getOne();
      if (who.CurrentItem.ConsumeStack(1) == null)
      {
        who.removeItemFromInventory(who.CurrentItem);
        who.showNotCarrying();
      }
      this.mutex.RequestLock((Action) (() =>
      {
        location.playSound("dropItemInWater");
        this.heldItems.Add(this.localDepositedItem);
        this.localDepositedItem = (Item) null;
        this.refreshFishEvent.Fire();
        this.mutex.ReleaseLock();
      }), (Action) (() =>
      {
        this.localDepositedItem = who.addItemToInventory(this.localDepositedItem);
        if (this.localDepositedItem != null)
          Game1.createItemDebris(this.localDepositedItem, new Vector2((float) ((double) this.TileLocation.X + (double) this.getTilesWide() / 2.0 + 0.5), this.TileLocation.Y + 0.5f) * 64f, -1, location);
        this.localDepositedItem = (Item) null;
      }));
      return true;
    }
    this.mutex.RequestLock(new Action(((StorageFurniture) this).ShowMenu));
    return true;
  }

  public virtual bool CanBeDeposited(Item item)
  {
    if (item == null)
      return false;
    if (item.QualifiedItemId == "(TR)FrogEgg")
      return true;
    if (!(item is Hat) && !Utility.IsNormalObjectAtParentSheetIndex(item, item.ItemId))
      return false;
    if (item.QualifiedItemId == "(O)152" || item.QualifiedItemId == "(O)393" || item.QualifiedItemId == "(O)390" || item.QualifiedItemId == "(O)117" || item.QualifiedItemId == "(O)166" || item.QualifiedItemId == "(O)832" || item.QualifiedItemId == "(O)109" || item.QualifiedItemId == "(O)709" || item.QualifiedItemId == "(O)392" || item.QualifiedItemId == "(O)394" || item.QualifiedItemId == "(O)167" || item.QualifiedItemId == "(O)789" || item.QualifiedItemId == "(O)330" || item.QualifiedItemId == "(O)797")
      return true;
    if (!(item is Hat))
      return this.GetAquariumData().ContainsKey(item.ItemId);
    int num1 = 0;
    int num2 = 0;
    foreach (TankFish tankFish in this.tankFish)
    {
      if (tankFish.CanWearHat())
        ++num1;
    }
    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
    {
      if (heldItem is Hat)
        ++num2;
    }
    return num2 < num1;
  }

  public override void DayUpdate()
  {
    this.ResetFish();
    base.DayUpdate();
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (Game1.currentLocation == this.Location)
    {
      if (this.fishDirty)
      {
        this.fishDirty = false;
        this.UpdateDecorAndFish();
      }
      foreach (TankFish tankFish in this.tankFish)
        tankFish.Update(time);
      for (int index = 0; index < this.bubbles.Count; ++index)
      {
        Vector4 bubble = this.bubbles[index];
        bubble.W += 0.05f;
        if ((double) bubble.W > 1.0)
          bubble.W = 1f;
        bubble.Y += bubble.W;
        this.bubbles[index] = bubble;
        if ((double) bubble.Y >= (double) this.GetTankBounds().Height)
        {
          this.bubbles.RemoveAt(index);
          --index;
        }
      }
    }
    base.updateWhenCurrentLocation(time);
    this.refreshFishEvent.Poll();
  }

  /// <inheritdoc />
  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    this.generationSeed.Value = Game1.random.Next();
    this.fishDirty = true;
    return base.placementAction(location, x, y, who);
  }

  public Dictionary<string, string> GetAquariumData() => DataLoader.AquariumFish(Game1.content);

  /// <inheritdoc />
  public override bool onDresserItemWithdrawn(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock)
  {
    int num = base.onDresserItemWithdrawn(salable, who, countTaken, stock) ? 1 : 0;
    this.refreshFishEvent.Fire();
    return num != 0;
  }

  public virtual void UpdateFish()
  {
    List<Item> objList1 = new List<Item>();
    Dictionary<string, string> aquariumData = this.GetAquariumData();
    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
    {
      if (heldItem != null)
      {
        if (heldItem is StardewValley.Object @object)
          @object.reloadSprite();
        bool flag = heldItem.QualifiedItemId == "(TR)FrogEgg";
        if ((flag || Utility.IsNormalObjectAtParentSheetIndex(heldItem, heldItem.ItemId)) && (flag || aquariumData.ContainsKey(heldItem.ItemId)))
          objList1.Add(heldItem);
      }
    }
    List<Item> objList2 = new List<Item>();
    foreach (Item key in this._fishLookup.Keys)
    {
      if (!this.heldItems.Contains(key))
        objList2.Add(key);
    }
    for (int index = 0; index < objList1.Count; ++index)
    {
      Item key = objList1[index];
      if (!this._fishLookup.ContainsKey(key))
      {
        TankFish tankFish = new TankFish(this, key);
        this.tankFish.Add(tankFish);
        this._fishLookup[key] = tankFish;
      }
    }
    foreach (Item key in objList2)
    {
      this.tankFish.Remove(this._fishLookup[key]);
      this.heldItems.Remove(key);
    }
  }

  public virtual void UpdateDecorAndFish()
  {
    Random random1 = Utility.CreateRandom((double) this.generationSeed.Value);
    this.UpdateFish();
    this.decorationSlots.Clear();
    for (int index1 = 0; index1 < 3; ++index1)
    {
      for (int index2 = 0; index2 < this.getTilesWide(); ++index2)
      {
        Vector2 vector2 = new Vector2();
        if (index1 % 2 == 0)
        {
          if (index2 != this.getTilesWide() - 1)
            vector2.X = (float) (16 /*0x10*/ + index2 * 16 /*0x10*/);
          else
            continue;
        }
        else
          vector2.X = (float) (8 + index2 * 16 /*0x10*/);
        vector2.Y = 4f;
        vector2.Y += 3.33333325f * (float) index1;
        this.decorationSlots.Add(vector2);
      }
    }
    this.floorDecorationIndices.Clear();
    this.floorDecorations.Clear();
    this._currentDecorationIndex = 0;
    for (int index = 0; index < this.decorationSlots.Count; ++index)
    {
      this.floorDecorationIndices.Add(index);
      this.floorDecorations.Add(new KeyValuePair<Rectangle, Vector2>?());
    }
    Utility.Shuffle<int>(random1, this.floorDecorationIndices);
    Random random2 = Utility.CreateRandom((double) random1.Next());
    bool flag1 = this.GetItemCount("393") > 0;
    for (int index = 0; index < 1; ++index)
    {
      if (flag1)
        this.AddFloorDecoration(new Rectangle(16 /*0x10*/ * random2.Next(0, 5), 256 /*0x0100*/, 16 /*0x10*/, 16 /*0x10*/));
      else
        this._AdvanceDecorationIndex();
    }
    Random random3 = Utility.CreateRandom((double) random1.Next());
    bool flag2 = this.GetItemCount("152") > 0;
    for (int index = 0; index < 4; ++index)
    {
      if (flag2)
        this.AddFloorDecoration(new Rectangle(16 /*0x10*/ * random3.Next(0, 3), 288, 16 /*0x10*/, 16 /*0x10*/));
      else
        this._AdvanceDecorationIndex();
    }
    Random random4 = Utility.CreateRandom((double) random1.Next());
    bool flag3 = this.GetItemCount("390") > 0;
    for (int index = 0; index < 2; ++index)
    {
      if (flag3)
        this.AddFloorDecoration(new Rectangle(16 /*0x10*/ * random4.Next(0, 3), 272, 16 /*0x10*/, 16 /*0x10*/));
      else
        this._AdvanceDecorationIndex();
    }
    if (this.GetItemCount("117") > 0)
      this.AddFloorDecoration(new Rectangle(48 /*0x30*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("166") > 0)
      this.AddFloorDecoration(new Rectangle(64 /*0x40*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("797") > 0)
      this.AddFloorDecoration(new Rectangle(80 /*0x50*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("832") > 0)
      this.AddFloorDecoration(new Rectangle(96 /*0x60*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("109") > 0)
      this.AddFloorDecoration(new Rectangle(112 /*0x70*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("709") > 0)
      this.AddFloorDecoration(new Rectangle(128 /*0x80*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("392") > 0)
      this.AddFloorDecoration(new Rectangle(144 /*0x90*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("394") > 0)
      this.AddFloorDecoration(new Rectangle(160 /*0xA0*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("167") > 0)
      this.AddFloorDecoration(new Rectangle(176 /*0xB0*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("789") > 0)
      this.AddFloorDecoration(new Rectangle(192 /*0xC0*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
    if (this.GetItemCount("330") > 0)
      this.AddFloorDecoration(new Rectangle(208 /*0xD0*/, 288, 16 /*0x10*/, 16 /*0x10*/));
    else
      this._AdvanceDecorationIndex();
  }

  public virtual void AddFloorDecoration(Rectangle source_rect)
  {
    if (this._currentDecorationIndex == -1)
      return;
    int floorDecorationIndex = this.floorDecorationIndices[this._currentDecorationIndex];
    this._AdvanceDecorationIndex();
    int x = (int) this.decorationSlots[floorDecorationIndex].X;
    int y = (int) this.decorationSlots[floorDecorationIndex].Y;
    if (x < source_rect.Width / 2)
      x = source_rect.Width / 2;
    if (x > this.GetTankBounds().Width / 4 - source_rect.Width / 2)
      x = this.GetTankBounds().Width / 4 - source_rect.Width / 2;
    KeyValuePair<Rectangle, Vector2> keyValuePair = new KeyValuePair<Rectangle, Vector2>(source_rect, new Vector2((float) x, (float) y));
    this.floorDecorations[floorDecorationIndex] = new KeyValuePair<Rectangle, Vector2>?(keyValuePair);
  }

  protected virtual void _AdvanceDecorationIndex()
  {
    for (int index = 0; index < this.decorationSlots.Count; ++index)
    {
      ++this._currentDecorationIndex;
      if (this._currentDecorationIndex >= this.decorationSlots.Count)
        this._currentDecorationIndex = 0;
      if (!this.floorDecorations[this.floorDecorationIndices[this._currentDecorationIndex]].HasValue)
        return;
    }
    this._currentDecorationIndex = 1;
  }

  public override void OnMenuClose()
  {
    this.refreshFishEvent.Fire();
    base.OnMenuClose();
  }

  public Vector2 GetFishSortRegion()
  {
    return new Vector2(this.GetBaseDrawLayer() + 1E-06f, this.GetGlassDrawLayer() - 1E-06f);
  }

  public float GetGlassDrawLayer() => this.GetBaseDrawLayer() + 0.0001f;

  public float GetBaseDrawLayer()
  {
    return this.furniture_type.Value != 12 ? (float) (this.boundingBox.Value.Bottom - (this.furniture_type.Value == 6 || this.furniture_type.Value == 13 ? 48 /*0x30*/ : 8)) / 10000f : 2E-09f;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    Vector2 vector2_1 = Vector2.Zero;
    if (this.isTemporarilyInvisible)
      return;
    Vector2 vector2_2 = this.drawPosition.Value;
    if (!Furniture.isDrawingLocationFurniture)
    {
      vector2_2 = new Vector2((float) x, (float) y) * 64f;
      vector2_2.Y -= (float) (this.sourceRect.Height * 4 - this.boundingBox.Height);
    }
    if (this.shakeTimer > 0)
      vector2_1 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, vector2_2 + vector2_1), new Rectangle?(new Rectangle(sourceRect.X + sourceRect.Width, sourceRect.Y, sourceRect.Width, sourceRect.Height)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer());
    if (Furniture.isDrawingLocationFurniture)
    {
      for (int index = 0; index < this.tankFish.Count; ++index)
      {
        TankFish tankFish = this.tankFish[index];
        float draw_layer = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, tankFish.zPosition / 20f) + 1E-07f * (float) index;
        tankFish.Draw(spriteBatch, alpha, draw_layer);
      }
      Rectangle tankBounds;
      for (int index = 0; index < this.floorDecorations.Count; ++index)
      {
        KeyValuePair<Rectangle, Vector2>? floorDecoration = this.floorDecorations[index];
        if (floorDecoration.HasValue)
        {
          floorDecoration = this.floorDecorations[index];
          KeyValuePair<Rectangle, Vector2> keyValuePair = floorDecoration.Value;
          Vector2 vector2_3 = keyValuePair.Value;
          Rectangle key = keyValuePair.Key;
          float num = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, vector2_3.Y / 20f) - 1E-06f;
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D aquariumTexture = this.GetAquariumTexture();
          tankBounds = this.GetTankBounds();
          double x1 = (double) tankBounds.Left + (double) vector2_3.X * 4.0;
          tankBounds = this.GetTankBounds();
          double y1 = (double) (tankBounds.Bottom - 4) - (double) vector2_3.Y * 4.0;
          Vector2 local = Game1.GlobalToLocal(new Vector2((float) x1, (float) y1));
          Rectangle? sourceRectangle = new Rectangle?(key);
          Color color = Color.White * alpha;
          Vector2 origin = new Vector2((float) (key.Width / 2), (float) (key.Height - 4));
          double layerDepth = (double) num;
          spriteBatch1.Draw(aquariumTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
        }
      }
      foreach (Vector4 bubble in this.bubbles)
      {
        float num = Utility.Lerp(this.GetFishSortRegion().Y, this.GetFishSortRegion().X, bubble.Z / 20f) - 1E-06f;
        SpriteBatch spriteBatch2 = spriteBatch;
        Texture2D aquariumTexture = this.GetAquariumTexture();
        tankBounds = this.GetTankBounds();
        double x2 = (double) tankBounds.Left + (double) bubble.X;
        tankBounds = this.GetTankBounds();
        double y2 = (double) (tankBounds.Bottom - 4) - (double) bubble.Y - (double) bubble.Z * 4.0;
        Vector2 local = Game1.GlobalToLocal(new Vector2((float) x2, (float) y2));
        Rectangle? sourceRectangle = new Rectangle?(new Rectangle(0, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/));
        Color color = Color.White * alpha;
        Vector2 origin = new Vector2(8f, 8f);
        double scale = 4.0 * (double) bubble.W;
        double layerDepth = (double) num;
        spriteBatch2.Draw(aquariumTexture, local, sourceRectangle, color, 0.0f, origin, (float) scale, SpriteEffects.None, (float) layerDepth);
      }
    }
    base.draw(spriteBatch, x, y, alpha);
  }

  public int GetItemCount(string itemId)
  {
    int itemCount = 0;
    foreach (Item heldItem in (NetList<Item, NetRef<Item>>) this.heldItems)
    {
      if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, itemId))
        itemCount += heldItem.Stack;
    }
    return itemCount;
  }

  public virtual Rectangle GetTankBounds()
  {
    Rectangle sourceRect = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).GetSourceRect();
    int num1 = sourceRect.Height / 16 /*0x10*/;
    int num2 = sourceRect.Width / 16 /*0x10*/;
    Rectangle tankBounds = new Rectangle((int) this.TileLocation.X * 64 /*0x40*/, (int) (((double) this.TileLocation.Y - (double) this.getTilesHigh() - 1.0) * 64.0), num2 * 64 /*0x40*/, num1 * 64 /*0x40*/);
    tankBounds.X += 4;
    tankBounds.Width -= 8;
    if (this.QualifiedItemId == "(F)CCFishTank")
    {
      tankBounds.X += 24;
      tankBounds.Width -= 76;
    }
    tankBounds.Height -= 28;
    tankBounds.Y += 64 /*0x40*/;
    tankBounds.Height -= 64 /*0x40*/;
    return tankBounds;
  }

  public enum FishTankCategories
  {
    None,
    Swim,
    Ground,
    Decoration,
  }
}
