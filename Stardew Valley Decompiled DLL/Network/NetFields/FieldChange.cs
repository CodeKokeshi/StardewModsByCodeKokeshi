// Decompiled with JetBrains decompiler
// Type: Netcode.FieldChange`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace Netcode;

/// <summary>A delegate which handles a field value changing.</summary>
/// <param name="field">The field instance.</param>
/// <param name="oldValue">The previous field value.</param>
/// <param name="newValue">The new field value.</param>
public delegate void FieldChange<in TSelf, in TValue>(
  TSelf field,
  TValue oldValue,
  TValue newValue);
