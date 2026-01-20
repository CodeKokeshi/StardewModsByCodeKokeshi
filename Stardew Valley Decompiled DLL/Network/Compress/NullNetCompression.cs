// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Compress.NullNetCompression
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace StardewValley.Network.Compress;

/// <summary>A no-op compression wrapper for platforms that provide no compression.</summary>
internal class NullNetCompression : INetCompression
{
  /// <inheritdoc />
  public byte[] CompressAbove(byte[] data, int minSizeToCompress = 256 /*0x0100*/) => data;

  /// <inheritdoc />
  public byte[] DecompressBytes(byte[] data) => data;

  /// <inheritdoc />
  public bool TryDecompressStream(Stream dataStream, out byte[] decompressed)
  {
    decompressed = (byte[]) null;
    return false;
  }
}
