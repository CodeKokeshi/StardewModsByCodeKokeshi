// Decompiled with JetBrains decompiler
// Type: StardewValley.Mods.ModDataDictionary
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Network;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Mods;

public class ModDataDictionary : NetStringDictionary<string, NetString>
{
  public ModDataDictionary() => this.InterpolationWait = false;

  public virtual void SetFromSerialization(ModDataDictionary source)
  {
    this.Clear();
    if (source == null)
      return;
    foreach (string key in source.Keys)
      this[key] = source[key];
  }

  public ModDataDictionary GetForSerialization()
  {
    return Game1.game1 != null && Game1.game1.IsSaving && this.Length == 0 ? (ModDataDictionary) null : this;
  }

  public void CopyFrom(ModDataDictionary dict)
  {
    this.CopyFrom((IEnumerable<KeyValuePair<string, string>>) dict.Pairs);
  }
}
