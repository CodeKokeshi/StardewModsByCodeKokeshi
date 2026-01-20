// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetEvents.SetSimpleFlagRequest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Extensions;
using System.IO;

#nullable disable
namespace StardewValley.Network.NetEvents;

/// <summary>A request to set or unset a simple flag for a group of players.</summary>
public sealed class SetSimpleFlagRequest : BaseSetFlagRequest
{
  /// <summary>The flag to set for the matching players.</summary>
  public SimpleFlagType FlagType { get; private set; }

  /// <inheritdoc />
  public SetSimpleFlagRequest()
  {
  }

  /// <inheritdoc cref="M:StardewValley.Network.NetEvents.BaseSetFlagRequest.#ctor(StardewValley.Network.NetEvents.PlayerActionTarget,System.String,System.Boolean,System.Nullable{System.Int64})" />
  /// <param name="flagType">The flag to set for the matching players.</param>
  /// <param name="onlyPlayerId">The specific player ID to apply this event to, or <c>null</c> to apply it to all players matching <paramref name="target" />.</param>
  public SetSimpleFlagRequest(
    SimpleFlagType flagType,
    PlayerActionTarget target,
    string flagId,
    bool flagState,
    long? onlyPlayerId)
    : base(target, flagId, flagState, onlyPlayerId)
  {
    this.FlagType = flagType;
  }

  /// <inheritdoc />
  public override void Read(BinaryReader reader)
  {
    base.Read(reader);
    this.FlagType = (SimpleFlagType) reader.ReadByte();
  }

  /// <inheritdoc />
  public override void Write(BinaryWriter writer)
  {
    base.Write(writer);
    writer.Write((byte) this.FlagType);
  }

  /// <inheritdoc />
  public override void PerformAction(Farmer farmer)
  {
    switch (this.FlagType)
    {
      case SimpleFlagType.ActionApplied:
        farmer.triggerActionsRun.Toggle<string>(this.FlagId, this.FlagState);
        break;
      case SimpleFlagType.CookingRecipeKnown:
        if (this.FlagState)
        {
          farmer.cookingRecipes.TryAdd(this.FlagId, 0);
          break;
        }
        farmer.cookingRecipes.Remove(this.FlagId);
        break;
      case SimpleFlagType.CraftingRecipeKnown:
        if (this.FlagState)
        {
          farmer.craftingRecipes.TryAdd(this.FlagId, 0);
          break;
        }
        farmer.craftingRecipes.Remove(this.FlagId);
        break;
      case SimpleFlagType.DialogueAnswerSelected:
        farmer.dialogueQuestionsAnswered.Toggle<string>(this.FlagId, this.FlagState);
        break;
      case SimpleFlagType.EventSeen:
        farmer.eventsSeen.Toggle<string>(this.FlagId, this.FlagState);
        break;
      case SimpleFlagType.HasQuest:
        if (this.FlagState)
        {
          farmer.addQuest(this.FlagId);
          break;
        }
        farmer.removeQuest(this.FlagId);
        break;
      case SimpleFlagType.SongHeard:
        farmer.songsHeard.Toggle<string>(this.FlagId, this.FlagState);
        break;
    }
  }
}
