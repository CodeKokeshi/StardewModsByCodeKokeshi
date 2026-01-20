// Decompiled with JetBrains decompiler
// Type: StardewValley.TokenizableStrings.TokenParserDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.TokenizableStrings;

/// <summary>Provides the output for a token within a text parsed by <see cref="T:StardewValley.TokenizableStrings.TokenParser" />.</summary>
/// <param name="query">The full token string split by spaces, including the token name.</param>
/// <param name="replacement">The output string with which to replace the token within the text being parsed.</param>
/// <param name="random">The RNG to use for randomization.</param>
/// <param name="player">The player to use for any player-related checks.</param>
/// <returns>Returns whether the text was handled.</returns>
public delegate bool TokenParserDelegate(
  string[] query,
  out string replacement,
  Random random,
  Farmer player);
