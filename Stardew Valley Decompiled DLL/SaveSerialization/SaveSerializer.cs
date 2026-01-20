// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveSerialization.SaveSerializer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Quests;
using StardewValley.SaveMigrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SaveSerialization;

public static class SaveSerializer
{
  private static readonly Dictionary<Type, XmlSerializer> _serializerLookup = new Dictionary<Type, XmlSerializer>();

  public static XmlSerializer GetSerializer(Type type)
  {
    XmlSerializer serializer;
    if (!SaveSerializer._serializerLookup.TryGetValue(type, out serializer))
    {
      if (type == typeof (SaveGame))
        return SaveGame.serializer;
      if (type == typeof (Farmer))
        return SaveGame.farmerSerializer;
      if (type == typeof (GameLocation))
        return SaveGame.locationSerializer;
      if (type == typeof (DescriptionElement))
        return SaveGame.descriptionElementSerializer;
      if (type == typeof (SaveMigrator_1_6.LegacyDescriptionElement))
        return SaveGame.legacyDescriptionElementSerializer;
      serializer = new XmlSerializer(type);
      SaveSerializer._serializerLookup.Add(type, serializer);
    }
    return serializer;
  }

  public static void SerializeFast(this XmlSerializer serializer, Stream stream, object obj)
  {
    serializer.Serialize(stream, obj);
  }

  public static void Serialize<T>(XmlWriter xmlWriter, T obj)
  {
    SaveSerializer.GetSerializer(typeof (T)).SerializeFast(xmlWriter, (object) obj);
  }

  public static void SerializeFast(this XmlSerializer serializer, XmlWriter xmlWriter, object obj)
  {
    serializer.Serialize(xmlWriter, obj);
  }

  public static T Deserialize<T>(Stream stream)
  {
    return (T) SaveSerializer.GetSerializer(typeof (T)).DeserializeFast(stream);
  }

  public static T Deserialize<T>(XmlReader reader)
  {
    return (T) SaveSerializer.GetSerializer(typeof (T)).DeserializeFast(reader);
  }

  public static object DeserializeFast(this XmlSerializer serializer, Stream stream)
  {
    return serializer.Deserialize(stream);
  }

  public static object DeserializeFast(this XmlSerializer serializer, XmlReader reader)
  {
    return serializer.Deserialize(reader);
  }
}
