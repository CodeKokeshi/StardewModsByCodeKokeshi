// Decompiled with JetBrains decompiler
// Type: Netcode.Validation.NotImplicitNetFieldAttribute
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace Netcode.Validation;

/// <summary>Indicates that a field of this type isn't automatically a synchronized net field, even if it implements <see cref="T:Netcode.INetSerializable" /> or <see cref="T:Netcode.INetObject`1" />.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class NotImplicitNetFieldAttribute : Attribute
{
}
