// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Sign
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Sign : StardewValley.Object
{
  public const int OBJECT = 1;
  public const int HAT = 2;
  public const int BIG_OBJECT = 3;
  public const int RING = 4;
  public const int FURNITURE = 5;
  [XmlElement("displayItem")]
  public readonly NetRef<Item> displayItem = new NetRef<Item>();
  [XmlElement("displayType")]
  public readonly NetInt displayType = new NetInt();

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.displayItem, "displayItem").AddField((INetSerializable) this.displayType, "displayType");
  }

  public Sign()
  {
  }

  public Sign(Vector2 tile, string itemId)
    : base(tile, itemId)
  {
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return who.CurrentItem != null;
    Item currentItem = who.CurrentItem;
    if (currentItem == null)
      return false;
    if (who.isMoving())
      Game1.haltAfterCheck = false;
    this.displayItem.Value = currentItem.getOne();
    Game1.playSound("coin");
    this.displayType.Value = 1;
    switch (this.displayItem.Value)
    {
      case Hat _:
        this.displayType.Value = 2;
        break;
      case Ring _:
        this.displayType.Value = 4;
        break;
      case Furniture _:
        this.displayType.Value = 5;
        break;
      case StardewValley.Object @object:
        this.displayType.Value = @object.bigCraftable.Value ? 3 : 1;
        break;
    }
    return true;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    base.draw(spriteBatch, x, y, alpha);
    if (this.displayItem.Value == null)
      return;
    switch (this.displayType.Value)
    {
      case 1:
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) + 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 8 - 2))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 9.9999997473787516E-06), StackDrawType.Hide, Color.Black, false);
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) + 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 1.9999999494757503E-05), StackDrawType.Hide, Color.White, false);
        break;
      case 2:
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) + 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 9.9999997473787516E-06), StackDrawType.Hide, Color.Black, false);
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) + 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 1.9999999494757503E-05), StackDrawType.Hide, Color.White, false);
        break;
      case 3:
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 9.9999997473787516E-06), StackDrawType.Hide, Color.White, false);
        break;
      case 4:
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) - 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 9.9999997473787516E-06), StackDrawType.Hide, Color.Black, false);
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/) - 1f, (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 1.9999999494757503E-05), StackDrawType.Hide, Color.White, false);
        break;
      case 5:
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 8 - 1))), 0.75f, 0.45f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 9.9999997473787516E-06), StackDrawType.Hide, Color.Black, false);
        this.displayItem.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ + 21 + 4 - 1))), 0.75f, 1f, (float) ((double) Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 24) / 10000f) + (double) x * 9.9999997473787516E-06 + 1.9999999494757503E-05), StackDrawType.Hide, Color.White, false);
        break;
    }
  }

  /// <inheritdoc />
  public override bool ForEachItem(ForEachItemDelegate handler, GetForEachItemPathDelegate getPath)
  {
    return base.ForEachItem(handler, getPath) && ForEachItemHelper.ApplyToField<Item>(this.displayItem, handler, getPath);
  }
}
