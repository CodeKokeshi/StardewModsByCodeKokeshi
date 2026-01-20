// Decompiled with JetBrains decompiler
// Type: Netcode.NetEventBinary
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.IO;

#nullable disable
namespace Netcode;

public class NetEventBinary : AbstractNetEvent1<byte[]>
{
  public void Fire(NetEventBinary.ArgWriter argWriter)
  {
    byte[] buffer;
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
      {
        argWriter(writer);
        output.Position = 0L;
        buffer = new byte[output.Length];
        output.Read(buffer, 0, (int) output.Length);
      }
    }
    this.Fire(buffer);
  }

  public void AddReaderHandler(Action<BinaryReader> handler)
  {
    this.onEvent += (AbstractNetEvent1<byte[]>.Event) (bytes =>
    {
      using (MemoryStream input = new MemoryStream(bytes))
      {
        using (BinaryReader binaryReader = new BinaryReader((Stream) input))
          handler(binaryReader);
      }
    });
  }

  protected override byte[] readEventArg(BinaryReader reader, NetVersion version)
  {
    int count = reader.ReadInt32();
    return reader.ReadBytes(count);
  }

  protected override void writeEventArg(BinaryWriter writer, byte[] arg)
  {
    writer.Write(arg.Length);
    writer.Write(arg);
  }

  public delegate void ArgWriter(BinaryWriter writer);
}
