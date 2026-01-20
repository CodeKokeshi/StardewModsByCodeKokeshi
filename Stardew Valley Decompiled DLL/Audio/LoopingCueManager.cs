// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.LoopingCueManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Audio;
using Netcode;
using StardewValley.Network;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Audio;

public class LoopingCueManager
{
  private Dictionary<string, ICue> playingCues = new Dictionary<string, ICue>();
  private List<string> cuesToStop = new List<string>();

  public virtual void Update(GameLocation currentLocation)
  {
    NetDictionary<string, bool, NetBool, SerializableDictionary<string, bool>, NetStringDictionary<bool, NetBool>>.KeysCollection activeCues = currentLocation.netAudio.ActiveCues;
    foreach (string str in activeCues)
    {
      if (!this.playingCues.ContainsKey(str))
      {
        ICue cue;
        Game1.playSound(str, out cue);
        this.playingCues[str] = cue;
      }
    }
    foreach (KeyValuePair<string, ICue> playingCue in this.playingCues)
    {
      string key = playingCue.Key;
      if (!activeCues.Contains(key))
        this.cuesToStop.Add(key);
    }
    foreach (string key in this.cuesToStop)
    {
      this.playingCues[key].Stop(AudioStopOptions.AsAuthored);
      this.playingCues.Remove(key);
    }
    this.cuesToStop.Clear();
  }

  public void StopAll()
  {
    foreach (ICue cue in this.playingCues.Values)
      cue.Stop(AudioStopOptions.Immediate);
    this.playingCues.Clear();
  }
}
