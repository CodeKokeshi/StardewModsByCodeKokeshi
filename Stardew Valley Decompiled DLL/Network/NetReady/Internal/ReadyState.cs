// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetReady.Internal.ReadyState
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Network.NetReady.Internal;

/// <summary>The possible states for a ready check.</summary>
internal enum ReadyState : byte
{
  /// <summary>Not marked as ready to proceed with the check.</summary>
  NotReady,
  /// <summary>Ready to proceed, but can still cancel.</summary>
  Ready,
  /// <summary>Ready to proceed, and can no longer cancel.</summary>
  Locked,
}
