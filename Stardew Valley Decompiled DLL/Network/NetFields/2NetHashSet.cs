// Decompiled with JetBrains decompiler
// Type: Netcode.NetIntHashSet
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetIntHashSet : NetHashSet<int>
{
  public override int ReadValue(BinaryReader reader) => reader.ReadInt32();

  public override void WriteValue(BinaryWriter writer, int value) => writer.Write(value);
}
