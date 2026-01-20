// Decompiled with JetBrains decompiler
// Type: StardewValley.Internal.StaticDelegateBuilder
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable disable
namespace StardewValley.Internal;

/// <summary>Handles creating delegates for static methods by their string method names.</summary>
public static class StaticDelegateBuilder
{
  /// <summary>A cache of delegate resolution results, indexed by delegate type and then full method name.</summary>
  private static readonly Dictionary<Type, Dictionary<string, StaticDelegateBuilder.CachedDelegate>> CachedDelegates = new Dictionary<Type, Dictionary<string, StaticDelegateBuilder.CachedDelegate>>();

  /// <summary>Create a delegate for a static method.</summary>
  /// <typeparam name="TDelegate">The delegate type.</typeparam>
  /// <param name="fullMethodName">The full method name in the form <c>fullTypeName.methodName</c> (like <c>StardewValley.Object.OutputDeconstructor</c>).</param>
  /// <param name="createdDelegate">The created delegate instance, if valid.</param>
  /// <param name="error">An error phrase indicating why the delegate couldn't be created, if applicable.</param>
  /// <returns>Returns whether the delegate was successfully created.</returns>
  public static bool TryCreateDelegate<TDelegate>(
    string fullMethodName,
    out TDelegate createdDelegate,
    out string error)
    where TDelegate : Delegate
  {
    if (string.IsNullOrWhiteSpace(fullMethodName))
    {
      error = "the method name can't be empty";
      createdDelegate = default (TDelegate);
      return false;
    }
    Dictionary<string, StaticDelegateBuilder.CachedDelegate> dictionary1;
    if (!StaticDelegateBuilder.CachedDelegates.TryGetValue(typeof (TDelegate), out dictionary1))
      StaticDelegateBuilder.CachedDelegates[typeof (TDelegate)] = dictionary1 = new Dictionary<string, StaticDelegateBuilder.CachedDelegate>();
    StaticDelegateBuilder.CachedDelegate cachedDelegate1;
    if (!dictionary1.TryGetValue(fullMethodName, out cachedDelegate1))
    {
      string[] strArray = LegacyShims.SplitAndTrim(fullMethodName, ':');
      if (strArray.Length != 2)
      {
        error = "invalid method name format, expected a type full name and method separated with a colon (:)";
        createdDelegate = default (TDelegate);
        return false;
      }
      string str = strArray[0];
      string name = strArray[1];
      if (Game1.GameAssemblyName != "Stardew Valley" && str.Contains("Stardew Valley"))
      {
        string[] array = LegacyShims.SplitAndTrim(str, ',');
        if (ArgUtility.Get(array, 1) == "Stardew Valley")
        {
          array[1] = Game1.GameAssemblyName;
          str = string.Join(", ", array);
        }
      }
      Type type = Type.GetType(str);
      if (type == (Type) null)
      {
        error = $"could not find type '{str}'";
        createdDelegate = default (TDelegate);
        return false;
      }
      MethodInfo method1 = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      if (method1 == (MethodInfo) null)
      {
        error = $"could not find method '{name}' on type '{str}'";
        createdDelegate = default (TDelegate);
        return false;
      }
      if (!method1.IsStatic)
      {
        error = $"found method '{name}' on type '{str}', but the method isn't static";
        createdDelegate = default (TDelegate);
        return false;
      }
      try
      {
        createdDelegate = (TDelegate) Delegate.CreateDelegate(typeof (TDelegate), (object) null, method1);
        error = (string) null;
      }
      catch (ArgumentException ex)
      {
        MethodInfo method2 = typeof (TDelegate).GetMethod("Invoke");
        createdDelegate = default (TDelegate);
        error = $"failed to bind method '{fullMethodName}': it didn't match the expected signature {method2.ReturnType} method({string.Join(", ", ((IEnumerable<ParameterInfo>) method2.GetParameters()).Select<ParameterInfo, string>((Func<ParameterInfo, string>) (p => $"{p.ParameterType} {p.Name}")))})";
      }
      Dictionary<string, StaticDelegateBuilder.CachedDelegate> dictionary2 = dictionary1;
      string key = fullMethodName;
      cachedDelegate1 = new StaticDelegateBuilder.CachedDelegate((object) createdDelegate, error);
      StaticDelegateBuilder.CachedDelegate cachedDelegate2 = cachedDelegate1;
      dictionary2[key] = cachedDelegate2;
    }
    createdDelegate = (TDelegate) cachedDelegate1.CreatedDelegate;
    error = cachedDelegate1.Error;
    return (Delegate) createdDelegate != (Delegate) null;
  }

  /// <summary>A cached delegate creation.</summary>
  /// <summary>Construct an instance.</summary>
  /// <param name="createdDelegate">The created delegate instance, if valid.</param>
  /// <param name="error">An error phrase indicating why the delegate couldn't be created, if applicable.</param>
  private struct CachedDelegate(object createdDelegate, string error)
  {
    /// <summary>The created delegate instance, if valid.</summary>
    public readonly object CreatedDelegate = createdDelegate;
    /// <summary>An error phrase indicating why the delegate couldn't be created, if applicable.</summary>
    public readonly string Error = error;
  }
}
