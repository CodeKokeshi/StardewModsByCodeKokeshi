// Decompiled with JetBrains decompiler
// Type: StardewValley.SoundBankWrapper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley;

/// <summary>The default sound bank implementation which defers to MonoGame audio.</summary>
public class SoundBankWrapper : ISoundBank, IDisposable
{
  /// <summary>The audio cue name used when a non-existent audio cue is requested to avoid a game crash.</summary>
  private string DefaultCueName = "shiny4";
  /// <summary>The underlying MonoGame sound bank.</summary>
  private SoundBank soundBank;

  /// <inheritdoc />
  public bool IsInUse => this.soundBank.IsInUse;

  /// <inheritdoc />
  public bool IsDisposed => this.soundBank.IsDisposed;

  /// <summary>Construct an instance.</summary>
  /// <param name="soundBank">The underlying MonoGame sound bank.</param>
  public SoundBankWrapper(SoundBank soundBank) => this.soundBank = soundBank;

  /// <inheritdoc />
  public ICue GetCue(string name)
  {
    if (!this.Exists(name))
    {
      Game1.log.Error($"Can't get audio ID '{name}' because it doesn't exist.");
      name = this.DefaultCueName;
    }
    return (ICue) new CueWrapper(this.soundBank.GetCue(name));
  }

  /// <inheritdoc />
  public void PlayCue(string name)
  {
    if (!this.Exists(name))
    {
      Game1.log.Error($"Can't play audio ID '{name}' because it doesn't exist.");
      name = this.DefaultCueName;
    }
    this.soundBank.PlayCue(name);
  }

  /// <inheritdoc />
  public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
  {
    this.soundBank.PlayCue(name, listener, emitter);
  }

  /// <inheritdoc />
  public void Dispose() => this.soundBank.Dispose();

  /// <inheritdoc />
  public void AddCue(CueDefinition definition) => this.soundBank.AddCue(definition);

  /// <inheritdoc />
  public bool Exists(string name) => this.soundBank.Exists(name);

  /// <inheritdoc />
  public CueDefinition GetCueDefinition(string name) => this.soundBank.GetCueDefinition(name);
}
