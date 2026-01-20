// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.EventPreconditionDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Delegates;

/// <summary>The delegate for an event precondition registered via <see cref="M:StardewValley.Event.RegisterPrecondition(System.String,StardewValley.Delegates.EventPreconditionDelegate)" />.</summary>
/// <param name="location">The location which is checking the event.</param>
/// <param name="eventId">The unique ID for the event being checked.</param>
/// <param name="args">The space-delimited event precondition string, including the precondition name.</param>
public delegate bool EventPreconditionDelegate(
  GameLocation location,
  string eventId,
  string[] args);
