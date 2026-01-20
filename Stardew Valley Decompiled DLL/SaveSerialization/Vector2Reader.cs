// Decompiled with JetBrains decompiler
// Type: StardewValley.SaveSerialization.Vector2Reader
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.SaveSerialization;

public class Vector2Reader : XmlSerializationReader
{
  public Vector2 ReadVector2()
  {
    XmlReader reader = this.Reader;
    reader.ReadStartElement("Vector2");
    reader.ReadStartElement("X");
    float x = reader.ReadContentAsFloat();
    reader.ReadEndElement();
    reader.ReadStartElement("Y");
    float y = reader.ReadContentAsFloat();
    reader.ReadEndElement();
    reader.ReadEndElement();
    return new Vector2(x, y);
  }

  protected override void InitCallbacks()
  {
  }

  protected override void InitIDs()
  {
  }
}
