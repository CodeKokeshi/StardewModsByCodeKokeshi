// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BoatTunnel
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Minigames;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Text;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class BoatTunnel : GameLocation
{
  private Texture2D boatTexture;
  private Vector2 boatPosition;
  public Microsoft.Xna.Framework.Rectangle gateRect = new Microsoft.Xna.Framework.Rectangle(0, 120, 32 /*0x20*/, 40);
  protected int _gateFrame;
  protected int _gateDirection;
  protected float _gateFrameTimer;
  public const float GATE_SECONDS_PER_FRAME = 0.1f;
  public const int GATE_FRAMES = 5;
  protected int _boatOffset;
  protected int _boatDirection;
  public const int PLANK_MAX_OFFSET = 16 /*0x10*/;
  public float _plankPosition;
  public float _plankDirection;
  protected Farmer _farmerActor;
  protected Event _boatEvent;
  protected bool _playerPathing;
  protected int nonBlockingPause;
  protected float _nextBubble;
  protected float _nextSlosh;
  protected float _nextSmoke;
  protected float _plankShake;
  protected int forceWarpTimer;
  protected bool _boatAnimating;
  public BoatTunnel.TunnelAnimationState animationState;

  /// <summary>The gold price to buy a ticket on the boat.</summary>
  public int TicketPrice { get; set; } = 1000;

  public BoatTunnel()
  {
  }

  public BoatTunnel(string map, string name)
    : base(map, name)
  {
  }

  public override void cleanupBeforePlayerExit()
  {
    Game1.player.controller = (PathFindController) null;
    base.cleanupBeforePlayerExit();
  }

  public virtual bool GateFinishedAnimating()
  {
    if (this._gateDirection < 0)
      return this._gateFrame <= 0;
    return this._gateDirection <= 0 || this._gateFrame >= 5;
  }

  public virtual bool PlankFinishedAnimating()
  {
    if ((double) this._plankDirection < 0.0)
      return (double) this._plankPosition <= 0.0;
    return (double) this._plankDirection <= 0.0 || (double) this._plankPosition >= 16.0;
  }

  public virtual void SetCurrentState(BoatTunnel.TunnelAnimationState animation_state)
  {
    this.animationState = animation_state;
  }

  public virtual void UpdateGateTileProperty()
  {
    if (this._gateFrame == 0)
      this.setTileProperty(6, 8, "Back", "TemporaryBarrier", "T");
    else
      this.removeTileProperty(6, 8, "Back", "TemporaryBarrier");
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (this.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings") == "BoatTicket")
    {
      if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine"))
      {
        if (who.Items.ContainsId("(O)787", 5))
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateBatteries"), this.createYesNoResponses(), "WillyBoatDonateBatteries");
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateBatteriesHint"));
      }
      else if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed"))
      {
        if (Game1.player.isRidingHorse() && Game1.player.mount != null)
        {
          Game1.player.mount.checkAction(Game1.player, (GameLocation) this);
        }
        else
        {
          string numberWithCommas = Utility.getNumberWithCommas(this.TicketPrice);
          if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.es)
            this.createQuestionDialogueWithCustomWidth(Game1.content.LoadString("Strings\\Locations:BuyTicket", (object) numberWithCommas), this.createYesNoResponses(), "Boat");
          else
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BuyTicket", (object) numberWithCommas), this.createYesNoResponses(), "Boat");
        }
      }
      return true;
    }
    if (!Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed"))
    {
      if (tileLocation.X == 6 && tileLocation.Y == 8 && !Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull"))
      {
        if (who.Items.ContainsId("(O)709", 200))
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateHardwood"), this.createYesNoResponses(), "WillyBoatDonateHardwood");
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateHardwoodHint"));
        return true;
      }
      if (tileLocation.X == 8 && tileLocation.Y == 10 && !Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor"))
      {
        if (who.Items.ContainsId("(O)337", 5))
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateIridium"), this.createYesNoResponses(), "WillyBoatDonateIridium");
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_DonateIridiumHint"));
        return true;
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    return !Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed") && (xTile == 6 && yTile == 8 || xTile == 8 && yTile == 10) || base.isActionableTile(xTile, yTile, who);
  }

  public override bool answerDialogue(Response answer)
  {
    if (this.lastQuestionKey != null && this.afterQuestion == null)
    {
      string str = $"{ArgUtility.SplitBySpaceAndGet(this.lastQuestionKey, 0)}_{answer.responseKey}";
      int ticketPrice = this.TicketPrice;
      switch (str)
      {
        case "Boat_Yes":
          if (Game1.player.Money >= ticketPrice)
          {
            Game1.player.Money -= ticketPrice;
            this.StartDeparture();
          }
          else if (Game1.player.Money < ticketPrice)
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
          return true;
        case "WillyBoatDonateBatteries_Yes":
          Game1.multiplayer.globalChatInfoMessage("RepairBoatMachine", Game1.player.Name);
          Game1.player.Items.ReduceId("(O)787", 5);
          DelayedAction.playSoundAfterDelay("openBox", 600);
          Game1.addMailForTomorrow("willyBoatTicketMachine", true, true);
          this.checkForBoatComplete();
          return true;
        case "WillyBoatDonateHardwood_Yes":
          Game1.multiplayer.globalChatInfoMessage("RepairBoatHull", Game1.player.Name);
          Game1.player.Items.ReduceId("(O)709", 200);
          DelayedAction.playSoundAfterDelay("Ship", 600);
          Game1.addMailForTomorrow("willyBoatHull", true, true);
          this.checkForBoatComplete();
          return true;
        case "WillyBoatDonateIridium_Yes":
          Game1.multiplayer.globalChatInfoMessage("RepairBoatAnchor", Game1.player.Name);
          Game1.player.Items.ReduceId("(O)337", 5);
          DelayedAction.playSoundAfterDelay("clank", 600);
          DelayedAction.playSoundAfterDelay("clank", 1200);
          DelayedAction.playSoundAfterDelay("clank", 1800);
          Game1.addMailForTomorrow("willyBoatAnchor", true, true);
          this.checkForBoatComplete();
          return true;
      }
    }
    return base.answerDialogue(answer);
  }

  private void checkForBoatComplete()
  {
    if (!Game1.player.hasOrWillReceiveMail("willyBoatTicketMachine") || !Game1.player.hasOrWillReceiveMail("willyBoatHull") || !Game1.player.hasOrWillReceiveMail("willyBoatAnchor"))
      return;
    Game1.player.freezePause = 1500;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      Game1.multiplayer.globalChatInfoMessage("RepairBoat");
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BoatTunnel_boatcomplete"));
    }), 1500);
  }

  public override bool shouldShadowBeDrawnAboveBuildingsLayer(Vector2 p)
  {
    return (double) p.Y <= 8.0 || (double) p.Y <= 10.0 && (double) p.X >= 4.0 && (double) p.X <= 8.0 || base.shouldShadowBeDrawnAboveBuildingsLayer(p);
  }

  public virtual void StartDeparture()
  {
    xTile.Dimensions.Rectangle viewport = Game1.viewport;
    Vector2 position = Game1.player.Position;
    int facingDirection = Game1.player.FacingDirection;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("none/0 0/farmer 0 0 0 Willy 6 12 0/playMusic none/skippable");
    if (Game1.stats.Get("boatRidesToIsland") <= 0U)
      stringBuilder.Append($"/textAboveHead Willy \"{Game1.content.LoadString("Strings\\Locations:BoatTunnel_willyText_firstRide")}\"");
    else if (Game1.random.NextDouble() < 0.2)
      stringBuilder.Append($"/textAboveHead Willy \"{Game1.content.LoadString("Strings\\Locations:BoatTunnel_willyText_random" + Game1.random.Next(2).ToString())}\"");
    stringBuilder.Append("/move Willy 0 -3 0/pause 500/locationSpecificCommand open_gate/viewport move 0 -1 1000/pause 500/move Willy 0 -2 3/move Willy -1 0 1/locationSpecificCommand path_player 6 5 2/move Willy 1 0 2/move Willy 0 1 2/pause 250/playSound clubhit/animate Willy false false 500 27/locationSpecificCommand retract_plank/jump Willy 4/pause 750/move Willy 0 -1 0/locationSpecificCommand close_gate/pause 200/move Willy 3 0 1/locationSpecificCommand offset_willy/move Willy 1 0 1");
    stringBuilder.Append("/locationSpecificCommand non_blocking_pause 1000/playerControl boatRide/playSound furnace/locationSpecificCommand animate_boat_start/locationSpecificCommand non_blocking_pause 1000/locationSpecificCommand boat_depart/locationSpecificCommand animate_boat_move/fade/viewport -5000 -5000/end tunnelDepart");
    this._boatEvent = new Event(stringBuilder.ToString(), (string) null, "-78765", Game1.player)
    {
      showWorldCharacters = true,
      showGroundObjects = true,
      ignoreObjectCollisions = false
    };
    this._boatEvent.onEventFinished += new Action(this.OnBoatEventEnd);
    this.currentEvent = this._boatEvent;
    this._boatEvent.Update((GameLocation) this, Game1.currentGameTime);
    Game1.eventUp = true;
    Game1.viewport = viewport;
    this._farmerActor = this.currentEvent.getCharacterByName("farmer") as Farmer;
    this._farmerActor.Position = position;
    this._farmerActor.faceDirection(facingDirection);
    (this.currentEvent.getCharacterByName("Willy") as NPC).IsInvisible = false;
    int num = (int) Game1.stats.Increment("boatRidesToIsland");
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (this._boatDirection != 0)
    {
      this._boatOffset += this._boatDirection;
      if (this.currentEvent != null)
      {
        foreach (NPC actor in this.currentEvent.actors)
        {
          actor.shouldShadowBeOffset = true;
          actor.drawOffset.X = (float) this._boatOffset;
        }
        foreach (Farmer farmerActor in this.currentEvent.farmerActors)
        {
          farmerActor.shouldShadowBeOffset = true;
          farmerActor.drawOffset.X = (float) this._boatOffset;
        }
      }
    }
    if (!this.PlankFinishedAnimating())
    {
      this._plankPosition += this._plankDirection;
      if (this.PlankFinishedAnimating())
        this._plankDirection = 0.0f;
    }
    if (!this.GateFinishedAnimating())
    {
      this._gateFrameTimer += (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._gateFrameTimer >= 0.10000000149011612)
      {
        this._gateFrameTimer -= 0.1f;
        this._gateFrame += this._gateDirection;
      }
    }
    else
      this._gateFrameTimer = 0.0f;
    if ((double) this._plankShake > 0.0)
    {
      this._plankShake -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this._plankShake < 0.0)
        this._plankShake = 0.0f;
    }
    Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(24, 188, 16 /*0x10*/, 220);
    r.X += (int) this.GetBoatPosition().X;
    r.Y += (int) this.GetBoatPosition().Y;
    TimeSpan elapsedGameTime;
    if ((double) this._boatDirection != 0.0)
    {
      if ((double) this._nextBubble > 0.0)
      {
        double nextBubble = (double) this._nextBubble;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this._nextBubble = (float) (nextBubble - totalSeconds);
      }
      else
      {
        this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 50f, 9, 1, Utility.getRandomPositionInThisRectangle(r, Game1.random), false, false, 0.0f, 0.025f, Color.White, 1f, 0.0f, 0.0f, 0.0f)
        {
          acceleration = new Vector2(-0.25f * (float) Math.Sign(this._boatDirection), 0.0f)
        });
        this._nextBubble = 0.01f;
      }
      if ((double) this._nextSlosh > 0.0)
      {
        double nextSlosh = (double) this._nextSlosh;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this._nextSlosh = (float) (nextSlosh - totalSeconds);
      }
      else
      {
        Game1.playSound("waterSlosh");
        this._nextSlosh = 0.5f;
      }
    }
    if (!this._boatAnimating)
      return;
    if ((double) this._nextSmoke > 0.0)
    {
      double nextSmoke = (double) this._nextSmoke;
      elapsedGameTime = time.ElapsedGameTime;
      double totalSeconds = elapsedGameTime.TotalSeconds;
      this._nextSmoke = (float) (nextSmoke - totalSeconds);
    }
    else
    {
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64 /*0x40*/, 128 /*0x80*/), 200f, 9, 1, new Vector2(80f, -32f) * 4f + this.GetBoatPosition(), false, false, 1f, 0.025f, Color.White, 1f, 0.025f, 0.0f, 0.0f)
      {
        acceleration = new Vector2(-0.25f, -0.15f)
      });
      this._nextSmoke = 0.2f;
    }
  }

  public virtual void OnBoatEventEnd()
  {
    if (this._boatEvent == null)
      return;
    foreach (NPC actor in this._boatEvent.actors)
    {
      actor.shouldShadowBeOffset = false;
      actor.drawOffset.X = 0.0f;
    }
    foreach (Farmer farmerActor in this._boatEvent.farmerActors)
    {
      farmerActor.shouldShadowBeOffset = false;
      farmerActor.drawOffset.X = 0.0f;
    }
    this.ResetBoat();
    this._boatEvent = (Event) null;
    if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
      return;
    Game1.addMailForTomorrow("seenBoatJourney", true);
    Game1.currentMinigame = (IMinigame) new BoatJourney();
  }

  public override bool RunLocationSpecificEventCommand(
    Event current_event,
    string command_string,
    bool first_run,
    params string[] args)
  {
    if (command_string != null)
    {
      switch (command_string.Length)
      {
        case 9:
          if (command_string == "open_gate")
          {
            if (first_run)
              Game1.playSound("openChest");
            this._gateDirection = 1;
            if (this.GateFinishedAnimating())
              this.UpdateGateTileProperty();
            return this.GateFinishedAnimating();
          }
          break;
        case 10:
          if (command_string == "close_gate")
          {
            this._gateDirection = -1;
            if (this.GateFinishedAnimating())
              this.UpdateGateTileProperty();
            return this.GateFinishedAnimating();
          }
          break;
        case 11:
          switch (command_string[0])
          {
            case 'b':
              if (command_string == "boat_depart")
              {
                if (first_run)
                  this._boatDirection = 1;
                return this._boatOffset >= 100;
              }
              break;
            case 'p':
              if (command_string == "path_player")
              {
                int x = ArgUtility.GetInt(args, 0);
                int y = ArgUtility.GetInt(args, 1);
                int num = ArgUtility.GetInt(args, 2, 2);
                if (first_run)
                {
                  this._playerPathing = true;
                  Game1.player.controller = new PathFindController((Character) Game1.player, (GameLocation) this, new Point(x, y), num, new PathFindController.endBehavior(this.OnReachedBoatDeck))
                  {
                    allowPlayerPathingInEvent = true
                  };
                  Game1.player.canOnlyWalk = false;
                  Game1.player.setRunning(true, true);
                  if (Game1.player.mount != null)
                    Game1.player.mount.farmerPassesThrough = true;
                  this.forceWarpTimer = 8000;
                }
                if (this.forceWarpTimer > 0)
                {
                  this.forceWarpTimer -= (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                  if (this.forceWarpTimer <= 0)
                  {
                    this.forceWarpTimer = 0;
                    Game1.player.controller = (PathFindController) null;
                    Game1.player.setTileLocation(new Vector2((float) x, (float) y));
                    Game1.player.faceDirection(num);
                    this.OnReachedBoatDeck((Character) Game1.player, (GameLocation) this);
                  }
                }
                return !this._playerPathing;
              }
              break;
          }
          break;
        case 12:
          switch (command_string[0])
          {
            case 'e':
              if (command_string == "extend_plank")
              {
                if (first_run)
                  this._plankDirection = -0.25f;
                return true;
              }
              break;
            case 'o':
              if (command_string == "offset_willy" && first_run)
              {
                this._boatEvent.getActorByName("Willy").drawOffset.Y = -24f;
                break;
              }
              break;
          }
          break;
        case 13:
          if (command_string == "retract_plank")
          {
            if (first_run)
              this._plankDirection = 0.25f;
            return true;
          }
          break;
        case 18:
          switch (command_string[0])
          {
            case 'a':
              if (command_string == "animate_boat_start")
              {
                if (first_run)
                {
                  this._boatAnimating = true;
                  Game1.player.canOnlyWalk = false;
                }
                return true;
              }
              break;
            case 'n':
              if (command_string == "non_blocking_pause")
              {
                if (first_run)
                {
                  this.nonBlockingPause = ArgUtility.GetInt(args, 0);
                  return false;
                }
                this.nonBlockingPause -= (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
                if (this.nonBlockingPause >= 0)
                  return false;
                this.nonBlockingPause = 0;
                return true;
              }
              break;
          }
          break;
      }
    }
    return base.RunLocationSpecificEventCommand(current_event, command_string, first_run, args);
  }

  public virtual void OnReachedBoatDeck(Character character, GameLocation location)
  {
    this._playerPathing = false;
    Game1.player.controller = (PathFindController) null;
    Game1.player.canOnlyWalk = true;
    this.forceWarpTimer = 0;
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    this.UpdateGateTileProperty();
  }

  protected override void resetLocalState()
  {
    this.critters = new List<Critter>();
    base.resetLocalState();
    this.boatTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\WillysBoat");
    if (Game1.random.NextDouble() < 0.10000000149011612)
      this.addCritter((Critter) new CrabCritter(new Vector2(128f, 640f)));
    if (Game1.random.NextDouble() < 0.10000000149011612)
      this.addCritter((Critter) new CrabCritter(new Vector2(576f, 672f)));
    this.ResetBoat();
  }

  public virtual void ResetBoat()
  {
    this._nextSmoke = 0.0f;
    this._nextBubble = 0.0f;
    this._boatAnimating = false;
    this.boatPosition = new Vector2(52f, 36f) * 4f;
    this._gateFrameTimer = 0.0f;
    this._gateDirection = 0;
    this._gateFrame = 0;
    this._boatOffset = 0;
    this._boatDirection = 0;
    this._plankPosition = 0.0f;
    this._plankDirection = 0.0f;
    this.UpdateGateTileProperty();
  }

  public Vector2 GetBoatPosition()
  {
    return this.boatPosition + new Vector2((float) this._boatOffset, 0.0f);
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    Vector2 boatPosition = this.GetBoatPosition();
    if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed") && Game1.farmEvent == null)
    {
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(4, 0, 156, 118)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, this.boatPosition.Y / 10000f);
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition + new Vector2(8f, 0.0f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 160 /*0xA0*/, 128 /*0x80*/, 96 /*0x60*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.boatPosition.Y + 408.0) / 10000.0));
      Vector2 vector2 = Vector2.Zero;
      if (!this.PlankFinishedAnimating() || (double) this._plankShake > 0.0)
        vector2 = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      b.Draw(this.boatTexture, Game1.GlobalToLocal(new Vector2(6f, 9f) * 64f + new Vector2(0.0f, (float) (int) this._plankPosition) * 4f + vector2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 176 /*0xB0*/, 17, 33)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((512.0 + (double) this._plankPosition * 4.0) / 10000.0));
      Microsoft.Xna.Framework.Rectangle gateRect = this.gateRect with
      {
        X = this._gateFrame * this.gateRect.Width
      };
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition + new Vector2(35f, 81f) * 4f), new Microsoft.Xna.Framework.Rectangle?(gateRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.boatPosition.Y + 428.0) / 10000.0));
    }
    else
    {
      b.Draw(this.boatTexture, Game1.GlobalToLocal(boatPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(4, 259, 156, 122)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, this.boatPosition.Y / 10000f);
      b.Draw(this.boatTexture, Game1.GlobalToLocal(new Vector2(6f, 9f) * 64f + new Vector2(0.0f, (float) (int) this._plankPosition) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(128 /*0x80*/, 176 /*0xB0*/, 17, 33)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((512.0 + (double) this._plankPosition * 4.0) / 10000.0));
      float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2));
      if (!Game1.eventUp)
      {
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull"))
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(416f, 456f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 4.0)), SpriteEffects.None, 1f);
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine"))
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(288f, 520f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 4.0)), SpriteEffects.None, 1f);
        if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor"))
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(544f, 520f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 4.0)), SpriteEffects.None, 1f);
      }
    }
    b.Draw(this.boatTexture, Game1.GlobalToLocal(new Vector2(4f, 8f) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(160 /*0xA0*/, 192 /*0xC0*/, 16 /*0x10*/, 32 /*0x20*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0512f);
  }

  public enum TunnelAnimationState
  {
    Idle,
    MoveWillyToGate,
    OpenGate,
    MoveWillyToCockpit,
    MoveFarmer,
    MovePlank,
    CloseGate,
    MoveBoat,
  }
}
