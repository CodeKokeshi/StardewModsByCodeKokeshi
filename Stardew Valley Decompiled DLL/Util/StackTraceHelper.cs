// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.StackTraceHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Diagnostics;

#nullable disable
namespace StardewValley.Util;

public class StackTraceHelper
{
  /// <summary>The underlying stack trace object.</summary>
  private object _StackTrace;

  public static string FromException(Exception ex) => ex?.StackTrace ?? "";

  /// <summary>The current stack trace information.</summary>
  public static string StackTrace => Environment.StackTrace;

  /// <summary>The number of frames in the stack trace.</summary>
  public int FrameCount => !(this._StackTrace is System.Diagnostics.StackTrace stackTrace) ? 0 : stackTrace.FrameCount;

  /// <summary>Construct an instance.</summary>
  public StackTraceHelper() => this._StackTrace = (object) new System.Diagnostics.StackTrace();

  /// <summary>Gets the specified stack frame.</summary>
  public StackFrame GetFrame(int index)
  {
    return !(this._StackTrace is System.Diagnostics.StackTrace stackTrace) ? (StackFrame) null : stackTrace.GetFrame(index);
  }

  /// <summary>Returns a copy of all stack frames in the current stack trace.</summary>
  public StackFrame[] GetFrames()
  {
    return (this._StackTrace is System.Diagnostics.StackTrace stackTrace ? stackTrace.GetFrames() : (StackFrame[]) null) ?? LegacyShims.EmptyArray<StackFrame>();
  }

  /// <summary>Builds a readable representation of the stack trace.</summary>
  public new string ToString()
  {
    return (this._StackTrace is System.Diagnostics.StackTrace stackTrace ? stackTrace.ToString() : (string) null) ?? "";
  }
}
