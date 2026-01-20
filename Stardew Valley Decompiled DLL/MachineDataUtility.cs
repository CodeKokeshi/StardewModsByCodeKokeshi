// Decompiled with JetBrains decompiler
// Type: StardewValley.MachineDataUtility
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Machines;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

/// <summary>Handles common logic for parsing and applying the data in <c>Data/Machines</c>.</summary>
/// <remarks>For more specific logic, see the logic in <see cref="T:StardewValley.Object" /> like <see cref="M:StardewValley.Object.PlaceInMachine(StardewValley.GameData.Machines.MachineData,StardewValley.Item,System.Boolean,StardewValley.Farmer,System.Boolean,System.Boolean)" />.</remarks>
public static class MachineDataUtility
{
  /// <summary>The token placeholders which can appear in an <see cref="P:StardewValley.GameData.ISpawnItemData.ItemId" /> field, and the methods which return their value.</summary>
  public static readonly IDictionary<string, MachineDataUtility.GetOutputTokenValueDelegate> OutputTokens = (IDictionary<string, MachineDataUtility.GetOutputTokenValueDelegate>) new Dictionary<string, MachineDataUtility.GetOutputTokenValueDelegate>()
  {
    ["DROP_IN_ID"] = new MachineDataUtility.GetOutputTokenValueDelegate(MachineDataUtility.GetTokenValue),
    ["DROP_IN_PRESERVE"] = new MachineDataUtility.GetOutputTokenValueDelegate(MachineDataUtility.GetTokenValue),
    ["NEARBY_FLOWER_ID"] = new MachineDataUtility.GetOutputTokenValueDelegate(MachineDataUtility.GetTokenValue),
    ["DROP_IN_QUALITY"] = new MachineDataUtility.GetOutputTokenValueDelegate(MachineDataUtility.GetTokenValue)
  };

  /// <summary>Get whether the inventory contains the additional items needed to run the machine.</summary>
  /// <param name="inventory">The inventory to search for matching items.</param>
  /// <param name="requirements">The additional required items, if any.</param>
  /// <param name="failedRequirement">The requirement which isn't met, if applicable.</param>
  public static bool HasAdditionalRequirements(
    IInventory inventory,
    IList<MachineItemAdditionalConsumedItems> requirements,
    out MachineItemAdditionalConsumedItems failedRequirement)
  {
    if (requirements != null && requirements.Count > 0)
    {
      foreach (MachineItemAdditionalConsumedItems requirement in (IEnumerable<MachineItemAdditionalConsumedItems>) requirements)
      {
        if (inventory.CountId(requirement.ItemId) < requirement.RequiredCount)
        {
          failedRequirement = requirement;
          return false;
        }
      }
    }
    failedRequirement = (MachineItemAdditionalConsumedItems) null;
    return true;
  }

  /// <summary>Get whether an output rule matches the given item.</summary>
  /// <param name="machine">The machine instance.</param>
  /// <param name="rule">The machine output rule.</param>
  /// <param name="trigger">The rule trigger type to match.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  /// <param name="location">The location containing the machine.</param>
  /// <param name="triggerRule">The output rule trigger that matched, if applicable.</param>
  /// <param name="matchesExceptCount">Whether the output can be applied if <see cref="P:StardewValley.GameData.Machines.MachineOutputTriggerRule.RequiredCount" /> is ignored.</param>
  public static bool CanApplyOutput(
    Object machine,
    MachineOutputRule rule,
    MachineOutputTrigger trigger,
    Item inputItem,
    Farmer who,
    GameLocation location,
    out MachineOutputTriggerRule triggerRule,
    out bool matchesExceptCount)
  {
    matchesExceptCount = false;
    triggerRule = (MachineOutputTriggerRule) null;
    if (rule.Triggers == null)
      return false;
    foreach (MachineOutputTriggerRule trigger1 in rule.Triggers)
    {
      if (trigger1.Trigger.HasFlag((Enum) trigger) && (trigger1.Condition == null || GameStateQuery.CheckConditions(trigger1.Condition, location, who, inputItem: inputItem)))
      {
        if (trigger.HasFlag((Enum) MachineOutputTrigger.ItemPlacedInMachine) || trigger.HasFlag((Enum) MachineOutputTrigger.OutputCollected))
        {
          if (trigger1.RequiredItemId == null || ItemRegistry.HasItemId(inputItem, trigger1.RequiredItemId))
          {
            List<string> requiredTags = trigger1.RequiredTags;
            // ISSUE: explicit non-virtual call
            if ((requiredTags != null ? (__nonvirtual (requiredTags.Count) > 0 ? 1 : 0) : 0) == 0 || ItemContextTagManager.DoAllTagsMatch((IList<string>) trigger1.RequiredTags, inputItem.GetContextTags()))
            {
              if (trigger1.RequiredCount > inputItem.Stack)
              {
                triggerRule = trigger1;
                matchesExceptCount = true;
                continue;
              }
            }
            else
              continue;
          }
          else
            continue;
        }
        triggerRule = trigger1;
        matchesExceptCount = false;
        return true;
      }
    }
    return false;
  }

