// Decompiled with JetBrains decompiler
// Type: StardewValley.SerializableDictionaryWithCaseInsensitiveKeys`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

/// <summary>An implementation of <see cref="T:StardewValley.SerializableDictionary`2" /> that has case-insensitive keys.</summary>
/// <typeparam name="TValue">The value type.</typeparam>
/// <remarks>This avoids a limitation with <see cref="T:StardewValley.SerializableDictionary`2" /> where any custom comparer is lost on deserialization.</remarks>
public class SerializableDictionaryWithCaseInsensitiveKeys<TValue> : 
  SerializableDictionary<string, TValue>
{
  /// <summary>Construct an empty instance.</summary>
  public SerializableDictionaryWithCaseInsensitiveKeys()
    : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="data">The data to copy.</param>
  public SerializableDictionaryWithCaseInsensitiveKeys(IDictionary<string, TValue> data)
    : base(data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
  }
}
