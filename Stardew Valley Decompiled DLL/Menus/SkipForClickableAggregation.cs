// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.SkipForClickableAggregation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Menus;

/// <summary>
/// When specified, this field will not be automatically added to the allClickableComponents list when it is populated.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class SkipForClickableAggregation : Attribute
{
}
