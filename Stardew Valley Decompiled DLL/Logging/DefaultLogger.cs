// Decompiled with JetBrains decompiler
// Type: StardewValley.Logging.DefaultLogger
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;
using System.Text;

#nullable disable
namespace StardewValley.Logging;

/// <summary>A logger which writes to the console window in debug mode.</summary>
internal class DefaultLogger : IGameLogger
{
  /// <summary>The message builder used to format messages.</summary>
  private readonly StringBuilder MessageBuilder = new StringBuilder();
  /// <summary>The cached absolute path to the debug log file.</summary>
  private string _LogPath;
  /// <summary>Whether we have started the log file.</summary>
  private bool StartedLogFile;

  /// <summary>The absolute path to the debug log file.</summary>
  private string LogPath
  {
    get
    {
      if (this._LogPath == null)
        this._LogPath = Program.GetDebugLogPath();
      return this._LogPath;
    }
  }

  /// <summary>Whether to log messages to the console window.</summary>
  public bool ShouldWriteToConsole { get; }

  /// <summary>Whether to log messages to the debug log file.</summary>
  public bool ShouldWriteToLogFile { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="shouldWriteToConsole">Whether to log messages to the console window.</param>
  /// <param name="shouldWriteToLogFile">Whether to log messages to the debug log file.</param>
  public DefaultLogger(bool shouldWriteToConsole, bool shouldWriteToLogFile)
  {
    this.ShouldWriteToConsole = shouldWriteToConsole;
    this.ShouldWriteToLogFile = shouldWriteToLogFile;
    if (!shouldWriteToLogFile)
      return;
    this.WriteMessageToFile("");
  }

  /// <inheritdoc />
  public void Verbose(string message) => this.LogImpl(nameof (Verbose), message);

  /// <inheritdoc />
  public void Debug(string message) => this.LogImpl(nameof (Debug), message);

  /// <inheritdoc />
  public void Info(string message) => this.LogImpl(nameof (Info), message);

  /// <inheritdoc />
  public void Warn(string message) => this.LogImpl(nameof (Warn), message);

  /// <inheritdoc />
  public void Error(string error, Exception exception)
  {
    this.LogImpl(nameof (Error), error, exception);
  }

  private void WriteMessageToFile(string message)
  {
    if (this.LogPath == null)
      return;
    if (!this.StartedLogFile)
    {
      File.WriteAllText(this.LogPath, message);
      this.StartedLogFile = true;
      Game1.log.Verbose($"Starting log file at {DateTime.Now:yyyy-MM-dd HH:mm:ii}.");
    }
    else
    {
      try
      {
        File.AppendAllText(this.LogPath, message);
      }
      catch (Exception ex)
      {
        if (!this.ShouldWriteToConsole)
          return;
        Console.WriteLine($"Failed writing to log file:\n{ex}");
      }
    }
  }

  /// <summary>Log a message to the console and/or log file.</summary>
  /// <param name="level">The log level.</param>
  /// <param name="message">The message to log.</param>
  /// <param name="exception">The exception to logged, if applicable.</param>
  private void LogImpl(string level, string message, Exception exception = null)
  {
    bool shouldWriteToConsole = this.ShouldWriteToConsole;
    bool shouldWriteToLogFile = this.ShouldWriteToLogFile;
    if (!(shouldWriteToConsole | shouldWriteToLogFile))
      return;
    message = this.FormatLog(level, message, exception);
    if (shouldWriteToConsole)
      Console.WriteLine(message);
    if (!shouldWriteToLogFile)
      return;
    this.WriteMessageToFile(message);
  }

  /// <summary>Format a log message with the date and level for display.</summary>
  /// <param name="level">The log level.</param>
  /// <param name="text">The message to log.</param>
  /// <param name="exception">The exception to logged, if applicable.</param>
  private string FormatLog(string level, string text, Exception exception = null)
  {
    StringBuilder messageBuilder = this.MessageBuilder;
    try
    {
      Game1 game1 = Game1.game1;
      int instanceId = game1 != null ? game1.instanceId : 0;
      StringBuilder stringBuilder1 = messageBuilder.Append('[');
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder1);
      interpolatedStringHandler.AppendFormatted<DateTime>(DateTime.Now, "HH:mm:ss");
      interpolatedStringHandler.AppendLiteral(" ");
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      StringBuilder stringBuilder3 = stringBuilder2.Append(ref local).Append(level).Append(' ');
      string str;
      if (instanceId != 0)
        str = $"screen{instanceId}";
      else
        str = "game";
      stringBuilder3.Append(str).Append("] ").Append(text).AppendLine();
      if (exception != null)
        messageBuilder.Append((object) exception).AppendLine();
      return messageBuilder.ToString();
    }
    finally
    {
      messageBuilder.Clear();
    }
  }
}
