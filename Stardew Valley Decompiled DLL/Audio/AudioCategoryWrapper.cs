// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.AudioCategoryWrapper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;

#nullable disable
namespace StardewValley.Audio;

public class AudioCategoryWrapper : IAudioCategory
{
  private AudioCategory audioCategory;

  public AudioCategoryWrapper(AudioCategory category) => this.audioCategory = category;

  public void SetVolume(float volume) => this.audioCategory.SetVolume(volume);
}
