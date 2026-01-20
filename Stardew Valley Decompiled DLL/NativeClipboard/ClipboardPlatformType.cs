// Decompiled with JetBrains decompiler
// Type: StardewValley.NativeClipboard.ClipboardPlatformType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.NativeClipboard;

/// <summary>The platform that provides the clipboard.</summary>
internal enum ClipboardPlatformType
{
  /// <summary>The platform is Linux.</summary>
  Linux,
  /// <summary>The platform is macOS/OSX.</summary>
  OSX,
  /// <summary>The platform is Windows.</summary>
  Windows,
  /// <summary>The platform is unknown.</summary>
  Unknown,
}
