// Decompiled with JetBrains decompiler
// Type: StardewValley.Logging.CheatCommandChatLogger
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System;

#nullable disable
namespace StardewValley.Logging;

/// <summary>A logger which copies messages to the chat box, used when entering commands through the chat.</summary>
public class CheatCommandChatLogger : IGameLogger
{
  /// <summary>The chat box to which to copy messages.</summary>
  private readonly ChatBox ChatBox;

  /// <summary>Construct an instance.</summary>
  /// <param name="chatBox">The chat box to which to copy messages.</param>
  public CheatCommandChatLogger(ChatBox chatBox) => this.ChatBox = chatBox;

  /// <inheritdoc />
  public void Verbose(string message) => Game1.log.Verbose(message);

  /// <inheritdoc />
  public void Debug(string message)
  {
    this.ChatBox.addMessage(message, Color.Gray);
    Game1.log.Debug(message);
  }

  /// <inheritdoc />
  public void Info(string message)
  {
    this.ChatBox.addInfoMessage(message);
    Game1.log.Info(message);
  }

  /// <inheritdoc />
  public void Warn(string message)
  {
    this.ChatBox.addErrorMessage(message);
    Game1.log.Warn("[Warn] " + message);
  }

  /// <inheritdoc />
  public void Error(string error, Exception exception = null)
  {
    string message = "[Error] " + error;
    if (exception != null)
      message = $"{message}: {exception.Message}";
    this.ChatBox.addErrorMessage(message);
    Game1.log.Error(error, exception);
  }
}
