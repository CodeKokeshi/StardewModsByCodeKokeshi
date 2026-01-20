// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.IndoorsType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Buildings;

/// <summary>The type of indoors location a building has.</summary>
public enum IndoorsType
{
  /// <summary>The building doesn't have an indoors location.</summary>
  None,
  /// <summary>The building has a unique interior location that was created for this building, which isn't in <see cref="P:StardewValley.Game1.locations" /> separately.</summary>
  Instanced,
  /// <summary>The building links to a global location like <c>FarmHouse</c> for its interior, which is in <see cref="P:StardewValley.Game1.locations" /> separately.</summary>
  Global,
}
