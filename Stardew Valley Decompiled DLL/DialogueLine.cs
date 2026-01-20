// Decompiled with JetBrains decompiler
// Type: StardewValley.DialogueLine
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley;

/// <summary>As part of <see cref="T:StardewValley.Dialogue" />, a bit of dialogue shown in its own message box or an action to run when it's selected.</summary>
public class DialogueLine
{
  /// <summary>The text to display, or <see cref="F:System.String.Empty" /> to skip displaying text.</summary>
  public string Text;
  /// <summary>The action to perform when the dialogue is displayed.</summary>
  public Action SideEffects;

  /// <summary>Whether this entry has dialogue text to display.</summary>
  public bool HasText => !string.IsNullOrEmpty(this.Text) && this.Text != "{";

  /// <summary>Construct an instance.</summary>
  /// <param name="text">The text to display, or <see cref="F:System.String.Empty" /> to skip displaying text.</param>
  /// <param name="sideEffects">The action to perform when the dialogue is displayed.</param>
  public DialogueLine(string text, Action sideEffects = null)
  {
    this.Text = text ?? "";
    this.SideEffects = sideEffects;
  }
}
