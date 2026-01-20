// Decompiled with JetBrains decompiler
// Type: StardewValley.Chunk
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Chunk : INetObject<NetFields>
{
  /// <summary>The minimum number of milliseconds before an item in water sinks.</summary>
  public const int MinSinkTimer = 1900;
  /// <summary>The maximum number of milliseconds before an item in water sinks.</summary>
  public const int MaxSinkTimer = 2400;
  [XmlElement("position")]
  public NetPosition position = new NetPosition();
  [XmlIgnore]
  public readonly NetFloat xVelocity = new NetFloat().Interpolated(true, true);
  [XmlIgnore]
  public readonly NetFloat yVelocity = new NetFloat().Interpolated(true, true);
  [XmlIgnore]
  public readonly NetBool hasPassedRestingLineOnce = new NetBool(false);
  [XmlIgnore]
  public int bounces;
  /// <summary>If the item is floating in water, a visual Y pixel offset to apply for the bobbing animation.</summary>
  [XmlIgnore]
  public float bob;
  /// <summary>The number of milliseconds until this debris sinks, if it's in water and <see cref="F:StardewValley.Debris.isSinking" /> is true.</summary>
  public readonly NetInt sinkTimer = new NetInt();
  public readonly NetInt netDebrisType = new NetInt();
  [XmlIgnore]
  public bool hitWall;
  [XmlElement("xSpriteSheet")]
  public readonly NetInt xSpriteSheet = new NetInt();
  [XmlElement("ySpriteSheet")]
  public readonly NetInt ySpriteSheet = new NetInt();
  [XmlIgnore]
  public float rotation;
  [XmlIgnore]
  public float rotationVelocity;
  private readonly NetFloat netScale = new NetFloat().Interpolated(true, true);
  private readonly NetFloat netAlpha = new NetFloat();

  public int randomOffset
  {
    get => this.netDebrisType.Value;
    set => this.netDebrisType.Value = value;
  }

  public float scale
  {
    get => this.netScale.Value;
    set => this.netScale.Value = value;
  }

  public float alpha
  {
    get => this.netAlpha.Value;
    set => this.netAlpha.Value = value;
  }

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (Chunk));

  public Chunk()
  {
    this.sinkTimer.Value = Game1.random.Next(1900, 2401);
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.position.NetFields, "position.NetFields").AddField((INetSerializable) this.xVelocity, nameof (xVelocity)).AddField((INetSerializable) this.yVelocity, nameof (yVelocity)).AddField((INetSerializable) this.sinkTimer, nameof (sinkTimer)).AddField((INetSerializable) this.netDebrisType, nameof (netDebrisType)).AddField((INetSerializable) this.xSpriteSheet, nameof (xSpriteSheet)).AddField((INetSerializable) this.ySpriteSheet, nameof (ySpriteSheet)).AddField((INetSerializable) this.netScale, nameof (netScale)).AddField((INetSerializable) this.netAlpha, nameof (netAlpha)).AddField((INetSerializable) this.hasPassedRestingLineOnce, nameof (hasPassedRestingLineOnce));
    if (LocalMultiplayer.IsLocalMultiplayer(true))
      this.NetFields.DeltaAggregateTicks = (ushort) 10;
    else
      this.NetFields.DeltaAggregateTicks = (ushort) 30;
  }

  public Chunk(Vector2 position, float xVelocity, float yVelocity, int random_offset)
    : this()
  {
    this.position.Value = position;
    this.xVelocity.Value = xVelocity;
    this.yVelocity.Value = yVelocity;
    this.randomOffset = random_offset;
    this.alpha = 1f;
  }

  public float getSpeed()
  {
    return (float) Math.Sqrt((double) this.xVelocity.Value * (double) this.xVelocity.Value + (double) this.yVelocity.Value * (double) this.yVelocity.Value);
  }

  /// <summary>Get the visual pixel position, accounting for bob if it's sinking in water.</summary>
  public Vector2 GetVisualPosition()
  {
    return (double) this.bob == 0.0 ? this.position.Value : new Vector2(this.position.X, this.position.Y + this.bob);
  }
}
