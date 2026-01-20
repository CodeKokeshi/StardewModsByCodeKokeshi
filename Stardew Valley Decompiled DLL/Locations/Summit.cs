// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Summit
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Pets;
using StardewValley.Monsters;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Locations;

public class Summit : GameLocation
{
  private ICue wind;
  private float windGust;
  private float globalWind = -0.25f;
  [XmlIgnore]
  public bool isShowingEndSlideshow;

  public Summit()
  {
  }

  public Summit(string map, string name)
    : base(map, name)
  {
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (Game1.random.NextDouble() < 0.005 || (double) this.globalWind >= 1.0 || (double) this.globalWind <= 0.34999999403953552)
      this.windGust = (double) this.globalWind >= 0.34999999403953552 ? ((double) this.globalWind <= 0.75 ? (float) (Game1.random.Choose<int>(-1, 1) * Game1.random.Next(4, 6)) / 2000f : (float) -Game1.random.Next(2, 6) / 2000f) : (float) Game1.random.Next(3, 6) / 2000f;
    if (this.wind != null)
    {
      this.globalWind += this.windGust;
      this.globalWind = Utility.Clamp(this.globalWind, -0.5f, 1f);
      this.wind.SetVariable("Volume", Math.Abs(this.globalWind) * 60f);
      this.wind.SetVariable("Frequency", this.globalWind * 100f);
      Game1.sounds.SetPitch(this.wind, (float) (1200.0 + (double) Math.Abs(this.globalWind) * 1200.0));
    }
    if (Game1.background != null && Game1.background.cursed)
    {
      if (Game1.random.NextDouble() < 0.01)
      {
        Game1.playSound(Game1.random.Choose<string>("coin", "slimeHit", "squid_hit", "skeletonStep", "rabbit", "pig", "gulp"));
        if (Game1.options.screenFlash)
          Game1.background.c = Utility.getBlendedColor(Utility.getRandomRainbowColor(), Color.Black);
      }
      if (Game1.background.c.R > (byte) 0)
        --Game1.background.c.R;
      if (Game1.background.c.G > (byte) 0)
        --Game1.background.c.G;
      if (Game1.background.c.B > (byte) 0)
        --Game1.background.c.B;
    }
    base.UpdateWhenCurrentLocation(time);
    Season season = this.GetSeason();
    if (this.currentEvent == null && Game1.background != null && !Game1.background.cursed && this.temporarySprites.Count == 0 && Game1.random.NextDouble() < (Game1.timeOfDay >= 1800 ? (Game1.season != Season.Summer || Game1.dayOfMonth != 20 ? 0.001 : 1.0) : 0.0006))
    {
      Rectangle sourceRect = Rectangle.Empty;
      Vector2 position = new Vector2((float) Game1.viewport.Width, (float) Game1.random.Next(10, Game1.viewport.Height / 2));
      float x = -4f;
      int numberOfLoops = 200;
      float animationInterval = 100f;
      if (Game1.timeOfDay < 1800)
      {
        switch (season)
        {
          case Season.Spring:
          case Season.Fall:
            sourceRect = new Rectangle(640, 736, 16 /*0x10*/, 16 /*0x10*/);
            int num = Game1.random.Next(1, 4);
            x = -1f;
            for (int index = 0; index < num; ++index)
            {
              TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80 /*0x50*/, 121), 4, 200, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * -20)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite1.motion = new Vector2(-1f, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite1);
              TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80 /*0x50*/, 121), 4, 200, position + new Vector2((float) ((index + 1) * Game1.random.Next(15, 18)), (float) ((index + 1) * 20)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              temporaryAnimatedSprite2.motion = new Vector2(-1f, 0.0f);
              this.temporarySprites.Add(temporaryAnimatedSprite2);
            }
            break;
          case Season.Summer:
            sourceRect = new Rectangle(640, 752 + Game1.random.Choose<int>(16 /*0x10*/, 0), 16 /*0x10*/, 16 /*0x10*/);
            x = -0.5f;
            animationInterval = 150f;
            break;
        }
        if (Game1.random.NextDouble() < 0.25)
        {
          TemporaryAnimatedSprite temporaryAnimatedSprite;
          switch (season)
          {
            case Season.Spring:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(0, 302, 26, 18), (float) Game1.random.Next(80 /*0x50*/, 121), 4, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f,
                pingPong = true
              };
              break;
            case Season.Summer:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(1, 165, 24, 21), (float) Game1.random.Next(60, 80 /*0x50*/), 6, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f
              };
              break;
            case Season.Fall:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("TileSheets\\critters", new Rectangle(0, 64 /*0x40*/, 32 /*0x20*/, 32 /*0x20*/), (float) Game1.random.Next(60, 80 /*0x50*/), 5, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f,
                pingPong = true
              };
              break;
            case Season.Winter:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(104, 302, 26, 18), (float) Game1.random.Next(80 /*0x50*/, 121), 4, 200, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
              {
                layerDepth = 0.0f,
                pingPong = true
              };
              break;
            default:
              temporaryAnimatedSprite = new TemporaryAnimatedSprite();
              break;
          }
          temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
          this.temporarySprites.Add(temporaryAnimatedSprite);
        }
        else if (Game1.random.NextDouble() < 0.15 && Game1.stats.Get("childrenTurnedToDoves") > 1U)
        {
          for (int index = 0; (long) index < (long) Game1.stats.Get("childrenTurnedToDoves"); ++index)
          {
            sourceRect = Rectangle.Empty;
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(388, 1894, 24, 21), (float) Game1.random.Next(80 /*0x50*/, 121), 6, 200, position + new Vector2((float) ((index + 1) * (Game1.random.Next(25, 27) * 4)), (float) (Game1.random.Next(-32, 33) * 4)), false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              layerDepth = 0.0f
            };
            temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
            this.temporarySprites.Add(temporaryAnimatedSprite);
          }
        }
        if (Game1.MasterPlayer.eventsSeen.Contains("571102") && Game1.random.NextDouble() < 0.1)
        {
          sourceRect = Rectangle.Empty;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(222, 1890, 20, 9), 30f, 2, 99900, position, false, false, 0.01f, 0.0f, Color.White, 2f, 0.0f, 0.0f, 0.0f, true)
          {
            yPeriodic = true,
            yPeriodicLoopTime = 4000f,
            yPeriodicRange = 8f,
            layerDepth = 0.0f
          };
          temporaryAnimatedSprite.motion = new Vector2(-3f, 0.0f);
          this.temporarySprites.Add(temporaryAnimatedSprite);
        }
        if (Game1.MasterPlayer.eventsSeen.Contains("10") && Game1.random.NextDouble() < 0.05)
        {
          sourceRect = Rectangle.Empty;
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(206, 1827, 15, 25), 30f, 4, 99900, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            rotation = -1.04719758f,
            layerDepth = 0.0f
          };
          temporaryAnimatedSprite.motion = new Vector2(-4f, -0.5f);
          this.temporarySprites.Add(temporaryAnimatedSprite);
        }
      }
      else if (Game1.timeOfDay >= 1900)
      {
        sourceRect = new Rectangle(640, 816, 16 /*0x10*/, 16 /*0x10*/);
        x = -2f;
        numberOfLoops = 0;
        position.X -= (float) Game1.random.Next(64 /*0x40*/, Game1.viewport.Width);
        if (season == Season.Summer && Game1.dayOfMonth == 20)
        {
          int num = Game1.random.Next(3);
          for (int index = 0; index < num; ++index)
          {
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, (float) Game1.random.Next(80 /*0x50*/, 121), 4, numberOfLoops, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              layerDepth = 0.0f
            };
            temporaryAnimatedSprite.motion = new Vector2(x, 0.0f);
            this.temporarySprites.Add(temporaryAnimatedSprite);
            position.X -= (float) Game1.random.Next(64 /*0x40*/, Game1.viewport.Width);
            position.Y = (float) Game1.random.Next(0, 200);
          }
        }
        else if (Game1.season == Season.Winter)
        {
          if (Game1.timeOfDay >= 1700 && Game1.random.NextDouble() < 0.1)
          {
            sourceRect = new Rectangle(640, 800, 32 /*0x20*/, 16 /*0x10*/);
            numberOfLoops = 1000;
            position.X = (float) Game1.viewport.Width;
          }
          else
            sourceRect = Rectangle.Empty;
        }
      }
      if (Game1.timeOfDay >= 2200 && Game1.season == Season.Summer && Game1.dayOfMonth == 20 && Game1.random.NextDouble() < 0.05)
      {
        sourceRect = new Rectangle(640, 784, 16 /*0x10*/, 16 /*0x10*/);
        numberOfLoops = 200;
        position.X = (float) Game1.viewport.Width;
        x = -3f;
      }
      if (sourceRect != Rectangle.Empty && Game1.viewport.X > -10000)
      {
        TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, animationInterval, season == Season.Winter ? 2 : 4, numberOfLoops, position, false, false, 0.01f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          layerDepth = 0.0f
        };
        temporaryAnimatedSprite.motion = new Vector2(x, 0.0f);
        this.temporarySprites.Add(temporaryAnimatedSprite);
      }
    }
    if (Game1.viewport.X > -10000)
    {
      foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
      {
        temporarySprite.position.Y -= (float) (((double) Game1.viewport.Y - (double) Game1.previousViewportPosition.Y) / 8.0);
        temporarySprite.drawAboveAlwaysFront = true;
      }
    }
    if (Game1.eventUp)
    {
      foreach (TemporaryAnimatedSprite temporarySprite in this.temporarySprites)
        temporarySprite.attachedCharacter?.animateInFacingDirection(time);
    }
    else
      this.isShowingEndSlideshow = false;
  }

  public override void cleanupBeforePlayerExit()
  {
    this.isShowingEndSlideshow = false;
    base.cleanupBeforePlayerExit();
    Game1.background = (Background) null;
    Game1.displayHUD = true;
    this.wind?.Stop(AudioStopOptions.Immediate);
  }

  protected override void resetLocalState()
  {
    if (!Game1.player.team.farmPerfect.Value)
    {
      Game1.background = new Background(this);
      Game1.background.cursed = true;
      Game1.background.c = Color.Red;
      this.showQiCheatingEvent();
    }
    else
    {
      Game1.getAchievement(44);
      this.isShowingEndSlideshow = false;
      this.isOutdoors.Value = false;
      base.resetLocalState();
      Game1.background = new Background(this);
      this.temporarySprites.Clear();
      Game1.displayHUD = false;
      Game1.changeMusicTrack("winter_day_ambient", true, MusicContext.SubLocation);
      Game1.playSound("wind", out this.wind);
      this.globalWind = 0.0f;
      this.windGust = 1f / 1000f;
      if (Game1.player.mailReceived.Contains("Summit_event") || !Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
        return;
      string summitEvent = this.getSummitEvent();
      if (!(summitEvent != ""))
        return;
      Game1.player.songsHeard.Add("end_credits");
      Game1.player.mailReceived.Add("Summit_event");
      this.startEvent(new Event(summitEvent));
    }
  }

  public string GetSummitDialogue(string file, string key)
  {
    NPC spouse = Game1.player.getSpouse();
    string path = $"Data\\{file}:{key}";
    return !(spouse?.Name == "Penny") ? Game1.content.LoadString(path, (object) "") : Game1.content.LoadString(path, (object) "요");
  }

  private void showQiCheatingEvent()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (Game1.player.mailReceived.Contains("summit_cheat_event"))
    {
      Game1.player.health = -1;
    }
    else
    {
      stringBuilder.Append("winter_day_ambient/-1000 -1000/farmer 9 23 0 MrQi 11 13 0/viewport 11 13 clamp true/move farmer 0 -10 0/faceDirection MrQi 3/speak MrQi \"");
      stringBuilder.Append(Game1.content.LoadString("Strings\\1_6_Strings:QiSummitCheat") + "\"/faceDirection MrQi 0/pause 1000/playMusic none/pause 1000/speed MrQi 8/move MrQi -1 0 3/faceDirection farmer 2 true/animate farmer false true 100 94/startJittering/viewport -1000 -1000 true/end qiSummitCheat");
      this.startEvent(new Event(stringBuilder.ToString()));
      Game1.player.mailReceived.Add("summit_cheat_event");
    }
  }

  private string getSummitEvent()
  {
    StringBuilder stringBuilder = new StringBuilder();
    try
    {
      stringBuilder.Append("winter_day_ambient/-1000 -1000/farmer 9 23 0 ");
      NPC spouse = Game1.player.getSpouse();
      if (spouse != null && spouse.Name != "Krobus")
      {
        string name = spouse.Name;
        stringBuilder.Append(name).Append(" 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/move farmer 1 0 0/pause 2000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Spouse")).Append("\"/viewport move 0 -1 4000/pause 5000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro2_Spouse" + (this.sayGrufferSummitIntro(spouse) ? "_Gruff" : ""))).Append("\"/pause 400/emote farmer 56/pause 2000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Spouse")).Append("\"/pause 1000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1B_Spouse")).Append("\"/pause 2000/faceDirection ").Append(name).Append(" 3/faceDirection farmer 1/pause 1000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Spouse")).Append("\"/pause 2000/faceDirection ").Append(name).Append(" 0/faceDirection farmer 0/pause 2000/speak ").Append(name).Append(" \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue3_" + name));
        if (!stringBuilder.ToString()[stringBuilder.Length - 1].Equals('"'))
          stringBuilder.Append("\"");
        stringBuilder.Append("/emote farmer 20/pause 500/faceDirection farmer 1/faceDirection ").Append(name).Append(" 3/pause 1500/animate farmer false true 100 101/showKissFrame ").Append(name).Append("/playSound dwop/positionOffset farmer 8 0/positionOffset ").Append(name).Append(" -4 0/specificTemporarySprite heart 11 12/pause 10");
      }
      else if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
        stringBuilder.Append("Morris 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/pause 2000/speak Morris \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Morris")).Append("\"/viewport move 0 -1 4000/pause 5000/speak Morris \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Morris")).Append("\"/pause 2000/faceDirection Morris 3/speak Morris \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Morris")).Append("\"/pause 2000/faceDirection Morris 0/speak Morris \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Outro_Morris")).Append("\"/emote farmer 20/pause 10");
      else
        stringBuilder.Append("Lewis 11 13 0/skippable/viewport 10 17 clamp true/pause 2000/viewport move 0 -1 4000/move farmer 0 -10 0/pause 2000/speak Lewis \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Intro_Lewis")).Append("\"/viewport move 0 -1 4000/pause 5000/speak Lewis \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue1_Lewis")).Append("\"/pause 2000/faceDirection Lewis 3/speak Lewis \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Dialogue2_Lewis")).Append("\"/pause 2000/faceDirection Lewis 0/speak Lewis \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_Outro_Lewis")).Append("\"/pause 10");
      int num = 35000;
      if (Game1.player.mailReceived.Contains("Capsule_Broken"))
        num += 8000;
      if (Game1.player.totalMoneyEarned >= 100000000U)
        num += 8000;
      if (Game1.year <= 2)
        num += 8000;
      stringBuilder.Append("/playMusic moonlightJellies/pause 2000/specificTemporarySprite krobusraven/viewport move 0 -1 12000/pause 10/pause ").Append(num).Append("/pause 2000/playMusic none/viewport move 0 -1 5000/fade/playMusic end_credits/viewport -8000 -8000 true/removeTemporarySprites/specificTemporarySprite getEndSlideshow/pause 1000/playMusic none/pause 500").Append("/playMusic grandpas_theme/pause 2000/fade/viewport -3000 -2000/specificTemporarySprite doneWithSlideShow/removeTemporarySprites/pause 3000/addTemporaryActor MrQi 16 32 -998 -1000 2 true/addTemporaryActor Grandpa 1 1 -100 -100 2 true/specificTemporarySprite grandpaSpirit/viewport -1000 -1000 true/pause 6000/spriteText 3 \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage")).Append(" \"/spriteText 3 \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage2")).Append(" \"/spriteText 3 \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage3")).Append(" \"/spriteText 3 \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage4")).Append(" \"/spriteText 7 \"").Append(this.GetSummitDialogue("ExtraDialogue", "SummitEvent_closingmessage5")).Append(" \"/pause 400/playSound dwop/showFrame MrQi 1/pause 100/showFrame MrQi 2/pause 100/showFrame MrQi 3/pause 400/specificTemporarySprite grandpaThumbsUp/pause 10000/end");
    }
    catch (Exception ex)
    {
      return "";
    }
    return stringBuilder.ToString();
  }

  public string getEndSlideshow()
  {
    StringBuilder stringBuilder = new StringBuilder();
    int delayBeforeAnimationStart1 = 0;
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      string key = keyValuePair.Key;
      CharacterData data = keyValuePair.Value;
      if (data.EndSlideShow == EndSlideShowBehavior.MainGroup && this.TryDrawNpc(key, data, 90, delayBeforeAnimationStart1))
        delayBeforeAnimationStart1 += 500;
    }
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 2000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 488), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 2000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 976), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 2000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\night_market_tilesheet_objects", new Rectangle(586, 119, 122, 28), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 1464), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 2000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(324, 1936, 12, 20), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.40000000596046448 + 192.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 14000,
      startSound = "dogWhining"
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(43, 80 /*0x50*/, 51, 56), 90f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-1f, -4f),
      delayBeforeAnimationStart = 27000,
      startSound = "trashbear",
      drawAboveAlwaysFront = true
    });
    stringBuilder.Append("pause 10/spriteText 5 \"").Append(Utility.loadStringShort("UI", "EndCredit_Neighbors")).Append(" \"/pause 30000/");
    int delayBeforeAnimationStart2 = delayBeforeAnimationStart1 + 4000;
    int num1 = delayBeforeAnimationStart2;
    foreach (KeyValuePair<string, CharacterData> keyValuePair in (IEnumerable<KeyValuePair<string, CharacterData>>) Game1.characterData)
    {
      string key = keyValuePair.Key;
      CharacterData data = keyValuePair.Value;
      if (data.EndSlideShow == EndSlideShowBehavior.TrailingGroup && this.TryDrawNpc(key, data, 120, delayBeforeAnimationStart2))
        delayBeforeAnimationStart2 += 500;
    }
    int num2 = delayBeforeAnimationStart2 + 5000;
    stringBuilder.Append("spriteText 4 \"").Append(Utility.loadStringShort("UI", "EndCredit_Animals")).Append(" \"/pause ").Append(num2 - num1 + 22000);
    int num3 = num2;
    foreach (KeyValuePair<string, FarmAnimalData> keyValuePair in (IEnumerable<KeyValuePair<string, FarmAnimalData>>) Game1.farmAnimalData)
    {
      string key = keyValuePair.Key;
      FarmAnimalData farmAnimalData = keyValuePair.Value;
      if (farmAnimalData.ShowInSummitCredits)
      {
        int spriteWidth = farmAnimalData.SpriteWidth;
        int spriteHeight = farmAnimalData.SpriteHeight;
        int num4 = 0;
        this.TemporarySprites.Add(new TemporaryAnimatedSprite(farmAnimalData.Texture, new Rectangle(0, spriteHeight, spriteWidth, spriteHeight), 120f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) (int) ((double) Game1.viewport.Height * 0.5 - (double) (spriteHeight * 4))), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-3f, 0.0f),
          delayBeforeAnimationStart = num2
        });
        int num5 = num4 + spriteWidth * 4;
        int num6 = spriteWidth > 16 /*0x10*/ ? 4 : 0;
        if (farmAnimalData.BabyTexture != null && farmAnimalData.BabyTexture != farmAnimalData.Texture)
        {
          for (int index = 1; index <= 2; ++index)
            this.TemporarySprites.Add(new TemporaryAnimatedSprite(farmAnimalData.BabyTexture, new Rectangle(0, spriteHeight, spriteWidth, spriteHeight), 90f, 4, 999999, new Vector2((float) (Game1.viewport.Width + (spriteWidth + 2 + num6) * index * 4), (float) (int) ((double) Game1.viewport.Height * 0.5 - (double) (spriteHeight * 4))), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-3f, 0.0f),
              delayBeforeAnimationStart = num2
            });
          num5 += (spriteWidth + 2 + num6) * 4 * 2;
        }
        string text = TokenParser.ParseText(farmAnimalData.DisplayName) ?? key;
        float x = Game1.dialogueFont.MeasureString(text).X;
        this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(0, spriteHeight, spriteWidth, spriteHeight), 120f, 1, 999999, new Vector2((float) (Game1.viewport.Width + num5 / 2) - x / 2f, (float) (int) ((double) Game1.viewport.Height * 0.5 + 12.0)), false, true, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = new Vector2(-3f, 0.0f),
          delayBeforeAnimationStart = num2,
          text = text
        });
        num2 += 2000 + num6 * 300;
      }
    }
    int num7 = 0;
    foreach (Pet allPet in Utility.getAllPets())
    {
      PetData data;
      if (Pet.TryGetData(allPet.petType.Value, out data) && data.SummitPerfectionEvent != null)
      {
        PetBreed breedById = data.GetBreedById(allPet.whichBreed.Value);
        PetSummitPerfectionEventData summitPerfectionEvent = data.SummitPerfectionEvent;
        this.TemporarySprites.Add(new TemporaryAnimatedSprite(breedById.Texture, summitPerfectionEvent.SourceRect, 90f, summitPerfectionEvent.AnimationLength, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 320.0 + (allPet.petType.Value.Equals("Dog") ? 96.0 : 0.0))), false, summitPerfectionEvent.Flipped, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
        {
          motion = summitPerfectionEvent.Motion,
          delayBeforeAnimationStart = 38000 + num7 * 400,
          startSound = data.BarkSound,
          pingPong = summitPerfectionEvent.PingPong
        });
        ++num7;
      }
      if (num7 >= 20)
        break;
    }
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(64 /*0x40*/, 192 /*0xC0*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 45000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128 /*0x80*/, 160 /*0xA0*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 47000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128 /*0x80*/, 224 /*0xE0*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 6, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 128.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 48000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(32 /*0x20*/, 160 /*0xA0*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 320.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 49000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(32 /*0x20*/, 160 /*0xA0*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 288.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 49500,
      pingPong = true
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(34, 98, 32 /*0x20*/, 32 /*0x20*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 50000,
      pingPong = true
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 32 /*0x20*/, 32 /*0x20*/, 32 /*0x20*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 50500,
      pingPong = true
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128 /*0x80*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 55000,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(192 /*0xC0*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.39999389648438)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 55300,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(256 /*0x0100*/, 96 /*0x60*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.60000610351563)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 55600,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 128 /*0x80*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 57000,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(48 /*0x30*/, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.39999389648438)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 57300,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(96 /*0x60*/, 144 /*0x90*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 3, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.60000610351563)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 57600,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(192 /*0xC0*/, 288, 16 /*0x10*/, 16 /*0x10*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 345.60000610351563)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 58000,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(128 /*0x80*/, 288, 16 /*0x10*/, 16 /*0x10*/), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.39999389648438)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 58300,
      pingPong = true,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 3000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 224 /*0xE0*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 64.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 54000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\critters", new Rectangle(0, 240 /*0xF0*/, 16 /*0x10*/, 16 /*0x10*/), 90f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 64.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 55000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(67, 190, 24, 51), 90f, 3, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, -4f),
      delayBeforeAnimationStart = 68000,
      rotation = -0.196349546f,
      pingPong = true,
      drawAboveAlwaysFront = true
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(0, 0, 57, 70), 150f, 2, 999999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, -4f),
      delayBeforeAnimationStart = 69000,
      rotation = -0.196349546f,
      drawAboveAlwaysFront = true
    });
    stringBuilder.Append("/spriteText 1 \"").Append(Utility.loadStringShort("UI", "EndCredit_Fish")).Append(" \"/pause ").Append(num2 - num3 + 18000);
    int num8 = num2 + 6000;
    int num9 = num8;
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 70000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 86000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 91000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(140, 78, 28, 38), 250f, 2, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 152.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 102000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(257, 98, 182, 18), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 72.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 75000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 82000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 80000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\AquariumFish", new Rectangle(0, 287, 47, 14), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 56.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 84000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(132, 20, 8, 8), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 48.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 81500,
      yPeriodic = true,
      yPeriodicRange = 21f,
      yPeriodicLoopTime = 5000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(140, 20, 8, 8), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 48.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 83500,
      yPeriodic = true,
      yPeriodicRange = 21f,
      yPeriodicLoopTime = 5000f
    });
    Dictionary<string, string> dictionary1 = DataLoader.Fish(Game1.content);
    Dictionary<string, string> dictionary2 = DataLoader.AquariumFish(Game1.content);
    int num10 = 0;
    foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
    {
      try
      {
        string key = keyValuePair.Key;
        string str;
        if (dictionary2.TryGetValue(key, out str))
        {
          string text = ItemRegistry.GetData("(O)" + key)?.DisplayName ?? ArgUtility.SplitBySpaceAndGet(keyValuePair.Value, 0);
          string[] array = str.Split('/');
          string textureName = ArgUtility.Get(array, 6, "LooseSprites\\AquariumFish", false);
          int num11 = ArgUtility.GetInt(array, 0);
          Rectangle sourceRect = new Rectangle(24 * num11 % 480, 24 * num11 / 480 * 48 /*0x30*/, 24, 24);
          float x = Game1.dialogueFont.MeasureString(text).X;
          this.TemporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRect, 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/), (float) (int) ((double) Game1.viewport.Height * 0.52999997138977051 - (double) (num10 * 64 /*0x40*/) * 2.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num8,
            yPeriodic = true,
            yPeriodicLoopTime = (float) Game1.random.Next(1500, 2100),
            yPeriodicRange = 4f
          });
          this.TemporarySprites.Add(new TemporaryAnimatedSprite(textureName, sourceRect, 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/ + 48 /*0x30*/) - x / 2f, (float) (int) ((double) Game1.viewport.Height * 0.52999997138977051 - (double) (num10 * 64 /*0x40*/) * 2.0 + 64.0 + 16.0)), false, true, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num8,
            text = text
          });
          ++num10;
          if (num10 == 4)
          {
            num8 += 2000;
            num10 = 0;
          }
        }
      }
      catch (Exception ex)
      {
        Game1.log.Error($"Couldn't add fish '{keyValuePair.Key}' to summit event credits.", ex);
      }
    }
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\projectiles", new Rectangle(64 /*0x40*/, 0, 16 /*0x10*/, 16 /*0x10*/), 909f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-6f, 0.0f),
      delayBeforeAnimationStart = 123000,
      rotationChange = -0.1f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("Tilesheets\\projectiles", new Rectangle(64 /*0x40*/, 0, 16 /*0x10*/, 16 /*0x10*/), 909f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 339.20001220703125)), false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-6f, 0.0f),
      delayBeforeAnimationStart = 123300,
      rotationChange = -0.1f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 108000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 2564), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 108000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 640, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 5128), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 108000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(0, 1452, 300, 69), 900f, 1, 999999, new Vector2((float) (Game1.viewport.Width + 7692), (float) ((double) Game1.viewport.Height * 0.5 - 392.0)), false, false, 0.2f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 108000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 0, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 110000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(65, 0, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 115000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(96 /*0x60*/, 90, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 118000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 176 /*0xB0*/, 104, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 121000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(32 /*0x20*/, 320, 32 /*0x20*/, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 124000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(31 /*0x1F*/, 58, 67, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 127000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 98, 32 /*0x20*/, 23), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 92.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 132000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(49, 131, 47, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 137000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 0, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 113000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 20, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 116000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 40, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 119000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 60, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 126000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 120, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 129000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 100, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 134000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 120, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 139000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\upperCavePlants", new Rectangle(0, 0, 48 /*0x30*/, 21), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 84.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 142000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\upperCavePlants", new Rectangle(96 /*0x60*/, 0, 48 /*0x30*/, 21), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 84.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 146000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(2, 123, 19, 24), 90f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 352.0)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 145000,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 2500f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\temporary_sprites_1", new Rectangle(2, 123, 19, 24), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 358.39999389648438)), false, true, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-4f, 0.0f),
      delayBeforeAnimationStart = 142500,
      yPeriodic = true,
      yPeriodicRange = 8f,
      yPeriodicLoopTime = 2000f
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 0, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 149000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(65, 0, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 151000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(96 /*0x60*/, 90, 31 /*0x1F*/, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 154000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\bushes", new Rectangle(0, 176 /*0xB0*/, 104, 29), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 116.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 156000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 0, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 155000
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 20, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 152500
    });
    this.TemporarySprites.Add(new TemporaryAnimatedSprite("TerrainFeatures\\grass", new Rectangle(0, 40, 44, 13), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 + 240.0 - 52.0)), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      motion = new Vector2(-3f, 0.0f),
      delayBeforeAnimationStart = 158000
    });
    if (Game1.player.favoriteThing.Value.EqualsIgnoreCase("concernedape"))
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Minigames\\Clouds", new Rectangle(210, 842, 138, 130), 900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) ((double) Game1.viewport.Height * 0.5 - 240.0)), false, false, 0.7f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = 160000,
        startSound = "discoverMineral"
      });
    if (!Utility.hasFinishedJojaRoute() && Game1.netWorldState.Value.PerfectionWaivers == 0 && !Game1.netWorldState.Value.ActivatedGoldenParrot)
    {
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\Morris", new Rectangle(48 /*0x30*/, 128 /*0x80*/, 16 /*0x10*/, 32 /*0x20*/), 9900f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height), false, false, 0.7f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-7f, -4f),
        delayBeforeAnimationStart = 168500,
        startSound = "slimeHit",
        rotationChange = 0.05f
      });
      this.TemporarySprites.Add(new TemporaryAnimatedSprite()
      {
        text = Game1.content.LoadString("Strings\\1_6_Strings:JojaFreeRun"),
        color = Color.Lime,
        position = new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height) / 2f,
        interval = 3000f,
        totalNumberOfLoops = 1,
        animationLength = 1,
        delayBeforeAnimationStart = 169500,
        layerDepth = 0.71f,
        local = true
      });
    }
    stringBuilder.Append("/spriteText 2 \"").Append(Utility.loadStringShort("UI", "EndCredit_Monsters")).Append(" \"/pause ").Append(num8 - num9 + 19000);
    int num12 = num8 + 6000;
    foreach (KeyValuePair<string, string> monster in DataLoader.Monsters(Game1.content))
    {
      if (!(monster.Key == "Fireball") && !(monster.Key == "Skeleton Warrior"))
      {
        int height1 = 16 /*0x10*/;
        int width1 = 16 /*0x10*/;
        int num13 = 0;
        int animationLength = 4;
        bool flag = false;
        int num14 = 0;
        Character character = (Character) null;
        if (monster.Key.Contains("Bat") || monster.Key.Contains("Ghost"))
          height1 = 24;
        string key = monster.Key;
        if (key != null)
        {
          switch (key.Length)
          {
            case 3:
              switch (key[0])
              {
                case 'B':
                  if (key == "Bat")
                  {
                    Texture2D texture2D1 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Frost Bat");
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D1.Width, width1 * num13 / texture2D1.Width * height1, width1, height1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height1 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D1
                    });
                    Texture2D texture2D2 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Lava Bat");
                    int height2 = 24;
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D2.Width, width1 * num13 / texture2D2.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 96f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height2 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D2
                    });
                    Texture2D texture2D3 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Iridium Bat");
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D3.Width, width1 * num13 / texture2D3.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 288f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height2 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D3
                    });
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D3.Width, width1 * num13 / texture2D3.Width * height2, width1, height2), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 128 /*0x80*/ + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(monster.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      text = Utility.loadStringShort("UI", "EndCredit_Bats")
                    });
                    num12 += 1500;
                    continue;
                  }
                  goto label_125;
                case 'C':
                  if (key == "Cat")
                    continue;
                  goto label_125;
                case 'F':
                  if (key == "Fly")
                    break;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 4:
              switch (key[0])
              {
                case 'C':
                  if (key == "Crow")
                    continue;
                  goto label_125;
                case 'F':
                  if (key == "Frog")
                    continue;
                  goto label_125;
                case 'G':
                  if (key == "Grub")
                    break;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 5:
              switch (key[0])
              {
                case 'D':
                  if (key == "Duggy")
                    break;
                  goto label_125;
                case 'G':
                  if (key == "Ghost")
                    goto label_118;
                  goto label_125;
                case 'M':
                  if (key == "Mummy")
                    goto label_119;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 6:
              switch (key[1])
              {
                case 'l':
                  if (key == "Sludge")
                    continue;
                  goto label_125;
                case 'p':
                  switch (key)
                  {
                    case "Spider":
                      width1 = 32 /*0x20*/;
                      height1 = 32 /*0x20*/;
                      animationLength = 2;
                      goto label_125;
                    case "Spiker":
                      goto label_118;
                    default:
                      goto label_125;
                  }
                default:
                  goto label_125;
              }
            case 7:
              if (key == "Serpent")
              {
                width1 = 32 /*0x20*/;
                height1 = 32 /*0x20*/;
                animationLength = 5;
                goto label_125;
              }
              goto label_125;
            case 8:
              switch (key[0])
              {
                case 'L':
                  if (key == "Lava Bat")
                    continue;
                  goto label_125;
                case 'S':
                  if (key == "Skeleton")
                    goto label_119;
                  goto label_125;
                default:
                  goto label_125;
              }
            case 9:
              switch (key[0])
              {
                case 'B':
                  if (key == "Big Slime")
                  {
                    height1 = 32 /*0x20*/;
                    width1 = 32 /*0x20*/;
                    num14 = 64 /*0x40*/;
                    character = (Character) new BigSlime(Vector2.Zero, 0);
                    goto label_125;
                  }
                  goto label_125;
                case 'F':
                  if (key == "Frost Bat")
                    continue;
                  goto label_125;
                case 'L':
                  switch (key)
                  {
                    case "Lava Crab":
                      break;
                    case "Lava Lurk":
                      num13 = 4;
                      flag = true;
                      goto label_125;
                    default:
                      goto label_125;
                  }
                  break;
                case 'R':
                  if (key == "Rock Crab")
                    break;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 10:
              switch (key[0])
              {
                case 'B':
                  if (key == "Blue Squid")
                  {
                    width1 = 24;
                    height1 = 24;
                    animationLength = 5;
                    goto label_125;
                  }
                  goto label_125;
                case 'P':
                  if (key == "Pepper Rex")
                  {
                    width1 = 32 /*0x20*/;
                    height1 = 32 /*0x20*/;
                    goto label_125;
                  }
                  goto label_125;
                case 'S':
                  if (key == "Shadow Guy")
                  {
                    int height3 = 32 /*0x20*/;
                    int num15 = 4;
                    Texture2D texture2D4 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Brute");
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D4.Width, width1 * num15 / texture2D4.Width * height3, width1, height3), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height3 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D4
                    });
                    Texture2D texture2D5 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Shaman");
                    int height4 = 24;
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num15 % texture2D5.Width, width1 * num15 / texture2D5.Width * height4, width1, height4), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 96f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height4 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D5
                    });
                    Texture2D texture2D6 = Game1.content.Load<Texture2D>("Characters\\Monsters\\Shadow Sniper");
                    int height5 = 32 /*0x20*/;
                    int width2 = 32 /*0x20*/;
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width2 * num15 % texture2D6.Width, width2 * num15 / texture2D6.Width * height5, width2, height5), 100f, animationLength, 999999, new Vector2((float) Game1.viewport.Width + 288f, (float) ((double) Game1.viewport.Height * 0.5 - (double) (height5 * 4) - 16.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = character,
                      texture = texture2D6
                    });
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width2 * num15 % texture2D6.Width, width2 * num15 / texture2D6.Width * height5, width2, height5), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 128 /*0x80*/ + width2 * 4 / 2) - Game1.dialogueFont.MeasureString(monster.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      text = Utility.loadStringShort("UI", "EndCredit_ShadowPeople")
                    });
                    num12 += 1500;
                    continue;
                  }
                  goto label_125;
                default:
                  goto label_125;
              }
            case 11:
              switch (key[0])
              {
                case 'D':
                  if (key == "Dust Spirit")
                    goto label_110;
                  goto label_125;
                case 'F':
                  if (key == "Frost Jelly")
                    continue;
                  goto label_125;
                case 'G':
                  if (key == "Green Slime")
                  {
                    Texture2D texture2D = (Texture2D) null;
                    if (character == null)
                      texture2D = Game1.content.Load<Texture2D>("Characters\\Monsters\\Green Slime");
                    int height6 = 32 /*0x20*/;
                    int num16 = 4;
                    GreenSlime greenSlime1 = new GreenSlime(Vector2.Zero, 0);
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/ - 64 /*0x40*/), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = (Character) greenSlime1,
                      texture = (Texture2D) null
                    });
                    GreenSlime greenSlime2 = new GreenSlime(Vector2.Zero, 41);
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 96.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = (Character) greenSlime2,
                      texture = (Texture2D) null
                    });
                    GreenSlime greenSlime3 = new GreenSlime(Vector2.Zero, 81);
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 288.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = (Character) greenSlime3,
                      texture = (Texture2D) null
                    });
                    GreenSlime greenSlime4 = new GreenSlime(Vector2.Zero, 121);
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 240.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4 * 2) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = (Character) greenSlime4,
                      texture = (Texture2D) null
                    });
                    GreenSlime greenSlime5 = new GreenSlime(Vector2.Zero, 0);
                    greenSlime5.makeTigerSlime();
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) ((double) Game1.viewport.Width + 144.0 - 64.0), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height6 * 4 * 2) + 32.0)), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      yPeriodic = animationLength == 1,
                      yPeriodicRange = 16f,
                      yPeriodicLoopTime = 3000f,
                      attachedCharacter = (Character) greenSlime5,
                      texture = (Texture2D) null
                    });
                    this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num16 % texture2D.Width, width1 * num16 / texture2D.Width * height6, width1, height6), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/ + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(monster.Value.Split('/')[14]).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
                    {
                      motion = new Vector2(-3f, 0.0f),
                      delayBeforeAnimationStart = num12,
                      text = Utility.loadStringShort("UI", "EndCredit_Slimes")
                    });
                    num12 += 1500;
                    continue;
                  }
                  goto label_125;
                case 'I':
                  if (key == "Iridium Bat")
                    continue;
                  goto label_125;
                case 'M':
                  if (key == "Magma Duggy")
                    break;
                  goto label_125;
                case 'S':
                  if (key == "Stone Golem")
                    break;
                  goto label_125;
                case 'T':
                  if (key == "Tiger Slime")
                    continue;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 12:
              switch (key[0])
              {
                case 'C':
                  if (key == "Carbon Ghost")
                    goto label_118;
                  goto label_125;
                case 'I':
                  if (key == "Iridium Crab")
                    break;
                  goto label_125;
                case 'M':
                  if (key == "Magma Sprite")
                    goto label_113;
                  goto label_125;
                case 'P':
                  if (key == "Putrid Ghost")
                    goto label_118;
                  goto label_125;
                case 'S':
                  if (key == "Shadow Brute")
                    continue;
                  goto label_125;
                default:
                  goto label_125;
              }
              break;
            case 13:
              switch (key[8])
              {
                case ' ':
                  if (key == "Skeleton Mage")
                    goto label_119;
                  goto label_125;
                case 'S':
                  if (key == "Iridium Slime")
                    continue;
                  goto label_125;
                case 'a':
                  if (key == "Magma Sparker")
                    goto label_113;
                  goto label_125;
                case 'h':
                  if (key == "Shadow Shaman")
                    continue;
                  goto label_125;
                case 'n':
                  if (key == "Shadow Sniper")
                    continue;
                  goto label_125;
                case 'r':
                  if (key == "Royal Serpent")
                    continue;
                  goto label_125;
                default:
                  goto label_125;
              }
            case 15:
              switch (key[0])
              {
                case 'D':
                  if (key == "Dwarvish Sentry")
                    goto label_118;
                  goto label_125;
                case 'F':
                  if (key == "False Magma Cap")
                    goto label_110;
                  goto label_125;
                default:
                  goto label_125;
              }
            case 16 /*0x10*/:
              if (key == "Wilderness Golem")
                break;
              goto label_125;
            default:
              goto label_125;
          }
          height1 = 24;
          num13 = 4;
          goto label_125;
