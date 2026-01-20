// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandSouthEast
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Locations;

public class IslandSouthEast : IslandLocation
{
  private const string lightId = "IslandSouthEast";
  [XmlIgnore]
  public Texture2D mermaidSprites;
  [XmlIgnore]
  public int lastPlayedNote = -1;
  [XmlIgnore]
  public int songIndex = -1;
  [XmlIgnore]
  public int[] mermaidIdle = new int[1];
  [XmlIgnore]
  public int[] mermaidWave = new int[4]{ 1, 1, 2, 2 };
  [XmlIgnore]
  public int[] mermaidReward = new int[7]
  {
    3,
    3,
    3,
    3,
    3,
    4,
    4
  };
  [XmlIgnore]
  public int[] mermaidDance = new int[6]{ 5, 5, 5, 6, 6, 6 };
  [XmlIgnore]
  public int mermaidFrameIndex;
  [XmlIgnore]
  public int[] currentMermaidAnimation;
  [XmlIgnore]
  public float mermaidFrameTimer;
  [XmlIgnore]
  public float mermaidDanceTime;
  [XmlIgnore]
  public NetEvent0 mermaidPuzzleSuccess = new NetEvent0();
  [XmlElement("mermaidPuzzleFinished")]
  public NetBool mermaidPuzzleFinished = new NetBool();
  [XmlIgnore]
  public NetEvent0 fishWalnutEvent = new NetEvent0();
  [XmlElement("fishedWalnut")]
  public NetBool fishedWalnut = new NetBool();

  public IslandSouthEast()
  {
  }

