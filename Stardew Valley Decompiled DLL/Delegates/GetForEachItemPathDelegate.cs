// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.GetForEachItemPathDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Delegates;

/// <summary>Get the contextual path leading to an item in the world. For example, an item inside a chest would have the location and chest as path vlaues.</summary>
public delegate IList<object> GetForEachItemPathDelegate();
