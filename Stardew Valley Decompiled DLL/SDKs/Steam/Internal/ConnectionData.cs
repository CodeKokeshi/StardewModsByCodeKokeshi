// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.Internal.ConnectionData
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Steamworks;

#nullable disable
namespace StardewValley.SDKs.Steam.Internal;

/// <summary>Extra bookkeeping data for a connected client.</summary>
internal sealed class ConnectionData
{
  /// <summary>The Farmer ID associated with the connected client.</summary>
  public long FarmerId = long.MinValue;
  /// <summary>The Steam ID of the connected client.</summary>
  public CSteamID SteamId;
  /// <summary>The connection used to send data to the client.</summary>
  public HSteamNetConnection Connection;
  /// <summary>Whether the client has an active farmhand.</summary>
  public bool Online;
  /// <summary>The Steam display name of the connected client.</summary>
  public string DisplayName;

  /// <summary>Construct an instance.</summary>
  /// <param name="connection">The connection used to send data to the client.</param>
  /// <param name="steamId">The Steam ID of the connected client.</param>
  /// <param name="displayName">The Steam display name of the connected client.</param>
  public ConnectionData(HSteamNetConnection connection, CSteamID steamId, string displayName)
  {
    this.Connection = connection;
    this.SteamId = steamId;
    this.DisplayName = displayName;
  }
}
