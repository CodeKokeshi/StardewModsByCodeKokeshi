// Decompiled with JetBrains decompiler
// Type: StardewValley.Tests.ExtractSyntaxDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Tests;

/// <summary>Get a syntactic representation from a specific asset type for <see cref="T:StardewValley.Tests.SyntaxAbstractor" />.</summary>
/// <param name="syntaxAbstractor">The syntax abstractor instance to use.</param>
/// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
/// <param name="key">The entry key.</param>
/// <param name="text">The entry value to parse.</param>
public delegate string ExtractSyntaxDelegate(
  SyntaxAbstractor syntaxAbstractor,
  string baseAssetName,
  string key,
  string text);
