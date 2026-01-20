// Decompiled with JetBrains decompiler
// Type: StardewValley.MapSeat
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class MapSeat : INetObject<NetFields>, ISittable
{
  [XmlIgnore]
  public static Texture2D mapChairTexture;
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> sittingFarmers = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public NetVector2 tilePosition = new NetVector2();
  [XmlIgnore]
  public NetVector2 size = new NetVector2();
  [XmlIgnore]
  public NetInt direction = new NetInt();
  [XmlIgnore]
  public NetVector2 drawTilePosition = new NetVector2(new Vector2(-1f, -1f));
  [XmlIgnore]
  public NetBool seasonal = new NetBool();
  [XmlIgnore]
  public NetString seatType = new NetString();
  [XmlIgnore]
  public NetString textureFile = new NetString((string) null);
  [XmlIgnore]
  public string _loadedTextureFile;
  [XmlIgnore]
  public Texture2D overlayTexture;
  [XmlIgnore]
  public int localSittingDirection = 2;
  [XmlIgnore]
  public Vector3? customDrawValues;

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (MapSeat));

  public MapSeat()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.sittingFarmers, nameof (sittingFarmers)).AddField((INetSerializable) this.tilePosition, nameof (tilePosition)).AddField((INetSerializable) this.size, nameof (size)).AddField((INetSerializable) this.direction, nameof (direction)).AddField((INetSerializable) this.drawTilePosition, nameof (drawTilePosition)).AddField((INetSerializable) this.seasonal, nameof (seasonal)).AddField((INetSerializable) this.seatType, nameof (seatType)).AddField((INetSerializable) this.textureFile, nameof (textureFile));
  }

  public static MapSeat FromData(string data, int x, int y)
  {
    MapSeat mapSeat = new MapSeat();
    try
    {
      string[] strArray = data.Split('/');
      mapSeat.tilePosition.Set(new Vector2((float) x, (float) y));
      mapSeat.size.Set(new Vector2((float) int.Parse(strArray[0]), (float) int.Parse(strArray[1])));
      mapSeat.seatType.Value = strArray[3];
      if (strArray[2] == "opposite")
      {
        mapSeat.direction.Value = -2;
      }
      else
      {
        int parsed;
        if (Utility.TryParseDirection(strArray[2], out parsed))
          mapSeat.direction.Value = parsed;
        else
          mapSeat.direction.Value = 2;
      }
      mapSeat.drawTilePosition.Set(new Vector2((float) int.Parse(strArray[4]), (float) int.Parse(strArray[5])));
      mapSeat.seasonal.Value = strArray[6] == "true";
      if (strArray.Length > 7)
        mapSeat.textureFile.Value = strArray[7];
      else
        mapSeat.textureFile.Value = (string) null;
    }
    catch (Exception ex)
    {
    }
    return mapSeat;
  }

  public bool IsBlocked(GameLocation location)
  {
    Rectangle seatBounds = this.GetSeatBounds();
    seatBounds.X *= 64 /*0x40*/;
    seatBounds.Y *= 64 /*0x40*/;
    seatBounds.Width *= 64 /*0x40*/;
    seatBounds.Height *= 64 /*0x40*/;
    Rectangle rectangle = seatBounds;
    switch (this.direction.Value)
    {
      case 0:
        rectangle.Y -= 32 /*0x20*/;
        rectangle.Height += 32 /*0x20*/;
        break;
      case 1:
        rectangle.Width += 32 /*0x20*/;
        break;
      case 2:
        rectangle.Height += 32 /*0x20*/;
        break;
      case 3:
        rectangle.X -= 32 /*0x20*/;
        rectangle.Width += 32 /*0x20*/;
        break;
    }
    foreach (NPC npc in Game1.CurrentEvent != null ? Game1.CurrentEvent.actors : location.characters.ToList<NPC>())
    {
      Rectangle boundingBox = npc.GetBoundingBox();
      if (boundingBox.Intersects(seatBounds) || !npc.isMovingOnPathFindPath.Value && boundingBox.Intersects(rectangle))
        return true;
    }
    return false;
  }

  public bool IsSittingHere(Farmer who) => this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID);

  public bool HasSittingFarmers() => this.sittingFarmers.Length > 0;

  public List<Vector2> GetSeatPositions(bool ignore_offsets = false)
  {
    this.customDrawValues = new Vector3?();
    List<Vector2> seatPositions = new List<Vector2>();
    switch (this.seatType.Value)
    {
      case "playground":
        Vector2 vector2_1 = new Vector2(this.tilePosition.X + 0.75f, this.tilePosition.Y);
        if (!ignore_offsets)
          vector2_1.Y -= 0.1f;
        seatPositions.Add(vector2_1);
        break;
      case "ccdesk":
        Vector2 vector2_2 = new Vector2(this.tilePosition.X + 0.5f, this.tilePosition.Y);
        if (!ignore_offsets)
          vector2_2.Y -= 0.4f;
        seatPositions.Add(vector2_2);
        break;
      default:
        if (this.seatType.Value.StartsWith("custom "))
        {
          float x = 0.0f;
          float y = 0.0f;
          float z = 0.0f;
          string[] strArray = ArgUtility.SplitBySpace(this.seatType.Value);
          try
          {
            if (strArray.Length > 1)
              x = float.Parse(strArray[1]);
            if (strArray.Length > 2)
              y = float.Parse(strArray[2]);
            if (strArray.Length > 3)
              z = float.Parse(strArray[3]);
          }
          catch (Exception ex)
          {
          }
          this.customDrawValues = new Vector3?(new Vector3(x, y, z));
          Vector2 vector2_3 = new Vector2(this.tilePosition.X + this.customDrawValues.Value.X, this.tilePosition.Y);
          if (!ignore_offsets)
            vector2_3.Y += this.customDrawValues.Value.Y;
          seatPositions.Add(vector2_3);
          break;
        }
        for (int index1 = 0; (double) index1 < (double) this.size.X; ++index1)
        {
          for (int index2 = 0; (double) index2 < (double) this.size.Y; ++index2)
          {
            Vector2 vector2_4 = new Vector2(0.0f, 0.0f);
            if (this.seatType.Value.StartsWith("bench"))
            {
              if (this.direction.Value == 2)
                vector2_4.Y += 0.25f;
              else if ((this.direction.Value == 3 || this.direction.Value == 1) && index2 == 0)
                vector2_4.Y += 0.5f;
            }
            if (this.seatType.Value.StartsWith("picnic"))
            {
              switch (this.direction.Value)
              {
                case 0:
                  vector2_4.Y += 0.25f;
                  break;
                case 2:
                  vector2_4.Y -= 0.25f;
                  break;
              }
            }
            if (this.seatType.Value.EndsWith("swings"))
              vector2_4.Y -= 0.5f;
            else if (this.seatType.Value.EndsWith("summitbench"))
              vector2_4.Y -= 0.2f;
            else if (this.seatType.Value.EndsWith("tall"))
              vector2_4.Y -= 0.3f;
            else if (this.seatType.Value.EndsWith("short"))
              vector2_4.Y += 0.3f;
            if (ignore_offsets)
              vector2_4 = Vector2.Zero;
            seatPositions.Add(this.tilePosition.Value + new Vector2((float) index1 + vector2_4.X, (float) index2 + vector2_4.Y));
          }
        }
        break;
    }
    return seatPositions;
  }

  public virtual void Draw(SpriteBatch b)
  {
    if (this._loadedTextureFile != this.textureFile.Value)
    {
      this._loadedTextureFile = this.textureFile.Value;
      try
      {
        this.overlayTexture = Game1.content.Load<Texture2D>(this._loadedTextureFile);
      }
      catch (Exception ex)
      {
        this.overlayTexture = (Texture2D) null;
      }
    }
    if (this.overlayTexture == null)
      this.overlayTexture = MapSeat.mapChairTexture;
    if ((double) this.drawTilePosition.Value.X < 0.0 || !this.HasSittingFarmers())
      return;
    float num = 0.0f;
    if (this.customDrawValues.HasValue)
      num = this.customDrawValues.Value.Z;
    else if (this.seatType.Value.StartsWith("highback_chair") || this.seatType.Value.StartsWith("ccdesk"))
      num = 1f;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(this.tilePosition.X * 64f, (float) (((double) this.tilePosition.Y - (double) num) * 64.0)));
    float layerDepth = (float) (((double) (int) this.tilePosition.Y + (double) this.size.Y + 0.1) * 64.0) / 10000f;
    Rectangle rectangle = new Rectangle((int) this.drawTilePosition.Value.X * 16 /*0x10*/, (int) ((double) this.drawTilePosition.Value.Y - (double) num) * 16 /*0x10*/, (int) this.size.Value.X * 16 /*0x10*/, (int) ((double) this.size.Value.Y + (double) num) * 16 /*0x10*/);
    if (this.seasonal.Value)
      rectangle.X += rectangle.Width * Game1.currentLocation.GetSeasonIndex();
    b.Draw(this.overlayTexture, local, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
  }

  public bool OccupiesTile(int x, int y) => this.GetSeatBounds().Contains(x, y);

  public virtual Vector2? AddSittingFarmer(Farmer who)
  {
    if (who == Game1.player)
    {
      this.localSittingDirection = this.direction.Value;
      if (this.seatType.Value.StartsWith("stool"))
        this.localSittingDirection = Game1.player.FacingDirection;
      if (this.direction.Value == -2)
        this.localSittingDirection = Utility.GetOppositeFacingDirection(Game1.player.FacingDirection);
      if (this.seatType.Value.StartsWith("bathchair") && this.localSittingDirection == 0)
        this.localSittingDirection = 2;
    }
    List<Vector2> seatPositions = this.GetSeatPositions(false);
    if (seatPositions.Count == 0)
      return new Vector2?();
    bool[] seatsFilled;
    this.CheckSeatOccupancyIfTemporaryMap(who, seatPositions, out seatsFilled);
    if (((IEnumerable<bool>) seatsFilled).All<bool>((Func<bool, bool>) (occupied => occupied)))
      return new Vector2?();
    int num1 = -1;
    Vector2? nullable = new Vector2?();
    float num2 = 96f;
    for (int index = 0; index < seatPositions.Count; ++index)
    {
      if (!this.sittingFarmers.Values.Contains<int>(index) && !seatsFilled[index])
      {
        float num3 = ((seatPositions[index] + new Vector2(0.5f, 0.5f)) * 64f - who.getStandingPosition()).Length();
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          nullable = new Vector2?(seatPositions[index]);
          num1 = index;
        }
      }
    }
    if (nullable.HasValue)
      this.sittingFarmers[who.UniqueMultiplayerID] = num1;
    return nullable;
  }

  public bool IsSeatHere(GameLocation location) => location.mapSeats.Contains(this);

  public int GetSittingDirection() => this.localSittingDirection;

  public Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false)
  {
    int index;
    return this.sittingFarmers.TryGetValue(who.UniqueMultiplayerID, out index) ? new Vector2?(this.GetSeatPositions(ignore_offsets)[index]) : new Vector2?();
  }

  public virtual Rectangle GetSeatBounds()
  {
    if (this.seatType.Value == "chair" && this.direction.Value == 0)
    {
      Rectangle rectangle = new Rectangle((int) this.tilePosition.X, (int) this.tilePosition.Y + 1, (int) this.size.X, (int) this.size.Y - 1);
    }
    return new Rectangle((int) this.tilePosition.X, (int) this.tilePosition.Y, (int) this.size.X, (int) this.size.Y);
  }

  public virtual void RemoveSittingFarmer(Farmer farmer)
  {
    this.sittingFarmers.Remove(farmer.UniqueMultiplayerID);
  }

  public virtual int GetSittingFarmerCount() => this.sittingFarmers.Length;

  /// <summary>Manually check seat occupancy if we're in a non-synced temporary location (e.g. for an event or festival).</summary>
  /// <param name="who">The player for which to load seats.</param>
  /// <param name="seatPositions">The tile positions containing seats.</param>
  /// <param name="seatsFilled">The flags which indicate whether each available seat is occupied.</param>
  private void CheckSeatOccupancyIfTemporaryMap(
    Farmer who,
    List<Vector2> seatPositions,
    out bool[] seatsFilled)
  {
    seatsFilled = new bool[seatPositions.Count];
    GameLocation currentLocation = who.currentLocation;
    if ((currentLocation != null ? (!currentLocation.IsTemporary ? 1 : 0) : 1) != 0)
      return;
    FarmerCollection farmerCollection = currentLocation.farmers ?? Game1.getOnlineFarmers();
    if (farmerCollection.Count <= 1)
      return;
    List<Vector2> seatPositions1 = this.GetSeatPositions(true);
    Vector2 result1 = seatPositions1[0];
    Vector2 result2 = seatPositions1[0];
    for (int index = 1; index < seatPositions1.Count; ++index)
    {
      Vector2 vector2 = seatPositions1[index];
      Vector2.Min(ref result1, ref vector2, out result1);
      Vector2.Max(ref result2, ref vector2, out result2);
    }
    result1 -= new Vector2(1E-05f, 1E-05f);
    result2 += new Vector2(1E-05f, 1E-05f);
    int count = seatPositions1.Count;
    foreach (Farmer farmer in farmerCollection)
    {
      if (farmer.isSitting.Value && !((NetFieldBase<long, NetLong>) farmer.uniqueMultiplayerID == who.uniqueMultiplayerID))
      {
        Vector2 vector2_1 = farmer.mapChairSitPosition.Value;
        if ((double) vector2_1.X > (double) result1.X && (double) vector2_1.X < (double) result2.X && (double) vector2_1.Y > (double) result1.Y && (double) vector2_1.Y < (double) result2.Y)
        {
          for (int index = 0; index < seatPositions1.Count; ++index)
          {
            if (!seatsFilled[index])
            {
              Vector2 vector2_2 = seatPositions1[index] - vector2_1;
              if ((double) Math.Abs(vector2_2.X) < 9.9999997473787516E-06 && (double) Math.Abs(vector2_2.Y) < 9.9999997473787516E-06)
              {
                seatsFilled[index] = true;
                --count;
                break;
              }
            }
          }
          if (count == 0)
            break;
        }
      }
    }
  }
}
