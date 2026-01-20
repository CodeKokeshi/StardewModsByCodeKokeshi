// Decompiled with JetBrains decompiler
// Type: Netcode.Validation.NotNetFieldAttribute
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace Netcode.Validation;

/// <summary>Indicates that the field isn't synchronized in multiplayer, so there's no need to validate it as such in <see cref="T:Netcode.Validation.NetFieldValidator" />.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class NotNetFieldAttribute : Attribute
{
}
