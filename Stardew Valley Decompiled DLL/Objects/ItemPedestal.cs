// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ItemPedestal
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class ItemPedestal : StardewValley.Object
{
  [XmlIgnore]
  public NetMutex itemModifyMutex = new NetMutex();
  [XmlElement("requiredItem")]
  public NetRef<StardewValley.Object> requiredItem = new NetRef<StardewValley.Object>();
  [XmlElement("successColor")]
  public NetColor successColor = new NetColor();
  [XmlElement("lockOnSuccess")]
  public NetBool lockOnSuccess = new NetBool();
  [XmlElement("locked")]
  public NetBool locked = new NetBool();
  [XmlElement("match")]
  public NetBool match = new NetBool();
  /// <summary>Whether this is a pedestal at the Ginger Island shrine, which can't be destroyed or picked up.</summary>
  [XmlElement("isIslandShrinePedestal")]
  public readonly NetBool isIslandShrinePedestal = new NetBool();
  [XmlIgnore]
  public Texture2D texture;

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.itemModifyMutex.NetFields, "itemModifyMutex.NetFields").AddField((INetSerializable) this.requiredItem, "requiredItem").AddField((INetSerializable) this.successColor, "successColor").AddField((INetSerializable) this.lockOnSuccess, "lockOnSuccess").AddField((INetSerializable) this.locked, "locked").AddField((INetSerializable) this.match, "match").AddField((INetSerializable) this.isIslandShrinePedestal, "isIslandShrinePedestal");
    this.heldObject.InterpolationWait = false;
  }

  public ItemPedestal()
  {
  }

  public ItemPedestal(
    Vector2 tile,
    StardewValley.Object required_item,
    bool lock_on_success,
    Color success_color,
    string itemId = "221")
    : base(tile, itemId)
  {
    this.requiredItem.Value = required_item;
    this.lockOnSuccess.Value = lock_on_success;
    this.successColor.Value = success_color;
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new ItemPedestal(this.TileLocation, (StardewValley.Object) this.requiredItem.Value?.getOne(), this.lockOnSuccess.Value, this.successColor.Value, this.ItemId);
  }

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is ItemPedestal itemPedestal))
      return;
    this.isIslandShrinePedestal.Value = itemPedestal.isIslandShrinePedestal.Value;
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    GameLocation location = this.Location;
    if (location == null || this.locked.Value || !dropInItem.canBeTrashed())
      return false;
    if (this.heldObject.Value != null && !probe)
    {
      this.DropObject(who);
      return false;
    }
    if (!(dropInItem.GetType() == typeof (StardewValley.Object)))
      return false;
    if (!probe)
    {
      StardewValley.Object placed_object = dropInItem.getOne() as StardewValley.Object;
      this.itemModifyMutex.RequestLock((Action) (() =>
      {
        location.playSound("woodyStep");
        this.heldObject.Value = placed_object;
        this.UpdateItemMatch();
        this.itemModifyMutex.ReleaseLock();
      }), (Action) (() =>
      {
        if (placed_object == this.heldObject.Value)
          return;
        Game1.createItemDebris((Item) placed_object, (this.TileLocation + new Vector2(0.5f, 0.5f)) * 64f, -1, location);
      }));
    }
    return true;
  }

  public virtual void UpdateItemMatch()
  {
    bool flag = false;
    if (this.heldObject.Value != null && this.requiredItem.Value != null && Utility.getStandardDescriptionFromItem((Item) this.heldObject.Value, 1) == Utility.getStandardDescriptionFromItem((Item) this.requiredItem.Value, 1))
      flag = true;
    if (flag == this.match.Value)
      return;
    this.match.Value = flag;
    if (!this.match.Value || !this.lockOnSuccess.Value)
      return;
    this.locked.Value = true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool checking_for_activity = false)
  {
    return !this.locked.Value && (checking_for_activity || this.DropObject(who));
  }

  public bool DropObject(Farmer who)
  {
    if (this.heldObject.Value == null)
      return false;
    this.itemModifyMutex.RequestLock((Action) (() =>
    {
      StardewValley.Object @object = this.heldObject.Value;
      this.heldObject.Value = (StardewValley.Object) null;
      if (who.addItemToInventoryBool((Item) @object))
      {
        @object.performRemoveAction();
        Game1.playSound("coin");
      }
      else
        this.heldObject.Value = @object;
      this.UpdateItemMatch();
      this.itemModifyMutex.ReleaseLock();
    }));
    return true;
  }

  public override bool performToolAction(Tool t)
  {
    return !this.isIslandShrinePedestal.Value && base.performToolAction(t);
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    this.itemModifyMutex.Update(location);
  }

  public override bool onExplosion(Farmer who)
  {
    return !this.isIslandShrinePedestal.Value && base.onExplosion(who);
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    this.itemModifyMutex.ReleaseLock();
  }

  public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
  {
    Vector2 globalPosition = new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/));
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) (((double) globalPosition.Y - 2.0) / 10000.0)));
    if (this.match.Value)
      b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(dataOrErrorItem.GetSourceRect(1)), this.successColor.Value, 0.0f, new Vector2(0.0f, 16f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) (((double) globalPosition.Y - 1.0) / 10000.0)));
    if (this.heldObject.Value == null)
      return;
    Vector2 vector2 = new Vector2((float) x, (float) y);
    if (this.heldObject.Value.bigCraftable.Value)
      --vector2.Y;
    this.heldObject.Value.draw(b, (int) vector2.X * 64 /*0x40*/, (int) (((double) vector2.Y - 0.20000000298023224) * 64.0) - 64 /*0x40*/, globalPosition.Y / 10000f, 1f);
  }
}
