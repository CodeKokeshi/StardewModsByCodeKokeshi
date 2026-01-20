// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveSerialization.Vector2Serializer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SaveSerialization;

public class Vector2Serializer : XmlSerializer
{
  private Vector2Reader _reader = new Vector2Reader();
  private Vector2Writer _writer = new Vector2Writer();

  public Vector2Serializer()
    : base(typeof (Vector2))
  {
  }

  protected override XmlSerializationReader CreateReader() => (XmlSerializationReader) this._reader;

  protected override XmlSerializationWriter CreateWriter() => (XmlSerializationWriter) this._writer;

  public override bool CanDeserialize(XmlReader xmlReader) => xmlReader.IsStartElement("Vector2");

  protected override void Serialize(object o, XmlSerializationWriter writer)
  {
    this._writer.WriteVector2((Vector2) o);
  }

  protected override object Deserialize(XmlSerializationReader reader)
  {
    return (object) this._reader.ReadVector2();
  }
}
