// Decompiled with JetBrains decompiler
// Type: StardewValley.Triggers.CachedTriggerAction
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.GameData;

#nullable disable
namespace StardewValley.Triggers;

/// <summary>A cached, pre-parsed representation of a trigger action defined in <c>Data/TriggerActions</c>.</summary>
public class CachedTriggerAction
{
  /// <summary>The original trigger action data from <c>Data/TriggerActions</c>.</summary>
  public TriggerActionData Data { get; }

  /// <summary>The validated actions to invoke.</summary>
  public CachedAction[] Actions { get; }

  /// <summary>The validated space-delimited action strings.</summary>
  public string[] ActionStrings { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="data">The original trigger action data from <c>Data/TriggerActions</c>.</param>
  /// <param name="actions">The validated actions to invoke.</param>
  public CachedTriggerAction(TriggerActionData data, CachedAction[] actions)
  {
    this.Data = data;
    this.Actions = actions;
    if (actions.Length == 0)
    {
      this.ActionStrings = LegacyShims.EmptyArray<string>();
    }
    else
    {
      this.ActionStrings = new string[actions.Length];
      for (int index = 0; index < actions.Length; ++index)
        this.ActionStrings[index] = string.Join(" ", actions[index].Args);
    }
  }
}
