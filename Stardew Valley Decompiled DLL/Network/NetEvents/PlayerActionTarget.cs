// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetEvents.PlayerActionTarget
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Network.NetEvents;

/// <summary>The player(s) to change for a net event request.</summary>
public enum PlayerActionTarget : byte
{
  /// <summary>Apply the action to the current player.</summary>
  Current,
  /// <summary>Apply the action to the main player.</summary>
  Host,
  /// <summary>Apply the action to all players (regardless of whether they're online).</summary>
  All,
}
