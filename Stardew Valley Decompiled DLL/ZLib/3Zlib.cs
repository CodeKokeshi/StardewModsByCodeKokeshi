// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.CompressionMode
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace Ionic.Zlib;

/// <summary>
/// An enum to specify the direction of transcoding - whether to compress or decompress.
/// </summary>
public enum CompressionMode
{
  /// <summary>
  /// Used to specify that the stream should compress the data.
  /// </summary>
  Compress,
  /// <summary>
  /// Used to specify that the stream should decompress the data.
  /// </summary>
  Decompress,
}
