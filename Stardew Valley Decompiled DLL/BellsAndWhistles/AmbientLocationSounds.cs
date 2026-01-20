// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.AmbientLocationSounds
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.BellsAndWhistles;

[InstanceStatics]
public class AmbientLocationSounds
{
  public const int sound_babblingBrook = 0;
  public const int sound_cracklingFire = 1;
  public const int sound_engine = 2;
  public const int sound_cricket = 3;
  public const int sound_waterfall = 4;
  public const int sound_waterfall_big = 5;
  public const int numberOfSounds = 6;
  public const float doNotPlay = 9999999f;
  private static Dictionary<Vector2, int> sounds = new Dictionary<Vector2, int>();
  private static int updateTimer = 100;
  private static int farthestSoundDistance = 1024 /*0x0400*/;
  private static float[] shortestDistanceForCue;
  private static ICue babblingBrook;
  private static ICue cracklingFire;
  private static ICue engine;
  private static ICue cricket;
  private static ICue waterfall;
  private static ICue waterfallBig;
  private static float volumeOverrideForLocChange;

  public static void InitShared()
  {
    if (AmbientLocationSounds.babblingBrook == null)
    {
      Game1.playSound("babblingBrook", out AmbientLocationSounds.babblingBrook);
      AmbientLocationSounds.babblingBrook.Pause();
    }
    if (AmbientLocationSounds.cracklingFire == null)
    {
      Game1.playSound("cracklingFire", out AmbientLocationSounds.cracklingFire);
      AmbientLocationSounds.cracklingFire.Pause();
    }
    if (AmbientLocationSounds.engine == null)
    {
      Game1.playSound("heavyEngine", out AmbientLocationSounds.engine);
      AmbientLocationSounds.engine.Pause();
    }
    if (AmbientLocationSounds.cricket == null)
    {
      Game1.playSound("cricketsAmbient", out AmbientLocationSounds.cricket);
      AmbientLocationSounds.cricket.Pause();
    }
    if (AmbientLocationSounds.waterfall == null)
    {
      Game1.playSound("waterfall", out AmbientLocationSounds.waterfall);
      AmbientLocationSounds.waterfall.Pause();
    }
    if (AmbientLocationSounds.waterfallBig == null)
    {
      Game1.playSound("waterfall_big", out AmbientLocationSounds.waterfallBig);
      AmbientLocationSounds.waterfallBig.Pause();
    }
    AmbientLocationSounds.shortestDistanceForCue = new float[6];
  }

