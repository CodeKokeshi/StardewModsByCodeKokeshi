// Decompiled with JetBrains decompiler
// Type: Netcode.SerializationFacade`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Netcode;

public abstract class SerializationFacade<SerialT> : IEnumerable<SerialT>, IEnumerable
{
  protected abstract SerialT Serialize();

  protected abstract void Deserialize(SerialT serialValue);

  public IEnumerator<SerialT> GetEnumerator()
  {
    return Enumerable.Repeat<SerialT>(this.Serialize(), 1).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public void Add(SerialT value) => this.Deserialize(value);
}
