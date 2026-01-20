// Decompiled with JetBrains decompiler
// Type: StardewValley.SerializableDictionary`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.SaveSerialization;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

/// <summary>A dictionary that can be read and written in the save XML.</summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
[XmlRoot("dictionary")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
  private static XmlSerializer _keySerializer = SaveSerializer.GetSerializer(typeof (TKey));
  private static XmlSerializer _valueSerializer = SaveSerializer.GetSerializer(typeof (TValue));

  /// <summary>Construct an empty instance.</summary>
  public SerializableDictionary()
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="data">The data to copy.</param>
  public SerializableDictionary(IDictionary<TKey, TValue> data)
    : base(data)
  {
  }

  /// <summary>Create an instance from a dictionary with a different value type.</summary>
  /// <typeparam name="TSourceValue">The value type in the source data to copy.</typeparam>
  /// <param name="data">The data to copy.</param>
  /// <param name="getValue">Get the value to use for an entry in the original data.</param>
  public static SerializableDictionary<TKey, TValue> BuildFrom<TSourceValue>(
    IDictionary<TKey, TSourceValue> data,
    Func<TSourceValue, TValue> getValue)
  {
    SerializableDictionary<TKey, TValue> serializableDictionary = new SerializableDictionary<TKey, TValue>();
    foreach (KeyValuePair<TKey, TSourceValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TSourceValue>>) data)
      serializableDictionary[keyValuePair.Key] = getValue(keyValuePair.Value);
    return serializableDictionary;
  }

  /// <summary>Create an instance from a dictionary with different key and value types.</summary>
  /// <typeparam name="TSourceKey">The key type in the source data to copy.</typeparam>
  /// <typeparam name="TSourceValue">The value type in the source data to copy.</typeparam>
  /// <param name="data">The data to copy.</param>
  /// <param name="getKey">Get the key to use for an entry in the original data.</param>
  /// <param name="getValue">Get the value to use for an entry in the original data.</param>
  public static SerializableDictionary<TKey, TValue> BuildFrom<TSourceKey, TSourceValue>(
    IDictionary<TSourceKey, TSourceValue> data,
    Func<TSourceKey, TKey> getKey,
    Func<TSourceValue, TValue> getValue)
  {
    SerializableDictionary<TKey, TValue> serializableDictionary = new SerializableDictionary<TKey, TValue>();
    foreach (KeyValuePair<TSourceKey, TSourceValue> keyValuePair in (IEnumerable<KeyValuePair<TSourceKey, TSourceValue>>) data)
      serializableDictionary[getKey(keyValuePair.Key)] = getValue(keyValuePair.Value);
    return serializableDictionary;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="comparer">The equality comparer to use when comparing keys, or null to use the default comparer for the key type.</param>
  protected SerializableDictionary(IEqualityComparer<TKey> comparer = null)
    : base(comparer)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="data">The data to copy.</param>
  /// <param name="comparer">The equality comparer to use when comparing keys, or null to use the default comparer for the key type.</param>
  protected SerializableDictionary(IDictionary<TKey, TValue> data, IEqualityComparer<TKey> comparer = null)
    : base(data, comparer)
  {
  }

  public new void Add(TKey key, TValue value)
  {
    base.Add(key, value);
    this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Add, key, value));
  }

  public new bool Remove(TKey key)
  {
    TValue v;
    if (!this.TryGetValue(key, out v))
      return false;
    base.Remove(key);
    this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Remove, key, v));
    return true;
  }

  public new void Clear()
  {
    base.Clear();
    this.OnCollectionChanged((object) this, new SerializableDictionary<TKey, TValue>.ChangeArgs(ChangeType.Clear, default (TKey), default (TValue)));
  }

  public event SerializableDictionary<TKey, TValue>.ChangeCallback CollectionChanged;

  private void OnCollectionChanged(
    object sender,
    SerializableDictionary<TKey, TValue>.ChangeArgs args)
  {
    SerializableDictionary<TKey, TValue>.ChangeCallback collectionChanged = this.CollectionChanged;
    if (collectionChanged == null)
      return;
    collectionChanged(sender ?? (object) this, args);
  }

  public XmlSchema GetSchema() => (XmlSchema) null;

  public void ReadXml(XmlReader reader)
  {
    int num = reader.IsEmptyElement ? 1 : 0;
    reader.Read();
    if (num != 0)
      return;
    while (reader.NodeType != XmlNodeType.EndElement)
    {
      reader.ReadStartElement("item");
      reader.ReadStartElement("key");
      bool flag1 = false;
      TKey key = default (TKey);
      if (typeof (TKey) == typeof (string))
      {
        switch (reader.Name)
        {
          case "int":
            key = (TKey) Convert.ChangeType((object) SaveSerializer.Deserialize<int>(reader), typeof (TKey));
            flag1 = true;
            break;
          case "LocationContext":
            reader.ReadStartElement();
            key = (TKey) Convert.ChangeType((object) reader.ReadContentAsString(), typeof (TKey));
            reader.ReadEndElement();
            flag1 = true;
            break;
        }
      }
      if (!flag1)
        key = (TKey) SerializableDictionary<TKey, TValue>._keySerializer.DeserializeFast(reader);
      reader.ReadEndElement();
      reader.ReadStartElement("value");
      TValue obj = default (TValue);
      bool flag2 = false;
      if (typeof (TValue) == typeof (string) && reader.Name == "int")
      {
        obj = (TValue) Convert.ChangeType((object) SaveSerializer.Deserialize<int>(reader), typeof (TValue));
        flag2 = true;
      }
      if (!flag2)
        obj = (TValue) SerializableDictionary<TKey, TValue>._valueSerializer.DeserializeFast(reader);
      reader.ReadEndElement();
      this.AddDuringDeserialization(key, obj);
      reader.ReadEndElement();
      int content = (int) reader.MoveToContent();
    }
    reader.ReadEndElement();
  }

  public void WriteXml(XmlWriter writer)
  {
    foreach (TKey key in this.Keys)
    {
      writer.WriteStartElement("item");
      writer.WriteStartElement("key");
      SerializableDictionary<TKey, TValue>._keySerializer.SerializeFast(writer, (object) key);
      writer.WriteEndElement();
      writer.WriteStartElement("value");
      TValue obj = this[key];
      SerializableDictionary<TKey, TValue>._valueSerializer.SerializeFast(writer, (object) obj);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
  }

  /// <summary>Add a pair read from the raw XML during deserialization.</summary>
  /// <param name="key">The key to add.</param>
  /// <param name="value">The value to add.</param>
  protected virtual void AddDuringDeserialization(TKey key, TValue value) => base.Add(key, value);

  public struct ChangeArgs(ChangeType type, TKey k, TValue v)
  {
    public readonly ChangeType Type = type;
    public readonly TKey Key = k;
    public readonly TValue Value = v;
  }

  public delegate void ChangeCallback(
    object sender,
    SerializableDictionary<TKey, TValue>.ChangeArgs args);
}