  /// <summary>Get the first output rule which matches the given item, if any.</summary>
  /// <param name="machine">The machine instance.</param>
  /// <param name="machineData">The machine data from which to get an output rule.</param>
  /// <param name="trigger">The rule trigger type to match.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  /// <param name="location">The location containing the machine.</param>
  /// <param name="rule">The output rule found, if applicable.</param>
  /// <param name="triggerRule">The output rule trigger that matched, if applicable.</param>
  /// <param name="ruleIgnoringCount">If no output rule was found, the output rule that would have matched if we ignore the <see cref="P:StardewValley.GameData.Machines.MachineOutputTriggerRule.RequiredCount" /> field. If there are multiple such rules, the first one with a <see cref="F:StardewValley.GameData.Machines.MachineOutputRule.InvalidCountMessage" /> set is selected, else the first one in the list.</param>
  /// <param name="triggerIgnoringCount">The output rule trigger that matched for <paramref name="ruleIgnoringCount" />, if applicable.</param>
  public static bool TryGetMachineOutputRule(
    Object machine,
    MachineData machineData,
    MachineOutputTrigger trigger,
    Item inputItem,
    Farmer who,
    GameLocation location,
    out MachineOutputRule rule,
    out MachineOutputTriggerRule triggerRule,
    out MachineOutputRule ruleIgnoringCount,
    out MachineOutputTriggerRule triggerIgnoringCount)
  {
    rule = (MachineOutputRule) null;
    triggerRule = (MachineOutputTriggerRule) null;
    ruleIgnoringCount = (MachineOutputRule) null;
    triggerIgnoringCount = (MachineOutputTriggerRule) null;
    if (machineData?.OutputRules == null)
      return false;
    foreach (MachineOutputRule outputRule in machineData.OutputRules)
    {
      bool matchesExceptCount;
      if (MachineDataUtility.CanApplyOutput(machine, outputRule, trigger, inputItem, who, location, out triggerRule, out matchesExceptCount))
      {
        rule = outputRule;
        return true;
      }
      if (matchesExceptCount && (ruleIgnoringCount == null || ruleIgnoringCount.InvalidCountMessage == null && outputRule.InvalidCountMessage != null))
      {
        ruleIgnoringCount = outputRule;
        triggerIgnoringCount = triggerRule;
      }
    }
    return false;
  }

  /// <summary>Get the output item data which matches the given item, if any.</summary>
  /// <param name="machine">The machine instance.</param>
  /// <param name="machineData">The machine data from which to get the output data.</param>
  /// <param name="outputRule">The output rule from which to get the output data, or <c>null</c> to get a matching rule from the machine data.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  /// <param name="location">The location containing the machine.</param>
  public static MachineItemOutput GetOutputData(
    Object machine,
    MachineData machineData,
    MachineOutputRule outputRule,
    Item inputItem,
    Farmer who,
    GameLocation location)
  {
    return outputRule == null && !MachineDataUtility.TryGetMachineOutputRule(machine, machineData, MachineOutputTrigger.ItemPlacedInMachine, inputItem, who, location, out outputRule, out MachineOutputTriggerRule _, out MachineOutputRule _, out MachineOutputTriggerRule _) ? (MachineItemOutput) null : MachineDataUtility.GetOutputData(outputRule.OutputItem, outputRule.UseFirstValidOutput, inputItem, who, location);
  }

