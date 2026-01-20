// Decompiled with JetBrains decompiler
// Type: Netcode.NetRefTypes
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable disable
namespace Netcode;

internal static class NetRefTypes
{
  private static Dictionary<string, Type> types = new Dictionary<string, Type>();

  public static Type ReadType(this BinaryReader reader)
  {
    Type type = reader.ReadGenericType();
    if (type == (Type) null || !type.IsGenericTypeDefinition)
      return type;
    int length = type.GetGenericArguments().Length;
    Type[] typeArray = new Type[length];
    for (int index = 0; index < length; ++index)
      typeArray[index] = reader.ReadType();
    return type.MakeGenericType(typeArray);
  }

  private static Type ReadGenericType(this BinaryReader reader)
  {
    string typeName = reader.ReadString();
    if (typeName.Length == 0)
      return (Type) null;
    Type type = NetRefTypes.GetType(typeName);
    return !(type == (Type) null) ? type : throw new InvalidOperationException();
  }

  public static void WriteType(this BinaryWriter writer, Type type)
  {
    Type type1 = type;
    if (type != (Type) null && type.IsGenericType)
      type1 = type.GetGenericTypeDefinition();
    writer.WriteGenericType(type1);
    if (type1 == (Type) null || !type1.IsGenericType)
      return;
    foreach (Type genericArgument in type.GetGenericArguments())
      writer.WriteType(genericArgument);
  }

  private static void WriteGenericType(this BinaryWriter writer, Type type)
  {
    if (type == (Type) null)
      writer.Write("");
    else
      writer.Write(type.FullName);
  }

  public static void WriteTypeOf<T>(this BinaryWriter writer, T value)
  {
    if ((object) value == null)
      writer.WriteType((Type) null);
    else
      writer.WriteType(value.GetType());
  }

  private static Type GetType(string typeName)
  {
    Type type1;
    if (NetRefTypes.types.TryGetValue(typeName, out type1))
      return type1;
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      Type type2 = assembly.GetType(typeName);
      if (type2 != (Type) null)
      {
        NetRefTypes.types[typeName] = type2;
        return type2;
      }
    }
    return (Type) null;
  }
}
