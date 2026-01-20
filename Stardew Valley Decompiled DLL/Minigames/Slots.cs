// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.Slots
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Minigames;

public class Slots : IMinigame
{
  public const float slotTurnRate = 0.008f;
  public const int numberOfIcons = 8;
  public const int defaultBet = 10;
  private string coinBuffer;
  private List<float> slots;
  private List<float> slotResults;
  private ClickableComponent spinButton10;
  private ClickableComponent spinButton100;
  private ClickableComponent doneButton;
  public bool spinning;
  public bool showResult;
  public float payoutModifier;
  public int currentBet;
  public int spinsCount;
  public int slotsFinished;
  public int endTimer;
  public ClickableComponent currentlySnappedComponent;

  public Slots(int toBet = -1, bool highStakes = false)
  {
    this.coinBuffer = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.zh ? "　　" : "  ";
    this.currentBet = toBet;
    if (this.currentBet == -1)
      this.currentBet = 10;
    this.slots = new List<float>() { 0.0f, 0.0f, 0.0f };
    this.slotResults = new List<float>()
    {
      0.0f,
      0.0f,
      0.0f
    };
    Game1.playSound("newArtifact");
    this.setSlotResults(this.slots);
    int yOffset = 44;
    this.spinButton10 = this.CreateSpinButton(32 /*0x20*/, yOffset, "Strings\\StringsFromCSFiles:Slots.cs.12117");
    this.spinButton100 = this.CreateSpinButton(37, yOffset + 64 /*0x40*/, "Strings\\StringsFromCSFiles:Slots.cs.12118");
    this.doneButton = this.CreateSpinButton(30, yOffset + 128 /*0x80*/, "Strings\\StringsFromCSFiles:NameSelect.cs.3864");
    if (!Game1.isAnyGamePadButtonBeingPressed())
      return;
    Game1.setMousePosition(this.spinButton10.bounds.Center);
    if (!Game1.options.SnappyMenus)
      return;
    this.currentlySnappedComponent = this.spinButton10;
  }

