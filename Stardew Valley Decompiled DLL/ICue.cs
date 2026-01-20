// Decompiled with JetBrains decompiler
// Type: StardewValley.ICue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley;

public interface ICue : IDisposable
{
  void Play();

  void Pause();

  void Resume();

  void Stop(AudioStopOptions options);

  void SetVariable(string var, int val);

  void SetVariable(string var, float val);

  float GetVariable(string var);

  bool IsStopped { get; }

  bool IsStopping { get; }

  bool IsPlaying { get; }

  bool IsPaused { get; }

  string Name { get; }

  float Pitch { get; set; }

  float Volume { get; set; }

  bool IsPitchBeingControlledByRPC { get; }
}
