// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SocialPage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;
using StardewValley.GameData.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class SocialPage : IClickableMenu
{
  public const int slotsOnPage = 5;
  public string hoverText = "";
  public ClickableTextureComponent upButton;
  public ClickableTextureComponent downButton;
  public ClickableTextureComponent scrollBar;
  public Rectangle scrollBarRunner;
  /// <summary>The players and social NPCs shown in the list.</summary>
  public readonly List<SocialPage.SocialEntry> SocialEntries;
  /// <summary>The character portrait components.</summary>
  public readonly List<ClickableTextureComponent> sprites = new List<ClickableTextureComponent>();
  /// <summary>The index of the <see cref="F:StardewValley.Menus.SocialPage.SocialEntries" /> entry shown at the top of the scrolled view.</summary>
  public int slotPosition;
  /// <summary>The number of players shown in the list.</summary>
  public int numFarmers;
  /// <summary>The clickable slots over which character info is drawn.</summary>
  public readonly List<ClickableTextureComponent> characterSlots = new List<ClickableTextureComponent>();
  public bool scrolling;

  public SocialPage(int x, int y, int width, int height)
    : base(x, y, width, height)
  {
    this.SocialEntries = this.FindSocialCharacters();
    this.numFarmers = this.SocialEntries.Count<SocialPage.SocialEntry>((Func<SocialPage.SocialEntry, bool>) (p => p.IsPlayer));
    this.CreateComponents();
    this.slotPosition = 0;
    for (int index = 0; index < this.SocialEntries.Count; ++index)
    {
      if (!this.SocialEntries[index].IsPlayer)
      {
        this.slotPosition = index;
        break;
      }
    }
    this.setScrollBarToCurrentIndex();
    this.updateSlots();
  }

  /// <summary>Restore the page state when it's recreated for a window resize.</summary>
  /// <param name="oldPage">The previous page instance before it was recreated.</param>
  public void postWindowSizeChange(IClickableMenu oldPage)
  {
    if (!(oldPage is SocialPage socialPage))
      return;
    this.slotPosition = socialPage.slotPosition;
    this.setScrollBarToCurrentIndex();
  }

  /// <summary>Find all social NPCs which should be shown on the social page.</summary>
  public List<SocialPage.SocialEntry> FindSocialCharacters()
  {
    List<SocialPage.SocialEntry> collection = new List<SocialPage.SocialEntry>();
    Dictionary<string, SocialPage.SocialEntry> dictionary = new Dictionary<string, SocialPage.SocialEntry>();
    List<SocialPage.SocialEntry> source = new List<SocialPage.SocialEntry>();
    foreach (NPC allNpc in this.GetAllNpcs())
    {
      Friendship friendship;
      if (!Game1.player.friendshipData.TryGetValue(allNpc.Name, out friendship))
        friendship = (Friendship) null;
      if (allNpc is Child)
        source.Add(new SocialPage.SocialEntry(allNpc, friendship, (CharacterData) null, allNpc.displayName));
      else if (allNpc.CanSocialize)
      {
        CharacterData data = allNpc.GetData();
        string overrideDisplayName = allNpc.displayName;
        SocialTabBehavior? socialTab = data?.SocialTab;
        if (socialTab.HasValue)
        {
          switch (socialTab.GetValueOrDefault())
          {
            case SocialTabBehavior.UnknownUntilMet:
              if (friendship == null)
              {
                overrideDisplayName = "???";
                break;
              }
              break;
            case SocialTabBehavior.AlwaysShown:
              if (friendship == null)
              {
                Game1.player.friendshipData.Add(allNpc.Name, friendship = new Friendship());
                break;
              }
              break;
            case SocialTabBehavior.HiddenUntilMet:
              if (friendship != null)
                break;
              continue;
            case SocialTabBehavior.HiddenAlways:
              continue;
          }
        }
        dictionary[allNpc.Name] = new SocialPage.SocialEntry(allNpc, friendship, data, overrideDisplayName);
      }
    }
    int num = 0;
    foreach (KeyValuePair<string, Friendship> pair in Game1.player.friendshipData.Pairs)
    {
      SocialPage.SocialEntry socialEntry;
      if (dictionary.TryGetValue(pair.Key, out socialEntry))
        socialEntry.OrderMet = new int?(num++);
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (!allFarmer.IsLocalPlayer && (allFarmer.IsMainPlayer || allFarmer.isCustomized.Value) && !allFarmer.IsDedicatedPlayer)
      {
        Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, allFarmer.UniqueMultiplayerID);
        collection.Add(new SocialPage.SocialEntry(allFarmer, friendship));
      }
    }
    List<SocialPage.SocialEntry> socialCharacters = new List<SocialPage.SocialEntry>();
    socialCharacters.AddRange((IEnumerable<SocialPage.SocialEntry>) collection);
    socialCharacters.AddRange((IEnumerable<SocialPage.SocialEntry>) dictionary.Values.OrderByDescending<SocialPage.SocialEntry, int?>((Func<SocialPage.SocialEntry, int?>) (entry => entry.Friendship?.Points)).ThenBy<SocialPage.SocialEntry, int?>((Func<SocialPage.SocialEntry, int?>) (entry => entry.OrderMet)).ThenBy<SocialPage.SocialEntry, string>((Func<SocialPage.SocialEntry, string>) (entry => entry.DisplayName)));
    socialCharacters.AddRange((IEnumerable<SocialPage.SocialEntry>) source.OrderBy<SocialPage.SocialEntry, string>((Func<SocialPage.SocialEntry, string>) (p => p.DisplayName)));
    return socialCharacters;
  }

  /// <summary>Get all child or villager NPCs from the world and friendship data.</summary>
  public IEnumerable<NPC> GetAllNpcs()
  {
    HashSet<string> nonSocial = new HashSet<string>();
    Dictionary<string, NPC> found = new Dictionary<string, NPC>();
    Utility.ForEachCharacter((Func<NPC, bool>) (npc =>
    {
      if (npc is Child)
        found[npc.Name + "$$child"] = npc;
      else if (npc.IsVillager)
      {
        if (!npc.CanSocialize)
        {
          nonSocial.Add(npc.Name);
        }
        else
        {
          NPC npc1;
          if (found.TryGetValue(npc.Name, out npc1) && npc != npc1)
          {
            bool flag1 = true;
            if (Game1.IsClient)
            {
              int num1 = npc1.currentLocation.IsActiveLocation() ? 1 : 0;
              bool flag2 = npc.currentLocation.IsActiveLocation();
              int num2 = flag2 ? 1 : 0;
              if (num1 != num2)
              {
                if (flag2)
                  found[npc.Name] = npc;
                flag1 = false;
              }
            }
            if (flag1)
              Game1.log.Warn($"The social page found conflicting NPCs with name {npc.Name} (one at {npc1.currentLocation?.NameOrUniqueName} {npc1.TilePoint}, the other at {npc.currentLocation?.NameOrUniqueName} {npc.TilePoint}); only the first will be shown.");
          }
          else
            found[npc.Name] = npc;
        }
      }
      return true;
    }));
    Event currentEvent = Game1.currentLocation?.currentEvent;
    if (currentEvent != null)
    {
      foreach (NPC actor in currentEvent.actors)
      {
        if (actor.IsVillager && actor.CanSocialize)
          found[actor.Name] = actor;
      }
    }
    foreach (string key in Game1.player.friendshipData.Keys)
    {
      if (!nonSocial.Contains(key) && !found.ContainsKey(key) && NPC.TryGetData(key, out CharacterData _))
      {
        string nameForCharacter = NPC.getTextureNameForCharacter(key);
        string str = "Characters\\" + nameForCharacter;
        string assetName = "Portraits\\" + nameForCharacter;
        if (Game1.content.DoesAssetExist<Texture2D>(str))
        {
          if (Game1.content.DoesAssetExist<Texture2D>(assetName))
          {
            try
            {
              AnimatedSprite sprite = new AnimatedSprite(str, 0, 16 /*0x10*/, 32 /*0x20*/);
              Texture2D portrait = Game1.content.Load<Texture2D>(assetName);
              found[key] = new NPC(sprite, Vector2.Zero, "Town", 0, key, portrait, false);
            }
            catch
            {
            }
          }
        }
      }
    }
    return (IEnumerable<NPC>) found.Values;
  }

  /// <summary>Load the clickable components to display.</summary>
  public void CreateComponents()
  {
    this.sprites.Clear();
    this.characterSlots.Clear();
    for (int index = 0; index < this.SocialEntries.Count; ++index)
    {
      this.sprites.Add(this.CreateSpriteComponent(this.SocialEntries[index], index));
      ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth, 0, this.width - IClickableMenu.borderWidth * 2, this.rowPosition(1) - this.rowPosition(0)), (Texture2D) null, new Rectangle(0, 0, 0, 0), 4f);
      textureComponent1.myID = index;
      textureComponent1.downNeighborID = index + 1;
      textureComponent1.upNeighborID = index - 1;
      ClickableTextureComponent textureComponent2 = textureComponent1;
      if (textureComponent2.upNeighborID < 0)
        textureComponent2.upNeighborID = 12342;
      this.characterSlots.Add(textureComponent2);
    }
    this.upButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
    this.downButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 16 /*0x10*/, this.yPositionOnScreen + this.height - 64 /*0x40*/, 44, 48 /*0x30*/), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
    this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upButton.bounds.X + 12, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
    this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, this.height - 128 /*0x80*/ - this.upButton.bounds.Height - 8);
  }

  /// <summary>Create the clickable texture component for a character's portrait.</summary>
  /// <param name="entry">The social character to render.</param>
  /// <param name="index">The index in the list of entries.</param>
  public ClickableTextureComponent CreateSpriteComponent(SocialPage.SocialEntry entry, int index)
  {
    Rectangle bounds = new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth + 4, 0, this.width, 64 /*0x40*/);
    Rectangle sourceRect = entry.IsPlayer || !(entry.Character is NPC character) ? Rectangle.Empty : character.getMugShotSourceRect();
    return new ClickableTextureComponent(index.ToString(), bounds, (string) null, "", entry.Character.Sprite.Texture, sourceRect, 4f);
  }

  /// <summary>Get the social entry from its index in the list.</summary>
  /// <param name="index">The index in the social list.</param>
  public SocialPage.SocialEntry GetSocialEntry(int index)
  {
    if (index < 0 || index >= this.SocialEntries.Count)
      index = 0;
    return this.SocialEntries[index];
  }

  public override void snapToDefaultClickableComponent()
  {
    if (this.slotPosition < this.characterSlots.Count)
      this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[this.slotPosition];
    this.snapCursorToCurrentSnappedComponent();
  }

  public void updateSlots()
  {
    for (int index = 0; index < this.characterSlots.Count; ++index)
      this.characterSlots[index].bounds.Y = this.rowPosition(index - 1);
    int num1 = 0;
    for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5; ++slotPosition)
    {
      if (this.sprites.Count > slotPosition)
      {
        int num2 = this.yPositionOnScreen + IClickableMenu.borderWidth + 32 /*0x20*/ + 112 /*0x70*/ * num1 + 32 /*0x20*/;
        this.sprites[slotPosition].bounds.Y = num2;
      }
      ++num1;
    }
    this.populateClickableComponentList();
    this.addTabsToClickableComponents();
  }

  public void addTabsToClickableComponents()
  {
    if (!(Game1.activeClickableMenu is GameMenu activeClickableMenu) || this.allClickableComponents.Contains(activeClickableMenu.tabs[0]))
      return;
    this.allClickableComponents.AddRange((IEnumerable<ClickableComponent>) activeClickableMenu.tabs);
  }

  protected void _SelectSlot(SocialPage.SocialEntry entry)
  {
    bool flag = false;
    for (int index = 0; index < this.SocialEntries.Count; ++index)
    {
      SocialPage.SocialEntry socialEntry = this.SocialEntries[index];
      if (socialEntry.InternalName == entry.InternalName && socialEntry.IsPlayer == entry.IsPlayer && socialEntry.IsChild == entry.IsChild)
      {
        this._SelectSlot((ClickableComponent) this.characterSlots[index]);
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    this._SelectSlot((ClickableComponent) this.characterSlots[0]);
  }

  protected void _SelectSlot(ClickableComponent slot_component)
  {
    if (slot_component == null || !((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(slot_component))
      return;
    int num = this.characterSlots.IndexOf(slot_component as ClickableTextureComponent);
    this.currentlySnappedComponent = slot_component;
    if (num < this.slotPosition)
      this.slotPosition = num;
    else if (num >= this.slotPosition + 5)
      this.slotPosition = num - 5 + 1;
    this.setScrollBarToCurrentIndex();
    this.updateSlots();
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  public void ConstrainSelectionToVisibleSlots()
  {
    if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      return;
    int index = this.characterSlots.IndexOf(this.currentlySnappedComponent as ClickableTextureComponent);
    if (index < this.slotPosition)
      index = this.slotPosition;
    else if (index >= this.slotPosition + 5)
      index = this.slotPosition + 5 - 1;
    this.currentlySnappedComponent = (ClickableComponent) this.characterSlots[index];
    if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
      return;
    this.snapCursorToCurrentSnappedComponent();
  }

  public override void snapCursorToCurrentSnappedComponent()
  {
    if (this.currentlySnappedComponent != null && ((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      Game1.setMousePosition(this.currentlySnappedComponent.bounds.Left + 64 /*0x40*/, this.currentlySnappedComponent.bounds.Center.Y);
    else
      base.snapCursorToCurrentSnappedComponent();
  }

  public override void applyMovementKey(int direction)
  {
    base.applyMovementKey(direction);
    if (!((IEnumerable<ClickableComponent>) this.characterSlots).Contains<ClickableComponent>(this.currentlySnappedComponent))
      return;
    this._SelectSlot(this.currentlySnappedComponent);
  }

  /// <inheritdoc />
  public override void leftClickHeld(int x, int y)
  {
    base.leftClickHeld(x, y);
    if (!this.scrolling)
      return;
    int y1 = this.scrollBar.bounds.Y;
    this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - 64 /*0x40*/ - 12 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upButton.bounds.Height + 20));
    this.slotPosition = Math.Min(this.sprites.Count - 5, Math.Max(0, (int) ((double) this.sprites.Count * (double) ((float) (y - this.scrollBarRunner.Y) / (float) this.scrollBarRunner.Height))));
    this.setScrollBarToCurrentIndex();
    int y2 = this.scrollBar.bounds.Y;
    if (y1 == y2)
      return;
    Game1.playSound("shiny4");
  }

  /// <inheritdoc />
  public override void releaseLeftClick(int x, int y)
  {
    base.releaseLeftClick(x, y);
    this.scrolling = false;
  }

  private void setScrollBarToCurrentIndex()
  {
    if (this.sprites.Count > 0)
    {
      this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.sprites.Count - 5 + 1) * this.slotPosition + this.upButton.bounds.Bottom + 4;
      if (this.slotPosition == this.sprites.Count - 5)
        this.scrollBar.bounds.Y = this.downButton.bounds.Y - this.scrollBar.bounds.Height - 4;
    }
    this.updateSlots();
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    base.receiveScrollWheelAction(direction);
    if (direction > 0 && this.slotPosition > 0)
    {
      this.upArrowPressed();
      this.ConstrainSelectionToVisibleSlots();
      Game1.playSound("shiny4");
    }
    else
    {
      if (direction >= 0 || this.slotPosition >= Math.Max(0, this.sprites.Count - 5))
        return;
      this.downArrowPressed();
      this.ConstrainSelectionToVisibleSlots();
      Game1.playSound("shiny4");
    }
  }

  public void upArrowPressed()
  {
    --this.slotPosition;
    this.updateSlots();
    this.upButton.scale = 3.5f;
    this.setScrollBarToCurrentIndex();
  }

  public void downArrowPressed()
  {
    ++this.slotPosition;
    this.updateSlots();
    this.downButton.scale = 3.5f;
    this.setScrollBarToCurrentIndex();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.upButton.containsPoint(x, y) && this.slotPosition > 0)
    {
      this.upArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.downButton.containsPoint(x, y) && this.slotPosition < this.sprites.Count - 5)
    {
      this.downArrowPressed();
      Game1.playSound("shwip");
    }
    else if (this.scrollBar.containsPoint(x, y))
      this.scrolling = true;
    else if (!this.downButton.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + 128 /*0x80*/ && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height)
    {
      this.scrolling = true;
      this.leftClickHeld(x, y);
      this.releaseLeftClick(x, y);
    }
    else
    {
      for (int index = 0; index < this.characterSlots.Count; ++index)
      {
        if (index >= this.slotPosition && index < this.slotPosition + 5 && this.characterSlots[index].bounds.Contains(x, y))
        {
          SocialPage.SocialEntry socialEntry = this.GetSocialEntry(index);
          if (!socialEntry.IsPlayer && !socialEntry.IsChild && Game1.player.friendshipData.ContainsKey(socialEntry.Character.name.Value))
          {
            Game1.playSound("bigSelect");
            int cached_slot_position = this.slotPosition;
            ProfileMenu profileMenu = new ProfileMenu(socialEntry, this.SocialEntries);
            profileMenu.exitFunction = (IClickableMenu.onExit) (() =>
            {
              if (!(((GameMenu) (Game1.activeClickableMenu = (IClickableMenu) new GameMenu(GameMenu.socialTab, playOpeningSound: false))).GetCurrentPage() is SocialPage currentPage2))
                return;
              currentPage2.slotPosition = cached_slot_position;
              currentPage2._SelectSlot(profileMenu.Current);
            });
            Game1.activeClickableMenu = (IClickableMenu) profileMenu;
            if (!Game1.options.SnappyMenus)
              return;
            profileMenu.snapToDefaultClickableComponent();
            return;
          }
          Game1.playSound("shiny4");
          break;
        }
      }
      this.slotPosition = Math.Max(0, Math.Min(this.sprites.Count - 5, this.slotPosition));
    }
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    this.upButton.tryHover(x, y);
    this.downButton.tryHover(x, y);
  }

  public bool isCharacterSlotClickable(int i)
  {
    SocialPage.SocialEntry socialEntry = this.GetSocialEntry(i);
    return socialEntry != null && !socialEntry.IsPlayer && !socialEntry.IsChild && socialEntry.IsMet;
  }

  /// <summary>Draw an NPC's entry in the social page.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="i">The index of the NPC in <see cref="F:StardewValley.Menus.SocialPage.sprites" />.</param>
  public void drawNPCSlot(SpriteBatch b, int i)
  {
    SocialPage.SocialEntry socialEntry = this.GetSocialEntry(i);
    if (socialEntry == null)
      return;
    if ((!this.isCharacterSlotClickable(i) ? 0 : (this.characterSlots[i].bounds.Contains(Game1.getMouseX(), Game1.getMouseY()) ? 1 : 0)) != 0)
      b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + IClickableMenu.borderWidth - 4, this.sprites[i].bounds.Y - 4, this.characterSlots[i].bounds.Width, this.characterSlots[i].bounds.Height - 12), Color.White * 0.25f);
    this.sprites[i].draw(b);
    string internalName = socialEntry.InternalName;
    Gender gender = socialEntry.Gender;
    bool isDatable = socialEntry.IsDatable;
    bool isDating = socialEntry.IsDatingCurrentPlayer();
    bool currentPlayer = socialEntry.IsMarriedToCurrentPlayer();
    bool flag = socialEntry.IsRoommateForCurrentPlayer();
    float y = Game1.smallFont.MeasureString("W").Y;
    float num = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? (float) (-(double) y / 2.0) : 0.0f;
    b.DrawString(Game1.dialogueFont, socialEntry.DisplayName, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 64 /*0x40*/ - 20 + 96 /*0x60*/) - Game1.dialogueFont.MeasureString(socialEntry.DisplayName).X / 2f, (float) ((double) (this.sprites[i].bounds.Y + 48 /*0x30*/) + (double) num - (isDatable ? 24.0 : 20.0))), Game1.textColor);
    for (int hearts = 0; hearts < Math.Max(Utility.GetMaximumHeartsForCharacter((Character) Game1.getCharacterFromName(internalName)), 10); ++hearts)
      this.drawNPCSlotHeart(b, i, socialEntry, hearts, isDating, currentPlayer);
    if (isDatable | flag)
    {
      string text1 = !flag ? (!currentPlayer ? (!socialEntry.IsMarriedToAnyone() ? (!(!Game1.player.isMarriedOrRoommates() & isDating) ? (!socialEntry.IsDivorcedFromCurrentPlayer() ? (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Female") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Male")) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExWife") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExHusband"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Girlfriend") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Boyfriend"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_FemaleNpc") : Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_MaleNpc"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Wife") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Husband"))) : (gender == Gender.Female ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Housemate_Female") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Housemate_Male"));
      int width = (IClickableMenu.borderWidth * 3 + 128 /*0x80*/ - 40 + 192 /*0xC0*/) / 2;
      string text2 = Game1.parseText(text1, Game1.smallFont, width);
      Vector2 vector2 = Game1.smallFont.MeasureString(text2);
      b.DrawString(Game1.smallFont, text2, new Vector2((float) (this.xPositionOnScreen + 192 /*0xC0*/ + 8) - vector2.X / 2f, (float) this.sprites[i].bounds.Bottom - (vector2.Y - y)), Game1.textColor);
    }
    if (!currentPlayer && !socialEntry.IsChild)
    {
      Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2((float) (this.xPositionOnScreen + 384 + 304), (float) (this.sprites[i].bounds.Y - 4)), new Rectangle(166, 174, 14, 12), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f, horizontalShadowOffset: 0, shadowIntensity: 0.2f);
      SpriteBatch spriteBatch1 = b;
      Texture2D mouseCursors1 = Game1.mouseCursors;
      Vector2 position1 = new Vector2((float) (this.xPositionOnScreen + 384 + 296), (float) (this.sprites[i].bounds.Y + 32 /*0x20*/ + 20));
      Friendship friendship1 = socialEntry.Friendship;
      Rectangle? sourceRectangle1 = new Rectangle?(new Rectangle(227 + ((friendship1 != null ? (friendship1.GiftsThisWeek >= 2 ? 1 : 0) : 0) != 0 ? 9 : 0), 425, 9, 9));
      Color white1 = Color.White;
      Vector2 zero1 = Vector2.Zero;
      spriteBatch1.Draw(mouseCursors1, position1, sourceRectangle1, white1, 0.0f, zero1, 4f, SpriteEffects.None, 0.88f);
      SpriteBatch spriteBatch2 = b;
      Texture2D mouseCursors2 = Game1.mouseCursors;
      Vector2 position2 = new Vector2((float) (this.xPositionOnScreen + 384 + 336), (float) (this.sprites[i].bounds.Y + 32 /*0x20*/ + 20));
      Friendship friendship2 = socialEntry.Friendship;
      Rectangle? sourceRectangle2 = new Rectangle?(new Rectangle(227 + ((friendship2 != null ? (friendship2.GiftsThisWeek >= 1 ? 1 : 0) : 0) != 0 ? 9 : 0), 425, 9, 9));
      Color white2 = Color.White;
      Vector2 zero2 = Vector2.Zero;
      spriteBatch2.Draw(mouseCursors2, position2, sourceRectangle2, white2, 0.0f, zero2, 4f, SpriteEffects.None, 0.88f);
      Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2((float) (this.xPositionOnScreen + 384 + 424), (float) this.sprites[i].bounds.Y), new Rectangle(180, 175, 13, 11), Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: 0.88f, horizontalShadowOffset: 0, shadowIntensity: 0.2f);
      SpriteBatch spriteBatch3 = b;
      Texture2D mouseCursors3 = Game1.mouseCursors;
      Vector2 position3 = new Vector2((float) (this.xPositionOnScreen + 384 + 432), (float) (this.sprites[i].bounds.Y + 32 /*0x20*/ + 20));
      Friendship friendship3 = socialEntry.Friendship;
      Rectangle? sourceRectangle3 = new Rectangle?(new Rectangle(227 + ((friendship3 != null ? (friendship3.TalkedToToday ? 1 : 0) : 0) != 0 ? 9 : 0), 425, 9, 9));
      Color white3 = Color.White;
      Vector2 zero3 = Vector2.Zero;
      spriteBatch3.Draw(mouseCursors3, position3, sourceRectangle3, white3, 0.0f, zero3, 4f, SpriteEffects.None, 0.88f);
    }
    if (currentPlayer)
    {
      if (flag && !(internalName == "Krobus"))
        return;
      b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192 /*0xC0*/), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag ? 808 : 460, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
    }
    else
    {
      if (!isDating)
        return;
      b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192 /*0xC0*/), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag ? 808 : 458, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
    }
  }

  /// <summary>Draw the heart sprite for an NPC's entry in the social page.</summary>
  /// <param name="b">The sprite batch being drawn.</param>
  /// <param name="npcIndex">The index of the NPC in <see cref="F:StardewValley.Menus.SocialPage.sprites" />.</param>
  /// <param name="entry">The NPC's cached social data.</param>
  /// <param name="hearts">The current heart index being drawn (starting at 0 for the first heart).</param>
  /// <param name="isDating">Whether the player is currently dating this NPC.</param>
  /// <param name="isCurrentSpouse">Whether the player is currently married to this NPC.</param>
  public void drawNPCSlotHeart(
    SpriteBatch b,
    int npcIndex,
    SocialPage.SocialEntry entry,
    int hearts,
    bool isDating,
    bool isCurrentSpouse)
  {
    bool flag = entry.IsDatable && !isDating && !isCurrentSpouse && hearts >= 8;
    int x = hearts < entry.HeartLevel | flag ? 211 : 218;
    Color color = hearts < 10 & flag ? Color.Black * 0.35f : Color.White;
    if (hearts < 10)
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 320 - 4 + hearts * 32 /*0x20*/), (float) (this.sprites[npcIndex].bounds.Y + 64 /*0x40*/ - 28)), new Rectangle?(new Rectangle(x, 428, 7, 6)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
    else
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 320 - 4 + (hearts - 10) * 32 /*0x20*/), (float) (this.sprites[npcIndex].bounds.Y + 64 /*0x40*/)), new Rectangle?(new Rectangle(x, 428, 7, 6)), color, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
  }

  public int rowPosition(int i)
  {
    int num1 = i - this.slotPosition;
    int num2 = 112 /*0x70*/;
    return this.yPositionOnScreen + IClickableMenu.borderWidth + 160 /*0xA0*/ + 4 + num1 * num2;
  }

  public void drawFarmerSlot(SpriteBatch b, int i)
  {
    SocialPage.SocialEntry socialEntry = this.GetSocialEntry(i);
    if (socialEntry == null)
      return;
    if (!socialEntry.IsPlayer)
    {
      Game1.log.Warn($"Social page can't draw farmer slot for index {i}: this is NPC '{socialEntry.InternalName}', not a farmer.");
    }
    else
    {
      Farmer character = (Farmer) socialEntry.Character;
      Gender gender = socialEntry.Gender;
      ClickableTextureComponent sprite = this.sprites[i];
      int x = sprite.bounds.X;
      int y1 = sprite.bounds.Y;
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      Rectangle rectangle = scissorRectangle;
      rectangle.Height = Math.Min(rectangle.Bottom, this.rowPosition(i)) - rectangle.Y - 4;
      b.GraphicsDevice.ScissorRectangle = rectangle;
      FarmerRenderer.isDrawingForUI = true;
      try
      {
        character.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(character.bathingClothes.Value ? 108 : 0, 0, false, false), character.bathingClothes.Value ? 108 : 0, new Rectangle(0, character.bathingClothes.Value ? 576 : 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2((float) x, (float) y1), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, character);
      }
      finally
      {
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      }
      FarmerRenderer.isDrawingForUI = false;
      int num1 = socialEntry.IsMarriedToCurrentPlayer() ? 1 : 0;
      float y2 = Game1.smallFont.MeasureString("W").Y;
      float num2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? (float) (-(double) y2 / 2.0) : 0.0f;
      b.DrawString(Game1.dialogueFont, character.Name, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 96 /*0x60*/ - 20), (float) ((double) (this.sprites[i].bounds.Y + 48 /*0x30*/) + (double) num2 - 24.0)), Game1.textColor);
      string text1 = !Game1.content.ShouldUseGenderedCharacterTranslations() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Female") : (gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Female").Split('/')[0] : ((IEnumerable<string>) Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Single_Female").Split('/')).Last<string>());
      if (num1 != 0)
        text1 = gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Husband") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Wife");
      else if (character.isMarriedOrRoommates() && !character.hasRoommate())
        text1 = gender == Gender.Male ? Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_MaleNpc") : Game1.content.LoadString("Strings\\UI:SocialPage_Relationship_MarriedToOtherPlayer_FemaleNpc");
      else if (!Game1.player.isMarriedOrRoommates() && socialEntry.IsDatingCurrentPlayer())
        text1 = gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Boyfriend") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_Girlfriend");
      else if (socialEntry.IsDivorcedFromCurrentPlayer())
        text1 = gender == Gender.Male ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExHusband") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage_Relationship_ExWife");
      int width = (IClickableMenu.borderWidth * 3 + 128 /*0x80*/ - 40 + 192 /*0xC0*/) / 2;
      string text2 = Game1.parseText(text1, Game1.smallFont, width);
      Vector2 vector2 = Game1.smallFont.MeasureString(text2);
      b.DrawString(Game1.smallFont, text2, new Vector2((float) (this.xPositionOnScreen + 192 /*0xC0*/ + 8) - vector2.X / 2f, (float) this.sprites[i].bounds.Bottom - (vector2.Y - y2)), Game1.textColor);
      if (num1 != 0)
      {
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192 /*0xC0*/), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 801, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
      else
      {
        if (!socialEntry.IsDatingCurrentPlayer())
          return;
        b.Draw(Game1.objectSpriteSheet, new Vector2((float) (this.xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192 /*0xC0*/), (float) this.sprites[i].bounds.Y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 458, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    b.End();
    b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
    this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 128 /*0x80*/ + 4, true);
    this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 192 /*0xC0*/ + 32 /*0x20*/ + 20, true);
    this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, true);
    this.drawHorizontalPartition(b, this.yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 /*0x20*/ + 52, true);
    for (int slotPosition = this.slotPosition; slotPosition < this.slotPosition + 5 && slotPosition < this.sprites.Count; ++slotPosition)
    {
      SocialPage.SocialEntry socialEntry = this.GetSocialEntry(slotPosition);
      if (socialEntry != null)
      {
        if (socialEntry.IsPlayer)
          this.drawFarmerSlot(b, slotPosition);
        else
          this.drawNPCSlot(b, slotPosition);
      }
    }
    Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
    Rectangle rectangle = scissorRectangle with
    {
      Y = Math.Max(0, this.rowPosition(this.numFarmers - 1))
    };
    rectangle.Height -= rectangle.Y;
    if (rectangle.Height > 0)
    {
      b.GraphicsDevice.ScissorRectangle = rectangle;
      try
      {
        this.drawVerticalPartition(b, this.xPositionOnScreen + 256 /*0x0100*/ + 12, true);
        this.drawVerticalPartition(b, this.xPositionOnScreen + 384 + 368, true);
        this.drawVerticalPartition(b, this.xPositionOnScreen + 256 /*0x0100*/ + 12 + 352, true);
      }
      finally
      {
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      }
    }
    this.upButton.draw(b);
    this.downButton.draw(b);
    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
    this.scrollBar.draw(b);
    if (!this.hoverText.Equals(""))
      IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
    b.End();
    b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
  }

  /// <summary>An entry on the social page.</summary>
  public class SocialEntry
  {
    /// <summary>The backing field for <see cref="M:StardewValley.Menus.SocialPage.SocialEntry.IsMarriedToAnyone" />.</summary>
    private bool? CachedIsMarriedToAnyone;
    /// <summary>The character instance.</summary>
    public Character Character;
    /// <summary>The unique multiplayer ID for a player, or the internal name for an NPC.</summary>
    public readonly string InternalName;
    /// <summary>The translated display name.</summary>
    public readonly string DisplayName;
    /// <summary>Whether the current player has met this character.</summary>
    public readonly bool IsMet;
    /// <summary>Whether players can romance this character.</summary>
    public readonly bool IsDatable;
    /// <summary>How the NPC is shown on the social tab.</summary>
    public readonly SocialTabBehavior SocialTabBehavior;
    /// <summary>Whether this character is a child.</summary>
    public readonly bool IsChild;
    /// <summary>Whether this character is a player.</summary>
    public readonly bool IsPlayer;
    /// <summary>The character's gender identity.</summary>
    public readonly Gender Gender;
    /// <summary>The current player's heart level with this character.</summary>
    public readonly int HeartLevel;
    /// <summary>The current player's friendship data with the character, if any.</summary>
    public readonly Friendship Friendship;
    /// <summary>The NPC's character data, if applicable.</summary>
    public readonly CharacterData Data;
    /// <summary>The order in which the current player met this NPC, if applicable.</summary>
    public int? OrderMet;

    /// <summary>Construct an instance.</summary>
    /// <param name="player">The player for which to create an entry.</param>
    /// <param name="friendship">The current player's friendship with this character.</param>
    public SocialEntry(Farmer player, Friendship friendship)
    {
      this.Character = (Character) player;
      this.InternalName = player.UniqueMultiplayerID.ToString();
      this.DisplayName = player.Name;
      this.IsMet = true;
      this.IsPlayer = true;
      this.Gender = player.Gender;
      this.Friendship = friendship;
    }

    /// <summary>Construct an instance.</summary>
    /// <param name="npc">The NPC for which to create an entry.</param>
    /// <param name="friendship">The current player's friendship with this character.</param>
    /// <param name="data">The NPC's character data, if applicable.</param>
    /// <param name="overrideDisplayName">The translated display name, or <c>null</c> to get it from <paramref name="npc" />.</param>
    public SocialEntry(
      NPC npc,
      Friendship friendship,
      CharacterData data,
      string overrideDisplayName = null)
    {
      this.Character = (Character) npc;
      this.InternalName = npc.Name;
      this.DisplayName = overrideDisplayName ?? npc.displayName;
      this.IsMet = friendship != null || npc is Child;
      this.IsDatable = data != null && data.CanBeRomanced;
      this.SocialTabBehavior = data != null ? data.SocialTab : SocialTabBehavior.AlwaysShown;
      this.IsChild = npc is Child;
      this.Gender = npc.Gender;
      this.HeartLevel = (friendship != null ? friendship.Points : 0) / 250;
      this.Friendship = friendship;
      this.Data = data;
    }

    /// <summary>Get whether the current player is dating this character.</summary>
    public bool IsDatingCurrentPlayer()
    {
      Friendship friendship = this.Friendship;
      return friendship != null && friendship.IsDating();
    }

    /// <summary>Get whether the current player is married to this character.</summary>
    public bool IsMarriedToCurrentPlayer()
    {
      Friendship friendship = this.Friendship;
      return friendship != null && friendship.IsMarried();
    }

    /// <summary>Get whether the current player is a roommate with this character.</summary>
    public bool IsRoommateForCurrentPlayer()
    {
      Friendship friendship = this.Friendship;
      return friendship != null && friendship.IsRoommate();
    }

    /// <summary>Get whether the current player is married to this character.</summary>
    public bool IsDivorcedFromCurrentPlayer()
    {
      Friendship friendship = this.Friendship;
      return friendship != null && friendship.IsDivorced();
    }

    /// <summary>Get whether this character is married to any player.</summary>
    public bool IsMarriedToAnyone()
    {
      if (!this.CachedIsMarriedToAnyone.HasValue)
      {
        if (this.IsMarriedToCurrentPlayer())
        {
          this.CachedIsMarriedToAnyone = new bool?(true);
        }
        else
        {
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (allFarmer.spouse == this.InternalName && allFarmer.isMarriedOrRoommates())
            {
              this.CachedIsMarriedToAnyone = new bool?(true);
              break;
            }
          }
          if (!this.CachedIsMarriedToAnyone.HasValue)
            this.CachedIsMarriedToAnyone = new bool?(false);
        }
      }
      return this.CachedIsMarriedToAnyone.Value;
    }
  }
}
