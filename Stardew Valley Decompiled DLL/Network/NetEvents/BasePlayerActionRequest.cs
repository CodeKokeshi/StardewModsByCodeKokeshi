// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetEvents.BasePlayerActionRequest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.IO;

#nullable disable
namespace StardewValley.Network.NetEvents;

/// <summary>A net-synced request to perform an action for the target players.</summary>
public abstract class BasePlayerActionRequest : NetEventArg
{
  /// <summary>The players for which to perform an action.</summary>
  public PlayerActionTarget Target { get; private set; }

  /// <summary>The specific player ID to apply this event to, or <c>null</c> to apply it to all players matching <see cref="P:StardewValley.Network.NetEvents.BasePlayerActionRequest.Target" />.</summary>
  public long? OnlyPlayerId { get; private set; }

  /// <summary>Reads the request data from a net-sync stream.</summary>
  /// <param name="reader">The binary stream to read.</param>
  public virtual void Read(BinaryReader reader)
  {
    this.Target = (PlayerActionTarget) reader.ReadByte();
    this.OnlyPlayerId = reader.ReadBoolean() ? new long?(reader.ReadInt64()) : new long?();
  }

  /// <summary>Writes the request data to a net-sync stream.</summary>
  /// <param name="writer">The binary stream to write to.</param>
  public virtual void Write(BinaryWriter writer)
  {
    writer.Write((byte) this.Target);
    writer.Write(this.OnlyPlayerId.HasValue);
    if (!this.OnlyPlayerId.HasValue)
      return;
    writer.Write(this.OnlyPlayerId.Value);
  }

  /// <summary>Get whether this event should be applied to a given player.</summary>
  /// <param name="player">The player to check.</param>
  public bool MatchesPlayer(Farmer player)
  {
    if (this.OnlyPlayerId.HasValue && player.UniqueMultiplayerID != this.OnlyPlayerId.Value)
      return false;
    switch (this.Target)
    {
      case PlayerActionTarget.Current:
        return true;
      case PlayerActionTarget.Host:
        return Game1.IsMasterGame;
      case PlayerActionTarget.All:
        return true;
      default:
        Game1.log.Warn($"Can't process net request {this.GetType().AssemblyQualifiedName}: Invalid target '{this.Target}'");
        return false;
    }
  }

  /// <summary>Get whether this request should only be applied to the local player.</summary>
  public bool OnlyForLocalPlayer()
  {
    if (this.OnlyPlayerId.HasValue)
      return this.MatchesPlayer(Game1.player);
    switch (this.Target)
    {
      case PlayerActionTarget.Current:
        return true;
      case PlayerActionTarget.Host:
        return Game1.IsMasterGame;
      case PlayerActionTarget.All:
        return Game1.IsMasterGame && Game1.netWorldState.Value.farmhandData.Length == 0;
      default:
        Game1.log.Warn($"Can't process net request {this.GetType().AssemblyQualifiedName}: Invalid target '{this.Target}'");
        return false;
    }
  }

  /// <summary>Applies the request to the current player.</summary>
  /// <param name="farmer">The players to change.</param>
  public abstract void PerformAction(Farmer farmer);

  /// <summary>Constructs an instance.</summary>
  protected BasePlayerActionRequest()
    : this(PlayerActionTarget.Current, new long?())
  {
  }

  /// <summary>Constructs an instance.</summary>
  /// <param name="target">The players for which to perform the action.</param>
  /// <param name="onlyPlayerId">The specific player ID to apply this event to, or <c>null</c> to apply it to all players matching <paramref name="target" />.</param>
  protected BasePlayerActionRequest(PlayerActionTarget target, long? onlyPlayerId)
  {
    this.Target = target;
    this.OnlyPlayerId = onlyPlayerId;
  }
}