label_110:
          height1 = 24;
          num13 = 0;
          goto label_125;
label_113:
          animationLength = 7;
          num13 = 7;
          goto label_125;
label_118:
          animationLength = 1;
          goto label_125;
label_119:
          height1 = 32 /*0x20*/;
          num13 = 4;
        }
label_125:
        try
        {
          Texture2D texture2D = character == null ? Game1.content.Load<Texture2D>("Characters\\Monsters\\" + monster.Key) : character.Sprite.Texture;
          this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D.Width, width1 * num13 / texture2D.Width * height1 + 1, width1, height1 - 1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/), (float) ((double) Game1.viewport.Height * 0.5 - (double) (height1 * 4) - 16.0) + (float) num14), false, true, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num12,
            yPeriodic = animationLength == 1,
            yPeriodicRange = 16f,
            yPeriodicLoopTime = 3000f,
            attachedCharacter = character,
            texture = character == null ? texture2D : (Texture2D) null,
            pingPong = flag
          });
          this.TemporarySprites.Add(new TemporaryAnimatedSprite((string) null, new Rectangle(width1 * num13 % texture2D.Width, width1 * num13 / texture2D.Width * height1, width1, height1), 100f, animationLength, 999999, new Vector2((float) (Game1.viewport.Width + 192 /*0xC0*/ + width1 * 4 / 2) - Game1.dialogueFont.MeasureString(Game1.parseText(monster.Value.Split('/')[14], Game1.dialogueFont, 256 /*0x0100*/)).X / 2f, (float) Game1.viewport.Height * 0.5f), false, false, 0.9f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = num12,
            text = Game1.parseText(monster.Value.Split('/')[14], Game1.dialogueFont, 256 /*0x0100*/)
          });
          num12 += 1500;
        }
        catch
        {
        }
      }
    }
    return stringBuilder.ToString();
  }

  /// <summary>Try to draw an NPC in the ending slide show.</summary>
  /// <param name="name">The NPC's internal name.</param>
  /// <param name="data">The NPC's content data.</param>
  /// <param name="animationInterval">The interval for their walking animation.</param>
  /// <param name="delayBeforeAnimationStart">The millisecond delay until they begin walking across the screen.</param>
  public bool TryDrawNpc(
    string name,
    CharacterData data,
    int animationInterval,
    int delayBeforeAnimationStart)
  {
    try
    {
      string nameForCharacter = NPC.getTextureNameForCharacter(name);
      Rectangle sourceRect = new Rectangle(0, data.Size.Y * 3, data.Size.X, data.Size.Y);
      Vector2 position = new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.4f + (float) ((32 /*0x20*/ - sourceRect.Height) * 4));
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\" + nameForCharacter, sourceRect, 90f, 4, 999999, position, false, false, 0.9f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(-3f, 0.0f),
        delayBeforeAnimationStart = delayBeforeAnimationStart
      });
      return true;
    }
    catch
    {
      return false;
    }
  }

  private bool sayGrufferSummitIntro(NPC spouse)
  {
    switch (spouse.name.Value)
    {
      case "Harvey":
      case "Elliott":
        return false;
      case "Abigail":
      case "Maru":
        return true;
      default:
        return spouse.Gender == Gender.Male;
    }
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    if (!Game1.eventUp || !this.isShowingEndSlideshow)
      return;
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 400.0), Game1.viewport.Width, 8), Utility.GetPrismaticColor());
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 412.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.8f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 432.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.6f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 468.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.4f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 - 536.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.2f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 240.0), Game1.viewport.Width, 8), Utility.GetPrismaticColor());
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 256.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.8f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 276.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.6f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 312.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.4f);
    b.Draw(Game1.staminaRect, new Rectangle(0, (int) ((double) Game1.viewport.Height * 0.5 + 380.0), Game1.viewport.Width, 4), Utility.GetPrismaticColor() * 0.2f);
  }
}
