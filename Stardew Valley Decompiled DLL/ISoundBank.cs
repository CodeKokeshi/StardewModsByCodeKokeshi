// Decompiled with JetBrains decompiler
// Type: StardewValley.ISoundBank
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley;

/// <summary>The game API for getting and playing sounds.</summary>
public interface ISoundBank : IDisposable
{
  /// <summary>Whether there are any live cues in use from this sound bank.</summary>
  bool IsInUse { get; }

  /// <summary>Whether the sound bank has been disposed.</summary>
  bool IsDisposed { get; }

  /// <summary>Get a cue representing a sound in the sound bank.</summary>
  /// <param name="name">The sound ID to get.</param>
  /// <remarks>Cue instances are unique, even when sharing the same name. This allows multiple instances to simultaneously play.</remarks>
  ICue GetCue(string name);

  /// <summary>Play a sound defined in the sound bank.</summary>
  /// <param name="name">The sound ID to play.</param>
  void PlayCue(string name);

  /// <summary>Plays a sound defined in the sound bank with static 3D positional information.</summary>
  /// <param name="name">The sound ID to play.</param>
  /// <param name="listener">The listener state.</param>
  /// <param name="emitter">The cue emitter state.</param>
  void PlayCue(string name, AudioListener listener, AudioEmitter emitter);

  /// <summary>Add a sound to the sound bank.</summary>
  /// <param name="definition">The sound definition to add.</param>
  void AddCue(CueDefinition definition);

  /// <summary>Get whether a given cue currently exists in the sound bank.</summary>
  /// <param name="name">The cue name to find.</param>
  bool Exists(string name);

  /// <summary>Get the underlying definition for a sound in the sound bank.</summary>
  /// <param name="name">The sound ID to get.</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="name" /> doesn't match a sound in the bank.</exception>
  CueDefinition GetCueDefinition(string name);
}
