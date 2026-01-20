// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.TriggerActionDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Delegates;

/// <summary>A delegate which handles an action which can be triggered via <c>Data/TriggerActions</c>, registered via <see cref="M:StardewValley.Triggers.TriggerActionManager.RegisterAction(System.String,StardewValley.Delegates.TriggerActionDelegate)" />.</summary>
/// <param name="args">The space-delimited action string, including the action name.</param>
/// <param name="context">The trigger action context.</param>
/// <param name="error">An error phrase indicating why applying the action failed (like 'required argument X missing'), if applicable. This should always be set to <c>null</c> when returning true, and a non-empty message when returning false.</param>
/// <returns>Returns whether the action was handled successfully (regardless of whether it did anything).</returns>
public delegate bool TriggerActionDelegate(
  string[] args,
  TriggerActionContext context,
  out string error);
