// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.DummyAudioEngine
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using System;

#nullable disable
namespace StardewValley.Audio;

internal class DummyAudioEngine : IAudioEngine, IDisposable
{
  private IAudioCategory category = (IAudioCategory) new DummyAudioCategory();

  public AudioEngine Engine { get; }

  public bool IsDisposed { get; } = true;

  public void Update()
  {
  }

  public IAudioCategory GetCategory(string name) => this.category;

  public int GetCategoryIndex(string name) => -1;

  public void Dispose()
  {
  }
}