  public IslandSouthEast(string map, string name)
    : base(map, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.mermaidPuzzleSuccess, "mermaidPuzzleSuccess").AddField((INetSerializable) this.mermaidPuzzleFinished, "mermaidPuzzleFinished").AddField((INetSerializable) this.fishWalnutEvent, "fishWalnutEvent").AddField((INetSerializable) this.fishedWalnut, "fishedWalnut");
    this.mermaidPuzzleSuccess.onEvent += new NetEvent0.Event(this.OnMermaidPuzzleSuccess);
    this.fishWalnutEvent.onEvent += new NetEvent0.Event(this.OnFishWalnut);
  }

  public virtual void OnMermaidPuzzleSuccess()
  {
    this.currentMermaidAnimation = this.mermaidReward;
    this.mermaidFrameTimer = 0.0f;
    if (Game1.currentLocation == this)
      Game1.playSound("yoba");
    if (!Game1.IsMasterGame || this.mermaidPuzzleFinished.Value)
      return;
    Game1.player.team.MarkCollectedNut("Mermaid");
    this.mermaidPuzzleFinished.Value = true;
    for (int index = 0; index < 5; ++index)
      Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(32f, 33f) * 64f, 0, (GameLocation) this, 0);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (this.IsRainingHere())
    {
      this.setMapTile(16 /*0x10*/, 27, 3, "Back", "untitled tile sheet3", "");
      this.setMapTile(18, 27, 4, "Back", "untitled tile sheet3", "");
      this.setMapTile(20, 27, 5, "Back", "untitled tile sheet3", "");
      this.setMapTile(22, 27, 6, "Back", "untitled tile sheet3", "");
      this.setMapTile(24, 27, 7, "Back", "untitled tile sheet3", "");
      this.setMapTile(26, 27, 8, "Back", "untitled tile sheet3", "");
    }
    else
    {
      this.setMapTile(16 /*0x10*/, 27, 39, "Back", "untitled tile sheet", "");
      this.setMapTile(18, 27, 39, "Back", "untitled tile sheet", "");
      this.setMapTile(20, 27, 39, "Back", "untitled tile sheet", "");
      this.setMapTile(22, 27, 39, "Back", "untitled tile sheet", "");
      this.setMapTile(24, 27, 39, "Back", "untitled tile sheet", "");
      this.setMapTile(26, 27, 39, "Back", "untitled tile sheet", "");
    }
    if (IslandSouthEastCave.isPirateNight())
    {
      this.setMapTile(29, 18, 36, "Buildings", "untitled tile sheet3");
      this.setTileProperty(29, 18, "Buildings", "Passable", "T");
      this.setMapTile(29, 19, 68, "Buildings", "untitled tile sheet3");
      this.setTileProperty(29, 19, "Buildings", "Passable", "T");
      this.setMapTile(30, 18, 99, "Buildings", "untitled tile sheet3");
      this.setTileProperty(30, 18, "Buildings", "Passable", "T");
      this.setMapTile(30, 19, 131, "Buildings", "untitled tile sheet3");
      this.setTileProperty(30, 19, "Buildings", "Passable", "T");
    }
    else
    {
      this.setMapTile(29, 18, 35, "Buildings", "untitled tile sheet3");
      this.setTileProperty(29, 18, "Buildings", "Passable", "T");
      this.setMapTile(29, 19, 67, "Buildings", "untitled tile sheet3");
      this.setTileProperty(29, 19, "Buildings", "Passable", "T");
      this.setMapTile(30, 18, 35, "Buildings", "untitled tile sheet3");
      this.setTileProperty(30, 18, "Buildings", "Passable", "T");
      this.setMapTile(30, 19, 67, "Buildings", "untitled tile sheet3");
      this.setTileProperty(30, 19, "Buildings", "Passable", "T");
    }
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    this.mermaidSprites = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
    if (IslandSouthEastCave.isPirateNight())
    {
      Game1.changeMusicTrack("PIRATE_THEME(muffled)", true, MusicContext.SubLocation);
      if (!this.hasLightSource(nameof (IslandSouthEast)))
        this.sharedLights.AddLight(new LightSource(nameof (IslandSouthEast), 1, new Vector2(30.5f, 18.5f) * 64f, 4f, onlyLocation: this.NameOrUniqueName));
    }
    if (!this.AreMoonlightJelliesOut())
      return;
    this.addMoonlightJellies(50, Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, -24917.0), new Rectangle(0, 0, 0, 0));
  }

  public override void cleanupBeforePlayerExit()
  {
    this.removeLightSource(nameof (IslandSouthEast));
    base.cleanupBeforePlayerExit();
  }

  public override void SetBuriedNutLocations()
  {
    base.SetBuriedNutLocations();
    this.buriedNutPoints.Add(new Point(25, 17));
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.mermaidPuzzleSuccess.Poll();
    this.fishWalnutEvent.Poll();
    if (!this.fishedWalnut.Value && Game1.random.NextDouble() < 0.005)
    {
      this.playSound("waterSlosh");
      this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 8, 0, new Vector2(1216f, 1344f), false, Game1.random.NextBool(), 1f / 1000f, 0.01f, Color.White, 1f, 3f / 1000f, 0.0f, 0.0f));
    }
    if (!this.MermaidIsHere())
      return;
    bool flag = false;
    if (this.mermaidPuzzleFinished.Value)
    {
      foreach (Character farmer in this.farmers)
      {
        Point tilePoint = farmer.TilePoint;
        if (tilePoint.X > 24 && tilePoint.Y > 25)
        {
          flag = true;
          break;
        }
      }
    }
    if (flag && (this.currentMermaidAnimation == null || this.currentMermaidAnimation == this.mermaidIdle))
    {
      this.currentMermaidAnimation = this.mermaidWave;
      this.mermaidFrameIndex = 0;
      this.mermaidFrameTimer = 0.0f;
    }
    if ((double) this.mermaidDanceTime > 0.0)
    {
      if (this.currentMermaidAnimation == null || this.currentMermaidAnimation == this.mermaidIdle)
      {
        this.currentMermaidAnimation = this.mermaidDance;
        this.mermaidFrameTimer = 0.0f;
      }
      this.mermaidDanceTime -= (float) time.ElapsedGameTime.TotalSeconds;
      if ((double) this.mermaidDanceTime < 0.0 && this.currentMermaidAnimation == this.mermaidDance)
      {
        this.currentMermaidAnimation = this.mermaidIdle;
        this.mermaidFrameTimer = 0.0f;
      }
    }
    this.mermaidFrameTimer += (float) time.ElapsedGameTime.TotalSeconds;
    if ((double) this.mermaidFrameTimer <= 0.25)
      return;
    this.mermaidFrameTimer = 0.0f;
    ++this.mermaidFrameIndex;
    if (this.currentMermaidAnimation == null)
    {
      this.mermaidFrameIndex = 0;
    }
    else
    {
      if (this.mermaidFrameIndex < this.currentMermaidAnimation.Length)
        return;
      this.mermaidFrameIndex = 0;
      if (this.currentMermaidAnimation == this.mermaidReward)
      {
        if (flag)
          this.currentMermaidAnimation = this.mermaidWave;
        else
          this.currentMermaidAnimation = this.mermaidIdle;
      }
      else
      {
        if (flag || this.currentMermaidAnimation != this.mermaidWave)
          return;
        this.currentMermaidAnimation = this.mermaidIdle;
      }
    }
  }

  public bool MermaidIsHere() => this.IsRainingHere();

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (!this.MermaidIsHere())
      return;
    int num = 0;
    int mermaidFrameIndex = this.mermaidFrameIndex;
    int? length = this.currentMermaidAnimation?.Length;
    int valueOrDefault = length.GetValueOrDefault();
    if (mermaidFrameIndex < valueOrDefault & length.HasValue)
      num = this.currentMermaidAnimation[this.mermaidFrameIndex];
    b.Draw(this.mermaidSprites, Game1.GlobalToLocal(new Vector2(32f, 32f) * 64f + new Vector2(0.0f, -8f) * 4f), new Rectangle?(new Rectangle(304 + 28 * num, 592, 28, 36)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0009f);
  }

  public override Item getFish(
    float millisecondsAfterNibble,
    string bait,
    int waterDepth,
    Farmer who,
    double baitPotency,
    Vector2 bobberTile,
    string locationName = null)
  {
    if ((int) bobberTile.X < 18 || (int) bobberTile.X > 20 || (int) bobberTile.Y < 20 || (int) bobberTile.Y > 22)
      return base.getFish(millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile, (string) null);
    if (!this.fishedWalnut.Value)
    {
      Game1.player.team.MarkCollectedNut("StardropPool");
      if (!Game1.IsMultiplayer)
      {
        this.fishedWalnut.Value = true;
        return ItemRegistry.Create("(O)73");
      }
      this.fishWalnutEvent.Fire();
    }
    return (Item) null;
  }

  public void OnFishWalnut()
  {
    if (this.fishedWalnut.Value || !Game1.IsMasterGame)
      return;
    Vector2 vector2 = new Vector2(19f, 21f);
    Game1.createItemDebris(ItemRegistry.Create("(O)73"), vector2 * 64f + new Vector2(0.5f, 1.5f) * 64f, 0, (GameLocation) this, 0);
    Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(28, 100f, 2, 1, vector2 * 64f, false, false)
    {
      layerDepth = (float) ((((double) vector2.Y + 0.5) * 64.0 + 2.0) / 10000.0)
    });
    this.playSound("dropItemInWater");
    this.fishedWalnut.Value = true;
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    if (!(l is IslandSouthEast islandSouthEast))
      return;
    this.mermaidPuzzleFinished.Value = islandSouthEast.mermaidPuzzleFinished.Value;
    this.fishedWalnut.Value = islandSouthEast.fishedWalnut.Value;
  }

  public virtual void OnFlutePlayed(int pitch)
  {
    if (!this.MermaidIsHere())
      return;
    if (this.songIndex == -1)
    {
      this.lastPlayedNote = pitch;
      this.songIndex = 0;
    }
    int num = pitch - this.lastPlayedNote;
    if (num == 900)
    {
      this.songIndex = 1;
      this.mermaidDanceTime = 5f;
    }
    else
    {
      switch (this.songIndex)
      {
        case 1:
          if (num == -200)
          {
            ++this.songIndex;
            this.mermaidDanceTime = 5f;
            break;
          }
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
          break;
        case 2:
          if (num == -400)
          {
            ++this.songIndex;
            this.mermaidDanceTime = 5f;
            break;
          }
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
          break;
        case 3:
          if (num == 200)
          {
            this.songIndex = 0;
            this.mermaidPuzzleSuccess.Fire();
            this.mermaidDanceTime = 0.0f;
            break;
          }
          this.songIndex = -1;
          this.mermaidDanceTime = 0.0f;
          this.currentMermaidAnimation = this.mermaidIdle;
          break;
      }
    }
    this.lastPlayedNote = pitch;
  }
}
