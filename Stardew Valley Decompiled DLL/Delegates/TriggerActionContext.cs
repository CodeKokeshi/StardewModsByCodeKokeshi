// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.TriggerActionContext
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.GameData;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Delegates;

/// <summary>The contextual values for a <see cref="T:StardewValley.Delegates.TriggerActionDelegate" />.</summary>
/// <summary>Construct an instance.</summary>
/// <param name="trigger">The trigger for which the action is being invoked, or <c>"Manual"</c> if it's not being invoked via <c>Data/TriggerActions</c>.</param>
/// <param name="triggerArgs">The contextual arguments provided with the trigger, or an empty array if none were provided. For example, an 'item received' trigger might provide the item instance and index.</param>
/// <param name="data">The entry data in <c>Data/TriggerActions</c> for the action being applied, or <c>null</c> if the action is being applied some other way (e.g. <c>$action</c> in dialogue).</param>
/// <param name="customFields">The custom fields which can be set by mods for custom trigger action behavior.</param>
public readonly struct TriggerActionContext(
  string trigger,
  object[] triggerArgs,
  TriggerActionData data,
  Dictionary<string, object> customFields = null)
{
  /// <summary>The trigger for which the action is being invoked, or <c>"Manual"</c> if it's not being invoked via <c>Data/TriggerActions</c>.</summary>
  public readonly string Trigger = trigger;
  /// <summary>The contextual arguments provided with the trigger, or an empty array if none were provided. For example, an 'item received' trigger might provide the item instance and index.</summary>
  public readonly object[] TriggerArgs = triggerArgs;
  /// <summary>The entry data in <c>Data/TriggerActions</c> for the action being applied, or <c>null</c> if the action is being applied some other way (e.g. <c>$action</c> in dialogue).</summary>
  public readonly TriggerActionData Data = data;
  /// <summary>The custom fields which can be set by mods for custom trigger action behavior, or <c>null</c> if none were set.</summary>
  public readonly Dictionary<string, object> CustomFields = customFields;
}
