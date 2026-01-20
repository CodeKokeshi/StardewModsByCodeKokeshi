// Decompiled with JetBrains decompiler
// Type: Netcode.NetVector2HashSet
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace Netcode;

public class NetVector2HashSet : NetHashSet<Vector2>
{
  public override Vector2 ReadValue(BinaryReader reader)
  {
    return new Vector2(reader.ReadSingle(), reader.ReadSingle());
  }

  public override void WriteValue(BinaryWriter writer, Vector2 value)
  {
    writer.Write(value.X);
    writer.Write(value.Y);
  }
}
