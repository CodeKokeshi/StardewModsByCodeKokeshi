// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.DeepClonerExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Force.DeepCloner.Helpers;
using System;
using System.Security;

#nullable disable
namespace Force.DeepCloner;

/// <summary>Extensions for object cloning</summary>
public static class DeepClonerExtensions
{
  /// <summary>Performs deep (full) copy of object and related graph</summary>
  public static T DeepClone<T>(this T obj) => DeepClonerGenerator.CloneObject<T>(obj);

  /// <summary>
  /// Performs deep (full) copy of object and related graph to existing object
  /// </summary>
  /// <returns>existing filled object</returns>
  /// <remarks>Method is valid only for classes, classes should be descendants in reality, not in declaration</remarks>
  public static TTo DeepCloneTo<TFrom, TTo>(this TFrom objFrom, TTo objTo) where TTo : class, TFrom
  {
    return (TTo) DeepClonerGenerator.CloneObjectTo((object) objFrom, (object) objTo, true);
  }

  /// <summary>Performs shallow copy of object to existing object</summary>
  /// <returns>existing filled object</returns>
  /// <remarks>Method is valid only for classes, classes should be descendants in reality, not in declaration</remarks>
  public static TTo ShallowCloneTo<TFrom, TTo>(this TFrom objFrom, TTo objTo) where TTo : class, TFrom
  {
    return (TTo) DeepClonerGenerator.CloneObjectTo((object) objFrom, (object) objTo, false);
  }

  /// <summary>
  /// Performs shallow (only new object returned, without cloning of dependencies) copy of object
  /// </summary>
  public static T ShallowClone<T>(this T obj) => ShallowClonerGenerator.CloneObject<T>(obj);

  static DeepClonerExtensions()
  {
    if (!DeepClonerExtensions.PermissionCheck())
      throw new SecurityException("DeepCloner should have enough permissions to run. Grant FullTrust or Reflection permission.");
  }

  private static bool PermissionCheck()
  {
    try
    {
      new object().ShallowClone<object>();
    }
    catch (VerificationException ex)
    {
      return false;
    }
    catch (MemberAccessException ex)
    {
      return false;
    }
    return true;
  }
}
