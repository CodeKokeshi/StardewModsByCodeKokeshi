// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.MachineInteractDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Delegates;

/// <summary>The method signature for a custom <see cref="F:StardewValley.GameData.Machines.MachineData.InteractMethod" /> method.</summary>
/// <param name="machine">The machine instance for which to produce output.</param>
/// <param name="location">The location containing the machine.</param>
/// <param name="player">The player using the machine.</param>
/// <returns>Returns whether the interaction was handled.</returns>
public delegate bool MachineInteractDelegate(Object machine, GameLocation location, Farmer player);
