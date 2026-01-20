// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetMutexQueue`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Network;

public class NetMutexQueue<T> : INetObject<NetFields>
{
  private readonly NetLongDictionary<bool, NetBool> requests;
  private readonly NetLong currentOwner;
  private readonly List<T> localJobs;
  [XmlIgnore]
  public Action<T> Processor;

  [XmlIgnore]
  public NetFields NetFields { get; }

  public NetMutexQueue()
  {
    NetLongDictionary<bool, NetBool> netLongDictionary = new NetLongDictionary<bool, NetBool>();
    netLongDictionary.InterpolationWait = false;
    this.requests = netLongDictionary;
    NetLong netLong = new NetLong();
    netLong.InterpolationWait = false;
    this.currentOwner = netLong;
    this.localJobs = new List<T>();
    this.Processor = (Action<T>) (x => { });
    this.NetFields = new NetFields(nameof (NetMutexQueue<T>));
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.requests, nameof (requests)).AddField((INetSerializable) this.currentOwner, nameof (currentOwner));
  }

  public void Add(T job) => this.localJobs.Add(job);

  public bool Contains(T job) => this.localJobs.Contains(job);

  public void Clear() => this.localJobs.Clear();

  public void Update(GameLocation location)
  {
    FarmerCollection farmers = location.farmers;
    if (farmers.Contains(Game1.player) && this.localJobs.Count > 0)
      this.requests[Game1.player.UniqueMultiplayerID] = true;
    else
      this.requests.Remove(Game1.player.UniqueMultiplayerID);
    if (Game1.IsMasterGame)
    {
      this.requests.RemoveWhere((Func<KeyValuePair<long, bool>, bool>) (pair => farmers.FirstOrDefault<Farmer>((Func<Farmer, bool>) (f => f.UniqueMultiplayerID == pair.Key)) == null));
      if (!this.requests.ContainsKey(this.currentOwner.Value))
        this.currentOwner.Value = -1L;
    }
    if (this.currentOwner.Value == Game1.player.UniqueMultiplayerID)
    {
      foreach (T localJob in this.localJobs)
        this.Processor(localJob);
      this.localJobs.Clear();
      this.requests.Remove(Game1.player.UniqueMultiplayerID);
      this.currentOwner.Value = -1L;
    }
    long key;
    if (!Game1.IsMasterGame || this.currentOwner.Value != -1L || !Utility.TryGetRandom<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>((NetDictionary<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>) this.requests, out key, out bool _))
      return;
    this.currentOwner.Value = key;
  }
}
