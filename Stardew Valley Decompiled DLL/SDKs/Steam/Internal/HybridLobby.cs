// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.Internal.HybridLobby
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using Steamworks;

#nullable disable
namespace StardewValley.SDKs.Steam.Internal;

/// <summary>A lobby that can accept Steam connections, Galaxy connections, or both.</summary>
internal struct HybridLobby
{
  /// <summary>Whether this is a Galaxy lobby which allows Steam connections. This is only relevant to lobbies from invite codes.</summary>
  private bool IsHybrid;

  /// <summary>The underlying Steam lobby ID.</summary>
  public ulong SteamId { get; private set; }

  /// <summary>The underlying Galaxy lobby ID.</summary>
  public ulong GalaxyId { get; private set; }

  /// <summary>The type of lobby represented by this instance.</summary>
  public LobbyConnectionType LobbyType
  {
    get
    {
      CSteamID csteamId = new CSteamID(this.SteamId);
      if (csteamId.IsValid() && csteamId.IsLobby())
        return LobbyConnectionType.Steam;
      if (!new GalaxyID(this.GalaxyId).IsValid())
        return LobbyConnectionType.Invalid;
      return this.IsHybrid ? LobbyConnectionType.Hybrid : LobbyConnectionType.Galaxy;
    }
  }

  /// <summary>Constructs an instance which allows only Steam connections.</summary>
  /// <param name="steamID">The ID of the Steam lobby.</param>
  public HybridLobby(CSteamID steamID)
  {
    this.SteamId = steamID.m_SteamID;
    this.GalaxyId = 0UL;
    this.IsHybrid = false;
  }

  /// <summary>Constructs an instance which allows GOG Galaxy (and possibly Steam) connections.</summary>
  /// <param name="galaxyID">The ID of the Galaxy lobby.</param>
  /// <param name="isHybrid">Whether the Galaxy lobby supports Steam connections.</param>
  public HybridLobby(GalaxyID galaxyID, bool isHybrid = false)
  {
    this.SteamId = 0UL;
    this.GalaxyId = galaxyID.ToUint64();
    this.IsHybrid = isHybrid;
  }

  /// <summary>Invalidates the lobby and its lobby ID members.</summary>
  public void Clear()
  {
    this.SteamId = 0UL;
    this.GalaxyId = 0UL;
    this.IsHybrid = false;
  }
}
