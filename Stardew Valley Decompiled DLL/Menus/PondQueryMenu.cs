// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PondQueryMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class PondQueryMenu : IClickableMenu
{
  public const int region_okButton = 101;
  public const int region_emptyButton = 103;
  public const int region_noButton = 105;
  public const int region_nettingButton = 106;
  public new static int width = 384;
  public new static int height = 512 /*0x0200*/;
  public const int unresolved_needs_extra_height = 116;
  protected FishPond _pond;
  protected StardewValley.Object _fishItem;
  protected string _statusText = "";
  public ClickableTextureComponent okButton;
  public ClickableTextureComponent emptyButton;
  public ClickableTextureComponent yesButton;
  public ClickableTextureComponent noButton;
  public ClickableTextureComponent changeNettingButton;
  private bool confirmingEmpty;
  protected Rectangle _confirmationBoxRectangle;
  protected string _confirmationText;
  protected float _age;
  private string hoverText = "";

  public PondQueryMenu(FishPond fish_pond)
    : base(Game1.uiViewport.Width / 2 - PondQueryMenu.width / 2, Game1.uiViewport.Height / 2 - PondQueryMenu.height / 2, PondQueryMenu.width, PondQueryMenu.height)
  {
    Game1.player.Halt();
    PondQueryMenu.width = 384;
    PondQueryMenu.height = 512 /*0x0200*/;
    this._pond = fish_pond;
    this._fishItem = new StardewValley.Object(this._pond.fishType.Value, 1);
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 64 /*0x40*/ - IClickableMenu.borderWidth, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
    textureComponent1.myID = 101;
    textureComponent1.upNeighborID = -99998;
    this.okButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 256 /*0x0100*/ - IClickableMenu.borderWidth, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(32 /*0x20*/, 384, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent2.myID = 103;
    textureComponent2.downNeighborID = -99998;
    this.emptyButton = textureComponent2;
    ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + PondQueryMenu.width + 4, this.yPositionOnScreen + PondQueryMenu.height - 192 /*0xC0*/ - IClickableMenu.borderWidth, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, new Rectangle(48 /*0x30*/, 384, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent3.myID = 106;
    textureComponent3.downNeighborID = -99998;
    textureComponent3.upNeighborID = -99998;
    this.changeNettingButton = textureComponent3;
    if (Game1.options.SnappyMenus)
    {
      this.populateClickableComponentList();
      this.snapToDefaultClickableComponent();
    }
    this.UpdateState();
    this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.measureTotalHeight() / 2;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (Game1.globalFade)
      return;
    if (((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)))
    {
      Game1.playSound("smallSelect");
      if (!this.readyToClose())
        return;
      Game1.exitActiveMenu();
    }
    else
    {
      if (!Game1.options.SnappyMenus || ((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)))
        return;
      base.receiveKeyPress(key);
    }
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this._age += (float) time.ElapsedGameTime.TotalSeconds;
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (Game1.globalFade)
      return;
    if (this.confirmingEmpty)
    {
      if (this.yesButton.containsPoint(x, y))
      {
        Game1.playSound("fishSlap");
        this._pond.ClearPond();
        this.exitThisMenu();
      }
      else
      {
        if (!this.noButton.containsPoint(x, y))
          return;
        this.confirmingEmpty = false;
        Game1.playSound("smallSelect");
        if (!Game1.options.SnappyMenus)
          return;
        this.currentlySnappedComponent = this.getComponentWithID(103);
        this.snapCursorToCurrentSnappedComponent();
      }
    }
    else
    {
      if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
      {
        Game1.exitActiveMenu();
        Game1.playSound("smallSelect");
      }
      if (this.changeNettingButton.containsPoint(x, y))
      {
        Game1.playSound("drumkit6");
        ++this._pond.nettingStyle.Value;
        this._pond.nettingStyle.Value %= 4;
      }
      else
      {
        if (!this.emptyButton.containsPoint(x, y))
          return;
        this._confirmationBoxRectangle = new Rectangle(0, 0, 400, 100);
        this._confirmationBoxRectangle.X = Game1.uiViewport.Width / 2 - this._confirmationBoxRectangle.Width / 2;
        this._confirmationText = Game1.content.LoadString("Strings\\UI:PondQuery_ConfirmEmpty");
        this._confirmationText = Game1.parseText(this._confirmationText, Game1.smallFont, this._confirmationBoxRectangle.Width);
        this._confirmationBoxRectangle.Height = (int) Game1.smallFont.MeasureString(this._confirmationText).Y;
        this._confirmationBoxRectangle.Y = Game1.uiViewport.Height / 2 - this._confirmationBoxRectangle.Height / 2;
        this.confirmingEmpty = true;
        ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(Game1.uiViewport.Width / 2 - 64 /*0x40*/ - 4, this._confirmationBoxRectangle.Bottom + 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
        textureComponent1.myID = 111;
        textureComponent1.rightNeighborID = 105;
        this.yesButton = textureComponent1;
        ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(Game1.uiViewport.Width / 2 + 4, this._confirmationBoxRectangle.Bottom + 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);
        textureComponent2.myID = 105;
        textureComponent2.leftNeighborID = 111;
        this.noButton = textureComponent2;
        Game1.playSound("smallSelect");
        if (!Game1.options.SnappyMenus)
          return;
        this.populateClickableComponentList();
        this.currentlySnappedComponent = (ClickableComponent) this.noButton;
        this.snapCursorToCurrentSnappedComponent();
      }
    }
  }

  public override bool readyToClose() => base.readyToClose() && !Game1.globalFade;

  /// <inheritdoc />
  public override void receiveRightClick(int x, int y, bool playSound = true)
  {
    if (Game1.globalFade || !this.readyToClose())
      return;
    Game1.exitActiveMenu();
    Game1.playSound("smallSelect");
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    if (this.okButton != null)
    {
      if (this.okButton.containsPoint(x, y))
        this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
      else
        this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
    }
    if (this.emptyButton != null)
    {
      if (this.emptyButton.containsPoint(x, y))
      {
        this.emptyButton.scale = Math.Min(4.1f, this.emptyButton.scale + 0.05f);
        this.hoverText = Game1.content.LoadString("Strings\\UI:PondQuery_EmptyPond", (object) 10);
      }
      else
        this.emptyButton.scale = Math.Max(4f, this.emptyButton.scale - 0.05f);
    }
    if (this.changeNettingButton != null)
    {
      if (this.changeNettingButton.containsPoint(x, y))
      {
        this.changeNettingButton.scale = Math.Min(4.1f, this.changeNettingButton.scale + 0.05f);
        this.hoverText = Game1.content.LoadString("Strings\\UI:PondQuery_ChangeNetting", (object) 10);
      }
      else
        this.changeNettingButton.scale = Math.Max(4f, this.emptyButton.scale - 0.05f);
    }
    if (this.yesButton != null)
    {
      if (this.yesButton.containsPoint(x, y))
        this.yesButton.scale = Math.Min(1.1f, this.yesButton.scale + 0.05f);
      else
        this.yesButton.scale = Math.Max(1f, this.yesButton.scale - 0.05f);
    }
    if (this.noButton == null)
      return;
    if (this.noButton.containsPoint(x, y))
      this.noButton.scale = Math.Min(1.1f, this.noButton.scale + 0.05f);
    else
      this.noButton.scale = Math.Max(1f, this.noButton.scale - 0.05f);
  }

  public static string GetFishTalkSuffix(StardewValley.Object fishItem)
  {
    HashSet<string> contextTags = fishItem.GetContextTags();
    if (contextTags.Contains("fish_talk_rude"))
      return "_Rude";
    if (contextTags.Contains("fish_talk_stiff"))
      return "_Stiff";
    if (contextTags.Contains("fish_talk_demanding"))
      return "_Demanding";
    foreach (string str in contextTags)
    {
      if (str.StartsWithIgnoreCase("fish_talk_"))
      {
        char[] charArray = str.Substring("fish_talk".Length).ToCharArray();
        bool flag = false;
        for (int index = 0; index < charArray.Length; ++index)
        {
          if (charArray[index] == '_')
            flag = true;
          else if (flag)
          {
            charArray[index] = char.ToUpper(charArray[index]);
            flag = false;
          }
        }
        return new string(charArray);
      }
    }
    return contextTags.Contains("fish_carnivorous") ? "_Carnivore" : "";
  }

  public static string getCompletedRequestString(FishPond pond, StardewValley.Object fishItem, Random r)
  {
    if (fishItem != null)
    {
      string fishTalkSuffix = PondQueryMenu.GetFishTalkSuffix(fishItem);
      if (fishTalkSuffix != "")
        return Lexicon.capitalize(Game1.content.LoadString($"Strings\\UI:PondQuery_StatusRequestComplete{fishTalkSuffix}{r.Next(3).ToString()}", (object) pond.neededItem.Value.DisplayName));
    }
    return Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestComplete" + r.Next(7).ToString(), (object) pond.neededItem.Value.DisplayName);
  }

  public void UpdateState()
  {
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) this._pond.seedOffset.Value);
    if (this._pond.currentOccupants.Value <= 0)
    {
      this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusNoFish");
    }
    else
    {
      if (this._pond.neededItem.Value != null)
      {
        if (this._pond.hasCompletedRequest.Value)
        {
          this._statusText = PondQueryMenu.getCompletedRequestString(this._pond, this._fishItem, daySaveRandom);
          return;
        }
        if (this._pond.HasUnresolvedNeeds())
        {
          string sub2 = this._pond.neededItemCount.Value.ToString() ?? "";
          if (this._pond.neededItemCount.Value <= 1)
          {
            sub2 = Lexicon.getProperArticleForWord(this._pond.neededItem.Value.DisplayName);
            if (sub2 == "")
              sub2 = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestOneCount");
          }
          if (this._fishItem != null)
          {
            if (this._fishItem.HasContextTag("fish_talk_rude"))
            {
              this._statusText = Lexicon.capitalize(Game1.content.LoadString($"Strings\\UI:PondQuery_StatusRequestPending_Rude{daySaveRandom.Next(3).ToString()}_{(Game1.player.IsMale ? "Male" : "Female")}", (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
              return;
            }
            string fishTalkSuffix = PondQueryMenu.GetFishTalkSuffix(this._fishItem);
            if (fishTalkSuffix != "")
            {
              this._statusText = Lexicon.capitalize(Game1.content.LoadString($"Strings\\UI:PondQuery_StatusRequestPending{fishTalkSuffix}{daySaveRandom.Next(3).ToString()}", (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
              return;
            }
          }
          this._statusText = Lexicon.capitalize(Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequestPending" + daySaveRandom.Next(7).ToString(), (object) Lexicon.makePlural(this._pond.neededItem.Value.DisplayName, this._pond.neededItemCount.Value == 1), (object) sub2, (object) this._pond.neededItem.Value.DisplayName));
          return;
        }
      }
      if (this._fishItem != null && (this._fishItem.QualifiedItemId == "(O)397" || this._fishItem.QualifiedItemId == "(O)393"))
        this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusOk_Coral", (object) this._fishItem.DisplayName);
      else
        this._statusText = Game1.content.LoadString("Strings\\UI:PondQuery_StatusOk" + daySaveRandom.Next(7).ToString());
    }
  }

  private int measureTotalHeight() => 644 + this.measureExtraTextHeight(this.getDisplayedText());

  private int measureExtraTextHeight(string displayed_text)
  {
    return Math.Max(0, (int) Game1.smallFont.MeasureString(displayed_text).Y - 90) + 4;
  }

  private string getDisplayedText()
  {
    return Game1.parseText(this._statusText, Game1.smallFont, PondQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - 64 /*0x40*/);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.globalFade)
    {
      Viewport viewport;
      if (!Game1.options.showClearBackgrounds)
      {
        SpriteBatch spriteBatch = b;
        Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        Rectangle bounds = viewport.Bounds;
        Color color = Color.Black * 0.75f;
        spriteBatch.Draw(fadeToBlackRect, bounds, color);
      }
      bool flag = this._pond.neededItem.Value != null && this._pond.HasUnresolvedNeeds() && !this._pond.hasCompletedRequest.Value;
      string text1 = Game1.content.LoadString("Strings\\UI:PondQuery_Name", (object) this._fishItem.DisplayName);
      Vector2 vector2_1 = Game1.smallFont.MeasureString(text1);
      Game1.DrawBox((int) ((double) (Game1.uiViewport.Width / 2) - ((double) vector2_1.X + 64.0) * 0.5), this.yPositionOnScreen - 4 + 128 /*0x80*/, (int) ((double) vector2_1.X + 64.0), 64 /*0x40*/);
      Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float) (Game1.uiViewport.Width / 2) - vector2_1.X * 0.5f, (float) ((double) (this.yPositionOnScreen - 4) + 160.0 - (double) vector2_1.Y * 0.5)), Color.Black);
      string displayedText = this.getDisplayedText();
      int num1 = 0;
      if (flag)
        num1 += 116;
      int num2 = this.measureExtraTextHeight(displayedText);
      Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + 128 /*0x80*/, PondQueryMenu.width, PondQueryMenu.height - 128 /*0x80*/ + num1 + num2, false, true);
      string text2 = Game1.content.LoadString("Strings\\UI:PondQuery_Population", (object) (this._pond.FishCount.ToString() ?? ""), (object) this._pond.maxOccupants);
      Vector2 vector2_2 = Game1.smallFont.MeasureString(text2);
      Utility.drawTextWithShadow(b, text2, Game1.smallFont, new Vector2(this._pond.goldenAnimalCracker.Value ? (float) (this.xPositionOnScreen + IClickableMenu.borderWidth + 4) : (float) (this.xPositionOnScreen + PondQueryMenu.width / 2) - vector2_2.X * 0.5f, (float) (this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16 /*0x10*/ + 128 /*0x80*/)), Game1.textColor);
      int val1 = this._pond.maxOccupants.Value;
      float num3 = 13f;
      int num4 = 0;
      int num5 = 0;
      for (int index = 0; index < val1; ++index)
      {
        float num6 = (float) Math.Sin((double) this._age * 1.0 + (double) num4 * 0.75 + (double) num5 * 0.25) * 2f;
        if (index < this._pond.FishCount)
          this._fishItem.drawInMenu(b, new Vector2((float) ((double) (this.xPositionOnScreen + PondQueryMenu.width / 2) - (double) num3 * (double) Math.Min(val1, 5) * 4.0 * 0.5 + (double) num3 * 4.0 * (double) num4 - 12.0), (float) ((double) (this.yPositionOnScreen + (int) ((double) num6 * 4.0)) + (double) (num5 * 4) * (double) num3 + 275.20001220703125)), 0.75f, 1f, 0.0f, StackDrawType.Hide, Color.White, false);
        else
          this._fishItem.drawInMenu(b, new Vector2((float) ((double) (this.xPositionOnScreen + PondQueryMenu.width / 2) - (double) num3 * (double) Math.Min(val1, 5) * 4.0 * 0.5 + (double) num3 * 4.0 * (double) num4 - 12.0), (float) ((double) (this.yPositionOnScreen + (int) ((double) num6 * 4.0)) + (double) (num5 * 4) * (double) num3 + 275.20001220703125)), 0.75f, 0.35f, 0.0f, StackDrawType.Hide, Color.Black, false);
        ++num4;
        if (num4 == 5)
        {
          num4 = 0;
          ++num5;
        }
      }
      Vector2 vector2_3 = Game1.smallFont.MeasureString(displayedText);
      Utility.drawTextWithShadow(b, displayedText, Game1.smallFont, new Vector2((float) (this.xPositionOnScreen + PondQueryMenu.width / 2) - vector2_3.X * 0.5f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 - (flag ? 32 /*0x20*/ : 48 /*0x30*/)) - vector2_3.Y), Game1.textColor);
      if (flag)
      {
        this.drawHorizontalPartition(b, (int) ((double) (this.yPositionOnScreen + PondQueryMenu.height + num2) - 48.0));
        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 60) + (float) (8.0 * (double) Game1.dialogueButtonScale / 10.0), (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 28)), new Rectangle(412, 495, 5, 4), Color.White, 1.57079637f, Vector2.Zero);
        string text3 = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring");
        Vector2 vector2_4 = Game1.smallFont.MeasureString(text3);
        int num7 = this.xPositionOnScreen + 88;
        float x1 = (float) num7;
        float x2 = (float) ((double) x1 + (double) vector2_4.X + 4.0);
        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ja || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
        {
          x2 = (float) (num7 - 8);
          x1 = (float) (num7 + 76);
        }
        Utility.drawTextWithShadow(b, text3, Game1.smallFont, new Vector2(x1, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 24)), Game1.textColor);
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this._pond.neededItem.Value.QualifiedItemId);
        Texture2D texture = dataOrErrorItem.GetTexture();
        Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
        b.Draw(texture, new Vector2(x2, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 4)), new Rectangle?(sourceRect), Color.Black * 0.4f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(texture, new Vector2(x2 + 4f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2)), new Rectangle?(sourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        if (this._pond.neededItemCount.Value > 1)
          Utility.drawTinyDigits(this._pond.neededItemCount.Value, b, new Vector2(x2 + 48f, (float) (this.yPositionOnScreen + PondQueryMenu.height + num2 + 48 /*0x30*/)), 3f, 1f, Color.White);
      }
      if (this._pond.goldenAnimalCracker.Value && Game1.objectSpriteSheet_2 != null)
        Utility.drawWithShadow(b, Game1.objectSpriteSheet_2, new Vector2((float) (this.xPositionOnScreen + PondQueryMenu.width) - 105.6f, (float) this.yPositionOnScreen + 224f), new Rectangle(16 /*0x10*/, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.89f);
      this.okButton.draw(b);
      this.emptyButton.draw(b);
      this.changeNettingButton.draw(b);
      if (this.confirmingEmpty)
      {
        if (!Game1.options.showClearBackgrounds)
        {
          SpriteBatch spriteBatch = b;
          Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          Rectangle bounds = viewport.Bounds;
          Color color = Color.Black * 0.75f;
          spriteBatch.Draw(fadeToBlackRect, bounds, color);
        }
        int num8 = 16 /*0x10*/;
        this._confirmationBoxRectangle.Width += num8;
        this._confirmationBoxRectangle.Height += num8;
        this._confirmationBoxRectangle.X -= num8 / 2;
        this._confirmationBoxRectangle.Y -= num8 / 2;
        Game1.DrawBox(this._confirmationBoxRectangle.X, this._confirmationBoxRectangle.Y, this._confirmationBoxRectangle.Width, this._confirmationBoxRectangle.Height);
        this._confirmationBoxRectangle.Width -= num8;
        this._confirmationBoxRectangle.Height -= num8;
        this._confirmationBoxRectangle.X += num8 / 2;
        this._confirmationBoxRectangle.Y += num8 / 2;
        b.DrawString(Game1.smallFont, this._confirmationText, new Vector2((float) this._confirmationBoxRectangle.X, (float) this._confirmationBoxRectangle.Y), Game1.textColor);
        this.yesButton.draw(b);
        this.noButton.draw(b);
      }
      else
      {
        string hoverText = this.hoverText;
        if ((hoverText != null ? (hoverText.Length > 0 ? 1 : 0) : 0) != 0)
          IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
      }
    }
    this.drawMouse(b);
  }
}
