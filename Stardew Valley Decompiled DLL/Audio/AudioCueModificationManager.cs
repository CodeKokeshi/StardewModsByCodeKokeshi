// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.AudioCueModificationManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using StardewValley.Extensions;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Audio;

/// <summary>Applies audio changes from the <c>Data/AudioChanges</c> asset to the game's soundbank.</summary>
public class AudioCueModificationManager
{
  /// <summary>The audio changes to apply from the <c>Data/AudioChanges</c> asset.</summary>
  public Dictionary<string, AudioCueData> cueModificationData;

  /// <summary>Initialize the manager when the game starts.</summary>
  public void OnStartup()
  {
    this.cueModificationData = DataLoader.AudioChanges(Game1.content);
    this.ApplyAllCueModifications();
  }

  /// <summary>Apply all changes registered through the <c>Data/AudioChanges</c> asset.</summary>
  public virtual void ApplyAllCueModifications()
  {
    foreach (string key in this.cueModificationData.Keys)
      this.ApplyCueModification(key);
  }

  /// <summary>Get the absolute file path for a content-relative path.</summary>
  /// <param name="filePath">The file path relative to the game's <c>Content</c> folder.</param>
  public virtual string GetFilePath(string filePath)
  {
    return Path.Combine(Game1.content.RootDirectory, filePath);
  }

  /// <summary>Apply a change registered through the <c>Data/AudioChanges</c> asset.</summary>
  /// <param name="key">The entry key to apply in the asset.</param>
  public virtual void ApplyCueModification(string key)
  {
    try
    {
      AudioCueData audioCueData;
      if (!this.cueModificationData.TryGetValue(key, out audioCueData))
        return;
      bool flag = false;
      int categoryIndex = Game1.audioEngine.GetCategoryIndex("Default");
      CueDefinition definition;
      if (Game1.soundBank.Exists(audioCueData.Id))
      {
        definition = Game1.soundBank.GetCueDefinition(audioCueData.Id);
        flag = true;
      }
      else
      {
        definition = new CueDefinition();
        definition.name = audioCueData.Id;
      }
      if (audioCueData.Category != null)
        categoryIndex = Game1.audioEngine.GetCategoryIndex(audioCueData.Category);
      if (audioCueData.FilePaths != null)
      {
        SoundEffect[] array = new SoundEffect[audioCueData.FilePaths.Count];
        for (int index = 0; index < audioCueData.FilePaths.Count; ++index)
        {
          string filePath = this.GetFilePath(audioCueData.FilePaths[index]);
          bool vorbis = Path.GetExtension(filePath).EqualsIgnoreCase(".ogg");
          int num = 0;
          try
          {
            SoundEffect soundEffect;
            if (vorbis && audioCueData.StreamedVorbis)
            {
              soundEffect = (SoundEffect) new OggStreamSoundEffect(filePath);
            }
            else
            {
              using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                soundEffect = SoundEffect.FromStream((Stream) fileStream, vorbis);
            }
            array[index - num] = soundEffect;
          }
          catch (Exception ex)
          {
            Game1.log.Error("Error loading sound: " + filePath, ex);
            ++num;
          }
          if (num > 0)
            Array.Resize<SoundEffect>(ref array, array.Length - num);
        }
        definition.SetSound(array, categoryIndex, audioCueData.Looped, audioCueData.UseReverb);
        if (flag)
        {
          Action onModified = definition.OnModified;
          if (onModified != null)
            onModified();
        }
      }
      Game1.soundBank.AddCue(definition);
    }
    catch (NoAudioHardwareException ex)
    {
      Game1.log.Warn($"Can't apply modifications for audio cue '{key}' because there's no audio hardware available.");
    }
  }
}
