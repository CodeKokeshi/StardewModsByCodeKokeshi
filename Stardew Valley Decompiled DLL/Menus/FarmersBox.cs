// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FarmersBox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

internal class FarmersBox : IClickableMenu
{
  private readonly List<Farmer> _farmers = new List<Farmer>();
  public float _updateTimer;

  public FarmersBox()
    : base(0, 200, 528, 400)
  {
  }

  private void UpdateFarmers(List<ClickableComponent> parentComponents)
  {
    if ((double) this._updateTimer > 0.0)
      return;
    this._farmers.Clear();
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      this._farmers.Add(onlineFarmer);
    this._updateTimer = 1f;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    this._updateTimer -= (float) time.ElapsedGameTime.TotalSeconds;
  }

  public void draw(
    SpriteBatch b,
    int left,
    int bottom,
    ClickableComponent current,
    List<ClickableComponent> parentComponents)
  {
    this.UpdateFarmers(parentComponents);
    if (this._farmers.Count == 0)
      return;
    int num = 100;
    this.height = num * this._farmers.Count;
    this.xPositionOnScreen = left;
    this.yPositionOnScreen = bottom - this.height;
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(301, 288, 15, 15), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f, false);
    b.End();
    b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
    int x1 = this.xPositionOnScreen + 16 /*0x10*/;
    int positionOnScreen = this.yPositionOnScreen;
    for (int index = 0; index < this._farmers.Count; ++index)
    {
      Farmer farmer = this._farmers[index];
      Rectangle rectangle = scissorRectangle with
      {
        X = x1,
        Y = positionOnScreen,
        Height = num - 8,
        Width = 200
      };
      b.GraphicsDevice.ScissorRectangle = rectangle;
      FarmerRenderer.isDrawingForUI = true;
      farmer.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(farmer.bathingClothes.Value ? 108 : 0, 0, false, false), farmer.bathingClothes.Value ? 108 : 0, new Rectangle(0, farmer.bathingClothes.Value ? 576 : 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2((float) x1, (float) positionOnScreen), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, farmer);
      FarmerRenderer.isDrawingForUI = false;
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      int x2 = x1 + 80 /*0x50*/;
      int y1 = positionOnScreen + 12;
      string text1 = ChatBox.formattedUserName(farmer);
      b.DrawString(Game1.dialogueFont, text1, new Vector2((float) x2, (float) y1), Color.White);
      string userName = Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID);
      if (!string.IsNullOrEmpty(userName))
      {
        int y2 = y1 + (Game1.dialogueFont.LineSpacing + 4);
        string text2 = $"({userName})";
        b.DrawString(Game1.smallFont, text2, new Vector2((float) x2, (float) y2), Color.White);
      }
      positionOnScreen += num;
    }
    b.GraphicsDevice.ScissorRectangle = scissorRectangle;
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
  }
}
