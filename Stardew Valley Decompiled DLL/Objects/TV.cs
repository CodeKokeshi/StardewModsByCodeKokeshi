// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.TV
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace StardewValley.Objects;

public class TV : Furniture
{
  public const int customChannel = 1;
  public const int weatherChannel = 2;
  public const int fortuneTellerChannel = 3;
  public const int tipsChannel = 4;
  public const int cookingChannel = 5;
  public const int fishingChannel = 6;
  private int currentChannel;
  private TemporaryAnimatedSprite screen;
  private TemporaryAnimatedSprite screenOverlay;
  private static Dictionary<int, string> weekToRecipeMap;

  public TV()
  {
  }

  public TV(string itemId, Vector2 tile)
    : base(itemId, tile)
  {
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    List<Response> responseList = new List<Response>();
    responseList.Add(new Response("Weather", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13105")));
    responseList.Add(new Response("Fortune", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13107")));
    switch (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth))
    {
      case "Mon":
      case "Thu":
        responseList.Add(new Response("Livin'", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13111")));
        break;
      case "Sun":
        responseList.Add(new Response("The", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13114")));
        break;
      case "Wed":
        if (Game1.stats.DaysPlayed > 7U)
        {
          responseList.Add(new Response("The", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13117")));
          break;
        }
        break;
    }
    if (Game1.Date.Season == Season.Fall && Game1.Date.DayOfMonth == 26 && Game1.stats.Get("childrenTurnedToDoves") > 0U && !who.mailReceived.Contains("cursed_doll"))
      responseList.Add(new Response("???", "???"));
    if (Game1.player.mailReceived.Contains("pamNewChannel"))
      responseList.Add(new Response("Fishing", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel")));
    responseList.Add(new Response("(Leave)", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13118")));
    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"), responseList.ToArray(), new GameLocation.afterQuestionBehavior(this.selectChannel));
    Game1.player.Halt();
    return true;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new TV(this.ItemId, this.tileLocation.Value);

  public virtual void selectChannel(Farmer who, string answer)
  {
    if (Game1.IsGreenRainingHere())
    {
      this.currentChannel = 9999;
      this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(386, 334, 42, 28), 40f, 3, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
      Game1.drawObjectDialogue("...................");
      Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
    }
    else
    {
      switch (ArgUtility.SplitBySpaceAndGet(answer, 0))
      {
        case "Weather":
          this.currentChannel = 2;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 305, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getWeatherChannelOpening()));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        case "Fortune":
          this.currentChannel = 3;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 305, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getFortuneTellerOpening()));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        case "Livin'":
          this.currentChannel = 4;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(517, 361, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13124")));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        case "The":
          this.currentChannel = 5;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(602, 361, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13127")));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        case "???":
          Game1.changeMusicTrack("none");
          this.currentChannel = 666;
          this.screen = new TemporaryAnimatedSprite("Maps\\springobjects", new Rectangle(112 /*0x70*/, 64 /*0x40*/, 16 /*0x10*/, 16 /*0x10*/), 150f, 1, 999999, this.getScreenPosition() + (this.QualifiedItemId == "(F)1468" ? new Vector2(56f, 32f) : new Vector2(8f, 8f)), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Cursed_Doll")));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        case "Fishing":
          this.currentChannel = 6;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(172, 33, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Fishing_Channel_Intro")));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
      }
    }
    if (this.currentChannel <= 0)
      return;
    Game1.currentLightSources.Add(new LightSource(this.GenerateLightSourceId(this.TileLocation) + "_Screen", 2, this.getScreenPosition() + (this.QualifiedItemId == "(F)1468" ? new Vector2(88f, 80f) : new Vector2(38f, 48f)), this.QualifiedItemId == "(F)1468" ? 1f : 0.55f, Color.Black, onlyLocation: this.Location?.NameOrUniqueName));
  }

  protected virtual string getFortuneTellerOpening()
  {
    switch (Game1.random.Next(5))
    {
      case 0:
        return !Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13130") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13128");
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13132");
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13133");
      case 3:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13134");
      case 4:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13135");
      default:
        return "";
    }
  }

  protected virtual string getWeatherChannelOpening()
  {
    return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13136");
  }

  public virtual float getScreenSizeModifier()
  {
    return !(this.QualifiedItemId == "(F)1468") && !(this.QualifiedItemId == "(F)2326") ? 2f : 4f;
  }

  public virtual Vector2 getScreenPosition()
  {
    switch (this.QualifiedItemId)
    {
      case "(F)1466":
        return new Vector2((float) (this.boundingBox.X + 24), (float) this.boundingBox.Y);
      case "(F)1468":
        return new Vector2((float) (this.boundingBox.X + 12), (float) (this.boundingBox.Y - 128 /*0x80*/ + 32 /*0x20*/));
      case "(F)2326":
        return new Vector2((float) (this.boundingBox.X + 12), (float) (this.boundingBox.Y - 128 /*0x80*/ + 40));
      case "(F)1680":
        return new Vector2((float) (this.boundingBox.X + 24), (float) (this.boundingBox.Y - 12));
      case "(F)RetroTV":
        return new Vector2((float) (this.boundingBox.X + 24), (float) (this.boundingBox.Y - 64 /*0x40*/));
      default:
        return Vector2.Zero;
    }
  }

  public virtual void proceedToNextScene()
  {
    switch (this.currentChannel)
    {
      case 2:
        if (this.screenOverlay == null)
        {
          if (Utility.isGreenRainDay(Game1.dayOfMonth + 1, Game1.season))
            this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(213, 335, 43, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f)
            {
              id = 776
            };
          else
            this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(497, 305, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f)
            {
              id = 777
            };
          Game1.drawObjectDialogue(Game1.parseText(this.getWeatherForecast()));
          this.setWeatherOverlay();
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        }
        if (Game1.player.hasOrWillReceiveMail("Visited_Island") && this.screen.id == 777)
        {
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(148, 62, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getIslandWeatherForecast()));
          this.setWeatherOverlay(true);
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        }
        this.turnOffTV();
        break;
      case 3:
        if (this.screenOverlay == null)
        {
          this.screen = Game1.player.team.sharedDailyLuck.Value < 0.1 ? (Game1.player.team.sharedDailyLuck.Value > -0.1 ? new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(624, 305, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f) : new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(424, 476, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f)) : new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(424, 447, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.9999997473787516E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getFortuneForecast(Game1.player)));
          this.setFortuneOverlay(Game1.player);
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          break;
        }
        this.turnOffTV();
        break;
      case 4:
        if (this.screenOverlay == null)
        {
          Game1.drawObjectDialogue(Game1.parseText(this.getTodaysTip()));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
          break;
        }
        this.turnOffTV();
        break;
      case 5:
        if (this.screenOverlay == null)
        {
          Game1.multipleDialogues(this.getWeeklyRecipe());
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
          break;
        }
        this.turnOffTV();
        break;
      case 6:
        if (this.screenOverlay == null)
        {
          Game1.multipleDialogues(this.getFishingInfo());
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
          break;
        }
        this.turnOffTV();
        break;
      case 666:
        Game1.flashAlpha = 1f;
        Game1.playSound("batScreech");
        Game1.createItemDebris(ItemRegistry.Create("(O)103"), Game1.player.getStandingPosition(), 1, Game1.currentLocation);
        Game1.player.mailReceived.Add("cursed_doll");
        this.turnOffTV();
        break;
      case 9999:
        this.turnOffTV();
        break;
    }
  }

  public virtual void turnOffTV()
  {
    this.currentChannel = 0;
    this.screen = (TemporaryAnimatedSprite) null;
    this.screenOverlay = (TemporaryAnimatedSprite) null;
    Utility.removeLightSource(this.GenerateLightSourceId(this.TileLocation) + "_Screen");
  }

  protected virtual void setWeatherOverlay(bool island = false)
  {
    WorldDate date = new WorldDate(Game1.Date);
    ++date.TotalDays;
    this.setWeatherOverlay(!island ? (!Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow)) : Game1.netWorldState.Value.GetWeatherForLocation("Island").WeatherForTomorrow);
  }

  protected virtual void setWeatherOverlay(string weatherId)
  {
    switch (weatherId)
    {
      case "Snow":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(465, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      case "Rain":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(465, 333, 13, 13), 70f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      case "GreenRain":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(178, 363, 13, 13), 80f, 6, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      case "Wind":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.IsSpring ? new Rectangle(465, 359, 13, 13) : (Game1.IsFall ? new Rectangle(413, 359, 13, 13) : new Rectangle(465, 346, 13, 13)), 70f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      case "Storm":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 346, 13, 13), 120f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      case "Festival":
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 372, 13, 13), 120f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
      default:
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        break;
    }
  }

  private string[] getFishingInfo()
  {
    List<string> stringList1 = new List<string>();
    StringBuilder stringBuilder1 = new StringBuilder();
    StringBuilder stringBuilder2 = new StringBuilder();
    int seasonIndex = Game1.seasonIndex;
    stringBuilder1.AppendLine($"---{Utility.getSeasonNameFromNumber(seasonIndex)}---^^");
    Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);
    IDictionary<string, LocationData> locationData = Game1.locationData;
    List<string> stringList2 = new List<string>();
    int num = 0;
    foreach (KeyValuePair<string, string> keyValuePair1 in dictionary)
    {
      if (!keyValuePair1.Value.Contains("spring summer fall winter"))
      {
        stringList2.Clear();
        foreach (KeyValuePair<string, LocationData> keyValuePair2 in (IEnumerable<KeyValuePair<string, LocationData>>) locationData)
        {
          string key = keyValuePair2.Key;
          GameLocation location = (GameLocation) null;
          bool flag = false;
          if (keyValuePair2.Value.Fish != null)
          {
            foreach (SpawnFishData spawnFishData in keyValuePair2.Value.Fish)
            {
              if (!spawnFishData.IsBossFish)
              {
                Season? season1 = spawnFishData.Season;
                if (season1.HasValue)
                {
                  season1 = spawnFishData.Season;
                  Season season2 = Game1.season;
                  if (!(season1.GetValueOrDefault() == season2 & season1.HasValue))
                    continue;
                }
                if (spawnFishData.ItemId == keyValuePair1.Key || spawnFishData.ItemId == "(O)" + keyValuePair1.Key)
                {
                  if (spawnFishData.Condition != null)
                  {
                    location = location ?? Game1.getLocationFromName(key);
                    if (!GameStateQuery.CheckConditions(spawnFishData.Condition, location))
                      continue;
                  }
                  flag = true;
                  break;
                }
              }
            }
          }
          if (flag)
          {
            string sanitizedFishingLocation = this.getSanitizedFishingLocation(key);
            if (sanitizedFishingLocation != "" && !stringList2.Contains(sanitizedFishingLocation))
              stringList2.Add(sanitizedFishingLocation);
          }
        }
        if (stringList2.Count > 0)
        {
          string[] strArray1 = keyValuePair1.Value.Split('/');
          string[] strArray2 = ArgUtility.SplitBySpace(strArray1[5]);
          string str1 = ItemRegistry.GetData("(O)" + keyValuePair1.Key)?.DisplayName ?? strArray1[0];
          string str2 = strArray1[7];
          string str3 = strArray2[0];
          string str4 = strArray2[1];
          stringBuilder2.Append(str1);
          stringBuilder2.Append("...... ");
          stringBuilder2.Append(Game1.getTimeOfDayString(Convert.ToInt32(str3)).Replace(" ", ""));
          stringBuilder2.Append("-");
          stringBuilder2.Append(Game1.getTimeOfDayString(Convert.ToInt32(str4)).Replace(" ", ""));
          if (str2 != "both")
            stringBuilder2.Append(", " + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_" + str2));
          bool flag = false;
          foreach (string str5 in stringList2)
          {
            if (str5 != "")
            {
              flag = true;
              stringBuilder2.Append(", ");
              stringBuilder2.Append(str5);
            }
          }
          if (flag)
          {
            stringBuilder2.Append("^^");
            stringBuilder1.Append(stringBuilder2.ToString());
            ++num;
          }
          stringBuilder2.Clear();
          if (num > 3)
          {
            stringList1.Add(stringBuilder1.ToString());
            stringBuilder1.Clear();
            num = 0;
          }
        }
      }
    }
    return stringList1.ToArray();
  }

  private string getSanitizedFishingLocation(string rawLocationName)
  {
    switch (rawLocationName)
    {
      case "Town":
      case "Forest":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_River");
      case "Beach":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_Ocean");
      case "Mountain":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_Lake");
      default:
        return "";
    }
  }

  protected virtual string getTodaysTip()
  {
    string todaysTip;
    if (!DataLoader.Tv_TipChannel(Game1.temporaryContent).TryGetValue((Game1.stats.DaysPlayed % 224U /*0xE0*/).ToString() ?? "", out todaysTip))
      todaysTip = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13148");
    return todaysTip;
  }

  protected int getRerunWeek()
  {
    int maxValue = Math.Min(((int) Game1.stats.DaysPlayed - 3) / 7, 32 /*0x20*/);
    if (TV.weekToRecipeMap == null)
    {
      TV.weekToRecipeMap = new Dictionary<int, string>();
      Dictionary<string, string> dictionary = DataLoader.Tv_CookingChannel(Game1.temporaryContent);
      foreach (string key in dictionary.Keys)
        TV.weekToRecipeMap[Convert.ToInt32(key)] = dictionary[key].Split('/')[0];
    }
    List<Farmer> farmerList = new List<Farmer>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.isCustomized.Value && !allFarmer.IsDedicatedPlayer)
        farmerList.Add(allFarmer);
    }
    List<int> intList = new List<int>();
    for (int key = 1; key <= maxValue; ++key)
    {
      foreach (Farmer farmer in farmerList)
      {
        if (!farmer.cookingRecipes.ContainsKey(TV.weekToRecipeMap[key]))
        {
          intList.Add(key);
          break;
        }
      }
    }
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    return intList.Count != 0 ? intList[daySaveRandom.Next(intList.Count)] : Math.Max(1, 1 + daySaveRandom.Next(maxValue));
  }

  protected virtual string[] getWeeklyRecipe()
  {
    int num = (int) (Game1.stats.DaysPlayed % 224U /*0xE0*/ / 7U);
    if (Game1.stats.DaysPlayed % 224U /*0xE0*/ == 0U)
      num = 32 /*0x20*/;
    Dictionary<string, string> channelData = DataLoader.Tv_CookingChannel(Game1.temporaryContent);
    FarmerTeam team = Game1.player.team;
    if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed"))
    {
      if (team.lastDayQueenOfSauceRerunUpdated.Value != Game1.Date.TotalDays)
      {
        team.lastDayQueenOfSauceRerunUpdated.Set(Game1.Date.TotalDays);
        team.queenOfSauceRerunWeek.Set(this.getRerunWeek());
      }
      num = team.queenOfSauceRerunWeek.Value;
    }
    try
    {
      return this.getWeeklyRecipe(channelData, num.ToString());
    }
    catch
    {
      return this.getWeeklyRecipe(channelData, "1");
    }
  }

  private string[] getWeeklyRecipe(Dictionary<string, string> channelData, string id)
  {
    string str = channelData[id].Split('/')[0];
    bool flag = Game1.player.cookingRecipes.ContainsKey(str);
    string displayName = new CraftingRecipe(str, true).DisplayName;
    string[] weeklyRecipe = new string[2]
    {
      channelData[id].Split('/')[1],
      flag ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) displayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) displayName)
    };
    if (flag)
      return weeklyRecipe;
    Game1.player.cookingRecipes.Add(str, 0);
    return weeklyRecipe;
  }

  private string getIslandWeatherForecast()
  {
    ++new WorldDate(Game1.Date).TotalDays;
    string weatherForTomorrow = Game1.netWorldState.Value.GetWeatherForLocation("Island").WeatherForTomorrow;
    string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_IslandWeatherIntro");
    string islandWeatherForecast;
    switch (weatherForTomorrow)
    {
      case "Sun":
        islandWeatherForecast = str + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs." + Game1.random.Choose<string>("13182", "13183"));
        break;
      case "Rain":
        islandWeatherForecast = str + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13184");
        break;
      case "Storm":
        islandWeatherForecast = str + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13185");
        break;
      default:
        islandWeatherForecast = str + "???";
        break;
    }
    return islandWeatherForecast;
  }

  protected virtual string getWeatherForecast()
  {
    WorldDate date = new WorldDate(Game1.Date);
    ++date.TotalDays;
    return this.getWeatherForecast(!Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow));
  }

  protected virtual string getWeatherForecast(string weatherId)
  {
    switch (weatherId)
    {
      case "Festival":
        Dictionary<string, string> dictionary;
        try
        {
          dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>($"Data\\Festivals\\{Game1.currentSeason}{(Game1.dayOfMonth + 1).ToString()}");
        }
        catch (Exception ex)
        {
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
        }
        string[] strArray1 = dictionary["conditions"].Split('/');
        string[] strArray2 = ArgUtility.SplitBySpace(strArray1[1]);
        string str1 = dictionary["name"];
        string str2 = strArray1[0];
        int int32_1 = Convert.ToInt32(strArray2[0]);
        int int32_2 = Convert.ToInt32(strArray2[1]);
        string str3 = "";
        switch (str2)
        {
          case "Town":
            str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13170");
            break;
          case "Beach":
            str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13172");
            break;
          case "Forest":
            str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13174");
            break;
        }
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13175", (object) str1, (object) str3, (object) Game1.getTimeOfDayString(int32_1), (object) Game1.getTimeOfDayString(int32_2));
      case "Snow":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs." + Game1.random.Choose<string>("13180", "13181"));
      case "Rain":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13184");
      case "GreenRain":
        return Game1.content.LoadString("Strings\\1_6_Strings:GreenRainForecast");
      case "Storm":
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13185");
      case "Wind":
        switch (Game1.season)
        {
          case Season.Spring:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13187");
          case Season.Fall:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13189");
          default:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13190");
        }
      default:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs." + Game1.random.Choose<string>("13182", "13183"));
    }
  }

  public virtual void setFortuneOverlay(Farmer who)
  {
    if (who.DailyLuck < -0.07)
      this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(592, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
    else if (who.DailyLuck < -0.02)
      this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
    else if (who.DailyLuck > 0.07)
      this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(644, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
    else if (who.DailyLuck > 0.02)
      this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(592, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
    else
      this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.9999999494757503E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
  }

  public virtual string getFortuneForecast(Farmer who)
  {
    string fortuneForecast;
    if (who.team.sharedDailyLuck.Value == -0.12)
      fortuneForecast = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13191");
    else if (who.DailyLuck < -0.07)
      fortuneForecast = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13192");
    else if (who.DailyLuck < -0.02)
    {
      Utility.CreateDaySaveRandom();
      fortuneForecast = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs." + Game1.random.Choose<string>("13193", "13195"));
    }
    else
      fortuneForecast = who.team.sharedDailyLuck.Value != 0.12 ? (who.DailyLuck <= 0.07 ? (who.DailyLuck <= 0.02 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13200") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13199")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13198")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13197");
    if (who.DailyLuck == 0.0)
      fortuneForecast = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13201");
    return fortuneForecast;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    base.draw(spriteBatch, x, y, alpha);
    if (this.screen == null)
      return;
    this.screen.update(Game1.currentGameTime);
    this.screen.draw(spriteBatch);
    if (this.screenOverlay == null)
      return;
    this.screenOverlay.update(Game1.currentGameTime);
    this.screenOverlay.draw(spriteBatch);
  }
}
