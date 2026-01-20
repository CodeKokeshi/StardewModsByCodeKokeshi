// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BobberBar
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class BobberBar : IClickableMenu
{
  public const int timePerFishSizeReduction = 800;
  public const int bobberTrackHeight = 548;
  public const int bobberBarTrackHeight = 568;
  public const int xOffsetToBobberTrack = 64 /*0x40*/;
  public const int yOffsetToBobberTrack = 12;
  public const int mixed = 0;
  public const int dart = 1;
  public const int smooth = 2;
  public const int sink = 3;
  public const int floater = 4;
  public const int CHALLENGE_BAIT_MAX_FISHES = 3;
  public bool handledFishResult;
  public float difficulty;
  public int motionType;
  public string whichFish;
  /// <summary>A modifier that only affects the "damage" for not having the fish in the bobber bar.</summary>
  public float distanceFromCatchPenaltyModifier = 1f;
  /// <summary>The mail flag to set for the current player when the current <see cref="F:StardewValley.Menus.BobberBar.whichFish" /> is successfully caught.</summary>
  public string setFlagOnCatch;
  public float bobberPosition = 548f;
  public float bobberSpeed;
  public float bobberAcceleration;
  public float bobberTargetPosition;
  public float scale;
  public float everythingShakeTimer;
  public float floaterSinkerAcceleration;
  public float treasurePosition;
  public float treasureCatchLevel;
  public float treasureAppearTimer;
  public float treasureScale;
  public bool bobberInBar;
  public bool buttonPressed;
  public bool flipBubble;
  public bool fadeIn;
  public bool fadeOut;
  public bool treasure;
  public bool treasureCaught;
  public bool perfect;
  public bool bossFish;
  public bool beginnersRod;
  public bool fromFishPond;
  public bool goldenTreasure;
  public int bobberBarHeight;
  public int fishSize;
  public int fishQuality;
  public int minFishSize;
  public int maxFishSize;
  public int fishSizeReductionTimer;
  public int challengeBaitFishes = -1;
  public List<string> bobbers;
  public Vector2 barShake;
  public Vector2 fishShake;
  public Vector2 everythingShake;
  public Vector2 treasureShake;
  public float reelRotation;
  private SparklingText sparkleText;
  public float bobberBarPos;
  public float bobberBarSpeed;
  public float distanceFromCatching = 0.3f;
  public static ICue reelSound;
  public static ICue unReelSound;
  private Item fishObject;

  public BobberBar(
    string whichFish,
    float fishSize,
    bool treasure,
    List<string> bobbers,
    string setFlagOnCatch,
    bool isBossFish,
    string baitID = "",
    bool goldenTreasure = false)
    : base(0, 0, 96 /*0x60*/, 636)
  {
    this.fishObject = ItemRegistry.Create(whichFish);
    this.bobbers = bobbers;
    this.setFlagOnCatch = setFlagOnCatch;
    this.handledFishResult = false;
    this.treasure = treasure;
    this.goldenTreasure = goldenTreasure;
    this.treasureAppearTimer = (float) Game1.random.Next(1000, 3000);
    this.fadeIn = true;
    this.scale = 0.0f;
    this.whichFish = whichFish;
    Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);
    this.beginnersRod = Game1.player.CurrentTool is FishingRod && Game1.player.CurrentTool.upgradeLevel.Value == 1;
    this.bobberBarHeight = 96 /*0x60*/ + Game1.player.FishingLevel * 8;
    if (Game1.player.FishingLevel < 5 && this.beginnersRod)
      this.bobberBarHeight += 40 - Game1.player.FishingLevel * 8;
    this.bossFish = isBossFish;
    string key = whichFish;
    string str;
    ref string local = ref str;
    if (dictionary.TryGetValue(key, out local))
    {
      string[] strArray = str.Split('/');
      this.difficulty = (float) Convert.ToInt32(strArray[1]);
      switch (strArray[2].ToLower())
      {
        case nameof (mixed):
          this.motionType = 0;
          break;
        case nameof (dart):
          this.motionType = 1;
          break;
        case nameof (smooth):
          this.motionType = 2;
          break;
        case nameof (floater):
          this.motionType = 4;
          break;
        case "sinker":
          this.motionType = 3;
          break;
      }
      this.minFishSize = Convert.ToInt32(strArray[3]);
      this.maxFishSize = Convert.ToInt32(strArray[4]);
      this.fishSize = (int) ((double) this.minFishSize + (double) (this.maxFishSize - this.minFishSize) * (double) fishSize);
      ++this.fishSize;
      this.perfect = true;
      this.fishQuality = (double) fishSize < 0.33 ? 0 : ((double) fishSize < 0.66 ? 1 : 2);
      this.fishSizeReductionTimer = 800;
      for (int index = 0; index < Utility.getStringCountInList(bobbers, "(O)877"); ++index)
      {
        ++this.fishQuality;
        if (this.fishQuality > 2)
          this.fishQuality = 4;
      }
      if (this.beginnersRod)
      {
        this.fishQuality = 0;
        fishSize = (float) this.minFishSize;
      }
      if (Game1.player.stats.Get("blessingOfWaters") > 0U)
      {
        if ((double) this.difficulty > 20.0)
        {
          if (isBossFish)
            this.difficulty *= 0.75f;
          else
            this.difficulty /= 2f;
        }
        this.distanceFromCatchPenaltyModifier = 0.5f;
        int num = (int) Game1.player.stats.Decrement("blessingOfWaters");
        if (Game1.player.stats.Get("blessingOfWaters") == 0U)
          Game1.player.buffs.Remove("statue_of_blessings_3");
      }
    }
    NetStringIntArrayDictionary fishCaught = Game1.player.fishCaught;
    if ((fishCaught != null ? (fishCaught.Length == 0 ? 1 : 0) : 0) != 0)
    {
      this.distanceFromCatching = 0.1f;
      if ((double) this.difficulty < 50.0)
        this.difficulty = 50f;
    }
    this.Reposition();
    this.bobberBarHeight += Utility.getStringCountInList(bobbers, "(O)695") * 24;
    if (baitID == "(O)DeluxeBait")
      this.bobberBarHeight += 12;
    this.bobberBarPos = (float) (568 - this.bobberBarHeight);
    this.bobberPosition = 508f;
    this.bobberTargetPosition = (float) ((100.0 - (double) this.difficulty) / 100.0 * 548.0);
    if (baitID == "(O)ChallengeBait")
      this.challengeBaitFishes = 3;
    Game1.setRichPresence("fishing", (object) Game1.currentLocation.Name);
  }

  public virtual void Reposition()
  {
    switch (Game1.player.FacingDirection)
    {
      case 0:
        this.xPositionOnScreen = (int) Game1.player.Position.X - 64 /*0x40*/ - 132;
        this.yPositionOnScreen = (int) Game1.player.Position.Y - 274;
        break;
      case 1:
        this.xPositionOnScreen = (int) Game1.player.Position.X - 64 /*0x40*/ - 132;
        this.yPositionOnScreen = (int) Game1.player.Position.Y - 274;
        break;
      case 2:
        this.xPositionOnScreen = (int) Game1.player.Position.X - 64 /*0x40*/ - 132;
        this.yPositionOnScreen = (int) Game1.player.Position.Y - 274;
        break;
      case 3:
        this.xPositionOnScreen = (int) Game1.player.Position.X + 128 /*0x80*/;
        this.yPositionOnScreen = (int) Game1.player.Position.Y - 274;
        this.flipBubble = true;
        break;
    }
    this.xPositionOnScreen -= Game1.viewport.X;
    this.yPositionOnScreen -= Game1.viewport.Y + 64 /*0x40*/;
    if (this.xPositionOnScreen + 96 /*0x60*/ > Game1.viewport.Width)
      this.xPositionOnScreen = Game1.viewport.Width - 96 /*0x60*/;
    else if (this.xPositionOnScreen < 0)
      this.xPositionOnScreen = 0;
    if (this.yPositionOnScreen < 0)
    {
      this.yPositionOnScreen = 0;
    }
    else
    {
      if (this.yPositionOnScreen + 636 <= Game1.viewport.Height)
        return;
      this.yPositionOnScreen = Game1.viewport.Height - 636;
    }
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    base.gameWindowSizeChanged(oldBounds, newBounds);
    this.Reposition();
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
  }

  private static int SafeNext(Random random, int minValue, int maxValue)
  {
    return minValue >= maxValue ? maxValue : random.Next(minValue, maxValue);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    this.Reposition();
    if (this.sparkleText != null && this.sparkleText.update(time))
      this.sparkleText = (SparklingText) null;
    if ((double) this.everythingShakeTimer > 0.0)
    {
      this.everythingShakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      this.everythingShake = new Vector2((float) Game1.random.Next(-10, 11) / 10f, (float) Game1.random.Next(-10, 11) / 10f);
      if ((double) this.everythingShakeTimer <= 0.0)
        this.everythingShake = Vector2.Zero;
    }
    if (this.fadeIn)
    {
      this.scale += 0.05f;
      if ((double) this.scale >= 1.0)
      {
        this.scale = 1f;
        this.fadeIn = false;
      }
    }
    else if (this.fadeOut)
    {
      if ((double) this.everythingShakeTimer > 0.0 || this.sparkleText != null)
        return;
      this.scale -= 0.05f;
      if ((double) this.scale <= 0.0)
      {
        this.scale = 0.0f;
        this.fadeOut = false;
        string qualifiedItemId = Game1.player.CurrentTool is FishingRod currentTool ? currentTool.GetBait()?.QualifiedItemId : (string) null;
        int numCaught = this.bossFish || !(qualifiedItemId == "(O)774") || Game1.random.NextDouble() >= 0.25 + Game1.player.DailyLuck / 2.0 ? 1 : 2;
        if (this.challengeBaitFishes > 0)
          numCaught = this.challengeBaitFishes;
        if ((double) this.distanceFromCatching > 0.89999997615814209 && currentTool != null)
        {
          currentTool.pullFishFromWater(this.whichFish, this.fishSize, this.fishQuality, (int) this.difficulty, this.treasureCaught, this.perfect, this.fromFishPond, this.setFlagOnCatch, this.bossFish, numCaught);
        }
        else
        {
          Game1.player.completelyStopAnimatingOrDoingAction();
          currentTool?.doneFishing(Game1.player, true);
        }
        Game1.exitActiveMenu();
        Game1.setRichPresence("location", (object) Game1.currentLocation.Name);
      }
    }
    else
    {
      if (Game1.random.NextDouble() < (double) this.difficulty * (this.motionType == 2 ? 20.0 : 1.0) / 4000.0 && (this.motionType != 2 || (double) this.bobberTargetPosition == -1.0))
      {
        float num1 = 548f - this.bobberPosition;
        float bobberPosition = this.bobberPosition;
        float num2 = Math.Min(99f, this.difficulty + (float) Game1.random.Next(10, 45)) / 100f;
        this.bobberTargetPosition = this.bobberPosition + (float) Game1.random.Next((int) Math.Min(-bobberPosition, num1), (int) num1) * num2;
      }
      switch (this.motionType)
      {
        case 3:
          this.floaterSinkerAcceleration = Math.Min(this.floaterSinkerAcceleration + 0.01f, 1.5f);
          break;
        case 4:
          this.floaterSinkerAcceleration = Math.Max(this.floaterSinkerAcceleration - 0.01f, -1.5f);
          break;
      }
      if ((double) Math.Abs(this.bobberPosition - this.bobberTargetPosition) > 3.0 && (double) this.bobberTargetPosition != -1.0)
      {
        this.bobberAcceleration = (float) (((double) this.bobberTargetPosition - (double) this.bobberPosition) / ((double) Game1.random.Next(10, 30) + (100.0 - (double) Math.Min(100f, this.difficulty))));
        this.bobberSpeed += (float) (((double) this.bobberAcceleration - (double) this.bobberSpeed) / 5.0);
      }
      else
        this.bobberTargetPosition = this.motionType == 2 || Game1.random.NextDouble() >= (double) this.difficulty / 2000.0 ? -1f : this.bobberPosition + (Game1.random.NextBool() ? (float) Game1.random.Next(-100, -51) : (float) Game1.random.Next(50, 101));
      if (this.motionType == 1 && Game1.random.NextDouble() < (double) this.difficulty / 1000.0)
        this.bobberTargetPosition = this.bobberPosition + (Game1.random.NextBool() ? (float) BobberBar.SafeNext(Game1.random, -100 - (int) this.difficulty * 2, -51) : (float) BobberBar.SafeNext(Game1.random, 50, 101 + (int) this.difficulty * 2));
      this.bobberTargetPosition = Math.Max(-1f, Math.Min(this.bobberTargetPosition, 548f));
      this.bobberPosition += this.bobberSpeed + this.floaterSinkerAcceleration;
      if ((double) this.bobberPosition > 532.0)
        this.bobberPosition = 532f;
      else if ((double) this.bobberPosition < 0.0)
        this.bobberPosition = 0.0f;
      this.bobberInBar = (double) this.bobberPosition + 12.0 <= (double) this.bobberBarPos - 32.0 + (double) this.bobberBarHeight && (double) this.bobberPosition - 16.0 >= (double) this.bobberBarPos - 32.0;
      if ((double) this.bobberPosition >= (double) (548 - this.bobberBarHeight) && (double) this.bobberBarPos >= (double) (568 - this.bobberBarHeight - 4))
        this.bobberInBar = true;
      int num3 = this.buttonPressed ? 1 : 0;
      this.buttonPressed = Game1.oldMouseState.LeftButton == ButtonState.Pressed || Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.X) || Game1.oldPadState.IsButtonDown(Buttons.A));
      if (num3 == 0 && this.buttonPressed)
        Game1.playSound("fishingRodBend");
      float num4 = this.buttonPressed ? -0.25f : 0.25f;
      if (this.buttonPressed && (double) num4 < 0.0 && ((double) this.bobberBarPos == 0.0 || (double) this.bobberBarPos == (double) (568 - this.bobberBarHeight)))
        this.bobberBarSpeed = 0.0f;
      if (this.bobberInBar)
      {
        num4 *= this.bobbers.Contains("(O)691") ? 0.3f : 0.6f;
        if (this.bobbers.Contains("(O)691"))
        {
          for (int index = 0; index < Utility.getStringCountInList(this.bobbers, "(O)691"); ++index)
          {
            if ((double) this.bobberPosition + 16.0 < (double) this.bobberBarPos + (double) (this.bobberBarHeight / 2))
              this.bobberBarSpeed -= index > 0 ? 0.05f : 0.2f;
            else
              this.bobberBarSpeed += index > 0 ? 0.05f : 0.2f;
            if (index > 0)
              num4 *= 0.9f;
          }
        }
      }
      float bobberBarPos = this.bobberBarPos;
      this.bobberBarSpeed += num4;
      this.bobberBarPos += this.bobberBarSpeed;
      if ((double) this.bobberBarPos + (double) this.bobberBarHeight > 568.0)
      {
        this.bobberBarPos = (float) (568 - this.bobberBarHeight);
        this.bobberBarSpeed = (float) (-(double) this.bobberBarSpeed * 2.0 / 3.0 * (this.bobbers.Contains("(O)692") ? (double) Utility.getStringCountInList(this.bobbers, "(O)692") * 0.10000000149011612 : 1.0));
        if ((double) bobberBarPos + (double) this.bobberBarHeight < 568.0)
          Game1.playSound("shiny4");
      }
      else if ((double) this.bobberBarPos < 0.0)
      {
        this.bobberBarPos = 0.0f;
        this.bobberBarSpeed = (float) (-(double) this.bobberBarSpeed * 2.0 / 3.0);
        if ((double) bobberBarPos > 0.0)
          Game1.playSound("shiny4");
      }
      bool flag = false;
      TimeSpan elapsedGameTime;
      if (this.treasure)
      {
        float treasureAppearTimer1 = this.treasureAppearTimer;
        double treasureAppearTimer2 = (double) this.treasureAppearTimer;
        elapsedGameTime = time.ElapsedGameTime;
        double milliseconds = (double) elapsedGameTime.Milliseconds;
        this.treasureAppearTimer = (float) (treasureAppearTimer2 - milliseconds);
        if ((double) this.treasureAppearTimer <= 0.0)
        {
          if ((double) this.treasureScale < 1.0 && !this.treasureCaught)
          {
            if ((double) treasureAppearTimer1 > 0.0)
            {
              if ((double) this.bobberBarPos > 274.0)
              {
                this.treasurePosition = (float) Game1.random.Next(8, (int) this.bobberBarPos - 20);
              }
              else
              {
                int minValue = Math.Min(528, (int) this.bobberBarPos + this.bobberBarHeight);
                int maxValue = 500;
                this.treasurePosition = minValue > maxValue ? (float) (maxValue - 1) : (float) Game1.random.Next(minValue, maxValue);
              }
              Game1.playSound("dwop");
            }
            this.treasureScale = Math.Min(1f, this.treasureScale + 0.1f);
          }
          flag = (double) this.treasurePosition + 12.0 <= (double) this.bobberBarPos - 32.0 + (double) this.bobberBarHeight && (double) this.treasurePosition - 16.0 >= (double) this.bobberBarPos - 32.0;
          if (flag && !this.treasureCaught)
          {
            this.treasureCatchLevel += 0.0135f;
            this.treasureShake = new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-2, 3));
            if ((double) this.treasureCatchLevel >= 1.0)
            {
              Game1.playSound("newArtifact");
              this.treasureCaught = true;
            }
          }
          else if (this.treasureCaught)
          {
            this.treasureScale = Math.Max(0.0f, this.treasureScale - 0.1f);
          }
          else
          {
            this.treasureShake = Vector2.Zero;
            this.treasureCatchLevel = Math.Max(0.0f, this.treasureCatchLevel - 0.01f);
          }
        }
      }
      if (this.bobberInBar)
      {
        this.distanceFromCatching += 1f / 500f;
        this.reelRotation += 0.3926991f;
        this.fishShake.X = (float) Game1.random.Next(-10, 11) / 10f;
        this.fishShake.Y = (float) Game1.random.Next(-10, 11) / 10f;
        this.barShake = Vector2.Zero;
        Rumble.rumble(0.1f, 1000f);
        BobberBar.unReelSound?.Stop(AudioStopOptions.Immediate);
        if (BobberBar.reelSound == null || BobberBar.reelSound.IsStopped || BobberBar.reelSound.IsStopping || !BobberBar.reelSound.IsPlaying)
          Game1.playSound("fastReel", out BobberBar.reelSound);
      }
      else if (!flag || this.treasureCaught || !this.bobbers.Contains("(O)693"))
      {
        if (!this.fishShake.Equals(Vector2.Zero))
        {
          Game1.playSound("tinyWhip");
          this.perfect = false;
          Rumble.stopRumbling();
          if (this.challengeBaitFishes > 0)
          {
            --this.challengeBaitFishes;
            if (this.challengeBaitFishes <= 0)
              this.distanceFromCatching = 0.0f;
          }
        }
        int sizeReductionTimer = this.fishSizeReductionTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds = elapsedGameTime.Milliseconds;
        this.fishSizeReductionTimer = sizeReductionTimer - milliseconds;
        if (this.fishSizeReductionTimer <= 0)
        {
          this.fishSize = Math.Max(this.minFishSize, this.fishSize - 1);
          this.fishSizeReductionTimer = 800;
        }
        if (Game1.player.fishCaught != null && Game1.player.fishCaught.Length != 0 || Game1.currentMinigame != null)
        {
          if (this.bobbers.Contains("(O)694"))
          {
            float val2 = 3f / 1000f;
            float num5 = 1f / 1000f;
            for (int index = 0; index < Utility.getStringCountInList(this.bobbers, "(O)694"); ++index)
            {
              val2 -= num5;
              num5 /= 2f;
            }
            this.distanceFromCatching -= Math.Max(1f / 1000f, val2) * this.distanceFromCatchPenaltyModifier;
          }
          else
            this.distanceFromCatching -= (this.beginnersRod ? 1f / 500f : 3f / 1000f) * this.distanceFromCatchPenaltyModifier;
        }
        this.reelRotation -= 3.14159274f / Math.Max(10f, 200f - Math.Abs(this.bobberPosition - (this.bobberBarPos + (float) (this.bobberBarHeight / 2))));
        this.barShake.X = (float) Game1.random.Next(-10, 11) / 10f;
        this.barShake.Y = (float) Game1.random.Next(-10, 11) / 10f;
        this.fishShake = Vector2.Zero;
        BobberBar.reelSound?.Stop(AudioStopOptions.Immediate);
        if (BobberBar.unReelSound == null || BobberBar.unReelSound.IsStopped)
          Game1.playSound("slowReel", 600, out BobberBar.unReelSound);
      }
      this.distanceFromCatching = Math.Max(0.0f, Math.Min(1f, this.distanceFromCatching));
      if (Game1.player.CurrentTool != null)
        Game1.player.CurrentTool.tickUpdate(time, Game1.player);
      if ((double) this.distanceFromCatching <= 0.0)
      {
        this.fadeOut = true;
        this.everythingShakeTimer = 500f;
        Game1.playSound("fishEscape");
        this.handledFishResult = true;
        BobberBar.unReelSound?.Stop(AudioStopOptions.Immediate);
        BobberBar.reelSound?.Stop(AudioStopOptions.Immediate);
      }
      else if ((double) this.distanceFromCatching >= 1.0)
      {
        this.everythingShakeTimer = 500f;
        Game1.playSound("jingle1");
        this.fadeOut = true;
        this.handledFishResult = true;
        BobberBar.unReelSound?.Stop(AudioStopOptions.Immediate);
        BobberBar.reelSound?.Stop(AudioStopOptions.Immediate);
        if (this.perfect)
        {
          this.sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:BobberBar_Perfect"), Color.Yellow, Color.White, millisecondsDuration: 1500);
          if (Game1.isFestival())
            Game1.CurrentEvent.perfectFishing();
        }
        else if (this.fishSize == this.maxFishSize)
          --this.fishSize;
      }
    }
    if ((double) this.bobberPosition < 0.0)
      this.bobberPosition = 0.0f;
    if ((double) this.bobberPosition <= 548.0)
      return;
    this.bobberPosition = 548f;
  }

  public override bool readyToClose() => false;

  public override void emergencyShutDown()
  {
    base.emergencyShutDown();
    BobberBar.unReelSound?.Stop(AudioStopOptions.Immediate);
    BobberBar.reelSound?.Stop(AudioStopOptions.Immediate);
    if (!this.handledFishResult)
      Game1.playSound("fishEscape");
    this.fadeOut = true;
    this.everythingShakeTimer = 500f;
    this.distanceFromCatching = -1f;
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    if (!((IEnumerable<InputButton>) Game1.options.menuButton).Contains<InputButton>(new InputButton(key)))
      return;
    this.emergencyShutDown();
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    Game1.StartWorldDrawInUI(b);
    b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen - (this.flipBubble ? 44 : 20) + 104), (float) (this.yPositionOnScreen - 16 /*0x10*/ + 314)) + this.everythingShake, new Rectangle?(new Rectangle(652, 1685, 52, 157)), Color.White * 0.6f * this.scale, 0.0f, new Vector2(26f, 78.5f) * this.scale, 4f * this.scale, this.flipBubble ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f / 1000f);
    b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 70), (float) (this.yPositionOnScreen + 296)) + this.everythingShake, new Rectangle?(new Rectangle(644, 1999, 38, 150)), Color.White * this.scale, 0.0f, new Vector2(18.5f, 74f) * this.scale, 4f * this.scale, SpriteEffects.None, 0.01f);
    if ((double) this.scale == 1.0)
    {
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/), (float) (this.yPositionOnScreen + 12 + (int) this.bobberBarPos)) + this.barShake + this.everythingShake, new Rectangle?(new Rectangle(682, 2078, 9, 2)), this.bobberInBar ? Color.White : Color.White * 0.25f * (float) (Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/), (float) (this.yPositionOnScreen + 12 + (int) this.bobberBarPos + 8)) + this.barShake + this.everythingShake, new Rectangle?(new Rectangle(682, 2081, 9, 1)), this.bobberInBar ? Color.White : Color.White * 0.25f * (float) (Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2.0), 0.0f, Vector2.Zero, new Vector2(4f, (float) (this.bobberBarHeight - 16 /*0x10*/)), SpriteEffects.None, 0.89f);
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/), (float) (this.yPositionOnScreen + 12 + (int) this.bobberBarPos + this.bobberBarHeight - 8)) + this.barShake + this.everythingShake, new Rectangle?(new Rectangle(682, 2085, 9, 2)), this.bobberInBar ? Color.White : Color.White * 0.25f * (float) (Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2.0), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
      b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 4 + (int) (580.0 * (1.0 - (double) this.distanceFromCatching)), 16 /*0x10*/, (int) (580.0 * (double) this.distanceFromCatching)), Utility.getRedToGreenLerpColor(this.distanceFromCatching));
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 18), (float) (this.yPositionOnScreen + 514)) + this.everythingShake, new Rectangle?(new Rectangle(257, 1990, 5, 10)), Color.White, this.reelRotation, new Vector2(2f, 10f), 4f, SpriteEffects.None, 0.9f);
      if (this.goldenTreasure)
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/ + 18), (float) (this.yPositionOnScreen + 12 + 24) + this.treasurePosition) + this.treasureShake + this.everythingShake, new Rectangle?(new Rectangle(256 /*0x0100*/, 51, 20, 24)), Color.White, 0.0f, new Vector2(10f, 10f), 2f * this.treasureScale, SpriteEffects.None, 0.85f);
      else
        b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/ + 18), (float) (this.yPositionOnScreen + 12 + 24) + this.treasurePosition) + this.treasureShake + this.everythingShake, new Rectangle?(new Rectangle(638, 1865, 20, 24)), Color.White, 0.0f, new Vector2(10f, 10f), 2f * this.treasureScale, SpriteEffects.None, 0.85f);
      if ((double) this.treasureCatchLevel > 0.0 && !this.treasureCaught)
      {
        b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64 /*0x40*/, this.yPositionOnScreen + 12 + (int) this.treasurePosition, 40, 8), Color.DimGray * 0.5f);
        b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64 /*0x40*/, this.yPositionOnScreen + 12 + (int) this.treasurePosition, (int) ((double) this.treasureCatchLevel * 40.0), 8), Color.Orange);
      }
      b.Draw(Game1.mouseCursors, new Vector2((float) (this.xPositionOnScreen + 64 /*0x40*/ + 18), (float) (this.yPositionOnScreen + 12 + 24) + this.bobberPosition) + this.fishShake + this.everythingShake, new Rectangle?(new Rectangle(614 + (this.bossFish ? 20 : 0), 1840, 20, 20)), Color.White, 0.0f, new Vector2(10f, 10f), 2f, SpriteEffects.None, 0.88f);
      this.sparkleText?.draw(b, new Vector2((float) (this.xPositionOnScreen - 16 /*0x10*/), (float) (this.yPositionOnScreen - 64 /*0x40*/)));
      if (this.bobbers.Contains("(O)SonarBobber"))
      {
        int x = (double) this.xPositionOnScreen > (double) Game1.viewport.Width * 0.75 ? this.xPositionOnScreen - 80 /*0x50*/ : this.xPositionOnScreen + 216;
        bool flag = x < this.xPositionOnScreen;
        b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (x - 12), (float) (this.yPositionOnScreen + 40)) + this.everythingShake, new Rectangle?(new Rectangle(227, 6, 29, 24)), Color.White, 0.0f, new Vector2(10f, 10f), 4f, flag ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.88f);
        this.fishObject.drawInMenu(b, new Vector2((float) x, (float) this.yPositionOnScreen) + new Vector2(flag ? -8f : -4f, 4f) * 4f + this.everythingShake, 1f);
      }
      if (this.challengeBaitFishes > -1)
      {
        int num1 = (double) this.xPositionOnScreen > (double) Game1.viewport.Width * 0.75 ? this.xPositionOnScreen - 80 /*0x50*/ : this.xPositionOnScreen + 216;
        int num2 = this.bobbers.Contains("(O)SonarBobber") ? this.yPositionOnScreen + 136 : this.yPositionOnScreen + 40;
        Utility.drawWithShadow(b, Game1.mouseCursors_1_6, new Vector2((float) (num1 - 24) + this.everythingShake.X, (float) (num2 - 16 /*0x10*/) + this.everythingShake.Y), new Rectangle(240 /*0xF0*/, 31 /*0x1F*/, 15, 38), Color.White, 0.0f, Vector2.Zero, 4f);
        for (int index = 0; index < 3; ++index)
        {
          if (index < this.challengeBaitFishes)
            Utility.drawWithShadow(b, Game1.mouseCursors_1_6, new Vector2((float) (num1 - 12), (float) num2 + (float) (index * 20) * 2f) + this.everythingShake, new Rectangle(236, 205, 19, 19), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 2f, layerDepth: 0.88f);
          else
            b.Draw(Game1.mouseCursors_1_6, new Vector2((float) (num1 - 12), (float) num2 + (float) (index * 20) * 2f) + this.everythingShake, new Rectangle?(new Rectangle(217, 205, 19, 19)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 2f, SpriteEffects.None, 0.88f);
        }
      }
    }
    NetStringIntArrayDictionary fishCaught = Game1.player.fishCaught;
    if ((fishCaught != null ? (fishCaught.Length == 0 ? 1 : 0) : 0) != 0)
    {
      Vector2 position = new Vector2((float) (this.xPositionOnScreen + (this.flipBubble ? this.width + 64 /*0x40*/ + 8 : -200)), (float) (this.yPositionOnScreen + 192 /*0xC0*/));
      if (!Game1.options.gamepadControls)
        b.Draw(Game1.mouseCursors, position, new Rectangle?(new Rectangle(644, 1330, 48 /*0x30*/, 69)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
      else
        b.Draw(Game1.controllerMaps, position, new Rectangle?(Utility.controllerMapSourceRect(new Rectangle(681, 0, 96 /*0x60*/, 138))), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
    }
    Game1.EndWorldDrawInUI(b);
  }
}
