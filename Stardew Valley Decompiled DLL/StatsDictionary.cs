// Decompiled with JetBrains decompiler
// Type: StardewValley.StatsDictionary`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley;

/// <summary>An implementation of <see cref="T:StardewValley.SerializableDictionary`2" /> specialized for storing <see cref="T:StardewValley.Stats" /> values.</summary>
/// <typeparam name="TValue">The numeric stat value type. This must be <see cref="T:System.Int32" />, <see cref="T:System.Int64" />, or <see cref="T:System.UInt32" />.</typeparam>
public class StatsDictionary<TValue> : SerializableDictionaryWithCaseInsensitiveKeys<TValue>
{
  /// <inheritdoc />
  protected override void AddDuringDeserialization(string key, TValue value)
  {
    TValue obj;
    if (!this.TryGetValue(key, out obj))
    {
      base.AddDuringDeserialization(key, value);
    }
    else
    {
      long int64_1 = Convert.ToInt64((object) value);
      long int64_2 = Convert.ToInt64((object) obj);
      if (key.EqualsIgnoreCase("averageBedtime"))
      {
        if (int64_2 != 0L)
          return;
        this[key] = value;
      }
      else
        this[key] = (TValue) Convert.ChangeType((object) (int64_2 + int64_1), typeof (TValue));
    }
  }
}
