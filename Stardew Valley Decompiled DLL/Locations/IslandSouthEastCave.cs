// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSouthEastCave
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Tools;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandSouthEastCave : IslandLocation
{
  protected PerchingBirds _parrots;
  protected Texture2D _parrotTextures;
  public NetLongList drinksClaimed = new NetLongList();
  [XmlIgnore]
  public bool wasPirateCaveOnLoad;
  private float smokeTimer;

  public IslandSouthEastCave()
  {
  }

  public IslandSouthEastCave(string map, string name)
    : base(map, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.drinksClaimed, "drinksClaimed");
  }

  public override void updateMap()
  {
    if (IslandSouthEastCave.isPirateNight())
      this.mapPath.Value = "Maps\\IslandSouthEastCave_pirates";
    else
      this.mapPath.Value = "Maps\\IslandSouthEastCave";
    base.updateMap();
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (!IslandSouthEastCave.isPirateNight())
      return;
    this.setTileProperty(19, 9, "Buildings", "Action", "MessageSpeech Pirates1");
    this.setTileProperty(20, 9, "Buildings", "Action", "MessageSpeech Pirates2");
    this.setTileProperty(26, 17, "Buildings", "Action", "MessageSpeech Pirates3");
    this.setTileProperty(23, 8, "Buildings", "Action", "MessageSpeech Pirates4");
    this.setTileProperty(27, 5, "Buildings", "Action", "MessageSpeech Pirates5");
    this.setTileProperty(32 /*0x20*/, 6, "Buildings", "Action", "MessageSpeech Pirates6");
    this.setTileProperty(30, 8, "Buildings", "Action", "DartsGame");
    this.setTileProperty(33, 8, "Buildings", "Action", "Bartender");
  }

  protected override void resetLocalState()
  {
    this.wasPirateCaveOnLoad = IslandSouthEastCave.isPirateNight();
    base.resetLocalState();
    if (IslandSouthEastCave.isPirateNight())
    {
      this.addFlame(new Vector2(25.6f, 5.7f), 0.0f);
      this.addFlame(new Vector2(18f, 11f) + new Vector2(0.2f, -0.05f));
      this.addFlame(new Vector2(22f, 11f) + new Vector2(0.2f, -0.05f));
      this.addFlame(new Vector2(23f, 16f) + new Vector2(0.2f, -0.05f));
      this.addFlame(new Vector2(19f, 27f) + new Vector2(0.2f, -0.05f));
      this.addFlame(new Vector2(33f, 10f) + new Vector2(0.2f, -0.05f));
      this.addFlame(new Vector2(21f, 22f) + new Vector2(0.2f, -0.05f));
      this._parrotTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\parrots");
      this._parrots = new PerchingBirds(this._parrotTextures, 3, 24, 24, new Vector2(12f, 19f), new Point[5]
      {
        new Point(12, 2),
        new Point(35, 6),
        new Point(25, 14),
        new Point(28, 1),
        new Point(27, 12)
      }, new Point[0]);
      this._parrots.peckDuration = 0;
      for (int index = 0; index < 3; ++index)
        this._parrots.AddBird(Game1.random.Next(0, 4));
      Game1.changeMusicTrack("PIRATE_THEME", true);
    }
    if (!this.AreMoonlightJelliesOut())
      return;
    this.addMoonlightJellies(40, Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, -24917.0), new Microsoft.Xna.Framework.Rectangle(0, 0, 30, 15));
  }

  public static bool isWearingPirateClothes(Farmer who)
  {
    if (who.hat.Value != null)
    {
      string itemId = who.hat.Value.ItemId;
      if (itemId == "62" || itemId == "76" || itemId == "24")
        return true;
    }
    return who.hasTrinketWithID("ParrotEgg");
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (who.IsLocalPlayer)
    {
      switch (ArgUtility.Get(action, 0))
      {
        case "Bartender":
          if (IslandSouthEastCave.isWearingPirateClothes(who))
          {
            if (this.drinksClaimed.Contains(Game1.player.UniqueMultiplayerID))
            {
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:PirateBartender_PirateClothes_NoMore"));
              break;
            }
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:PirateBartender_PirateClothes"));
            Game1.afterDialogues = (Game1.afterFadeFunction) (() => who.addItemByMenuIfNecessary(ItemRegistry.Create("(O)459"), (ItemGrabMenu.behaviorOnItemSelect) ((x, y) => this.drinksClaimed.Add(Game1.player.UniqueMultiplayerID))));
            break;
          }
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:Pirates8"));
          break;
        case "DartsGame":
          string question;
          switch (Game1.player.team.GetDroppedLimitedNutCount("Darts"))
          {
            case 0:
              question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_0");
              break;
            case 1:
              question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_1");
              break;
            case 2:
              question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_2");
              break;
            default:
              question = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_3");
              break;
          }
          this.createQuestionDialogue(question, this.createYesNoResponses(), "DartsGame");
          break;
      }
    }
    return base.performAction(action, who, tileLocation);
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    switch (questionAndAnswer)
    {
      case null:
        return false;
      case "DartsGame_Yes":
        int dart_count;
        switch (Game1.player.team.GetDroppedLimitedNutCount("Darts"))
        {
          case 1:
            dart_count = 15;
            break;
          case 2:
            dart_count = 10;
            break;
          default:
            dart_count = 20;
            break;
        }
        Game1.currentMinigame = (IMinigame) new Darts(dart_count);
        return true;
      default:
        return base.answerDialogueAction(questionAndAnswer, questionParams);
    }
  }

  public override void cleanupBeforePlayerExit()
  {
    this._parrots = (PerchingBirds) null;
    this._parrotTextures = (Texture2D) null;
    base.cleanupBeforePlayerExit();
  }

  private void addFlame(Vector2 tileLocation, float sort_offset_tiles = 2.25f)
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), tileLocation * 64f, false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandSouthEastCave_Flame",
      lightRadius = 2f,
      scale = 4f,
      layerDepth = (float) (((double) tileLocation.Y + (double) sort_offset_tiles) * 64.0 / 10000.0)
    });
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    this._parrots?.Draw(b);
    base.drawAboveAlwaysFrontLayer(b);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    this.drinksClaimed.Clear();
    base.DayUpdate(dayOfMonth);
  }

  public override void SetBuriedNutLocations()
  {
    base.SetBuriedNutLocations();
    this.buriedNutPoints.Add(new Point(36, 26));
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (!IslandSouthEastCave.isPirateNight())
      return;
    if (Game1.currentLocation == this && !this.wasPirateCaveOnLoad && Game1.locationRequest == null && Game1.activeClickableMenu == null && Game1.currentMinigame == null && Game1.CurrentEvent == null)
    {
      if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is FishingRod currentTool && (currentTool.pullingOutOfWater || currentTool.fishCaught || currentTool.showingTreasure))
        return;
      Game1.player.completelyStopAnimatingOrDoingAction();
      Game1.warpFarmer("IslandSouthEast", 29, 19, 1);
    }
    this._parrots?.Update(time);
    this.smokeTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.smokeTimer > 0.0)
      return;
    Utility.addSmokePuff((GameLocation) this, new Vector2(25.6f, 5.7f) * 64f);
    Utility.addSmokePuff((GameLocation) this, new Vector2(34f, 7.2f) * 64f);
    this.smokeTimer = 1000f;
  }

  public static bool isPirateNight()
  {
    return !Game1.IsRainingHere() && Game1.timeOfDay >= 2000 && Game1.dayOfMonth % 2 == 0;
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    if (!(l is IslandSouthEastCave islandSouthEastCave))
      return;
    this.drinksClaimed.Clear();
    foreach (long num in (NetList<long, NetLong>) islandSouthEastCave.drinksClaimed)
      this.drinksClaimed.Add(num);
  }
}
