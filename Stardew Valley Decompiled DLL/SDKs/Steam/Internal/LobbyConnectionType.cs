// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.Internal.LobbyConnectionType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.SDKs.Steam.Internal;

/// <summary>A connection type supported by a lobby.</summary>
internal enum LobbyConnectionType
{
  /// <summary>A lobby which only allows Steam connections.</summary>
  Steam,
  /// <summary>A lobby which only allows GOG Galaxy connections.</summary>
  Galaxy,
  /// <summary>A lobby which allows both GOG Galaxy and Steam connections.</summary>
  Hybrid,
  /// <summary>An invalid or cleared lobby.</summary>
  Invalid,
}
