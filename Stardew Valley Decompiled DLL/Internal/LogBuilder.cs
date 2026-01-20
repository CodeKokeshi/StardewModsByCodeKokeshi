// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.LogBuilder
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Text;

#nullable disable
namespace StardewValley.Internal;

/// <summary>Builds an incremental log of discrete steps.</summary>
public class LogBuilder
{
  /// <summary>The underlying log.</summary>
  public readonly StringBuilder Log;
  /// <summary>The indent to apply to logged messages.</summary>
  public readonly int Indent;

  /// <summary>Construct an instance.</summary>
  /// <param name="indent">The indent to apply to logged messages.</param>
  public LogBuilder(int indent = 0)
    : this(new StringBuilder(), indent)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="log">The underlying log.</param>
  /// <param name="indent">The indent to apply to logged messages.</param>
  public LogBuilder(StringBuilder log, int indent = 0)
  {
    this.Log = log ?? throw new ArgumentNullException(nameof (log));
    this.Indent = indent;
  }

  /// <summary>Append a blank line to the log.</summary>
  public void AppendLine() => this.Log.AppendLine();

  /// <summary>Append a message to the log.</summary>
  /// <param name="message">The message to log.</param>
  public void AppendLine(string message)
  {
    if (this.Indent > 0 && message.Length > 0)
      message = message.PadLeft(message.Length + this.Indent, ' ');
    this.Log.AppendLine(message);
  }

  /// <summary>Get a new log builder which writes to the same log with a specified indent.</summary>
  /// <param name="indent">The indent to apply to logged messages.</param>
  public LogBuilder GetIndentedLog(int indent = 3)
  {
    return new LogBuilder(this.Log, this.Indent + indent);
  }
}
