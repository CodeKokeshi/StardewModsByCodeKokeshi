// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.IScreenReadable
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Menus;

/// <summary>A UI element that provides information for screen readers.</summary>
/// <remarks>These values aren't displayed by the game; they're provided to allow for implementing screen reader mods.</remarks>
public interface IScreenReadable
{
  /// <summary>If set, the translated text which represents this component for a screen reader. This may be the displayed text (for a text component), or an equivalent representation (e.g. "exit" for an 'X' button).</summary>
  string ScreenReaderText { get; }

  /// <summary>If set, a translated tooltip-like description for this component which can be displayed by screen readers, in addition to the <see cref="P:StardewValley.Menus.IScreenReadable.ScreenReaderText" />.</summary>
  string ScreenReaderDescription { get; }

  /// <summary>Whether this is a purely visual component which should be ignored by screen readers.</summary>
  bool ScreenReaderIgnore { get; }
}
