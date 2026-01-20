// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.LanguageSelectionMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace StardewValley.Menus;

public class LanguageSelectionMenu : IClickableMenu
{
  public new static int width = 500;
  public new static int height = 728;
  protected int _currentPage;
  protected int _pageCount;
  public readonly 
  #nullable disable
  Dictionary<string, LanguageSelectionMenu.LanguageEntry> languages;
  public readonly List<ClickableComponent> languageButtons = new List<ClickableComponent>();
  public ClickableTextureComponent nextPageButton;
  public ClickableTextureComponent previousPageButton;

  public LanguageSelectionMenu()
  {
    Texture2D texture1 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\LanguageButtons");
    this.languages = ((IEnumerable<LanguageSelectionMenu.LanguageEntry>) new LanguageSelectionMenu.LanguageEntry[12]
    {
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.en, (ModLanguage) null, texture1, 0),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.ru, (ModLanguage) null, texture1, 3),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.zh, (ModLanguage) null, texture1, 4),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.de, (ModLanguage) null, texture1, 6),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.pt, (ModLanguage) null, texture1, 2),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.fr, (ModLanguage) null, texture1, 7),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.es, (ModLanguage) null, texture1, 1),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.ja, (ModLanguage) null, texture1, 5),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.ko, (ModLanguage) null, texture1, 8),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.it, (ModLanguage) null, texture1, 10),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.tr, (ModLanguage) null, texture1, 9),
      new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.hu, (ModLanguage) null, texture1, 11)
    }).ToDictionary<LanguageSelectionMenu.LanguageEntry, string>((Func<LanguageSelectionMenu.LanguageEntry, string>) (p => p.LanguageCode.ToString()));
    foreach (ModLanguage additionalLanguage in DataLoader.AdditionalLanguages(Game1.content))
    {
      Texture2D texture2 = Game1.temporaryContent.Load<Texture2D>(additionalLanguage.ButtonTexture);
      this.languages["ModLanguage_" + additionalLanguage.Id] = new LanguageSelectionMenu.LanguageEntry(LocalizedContentManager.LanguageCode.mod, additionalLanguage, texture2, 0);
    }
    this._pageCount = (int) Math.Floor((double) (this.languages.Count - 1) / 12.0) + 1;
    this.SetupButtons();
  }

  private void SetupButtons()
  {
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int) ((double) LanguageSelectionMenu.width * 2.5), LanguageSelectionMenu.height);
    this.languageButtons.Clear();
    int width = LanguageSelectionMenu.width - 128 /*0x80*/;
    int height = 83;
    int num1 = 12 * this._currentPage;
    int num2 = num1 + 11;
    int num3 = 0;
    int num4 = 0;
    int num5 = 0;
    foreach (KeyValuePair<string, LanguageSelectionMenu.LanguageEntry> language in this.languages)
    {
      if (num3 < num1)
        ++num3;
      else if (num3 <= num2)
      {
        this.languageButtons.Add(new ClickableComponent(new Rectangle((int) centeringOnScreen.X + 64 /*0x40*/ + num5 * 6 * 64 /*0x40*/, (int) centeringOnScreen.Y + LanguageSelectionMenu.height - 30 - height * (6 - num4) - 16 /*0x10*/, width, height), language.Key, (string) null)
        {
          myID = num3 - num1,
          downNeighborID = -99998,
          leftNeighborID = -99998,
          rightNeighborID = -99998,
          upNeighborID = -99998
        });
        ++num3;
        ++num5;
        if (num5 > 2)
        {
          ++num4;
          num5 = 0;
        }
      }
      else
        break;
    }
    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle((int) centeringOnScreen.X + 4, (int) centeringOnScreen.Y + LanguageSelectionMenu.height / 2 - 25, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f);
    textureComponent1.myID = 554;
    textureComponent1.downNeighborID = -99998;
    textureComponent1.leftNeighborID = -99998;
    textureComponent1.rightNeighborID = -99998;
    textureComponent1.upNeighborID = -99998;
    textureComponent1.visible = this._currentPage > 0;
    this.previousPageButton = textureComponent1;
    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle((int) ((double) centeringOnScreen.X + (double) LanguageSelectionMenu.width * 2.5) - 32 /*0x20*/, (int) centeringOnScreen.Y + LanguageSelectionMenu.height / 2 - 25, 48 /*0x30*/, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f);
    textureComponent2.myID = 555;
    textureComponent2.downNeighborID = -99998;
    textureComponent2.leftNeighborID = -99998;
    textureComponent2.rightNeighborID = -99998;
    textureComponent2.upNeighborID = -99998;
    textureComponent2.visible = this._currentPage < this._pageCount - 1;
    this.nextPageButton = textureComponent2;
    if (!Game1.options.SnappyMenus)
      return;
    ClickableComponent snappedComponent = this.currentlySnappedComponent;
    int id = snappedComponent != null ? snappedComponent.myID : 0;
    this.populateClickableComponentList();
    this.currentlySnappedComponent = this.getComponentWithID(id);
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    if (this.nextPageButton.visible && this.nextPageButton.containsPoint(x, y))
    {
      Game1.playSound("shwip");
      ++this._currentPage;
      this.SetupButtons();
    }
    else if (this.previousPageButton.visible && this.previousPageButton.containsPoint(x, y))
    {
      Game1.playSound("shwip");
      --this._currentPage;
      this.SetupButtons();
    }
    else
    {
      foreach (ClickableComponent languageButton in this.languageButtons)
      {
        if (languageButton.containsPoint(x, y))
        {
          Game1.playSound("select");
          LanguageSelectionMenu.LanguageEntry valueOrDefault = this.languages.GetValueOrDefault<string, LanguageSelectionMenu.LanguageEntry>(languageButton.name);
          if (valueOrDefault == null)
          {
            Game1.log.Error($"Received click on unknown language button '{languageButton.name}'.");
          }
          else
          {
            if (Game1.options.SnappyMenus)
            {
              Game1.activeClickableMenu.setCurrentlySnappedComponentTo(81118);
              Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
            this.ApplyLanguage(valueOrDefault);
            this.exitThisMenu();
            break;
          }
        }
      }
    }
  }

  public virtual void ApplyLanguage(LanguageSelectionMenu.LanguageEntry entry)
  {
    if (entry.ModLanguage != null)
      LocalizedContentManager.SetModLanguage(entry.ModLanguage);
    else
      LocalizedContentManager.CurrentLanguageCode = entry.LanguageCode;
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    base.performHoverAction(x, y);
    foreach (ClickableComponent languageButton in this.languageButtons)
    {
      if (languageButton.containsPoint(x, y))
      {
        if (languageButton.label == null)
        {
          Game1.playSound("Cowboy_Footstep");
          languageButton.label = "hovered";
        }
      }
      else
        languageButton.label = (string) null;
    }
    this.previousPageButton.tryHover(x, y);
    this.nextPageButton.tryHover(x, y);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int) ((double) LanguageSelectionMenu.width * 2.5), LanguageSelectionMenu.height);
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int) centeringOnScreen.X + 32 /*0x20*/, (int) centeringOnScreen.Y + 156, (int) ((double) LanguageSelectionMenu.width * 2.5499999523162842) - 64 /*0x40*/, LanguageSelectionMenu.height / 2 + 25, Color.White, 4f);
    foreach (ClickableComponent languageButton in this.languageButtons)
    {
      LanguageSelectionMenu.LanguageEntry valueOrDefault = this.languages.GetValueOrDefault<string, LanguageSelectionMenu.LanguageEntry>(languageButton.name);
      if (valueOrDefault != null)
      {
        int y = (valueOrDefault.SpriteIndex <= 6 ? valueOrDefault.SpriteIndex * 78 : (valueOrDefault.SpriteIndex - 7) * 78) + (languageButton.label != null ? 39 : 0);
        int x = valueOrDefault.SpriteIndex > 6 ? 174 : 0;
        b.Draw(valueOrDefault.Texture, languageButton.bounds, new Rectangle?(new Rectangle(x, y, 174, 40)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), SpriteEffects.None, 0.0f);
      }
    }
    this.previousPageButton.draw(b);
    this.nextPageButton.draw(b);
    if (Game1.activeClickableMenu != this)
      return;
    this.drawMouse(b);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.SetupButtons();
  }

  /// <summary>A language which can be selected in this menu.</summary>
  public class LanguageEntry
  {
    /// <summary>The language code for this entry.</summary>
    public readonly LocalizedContentManager.LanguageCode LanguageCode;
    /// <summary>The data for this language in <c>Data/AdditionalLanguages</c>, if applicable.</summary>
    public readonly ModLanguage ModLanguage;
    /// <summary>The button texture to render.</summary>
    public readonly Texture2D Texture;
    /// <summary>The sprite index for the button in the <see cref="F:StardewValley.Menus.LanguageSelectionMenu.LanguageEntry.Texture" />.</summary>
    public readonly int SpriteIndex;

    /// <summary>Construct an instance.</summary>
    /// <param name="languageCode"><inheritdoc cref="F:StardewValley.Menus.LanguageSelectionMenu.LanguageEntry.LanguageCode" path="/summary" /></param>
    /// <param name="modLanguage"><inheritdoc cref="F:StardewValley.Menus.LanguageSelectionMenu.LanguageEntry.ModLanguage" path="/summary" /></param>
    /// <param name="texture"><inheritdoc cref="F:StardewValley.Menus.LanguageSelectionMenu.LanguageEntry.Texture" path="/summary" /></param>
    /// <param name="spriteIndex"><inheritdoc cref="F:StardewValley.Menus.LanguageSelectionMenu.LanguageEntry.SpriteIndex" path="/summary" /></param>
    public LanguageEntry(
      LocalizedContentManager.LanguageCode languageCode,
      ModLanguage modLanguage,
      Texture2D texture,
      int spriteIndex)
    {
      this.LanguageCode = languageCode;
      this.ModLanguage = modLanguage;
      this.Texture = texture;
      this.SpriteIndex = spriteIndex;
    }
  }
}
