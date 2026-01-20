// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BeachNightMarket
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class BeachNightMarket : GameLocation
{
  private Texture2D shopClosedTexture;
  private float smokeTimer;
  private string paintingMailKey;
  private bool hasReceivedFreeGift;
  private bool hasShownCCUpgrade;

  public BeachNightMarket() => this.forceLoadPathLayerLights = true;

  public BeachNightMarket(string mapPath, string name)
    : base(mapPath, name)
  {
    this.forceLoadPathLayerLights = true;
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.objects.Clear();
    this.hasReceivedFreeGift = false;
    this.paintingMailKey = $"NightMarketYear{Game1.year.ToString()}Day{this.getDayOfNightMarket().ToString()}_paintingSold";
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (Game1.timeOfDay < 1700)
    {
      b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(39f, 29f) * 64f + new Vector2(-1f, -3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(72, 167, 16 /*0x10*/, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(47f, 34f) * 64f + new Vector2(7f, -3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(45, 170, 26, 14)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
      b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(19f, 31f) * 64f + new Vector2(6f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(89, 164, 18, 23)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f / 1000f);
    }
    if (Game1.player.mailReceived.Contains(this.paintingMailKey))
      return;
    b.Draw(this.shopClosedTexture, Game1.GlobalToLocal(new Vector2(41f, 33f) * 64f + new Vector2(2f, 2f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144 /*0x90*/ + (this.getDayOfNightMarket() - 1 + (Game1.year - 1) % 3 * 3) * 28, 201, 28, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.225000009f);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "night market"))
    {
      case 68:
        if (Game1.timeOfDay < 1700)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterClosed"));
          break;
        }
        if (Game1.player.mailReceived.Contains(this.paintingMailKey))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
          break;
        }
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterQuestion"), this.createYesNoResponses(), "PainterQuestion");
        break;
      case 69:
      case 877:
        if (Game1.timeOfDay < 1700)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverClosed"));
          break;
        }
        if (!this.hasReceivedFreeGift)
        {
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverQuestion"), this.createYesNoResponses(), "GiftGiverQuestion");
          break;
        }
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
        break;
      case 70:
        Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + this.getDayOfNightMarket().ToString(), (GameLocation) this);
        break;
      case 399:
        Utility.TryOpenShopMenu("Traveler", (GameLocation) this);
        break;
      case 595:
        Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", (GameLocation) this);
        break;
      case 653:
        if (Game1.RequireLocation<Submarine>("Submarine").submerged.Value || Game1.netWorldState.Value.IsSubmarineLocked)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_SubmarineInUse"));
          return true;
        }
        break;
      case 1285:
        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_WarperQuestion"), this.createYesNoResponses(), "WarperQuestion");
        break;
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public int getDayOfNightMarket() => Utility.GetDayOfPassiveFestival("NightMarket");

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    switch (questionAndAnswer)
    {
      case null:
        return false;
      case "WarperQuestion_Yes":
        if (Game1.player.Money < 250)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
        }
        else
        {
          Game1.player.Money -= 250;
          Game1.player.CanMove = true;
          ItemRegistry.Create<Object>("(O)688").performUseAction((GameLocation) this);
          Game1.player.freezePause = 5000;
        }
        return true;
      case "PainterQuestion_Yes":
        if (Game1.player.mailReceived.Contains(this.paintingMailKey))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_PainterSold"));
          break;
        }
        if (Game1.player.Money < 1200)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
          break;
        }
        Game1.player.Money -= 1200;
        Game1.activeClickableMenu = (IClickableMenu) null;
        Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(F)" + (1838 + ((this.getDayOfNightMarket() - 1) * 2 + (Game1.year - 1) % 3 * 6)).ToString()));
        Game1.multiplayer.globalChatInfoMessage("Lupini", Game1.player.Name);
        Game1.multiplayer.broadcastPartyWideMail(this.paintingMailKey, Multiplayer.PartyWideMessageQueue.SeenMail, true);
        break;
      case "GiftGiverQuestion_Yes":
        if (this.hasReceivedFreeGift)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BeachNightMarket_GiftGiverEnjoy"));
          break;
        }
        Game1.player.freezePause = 5000;
        this.temporarySprites.Add(new TemporaryAnimatedSprite()
        {
          texture = this.shopClosedTexture,
          layerDepth = 0.2442f,
          scale = 4f,
          sourceRectStartingPos = new Vector2(354f, 168f),
          sourceRect = new Microsoft.Xna.Framework.Rectangle(354, 168, 32 /*0x20*/, 32 /*0x20*/),
          animationLength = 1,
          id = 777,
          holdLastFrame = true,
          interval = 250f,
          position = new Vector2(13f, 36f) * 64f,
          delayBeforeAnimationStart = 500,
          endFunction = new TemporaryAnimatedSprite.endBehavior(this.getFreeGiftPartOne)
        });
        this.hasReceivedFreeGift = true;
        break;
    }
    return base.answerDialogueAction(questionAndAnswer, questionParams);
  }

  public void getFreeGiftPartOne(int extra)
  {
    this.removeTemporarySpritesWithIDLocal(777);
    Game1.playSound("Milking");
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = this.shopClosedTexture,
      layerDepth = 0.2442f,
      scale = 4f,
      sourceRect = new Microsoft.Xna.Framework.Rectangle(386, 168, 32 /*0x20*/, 32 /*0x20*/),
      animationLength = 1,
      id = 778,
      holdLastFrame = true,
      interval = 9500f,
      position = new Vector2(13f, 36f) * 64f
    });
    for (int index = 0; index <= 2000; index += 100)
      this.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = this.shopClosedTexture,
        delayBeforeAnimationStart = index,
        id = 778,
        layerDepth = 0.244300008f,
        scale = 4f,
        sourceRect = new Microsoft.Xna.Framework.Rectangle(362, 170, 2, 2),
        animationLength = 1,
        interval = 100f,
        position = new Vector2(13f, 36f) * 64f + new Vector2(8f, 12f) * 4f,
        motion = new Vector2(0.0f, 2f),
        endFunction = index == 2000 ? new TemporaryAnimatedSprite.endBehavior(this.getFreeGift) : (TemporaryAnimatedSprite.endBehavior) null
      });
  }

  public void getFreeGift(int extra)
  {
    Game1.player.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(O)395"));
    this.removeTemporarySpritesWithIDLocal(778);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (force)
      this.hasShownCCUpgrade = false;
    if (Game1.RequireLocation<Beach>("Beach").bridgeFixed.Value || NetWorldState.checkAnywhereForWorldStateID("beachBridgeFixed"))
      Beach.fixBridge((GameLocation) this);
    if (!Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts"))
      return;
    Beach.showCommunityUpgradeShortcuts((GameLocation) this, ref this.hasShownCCUpgrade);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.timeOfDay >= 1700)
      Game1.changeMusicTrack("night_market");
    else
      Game1.changeMusicTrack("ocean");
    this.shopClosedTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
    this.temporarySprites.Add((TemporaryAnimatedSprite) new EmilysParrot(new Vector2(2968f, 2056f)));
    this.paintingMailKey = $"NightMarketYear{Game1.year.ToString()}Day{this.getDayOfNightMarket().ToString()}_paintingSold";
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    base.performTenMinuteUpdate(timeOfDay);
    if (timeOfDay != 1700 || !Game1.currentLocation.Equals((GameLocation) this))
      return;
    Game1.changeMusicTrack("night_market");
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = this.shopClosedTexture,
      sourceRect = new Microsoft.Xna.Framework.Rectangle(89, 164, 18, 23),
      layerDepth = 1f / 1000f,
      interval = 100f,
      position = new Vector2(19f, 31f) * 64f + new Vector2(6f, 10f) * 4f,
      scale = 4f,
      animationLength = 3
    });
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.smokeTimer -= (float) time.ElapsedGameTime.Milliseconds;
    if ((double) this.smokeTimer > 0.0)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = this.shopClosedTexture,
      sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 180, 9, 11),
      sourceRectStartingPos = new Vector2(0.0f, 180f),
      layerDepth = 1f,
      interval = 250f,
      position = new Vector2(35f, 38f) * 64f + new Vector2(9f, 6f) * 4f,
      scale = 4f,
      scaleChange = 0.005f,
      alpha = 0.75f,
      alphaFade = 0.005f,
      motion = new Vector2(0.0f, -0.5f),
      acceleration = new Vector2((float) (Game1.random.NextDouble() - 0.5) / 100f, 0.0f),
      animationLength = 3,
      holdLastFrame = true
    });
    this.smokeTimer = 1250f;
  }
}
