// Decompiled with JetBrains decompiler
// Type: Netcode.SerializationCollectionFacade`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Netcode;

public abstract class SerializationCollectionFacade<SerialT> : IEnumerable<SerialT>, IEnumerable
{
  protected abstract List<SerialT> Serialize();

  protected abstract void DeserializeAdd(SerialT serialElem);

  public IEnumerator<SerialT> GetEnumerator()
  {
    return (IEnumerator<SerialT>) this.Serialize().GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(SerialT value) => this.DeserializeAdd(value);
}
