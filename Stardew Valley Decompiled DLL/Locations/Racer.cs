// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Racer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Locations;

public class Racer : INetObject<NetFields>
{
  public NetBool moving = new NetBool();
  public Vector2? lastPosition;
  public NetPosition position = new NetPosition();
  public NetInt direction = new NetInt();
  public float horizontalPosition = -1f;
  public int currentTrackIndex = -1;
  public Vector2 segmentStart = Vector2.Zero;
  public Vector2 segmentEnd = Vector2.Zero;
  public NetVector2 jumpSegmentStart = new NetVector2();
  public NetVector2 jumpSegmentEnd = new NetVector2();
  public NetBool jumping = new NetBool();
  public NetBool tripping = new NetBool();
  public NetBool drawAboveMap = new NetBool();
  public float moveSpeed = 3f;
  public float minMoveSpeed = 3f;
  public float maxMoveSpeed = 6f;
  public float height;
  public float tripTimer;
  public NetInt racerIndex = new NetInt();
  protected Texture2D _texture;
  public bool frame;
  public float nextFrameSwap;
  public float burstDuration;
  public float nextBurst;
  public float extraLuck;
  public float gravity;
  public int _tripLeaps;
  public float progress;
  public NetInt sabotages = new NetInt(0);

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields("DesertFestival.Racer");

  public Racer()
  {
    this.InitNetFields();
    this.direction.Value = 3;
    this._texture = Game1.content.Load<Texture2D>("LooseSprites\\DesertRacers");
  }

  public Racer(int index)
    : this()
  {
    this.racerIndex.Value = index;
    this.ResetMoveSpeed();
  }

  public virtual void ResetMoveSpeed()
  {
    this.minMoveSpeed = 1.5f;
    this.maxMoveSpeed = 4f;
    this.extraLuck = Utility.RandomFloat(-0.25f, 0.25f);
    if (this.racerIndex.Value == 3)
    {
      this.minMoveSpeed = 0.5f;
      this.maxMoveSpeed = 3.5f;
    }
    this.SpeedBurst();
  }

