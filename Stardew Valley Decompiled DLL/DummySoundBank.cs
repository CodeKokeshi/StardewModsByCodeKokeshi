// Decompiled with JetBrains decompiler
// Type: StardewValley.DummySoundBank
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley;

/// <summary>A sound bank implementation which does nothing, used when the game can't play audio.</summary>
public class DummySoundBank : ISoundBank, IDisposable
{
  /// <summary>An empty cue instance which does nothing.</summary>
  internal static readonly ICue DummyCue = (ICue) new StardewValley.DummyCue();

  /// <inheritdoc />
  public bool IsInUse => false;

  /// <inheritdoc />
  public bool IsDisposed => true;

  /// <inheritdoc />
  public bool Exists(string name) => true;

  /// <inheritdoc />
  public ICue GetCue(string name) => DummySoundBank.DummyCue;

  /// <inheritdoc />
  public void PlayCue(string name)
  {
  }

  /// <inheritdoc />
  public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
  {
  }

  /// <inheritdoc />
  public void AddCue(CueDefinition definition)
  {
  }

  /// <inheritdoc />
  public CueDefinition GetCueDefinition(string name) => (CueDefinition) null;

  /// <summary>An empty cue instance which does nothing.</summary>
  public void Dispose()
  {
  }
}
