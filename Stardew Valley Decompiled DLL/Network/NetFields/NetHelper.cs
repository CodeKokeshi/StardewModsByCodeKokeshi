// Decompiled with JetBrains decompiler
// Type: Netcode.NetHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley;

#nullable disable
namespace Netcode;

/// <summary>Provides utility methods for implementing net fields.</summary>
internal static class NetHelper
{
  /// <summary>Log a validation warning to the console.</summary>
  /// <param name="message">The warning text to log.</param>
  public static void LogWarning(string message) => Game1.log.Warn(message);

  /// <summary>Log a validation trace message to the console.</summary>
  /// <param name="message">The warning text to log.</param>
  public static void LogVerbose(string message) => Game1.log.Verbose(message);
}
