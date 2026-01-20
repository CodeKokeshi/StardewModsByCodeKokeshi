// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Raccoon
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Characters;

public class Raccoon : NPC
{
  [XmlElement("mrs_raccoon")]
  public readonly NetBool mrs_raccoon = new NetBool();
  [XmlIgnore]
  public readonly NetMutex mutex = new NetMutex();
  private bool wasTalkedTo;
  private float updateFacingDirectionTimer;

  public Raccoon() => this.reloadSprite(false);

  public Raccoon(bool mrs_racooon = false)
    : base(new AnimatedSprite("Characters\\raccoon", mrs_racooon ? 40 : 0, 32 /*0x20*/, 32 /*0x20*/), new Vector2(54.5f, 8.25f) * 64f, 2, nameof (Raccoon))
  {
    this.HideShadow = true;
    this.mrs_raccoon.Value = mrs_racooon;
    this.Breather = false;
    if (!mrs_racooon)
      return;
    this.Position = new Vector2(56.5f, 8.25f) * 64f;
    this.Name = "MrsRaccoon";
  }

  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.HideShadow = true;
    this.Breather = false;
    if (this.Sprite == null)
      this.Sprite = new AnimatedSprite("Characters\\raccoon", this.mrs_raccoon.Value ? 40 : 0, 32 /*0x20*/, 32 /*0x20*/);
    if (!this.mrs_raccoon.Value)
      return;
    this.Position = new Vector2(56.5f, 8.25f) * 64f;
    this.Name = "MrsRaccoon";
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.mrs_raccoon, "mrs_raccoon");
    this.NetFields.AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields");
  }

  public void activate()
  {
    if (this.mrs_raccoon.Value)
    {
      Utility.TryOpenShopMenu(nameof (Raccoon), this.Name);
    }
    else
    {
      bool flag = Game1.netWorldState.Value.Date.TotalDays - Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished < 7;
      if (!this.wasTalkedTo)
      {
        int timesFedRaccoons = Game1.netWorldState.Value.TimesFedRaccoons;
        if (timesFedRaccoons == 0)
          flag = false;
        if (timesFedRaccoons >= 5 && !flag)
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_intro"));
        else if (timesFedRaccoons > 5 & flag)
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_interim"));
        else
          Game1.drawObjectDialogue(Game1.content.LoadString($"Strings\\1_6_Strings:Raccoon_{(flag ? "interim_" : "intro_")}{timesFedRaccoons.ToString()}"));
        if (flag)
          return;
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => this.mutex.RequestLock((Action) (() => this._activateMrRaccoon()), (Action) (() => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_busy")))));
      }
      else
      {
        if (flag)
          return;
        this.mutex.RequestLock((Action) (() => this._activateMrRaccoon()), (Action) (() => Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_busy"))));
      }
    }
  }

  public override void dayUpdate(int dayOfMonth)
  {
    base.dayUpdate(dayOfMonth);
    this.wasTalkedTo = false;
    this.mutex?.ReleaseLock();
  }

  private void _activateMrRaccoon()
  {
    this.wasTalkedTo = true;
    if (Game1.netWorldState.Value.SeasonOfCurrentRacconBundle == -1)
      Game1.netWorldState.Value.SeasonOfCurrentRacconBundle = (Game1.seasonIndex + (Game1.dayOfMonth > 21 ? 1 : 0)) % 4;
    JunimoNoteMenu junimoNoteMenu = new JunimoNoteMenu(Raccoon.GetBundle(), "LooseSprites\\raccoon_bundle_menu");
    junimoNoteMenu.onIngredientDeposit = (Action<int>) (index => Game1.netWorldState.Value.raccoonBundles[index] = true);
    junimoNoteMenu.onBundleComplete = new Action<JunimoNoteMenu>(this.bundleComplete);
    junimoNoteMenu.onScreenSwipeFinished = new Action<JunimoNoteMenu>(this.bundleCompleteAfterSwipe);
    junimoNoteMenu.behaviorBeforeCleanup = (Action<IClickableMenu>) (_ => this.mutex?.ReleaseLock());
    Game1.activeClickableMenu = (IClickableMenu) junimoNoteMenu;
  }

  /// <summary>Get the bundle which will be requested by Mr. Raccoon.</summary>
  public static Bundle GetBundle() => Raccoon.GetBundle(Game1.netWorldState.Value.TimesFedRaccoons);

  /// <summary>Get the bundle which will be requested by Mr. Raccoon.</summary>
  /// <param name="timesFed">The number of raccoon bundles that have already been completed.</param>
  public static Bundle GetBundle(int timesFed)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (timesFed * 377));
    for (int index = 0; index < 10; ++index)
      random.Next();
    int whichBundle = timesFed < 5 ? timesFed % 5 : random.Next(5);
    List<BundleIngredientDescription> ingredients = new List<BundleIngredientDescription>();
    Raccoon.AddNextIngredient(ingredients, whichBundle, random);
    Raccoon.AddNextIngredient(ingredients, whichBundle, random);
    Raccoon.AddNextIngredient(ingredients, whichBundle, random);
    return new Bundle("Seafood", (string) null, ingredients, new bool[1])
    {
      bundleTextureOverride = Game1.content.Load<Texture2D>("LooseSprites\\BundleSprites"),
      bundleTextureIndexOverride = 14 + whichBundle,
      bundleIndex = whichBundle
    };
  }

  public Item getBundleReward()
  {
    switch (Game1.netWorldState.Value.TimesFedRaccoons)
    {
      case 1:
        return Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, Game1.random, 25);
      case 2:
        Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Raccoon_expanded");
        return ItemRegistry.Create("(O)Book_WildSeeds");
      case 3:
        Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Raccoon_expanded");
        return ItemRegistry.Create("(H)RaccoonHat");
      case 4:
        Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Raccoon_expanded");
        return ItemRegistry.Create("(O)872", 5);
      case 5:
        Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Raccoon_expanded");
        return ItemRegistry.Create("(F)JungleTank");
      case 6:
        Game1.Multiplayer.broadcastGlobalMessage("Strings\\1_6_Strings:Raccoon_expanded");
        break;
    }
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) (Game1.netWorldState.Value.TimesFedRaccoons * 377));
    for (int index = 0; index < 10; ++index)
      random.Next();
    switch (random.Next(5))
    {
      case 0:
        return ItemRegistry.Create("(O)872", 7);
      case 1:
        return ItemRegistry.Create("(O)PurpleBook");
      case 2:
        if (Game1.netWorldState.Value.GoldenWalnutsFound >= 100 && (double) Utility.getFarmerItemsShippedPercent() < 1.0)
        {
          Item basicShippedItem = Utility.recentlyDiscoveredMissingBasicShippedItem;
          if (basicShippedItem != null && basicShippedItem.Category != -26 && basicShippedItem.ItemId != "812")
            return basicShippedItem;
        }
        return ItemRegistry.Create("(O)MysteryBox", 5);
      case 3:
        return ItemRegistry.Create("(O)StardropTea");
      case 4:
        return Utility.getRaccoonSeedForCurrentTimeOfYear(Game1.player, Game1.random, 25);
      default:
        return ItemRegistry.Create("(O)MysteryBox", 3);
    }
  }

  private void bundleCompleteAfterSwipe(JunimoNoteMenu menu)
  {
    Game1.activeClickableMenu = (IClickableMenu) null;
    this.mutex?.ReleaseLock();
    Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished = Game1.netWorldState.Value.Date.TotalDays;
    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:Raccoon_receive"));
    Game1.afterDialogues = (Game1.afterFadeFunction) (() => Game1.player.addItemByMenuIfNecessaryElseHoldUp(this.getBundleReward()));
  }

  private void bundleComplete(JunimoNoteMenu menu)
  {
    JunimoNoteMenu.screenSwipe = new ScreenSwipe(1);
    ++Game1.netWorldState.Value.TimesFedRaccoons;
    Game1.netWorldState.Value.raccoonBundles[0] = false;
    Game1.netWorldState.Value.raccoonBundles[1] = false;
    Game1.netWorldState.Value.SeasonOfCurrentRacconBundle = -1;
    this.wasTalkedTo = false;
  }

  private static void AddNextIngredient(
    List<BundleIngredientDescription> ingredients,
    int whichBundle,
    Random r)
  {
    int count = ingredients.Count;
    int currentRacconBundle = Game1.netWorldState.Value.SeasonOfCurrentRacconBundle;
    switch (whichBundle)
    {
      case 0:
        if (count != 0)
        {
          if (count != 1)
            break;
          string[][] strArray = new string[4][]
          {
            new string[7]
            {
              "136",
              "132",
              "700",
              "702",
              "156",
              "267",
              "706"
            },
            new string[11]
            {
              "136",
              "132",
              "700",
              "702",
              "156",
              "267",
              "706",
              "138",
              "701",
              "146",
              "130"
            },
            new string[9]
            {
              "136",
              "132",
              "700",
              "702",
              "156",
              "701",
              "269",
              "139",
              "139"
            },
            new string[9]
            {
              "136",
              "132",
              "700",
              "702",
              "156",
              "146",
              "130",
              "141",
              "269"
            }
          };
          ingredients.Add(new BundleIngredientDescription("SmokedFish", 1, 0, Game1.netWorldState.Value.raccoonBundles[1], r.ChooseFrom<string>((IList<string>) strArray[currentRacconBundle])));
          break;
        }
        ingredients.Add(new BundleIngredientDescription(r.ChooseFrom<string>((IList<string>) new string[7]
        {
          "722",
          "721",
          "716",
          "719",
          "723",
          "718",
          "372"
        }), 5, 0, Game1.netWorldState.Value.raccoonBundles[0]));
        break;
      case 1:
        string[][] strArray1 = new string[4][]
        {
          new string[5]{ "90", "634", "638", "400", "88" },
          new string[7]
          {
            "90",
            "258",
            "260",
            "635",
            "636",
            "88",
            "396"
          },
          new string[7]
          {
            "90",
            "613",
            "282",
            "637",
            "410",
            "88",
            "406"
          },
          new string[6]
          {
            "90",
            "414",
            "414",
            "88",
            "Powdermelon",
            "Powdermelon"
          }
        };
        switch (count)
        {
          case 0:
            ingredients.Add(new BundleIngredientDescription("DriedFruit", 1, 0, Game1.netWorldState.Value.raccoonBundles[0], r.ChooseFrom<string>((IList<string>) strArray1[currentRacconBundle])));
            return;
          case 1:
            string preservesId1 = "";
            while (preservesId1 == "" || preservesId1 == ingredients[0].preservesId)
              preservesId1 = r.ChooseFrom<string>((IList<string>) strArray1[currentRacconBundle]);
            ingredients.Add(new BundleIngredientDescription("Jelly", 1, 0, Game1.netWorldState.Value.raccoonBundles[1], preservesId1));
            return;
          default:
            return;
        }
      case 2:
        string[][] strArray2 = new string[4][]
        {
          new string[3]{ "422", "404", "257" },
          new string[2]{ "422", "404" },
          new string[3]{ "422", "404", "281" },
          new string[2]{ "422", "404" }
        };
        if (count != 0)
        {
          if (count != 1)
            break;
          ingredients.Add(new BundleIngredientDescription(r.ChooseFrom<string>((IList<string>) new string[3]
          {
            "-5",
            "78",
            "157"
          }), 5, 0, Game1.netWorldState.Value.raccoonBundles[1]));
          break;
        }
        ingredients.Add(new BundleIngredientDescription("DriedMushroom", 1, 0, Game1.netWorldState.Value.raccoonBundles[0], r.ChooseFrom<string>((IList<string>) strArray2[currentRacconBundle])));
        break;
      case 3:
        string[][] strArray3 = new string[4][]
        {
          new string[8]
          {
            "190",
            "188",
            "250",
            "192",
            "16",
            "22",
            "Carrot",
            "Carrot"
          },
          new string[6]
          {
            "270",
            "264",
            "256",
            "78",
            "SummerSquash",
            "SummerSquash"
          },
          new string[5]{ "Broccoli", "Broccoli", "278", "272", "276" },
          new string[3]{ "416", "412", "78" }
        };
        switch (count)
        {
          case 0:
            ingredients.Add(new BundleIngredientDescription("Juice", 1, 0, Game1.netWorldState.Value.raccoonBundles[0], r.ChooseFrom<string>((IList<string>) strArray3[currentRacconBundle])));
            return;
          case 1:
            string preservesId2 = "";
            while (preservesId2 == "" || preservesId2 == ingredients[0].preservesId)
              preservesId2 = r.ChooseFrom<string>((IList<string>) strArray3[currentRacconBundle]);
            ingredients.Add(new BundleIngredientDescription("Pickle", 1, 0, Game1.netWorldState.Value.raccoonBundles[1], preservesId2));
            return;
          default:
            return;
        }
      case 4:
        string[] options = new string[14]
        {
          "Moss_10",
          "110_1",
          "168_5",
          "766_99",
          "767_20",
          "535_8",
          "536_5",
          "537_3",
          "393_4",
          "397_2",
          "684_20",
          "72_1",
          "68_3",
          "156_3"
        };
        switch (count)
        {
          case 0:
            string str1 = r.ChooseFrom<string>((IList<string>) options);
            ingredients.Add(new BundleIngredientDescription(str1.Split('_')[0], Convert.ToInt32(str1.Split('_')[1]), 0, Game1.netWorldState.Value.raccoonBundles[0]));
            return;
          case 1:
            string str2 = "";
            while (str2 == "" || str2.Split("_")[0] == ingredients[0].id)
              str2 = r.ChooseFrom<string>((IList<string>) options);
            ingredients.Add(new BundleIngredientDescription(str2.Split('_')[0], Convert.ToInt32(str2.Split('_')[1]), 0, Game1.netWorldState.Value.raccoonBundles[1]));
            return;
          default:
            return;
        }
    }
  }

  public override void update(GameTime time, GameLocation location)
  {
    int shakeTimer = this.shakeTimer;
    base.update(time, location);
    this.mutex?.Update(location);
    if (this.mrs_raccoon.Value)
      this.Sprite.CurrentFrame = time.TotalGameTime.TotalMilliseconds % 13200.0 > 10000.0 ? 40 + (int) (time.TotalGameTime.TotalMilliseconds % 800.0 / 100.0) : 32 /*0x20*/ + (int) (time.TotalGameTime.TotalMilliseconds % 1200.0 / 150.0);
    else if ((double) Vector2.Distance(this.Position, Game1.player.getStandingPosition()) < 256.0)
    {
      switch (this.getGeneralDirectionTowards(Game1.player.getStandingPosition(), 32 /*0x20*/, useTileCalculations: false))
      {
        case 0:
          this.Sprite.CurrentFrame = 16 /*0x10*/ + (int) (time.TotalGameTime.TotalMilliseconds % 800.0 / 100.0);
          break;
        case 1:
        case 2:
        case 3:
          this.Sprite.CurrentFrame = (int) (time.TotalGameTime.TotalMilliseconds % 800.0 / 100.0);
          break;
      }
    }
    else
      this.Sprite.CurrentFrame = time.TotalGameTime.TotalMilliseconds % 8000.0 < 3200.0 ? (int) (time.TotalGameTime.TotalMilliseconds % 800.0 / 100.0) : 48 /*0x30*/ + (int) (time.TotalGameTime.TotalMilliseconds % 400.0 / 100.0);
  }

  public override bool checkAction(Farmer who, GameLocation l)
  {
    if (this.shakeTimer <= 0)
    {
      if (this.mrs_raccoon.Value)
        this.playNearbySoundLocal(nameof (Raccoon), new int?(2400));
      else
        this.playNearbySoundLocal(nameof (Raccoon));
      this.shakeTimer = 200;
      who.freezePause = 300;
      DelayedAction.functionAfterDelay(new Action(this.activate), 300);
    }
    return true;
  }

  public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
  {
    base.performTenMinuteUpdate(timeOfDay, l);
  }

  public override void draw(SpriteBatch b) => base.draw(b);
}
