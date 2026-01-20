// Decompiled with JetBrains decompiler
// Type: StardewValley.NativeClipboard.LinuxSdlClipboard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.NativeClipboard;

/// <summary>Provides a wrapper around SDL's clipboard API for Linux.</summary>
internal sealed class LinuxSdlClipboard : SdlClipboard
{
  /// <summary>Constructs an instance and sets the providing platform name.</summary>
  public LinuxSdlClipboard() => this.PlatformName = "Linux";
}
