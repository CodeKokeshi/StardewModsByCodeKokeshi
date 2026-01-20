// Decompiled with JetBrains decompiler
// Type: Netcode.NetFieldDictionary`5
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace Netcode;

public abstract class NetFieldDictionary<TKey, TValue, TField, TSerialDict, TSelf> : 
  NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
  where TField : NetField<TValue, TField>, new()
  where TSerialDict : IDictionary<TKey, TValue>, new()
  where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
{
  public NetFieldDictionary()
  {
  }

  public NetFieldDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    : base(pairs)
  {
  }

  protected override void setFieldValue(TField field, TKey key, TValue value)
  {
    field.Value = value;
  }

  protected override TValue getFieldValue(TField field) => field.Value;

  protected override TValue getFieldTargetValue(TField field) => field.TargetValue;
}
