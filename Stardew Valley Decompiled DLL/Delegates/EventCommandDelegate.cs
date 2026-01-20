// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.EventCommandDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Delegates;

/// <summary>The delegate for an event command registered via <see cref="M:StardewValley.Event.RegisterCommand(System.String,StardewValley.Delegates.EventCommandDelegate)" />.</summary>
/// <param name="event">The event running the command.</param>
/// <param name="args">The space-delimited event command string, including the command name.</param>
/// <param name="context">The context for the active event.</param>
public delegate void EventCommandDelegate(Event @event, string[] args, EventContext context);
