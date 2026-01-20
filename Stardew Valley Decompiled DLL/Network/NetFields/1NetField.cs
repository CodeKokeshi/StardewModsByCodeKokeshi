// Decompiled with JetBrains decompiler
// Type: Netcode.NetField`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Netcode;

public abstract class NetField<T, TSelf> : NetFieldBase<T, TSelf>, IEnumerable<T>, IEnumerable where TSelf : NetField<T, TSelf>
{
  private bool xmlInitialized;

  public NetField()
  {
  }

  public NetField(T value)
    : base(value)
  {
  }

  public IEnumerator<T> GetEnumerator() => Enumerable.Repeat<T>(this.Get(), 1).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(T value)
  {
    if (this.xmlInitialized || this.Parent != null)
      throw new InvalidOperationException($"{this.GetType().Name} already has value {this.ToString()}");
    this.cleanSet(value);
    this.xmlInitialized = true;
  }
}
