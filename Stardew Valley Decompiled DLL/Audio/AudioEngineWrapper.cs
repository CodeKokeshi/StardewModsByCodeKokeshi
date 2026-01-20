// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.AudioEngineWrapper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley.Audio;

internal class AudioEngineWrapper : IAudioEngine, IDisposable
{
  private AudioEngine audioEngine;

  public AudioEngine Engine => this.audioEngine;

  public bool IsDisposed => this.audioEngine.IsDisposed;

  public AudioEngineWrapper(AudioEngine engine) => this.audioEngine = engine;

  public void Dispose() => this.audioEngine.Dispose();

  public IAudioCategory GetCategory(string name)
  {
    return (IAudioCategory) new AudioCategoryWrapper(this.audioEngine.GetCategory(name));
  }

  public int GetCategoryIndex(string name) => this.audioEngine.GetCategoryIndex(name);

  public void Update() => this.audioEngine.Update();
}
