// Decompiled with JetBrains decompiler
// Type: StardewValley.Extensions.ReflectionExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Sickhead.Engine.Util;
using System;
using System.Reflection;

#nullable disable
namespace StardewValley.Extensions;

/// <summary>Provides utility extension for reflection.</summary>
public static class ReflectionExtensions
{
  /// <summary>Try to set the field or property's value from its string representation.</summary>
  /// <param name="info">The field or property to set.</param>
  /// <param name="obj">The object instance whose field or property to set.</param>
  /// <param name="rawValue">A string representation of the value to set. This will be converted to the property type if possible.</param>
  /// <param name="index">Optional index values for an indexed property. This should be null for fields or non-indexed properties.</param>
  /// <param name="error">An error indicating why the property value could not be set, if applicable.</param>
  public static bool TrySetValueFromString(
    this MemberInfo info,
    object obj,
    string rawValue,
    object[] index,
    out string error)
  {
    FieldInfo fieldInfo = info as FieldInfo;
    Type conversionType;
    bool flag;
    if ((object) fieldInfo == null)
    {
      PropertyInfo propertyInfo = info as PropertyInfo;
      if ((object) propertyInfo != null)
      {
        conversionType = propertyInfo.PropertyType;
        flag = propertyInfo.CanWrite;
      }
      else
      {
        error = "the member is not a field or property";
        return false;
      }
    }
    else
    {
      conversionType = fieldInfo.FieldType;
      flag = !fieldInfo.IsLiteral && !fieldInfo.IsLiteral;
    }
    if (!flag)
    {
      error = $"the {((object) (info as FieldInfo) != null ? "field" : "property")} property is read-only";
      return false;
    }
    object obj1;
    try
    {
      obj1 = Convert.ChangeType((object) rawValue, conversionType);
    }
    catch (FormatException ex)
    {
      error = $"can't convert value '{rawValue}' to the '{conversionType.FullName}' type";
      return false;
    }
    try
    {
      info.SetValue(obj, obj1, index);
      error = (string) null;
      return true;
    }
    catch (Exception ex)
    {
      error = ex.Message;
      return false;
    }
  }
}
