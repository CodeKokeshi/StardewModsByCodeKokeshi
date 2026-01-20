// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveSerialization.Vector2Writer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SaveSerialization;

public class Vector2Writer : XmlSerializationWriter
{
  public void WriteVector2(Vector2 vec)
  {
    XmlWriter writer = this.Writer;
    writer.WriteStartElement("Vector2");
    writer.WriteStartElement("X");
    writer.WriteValue(vec.X);
    writer.WriteEndElement();
    writer.WriteStartElement("Y");
    writer.WriteValue(vec.Y);
    writer.WriteEndElement();
    writer.WriteEndElement();
  }

  protected override void InitCallbacks()
  {
  }
}
