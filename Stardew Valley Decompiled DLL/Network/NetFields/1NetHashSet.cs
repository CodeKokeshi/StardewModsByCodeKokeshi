// Decompiled with JetBrains decompiler
// Type: Netcode.NetStringHashSet
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetStringHashSet : NetHashSet<string>
{
  public override string ReadValue(BinaryReader reader)
  {
    return !reader.ReadBoolean() ? (string) null : reader.ReadString();
  }

  public override void WriteValue(BinaryWriter writer, string value)
  {
    writer.Write(value != null);
    if (value == null)
      return;
    writer.Write(value);
  }
}
