// Decompiled with JetBrains decompiler
// Type: StardewValley.Triggers.TriggerActionManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Delegates;
using StardewValley.GameData;
using StardewValley.Network.NetEvents;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace StardewValley.Triggers;

/// <summary>Manages trigger actions defined in the <c>Data/TriggerActions</c> asset, which perform actions when their conditions are met.</summary>
public static class TriggerActionManager
{
  /// <summary>The trigger type raised overnight immediately before the game changes the date, sets up the new day, and saves.</summary>
  public const string trigger_dayEnding = "DayEnding";
  /// <summary>The trigger type raised when the player starts a day, after either sleeping or loading.</summary>
  public const string trigger_dayStarted = "DayStarted";
  /// <summary>The trigger type raised when the player arrives in a new location.</summary>
  public const string trigger_locationChanged = "LocationChanged";
  /// <summary>The trigger type used for actions that are triggered elsewhere than <c>Data/TriggerActions</c>.</summary>
  public const string trigger_manual = "Manual";
  /// <summary>The trigger types that can be used in the <see cref="F:StardewValley.GameData.TriggerActionData.Trigger" /> field.</summary>
  private static readonly HashSet<string> ValidTriggerTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
  {
    "DayEnding",
    "DayStarted",
    "LocationChanged",
    "Manual"
  };
  /// <summary>The action handlers indexed by name.</summary>
  /// <remarks>Action names are case-insensitive.</remarks>
  private static readonly Dictionary<string, TriggerActionDelegate> ActionHandlers = new Dictionary<string, TriggerActionDelegate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>A cached lookup of actions by trigger name.</summary>
  private static readonly Dictionary<string, List<CachedTriggerAction>> ActionsByTrigger = new Dictionary<string, List<CachedTriggerAction>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>A cached lookup of parsed action strings.</summary>
  private static readonly Dictionary<string, CachedAction> ActionCache = new Dictionary<string, CachedAction>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>A parsed action which does nothing.</summary>
  private static readonly CachedAction NullAction;
  /// <summary>The trigger action context used for a default manual option.</summary>
  private static readonly TriggerActionContext EmptyManualContext = new TriggerActionContext("Manual", LegacyShims.EmptyArray<object>(), (TriggerActionData) null);

