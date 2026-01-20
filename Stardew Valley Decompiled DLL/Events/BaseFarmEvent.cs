// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.BaseFarmEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

#nullable disable
namespace StardewValley.Events;

/// <inheritdoc />
public abstract class BaseFarmEvent : FarmEvent, INetObject<NetFields>
{
  /// <summary>The multiplayer-synchronized fields for this event.</summary>
  public NetFields NetFields { get; private set; }

  /// <summary>Construct an instance.</summary>
  protected BaseFarmEvent() => this.initNetFields();

  /// <summary>Initialize the multiplayer-synchronized fields for this instance.</summary>
  public virtual void initNetFields()
  {
    this.NetFields = new NetFields(this.GetType().Name).SetOwner((INetObject<NetFields>) this);
  }

  /// <inheritdoc />
  public virtual bool setUp() => false;

  /// <inheritdoc />
  public virtual bool tickUpdate(GameTime time) => true;

  /// <inheritdoc />
  public virtual void draw(SpriteBatch b)
  {
  }

  /// <inheritdoc />
  public virtual void drawAboveEverything(SpriteBatch b)
  {
  }

  /// <inheritdoc />
  public virtual void makeChangesToLocation()
  {
  }

  /// <summary>Auto-generate a default light source ID for this event.</summary>
  protected virtual string GenerateLightSourceId() => this.GetType().Name;
}
