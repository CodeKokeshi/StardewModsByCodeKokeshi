// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.ChestHit.Internal.ChestHitMessageType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Network.ChestHit.Internal;

/// <summary>The network message types used to synchronize chest hits.</summary>
internal enum ChestHitMessageType : byte
{
  /// <summary>Sent by clients when they hit a chest.</summary>
  Sync,
  /// <summary>Sent by the server to signal a chest has been moved.</summary>
  Move,
  /// <summary>Sent by the server to signal a chest has been deleted.</summary>
  Delete,
}
