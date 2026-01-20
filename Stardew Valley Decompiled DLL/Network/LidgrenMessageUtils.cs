// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.LidgrenMessageUtils
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Lidgren.Network;
using System.IO;

#nullable disable
namespace StardewValley.Network;

/// <summary>A set of utilities for packing/unpacking data within Lidgren messages.</summary>
public static class LidgrenMessageUtils
{
  /// <summary>Write (and potentially compress) the message from <paramref name="srcMsg" /> into <paramref name="destMsg" />.</summary>
  /// <param name="srcMsg">The outgoing message to read data from.</param>
  /// <param name="destMsg">The net outgoing message to write (and potentially compress) data into.</param>
  internal static void WriteMessage(OutgoingMessage srcMsg, NetOutgoingMessage destMsg)
  {
    byte[] array;
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
      {
        srcMsg.Write(writer);
        array = output.ToArray();
      }
    }
    using (MemoryStream memoryStream = new MemoryStream(Program.netCompression.CompressAbove(array, 1024 /*0x0400*/)))
    {
      using (NetBufferWriteStream destination = new NetBufferWriteStream((NetBuffer) destMsg))
        memoryStream.CopyTo((Stream) destination);
    }
  }

  /// <summary>Reads a message from <paramref name="stream" /> into <paramref name="msg" />, and decompresses it if necessary.</summary>
  /// <param name="stream">The stream to read message data from.</param>
  /// <param name="msg">The message to write (and potentially decompress) data into.</param>
  internal static void ReadStreamToMessage(NetBufferReadStream stream, IncomingMessage msg)
  {
    Stream input = (Stream) stream;
    byte[] decompressed;
    if (Program.netCompression.TryDecompressStream((Stream) stream, out decompressed))
      input = (Stream) new MemoryStream(decompressed);
    using (BinaryReader reader = new BinaryReader(input))
      msg.Read(reader);
  }
}
