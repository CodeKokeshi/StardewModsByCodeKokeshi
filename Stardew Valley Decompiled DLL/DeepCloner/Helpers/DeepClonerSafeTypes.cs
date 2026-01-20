// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.DeepClonerSafeTypes
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Force.DeepCloner.Helpers;

/// <summary>
/// Safe types are types, which can be copied without real cloning. e.g. simple structs or strings (it is immutable)
/// </summary>
internal static class DeepClonerSafeTypes
{
  internal static readonly ConcurrentDictionary<Type, bool> KnownTypes = new ConcurrentDictionary<Type, bool>();

  static DeepClonerSafeTypes()
  {
    Type[] typeArray = new Type[19]
    {
      typeof (byte),
      typeof (short),
      typeof (ushort),
      typeof (int),
      typeof (uint),
      typeof (long),
      typeof (ulong),
      typeof (float),
      typeof (double),
      typeof (Decimal),
      typeof (char),
      typeof (string),
      typeof (bool),
      typeof (DateTime),
      typeof (IntPtr),
      typeof (UIntPtr),
      typeof (Guid),
      Type.GetType("System.RuntimeType"),
      Type.GetType("System.RuntimeTypeHandle")
    };
    foreach (Type key in typeArray)
      DeepClonerSafeTypes.KnownTypes.TryAdd(key, true);
  }

  private static bool CanReturnSameType(Type type, HashSet<Type> processingTypes)
  {
    bool flag;
    if (DeepClonerSafeTypes.KnownTypes.TryGetValue(type, out flag))
      return flag;
    if (type.IsEnum() || type.IsPointer)
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.FullName.StartsWith("System.DBNull"))
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.FullName.StartsWith("System.RuntimeType"))
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.FullName.StartsWith("System.Reflection.") && object.Equals((object) type.GetTypeInfo().Assembly, (object) typeof (PropertyInfo).GetTypeInfo().Assembly))
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.IsSubclassOfTypeByName("CriticalFinalizerObject"))
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.FullName.StartsWith("Microsoft.Extensions.DependencyInjection."))
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (type.FullName == "Microsoft.EntityFrameworkCore.Internal.ConcurrencyDetector")
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
      return true;
    }
    if (!type.IsValueType())
    {
      DeepClonerSafeTypes.KnownTypes.TryAdd(type, false);
      return false;
    }
    if (processingTypes == null)
      processingTypes = new HashSet<Type>();
    processingTypes.Add(type);
    List<FieldInfo> fieldInfoList = new List<FieldInfo>();
    Type t = type;
    do
    {
      fieldInfoList.AddRange((IEnumerable<FieldInfo>) t.GetAllFields());
      t = t.BaseType();
    }
    while (t != (Type) null);
    foreach (FieldInfo fieldInfo in fieldInfoList)
    {
      Type fieldType = fieldInfo.FieldType;
      if (!processingTypes.Contains(fieldType) && !DeepClonerSafeTypes.CanReturnSameType(fieldType, processingTypes))
      {
        DeepClonerSafeTypes.KnownTypes.TryAdd(type, false);
        return false;
      }
    }
    DeepClonerSafeTypes.KnownTypes.TryAdd(type, true);
    return true;
  }

  public static bool CanReturnSameObject(Type type)
  {
    return DeepClonerSafeTypes.CanReturnSameType(type, (HashSet<Type>) null);
  }
}
