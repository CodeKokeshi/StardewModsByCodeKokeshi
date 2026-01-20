// Decompiled with JetBrains decompiler
// Type: StardewValley.DummyCue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley;

public class DummyCue : ICue, IDisposable
{
  public void Play()
  {
  }

  public void Pause()
  {
  }

  public void Resume()
  {
  }

  public void SetVariable(string var, int val)
  {
  }

  public void SetVariable(string var, float val)
  {
  }

  public float GetVariable(string var) => 0.0f;

  public bool IsStopped => true;

  public bool IsStopping => false;

  public bool IsPlaying => false;

  public bool IsPaused => false;

  public string Name => "";

  public void Stop(AudioStopOptions options)
  {
  }

  public void Dispose()
  {
  }

  public float Volume
  {
    get => 1f;
    set
    {
    }
  }

  public float Pitch
  {
    get => 0.0f;
    set
    {
    }
  }

  public bool IsPitchBeingControlledByRPC => true;
}
