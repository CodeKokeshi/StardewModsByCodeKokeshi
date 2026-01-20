// Decompiled with JetBrains decompiler
// Type: Netcode.Validation.NetFieldValidatorEntry
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Reflection;

#nullable disable
namespace Netcode.Validation;

/// <summary>The metadata for a field being validated by <see cref="T:Netcode.Validation.NetFieldValidator" />.</summary>
public class NetFieldValidatorEntry
{
  /// <summary>The name of the net field being synced.</summary>
  public string Name { get; }

  /// <summary>The synchronized field value.</summary>
  public object Value { get; }

  /// <summary>The C# field on the owner.</summary>
  public FieldInfo FromField { get; }

  /// <summary>Construct an instance.</summary>
  /// <param name="name">The name of the net field being synced.</param>
  /// <param name="value">The raw net field.</param>
  /// <param name="fromField">The C# field or property on the owner.</param>
  public NetFieldValidatorEntry(string name, object value, FieldInfo fromField)
  {
    this.Name = name;
    this.Value = value;
    this.FromField = fromField;
  }

  /// <summary>Get a validator entry for a C# field or property, if it's a net field.</summary>
  /// <param name="owner">The object instance whose net fields are being read.</param>
  /// <param name="field">The C# field or property to read.</param>
  /// <param name="netField">The validator entry, if it's a net field.</param>
  public static bool TryGetNetField(
    INetObject<NetFields> owner,
    FieldInfo field,
    out NetFieldValidatorEntry netField)
  {
    if (field.Name != "NetFields" && field.Name[0] != '<')
    {
      Type fieldType = field.FieldType;
      if (typeof (INetSerializable).IsAssignableFrom(fieldType) && !NetFieldValidatorEntry.IsMarkedNotImplicitNetField(fieldType))
      {
        INetSerializable netSerializable = (INetSerializable) field.GetValue((object) owner);
        netField = new NetFieldValidatorEntry(netSerializable?.Name, (object) netSerializable, field);
        return true;
      }
      if (typeof (INetObject<NetFields>).IsAssignableFrom(fieldType) && !NetFieldValidatorEntry.IsMarkedNotImplicitNetField(fieldType))
      {
        INetObject<NetFields> netObject = (INetObject<NetFields>) field.GetValue((object) owner);
        netField = new NetFieldValidatorEntry(netObject?.NetFields.Name, (object) netObject, field);
        return true;
      }
    }
    netField = (NetFieldValidatorEntry) null;
    return false;
  }

  /// <summary>Get whether a field is marked with <see cref="T:Netcode.Validation.NotNetFieldAttribute" />.</summary>
  public bool IsMarkedNotNetField()
  {
    return this.FromField.GetCustomAttribute<NotNetFieldAttribute>() != null;
  }

  /// <summary>Get whether a type is marked with <see cref="T:Netcode.Validation.NotImplicitNetFieldAttribute" />.</summary>
  /// <param name="type">The type to check.</param>
  public static bool IsMarkedNotImplicitNetField(Type type)
  {
    return type.GetCustomAttribute<NotImplicitNetFieldAttribute>(true) != null;
  }
}
