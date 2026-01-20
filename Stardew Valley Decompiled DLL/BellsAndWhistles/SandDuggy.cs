// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.SandDuggy
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class SandDuggy : INetObject<NetFields>
{
  [XmlIgnore]
  public NetList<Point, NetPoint> holeLocations = new NetList<Point, NetPoint>();
  [XmlIgnore]
  public int frame;
  [XmlIgnore]
  public NetInt currentHoleIndex = new NetInt(0);
  [XmlIgnore]
  public int _localIndex;
  [XmlIgnore]
  public NetLocationRef locationRef = new NetLocationRef();
  [XmlIgnore]
  public SandDuggy.State currentState;
  [XmlIgnore]
  public Texture2D texture;
  [XmlIgnore]
  public float nextFrameUpdate;
  [XmlElement("whacked")]
  public NetBool whacked = new NetBool(false);

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (SandDuggy));

  public SandDuggy() => this.InitNetFields();

  public SandDuggy(GameLocation location, Point[] points)
    : this()
  {
    this.locationRef.Value = location;
    foreach (Point point in points)
      this.holeLocations.Add(point);
    this.currentHoleIndex.Value = this.FindRandomFreePoint();
  }

  public virtual int FindRandomFreePoint()
  {
    if (this.locationRef.Value == null)
      return -1;
    List<int> options = new List<int>();
    for (int index = 0; index < this.holeLocations.Count; ++index)
    {
      Point holeLocation = this.holeLocations[index];
      if (!this.locationRef.Value.isObjectAtTile(holeLocation.X, holeLocation.Y) && !this.locationRef.Value.isTerrainFeatureAt(holeLocation.X, holeLocation.Y) && !this.locationRef.Value.terrainFeatures.ContainsKey(Utility.PointToVector2(holeLocation)))
        options.Add(index);
    }
    if (options.Count == 1)
      return options[0];
    options.RemoveAll((Predicate<int>) (index =>
    {
      Point holeLocation = this.holeLocations[index];
      foreach (Farmer farmer in this.locationRef.Value.farmers)
      {
        if (this.NearFarmer(holeLocation, farmer))
          return true;
      }
      return false;
    }));
    return options.Count > 0 ? Game1.random.ChooseFrom<int>((IList<int>) options) : -1;
  }

  public virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.holeLocations, "holeLocations").AddField((INetSerializable) this.currentHoleIndex, "currentHoleIndex").AddField((INetSerializable) this.locationRef.NetFields, "locationRef.NetFields").AddField((INetSerializable) this.whacked, "whacked");
    this.whacked.fieldChangeVisibleEvent += new FieldChange<NetBool, bool>(this.OnWhackedChanged);
  }

  public virtual void OnWhackedChanged(NetBool field, bool old_value, bool new_value)
  {
    if (Game1.gameMode == (byte) 6 || Utility.ShouldIgnoreValueChangeCallback() || !this.whacked.Value)
      return;
    if (Game1.IsMasterGame)
    {
      int index = this.currentHoleIndex.Value;
      if (index == -1)
        index = 0;
      Game1.player.team.MarkCollectedNut(nameof (SandDuggy));
      Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2((float) this.holeLocations[index].X, (float) this.holeLocations[index].Y) * 64f, -1, this.locationRef.Value);
    }
    if (Game1.currentLocation != this.locationRef.Value)
      return;
    this.AnimateWhacked();
  }

  public virtual void AnimateWhacked()
  {
    if (Game1.currentLocation != this.locationRef.Value)
      return;
    int index = this.currentHoleIndex.Value;
    if (index == -1)
      index = 0;
    Vector2 vector2 = new Vector2((float) this.holeLocations[index].X, (float) this.holeLocations[index].Y);
    int ground_position = (int) ((double) vector2.Y * 64.0 - 32.0);
    if (Utility.isOnScreen((vector2 + new Vector2(0.5f, 0.5f)) * 64f, 64 /*0x40*/))
    {
      Game1.playSound("axchop");
      Game1.playSound("rockGolemHit");
    }
    TemporaryAnimatedSprite duggy_sprite = new TemporaryAnimatedSprite("LooseSprites/SandDuggy", new Rectangle(0, 48 /*0x30*/, 16 /*0x10*/, 48 /*0x30*/), new Vector2(vector2.X * 64f, (float) ((double) vector2.Y * 64.0 - 32.0)), false, 0.0f, Color.White)
    {
      motion = new Vector2(2f, -3f),
      acceleration = new Vector2(0.0f, 0.25f),
      interval = 1000f,
      animationLength = 1,
      alphaFade = 0.02f,
      layerDepth = 0.07682f,
      scale = 4f,
      yStopCoordinate = ground_position
    };
    duggy_sprite.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (extra_info =>
    {
      duggy_sprite.motion.Y = -3f;
      duggy_sprite.acceleration.Y = 0.25f;
      duggy_sprite.yStopCoordinate = ground_position;
      duggy_sprite.flipped = !duggy_sprite.flipped;
    });
    Game1.currentLocation.temporarySprites.Add(duggy_sprite);
  }

  public virtual void ResetForPlayerEntry()
  {
    this.texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SandDuggy");
  }

  public virtual void PerformToolAction(Tool tool, int tile_x, int tile_y)
  {
    if (this.currentState != SandDuggy.State.Idle || this._localIndex < 0)
      return;
    Point holeLocation = this.holeLocations[this._localIndex];
    if (holeLocation.X != tile_x || holeLocation.Y != tile_y)
      return;
    this.whacked.Value = true;
  }

  public virtual bool NearFarmer(Point location, Farmer farmer)
  {
    return Math.Abs(location.X - farmer.TilePoint.X) <= 2 && Math.Abs(location.Y - farmer.TilePoint.Y) <= 2;
  }

  public virtual void Update(GameTime time)
  {
    if (this.whacked.Value)
      return;
    if (this.currentHoleIndex.Value >= 0 && this.NearFarmer(this.holeLocations[this.currentHoleIndex.Value], Game1.player) && this.FindRandomFreePoint() != this.currentHoleIndex.Value)
    {
      this.currentHoleIndex.Value = -1;
      DelayedAction.playSoundAfterDelay(Game1.random.NextDouble() < 0.1 ? "cowboy_gopher" : "tinyWhip", 200);
    }
    this.nextFrameUpdate -= (float) time.ElapsedGameTime.TotalSeconds;
    if (this.currentHoleIndex.Value < 0 && Game1.IsMasterGame)
      this.currentHoleIndex.Value = this.FindRandomFreePoint();
    if (this.currentState == SandDuggy.State.DigDown && this.frame == 0)
    {
      if (this.currentHoleIndex.Value >= 0)
        this.currentState = SandDuggy.State.DigUp;
      this._localIndex = this.currentHoleIndex.Value;
    }
    if (this.currentHoleIndex.Value == -1 || this.currentHoleIndex.Value != this._localIndex)
      this.currentState = SandDuggy.State.DigDown;
    if ((double) this.nextFrameUpdate > 0.0)
      return;
    if (this._localIndex >= 0)
    {
      switch (this.currentState)
      {
        case SandDuggy.State.DigUp:
          if (this._localIndex >= 0)
          {
            ++this.frame;
            if (this.frame >= 4)
            {
              this.currentState = SandDuggy.State.Idle;
              break;
            }
            break;
          }
          break;
        case SandDuggy.State.Idle:
          ++this.frame;
          if (this.frame > 7)
          {
            this.frame = 4;
            break;
          }
          break;
        case SandDuggy.State.DigDown:
          --this.frame;
          if (this.frame <= 0)
          {
            this.frame = 0;
            break;
          }
          break;
      }
    }
    this.nextFrameUpdate = 0.075f;
  }

  public virtual void Draw(SpriteBatch b)
  {
    if (this.whacked.Value || this._localIndex < 0)
      return;
    Point holeLocation = this.holeLocations[this._localIndex];
    Vector2 globalPosition = (new Vector2((float) holeLocation.X, (float) holeLocation.Y) + new Vector2(0.5f, 0.5f)) * 64f;
    b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(new Rectangle(this.frame % 4 * 16 /*0x10*/, this.frame / 4 * 24, 16 /*0x10*/, 24)), Color.White, 0.0f, new Vector2(8f, 20f), 4f, SpriteEffects.None, globalPosition.Y / 10000f);
  }

  public enum State
  {
    DigUp,
    Idle,
    DigDown,
  }
}
