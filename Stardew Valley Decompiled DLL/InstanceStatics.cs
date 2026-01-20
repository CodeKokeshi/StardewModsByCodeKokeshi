// Decompiled with JetBrains decompiler
// Type: StardewValley.InstanceStatics
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley;

/// <summary>
/// When specified, all static fields in this class will be instanced for split screen multiplayer by default.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class InstanceStatics : Attribute
{
}
