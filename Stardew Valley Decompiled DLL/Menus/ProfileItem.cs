// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ProfileItem
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace StardewValley.Menus;

public class ProfileItem
{
  protected ProfileMenu _context;
  public string itemName = "";
  protected Vector2 _nameDrawPosition;

  public ProfileItem(ProfileMenu context, string name)
  {
    this._context = context;
    this.itemName = name;
  }

  public virtual void Unload()
  {
  }

  public virtual string GetName() => this.itemName;

  public virtual void performHover(int x, int y)
  {
  }

  public virtual float HandleLayout(float draw_y, Rectangle content_rectangle, int index)
  {
    if (index > 0)
      draw_y += Game1.smallFont.MeasureString(this.GetName()).Y;
    this._nameDrawPosition = new Vector2((float) content_rectangle.Left, draw_y);
    draw_y += Game1.smallFont.MeasureString(this.GetName()).Y;
    return draw_y;
  }

  public virtual void DrawItemName(SpriteBatch b)
  {
    b.DrawString(Game1.smallFont, this.GetName(), this._nameDrawPosition, Game1.textColor);
  }

  public virtual void Draw(SpriteBatch b)
  {
    this.DrawItemName(b);
    this.DrawItem(b);
  }

  public virtual void DrawItem(SpriteBatch b)
  {
  }

  public virtual bool ShouldDraw() => true;
}