  /// <summary>Get the output item data which matches the given item, if any.</summary>
  /// <param name="outputs">The output entries to choose from.</param>
  /// <param name="useFirstValidOutput">Whether to return the first matching output; else a valid one will be chosen at random.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  /// <param name="location">The location containing the machine.</param>
  public static MachineItemOutput GetOutputData(
    List<MachineItemOutput> outputs,
    bool useFirstValidOutput,
    Item inputItem,
    Farmer who,
    GameLocation location)
  {
    if (outputs == null || outputs.Count <= 0)
      return (MachineItemOutput) null;
    List<MachineItemOutput> options = !useFirstValidOutput ? new List<MachineItemOutput>() : (List<MachineItemOutput>) null;
    foreach (MachineItemOutput output in outputs)
    {
      if (GameStateQuery.CheckConditions(output.Condition, location, who, inputItem: inputItem))
      {
        if (useFirstValidOutput)
          return output;
        options.Add(output);
      }
    }
    return useFirstValidOutput ? (MachineItemOutput) null : Game1.random.ChooseFrom<MachineItemOutput>((IList<MachineItemOutput>) options);
  }

  /// <summary>Get the item to produce for a given output data.</summary>
  /// <param name="machine">The machine which will produce output.</param>
  /// <param name="outputData">The machine output data.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  /// <param name="probe">Whether the machine is only checking the output that would be produced. If so, the input/machine shouldn't be changed and no animations/sounds should play.</param>
  /// <param name="overrideMinutesUntilReady">The in-game minutes until the item will be ready to collect, if set. This overrides the equivalent fields in the machine data if set.</param>
  public static Item GetOutputItem(
    Object machine,
    MachineItemOutput outputData,
    Item inputItem,
    Farmer who,
    bool probe,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (outputData == null)
      return (Item) null;
    ItemQueryContext context = new ItemQueryContext(machine.Location, who, Game1.random, $"machine '{machine.QualifiedItemId}' > output rules");
    Item outputItem;
    if (outputData.OutputMethod != null)
    {
      MachineOutputDelegate createdDelegate;
      string error;
      if (!StaticDelegateBuilder.TryCreateDelegate<MachineOutputDelegate>(outputData.OutputMethod, out createdDelegate, out error))
      {
        Game1.log.Warn($"Machine {machine.QualifiedItemId} has invalid item output method '{outputData.OutputMethod}': {error}");
        return (Item) null;
      }
      outputItem = (Item) ItemQueryResolver.ApplyItemFields((ISalable) createdDelegate(machine, inputItem, probe, outputData, who, out overrideMinutesUntilReady), (ISpawnItemData) outputData, context, inputItem);
    }
    else
      outputItem = !(outputData.ItemId == "DROP_IN") ? ItemQueryResolver.TryResolveRandomItem((ISpawnItemData) outputData, context, formatItemId: (Func<string, string>) (id => MachineDataUtility.FormatOutputId(id, machine, outputData, inputItem, who)), inputItem: inputItem, logError: (Action<string, string>) ((query, error) => Game1.log.Error($"Machine '{machine.QualifiedItemId}' failed parsing item query '{query}' for output '{outputData.Id}': {error}."))) : (Item) ItemQueryResolver.ApplyItemFields((ISalable) inputItem?.getOne(), (ISpawnItemData) outputData, context, inputItem);
    if (outputItem == null)
      return (Item) null;
    if (outputData.CopyColor)
    {
      Color? nullable = inputItem is ColoredObject coloredObject ? new Color?(coloredObject.color.Value) : ItemContextTagManager.GetColorFromTags(inputItem);
      ColoredObject coloredItem;
      if (nullable.HasValue && ColoredObject.TrySetColor(outputItem, nullable.Value, out coloredItem))
        outputItem = (Item) coloredItem;
    }
    if (outputData.CopyQuality && inputItem != null)
    {
      outputItem.Quality = inputItem.Quality;
      List<QuantityModifier> qualityModifiers = outputData.QualityModifiers;
      // ISSUE: explicit non-virtual call
      if ((qualityModifiers != null ? (__nonvirtual (qualityModifiers.Count) > 0 ? 1 : 0) : 0) != 0)
        outputItem.Quality = (int) Utility.ApplyQuantityModifiers((float) outputItem.Quality, (IList<QuantityModifier>) outputData.QualityModifiers, outputData.QualityModifierMode, machine.Location, who, outputItem, inputItem);
    }
    if (outputItem is Object object1 && outputData.ObjectInternalName != null)
    {
      string str = string.Format(outputData.ObjectInternalName, (object) (inputItem?.Name ?? ""));
      object1.Name = str;
    }
    if (outputItem is Object object3)
    {
      Object object2 = inputItem as Object;
      if (outputData.CopyPrice && object2 != null)
        object3.Price = object2.Price;
      List<QuantityModifier> priceModifiers = outputData.PriceModifiers;
      // ISSUE: explicit non-virtual call
      if ((priceModifiers != null ? (__nonvirtual (priceModifiers.Count) > 0 ? 1 : 0) : 0) != 0)
        object3.Price = (int) Utility.ApplyQuantityModifiers((float) object3.Price, (IList<QuantityModifier>) outputData.PriceModifiers, outputData.PriceModifierMode, machine.Location, who, outputItem, inputItem);
      if (!string.IsNullOrWhiteSpace(outputData.PreserveType))
        object3.preserve.Value = new Object.PreserveType?((Object.PreserveType) Enum.Parse(typeof (Object.PreserveType), outputData.PreserveType));
      if (!string.IsNullOrWhiteSpace(outputData.PreserveId))
      {
        switch (outputData.PreserveId)
        {
          case "DROP_IN":
            object3.preservedParentSheetIndex.Value = inputItem?.ItemId;
            break;
          case "DROP_IN_PRESERVE":
            object3.preservedParentSheetIndex.Value = object2?.GetPreservedItemId();
            break;
          default:
            object3.preservedParentSheetIndex.Value = outputData.PreserveId;
            break;
        }
      }
    }
    return outputItem;
  }

