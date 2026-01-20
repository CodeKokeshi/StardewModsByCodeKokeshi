// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.SoundsHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;
using System.Text;

#nullable disable
namespace StardewValley.Audio;

/// <inheritdoc cref="T:StardewValley.Audio.ISoundsHelper" />
public class SoundsHelper : ISoundsHelper
{
  /// <summary>The default pitch value.</summary>
  public const float DefaultPitch = 1200f;
  /// <summary>The maximum pitch value.</summary>
  public const float MaxPitch = 2400f;
  /// <summary>The maximum distance from the screen at which a positional sound can play. The audio volume drops linearly until it reaches zero.</summary>
  public static int MaxDistanceFromScreen = 12;
  /// <summary>The method which logs sounds, if logging is enabled.</summary>
  private Action<string, GameLocation, Vector2?, int?, float, SoundContext, string> LogSound;

  /// <inheritdoc />
  public virtual bool LogSounds
  {
    get => this.LogSound != null;
    set
    {
      if (value)
        this.LogSound = new Action<string, GameLocation, Vector2?, int?, float, SoundContext, string>(this.LogSoundImpl);
      else
        this.LogSound = (Action<string, GameLocation, Vector2?, int?, float, SoundContext, string>) null;
    }
  }

  /// <inheritdoc />
  public virtual bool ShouldPlayLocal(SoundContext context)
  {
    return context != SoundContext.NPC || !Game1.eventUp;
  }

  /// <inheritdoc />
  public virtual float GetVolumeForDistance(GameLocation location, Vector2? position)
  {
    if (location == null)
      return 1f;
    if (location.NameOrUniqueName != Game1.currentLocation?.NameOrUniqueName)
      return 0.0f;
    if (!position.HasValue)
      return 1f;
    float num = Utility.distanceFromScreen(position.Value * 64f) / 64f;
    if ((double) num <= 0.0)
      return 1f;
    return (double) num >= (double) SoundsHelper.MaxDistanceFromScreen ? 0.0f : (float) (1.0 - (double) num / (double) SoundsHelper.MaxDistanceFromScreen);
  }

  /// <inheritdoc />
  public virtual bool PlayLocal(
    string cueName,
    GameLocation location,
    Vector2? position,
    int? pitch,
    SoundContext context,
    out ICue cue)
  {
    try
    {
      cue = Game1.soundBank.GetCue(cueName);
      this.SetPitch(cue, (float) ((double) pitch ?? 1200.0), pitch.HasValue);
      if (!this.ShouldPlayLocal(context))
      {
        Action<string, GameLocation, Vector2?, int?, float, SoundContext, string> logSound = this.LogSound;
        if (logSound != null)
          logSound(cueName, location, position, pitch, 1f, context, "disabled for context");
        return false;
      }
      float volumeForDistance = this.GetVolumeForDistance(location, position);
      if ((double) volumeForDistance <= 0.0)
      {
        Action<string, GameLocation, Vector2?, int?, float, SoundContext, string> logSound = this.LogSound;
        if (logSound != null)
          logSound(cueName, location, position, pitch, volumeForDistance, context, "disabled for distance");
        return false;
      }
      cue.Play();
      if ((double) volumeForDistance < 1.0)
        cue.Volume *= volumeForDistance;
      Action<string, GameLocation, Vector2?, int?, float, SoundContext, string> logSound1 = this.LogSound;
      if (logSound1 != null)
        logSound1(cueName, location, position, pitch, volumeForDistance, context, (string) null);
      return true;
    }
    catch (Exception ex)
    {
      Game1.debugOutput = Game1.parseText(ex.Message);
      Game1.log.Error("Error playing sound.", ex);
      cue = DummySoundBank.DummyCue;
      return false;
    }
  }

  /// <inheritdoc />
  public virtual void PlayAll(
    string cueName,
    GameLocation location,
    Vector2? position,
    int? pitch,
    SoundContext context)
  {
    if (this.CanSkipSoundSync(location, position, context))
      this.PlayLocal(cueName, location, position, pitch, context, out ICue _);
    else
      location.netAudio.Fire(cueName, position, pitch, context);
  }

  /// <inheritdoc />
  public void SetPitch(ICue cue, float pitch, bool forcePitch = true)
  {
    if (cue == null)
      return;
    cue.SetVariable("Pitch", pitch);
    if (!forcePitch)
      return;
    try
    {
      if (cue.IsPitchBeingControlledByRPC)
        return;
      cue.Pitch = Utility.Lerp(-1f, 1f, pitch / 2400f);
    }
    catch
    {
    }
  }

  /// <summary>Get whether a multiplayer sound can be played directly without syncing it to other players.</summary>
  /// <param name="location">The location in which the sound is playing.</param>
  /// <param name="position">The tile position from which the sound is playing.</param>
  /// <param name="context">The source which triggered the sound.</param>
  public virtual bool CanSkipSoundSync(
    GameLocation location,
    Vector2? position,
    SoundContext context)
  {
    if (!LocalMultiplayer.IsLocalMultiplayer(true) || Game1.eventUp && context == SoundContext.NPC)
      return false;
    if (this.ShouldPlayLocal(context) && (double) this.GetVolumeForDistance(location, position) > 0.0 || location == null)
      return true;
    bool someoneCanHear = false;
    foreach (Game1 gameInstance in GameRunner.instance.gameInstances)
    {
      if (gameInstance.instanceGameLocation?.NameOrUniqueName == location.NameOrUniqueName)
      {
        someoneCanHear = true;
        break;
      }
    }
    if (someoneCanHear && position.HasValue)
    {
      Vector2? nullable = position;
      Vector2 zero = Vector2.Zero;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != zero ? 1 : 0) : 1) != 0)
      {
        someoneCanHear = false;
        GameRunner.instance.ExecuteForInstances((Action<Game1>) (_ =>
        {
          if (someoneCanHear || !this.ShouldPlayLocal(context) || (double) this.GetVolumeForDistance(location, position) <= 0.0)
            return;
          someoneCanHear = true;
        }));
      }
    }
    return someoneCanHear;
  }

  /// <summary>Play a game sound for the local player.</summary>
  /// <param name="cueName">The sound ID to play.</param>
  /// <param name="location">The location in which the sound is playing, if applicable.</param>
  /// <param name="position">The tile position from which the sound is playing, or <c>null</c> if it's playing throughout the location.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
  /// <param name="volume">The distance-adjusted volume.</param>
  /// <param name="context">The source which triggered a game sound.</param>
  /// <param name="skipReason">The reason the sound wasn't played, if applicable.</param>
  protected virtual void LogSoundImpl(
    string cueName,
    GameLocation location,
    Vector2? position,
    int? pitch,
    float volume,
    SoundContext context,
    string skipReason = null)
  {
    int num = skipReason != null ? 1 : 0;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("Played sound '").Append(cueName).Append("'");
    if (location == null)
    {
      stringBuilder.Append(" everywhere");
    }
    else
    {
      stringBuilder.Append(" in ").Append(location.NameOrUniqueName);
      if (position.HasValue)
        stringBuilder.Append(" (").Append(position.Value.X).Append(", ").Append(position.Value.Y).Append(")");
    }
    if (pitch.HasValue)
      stringBuilder.Append(" with pitch ").Append(pitch.Value);
    if (num == 0 && (double) volume < 1.0)
      stringBuilder.Append(" with distance").Append(volume);
    if (num != 0)
      stringBuilder.Append(" (").Append(skipReason).Append(")");
    Game1.log.Debug(stringBuilder.ToString());
  }
}
