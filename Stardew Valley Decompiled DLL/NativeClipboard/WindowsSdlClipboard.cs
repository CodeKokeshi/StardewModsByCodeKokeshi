// Decompiled with JetBrains decompiler
// Type: StardewValley.NativeClipboard.WindowsSdlClipboard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace StardewValley.NativeClipboard;

/// <summary>Provides a wrapper around SDL's clipboard API for Windows.</summary>
internal sealed class WindowsSdlClipboard : SdlClipboard
{
  /// <inheritdoc cref="M:StardewValley.NativeClipboard.WindowsSdlClipboard.GetTextImpl" />
  [DllImport("SDL2.dll", CallingConvention = (CallingConvention) 2)]
  private static extern IntPtr SDL_GetClipboardText();

  /// <inheritdoc cref="M:StardewValley.NativeClipboard.WindowsSdlClipboard.SetTextImpl(System.IntPtr)" />
  [DllImport("SDL2.dll", CallingConvention = (CallingConvention) 2)]
  private static extern int SDL_SetClipboardText(IntPtr text);

  /// <summary>Constructs an instance and sets the providing platform name.</summary>
  public WindowsSdlClipboard() => this.PlatformName = "Windows";

  /// <inheritdoc />
  protected override IntPtr GetTextImpl() => WindowsSdlClipboard.SDL_GetClipboardText();

  /// <inheritdoc />
  protected override int SetTextImpl(IntPtr text) => WindowsSdlClipboard.SDL_SetClipboardText(text);
}
