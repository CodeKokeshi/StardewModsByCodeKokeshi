// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.ZlibException
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ionic.Zlib;

/// <summary>
/// A general purpose exception class for exceptions in the Zlib library.
/// </summary>
[Guid("ebc25cf6-9120-4283-b972-0e5520d0000E")]
public class ZlibException : Exception
{
  /// <summary>
  /// The ZlibException class captures exception information generated
  /// by the Zlib library.
  /// </summary>
  public ZlibException()
  {
  }

  /// <summary>
  /// This ctor collects a message attached to the exception.
  /// </summary>
  /// <param name="s">the message for the exception.</param>
  public ZlibException(string s)
    : base(s)
  {
  }
}
