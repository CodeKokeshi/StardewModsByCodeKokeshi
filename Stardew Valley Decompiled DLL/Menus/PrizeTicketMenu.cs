// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.PrizeTicketMenu
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class PrizeTicketMenu : IClickableMenu
{
  public const int WIDTH = 116;
  public const int HEIGHT = 94;
  public Texture2D texture;
  public ClickableTextureComponent mainButton;
  public float pressedButtonTimer;
  public List<Item> currentPrizeTrack = new List<Item>();
  public float getRewardTimer;
  public float moveRewardTrackTimer;
  public float moveRewardTrackPreTimer;
  public bool gettingReward;
  public bool movingRewardTrack;

  public PrizeTicketMenu()
    : base((int) Utility.getTopLeftPositionForCenteringOnScreen(464, 376).X, (int) Utility.getTopLeftPositionForCenteringOnScreen(464, 376).Y, 464, 376, true)
  {
    this.texture = Game1.content.Load<Texture2D>("LooseSprites\\PrizeTicketMenu");
    this.mainButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 192 /*0xC0*/, this.yPositionOnScreen + 216, 92, 88), this.texture, new Rectangle(150, 29, 23, 22), 4f);
    Game1.playSound("machine_bell");
    this.currentPrizeTrack.Add(PrizeTicketMenu.getPrizeItem((int) Game1.stats.Get("ticketPrizesClaimed")));
    this.currentPrizeTrack.Add(PrizeTicketMenu.getPrizeItem((int) Game1.stats.Get("ticketPrizesClaimed") + 1));
    this.currentPrizeTrack.Add(PrizeTicketMenu.getPrizeItem((int) Game1.stats.Get("ticketPrizesClaimed") + 2));
    this.currentPrizeTrack.Add(PrizeTicketMenu.getPrizeItem((int) Game1.stats.Get("ticketPrizesClaimed") + 3));
    this.currentlySnappedComponent = (ClickableComponent) this.mainButton;
    this.snapCursorToCurrentSnappedComponent();
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    if (this.mainButton.containsPoint(x, y) && (double) this.pressedButtonTimer <= 0.0 && !this.gettingReward && !this.movingRewardTrack)
    {
      if (this.mainButton.sourceRect.Y == 29)
        Game1.playSound("button_tap");
      this.mainButton.sourceRect.Y = 51;
    }
    else
      this.mainButton.sourceRect.Y = 29;
    base.performHoverAction(x, y);
  }

  public static Item getPrizeItem(int prizeLevel)
  {
    Random random1 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.player.UniqueMultiplayerID);
    switch (prizeLevel)
    {
      case 0:
        return Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, random1, 12);
      case 1:
        return ItemRegistry.Create(random1.Choose<string>("(O)631", "(O)630"));
      case 2:
        return random1.Choose<Item>(ItemRegistry.Create("(O)770", 10), ItemRegistry.Create("(O)MixedFlowerSeeds", 15));
      case 3:
        return ItemRegistry.Create("(O)MysteryBox", 3);
      case 4:
        return ItemRegistry.Create("(O)StardropTea");
      case 5:
        return ItemRegistry.Create(Game1.player.HouseUpgradeLevel > 0 ? "(F)BluePinstripeDoubleBed" : "(F)BluePinstripeBed");
      case 6:
        return ItemRegistry.Create(random1.Choose<string>("(O)621", "(BC)15", "(BC)MushroomLog"), 4);
      case 7:
        return ItemRegistry.Create(random1.Choose<string>("(O)633", "(O)632"));
      case 8:
        return ItemRegistry.Create("(O)Book_Friendship");
      case 9:
        return random1.Choose<Item>(ItemRegistry.Create("(O)286", 20), ItemRegistry.Create("(O)287", 12), ItemRegistry.Create("(O)288", 6));
      case 10:
        return ItemRegistry.Create("(H)SportsCap");
      case 11:
        return ItemRegistry.Create(random1.Choose<string>("(BC)FishSmoker", "(BC)Dehydrator"));
      case 12:
        return ItemRegistry.Create(random1.Choose<string>("(O)275", "(O)MysteryBox"), 4);
      case 13:
        return ItemRegistry.Create(random1.Choose<string>("(F)FancyHousePlant1", "(F)FancyHousePlant2", "(F)FancyHousePlant3"));
      case 14:
        return ItemRegistry.Create("(O)SkillBook_" + random1.Next(5).ToString());
      case 15:
        return ItemRegistry.Create("(O)StardropTea");
      case 16 /*0x10*/:
        return ItemRegistry.Create("(F)CowDecal");
      case 17:
        return ItemRegistry.Create("(O)749", 8);
      case 18:
        return ItemRegistry.Create(random1.Choose<string>("(BC)10", "(BC)12"), 4);
      case 19:
        return ItemRegistry.Create("(O)72", 5);
      case 20:
        return ItemRegistry.Create("(O)MysteryBox", 5);
      case 21:
        return ItemRegistry.Create("(O)279");
      default:
        Random random2 = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (prizeLevel - prizeLevel % 9));
        switch (prizeLevel % 9)
        {
          case 0:
            return ItemRegistry.Create("(O)MysteryBox", 5);
          case 1:
            return ItemRegistry.Create("(O)872", random2.Next(1, 3));
          case 2:
            return ItemRegistry.Create(random2.Choose<string>("(O)337", "(O)226", "(O)253", "(O)732", "(O)275"), 5);
          case 3:
            return ItemRegistry.Create(random2.Choose<string>("(F)FancyHousePlant1", "(F)FancyHousePlant2", "(F)FancyHousePlant3"));
          case 4:
            return ItemRegistry.Create("(O)StardropTea");
          case 5:
            return ItemRegistry.Create("(O)166");
          case 6:
            return ItemRegistry.Create("(O)645");
          case 7:
            return ItemRegistry.Create(random2.Choose<string>("(F)FancyTree1", "(F)FancyTree2", "(F)FancyTree3", "(F)PigPainting"));
          case 8:
            return random2.Choose<Item>(ItemRegistry.Create("(O)287", 15), ItemRegistry.Create("(O)288", 8));
          default:
            return ItemRegistry.Create("MysteryBox", 5);
        }
    }
  }

  public override bool readyToClose() => !this.gettingReward && base.readyToClose();

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (this.gettingReward)
      return;
    if (this.mainButton.containsPoint(x, y) && (double) this.pressedButtonTimer <= 0.0 && !this.movingRewardTrack)
    {
      Game1.playSound("button_press");
      this.pressedButtonTimer = 200f;
      if (Game1.player.Items.CountId("PrizeTicket") > 0)
      {
        this.gettingReward = true;
        this.getRewardTimer = 0.0f;
        DelayedAction.playSoundAfterDelay("discoverMineral", 750);
      }
    }
    base.receiveLeftClick(x, y, playSound);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    if ((double) this.pressedButtonTimer > 0.0)
    {
      this.pressedButtonTimer -= (float) (int) time.ElapsedGameTime.TotalMilliseconds;
      this.mainButton.sourceRect.Y = 73;
    }
    if ((double) this.pressedButtonTimer <= 0.0 && this.gettingReward)
    {
      this.getRewardTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.getRewardTimer > 2000.0)
      {
        this.getRewardTimer = 2000f;
        Game1.playSound("coin");
        if (!Game1.player.addItemToInventoryBool(this.currentPrizeTrack[0]))
          Game1.createItemDebris(this.currentPrizeTrack[0], Game1.player.getStandingPosition(), 1, Game1.player.currentLocation);
        Game1.player.Items.ReduceId("PrizeTicket", 1);
        int num = (int) Game1.stats.Increment("ticketPrizesClaimed");
        this.currentPrizeTrack.RemoveAt(0);
        this.moveRewardTrackPreTimer = 500f;
        this.gettingReward = false;
        this.movingRewardTrack = true;
        this.moveRewardTrackTimer = 0.0f;
      }
    }
    else if (this.movingRewardTrack)
    {
      if ((double) this.moveRewardTrackPreTimer > 0.0)
      {
        this.moveRewardTrackPreTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.moveRewardTrackPreTimer <= 0.0)
          Game1.playSound("ticket_machine_whir");
      }
      else
      {
        this.moveRewardTrackTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.moveRewardTrackTimer >= 2000.0)
        {
          this.movingRewardTrack = false;
          this.currentPrizeTrack.Add(PrizeTicketMenu.getPrizeItem((int) Game1.stats.Get("ticketPrizesClaimed") + 3));
        }
      }
    }
    base.update(time);
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    if (!Game1.options.showClearBackgrounds)
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
    b.Draw(this.texture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen) + new Vector2(25f, 18f) * 4f, new Rectangle?(new Rectangle(0, 106, 76, 22)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6f);
    for (int index = 0; index < this.currentPrizeTrack.Count; ++index)
    {
      Vector2 vector2 = new Vector2((float) (28 + 22 * index), 21f) * 4f;
      if (this.movingRewardTrack)
      {
        float num = (float) (88.0 - (double) this.moveRewardTrackTimer / 18.0);
        if ((double) num > 0.0)
        {
          vector2.X += num;
          if ((double) this.moveRewardTrackPreTimer <= 0.0)
          {
            vector2.X += (float) Game1.random.Next(-1, 2);
            vector2.Y += (float) Game1.random.Next(-1, 2);
          }
        }
      }
      if (index == 0)
        b.Draw(Game1.fadeToBlackRect, new Rectangle((int) this.Position.X + 100, (int) this.Position.Y + 76, 88, 80 /*0x50*/), Color.LightYellow * 0.33f);
      if (!this.gettingReward || index != 0)
        this.currentPrizeTrack[index].drawInMenu(b, this.Position + vector2, 1f);
    }
    b.Draw(this.texture, new Vector2((float) this.xPositionOnScreen, (float) this.yPositionOnScreen), new Rectangle?(new Rectangle(0, 0, 116, 94)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
    if (this.gettingReward)
    {
      Vector2 vector2 = new Vector2(28f, 21f) * 4f;
      vector2.Y -= this.getRewardTimer / 13f;
      vector2.Y = Math.Max(vector2.Y, 0.0f);
      vector2.X += this.getRewardTimer / 1000f * (float) Game1.random.Next(-1, 2);
      vector2.Y += this.getRewardTimer / 1000f * (float) Game1.random.Next(-1, 2);
      this.currentPrizeTrack[0].drawInMenu(b, this.Position + vector2, 1f, 1f, 0.9f, StackDrawType.Draw, Color.White, false);
    }
    string s = Game1.player.Items.CountId("PrizeTicket").ToString() ?? "";
    SpriteText.drawString(b, s, this.xPositionOnScreen + 360 - SpriteText.getWidthOfString(s) / 2, this.yPositionOnScreen + 276);
    this.mainButton.draw(b);
    base.draw(b);
    this.drawMouse(b);
  }
}
