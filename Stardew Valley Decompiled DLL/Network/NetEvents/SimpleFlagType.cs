// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetEvents.SimpleFlagType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Network.NetEvents;

/// <summary>A flag type which can be set for other players via <see cref="T:StardewValley.Network.NetEvents.SetSimpleFlagRequest" /> and <see cref="M:StardewValley.FarmerTeam.RequestSetSimpleFlag(StardewValley.Network.NetEvents.SimpleFlagType,StardewValley.Network.NetEvents.PlayerActionTarget,System.String,System.Boolean,System.Nullable{System.Int64})" /></summary>
public enum SimpleFlagType : byte
{
  /// <summary>An action ID applied for the player.</summary>
  ActionApplied,
  /// <summary>A cooking recipe learned by the player.</summary>
  CookingRecipeKnown,
  /// <summary>A cooking recipe learned by the player.</summary>
  CraftingRecipeKnown,
  /// <summary>A dialogue answer selected by the player.</summary>
  DialogueAnswerSelected,
  /// <summary>An event seen by the player.</summary>
  EventSeen,
  /// <summary>A quest within the player's quest log.</summary>
  HasQuest,
  /// <summary>A song track ID heard by the player.</summary>
  SongHeard,
}
