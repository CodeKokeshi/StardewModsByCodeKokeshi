// Decompiled with JetBrains decompiler
// Type: Sickhead.Engine.Util.MemberInfoExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Reflection;

#nullable disable
namespace Sickhead.Engine.Util;

/// <summary>
/// Allows Set/GetValue of MemberInfo(s) so that code does not need to
/// be written to work specifically on PropertyInfo or FieldInfo.
/// </summary>
public static class MemberInfoExtensions
{
  public static Type GetDataType(this MemberInfo info)
  {
    PropertyInfo propertyInfo = info as PropertyInfo;
    if ((object) propertyInfo != null)
      return propertyInfo.PropertyType;
    return (info as FieldInfo ?? throw new InvalidOperationException($"MemberInfo.GetDataType is not possible for type={info.GetType()}")).FieldType;
  }

  public static object GetValue(this MemberInfo info, object obj)
  {
    return info.GetValue(obj, (object[]) null);
  }

  public static void SetValue(this MemberInfo info, object obj, object value)
  {
    info.SetValue(obj, value, (object[]) null);
  }

  public static object GetValue(this MemberInfo info, object obj, object[] index)
  {
    PropertyInfo propertyInfo = info as PropertyInfo;
    if ((object) propertyInfo != null)
      return propertyInfo.GetValue(obj, index);
    return (info as FieldInfo ?? throw new InvalidOperationException($"MemberInfo.GetValue is not possible for type={info.GetType()}")).GetValue(obj);
  }

  public static void SetValue(this MemberInfo info, object obj, object value, object[] index)
  {
    PropertyInfo propertyInfo = info as PropertyInfo;
    if ((object) propertyInfo == null)
      (info as FieldInfo ?? throw new InvalidOperationException($"MemberInfo.SetValue is not possible for type={info.GetType()}")).SetValue(obj, value);
    else
      propertyInfo.SetValue(obj, value, index);
  }

  public static bool IsStatic(this MemberInfo info)
  {
    PropertyInfo propertyInfo = info as PropertyInfo;
    if ((object) propertyInfo != null)
      return propertyInfo.GetGetMethod(true).IsStatic;
    FieldInfo fieldInfo = info as FieldInfo;
    if ((object) fieldInfo != null)
      return fieldInfo.IsStatic;
    return (info as MethodInfo ?? throw new InvalidOperationException($"MemberInfo.IsStatic is not possible for type={info.GetType()}")).IsStatic;
  }

  /// <summary>
  /// Returns true if this is a property or field that is accessible to be set via reflection
  /// on all platforms. Note: windows phone can only set public or internal scope members.
  /// </summary>
  public static bool CanBeSet(this MemberInfo info)
  {
    PropertyInfo propertyInfo = info as PropertyInfo;
    if ((object) propertyInfo == null)
    {
      FieldInfo fieldInfo = info as FieldInfo;
      if ((object) fieldInfo != null)
        return !fieldInfo.IsPrivate && !fieldInfo.IsFamily;
      throw new InvalidOperationException($"MemberInfo.CanSet is not possible for type={info.GetType()}");
    }
    MethodAttributes attributes = propertyInfo.GetSetMethod().Attributes;
    if (!propertyInfo.CanWrite)
      return true;
    return (attributes & MethodAttributes.Public) != MethodAttributes.Public && (attributes & MethodAttributes.Assembly) != MethodAttributes.Assembly;
  }

  /// <summary>
  /// In Win8 the static Delegate.Create was removed and added
  /// instead as an instance method on MethodInfo. Therefore it
  /// is most portable if the new api is used and this extension
  /// translates it to the older API on those platforms.
  /// </summary>
  public static Delegate CreateDelegate(this MethodInfo method, Type type, object target)
  {
    return Delegate.CreateDelegate(type, target, method);
  }

  public static Delegate CreateDelegate(this MethodInfo method, Type type)
  {
    return Delegate.CreateDelegate(type, method);
  }
}
