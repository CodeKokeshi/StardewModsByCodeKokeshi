// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Compress.LZ4NetCompression
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using LWJGL;
using System;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace StardewValley.Network.Compress;

/// <summary>Handles compression and decompression of network messages using LZ4 to reduce network traffic.</summary>
internal class LZ4NetCompression : INetCompression
{
  /// <summary>The size of the header used for compressed messages.</summary>
  private const int HeaderSize = 9;

  /// <inheritdoc />
  public byte[] CompressAbove(byte[] data, int minSizeToCompress = 256 /*0x0100*/)
  {
    if (data.Length < minSizeToCompress)
      return data;
    int dstCapacity = LZ4.CompressBound(data.Length);
    IntPtr num = Marshal.AllocHGlobal(dstCapacity + 9);
    IntPtr dest = IntPtr.Add(num, 9);
    int val = LZ4.CompressDefault(data, dest, data.Length, dstCapacity);
    Marshal.WriteByte(num, 0, (byte) 127 /*0x7F*/);
    Marshal.WriteInt32(num, 1, val);
    Marshal.WriteInt32(num, 5, data.Length);
    byte[] destination = new byte[val + 9];
    Marshal.Copy(num, destination, 0, destination.Length);
    Marshal.FreeHGlobal(num);
    return destination;
  }

  /// <inheritdoc />
  public byte[] DecompressBytes(byte[] data)
  {
    return data[0] != (byte) 127 /*0x7F*/ ? data : this.DecompressImpl(data);
  }

  /// <inheritdoc />
  /// <exception cref="T:System.ArgumentException">The stream doesn't support both reading and seeking.</exception>
  public bool TryDecompressStream(Stream dataStream, out byte[] decompressed)
  {
    decompressed = (byte[]) null;
    if (!dataStream.CanSeek || !dataStream.CanRead)
      throw new ArgumentException("dataStream must support both reading and seeking");
    long position = dataStream.Position;
    if ((byte) dataStream.ReadByte() != (byte) 127 /*0x7F*/)
    {
      dataStream.Seek(position, SeekOrigin.Begin);
      return false;
    }
    byte[] buffer = new byte[4];
    dataStream.Read(buffer, 0, 4);
    int int32 = BitConverter.ToInt32(buffer, 0);
    byte[] numArray = new byte[int32 + 9];
    dataStream.Read(numArray, 5, 4 + int32);
    decompressed = this.DecompressImpl(numArray);
    return true;
  }

  /// <summary>Decompress raw data without checking whether it's compressed.</summary>
  /// <param name="data">The compressed data.</param>
  /// <returns>Returns the data decompressed from <paramref name="data" />.</returns>
  private unsafe byte[] DecompressImpl(byte[] data)
  {
    int int32 = BitConverter.ToInt32(data, 5);
    byte[] dest = new byte[int32];
    fixed (byte* pointer = data)
      LZ4.DecompressSafe(IntPtr.Add((IntPtr) (void*) pointer, 9), dest, data.Length - 9, int32);
    return dest;
  }
}
