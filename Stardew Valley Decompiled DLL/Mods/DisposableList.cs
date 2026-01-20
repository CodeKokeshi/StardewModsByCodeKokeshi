// Decompiled with JetBrains decompiler
// Type: StardewValley.DisposableList`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

[Obsolete("This is only kept for backwards compatibility. It should no longer be used, and no longer does anything besides wrap the provided list.")]
public struct DisposableList<T>(List<T> list)
{
  private readonly List<T> _list = list;

  public DisposableList<T>.Enumerator GetEnumerator() => new DisposableList<T>.Enumerator(this);

  public struct Enumerator(DisposableList<T> parent) : IDisposable
  {
    private readonly DisposableList<T> _parent = parent;
    private int _index = 0;

    public T Current
    {
      get
      {
        if (this._parent._list == null || this._index == 0)
          throw new InvalidOperationException();
        return this._parent._list[this._index - 1];
      }
    }

    public bool MoveNext()
    {
      ++this._index;
      return this._parent._list != null && this._parent._list.Count >= this._index;
    }

    public void Reset() => this._index = 0;

    public void Dispose()
    {
    }
  }
}