  /// <summary>Register a trigger type.</summary>
  /// <param name="name">The trigger key. This is case-insensitive.</param>
  public static void RegisterTrigger(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      Game1.log.Error("Can't register an empty trigger type for Data/Triggers.");
    }
    else
    {
      TriggerActionManager.ValidTriggerTypes.Add(name);
      Game1.log.Verbose($"Registered trigger type for Data/Triggers: {name}.");
    }
  }

  /// <summary>Register an action handler.</summary>
  /// <param name="name">The action name. This is case-insensitive.</param>
  /// <param name="action">The handler to call when the action should apply.</param>
  public static void RegisterAction(string name, TriggerActionDelegate action)
  {
    if (TriggerActionManager.ActionHandlers.TryAdd(name, action))
      Game1.log.Verbose($"Registered trigger action handler '{name}'.");
    else
      Game1.log.Warn($"Can't add trigger action handler '{name}' because that name is already registered.");
  }

  /// <summary>Run all actions for a given trigger key.</summary>
  /// <param name="trigger">The trigger key to raise.</param>
  /// <param name="triggerArgs">The contextual arguments provided with the trigger, if applicable. For example, an 'item received' trigger might provide the item instance and index.</param>
  /// <param name="location">The location for which to check action conditions, or <c>null</c> to use the current location.</param>
  /// <param name="player">The player for which to check action conditions, or <c>null</c> to use the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check action conditions, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check action conditions, or <c>null</c> if not applicable.</param>
  public static void Raise(
    string trigger,
    object[] triggerArgs = null,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null)
  {
    string actualValue;
    if (TriggerActionManager.ValidTriggerTypes.TryGetValue(trigger, out actualValue))
    {
      trigger = actualValue;
      triggerArgs = triggerArgs ?? LegacyShims.EmptyArray<object>();
      foreach (CachedTriggerAction entry in (IEnumerable<CachedTriggerAction>) TriggerActionManager.GetActionsForTrigger(trigger))
        TriggerActionManager.TryRunActions(entry, trigger, triggerArgs, location, player, targetItem, inputItem);
    }
    else
      Game1.log.Error($"Can't raise unknown trigger type '{trigger}'.");
  }

  /// <summary>Parse a raw action value.</summary>
  /// <param name="action">The action string to parse.</param>
  /// <remarks>This is a low-level method. Most code should use <see cref="M:StardewValley.Triggers.TriggerActionManager.TryRunAction(System.String,System.String@,System.Exception@)" /> instead.</remarks>
  public static CachedAction ParseAction(string action)
  {
    if (string.IsNullOrWhiteSpace(action))
      return TriggerActionManager.NullAction;
    action = action.Trim();
    CachedAction action1;
    if (!TriggerActionManager.ActionCache.TryGetValue(action, out action1))
    {
      string[] args = ArgUtility.SplitBySpaceQuoteAware(action);
      string key = args[0];
      TriggerActionDelegate handler;
      CachedAction cachedAction;
      if (!TriggerActionManager.TryGetActionHandler(key, out handler))
        cachedAction = new CachedAction(args, TriggerActionManager.NullAction.Handler, $"unknown action '{key}' ignored (expected one of '{string.Join("', '", (IEnumerable<string>) TriggerActionManager.ActionHandlers.Keys.OrderBy<string, string>((Func<string, string>) (p => p), (IComparer<string>) StringComparer.OrdinalIgnoreCase))}')", true);
      else
        cachedAction = new CachedAction(args, handler, (string) null, false);
      action1 = cachedAction;
      TriggerActionManager.ActionCache[action] = action1;
    }
    return action1;
  }

  /// <summary>Get whether an action matches an existing action.</summary>
  /// <param name="action">The action string to validate.</param>
  /// <param name="error">An error phrase indicating why parsing the action failed (like 'unknown action X'), if applicable.</param>
  /// <returns>Returns whether the action was parsed successfully and matches an existing command.</returns>
  public static bool TryValidateActionExists(string action, out string error)
  {
    CachedAction action1 = TriggerActionManager.ParseAction(action);
    error = action1.Error;
    return error == null;
  }

  /// <summary>Run an action if it's valid.</summary>
  /// <param name="action">The action string to run.</param>
  /// <param name="error">An error phrase indicating why parsing or running the action failed (like 'unknown action X'), if applicable.</param>
  /// <param name="exception">An exception which accompanies <paramref name="error" />, if applicable.</param>
  /// <returns>Returns whether the action was applied successfully (regardless of whether it did anything).</returns>
  public static bool TryRunAction(string action, out string error, out Exception exception)
  {
    int num = TriggerActionManager.TryRunAction(TriggerActionManager.ParseAction(action), TriggerActionManager.EmptyManualContext, out error, out exception) ? 1 : 0;
    if (num != 0)
      return num != 0;
    if (!string.IsNullOrWhiteSpace(error))
      return num != 0;
    error = exception != null ? "an unhandled error occurred" : "the action failed but didn't provide an error message";
    return num != 0;
  }

  /// <summary>Run an action if it's valid.</summary>
  /// <param name="action">The action string to run.</param>
  /// <param name="trigger">The trigger key to raise.</param>
  /// <param name="triggerArgs">The contextual arguments provided with the trigger, if applicable. For example, an 'item received' trigger might provide the item instance and index.</param>
  /// <param name="error">An error phrase indicating why parsing or running the action failed (like 'unknown action X'), if applicable.</param>
  /// <param name="exception">An exception which accompanies <paramref name="error" />, if applicable.</param>
  /// <returns>Returns whether the action was applied successfully (regardless of whether it did anything).</returns>
  public static bool TryRunAction(
    string action,
    string trigger,
    object[] triggerArgs,
    out string error,
    out Exception exception)
  {
    if (trigger == null)
      throw new ArgumentNullException(nameof (trigger));
    if (triggerArgs == null)
      throw new ArgumentNullException(nameof (triggerArgs));
    TriggerActionContext context = !(trigger == "Manual") || triggerArgs.Length != 0 ? new TriggerActionContext(trigger, triggerArgs, (TriggerActionData) null) : TriggerActionManager.EmptyManualContext;
    return TriggerActionManager.TryRunAction(TriggerActionManager.ParseAction(action), context, out error, out exception);
  }

  /// <summary>Run an action if it's valid.</summary>
  /// <param name="action">The action to run.</param>
  /// <param name="context">The trigger action context.</param>
  /// <param name="error">An error phrase indicating why parsing or running the action failed (like 'unknown action X'), if applicable.</param>
  /// <param name="exception">An exception which accompanies <paramref name="error" />, if applicable.</param>
  /// <returns>Returns whether the action was applied successfully (regardless of whether it did anything).</returns>
  /// <remarks>This is a low-level method. Most code should use <see cref="M:StardewValley.Triggers.TriggerActionManager.TryRunAction(System.String,System.String@,System.Exception@)" /> instead.</remarks>
  public static bool TryRunAction(
    CachedAction action,
    TriggerActionContext context,
    out string error,
    out Exception exception)
  {
    if (action == null)
    {
      error = (string) null;
      exception = (Exception) null;
      return true;
    }
    if (action.Error != null)
    {
      error = action.Error;
      exception = (Exception) null;
      return false;
    }
    try
    {
      int num = action.Handler(action.Args, context, out error) ? 1 : 0;
      if (error != null)
      {
        exception = (Exception) null;
        return false;
      }
      exception = (Exception) null;
      return true;
    }
    catch (Exception ex)
    {
      error = "an unexpected error occurred";
      exception = ex;
      return false;
    }
  }

  /// <summary>Run all actions from a given <c>Data/TriggerActions</c> entry, if its fields match the current context.</summary>
  /// <param name="entry">The entry to apply from <c>Data/TriggerActions</c>, as returned by <see cref="M:StardewValley.Triggers.TriggerActionManager.GetActionsForTrigger(System.String)" />.</param>
  /// <param name="trigger">The trigger key to raise.</param>
  /// <param name="triggerArgs">The contextual arguments provided with the trigger, if applicable. For example, an 'item received' trigger might provide the item instance and index.</param>
  /// <param name="location">The location for which to check action conditions, or <c>null</c> to use the current location.</param>
  /// <param name="player">The player for which to check action conditions, or <c>null</c> to use the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check action conditions, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check action conditions, or <c>null</c> if not applicable.</param>
  /// <returns>Returns whether any of the actions were applied.</returns>
  public static bool TryRunActions(
    CachedTriggerAction entry,
    string trigger,
    object[] triggerArgs = null,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null)
  {
    TriggerActionData data = entry.Data;
    if (Game1.player.triggerActionsRun.Contains(data.Id))
      return false;
    if (data.SkipPermanentlyCondition != null && GameStateQuery.CheckConditions(data.SkipPermanentlyCondition))
    {
      Game1.player.triggerActionsRun.Add(data.Id);
      return false;
    }
    if (!TriggerActionManager.CanApplyIgnoringRun(data, location, player, targetItem, inputItem))
      return false;
    TriggerActionContext context = new TriggerActionContext(trigger, triggerArgs, data);
    foreach (CachedAction action in entry.Actions)
    {
      string error;
      Exception exception;
      if (!TriggerActionManager.TryRunAction(action, context, out error, out exception))
        Game1.log.Error($"Trigger action '{data.Id}' has action string '{string.Join(" ", action.Args)}' which couldn't be applied: {error}.", exception);
    }
    if (data.MarkActionApplied)
      Game1.player.triggerActionsRun.Add(data.Id);
    Game1.log.Verbose($"Applied trigger action '{data.Id}' with actions [{string.Join("], [", entry.ActionStrings)}].");
    return true;
  }

  /// <summary>Get the handler for an action key, if any.</summary>
  /// <param name="key">The action key. This is case-insensitive.</param>
  /// <param name="handler">The action handler, if found.</param>
  /// <returns>Returns whether a handler was found for the action key.</returns>
  public static bool TryGetActionHandler(string key, out TriggerActionDelegate handler)
  {
    return TriggerActionManager.ActionHandlers.TryGetValue(key, out handler);
  }

  /// <summary>Get the trigger actions in <c>Data/TriggerActions</c> registered for a given trigger, or an empty list if none are registered.</summary>
  /// <param name="trigger">The trigger key to raise.</param>
  /// <remarks>This is a low-level method. Most code should use <see cref="M:StardewValley.Triggers.TriggerActionManager.TryRunAction(System.String,System.String@,System.Exception@)" /> instead.</remarks>
  public static IReadOnlyList<CachedTriggerAction> GetActionsForTrigger(string trigger)
  {
    List<CachedTriggerAction> cachedTriggerActionList;
    return TriggerActionManager.GetActionsByTrigger().TryGetValue(trigger, out cachedTriggerActionList) ? (IReadOnlyList<CachedTriggerAction>) cachedTriggerActionList : (IReadOnlyList<CachedTriggerAction>) LegacyShims.EmptyArray<CachedTriggerAction>();
  }

  /// <summary>Get whether an action can be applied based on its conditions and whether it has already been run.</summary>
  /// <param name="action">The action to check.</param>
  /// <param name="location">The location for which to check action conditions, or <c>null</c> to use the current location.</param>
  /// <param name="player">The player for which to check action conditions, or <c>null</c> to use the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check action conditions, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check action conditions, or <c>null</c> if not applicable.</param>
  public static bool CanApply(
    TriggerActionData action,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null)
  {
    return !Game1.player.triggerActionsRun.Contains(action.Id) && TriggerActionManager.CanApplyIgnoringRun(action, location, player, targetItem, inputItem);
  }

  /// <summary>Rebuild the cached data from <c>Data/TriggerActions</c>.</summary>
  public static void ResetDataCache()
  {
    TriggerActionManager.ActionCache.Clear();
    TriggerActionManager.ActionsByTrigger.Clear();
  }

  /// <summary>Initialize the base static state.</summary>
  static TriggerActionManager()
  {
    foreach (MethodInfo method in typeof (TriggerActionManager.DefaultActions).GetMethods(BindingFlags.Static | BindingFlags.Public))
    {
      TriggerActionDelegate triggerActionDelegate = (TriggerActionDelegate) Delegate.CreateDelegate(typeof (TriggerActionDelegate), method);
      TriggerActionManager.ActionHandlers.Add(method.Name, triggerActionDelegate);
    }
    TriggerActionManager.NullAction = new CachedAction(LegacyShims.EmptyArray<string>(), TriggerActionManager.ActionHandlers["Null"], (string) null, true);
  }

  /// <summary>Get the registered actions by trigger, loading them if needed.</summary>
  private static Dictionary<string, List<CachedTriggerAction>> GetActionsByTrigger()
  {
    Dictionary<string, List<CachedTriggerAction>> actionsByTrigger = TriggerActionManager.ActionsByTrigger;
    if (actionsByTrigger.Count == 0)
    {
      foreach (string validTriggerType in TriggerActionManager.ValidTriggerTypes)
        actionsByTrigger[validTriggerType] = new List<CachedTriggerAction>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<CachedAction> cachedActionList = new List<CachedAction>();
      foreach (TriggerActionData triggerAction in DataLoader.TriggerActions(Game1.content))
      {
        if (string.IsNullOrWhiteSpace(triggerAction.Id))
          Game1.log.Error("Trigger action has no ID field and will be ignored.");
        else if (string.IsNullOrWhiteSpace(triggerAction.Trigger))
        {
          Game1.log.Error($"Trigger action '{triggerAction.Id}' has no trigger; expected one of '{string.Join("', '", (IEnumerable<string>) TriggerActionManager.ValidTriggerTypes)}'.");
        }
        else
        {
          if (string.IsNullOrWhiteSpace(triggerAction.Action))
          {
            List<string> actions = triggerAction.Actions;
            // ISSUE: explicit non-virtual call
            if ((actions != null ? (__nonvirtual (actions.Count) > 0 ? 1 : 0) : 0) == 0)
            {
              Game1.log.Error($"Trigger action '{triggerAction.Id}' has no defined actions.");
              continue;
            }
          }
          if (!stringSet.Add(triggerAction.Id))
          {
            Game1.log.Error($"Trigger action '{triggerAction.Id}' has a duplicate ID. Only the first instance will be used.");
          }
          else
          {
            cachedActionList.Clear();
            if (triggerAction.Action != null)
            {
              CachedAction action = TriggerActionManager.ParseAction(triggerAction.Action);
              if (action.Error != null)
                Game1.log.Error($"Trigger action '{triggerAction.Id}' will skip invalid action '{triggerAction.Action}': {action.Error}.");
              else if (!action.IsNullHandler)
                cachedActionList.Add(action);
            }
            if (triggerAction.Actions != null)
            {
              foreach (string action1 in triggerAction.Actions)
              {
                CachedAction action2 = TriggerActionManager.ParseAction(action1);
                if (action2.Error != null)
                  Game1.log.Error($"Trigger action '{triggerAction.Id}' will skip invalid action '{triggerAction.Action}': {action2.Error}.");
                else if (!action2.IsNullHandler)
                  cachedActionList.Add(action2);
              }
            }
            CachedTriggerAction cachedTriggerAction = new CachedTriggerAction(triggerAction, cachedActionList.ToArray());
            foreach (string key in ArgUtility.SplitBySpace(triggerAction.Trigger))
            {
              if (!TriggerActionManager.ValidTriggerTypes.Contains(key))
                Game1.log.Error($"Trigger action '{triggerAction.Id}' has unknown trigger '{key}'; expected one of '{string.Join("', '", (IEnumerable<string>) TriggerActionManager.ValidTriggerTypes)}'.");
              else
                actionsByTrigger[key].Add(cachedTriggerAction);
            }
          }
        }
      }
    }
    return actionsByTrigger;
  }

  /// <summary>Get whether an action can be applied based on its conditions, ignoring whether it has already been run.</summary>
  /// <param name="action">The action to check.</param>
  /// <param name="location">The location for which to check action conditions, or <c>null</c> to use the current location.</param>
  /// <param name="player">The player for which to check action conditions, or <c>null</c> to use the current player.</param>
  /// <param name="targetItem">The target item (e.g. machine output or tree fruit) for which to check action conditions, or <c>null</c> if not applicable.</param>
  /// <param name="inputItem">The input item (e.g. machine input) for which to check action conditions, or <c>null</c> if not applicable.</param>
  private static bool CanApplyIgnoringRun(
    TriggerActionData action,
    GameLocation location = null,
    Farmer player = null,
    Item targetItem = null,
    Item inputItem = null)
  {
    return (!action.HostOnly || Game1.IsMasterGame) && GameStateQuery.CheckConditions(action.Condition, location, player, targetItem, inputItem);
  }

  /// <summary>The low-level trigger actions defined by the base game. Most code should use <see cref="T:StardewValley.Triggers.TriggerActionManager" /> methods instead.</summary>
  /// <remarks>Every method within this class is an action whose name matches the method name. All actions must be static, public, and match <see cref="T:StardewValley.Delegates.TriggerActionDelegate" />.</remarks>
  public static class DefaultActions
  {
    /// <summary>An action which does nothing.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool Null(string[] args, TriggerActionContext context, out string error)
    {
      error = (string) null;
      return true;
    }

    /// <summary>Perform an action if a game state query matches, with an optional fallback action.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool If(string[] args, TriggerActionContext context, out string error)
    {
      int startAt1 = -1;
      for (int index = 1; index < args.Length; ++index)
      {
        if (args[index] == "##")
        {
          startAt1 = index + 1;
          break;
        }
      }
      if (startAt1 == -1 || startAt1 == args.Length)
        return InvalidFormatError(out error);
      int startAt2 = -1;
      for (int index = startAt1 + 1; index < args.Length; ++index)
      {
        if (args[index] == "##")
        {
          startAt2 = index + 1;
          break;
        }
      }
      if (startAt2 == args.Length - 1)
        return InvalidFormatError(out error);
      Exception exception;
      if (GameStateQuery.CheckConditions(ArgUtility.UnsplitQuoteAware(args, ' ', 1, startAt1 - 1 - 1)))
      {
        int count = startAt2 > -1 ? startAt2 - startAt1 - 1 : int.MaxValue;
        string action = ArgUtility.UnsplitQuoteAware(args, ' ', startAt1, count);
        if (!TriggerActionManager.TryRunAction(action, out error, out exception))
        {
          error = $"failed applying if-true action '{action}': {error}";
          return false;
        }
      }
      else if (startAt2 > -1)
      {
        string action = ArgUtility.UnsplitQuoteAware(args, ' ', startAt2);
        if (!TriggerActionManager.TryRunAction(action, out error, out exception))
        {
          error = $"failed applying if-false action '{action}': {error}";
          return false;
        }
      }
      error = (string) null;
      return true;

      static bool InvalidFormatError(out string outError)
      {
        outError = "invalid format: expected a string in the form 'If <game state query> ## <do if true>' or 'If <game state query> ## <do if true> ## <do if false>'";
        return false;
      }
    }

    /// <summary>Apply a buff to the current player.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddBuff(string[] args, TriggerActionContext context, out string error)
    {
      string id;
      int duration;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string buffId") || !ArgUtility.TryGetOptionalInt(args, 2, out duration, out error, -1, "int duration"))
        return false;
      Game1.player.applyBuff(new Buff(id, duration: duration));
      return true;
    }

    /// <summary>Remove a buff from the current player.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveBuff(string[] args, TriggerActionContext context, out string error)
    {
      string id;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string buffId"))
        return false;
      Game1.player.buffs.Remove(id);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddMail(string[] args, TriggerActionContext context, out string error)
    {
      PlayerActionTarget playerTarget;
      string mailId;
      MailType mailType;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out playerTarget, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out mailId, out error, name: "string mailId") || !ArgUtility.TryGetOptionalEnum<MailType>(args, 3, out mailType, out error, MailType.Tomorrow, "MailType mailType"))
        return false;
      Game1.player.team.RequestSetMail(playerTarget, mailId, mailType, true);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveMail(string[] args, TriggerActionContext context, out string error)
    {
      PlayerActionTarget playerTarget;
      string mailId;
      MailType mailType;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out playerTarget, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out mailId, out error, name: "string mailId") || !ArgUtility.TryGetOptionalEnum<MailType>(args, 3, out mailType, out error, MailType.All, "MailType mailType"))
        return false;
      Game1.player.team.RequestSetMail(playerTarget, mailId, mailType, false);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddQuest(string[] args, TriggerActionContext context, out string error)
    {
      string questId;
      if (!ArgUtility.TryGet(args, 1, out questId, out error, name: "string questId"))
        return false;
      Game1.player.addQuest(questId);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveQuest(string[] args, TriggerActionContext context, out string error)
    {
      string questID;
      if (!ArgUtility.TryGet(args, 1, out questID, out error, name: "string questId"))
        return false;
      Game1.player.removeQuest(questID);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddSpecialOrder(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string id;
      if (!ArgUtility.TryGet(args, 1, out id, out error, name: "string orderId"))
        return false;
      Game1.player.team.AddSpecialOrder(id);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveSpecialOrder(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string orderId;
      if (!ArgUtility.TryGet(args, 1, out orderId, out error, name: "string orderId"))
        return false;
      Game1.player.team.specialOrders.RemoveWhere((Func<SpecialOrder, bool>) (order => order.questKey.Value == orderId));
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddItem(string[] args, TriggerActionContext context, out string error)
    {
      string itemId;
      int amount;
      int quality;
      if (!ArgUtility.TryGet(args, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(args, 2, out amount, out error, 1, "int count") || !ArgUtility.TryGetOptionalInt(args, 3, out quality, out error, name: "int quality"))
        return false;
      Item obj = ItemRegistry.Create(itemId, amount, quality);
      if (obj != null)
        Game1.player.addItemByMenuIfNecessary(obj);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveItem(string[] args, TriggerActionContext context, out string error)
    {
      string itemId;
      int count;
      if (!ArgUtility.TryGet(args, 1, out itemId, out error, name: "string itemId") || !ArgUtility.TryGetOptionalInt(args, 2, out count, out error, 1, "int count"))
        return false;
      Game1.player.removeFirstOfThisItemFromInventory(itemId, count);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddMoney(string[] args, TriggerActionContext context, out string error)
    {
      int num;
      if (!ArgUtility.TryGetInt(args, 1, out num, out error, "int amount"))
        return false;
      Game1.player.Money += num;
      if (Game1.player.Money < 0)
        Game1.player.Money = 0;
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddFriendshipPoints(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string name;
      int amount;
      if (!ArgUtility.TryGet(args, 1, out name, out error, name: "string npcName") || !ArgUtility.TryGetInt(args, 2, out amount, out error, "int points"))
        return false;
      NPC characterFromName = Game1.getCharacterFromName(name);
      if (characterFromName == null)
      {
        error = $"no NPC found with name '{name}'";
        return false;
      }
      Game1.player.changeFriendship(amount, characterFromName);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool AddConversationTopic(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string key;
      int num;
      if (!ArgUtility.TryGet(args, 1, out key, out error, name: "string topicId") || !ArgUtility.TryGetOptionalInt(args, 2, out num, out error, 4, "int daysDuration"))
        return false;
      Game1.player.activeDialogueEvents[key] = num;
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveConversationTopic(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string key;
      if (!ArgUtility.TryGet(args, 1, out key, out error, name: "string topicId"))
        return false;
      Game1.player.activeDialogueEvents.Remove(key);
      return true;
    }

    /// <summary>Increment or decrement a stats value for the current player.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool IncrementStat(string[] args, TriggerActionContext context, out string error)
    {
      string key;
      int amount;
      if (!ArgUtility.TryGet(args, 1, out key, out error, false, "string statKey") || !ArgUtility.TryGetOptionalInt(args, 2, out amount, out error, 1, "int amount"))
        return false;
      int num = (int) Game1.player.stats.Increment(key, amount);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkActionApplied(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, false, "string actionId") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool applied"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.ActionApplied, target, flagId, flagState);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkCookingRecipeKnown(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, name: "string recipeKey") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool learned"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.CookingRecipeKnown, target, flagId, flagState);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkCraftingRecipeKnown(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, name: "string recipeKey") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool learned"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.CraftingRecipeKnown, target, flagId, flagState);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkEventSeen(string[] args, TriggerActionContext context, out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, false, "string eventId") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool seen"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.EventSeen, target, flagId, flagState);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkQuestionAnswered(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, false, "string questionId") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool answered"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.DialogueAnswerSelected, target, flagId, flagState);
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool MarkSongHeard(string[] args, TriggerActionContext context, out string error)
    {
      PlayerActionTarget target;
      string flagId;
      bool flagState;
      if (!ArgUtility.TryGetEnum<PlayerActionTarget>(args, 1, out target, out error, "PlayerActionTarget playerTarget") || !ArgUtility.TryGet(args, 2, out flagId, out error, false, "string trackId") || !ArgUtility.TryGetOptionalBool(args, 3, out flagState, out error, true, "bool heard"))
        return false;
      Game1.player.team.RequestSetSimpleFlag(SimpleFlagType.SongHeard, target, flagId, flagState);
      return true;
    }

    /// <summary>Remove all temporary animated sprites in the current location.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool RemoveTemporaryAnimatedSprites(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      Game1.currentLocation?.TemporarySprites.Clear();
      error = (string) null;
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool SetNpcInvisible(
      string[] args,
      TriggerActionContext context,
      out string error)
    {
      string name;
      int num;
      if (!ArgUtility.TryGet(args, 1, out name, out error, false, "string npcName") || !ArgUtility.TryGetInt(args, 2, out num, out error, "int daysDuration"))
        return false;
      NPC characterFromName = Game1.getCharacterFromName(name);
      if (characterFromName == null)
      {
        error = $"no NPC found with name '{name}'";
        return false;
      }
      characterFromName.IsInvisible = true;
      characterFromName.daysUntilNotInvisible = num;
      return true;
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.TriggerActionDelegate" />
    public static bool SetNpcVisible(string[] args, TriggerActionContext context, out string error)
    {
      string name;
      if (!ArgUtility.TryGet(args, 1, out name, out error, false, "string npcName"))
        return false;
      NPC characterFromName = Game1.getCharacterFromName(name);
      if (characterFromName == null)
      {
        error = $"no NPC found with name '{name}'";
        return false;
      }
      characterFromName.IsInvisible = false;
      characterFromName.daysUntilNotInvisible = 0;
      return true;
    }
  }
}
