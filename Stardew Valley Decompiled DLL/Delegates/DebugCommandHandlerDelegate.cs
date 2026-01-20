// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.DebugCommandHandlerDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Logging;

#nullable disable
namespace StardewValley.Delegates;

/// <summary>Handles a debug command.</summary>
/// <param name="command">The full debug command split by spaces, including the command name.</param>
/// <param name="log">The log to which to write debug command output.</param>
public delegate void DebugCommandHandlerDelegate(string[] command, IGameLogger log);