  private ClickableComponent CreateSpinButton(
    int baseWidth,
    int yOffset,
    string nameTranslationKey)
  {
    int buttonSizeOffset = this.GetButtonSizeOffset();
    int width = (baseWidth + buttonSizeOffset) * 4;
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, width, 52, -16, yOffset);
    return new ClickableComponent(new Rectangle((int) centeringOnScreen.X, (int) centeringOnScreen.Y, width, 52), Game1.content.LoadString(nameTranslationKey));
  }

  public void setSlotResults(List<float> toSet)
  {
    double num1 = Game1.random.NextDouble();
    double num2 = 1.0 + Game1.player.DailyLuck * 2.0 + (double) Game1.player.LuckLevel * 0.08;
    if (num1 < 0.001 * num2)
    {
      this.set(toSet, 5);
      this.payoutModifier = 2500f;
    }
    else if (num1 < 1.0 / 625.0 * num2)
    {
      this.set(toSet, 6);
      this.payoutModifier = 1000f;
    }
    else if (num1 < 1.0 / 400.0 * num2)
    {
      this.set(toSet, 7);
      this.payoutModifier = 500f;
    }
    else if (num1 < 0.005 * num2)
    {
      this.set(toSet, 4);
      this.payoutModifier = 200f;
    }
    else if (num1 < 0.007 * num2)
    {
      this.set(toSet, 3);
      this.payoutModifier = 120f;
    }
    else if (num1 < 0.01 * num2)
    {
      this.set(toSet, 2);
      this.payoutModifier = 80f;
    }
    else if (num1 < 0.02 * num2)
    {
      this.set(toSet, 1);
      this.payoutModifier = 30f;
    }
    else if (num1 < 0.12 * num2)
    {
      int num3 = Game1.random.Next(3);
      for (int index = 0; index < 3; ++index)
        toSet[index] = index == num3 ? (float) Game1.random.Next(7) : 7f;
      this.payoutModifier = 3f;
    }
    else if (num1 < 0.2 * num2)
    {
      this.set(toSet, 0);
      this.payoutModifier = 5f;
    }
    else if (num1 < 0.4 * num2)
    {
      int num4 = Game1.random.Next(3);
      for (int index = 0; index < 3; ++index)
        toSet[index] = index == num4 ? 7f : (float) Game1.random.Next(7);
      this.payoutModifier = 2f;
    }
    else
    {
      this.payoutModifier = 0.0f;
      int[] numArray = new int[8];
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int index2 = Game1.random.Next(6);
        while (numArray[index2] > 1)
          index2 = Game1.random.Next(6);
        toSet[index1] = (float) index2;
        ++numArray[index2];
      }
    }
  }

  private void set(List<float> toSet, int number)
  {
    toSet[0] = (float) number;
    toSet[1] = (float) number;
    toSet[2] = (float) number;
  }

  public bool tick(GameTime time)
  {
    TimeSpan elapsedGameTime;
    if (this.spinning && this.endTimer <= 0)
    {
      for (int slotsFinished = this.slotsFinished; slotsFinished < this.slots.Count; ++slotsFinished)
      {
        float slot = this.slots[slotsFinished];
        List<float> slots = this.slots;
        int index1 = slotsFinished;
        List<float> floatList = slots;
        int index2 = index1;
        double num1 = (double) slots[index1];
        elapsedGameTime = time.ElapsedGameTime;
        double num2 = (double) elapsedGameTime.Milliseconds * 0.00800000037997961 * (1.0 - (double) slotsFinished * 0.05000000074505806);
        double num3 = num1 + num2;
        floatList[index2] = (float) num3;
        this.slots[slotsFinished] %= 8f;
        if (slotsFinished == 2)
        {
          if ((double) slot % (0.25 + (double) this.slotsFinished * 0.5) > (double) this.slots[slotsFinished] % (0.25 + (double) this.slotsFinished * 0.5))
            Game1.playSound("shiny4");
          if ((double) slot > (double) this.slots[slotsFinished])
            ++this.spinsCount;
        }
        if (this.spinsCount > 0 && slotsFinished == this.slotsFinished)
        {
          double num4 = (double) Math.Abs(this.slots[slotsFinished] - this.slotResults[slotsFinished]);
          elapsedGameTime = time.ElapsedGameTime;
          double num5 = (double) elapsedGameTime.Milliseconds * 0.00800000037997961;
          if (num4 <= num5)
          {
            this.slots[slotsFinished] = this.slotResults[slotsFinished];
            ++this.slotsFinished;
            --this.spinsCount;
            Game1.playSound("Cowboy_gunshot");
          }
        }
      }
      if (this.slotsFinished >= 3)
        this.endTimer = (double) this.payoutModifier == 0.0 ? 600 : 1000;
    }
    if (this.endTimer > 0)
    {
      int endTimer = this.endTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.endTimer = endTimer - milliseconds;
      if (this.endTimer <= 0)
      {
        this.spinning = false;
        this.spinsCount = 0;
        this.slotsFinished = 0;
        if ((double) this.payoutModifier > 0.0)
        {
          this.showResult = true;
          Game1.playSound((double) this.payoutModifier >= 5.0 ? ((double) this.payoutModifier >= 10.0 ? "reward" : "money") : "newArtifact");
        }
        else
          Game1.playSound("breathout");
        Game1.player.clubCoins += (int) ((double) this.currentBet * (double) this.payoutModifier);
        if ((double) this.payoutModifier == 2500.0)
          Game1.multiplayer.globalChatInfoMessage("Jackpot", Game1.player.Name);
      }
    }
    this.spinButton10.scale = this.spinning || !this.spinButton10.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
    this.spinButton100.scale = this.spinning || !this.spinButton100.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
    this.doneButton.scale = this.spinning || !this.doneButton.bounds.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 1f : 1.05f;
    return false;
  }

  public void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!this.spinning && Game1.player.clubCoins >= 10 && this.spinButton10.bounds.Contains(x, y))
    {
      ++Club.timesPlayedSlots;
      this.setSlotResults(this.slotResults);
      this.spinning = true;
      Game1.playSound("bigSelect");
      this.currentBet = 10;
      this.slotsFinished = 0;
      this.spinsCount = 0;
      this.showResult = false;
      Game1.player.clubCoins -= 10;
    }
    if (!this.spinning && Game1.player.clubCoins >= 100 && this.spinButton100.bounds.Contains(x, y))
    {
      ++Club.timesPlayedSlots;
      this.setSlotResults(this.slotResults);
      Game1.playSound("bigSelect");
      this.spinning = true;
      this.slotsFinished = 0;
      this.spinsCount = 0;
      this.showResult = false;
      this.currentBet = 100;
      Game1.player.clubCoins -= 100;
    }
    if (this.spinning || !this.doneButton.bounds.Contains(x, y))
      return;
    Game1.playSound("bigDeSelect");
    Game1.currentMinigame = (IMinigame) null;
  }

  public void leftClickHeld(int x, int y)
  {
  }

  public void receiveRightClick(int x, int y, bool playSound = true)
  {
  }

  public void releaseLeftClick(int x, int y)
  {
  }

  public void releaseRightClick(int x, int y)
  {
  }

  public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

  public void receiveKeyPress(Keys k)
  {
    if (!this.spinning && (k.Equals((object) Keys.Escape) || Game1.options.doesInputListContain(Game1.options.menuButton, k)))
    {
      this.unload();
      Game1.playSound("bigDeSelect");
      Game1.currentMinigame = (IMinigame) null;
    }
    else
    {
      if (this.spinning || this.currentlySnappedComponent == null)
        return;
      if (Game1.options.doesInputListContain(Game1.options.moveDownButton, k))
      {
        if (this.currentlySnappedComponent.Equals((object) this.spinButton10))
        {
          this.currentlySnappedComponent = this.spinButton100;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
        else
        {
          if (!this.currentlySnappedComponent.Equals((object) this.spinButton100))
            return;
          this.currentlySnappedComponent = this.doneButton;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
      }
      else
      {
        if (!Game1.options.doesInputListContain(Game1.options.moveUpButton, k))
          return;
        if (this.currentlySnappedComponent.Equals((object) this.doneButton))
        {
          this.currentlySnappedComponent = this.spinButton100;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
        else
        {
          if (!this.currentlySnappedComponent.Equals((object) this.spinButton100))
            return;
          this.currentlySnappedComponent = this.spinButton10;
          Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }
      }
    }
  }

  public void receiveKeyRelease(Keys k)
  {
  }

  public int getIconIndex(int index)
  {
    switch (index)
    {
      case 0:
        return 24;
      case 1:
        return 186;
      case 2:
        return 138;
      case 3:
        return 392;
      case 4:
        return 254;
      case 5:
        return 434;
      case 6:
        return 72;
      case 7:
        return 638;
      default:
        return 24;
    }
  }

  public void draw(SpriteBatch b)
  {
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    SpriteBatch spriteBatch = b;
    Texture2D staminaRect = Game1.staminaRect;
    Viewport viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    int width = viewport1.Width;
    viewport1 = Game1.graphics.GraphicsDevice.Viewport;
    int height = viewport1.Height;
    Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
    Color color1 = new Color(38, 0, 7);
    spriteBatch.Draw(staminaRect, destinationRectangle, color1);
    b.Draw(Game1.mouseCursors, Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 228, 52, yOffset: -256), new Rectangle?(new Rectangle(441, 424, 66, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
    int num1 = Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 112 /*0x70*/;
    for (int index = 0; index < 3; ++index)
    {
      Vector2 position = new Vector2((float) (num1 + index * 104), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 128 /*0x80*/));
      b.Draw(Game1.mouseCursors, position, new Rectangle?(new Rectangle(306, 320, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      float num2 = (float) (((double) this.slots[index] + 1.0) % 8.0);
      int iconIndex1 = this.getIconIndex(((int) num2 + 8 - 1) % 8);
      int iconIndex2 = this.getIconIndex((iconIndex1 + 1) % 8);
      b.Draw(Game1.objectSpriteSheet, position - new Vector2(0.0f, (float) (-64.0 * ((double) num2 % 1.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, iconIndex1, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.objectSpriteSheet, position - new Vector2(0.0f, (float) (64.0 - 64.0 * ((double) num2 % 1.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, iconIndex2, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 132 + index * 26 * 4), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 192 /*0xC0*/)), new Rectangle?(new Rectangle(415, 385, 26, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
    }
    int num3 = num1 + 136;
    this.spinButton10.bounds.X = num3 - this.spinButton10.bounds.Width / 2;
    this.spinButton100.bounds.X = num3 - this.spinButton100.bounds.Width / 2;
    this.doneButton.bounds.X = num3 - this.doneButton.bounds.Width / 2;
    int buttonSizeOffset = this.GetButtonSizeOffset();
    b.Draw(Game1.mouseCursors, new Vector2((float) this.spinButton10.bounds.X, (float) this.spinButton10.bounds.Y), new Rectangle?(new Rectangle(441, 385, 32 /*0x20*/ + buttonSizeOffset, 13)), Color.White * (this.spinning || Game1.player.clubCoins < 10 ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f * this.spinButton10.scale, SpriteEffects.None, 0.99f);
    b.Draw(Game1.mouseCursors, new Vector2((float) this.spinButton100.bounds.X, (float) this.spinButton100.bounds.Y), new Rectangle?(new Rectangle(441, 398, 37 + buttonSizeOffset, 13)), Color.White * (this.spinning || Game1.player.clubCoins < 100 ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f * this.spinButton100.scale, SpriteEffects.None, 0.99f);
    b.Draw(Game1.mouseCursors, new Vector2((float) this.doneButton.bounds.X, (float) this.doneButton.bounds.Y), new Rectangle?(new Rectangle(441, 411, 30 + buttonSizeOffset, 13)), Color.White * (!this.spinning ? 1f : 0.5f), 0.0f, Vector2.Zero, 4f * this.doneButton.scale, SpriteEffects.None, 0.99f);
    SpriteBatch b1 = b;
    string s1 = this.coinBuffer + Game1.player.clubCoins.ToString();
    int x1 = Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 376;
    Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    int y1 = viewport2.Height / 2 - 120;
    Color? color2 = new Color?();
    SpriteText.drawStringWithScrollBackground(b1, s1, x1, y1, color: color2);
    SpriteBatch b2 = b;
    Texture2D mouseCursors = Game1.mouseCursors;
    viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    double x2 = (double) (viewport2.Width / 2 - 376 + 4);
    viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    double y2 = (double) (viewport2.Height / 2 - 120 + 4);
    Vector2 position1 = new Vector2((float) x2, (float) y2);
    Rectangle sourceRect = new Rectangle(211, 373, 9, 10);
    Color white = Color.White;
    Vector2 zero = Vector2.Zero;
    Utility.drawWithShadow(b2, mouseCursors, position1, sourceRect, white, 0.0f, zero, 4f, layerDepth: 1f);
    if (this.showResult)
    {
      SpriteBatch b3 = b;
      string s2 = "+" + (this.payoutModifier * (float) this.currentBet).ToString();
      viewport2 = Game1.graphics.GraphicsDevice.Viewport;
      int x3 = viewport2.Width / 2 - 372;
      int y3 = this.spinButton10.bounds.Y - 64 /*0x40*/ + 8;
      Color? color3 = new Color?(SpriteText.color_White);
      SpriteText.drawString(b3, s2, x3, y3, 9999, height: 9999, layerDepth: 1f, color: color3);
    }
    Vector2 vector2;
    ref Vector2 local = ref vector2;
    viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    double x4 = (double) (viewport2.Width / 2 + 200);
    viewport2 = Game1.graphics.GraphicsDevice.Viewport;
    double y4 = (double) (viewport2.Height / 2 - 352);
    local = new Vector2((float) x4, (float) y4);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), (int) vector2.X, (int) vector2.Y, 384, 704, Color.White, 4f);
    b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, 8f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
    SpriteText.drawString(b, "x2", (int) vector2.X + 192 /*0xC0*/ + 16 /*0x10*/, (int) vector2.Y + 24, 9999, height: 99999, color: new Color?(SpriteText.color_White));
    b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, 76f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
    b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(76f, 76f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(7), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
    SpriteText.drawString(b, "x3", (int) vector2.X + 192 /*0xC0*/ + 16 /*0x10*/, (int) vector2.Y + 68 + 24, 9999, height: 99999, color: new Color?(SpriteText.color_White));
    for (int index1 = 0; index1 < 8; ++index1)
    {
      int index2 = index1;
      if (index1 != 5)
      {
        if (index1 == 7)
          index2 = 5;
      }
      else
        index2 = 7;
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(8f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(76f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      b.Draw(Game1.objectSpriteSheet, vector2 + new Vector2(144f, (float) (8 + (index1 + 2) * 68)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.getIconIndex(index2), 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
      int num4 = 0;
      switch (index1)
      {
        case 0:
          num4 = 5;
          break;
        case 1:
          num4 = 30;
          break;
        case 2:
          num4 = 80 /*0x50*/;
          break;
        case 3:
          num4 = 120;
          break;
        case 4:
          num4 = 200;
          break;
        case 5:
          num4 = 500;
          break;
        case 6:
          num4 = 1000;
          break;
        case 7:
          num4 = 2500;
          break;
      }
      SpriteText.drawString(b, "x" + num4.ToString(), (int) vector2.X + 192 /*0xC0*/ + 16 /*0x10*/, (int) vector2.Y + (index1 + 2) * 68 + 24, 9999, height: 99999, color: new Color?(SpriteText.color_White));
    }
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), (int) vector2.X - 640, (int) vector2.Y, 1024 /*0x0400*/, 704, Color.Red, 4f, false);
    for (int index = 1; index < 8; ++index)
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3), (int) vector2.X - 640 - 4 * index, (int) vector2.Y - 4 * index, 1024 /*0x0400*/ + 8 * index, 704 + 8 * index, Color.Red * (float) (1.0 - (double) index * 0.15000000596046448), 4f, false);
    for (int index = 0; index < 17; ++index)
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(147, 472, 3, 3), (int) vector2.X - 640 + 8, (int) vector2.Y + index * 4 * 3 + 12, (int) (608.0 - (double) (index * 64 /*0x40*/) * 1.2000000476837158 + (double) (index * index * 4) * 0.699999988079071), 4, new Color(index * 25, index > 8 ? index * 10 : 0, (int) byte.MaxValue - index * 25), 4f, false);
    if (Game1.IsMultiplayer)
      Utility.drawTextWithColoredShadow(b, Game1.getTimeOfDayString(Game1.timeOfDay), Game1.dialogueFont, new Vector2(vector2.X + 416f - Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).X, vector2.Y - 72f), Color.Purple, Color.Black * 0.2f);
    if (!Game1.options.hardwareCursor)
      b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, (float) (4.0 + (double) Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
    b.End();
  }

  public void changeScreenSize()
  {
  }

  public void unload()
  {
  }

  public void receiveEventPoke(int data)
  {
  }

  public string minigameId() => nameof (Slots);

  public bool doMainGameUpdates() => false;

  public bool forceQuit()
  {
    if (this.spinning)
      Game1.player.clubCoins += this.currentBet;
    this.unload();
    return true;
  }

  /// <summary>Get the pixel amount to add to the spin button widths to account for longer translated text.</summary>
  public int GetButtonSizeOffset()
  {
    switch (Game1.content.GetCurrentLanguage())
    {
      case LocalizedContentManager.LanguageCode.ru:
        return 9;
      case LocalizedContentManager.LanguageCode.pt:
        return 10;
      case LocalizedContentManager.LanguageCode.de:
        return 3;
      case LocalizedContentManager.LanguageCode.fr:
        return 6;
      case LocalizedContentManager.LanguageCode.it:
        return 2;
      case LocalizedContentManager.LanguageCode.hu:
        return 4;
      default:
        return 0;
    }
  }
}
