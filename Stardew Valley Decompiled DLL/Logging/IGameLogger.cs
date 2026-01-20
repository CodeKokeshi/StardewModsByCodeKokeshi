// Decompiled with JetBrains decompiler
// Type: StardewValley.Logging.IGameLogger
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Logging;

/// <summary>Handles writing messages to the game log for Stardew Valley itself.</summary>
public interface IGameLogger
{
  /// <summary>Log tracing info intended for Stardew Valley developers troubleshooting specific issues.</summary>
  /// <param name="message">The message to log.</param>
  void Verbose(string message);

  /// <summary>Log troubleshooting info intended for developers or players.</summary>
  /// <param name="message">The message to log.</param>
  void Debug(string message);

  /// <summary>Log a message intended for players interacting with the console.</summary>
  /// <param name="message">The message to log.</param>
  void Info(string message);

  /// <summary>Log a potential problem that users should be aware of.</summary>
  void Warn(string message);

  /// <summary>Log an message indicating something has gone wrong.</summary>
  /// <param name="error">The message to log.</param>
  /// <param name="exception">The underlying exception.</param>
  void Error(string error, Exception exception = null);
}
