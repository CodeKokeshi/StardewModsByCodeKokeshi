// Decompiled with JetBrains decompiler
// Type: StardewValley.DesktopClipboard
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.NativeClipboard;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TextCopy;

#nullable disable
namespace StardewValley;

public class DesktopClipboard
{
  public const bool IsAvailable = true;

  public static bool GetText(ref string output)
  {
    output = SdlClipboard.GetText();
    if (output != null)
      return true;
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      output = "";
      output = ClipboardService.GetText();
      return true;
    }
    return DesktopClipboard.externalGetText("xclip", "-o", ref output) || DesktopClipboard.externalGetText("pbpaste", "", ref output);
  }

  public static bool SetText(string text)
  {
    if (text == null)
      text = "";
    if (SdlClipboard.SetText(text))
      return true;
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      ClipboardService.SetText(text);
      return true;
    }
    return DesktopClipboard.externalSetText("xclip", "-selection clipboard", text) || DesktopClipboard.externalSetText("pbcopy", "", text);
  }

  private static bool externalSetText(string executable, string arguments, string text)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo(executable, arguments)
    {
      RedirectStandardInput = true,
      UseShellExecute = false
    };
    try
    {
      using (Process process = Process.Start(startInfo))
      {
        process.StandardInput.Write(text);
        process.StandardInput.Close();
        process.WaitForExit();
        return process.ExitCode == 0;
      }
    }
    catch (Exception ex)
    {
      return false;
    }
  }

  private static bool externalGetText(string executable, string arguments, ref string output)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo(executable, arguments)
    {
      RedirectStandardOutput = true,
      UseShellExecute = false
    };
    try
    {
      using (Process process = Process.Start(startInfo))
      {
        string end = process.StandardOutput.ReadToEnd();
        process.StandardOutput.Close();
        process.WaitForExit();
        output = process.ExitCode != 0 ? "" : end;
        return true;
      }
    }
    catch (Exception ex)
    {
      return false;
    }
  }
}
