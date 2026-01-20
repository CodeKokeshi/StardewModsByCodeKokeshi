// Decompiled with JetBrains decompiler
// Type: StardewValley.NativeClipboard.SdlClipboard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace StardewValley.NativeClipboard;

/// <summary>A wrapper around SDL's native clipboard API.</summary>
internal abstract class SdlClipboard
{
  /// <summary>The underlying platform that provides the SDL clipboard API.</summary>
  private static SdlClipboard PlatformClipboard;
  /// <summary>The name of the platform providing the native SDL API.</summary>
  protected string PlatformName;
  /// <summary>The platform that the clipboard is running on.</summary>
  internal static readonly ClipboardPlatformType Platform = SdlClipboard.GetPlatformType();

  static SdlClipboard()
  {
    switch (SdlClipboard.Platform)
    {
      case ClipboardPlatformType.Linux:
        SdlClipboard.PlatformClipboard = (SdlClipboard) new LinuxSdlClipboard();
        break;
      case ClipboardPlatformType.OSX:
        SdlClipboard.PlatformClipboard = (SdlClipboard) new OsxSdlClipboard();
        break;
      case ClipboardPlatformType.Windows:
        SdlClipboard.PlatformClipboard = (SdlClipboard) new WindowsSdlClipboard();
        break;
      default:
        SdlClipboard.PlatformClipboard = (SdlClipboard) null;
        break;
    }
  }

  /// <summary>Retrieves the clipboard text from the underlying platform's native SDL API.</summary>
  /// <returns>A string containing the clipboard text, null if the clipboard was empty or if an error occurred.</returns>
  public static string GetText()
  {
    if (SdlClipboard.PlatformClipboard == null)
      return (string) null;
    IntPtr textImpl;
    try
    {
      textImpl = SdlClipboard.PlatformClipboard.GetTextImpl();
    }
    catch (Exception ex)
    {
      return (string) null;
    }
    if (textImpl == IntPtr.Zero)
      return (string) null;
    int length = 0;
    while (Marshal.ReadByte(textImpl, length) != (byte) 0)
      ++length;
    if (length == 0)
      return (string) null;
    byte[] numArray = new byte[length];
    Marshal.Copy(textImpl, numArray, 0, length);
    try
    {
      return Encoding.UTF8.GetString(numArray, 0, length);
    }
    catch (Exception ex)
    {
      return (string) null;
    }
  }

  /// <summary>Sets the clipboard text using the underlying platform's native SDL API.</summary>
  /// <param name="text">The string to replace the current clipboard text.</param>
  public static bool SetText(string text)
  {
    if (SdlClipboard.PlatformClipboard == null || text == null)
      return false;
    byte[] bytes = Encoding.UTF8.GetBytes(text);
    IntPtr num1 = Marshal.AllocHGlobal(bytes.Length + 1);
    try
    {
      Marshal.Copy(bytes, 0, num1, bytes.Length);
      Marshal.WriteByte(num1, bytes.Length, (byte) 0);
      int num2;
      try
      {
        num2 = SdlClipboard.PlatformClipboard.SetTextImpl(num1);
      }
      catch (Exception ex)
      {
        return false;
      }
      return num2 == 0;
    }
    finally
    {
      Marshal.FreeHGlobal(num1);
    }
  }

  /// <summary>Determines the platform-specific SDL clipboard API provider based on runtime information.</summary>
  private static ClipboardPlatformType GetPlatformType()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      return ClipboardPlatformType.Linux;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      return ClipboardPlatformType.OSX;
    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ClipboardPlatformType.Windows : ClipboardPlatformType.Unknown;
  }

  /// <summary>Retrieves the clipboard text from the native SDL API.</summary>
  /// <returns>Returns a pointer to a null-terminated C-string, containing the clipboard text. May be empty if an error occurred.</returns>
  protected virtual IntPtr GetTextImpl()
  {
    throw new NotImplementedException($"GetClipboardText() for {this.PlatformName} is not provided on this platform!");
  }

  /// <summary>Sets the clipboard text using the native SDL API.</summary>
  /// <param name="text">A pointer to a null-terminated, UTF-8 C-string.</param>
  protected virtual int SetTextImpl(IntPtr text)
  {
    throw new NotImplementedException($"SetClipboardText(...) for {this.PlatformName} is not provided on this platform!");
  }
}
