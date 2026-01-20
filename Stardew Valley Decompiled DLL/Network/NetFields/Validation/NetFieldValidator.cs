// Decompiled with JetBrains decompiler
// Type: Netcode.Validation.NetFieldValidator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Netcode.Validation;

/// <summary>A utility which auto-detects common net field issues.</summary>
public static class NetFieldValidator
{
  /// <summary>Detect and log warnings for common issues like net fields not added to the collection.</summary>
  /// <param name="owner">The object instance whose net fields to validate.</param>
  /// <param name="onError">The method to call when an error occurs.</param>
  public static void ValidateNetFields(INetObject<NetFields> owner, Action<string> onError)
  {
    string name = owner.NetFields.Name;
    HashSet<INetSerializable> trackedFields = new HashSet<INetSerializable>(owner.NetFields.GetFields(), (IEqualityComparer<INetSerializable>) ReferenceEqualityComparer.Instance);
    List<NetFieldValidatorEntry> fieldValidatorEntryList = new List<NetFieldValidatorEntry>();
    foreach (FieldInfo field in owner.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
    {
      NetFieldValidatorEntry netField;
      if (NetFieldValidatorEntry.TryGetNetField(owner, field, out netField))
      {
        if (netField.IsMarkedNotNetField())
        {
          if (NetFieldValidator.IsInCollection(trackedFields, (object) netField))
            onError(NetFieldValidator.GetFieldError(name, netField, "is marked [NotNetFieldAttribute] but still added to the collection"));
          else
            continue;
        }
        fieldValidatorEntryList.Add(netField);
      }
    }
    foreach (NetFieldValidatorEntry entry in fieldValidatorEntryList)
    {
      if (entry.Value == null)
        onError(NetFieldValidator.GetFieldError(name, entry, "is null"));
      else if (string.IsNullOrWhiteSpace(entry.Name))
        onError(NetFieldValidator.GetFieldError(name, entry, "has no name (and likely isn't in the collection)"));
      else if (!NetFieldValidator.IsInCollection(trackedFields, entry.Value))
        onError(NetFieldValidator.GetFieldError(name, entry, "isn't in the collection"));
    }
  }

  /// <summary>Get a human-readable error message for a field validation error.</summary>
  /// <param name="collectionName">The name of the net fields collection being validated.</param>
  /// <param name="entry">The validator entry for the net field being validated.</param>
  /// <param name="phrase">A short phrase which indicates why it failed validation, like <c>is null</c>.</param>
  private static string GetFieldError(
    string collectionName,
    NetFieldValidatorEntry entry,
    string phrase)
  {
    return $"The owner of {"NetFields"} collection '{collectionName}' has field '{entry.FromField.Name}' which {phrase}.";
  }

  /// <summary>Get whether the net field is in the owner's <see cref="P:Netcode.INetObject`1.NetFields" /> collection.</summary>
  /// <param name="trackedFields">The fields that are synced as part of the collection.</param>
  /// <param name="netField">The net field instance to find.</param>
  private static bool IsInCollection(HashSet<INetSerializable> trackedFields, object netField)
  {
    switch (netField)
    {
      case INetSerializable netSerializable:
        return trackedFields.Contains(netSerializable);
      case INetObject<NetFields> netObject:
        return trackedFields.Contains((INetSerializable) netObject.NetFields);
      default:
        return false;
    }
  }
}
