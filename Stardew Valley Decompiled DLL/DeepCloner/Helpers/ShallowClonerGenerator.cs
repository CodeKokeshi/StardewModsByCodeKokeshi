// Decompiled with JetBrains decompiler
// Type: Force.DeepCloner.Helpers.ShallowClonerGenerator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace Force.DeepCloner.Helpers;

internal static class ShallowClonerGenerator
{
  public static T CloneObject<T>(T obj)
  {
    if ((object) obj is ValueType)
      return typeof (T) == obj.GetType() ? obj : (T) ShallowObjectCloner.CloneObject((object) obj);
    if ((object) obj == null)
      return (T) null;
    return DeepClonerSafeTypes.CanReturnSameObject(obj.GetType()) ? obj : (T) ShallowObjectCloner.CloneObject((object) obj);
  }
}
