// Decompiled with JetBrains decompiler
// Type: StardewValley.Audio.SoundContext
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Audio;

/// <summary>The source which triggered a game sound.</summary>
public enum SoundContext
{
  /// <summary>The default sound context.</summary>
  Default,
  /// <summary>Sounds produced by NPCs in the world, like a door sound when they path out of a house.</summary>
  NPC,
}