  private void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.racerIndex, "racerIndex").AddField((INetSerializable) this.position.NetFields, "position.NetFields").AddField((INetSerializable) this.direction, "direction").AddField((INetSerializable) this.jumpSegmentStart, "jumpSegmentStart").AddField((INetSerializable) this.jumpSegmentEnd, "jumpSegmentEnd").AddField((INetSerializable) this.jumping, "jumping").AddField((INetSerializable) this.drawAboveMap, "drawAboveMap").AddField((INetSerializable) this.tripping, "tripping").AddField((INetSerializable) this.sabotages, "sabotages").AddField((INetSerializable) this.moving, "moving");
    this.jumpSegmentStart.Interpolated(false, false);
    this.jumpSegmentEnd.Interpolated(false, false);
  }

  public virtual void UpdateRaceProgress(DesertFestival location)
  {
    if (this.currentTrackIndex < 0)
    {
      this.progress = (float) location.raceTrack.Length;
    }
    else
    {
      Vector2 vector2_1 = this.segmentEnd - this.segmentStart;
      float num1 = vector2_1.Length();
      vector2_1.Normalize();
      Vector2 vector2_2 = this.position.Value - this.segmentStart;
      float num2 = Vector2.Dot(vector2_1, vector2_2);
      if ((double) num1 > 0.0)
        num1 = num2 / num1;
      this.progress = (float) this.currentTrackIndex + num1;
    }
  }

  public virtual void Update(DesertFestival location)
  {
    if (Game1.IsMasterGame)
    {
      bool flag = false;
      if (location.currentRaceState.Value == DesertFestival.RaceState.StartingLine && this.currentTrackIndex < 0)
      {
        if ((double) this.horizontalPosition < 0.0)
          this.horizontalPosition = (float) location.netRacers.IndexOf(this) / (float) (location.racerCount - 1);
        this.currentTrackIndex = 0;
        Vector3 trackPosition = location.GetTrackPosition(this.currentTrackIndex, this.horizontalPosition);
        this.segmentStart = this.position.Value;
        this.segmentEnd = new Vector2(trackPosition.X, trackPosition.Y);
      }
      float val2 = this.maxMoveSpeed;
      if (location.currentRaceState.Value == DesertFestival.RaceState.Go)
      {
        if (location.finishedRacers.Count <= 0)
        {
          if ((double) this.burstDuration > 0.0)
          {
            this.moveSpeed = this.maxMoveSpeed;
            this.burstDuration -= (float) Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
            if ((double) this.burstDuration <= 0.0)
            {
              this.burstDuration = 0.0f;
              this.nextBurst = Utility.RandomFloat(0.75f, 1.5f);
              if (Game1.random.NextDouble() + (double) this.extraLuck < 0.25)
                this.nextBurst *= 0.5f;
              if (this.racerIndex.Value == 3)
                this.nextBurst *= 0.25f;
              float val1 = (float) location.raceTrack.Length;
              foreach (Racer netRacer in location.netRacers)
                val1 = Math.Min(val1, netRacer.progress);
              if ((double) this.progress > (double) val1 && Game1.random.NextDouble() < (double) Math.Min((float) (0.05000000074505806 + (double) this.sabotages.Value * 0.20000000298023224), 0.5f))
              {
                this.tripping.Value = true;
                this.tripTimer = Utility.RandomFloat(1.5f, 2f);
              }
            }
          }
          else if ((double) this.nextBurst > 0.0)
          {
            this.moveSpeed = Utility.MoveTowards(this.moveSpeed, this.minMoveSpeed, 0.5f);
            this.nextBurst -= (float) Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
            if ((double) this.nextBurst <= 0.0)
            {
              this.SpeedBurst();
              this.nextBurst = 0.0f;
            }
          }
          val2 = this.moveSpeed;
        }
        if ((double) this.tripTimer > 0.0)
        {
          this.tripTimer -= (float) Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
          if ((double) this.tripTimer < 0.0)
          {
            this.tripTimer = 0.0f;
            this.tripping.Value = false;
          }
        }
      }
      if (this.jumping.Value)
        val2 = (double) (this.segmentEnd - this.segmentStart).Length() / 64.0 <= 3.0 ? 3f : 6f;
      else if (this.tripping.Value)
        val2 = 0.25f;
      if (this.segmentStart == this.segmentEnd && this.position.Value == this.segmentEnd && this.currentTrackIndex < 0)
        val2 = 0.0f;
      while ((double) val2 > 0.0)
      {
        float num = Math.Min((this.segmentEnd - this.position.Value).Length(), val2);
        val2 -= num;
        Vector2 vector2 = this.segmentEnd - this.position.Value;
        if ((double) vector2.X != 0.0 || (double) vector2.Y != 0.0)
        {
          vector2.Normalize();
          NetPosition position = this.position;
          position.Value = position.Value + vector2 * num;
          flag = true;
          if ((double) Math.Abs(vector2.Y) > (double) Math.Abs(vector2.X))
          {
            if ((double) vector2.Y < 0.0)
              this.direction.Value = 0;
            else
              this.direction.Value = 2;
          }
          else if ((double) vector2.X < 0.0)
            this.direction.Value = 3;
          else
            this.direction.Value = 1;
        }
        if ((double) (this.position.Value - this.segmentEnd).Length() < 0.0099999997764825821)
        {
          this.position.Value = this.segmentEnd;
          if (location.currentRaceState.Value == DesertFestival.RaceState.Go && this.currentTrackIndex >= 0)
          {
            Vector3 trackPosition1 = location.GetTrackPosition(this.currentTrackIndex, this.horizontalPosition);
            if ((double) trackPosition1.Z > 0.0)
            {
              this.tripping.Value = false;
              this.tripTimer = 0.0f;
              this.jumping.Value = true;
            }
            else
              this.jumping.Value = false;
            float z = trackPosition1.Z;
            if ((double) z != 2.0)
            {
              if ((double) z == 3.0)
                this.drawAboveMap.Value = false;
            }
            else
              this.drawAboveMap.Value = true;
            ++this.currentTrackIndex;
            if (this.currentTrackIndex >= location.raceTrack.Length)
            {
              this.currentTrackIndex = -2;
              this.segmentStart = this.segmentEnd;
              this.segmentEnd = new Vector2(44.5f, 37.5f - (float) location.finishedRacers.Count) * 64f;
              this.horizontalPosition = (float) (location.racerCount - 1 - location.finishedRacers.Count) / (float) (location.racerCount - 1);
              location.finishedRacers.Add(this.racerIndex.Value);
              if (location.finishedRacers.Count == 1)
              {
                location.announceRaceEvent.Fire("Race_Finish");
                location.OnRaceWon(this.racerIndex.Value);
              }
            }
            else
            {
              Vector3 trackPosition2 = location.GetTrackPosition(this.currentTrackIndex, this.horizontalPosition);
              this.segmentStart = this.segmentEnd;
              this.segmentEnd = new Vector2(trackPosition2.X, trackPosition2.Y);
            }
            if (this.jumping.Value)
            {
              this.jumpSegmentStart.Value = this.segmentStart;
              this.jumpSegmentEnd.Value = this.segmentEnd;
            }
          }
          else
          {
            val2 = 0.0f;
            this.segmentStart = this.segmentEnd;
            if (location.currentRaceState.Value >= DesertFestival.RaceState.StartingLine && location.currentRaceState.Value < DesertFestival.RaceState.Go)
              this.direction.Value = 0;
            else
              this.direction.Value = 3;
          }
        }
      }
      this.moving.Value = flag;
    }
    if (!this.lastPosition.HasValue)
      this.lastPosition = new Vector2?(this.position.Value);
    for (this.nextFrameSwap -= (this.lastPosition.Value - this.position.Value).Length(); (double) this.nextFrameSwap <= 0.0; this.nextFrameSwap += 8f)
      this.frame = !this.frame;
    this.lastPosition = new Vector2?(this.position.Value);
    if (!this.jumping.Value)
    {
      if (this.moving.Value)
      {
        if (this.tripping.Value && (double) this.height == 0.0)
        {
          this.gravity = this._tripLeaps != 0 ? Utility.RandomFloat(0.5f, 0.75f) : 1f;
          ++this._tripLeaps;
        }
        else if (this.racerIndex.Value == 2 && (double) this.height == 0.0)
          this.gravity = Utility.RandomFloat(0.25f, 0.5f);
      }
      if ((double) this.height != 0.0 || (double) this.gravity != 0.0)
      {
        this.height += this.gravity;
        this.gravity -= (float) (Game1.currentGameTime.ElapsedGameTime.TotalSeconds * 2.0);
        if ((double) this.gravity == 0.0)
          this.gravity = -0.0001f;
        if ((double) this.height <= 0.0)
        {
          this.gravity = 0.0f;
          this.height = 0.0f;
        }
      }
    }
    if (!this.tripping.Value)
      this._tripLeaps = 0;
    if (this.jumping.Value)
    {
      Vector2 vector2_1 = this.jumpSegmentEnd.Value - this.jumpSegmentStart.Value;
      float num1 = vector2_1.Length();
      vector2_1.Normalize();
      Vector2 vector2_2 = this.position.Value - this.jumpSegmentStart.Value;
      float num2 = Vector2.Dot(vector2_1, vector2_2);
      if ((double) num1 <= 0.0)
        return;
      this.height = (float) Math.Sin((double) Utility.Clamp(num2 / num1, 0.0f, 1f) * Math.PI) * 48f;
    }
    else
    {
      if ((double) this.gravity != 0.0)
        return;
      this.height = 0.0f;
    }
  }

  public virtual void SpeedBurst()
  {
    this.burstDuration = Utility.RandomFloat(0.25f, 1f);
    if (Game1.random.NextDouble() + (double) this.extraLuck < 0.25)
      this.burstDuration *= 2f;
    if (this.racerIndex.Value == 3)
      this.burstDuration *= 0.25f;
    this.moveSpeed = this.maxMoveSpeed;
  }

  public virtual void Draw(SpriteBatch sb)
  {
    float layerDepth = (float) (((double) this.position.Y + (double) this.racerIndex.Value * 0.10000000149011612) / 10000.0);
    float num = Utility.Clamp((float) (1.0 - (double) this.height / 12.0), 0.0f, 1f);
    sb.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position.Value), new Rectangle?(), Color.White * 0.75f * num, 0.0f, new Vector2((float) (Game1.shadowTexture.Width / 2), (float) (Game1.shadowTexture.Height / 2)), new Vector2(3f, 3f), SpriteEffects.None, (float) ((double) layerDepth / 10000.0 - 1.0000000116860974E-07));
    SpriteEffects effects = SpriteEffects.None;
    Rectangle rectangle = new Rectangle(0, 0, 16 /*0x10*/, 16 /*0x10*/);
    rectangle.Y = this.racerIndex.Value * 16 /*0x10*/;
    switch (this.direction.Value)
    {
      case 0:
        rectangle.X = 0;
        break;
      case 1:
        rectangle.X = 32 /*0x20*/;
        break;
      case 2:
        rectangle.X = 64 /*0x40*/;
        break;
      case 3:
        rectangle.X = 32 /*0x20*/;
        effects = SpriteEffects.FlipHorizontally;
        break;
    }
    if (this.frame)
      rectangle.X += 16 /*0x10*/;
    Vector2 zero = Vector2.Zero;
    if (this.tripping.Value)
    {
      rectangle.X = 96 /*0x60*/;
      zero.X += (float) Game1.random.Next(-1, 2) * 0.5f;
      zero.Y += (float) Game1.random.Next(-1, 2) * 0.5f;
    }
    sb.Draw(this._texture, Game1.GlobalToLocal(this.position.Value + new Vector2(zero.X, -this.height + zero.Y) * 4f), new Rectangle?(rectangle), Color.White, 0.0f, new Vector2(8f, 14f), 4f, effects, layerDepth);
  }
}
