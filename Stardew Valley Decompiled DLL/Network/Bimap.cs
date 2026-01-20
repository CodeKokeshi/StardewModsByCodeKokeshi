// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Bimap`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class Bimap<L, R> : IEnumerable<KeyValuePair<L, R>>, IEnumerable
{
  private Dictionary<L, R> leftToRight = new Dictionary<L, R>();
  private Dictionary<R, L> rightToLeft = new Dictionary<R, L>();

  public R this[L l]
  {
    get => this.leftToRight[l];
    set
    {
      R key1;
      if (this.leftToRight.TryGetValue(l, out key1))
        this.rightToLeft.Remove(key1);
      L key2;
      if (this.rightToLeft.TryGetValue(value, out key2))
        this.leftToRight.Remove(key2);
      this.leftToRight[l] = value;
      this.rightToLeft[value] = l;
    }
  }

  public L this[R r]
  {
    get => this.rightToLeft[r];
    set
    {
      L key1;
      if (this.rightToLeft.TryGetValue(r, out key1))
        this.leftToRight.Remove(key1);
      R key2;
      if (this.leftToRight.TryGetValue(value, out key2))
        this.rightToLeft.Remove(key2);
      this.rightToLeft[r] = value;
      this.leftToRight[value] = r;
    }
  }

  public ICollection<L> LeftValues => (ICollection<L>) this.leftToRight.Keys;

  public ICollection<R> RightValues => (ICollection<R>) this.rightToLeft.Keys;

  public int Count => this.rightToLeft.Count;

  public void Clear()
  {
    this.leftToRight.Clear();
    this.rightToLeft.Clear();
  }

  public void Add(L l, R r)
  {
    if (this.leftToRight.ContainsKey(l) || this.rightToLeft.ContainsKey(r))
      throw new ArgumentException();
    this.leftToRight.Add(l, r);
    this.rightToLeft.Add(r, l);
  }

  public bool ContainsLeft(L l) => this.leftToRight.ContainsKey(l);

  public bool ContainsRight(R r) => this.rightToLeft.ContainsKey(r);

  public void RemoveLeft(L l)
  {
    R key;
    if (this.leftToRight.TryGetValue(l, out key))
      this.rightToLeft.Remove(key);
    this.leftToRight.Remove(l);
  }

  public void RemoveRight(R r)
  {
    L key;
    if (this.rightToLeft.TryGetValue(r, out key))
      this.leftToRight.Remove(key);
    this.rightToLeft.Remove(r);
  }

  public L GetLeft(R r) => this.rightToLeft[r];

  public R GetRight(L l) => this.leftToRight[l];

  public IEnumerator<KeyValuePair<L, R>> GetEnumerator()
  {
    return (IEnumerator<KeyValuePair<L, R>>) this.leftToRight.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
