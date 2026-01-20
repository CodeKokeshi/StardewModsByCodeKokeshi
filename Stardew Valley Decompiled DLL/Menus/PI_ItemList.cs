// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PI_ItemList
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class PI_ItemList : ProfileItem
{
  protected List<Item> _items;
  protected List<ClickableTextureComponent> _components;
  protected float _height;
  protected List<Vector2> _emptyBoxPositions;

  public PI_ItemList(ProfileMenu context, string name, List<Item> values)
    : base(context, name)
  {
    this._items = values;
    this._components = new List<ClickableTextureComponent>();
    this._height = 0.0f;
    this._emptyBoxPositions = new List<Vector2>();
    this._UpdateIcons();
  }

  public override void Unload()
  {
    base.Unload();
    this._ClearItems();
  }

  protected void _ClearItems()
  {
    for (int index = 0; index < this._components.Count; ++index)
      this._context.UnregisterClickable((ClickableComponent) this._components[index]);
    this._components.Clear();
  }

  protected void _UpdateIcons()
  {
    this._ClearItems();
    Vector2 vector2 = new Vector2(0.0f, 0.0f);
    for (int index = 0; index < this._items.Count; ++index)
    {
      Item obj = this._items[index];
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(obj.QualifiedItemId);
      ClickableTextureComponent textureComponent = new ClickableTextureComponent(obj.DisplayName, new Rectangle((int) vector2.X, (int) vector2.Y, 32 /*0x20*/, 32 /*0x20*/), (string) null, "", dataOrErrorItem.GetTexture(), dataOrErrorItem.GetSourceRect(), 2f);
      textureComponent.myID = 0;
      textureComponent.name = obj.DisplayName;
      textureComponent.upNeighborID = -99998;
      textureComponent.downNeighborID = -99998;
      textureComponent.leftNeighborID = -99998;
      textureComponent.rightNeighborID = -99998;
      textureComponent.region = 502;
      ClickableTextureComponent clickable = textureComponent;
      this._components.Add(clickable);
      this._context.RegisterClickable((ClickableComponent) clickable);
    }
  }

  public override float HandleLayout(float draw_y, Rectangle content_rectangle, int index)
  {
    this._emptyBoxPositions.Clear();
    draw_y = base.HandleLayout(draw_y, content_rectangle, index);
    int num = 0;
    int val2 = (int) draw_y;
    Point point = new Point(4, 4);
    for (int index1 = 0; index1 < this._components.Count; ++index1)
    {
      ClickableTextureComponent component = this._components[index1];
      if (num + component.bounds.Width + point.Y > content_rectangle.Width)
      {
        num = 0;
        draw_y += (float) (component.bounds.Height + point.Y);
      }
      component.bounds.X = content_rectangle.Left + num;
      component.bounds.Y = (int) draw_y;
      num += component.bounds.Width + point.X;
      val2 = Math.Max((int) draw_y + component.bounds.Height, val2);
    }
    for (; num + 32 /*0x20*/ + point.X <= content_rectangle.Width; num += 32 /*0x20*/ + point.X)
      this._emptyBoxPositions.Add(new Vector2((float) (content_rectangle.Left + num), draw_y));
    return (float) (val2 + 8);
  }

  public override void DrawItem(SpriteBatch b)
  {
    for (int index = 0; index < this._components.Count; ++index)
    {
      ClickableTextureComponent component = this._components[index];
      b.Draw(Game1.menuTexture, new Rectangle(component.bounds.X, component.bounds.Y, 32 /*0x20*/, 32 /*0x20*/), new Rectangle?(new Rectangle(64 /*0x40*/, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 4.3E-05f);
      b.Draw(Game1.menuTexture, new Rectangle(component.bounds.X, component.bounds.Y, 32 /*0x20*/, 32 /*0x20*/), new Rectangle?(new Rectangle(128 /*0x80*/, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 4.3E-05f);
      this._components[index].draw(b, Color.White, 4.1E-05f);
      if (Game1.player.Items.ContainsId(this._items[index].ItemId))
        b.Draw(Game1.mouseCursors, new Rectangle(this._components[index].bounds.X + 32 /*0x20*/ - 11, this._components[index].bounds.Y + 32 /*0x20*/ - 13, 11, 13), new Rectangle?(new Rectangle(268, 1436, 11, 13)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 4E-05f);
    }
    for (int index = 0; index < this._emptyBoxPositions.Count; ++index)
    {
      b.Draw(Game1.menuTexture, new Rectangle((int) this._emptyBoxPositions[index].X, (int) this._emptyBoxPositions[index].Y, 32 /*0x20*/, 32 /*0x20*/), new Rectangle?(new Rectangle(64 /*0x40*/, 896, 64 /*0x40*/, 64 /*0x40*/)), Color.White * 0.5f, 0.0f, Vector2.Zero, SpriteEffects.None, 4.3E-05f);
      b.Draw(Game1.menuTexture, new Rectangle((int) this._emptyBoxPositions[index].X, (int) this._emptyBoxPositions[index].Y, 32 /*0x20*/, 32 /*0x20*/), new Rectangle?(new Rectangle(128 /*0x80*/, 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 4.3E-05f);
    }
  }

  public override void performHover(int x, int y)
  {
    for (int index = 0; index < this._components.Count; ++index)
    {
      if (this._components[index].bounds.Contains(new Point(x, y)))
        this._context.hoveredItem = this._items[index];
    }
  }

  public override bool ShouldDraw() => this._items.Count > 0;
}
