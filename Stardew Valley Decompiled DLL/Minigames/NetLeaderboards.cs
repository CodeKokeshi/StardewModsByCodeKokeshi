// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.NetLeaderboards
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Minigames;

public class NetLeaderboards : INetObject<NetFields>
{
  public NetObjectList<NetLeaderboardsEntry> entries = new NetObjectList<NetLeaderboardsEntry>();
  public NetInt maxEntries = new NetInt(10);

  public NetFields NetFields { get; } = new NetFields(nameof (NetLeaderboards));

  public void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.entries, "entries").AddField((INetSerializable) this.maxEntries, "maxEntries");
  }

  public NetLeaderboards() => this.InitNetFields();

  public void AddScore(string name, int score)
  {
    List<NetLeaderboardsEntry> list = new List<NetLeaderboardsEntry>((IEnumerable<NetLeaderboardsEntry>) this.entries);
    list.Add(new NetLeaderboardsEntry(name, score));
    list.Sort((Comparison<NetLeaderboardsEntry>) ((a, b) => a.score.Value.CompareTo(b.score.Value)));
    list.Reverse();
    while (list.Count > this.maxEntries.Value)
      list.RemoveAt(list.Count - 1);
    this.entries.Set((IList<NetLeaderboardsEntry>) list);
  }

  public List<KeyValuePair<string, int>> GetScores()
  {
    List<KeyValuePair<string, int>> scores = new List<KeyValuePair<string, int>>();
    foreach (NetLeaderboardsEntry entry in (NetList<NetLeaderboardsEntry, NetRef<NetLeaderboardsEntry>>) this.entries)
      scores.Add(new KeyValuePair<string, int>(entry.name.Value, entry.score.Value));
    scores.Sort((Comparison<KeyValuePair<string, int>>) ((a, b) => a.Value.CompareTo(b.Value)));
    scores.Reverse();
    return scores;
  }

  public void LoadScores(List<KeyValuePair<string, int>> scores)
  {
    this.entries.Clear();
    foreach (KeyValuePair<string, int> score in scores)
      this.AddScore(score.Key, score.Value);
  }
}
