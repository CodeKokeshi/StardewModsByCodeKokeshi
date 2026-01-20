// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerCollection
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class NetFarmerCollection : 
  INetObject<NetFields>,
  ICollection<Farmer>,
  IEnumerable<Farmer>,
  IEnumerable
{
  private List<Farmer> farmers = new List<Farmer>();
  private NetLongDictionary<bool, NetBool> uids = new NetLongDictionary<bool, NetBool>();

  public NetFields NetFields { get; } = new NetFields(nameof (NetFarmerCollection));

  public int Count => this.farmers.Count;

  public bool IsReadOnly => false;

  public event NetFarmerCollection.FarmerEvent FarmerAdded;

  public event NetFarmerCollection.FarmerEvent FarmerRemoved;

  public NetFarmerCollection()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.uids, nameof (uids));
    this.uids.OnValueAdded += (NetDictionary<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>.ContentsChangeEvent) ((uid, _) =>
    {
      Farmer farmer = this.getFarmer(uid);
      if (farmer == null || this.farmers.Contains(farmer))
        return;
      this.farmers.Add(farmer);
      NetFarmerCollection.FarmerEvent farmerAdded = this.FarmerAdded;
      if (farmerAdded == null)
        return;
      farmerAdded(farmer);
    });
    this.uids.OnValueRemoved += (NetDictionary<long, bool, NetBool, SerializableDictionary<long, bool>, NetLongDictionary<bool, NetBool>>.ContentsChangeEvent) ((uid, _) =>
    {
      Farmer farmer = this.getFarmer(uid);
      if (farmer == null)
        return;
      this.farmers.Remove(farmer);
      NetFarmerCollection.FarmerEvent farmerRemoved = this.FarmerRemoved;
      if (farmerRemoved == null)
        return;
      farmerRemoved(farmer);
    });
  }

  private static bool playerIsOnline(long uid)
  {
    if (Game1.player.UniqueMultiplayerID == uid || (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && Game1.serverHost.Value.UniqueMultiplayerID == uid)
      return true;
    return Game1.otherFarmers.ContainsKey(uid) && !Game1.multiplayer.isDisconnecting(uid);
  }

  public bool RetainOnlinePlayers()
  {
    int length = this.uids.Length;
    if (length == 0)
      return false;
    this.uids.RemoveWhere((Func<KeyValuePair<long, bool>, bool>) (pair => !NetFarmerCollection.playerIsOnline(pair.Key)));
    this.farmers.Clear();
    foreach (long key in this.uids.Keys)
    {
      Farmer farmer = this.getFarmer(key);
      if (farmer != null)
        this.farmers.Add(farmer);
    }
    return this.uids.Length < length;
  }

  private Farmer getFarmer(long uid)
  {
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
    {
      if (onlineFarmer.UniqueMultiplayerID == uid)
        return onlineFarmer;
    }
    return (Farmer) null;
  }

  public void Add(Farmer item)
  {
    this.farmers.Add(item);
    this.uids.TryAdd(item.UniqueMultiplayerID, true);
  }

  public void Clear()
  {
    this.farmers.Clear();
    this.uids.Clear();
  }

  public bool Contains(Farmer item) => this.farmers.Contains(item);

  public void CopyTo(Farmer[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException();
    if (arrayIndex < 0)
      throw new ArgumentOutOfRangeException();
    if (this.Count - arrayIndex > array.Length)
      throw new ArgumentException();
    foreach (Farmer farmer in this)
      array[arrayIndex++] = farmer;
  }

  public bool Remove(Farmer item)
  {
    this.uids.Remove(item.UniqueMultiplayerID);
    return this.farmers.Remove(item);
  }

  public IEnumerator<Farmer> GetEnumerator() => (IEnumerator<Farmer>) this.farmers.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public delegate void FarmerEvent(Farmer f);
}