  public static void update(GameTime time)
  {
    if (AmbientLocationSounds.sounds.Count == 0)
      return;
    if ((double) AmbientLocationSounds.volumeOverrideForLocChange < 1.0)
      AmbientLocationSounds.volumeOverrideForLocChange += (float) time.ElapsedGameTime.Milliseconds * 0.0003f;
    AmbientLocationSounds.updateTimer -= time.ElapsedGameTime.Milliseconds;
    if (AmbientLocationSounds.updateTimer > 0)
      return;
    for (int index = 0; index < AmbientLocationSounds.shortestDistanceForCue.Length; ++index)
      AmbientLocationSounds.shortestDistanceForCue[index] = 9999999f;
    Vector2 standingPosition = Game1.player.getStandingPosition();
    foreach (KeyValuePair<Vector2, int> sound in AmbientLocationSounds.sounds)
    {
      float num = Vector2.Distance(sound.Key, standingPosition);
      if ((double) AmbientLocationSounds.shortestDistanceForCue[sound.Value] > (double) num)
        AmbientLocationSounds.shortestDistanceForCue[sound.Value] = num;
    }
    if ((double) AmbientLocationSounds.volumeOverrideForLocChange >= 0.0)
    {
      for (int index = 0; index < AmbientLocationSounds.shortestDistanceForCue.Length; ++index)
      {
        if ((double) AmbientLocationSounds.shortestDistanceForCue[index] <= (double) AmbientLocationSounds.farthestSoundDistance * 1.5)
        {
          float num = (float) Math.Pow((double) Math.Min(AmbientLocationSounds.volumeOverrideForLocChange, Math.Min(1f, (float) (1.0 - (double) AmbientLocationSounds.shortestDistanceForCue[index] / ((double) AmbientLocationSounds.farthestSoundDistance * 1.5)))), 5.0);
          switch (index)
          {
            case 0:
              if (AmbientLocationSounds.babblingBrook != null)
              {
                AmbientLocationSounds.babblingBrook.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.babblingBrook.Resume();
                continue;
              }
              continue;
            case 1:
              if (AmbientLocationSounds.cracklingFire != null)
              {
                AmbientLocationSounds.cracklingFire.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.cracklingFire.Resume();
                continue;
              }
              continue;
            case 2:
              if (AmbientLocationSounds.engine != null)
              {
                AmbientLocationSounds.engine.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.engine.Resume();
                continue;
              }
              continue;
            case 3:
              if (AmbientLocationSounds.cricket != null)
              {
                AmbientLocationSounds.cricket.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.cricket.Resume();
                continue;
              }
              continue;
            case 4:
              if (AmbientLocationSounds.waterfall != null)
              {
                AmbientLocationSounds.waterfall.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.waterfall.Resume();
                continue;
              }
              continue;
            case 5:
              if (AmbientLocationSounds.waterfallBig != null)
              {
                AmbientLocationSounds.waterfallBig.SetVariable("Volume", num * 100f * Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel));
                AmbientLocationSounds.waterfallBig.Resume();
                continue;
              }
              continue;
            default:
              continue;
          }
        }
        else
        {
          switch (index)
          {
            case 0:
              ICue babblingBrook = AmbientLocationSounds.babblingBrook;
              if (babblingBrook != null)
              {
                babblingBrook.Pause();
                continue;
              }
              continue;
            case 1:
              ICue cracklingFire = AmbientLocationSounds.cracklingFire;
              if (cracklingFire != null)
              {
                cracklingFire.Pause();
                continue;
              }
              continue;
            case 2:
              ICue engine = AmbientLocationSounds.engine;
              if (engine != null)
              {
                engine.Pause();
                continue;
              }
              continue;
            case 3:
              ICue cricket = AmbientLocationSounds.cricket;
              if (cricket != null)
              {
                cricket.Pause();
                continue;
              }
              continue;
            case 4:
              ICue waterfall = AmbientLocationSounds.waterfall;
              if (waterfall != null)
              {
                waterfall.Pause();
                continue;
              }
              continue;
            case 5:
              ICue waterfallBig = AmbientLocationSounds.waterfallBig;
              if (waterfallBig != null)
              {
                waterfallBig.Pause();
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }
    AmbientLocationSounds.updateTimer = 100;
  }

  public static void changeSpecificVariable(string variableName, float value, int whichSound)
  {
    if (whichSound != 2)
      return;
    AmbientLocationSounds.engine?.SetVariable(variableName, value);
  }

  public static void addSound(Vector2 tileLocation, int whichSound)
  {
    AmbientLocationSounds.sounds.TryAdd(tileLocation * 64f, whichSound);
  }

  public static void removeSound(Vector2 tileLocation)
  {
    int num;
    if (!AmbientLocationSounds.sounds.TryGetValue(tileLocation * 64f, out num))
      return;
    switch (num)
    {
      case 0:
        ICue babblingBrook = AmbientLocationSounds.babblingBrook;
        if (babblingBrook != null)
        {
          babblingBrook.Pause();
          break;
        }
        break;
      case 1:
        ICue cracklingFire = AmbientLocationSounds.cracklingFire;
        if (cracklingFire != null)
        {
          cracklingFire.Pause();
          break;
        }
        break;
      case 2:
        ICue engine = AmbientLocationSounds.engine;
        if (engine != null)
        {
          engine.Pause();
          break;
        }
        break;
      case 3:
        ICue cricket = AmbientLocationSounds.cricket;
        if (cricket != null)
        {
          cricket.Pause();
          break;
        }
        break;
      case 4:
        ICue waterfall = AmbientLocationSounds.waterfall;
        if (waterfall != null)
        {
          waterfall.Pause();
          break;
        }
        break;
      case 5:
        ICue waterfallBig = AmbientLocationSounds.waterfallBig;
        if (waterfallBig != null)
        {
          waterfallBig.Pause();
          break;
        }
        break;
    }
    AmbientLocationSounds.sounds.Remove(tileLocation * 64f);
  }

  public static void onLocationLeave()
  {
    AmbientLocationSounds.sounds.Clear();
    AmbientLocationSounds.volumeOverrideForLocChange = -0.5f;
    AmbientLocationSounds.babblingBrook?.Pause();
    AmbientLocationSounds.cracklingFire?.Pause();
    if (AmbientLocationSounds.engine != null)
    {
      AmbientLocationSounds.engine.SetVariable("Frequency", 100f);
      AmbientLocationSounds.engine.Pause();
    }
    AmbientLocationSounds.cricket?.Pause();
    AmbientLocationSounds.waterfall?.Pause();
    AmbientLocationSounds.waterfallBig?.Pause();
  }
}
