// Decompiled with JetBrains decompiler
// Type: StardewValley.Hashing.IHashUtility
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Hashing;

/// <summary>Combines hash codes in a deterministic way that's consistent between both sessions and players.</summary>
/// <remarks>This avoids <see cref="M:System.String.GetHashCode" /> and <c>HashCode.Combine</c> which are non-deterministic across sessions or players. That's preferable for actual hashing, but it prevents us from using it as deterministic random seeds.</remarks>
public interface IHashUtility
{
  /// <summary>Get a deterministic hash code for a string.</summary>
  /// <param name="value">The string value to hash.</param>
  int GetDeterministicHashCode(string value);

  /// <summary>Get a deterministic hash code for a set of values.</summary>
  /// <param name="values">The values to hash.</param>
  int GetDeterministicHashCode(params int[] values);
}
