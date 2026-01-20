// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.GameStateQueryDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Delegates;

/// <summary>A <see cref="T:StardewValley.GameStateQuery" /> query resolver.</summary>
/// <param name="query">The game state query split by space, including the query key.</param>
/// <param name="context">The game state query context.</param>
/// <returns>Returns whether the query matches.</returns>
public delegate bool GameStateQueryDelegate(string[] query, GameStateQueryContext context);
