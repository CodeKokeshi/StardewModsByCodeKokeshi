// Decompiled with JetBrains decompiler
// Type: StardewValley.Hashing.HashUtility
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Data.HashFunction;
using System.Text;

#nullable disable
namespace StardewValley.Hashing;

/// <inheritdoc cref="T:StardewValley.Hashing.IHashUtility" />
public class HashUtility : IHashUtility
{
  /// <summary>The underlying hashing API.</summary>
  private static readonly IHashFunction Hasher = (IHashFunction) new xxHash(32 /*0x20*/);

  /// <inheritdoc />
  public int GetDeterministicHashCode(string value)
  {
    return this.GetDeterministicHashCode(Encoding.UTF8.GetBytes(value));
  }

  /// <inheritdoc />
  public int GetDeterministicHashCode(params int[] values)
  {
    byte[] numArray = new byte[values.Length * 4];
    Buffer.BlockCopy((Array) values, 0, (Array) numArray, 0, numArray.Length);
    return this.GetDeterministicHashCode(numArray);
  }

  /// <summary>Get a deterministic hash code for a byte data array.</summary>
  /// <param name="data">The data to hash.</param>
  public int GetDeterministicHashCode(byte[] data)
  {
    return BitConverter.ToInt32(HashUtility.Hasher.ComputeHash(data), 0);
  }
}