  /// <summary>Increment stats when an item is placed in the machine, if applicable.</summary>
  /// <param name="stats">The stats data to apply.</param>
  /// <param name="item">The item that was placed in the machine.</param>
  /// <param name="amount">The number of items that were placed in the machine.</param>
  public static void UpdateStats(List<StatIncrement> stats, Item item, int amount)
  {
    if (stats == null)
      return;
    foreach (StatIncrement stat in stats)
    {
      if (stat.RequiredItemId == null || ItemRegistry.HasItemId(item, stat.RequiredItemId))
      {
        List<string> requiredTags = stat.RequiredTags;
        // ISSUE: explicit non-virtual call
        if ((requiredTags != null ? (__nonvirtual (requiredTags.Count) > 0 ? 1 : 0) : 0) == 0 || ItemContextTagManager.DoAllTagsMatch((IList<string>) stat.RequiredTags, item.GetContextTags()))
        {
          int num = (int) Game1.stats.Increment(stat.StatName, amount);
        }
      }
    }
  }

  /// <summary>Apply a machine effect, if it's valid and its fields match.</summary>
  /// <param name="machine">The machine for which to apply effects.</param>
  /// <param name="effect">The machine effect to apply.</param>
  /// <param name="playSounds">Whether to play sounds when the item is placed.</param>
  public static bool PlayEffects(Object machine, MachineEffects effect, bool playSounds = true)
  {
    if (effect == null)
      return false;
    string condition1 = effect.Condition;
    GameLocation location1 = machine.Location;
    Item obj1 = machine.lastInputItem.Value;
    Object targetItem1 = machine.heldObject.Value;
    Item inputItem1 = obj1;
    if (!GameStateQuery.CheckConditions(condition1, location1, targetItem: (Item) targetItem1, inputItem: inputItem1))
      return false;
    if (playSounds)
    {
      List<MachineSoundData> sounds = effect.Sounds;
      // ISSUE: explicit non-virtual call
      if ((sounds != null ? (__nonvirtual (sounds.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (MachineSoundData sound in effect.Sounds)
        {
          if (sound.Delay <= 0)
            machine.Location.playSound(sound.Id, new Vector2?(machine.TileLocation));
          else
            DelayedAction.playSoundAfterDelay(sound.Id, sound.Delay, machine.Location, new Vector2?(machine.TileLocation));
        }
      }
    }
    if (effect.ShakeDuration >= 0)
      machine.shakeTimer = effect.ShakeDuration;
    if (effect.TemporarySprites != null)
    {
      foreach (TemporaryAnimatedSpriteDefinition temporarySprite in effect.TemporarySprites)
      {
        string condition2 = temporarySprite.Condition;
        GameLocation location2 = machine.Location;
        Item obj2 = machine.lastInputItem.Value;
        Object targetItem2 = machine.heldObject.Value;
        Item inputItem2 = obj2;
        if (GameStateQuery.CheckConditions(condition2, location2, targetItem: (Item) targetItem2, inputItem: inputItem2))
        {
          TemporaryAnimatedSprite fromData = TemporaryAnimatedSprite.CreateFromData(temporarySprite, machine.tileLocation.X, machine.tileLocation.Y, (float) (((double) machine.tileLocation.Y + 1.0) * 64.0 / 10000.0));
          Game1.multiplayer.broadcastSprites(machine.Location, fromData);
        }
      }
    }
    return true;
  }

  /// <summary>Replace machine placeholder tokens for an <see cref="P:StardewValley.GameData.ISpawnItemData.ItemId" /> field.</summary>
  /// <param name="id">The <see cref="P:StardewValley.GameData.ISpawnItemData.ItemId" /> value.</param>
  /// <param name="machine">The machine producing the output.</param>
  /// <param name="outputData">The machine output data.</param>
  /// <param name="inputItem">The item dropped into the machine, if any.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  public static string FormatOutputId(
    string id,
    Object machine,
    MachineItemOutput outputData,
    Item inputItem,
    Farmer who)
  {
    if (string.IsNullOrWhiteSpace(id))
      return id;
    bool flag = false;
    string[] strArray = ArgUtility.SplitBySpace(id);
    for (int index = 0; index < strArray.Length; ++index)
    {
      MachineDataUtility.GetOutputTokenValueDelegate tokenValueDelegate;
      if (MachineDataUtility.OutputTokens.TryGetValue(strArray[index], out tokenValueDelegate))
      {
        string str = strArray[index];
        strArray[index] = tokenValueDelegate(strArray[index], machine, outputData, inputItem, who);
        flag = flag || strArray[index] != str;
      }
    }
    return !flag ? id : string.Join(" ", strArray);
  }

  /// <summary>Get the value of a default output placeholder like <c>DROP_IN_ID</c>.</summary>
  /// <inheritdoc cref="T:StardewValley.MachineDataUtility.GetOutputTokenValueDelegate" />
  private static string GetTokenValue(
    string key,
    Object machine,
    MachineItemOutput outputData,
    Item inputItem,
    Farmer who)
  {
    switch (key)
    {
      case "DROP_IN_ID":
        return inputItem?.QualifiedItemId ?? "0";
      case "DROP_IN_PRESERVE":
        return (inputItem is Object @object ? @object.GetPreservedItemId() : (string) null) ?? "0";
      case "NEARBY_FLOWER_ID":
        return MachineDataUtility.GetNearbyFlowerItemId(machine) ?? "-1";
      case "DROP_IN_QUALITY":
        return inputItem?.Quality.ToString() ?? "";
      default:
        return key;
    }
  }

  /// <summary>Get the item ID produced by a flower within 5 tiles of the machine, if any.</summary>
  /// <param name="machine">The machine around which to check.</param>
  public static string GetNearbyFlowerItemId(Object machine)
  {
    return Utility.findCloseFlower(machine.Location, machine.tileLocation.Value, 5, (Func<Crop, bool>) (curCrop => !curCrop.forageCrop.Value))?.indexOfHarvest.Value;
  }

  /// <summary>Get the value of a token placeholder like <c>DROP_IN_ID</c>.</summary>
  /// <param name="key">The token placeholder like <c>DROP_IN_ID</c>.</param>
  /// <param name="machine">The machine which will produce output.</param>
  /// <param name="outputData">The machine output data.</param>
  /// <param name="inputItem">The item that was dropped into the machine.</param>
  /// <param name="who">The player interacting with the machine, if any.</param>
  public delegate string GetOutputTokenValueDelegate(
    string key,
    Object machine,
    MachineItemOutput outputData,
    Item inputItem,
    Farmer who);
}
