// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FieldOfficeMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class FieldOfficeMenu : MenuWithInventory
{
  private Texture2D fieldOfficeMenuTexture;
  private IslandFieldOffice office;
  private bool madeADonation;
  private bool gotReward;
  public List<ClickableComponent> pieceHolders = new List<ClickableComponent>();
  private float bearTimer;
  private float snakeTimer;
  private float batTimer;
  private float frogTimer;

  public FieldOfficeMenu(IslandFieldOffice office)
    : base(new InventoryMenu.highlightThisItem(FieldOfficeMenu.highlightBones), true, true, 16 /*0x10*/, 132)
  {
    FieldOfficeMenu fieldOfficeMenu = this;
    this.office = office;
    this.fieldOfficeMenuTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\FieldOfficeDonationMenu");
    Point point = new Point(this.xPositionOnScreen + 32 /*0x20*/, this.yPositionOnScreen + 96 /*0x60*/);
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 76, point.Y + 180, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[0] ? ItemRegistry.Create("(O)823") : (Item) null)
    {
      label = "823"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 144 /*0x90*/, point.Y + 180, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[1] ? ItemRegistry.Create("(O)824") : (Item) null)
    {
      label = "824"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 212, point.Y + 180, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[2] ? ItemRegistry.Create("(O)823") : (Item) null)
    {
      label = "823"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 76, point.Y + 112 /*0x70*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[3] ? ItemRegistry.Create("(O)822") : (Item) null)
    {
      label = "822"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 144 /*0x90*/, point.Y + 112 /*0x70*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[4] ? ItemRegistry.Create("(O)821") : (Item) null)
    {
      label = "821"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 212, point.Y + 112 /*0x70*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[5] ? ItemRegistry.Create("(O)820") : (Item) null)
    {
      label = "820"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 48 /*0x30*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[6] ? ItemRegistry.Create("(O)826") : (Item) null)
    {
      label = "826"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 128 /*0x80*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[7] ? ItemRegistry.Create("(O)826") : (Item) null)
    {
      label = "826"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 412, point.Y + 208 /*0xD0*/, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[8] ? ItemRegistry.Create("(O)825") : (Item) null)
    {
      label = "825"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 616, point.Y + 36, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[9] ? ItemRegistry.Create("(O)827") : (Item) null)
    {
      label = "827"
    });
    this.pieceHolders.Add(new ClickableComponent(new Rectangle(point.X + 624, point.Y + 156, 64 /*0x40*/, 64 /*0x40*/), office.piecesDonated[10] ? ItemRegistry.Create("(O)828") : (Item) null)
    {
      label = "828"
    });
    if (Game1.activeClickableMenu == null)
      Game1.playSound("bigSelect");
    for (int index = 0; index < this.pieceHolders.Count; ++index)
    {
      ClickableComponent pieceHolder = this.pieceHolders[index];
      int num1;
      int num2 = num1 = -99998;
      pieceHolder.leftNeighborID = num1;
      int num3;
      int num4 = num3 = num2;
      pieceHolder.rightNeighborID = num3;
      int num5;
      int num6 = num5 = num4;
      pieceHolder.downNeighborID = num5;
      pieceHolder.upNeighborID = num6;
      pieceHolder.myID = 1000 + index;
    }
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Top))
      clickableComponent.upNeighborID = -99998;
    foreach (ClickableComponent clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
    {
      clickableComponent.rightNeighborID = 4857;
      clickableComponent.rightNeighborImmutable = true;
    }
    this.populateClickableComponentList();
    if (Game1.options.SnappyMenus)
      this.snapToDefaultClickableComponent();
    this.trashCan.leftNeighborID = this.okButton.leftNeighborID = 11;
    this.exitFunction = (IClickableMenu.onExit) (() =>
    {
      if (!fieldOfficeMenu.madeADonation)
        return;
      string str = "Strings\\Locations:FieldOfficeDonated_" + Game1.random.Next(4).ToString();
      string dialogueText = Game1.content.LoadString(str);
      if (fieldOfficeMenu.gotReward)
        dialogueText = $"{dialogueText}#$b#{Game1.content.LoadString("Strings\\Locations:FieldOfficeDonated_Reward")}";
      Game1.DrawDialogue(new Dialogue(office.getSafariGuy(), str, dialogueText));
      if (!fieldOfficeMenu.gotReward)
        return;
      Game1.multiplayer.globalChatInfoMessage("FieldOfficeCompleteSet", Game1.player.Name);
    });
  }

  public override bool IsAutomaticSnapValid(
    int direction,
    ClickableComponent a,
    ClickableComponent b)
  {
    return (b.myID != 5948 || b.myID == 4857) && base.IsAutomaticSnapValid(direction, a, b);
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(0);
    this.snapCursorToCurrentSnappedComponent();
  }

  public static bool highlightBones(Item i)
  {
    if (i != null)
    {
      IslandFieldOffice islandFieldOffice = Game1.RequireLocation<IslandFieldOffice>("IslandFieldOffice");
      string qualifiedItemId = i.QualifiedItemId;
      if (qualifiedItemId != null && qualifiedItemId.Length == 6)
      {
        switch (qualifiedItemId[5])
        {
          case '0':
            if (qualifiedItemId == "(O)820" && !islandFieldOffice.piecesDonated[5])
              return true;
            break;
          case '1':
            if (qualifiedItemId == "(O)821" && !islandFieldOffice.piecesDonated[4])
              return true;
            break;
          case '2':
            if (qualifiedItemId == "(O)822" && !islandFieldOffice.piecesDonated[3])
              return true;
            break;
          case '3':
            if (qualifiedItemId == "(O)823" && (!islandFieldOffice.piecesDonated[0] || !islandFieldOffice.piecesDonated[2]))
              return true;
            break;
          case '4':
            if (qualifiedItemId == "(O)824" && !islandFieldOffice.piecesDonated[1])
              return true;
            break;
          case '5':
            if (qualifiedItemId == "(O)825" && !islandFieldOffice.piecesDonated[8])
              return true;
            break;
          case '6':
            if (qualifiedItemId == "(O)826" && (!islandFieldOffice.piecesDonated[7] || !islandFieldOffice.piecesDonated[6]))
              return true;
            break;
          case '7':
            if (qualifiedItemId == "(O)827" && !islandFieldOffice.piecesDonated[9])
              return true;
            break;
          case '8':
            if (qualifiedItemId == "(O)828" && !islandFieldOffice.piecesDonated[10])
              return true;
            break;
        }
      }
    }
    return false;
  }

  public static int getPieceIndexForDonationItem(string qualifiedItemId)
  {
    if (qualifiedItemId != null && qualifiedItemId.Length == 6)
    {
      switch (qualifiedItemId[5])
      {
        case '0':
          if (qualifiedItemId == "(O)820")
            return 5;
          break;
        case '1':
          if (qualifiedItemId == "(O)821")
            return 4;
          break;
        case '2':
          if (qualifiedItemId == "(O)822")
            return 3;
          break;
        case '3':
          if (qualifiedItemId == "(O)823")
            return 0;
          break;
        case '4':
          if (qualifiedItemId == "(O)824")
            return 1;
          break;
        case '5':
          if (qualifiedItemId == "(O)825")
            return 8;
          break;
        case '6':
          if (qualifiedItemId == "(O)826")
            return 7;
          break;
        case '7':
          if (qualifiedItemId == "(O)827")
            return 9;
          break;
        case '8':
          if (qualifiedItemId == "(O)828")
            return 10;
          break;
      }
    }
    return -1;
  }

  public static int getDonationPieceIndexNeededForSpot(int donationSpotIndex)
  {
    switch (donationSpotIndex)
    {
      case 0:
      case 2:
        return 823;
      case 1:
        return 824;
      case 3:
        return 822;
      case 4:
        return 821;
      case 5:
        return 820;
      case 6:
      case 7:
        return 826;
      case 8:
        return 825;
      case 9:
        return 827;
      case 10:
        return 828;
      default:
        return -1;
    }
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    base.receiveLeftClick(x, y, playSound);
    if (this.heldItem == null)
      return;
    int indexForDonationItem = FieldOfficeMenu.getPieceIndexForDonationItem(this.heldItem.QualifiedItemId);
    if (indexForDonationItem == -1)
      return;
    switch (this.heldItem.QualifiedItemId)
    {
      case "(O)823":
        if (this.donate(0, x, y))
          break;
        this.donate(2, x, y);
        break;
      case "(O)826":
        if (this.donate(7, x, y))
          break;
        this.donate(6, x, y);
        break;
      default:
        this.donate(indexForDonationItem, x, y);
        break;
    }
  }

  /// <inheritdoc />
  protected override void cleanupBeforeExit()
  {
    base.cleanupBeforeExit();
    if (this.office == null || !this.office.isRangeAllTrue(0, 11) || !this.office.plantsRestoredRight.Value || !this.office.plantsRestoredLeft.Value || Game1.player.hasOrWillReceiveMail("fieldOfficeFinale"))
      return;
    this.office.triggerFinaleCutscene();
  }

  private bool donate(int index, int x, int y)
  {
    if (!this.pieceHolders[index].containsPoint(x, y) || this.pieceHolders[index].item != null)
      return false;
    Item heldItem = this.heldItem;
    this.heldItem = heldItem.ConsumeStack(1);
    this.pieceHolders[index].item = ItemRegistry.Create(heldItem.QualifiedItemId);
    this.checkForSetFinish();
    this.gotReward = this.office.donatePiece(index);
    this.madeADonation = true;
    Game1.playSound("newArtifact");
    Game1.multiplayer.globalChatInfoMessage("FieldOfficeDonation", Game1.player.Name, TokenStringBuilder.ItemNameFor(heldItem));
    return true;
  }

  public void checkForSetFinish()
  {
    if (!this.office.centerSkeletonRestored.Value && this.pieceHolders[0].item != null && this.pieceHolders[1].item != null && this.pieceHolders[2].item != null && this.pieceHolders[3].item != null && this.pieceHolders[4].item != null && this.pieceHolders[5].item != null)
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        this.bearTimer = 500f;
        Game1.playSound("camel");
      }), 700);
    if (!this.office.snakeRestored.Value && this.pieceHolders[6].item != null && this.pieceHolders[7].item != null && this.pieceHolders[8].item != null)
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        this.snakeTimer = 1500f;
        Game1.playSound("steam");
      }), 700);
    if (!this.office.batRestored.Value && this.pieceHolders[9].item != null)
      DelayedAction.functionAfterDelay((Action) (() =>
      {
        this.batTimer = 1500f;
        Game1.playSound("batScreech");
      }), 700);
    if (this.office.frogRestored.Value || this.pieceHolders[10].item == null)
      return;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      this.frogTimer = 1000f;
      Game1.playSound("croak");
    }), 700);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    TimeSpan elapsedGameTime;
    if ((double) this.bearTimer > 0.0)
    {
      double bearTimer = (double) this.bearTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
      this.bearTimer = (float) (bearTimer - totalMilliseconds);
    }
    if ((double) this.snakeTimer > 0.0)
    {
      double snakeTimer = (double) this.snakeTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
      this.snakeTimer = (float) (snakeTimer - totalMilliseconds);
    }
    if ((double) this.batTimer > 0.0)
    {
      double batTimer = (double) this.batTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
      this.batTimer = (float) (batTimer - totalMilliseconds);
    }
    if ((double) this.frogTimer <= 0.0)
      return;
    double frogTimer = (double) this.frogTimer;
    elapsedGameTime = time.ElapsedGameTime;
    double totalMilliseconds1 = elapsedGameTime.TotalMilliseconds;
    this.frogTimer = (float) (frogTimer - totalMilliseconds1);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    this.draw(b, drawDescriptionArea: false, red: 0, green: 80 /*0x50*/, blue: 80 /*0x50*/);
    b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 /*0x20*/), (float) (this.yPositionOnScreen + 96 /*0x60*/)), new Rectangle?(new Rectangle(0, 0, 204, 80 /*0x50*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + this.width - 160 /*0xA0*/), (float) (this.yPositionOnScreen + 108) + ((double) this.batTimer > 0.0 ? (float) (Math.Sin((1500.0 - (double) this.batTimer) / 80.0) * 64.0 / 4.0) : 0.0f)), new Rectangle?(new Rectangle(68, 84, 30, 20)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    foreach (ClickableComponent pieceHolder in this.pieceHolders)
      pieceHolder.item?.drawInMenu(b, Utility.PointToVector2(pieceHolder.bounds.Location), 1f);
    if ((double) this.bearTimer > 0.0)
      b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 /*0x20*/ + 240 /*0xF0*/), (float) (this.yPositionOnScreen + 96 /*0x60*/ + 36)), new Rectangle?(new Rectangle(0, 81, 37, 29)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    else if ((double) this.snakeTimer > 0.0 && (double) this.snakeTimer / 300.0 % 2.0 != 0.0)
      b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 /*0x20*/ + 484), (float) (this.yPositionOnScreen + 96 /*0x60*/ + 232)), new Rectangle?(new Rectangle(47, 84, 19, 19)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    else if ((double) this.frogTimer > 0.0)
      b.Draw(this.fieldOfficeMenuTexture, new Vector2((float) (this.xPositionOnScreen + 32 /*0x20*/ + 708), (float) (this.yPositionOnScreen + 96 /*0x60*/ + 140)), new Rectangle?(new Rectangle(100, 89, 18, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    if (this.heldItem != null)
    {
      int indexForDonationItem = FieldOfficeMenu.getPieceIndexForDonationItem(this.heldItem.QualifiedItemId);
      if (indexForDonationItem != -1)
        this.drawHighlightedSquare(indexForDonationItem, b);
    }
    this.drawMouse(b);
    this.heldItem?.drawInMenu(b, new Vector2((float) (Game1.getOldMouseX() + 16 /*0x10*/), (float) (Game1.getOldMouseY() + 16 /*0x10*/)), 1f);
  }

  private void drawHighlightedSquare(int index, SpriteBatch b)
  {
    Rectangle rectangle = new Rectangle();
    string qualifiedItemId = this.heldItem.QualifiedItemId;
    if (qualifiedItemId != null && qualifiedItemId.Length == 6)
    {
      switch (qualifiedItemId[5])
      {
        case '0':
          if (qualifiedItemId == "(O)820")
            break;
          goto label_17;
        case '1':
          if (qualifiedItemId == "(O)821")
            break;
          goto label_17;
        case '2':
          if (qualifiedItemId == "(O)822")
            break;
          goto label_17;
        case '3':
          if (qualifiedItemId == "(O)823")
            break;
          goto label_17;
        case '4':
          if (qualifiedItemId == "(O)824")
            break;
          goto label_17;
        case '5':
          if (qualifiedItemId == "(O)825")
            goto label_14;
          goto label_17;
        case '6':
          if (qualifiedItemId == "(O)826")
            goto label_14;
          goto label_17;
        case '7':
          if (qualifiedItemId == "(O)827")
          {
            rectangle = new Rectangle(157, 86, 18, 18);
            goto label_17;
          }
          goto label_17;
        case '8':
          if (qualifiedItemId == "(O)828")
          {
            rectangle = new Rectangle(176 /*0xB0*/, 86, 18, 18);
            goto label_17;
          }
          goto label_17;
        default:
          goto label_17;
      }
      rectangle = new Rectangle(119, 86, 18, 18);
      goto label_17;
label_14:
      rectangle = new Rectangle(138, 86, 18, 18);
    }
label_17:
    if (this.pieceHolders[index].item == null)
      b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[index].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
    switch (this.heldItem.QualifiedItemId)
    {
      case "(O)823":
        if (index != 0 || this.pieceHolders[2].item != null)
          break;
        b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[2].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
        break;
      case "(O)826":
        if (index != 7 || this.pieceHolders[6].item != null)
          break;
        b.Draw(this.fieldOfficeMenuTexture, Utility.PointToVector2(this.pieceHolders[6].bounds.Location) + new Vector2(-1f, -1f) * 4f, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
        break;
    }
  }
}
