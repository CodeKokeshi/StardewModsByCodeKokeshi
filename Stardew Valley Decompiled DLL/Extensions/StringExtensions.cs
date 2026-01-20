// Decompiled with JetBrains decompiler
// Type: StardewValley.Extensions.StringExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Extensions;

/// <summary>Provides utility extension methods on <see cref="T:System.String" /> values.</summary>
public static class StringExtensions
{
  /// <summary>Get whether a specified string occurs within this string, using ordinal (binary) sort rules and ignoring the capitalization of the strings being compared.</summary>
  /// <param name="str">The string within which to search for a match.</param>
  /// <param name="value">The value to find within the <paramref name="str" />.</param>
  public static bool ContainsIgnoreCase(this string str, string value)
  {
    return str != null && str.Contains(value, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Get whether a specified string is equal to this string, using ordinal (binary) sort rules and ignoring the capitalization of the strings being compared.</summary>
  /// <param name="str">The string within which to search for a match.</param>
  /// <param name="value">The value to find within the <paramref name="str" />.</param>
  public static bool EqualsIgnoreCase(this string str, string value)
  {
    return string.Equals(str, value, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Get the zero-based index of the first occurence of a specified value in this string, using ordinal (binary) sort rules and ignoring the capitalization of the strings being compared.</summary>
  /// <param name="str">The string within which to search for a match.</param>
  /// <param name="value">The value to find within the <paramref name="str" />.</param>
  /// <returns>Returns the index position of the <paramref name="value" /> within the <paramref name="str" />, or <c>-1</c> if not found.</returns>
  public static int IndexOfIgnoreCase(this string str, string value)
  {
    return str == null ? -1 : str.IndexOf(value, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Get whether this string starts with a specified string, using ordinal (binary) sort rules and ignoring the capitalization of the strings being compared.</summary>
  /// <param name="str">The string within which to search for a match.</param>
  /// <param name="value">The value to find within the <paramref name="str" />.</param>
  public static bool StartsWithIgnoreCase(this string str, string value)
  {
    return str != null && str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>Get whether this string ends with a specified string, using ordinal (binary) sort rules and ignoring the capitalization of the strings being compared.</summary>
  /// <param name="str">The string within which to search for a match.</param>
  /// <param name="value">The value to find within the <paramref name="str" />.</param>
  public static bool EndsWithIgnoreCase(this string str, string value)
  {
    return str != null && str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
  }
}
