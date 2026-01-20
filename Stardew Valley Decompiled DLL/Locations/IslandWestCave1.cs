// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandWestCave1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandWestCave1 : IslandLocation
{
  public const string lightSourceId = "IslandWestCave1";
  [XmlIgnore]
  protected List<IslandWestCave1.CaveCrystal> crystals = new List<IslandWestCave1.CaveCrystal>();
  public const int PHASE_INTRO = 0;
  public const int PHASE_PLAY_SEQUENCE = 1;
  public const int PHASE_WAIT_FOR_PLAYER_INPUT = 2;
  public const int PHASE_NOTHING = 3;
  public const int PHASE_SUCCESSFUL_SEQUENCE = 4;
  public const int PHASE_OUTRO = 5;
  [XmlElement("completed")]
  public NetBool completed = new NetBool();
  [XmlIgnore]
  public NetBool isActivated = new NetBool(false);
  [XmlIgnore]
  public NetFloat netPhaseTimer = new NetFloat();
  [XmlIgnore]
  public float localPhaseTimer;
  [XmlIgnore]
  public float betweenNotesTimer;
  [XmlIgnore]
  public int localPhase;
  [XmlIgnore]
  public NetInt netPhase = new NetInt(3);
  [XmlIgnore]
  public NetInt currentDifficulty = new NetInt(2);
  [XmlIgnore]
  public NetInt currentCrystalSequenceIndex = new NetInt(0);
  [XmlIgnore]
  public int currentPlaybackCrystalSequenceIndex;
  [XmlIgnore]
  public NetInt timesFailed = new NetInt(0);
  [XmlIgnore]
  public NetList<int, NetInt> currentCrystalSequence = new NetList<int, NetInt>();
  [XmlIgnore]
  public NetEvent1Field<int, NetInt> enterValueEvent = new NetEvent1Field<int, NetInt>();

  public IslandWestCave1()
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.netPhase, "netPhase").AddField((INetSerializable) this.isActivated, "isActivated").AddField((INetSerializable) this.currentDifficulty, "currentDifficulty").AddField((INetSerializable) this.currentCrystalSequenceIndex, "currentCrystalSequenceIndex").AddField((INetSerializable) this.currentCrystalSequence, "currentCrystalSequence").AddField(this.enterValueEvent.NetFields, "enterValueEvent.NetFields").AddField((INetSerializable) this.netPhaseTimer, "netPhaseTimer").AddField((INetSerializable) this.completed, "completed").AddField((INetSerializable) this.timesFailed, "timesFailed");
    this.enterValueEvent.onEvent += new AbstractNetEvent1<int>.Event(this.enterValue);
    this.isActivated.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.onActivationChanged);
  }

  public IslandWestCave1(string map, string name)
    : base(map, name)
  {
  }

  public void onActivationChanged(NetBool field, bool old_value, bool new_value)
  {
    this.updateActivationVisuals();
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    this.resetPuzzle();
  }

  public void resetPuzzle()
  {
    this.isActivated.Value = false;
    this.updateActivationVisuals();
    this.netPhase.Value = 3;
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    this.UpdateActivationTiles();
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (this.crystals.Count == 0)
    {
      this.crystals.Add(new IslandWestCave1.CaveCrystal()
      {
        tileLocation = new Vector2(3f, 4f),
        color = new Color(220, 0, (int) byte.MaxValue),
        currentColor = new Color(220, 0, (int) byte.MaxValue),
        id = 1,
        pitch = 0
      });
      this.crystals.Add(new IslandWestCave1.CaveCrystal()
      {
        tileLocation = new Vector2(4f, 6f),
        color = Color.Lime,
        currentColor = Color.Lime,
        id = 2,
        pitch = 700
      });
      this.crystals.Add(new IslandWestCave1.CaveCrystal()
      {
        tileLocation = new Vector2(6f, 7f),
        color = new Color((int) byte.MaxValue, 50, 100),
        currentColor = new Color((int) byte.MaxValue, 50, 100),
        id = 3,
        pitch = 1200
      });
      this.crystals.Add(new IslandWestCave1.CaveCrystal()
      {
        tileLocation = new Vector2(8f, 6f),
        color = new Color(0, 200, (int) byte.MaxValue),
        currentColor = new Color(0, 200, (int) byte.MaxValue),
        id = 4,
        pitch = 1400
      });
      this.crystals.Add(new IslandWestCave1.CaveCrystal()
      {
        tileLocation = new Vector2(9f, 4f),
        color = new Color((int) byte.MaxValue, 180, 0),
        currentColor = new Color((int) byte.MaxValue, 180, 0),
        id = 5,
        pitch = 1600
      });
    }
    this.updateActivationVisuals();
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (who.IsLocalPlayer)
    {
      switch (ArgUtility.Get(action, 0))
      {
        case "Crystal":
          int num;
          string error;
          if (!ArgUtility.TryGetInt(action, 1, out num, out error, "int crystalId"))
          {
            this.LogTileActionError(action, tileLocation.X, tileLocation.Y, error);
            return false;
          }
          if (this.netPhase.Value == 5 || this.netPhase.Value == 3 || this.netPhase.Value == 2)
          {
            this.enterValueEvent.Fire(num);
            return true;
          }
          break;
        case "CrystalCaveActivate":
          if (!this.isActivated.Value && !this.completed.Value)
          {
            this.isActivated.Value = true;
            Game1.playSound("openBox");
            this.updateActivationVisuals();
            this.netPhaseTimer.Value = 1200f;
            this.netPhase.Value = 0;
            this.currentDifficulty.Value = 2;
            return true;
          }
          break;
      }
    }
    return base.performAction(action, who, tileLocation);
  }

  public virtual void updateActivationVisuals()
  {
    if (this.map == null || Game1.gameMode == (byte) 6 || Game1.currentLocation != this)
      return;
    if (this.isActivated.Value || this.completed.Value)
      Game1.currentLightSources.Add(new LightSource(nameof (IslandWestCave1), 1, new Vector2(6.5f, 1f) * 64f, 2f, Color.Black, onlyLocation: this.NameOrUniqueName));
    else
      Utility.removeLightSource(nameof (IslandWestCave1));
    this.UpdateActivationTiles();
    if (!this.completed.Value)
      return;
    this.addCompletionTorches();
  }

  public virtual void UpdateActivationTiles()
  {
    if (this.map == null || Game1.gameMode == (byte) 6 || Game1.currentLocation != this)
      return;
    this.setMapTile(6, 1, this.isActivated.Value || this.completed.Value ? 33 : 31 /*0x1F*/, "Buildings", "untitled tile sheet");
  }

  public virtual void enterValue(int which)
  {
    if (this.netPhase.Value == 2 && Game1.IsMasterGame && this.currentCrystalSequence.Count > this.currentCrystalSequenceIndex.Value)
    {
      if (this.currentCrystalSequence[this.currentCrystalSequenceIndex.Value] == which - 1)
      {
        ++this.currentCrystalSequenceIndex.Value;
        if (this.currentCrystalSequenceIndex.Value >= this.currentCrystalSequence.Count)
        {
          DelayedAction.playSoundAfterDelay(this.currentDifficulty.Value == 7 ? "discoverMineral" : "newArtifact", 500, (GameLocation) this);
          this.netPhaseTimer.Value = 2000f;
          this.netPhase.Value = 4;
        }
      }
      else
      {
        this.playSound("cancel");
        this.resetPuzzle();
        ++this.timesFailed.Value;
        return;
      }
    }
    if (this.crystals.Count <= which - 1)
      return;
    this.crystals[which - 1].activate();
  }

  public override void cleanupBeforePlayerExit()
  {
    this.crystals.Clear();
    base.cleanupBeforePlayerExit();
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    this.enterValueEvent.Poll();
    if ((this.localPhase != 1 || this.currentPlaybackCrystalSequenceIndex < 0 || this.currentPlaybackCrystalSequenceIndex >= this.currentCrystalSequence.Count) && this.localPhase != this.netPhase.Value)
    {
      this.localPhaseTimer = this.netPhaseTimer.Value;
      this.localPhase = this.netPhase.Value;
      this.currentPlaybackCrystalSequenceIndex = this.localPhase == 1 ? 0 : -1;
    }
    base.UpdateWhenCurrentLocation(time);
    foreach (IslandWestCave1.CaveCrystal crystal in this.crystals)
      crystal.update();
    TimeSpan elapsedGameTime;
    if ((double) this.localPhaseTimer > 0.0)
    {
      double localPhaseTimer = (double) this.localPhaseTimer;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
      this.localPhaseTimer = (float) (localPhaseTimer - totalMilliseconds);
      if ((double) this.localPhaseTimer <= 0.0)
      {
        switch (this.localPhase)
        {
          case 0:
          case 4:
            this.currentPlaybackCrystalSequenceIndex = 0;
            if (Game1.IsMasterGame)
            {
              ++this.currentDifficulty.Value;
              this.currentCrystalSequence.Clear();
              this.currentCrystalSequenceIndex.Value = 0;
              if (this.currentDifficulty.Value > (this.timesFailed.Value < 8 ? 7 : 6))
              {
                this.netPhaseTimer.Value = 10f;
                this.netPhase.Value = 5;
                break;
              }
              for (int index = 0; index < this.currentDifficulty.Value; ++index)
                this.currentCrystalSequence.Add(Game1.random.Next(5));
              this.netPhase.Value = 1;
            }
            this.betweenNotesTimer = 600f;
            break;
          case 5:
            if (Game1.currentLocation == this)
            {
              Game1.playSound("fireball");
              Utility.addSmokePuff((GameLocation) this, new Vector2(5f, 1f) * 64f);
              Utility.addSmokePuff((GameLocation) this, new Vector2(7f, 1f) * 64f);
            }
            if (Game1.IsMasterGame)
            {
              Game1.player.team.MarkCollectedNut("IslandWestCavePuzzle");
              Game1.createObjectDebris("(O)73", 5, 1, (GameLocation) this);
              Game1.createObjectDebris("(O)73", 7, 1, (GameLocation) this);
              Game1.createObjectDebris("(O)73", 6, 1, (GameLocation) this);
            }
            this.completed.Value = true;
            if (Game1.currentLocation == this)
            {
              this.addCompletionTorches();
              break;
            }
            break;
        }
      }
    }
    if (this.localPhase != 1)
      return;
    double betweenNotesTimer = (double) this.betweenNotesTimer;
    elapsedGameTime = time.ElapsedGameTime;
    double totalMilliseconds1 = elapsedGameTime.TotalMilliseconds;
    this.betweenNotesTimer = (float) (betweenNotesTimer - totalMilliseconds1);
    if ((double) this.betweenNotesTimer > 0.0 || this.currentCrystalSequence.Count <= 0 || this.currentPlaybackCrystalSequenceIndex < 0)
      return;
    int index1 = this.currentCrystalSequence[this.currentPlaybackCrystalSequenceIndex];
    if (index1 < this.crystals.Count)
      this.crystals[index1].activate();
    ++this.currentPlaybackCrystalSequenceIndex;
    int num = this.currentDifficulty.Value;
    if (this.currentDifficulty.Value > 5)
    {
      --num;
      if (this.timesFailed.Value >= 4)
        --num;
      if (this.timesFailed.Value >= 6)
        --num;
      if (this.timesFailed.Value >= 8)
        num = 3;
    }
    else if (this.timesFailed.Value >= 4 && this.currentDifficulty.Value > 4)
      --num;
    this.betweenNotesTimer = 1500f / (float) num;
    if (this.currentDifficulty.Value > (this.timesFailed.Value < 8 ? 7 : 6))
      this.betweenNotesTimer = 100f;
    if (this.currentPlaybackCrystalSequenceIndex < this.currentCrystalSequence.Count)
      return;
    this.currentPlaybackCrystalSequenceIndex = -1;
    if (this.currentDifficulty.Value > (this.timesFailed.Value < 8 ? 7 : 6))
    {
      if (!Game1.IsMasterGame)
        return;
      this.netPhaseTimer.Value = 1000f;
      this.netPhase.Value = 5;
    }
    else
    {
      if (!Game1.IsMasterGame)
        return;
      this.netPhase.Value = 2;
      this.currentCrystalSequenceIndex.Value = 0;
    }
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    if (!(l is IslandWestCave1 islandWestCave1))
      return;
    this.completed.Value = islandWestCave1.completed.Value;
  }

  public void addCompletionTorches()
  {
    if (!this.completed.Value)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(5f, 1f) * 64f + new Vector2(0.0f, -20f), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandWestCave1_Torch_1",
      lightRadius = 2f,
      scale = 4f,
      layerDepth = 0.013439999f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(7f, 1f) * 64f + new Vector2(8f, -20f), false, 0.0f, Color.White)
    {
      interval = 50f,
      totalNumberOfLoops = 99999,
      animationLength = 4,
      lightId = "IslandWestCave1_Torch_2",
      lightRadius = 2f,
      scale = 4f,
      layerDepth = 0.013439999f
    });
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    foreach (IslandWestCave1.CaveCrystal crystal in this.crystals)
      crystal.draw(b);
  }

  public class CaveCrystal
  {
    public Vector2 tileLocation;
    public int id;
    public int pitch;
    public Color color;
    public Color currentColor;
    public float shakeTimer;
    public float glowTimer;

    public void update()
    {
      if ((double) this.glowTimer > 0.0)
      {
        this.glowTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
        this.currentColor.R = (byte) Utility.Lerp((float) this.color.R, (float) byte.MaxValue, this.glowTimer / 1000f);
        this.currentColor.G = (byte) Utility.Lerp((float) this.color.G, (float) byte.MaxValue, this.glowTimer / 1000f);
        this.currentColor.B = (byte) Utility.Lerp((float) this.color.B, (float) byte.MaxValue, this.glowTimer / 1000f);
      }
      if ((double) this.shakeTimer <= 0.0)
        return;
      this.shakeTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public void activate()
    {
      this.glowTimer = 1000f;
      this.shakeTimer = 100f;
      Game1.playSound("crystal", new int?(this.pitch));
      this.currentColor = this.color;
    }

    public void draw(SpriteBatch b)
    {
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(this.tileLocation * 64f + new Vector2(8f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(188, 228, 52, 28)), this.currentColor, 0.0f, new Vector2(52f, 28f) / 2f, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y * 64.0 + 64.0 - 8.0) / 10000.0));
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(this.tileLocation * 64f + new Vector2(0.0f, -52f) + new Vector2((double) this.shakeTimer > 0.0 ? (float) Game1.random.Next(-1, 2) : 0.0f, (double) this.shakeTimer > 0.0 ? (float) Game1.random.Next(-1, 2) : 0.0f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(240 /*0xF0*/, 227, 16 /*0x10*/, 29)), this.currentColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) this.tileLocation.Y * 64.0 + 64.0 - 4.0) / 10000.0));
    }
  }
}
