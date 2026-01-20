// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetBundles
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable disable
namespace StardewValley.Network;

public class NetBundles : 
  NetDictionary<int, bool[], NetArray<bool, NetBool>, SerializableDictionary<int, bool[]>, NetBundles>
{
  protected override int ReadKey(BinaryReader reader)
  {
    int num = reader.ReadInt32();
    if (!(Game1.activeClickableMenu is JunimoNoteMenu activeClickableMenu))
      return num;
    activeClickableMenu.bundlesChanged = true;
    return num;
  }

  protected override void WriteKey(BinaryWriter writer, int key) => writer.Write(key);

  protected override void setFieldValue(NetArray<bool, NetBool> field, int key, bool[] value)
  {
    field.Set((IList<bool>) value);
  }

  protected override bool[] getFieldValue(NetArray<bool, NetBool> field) => field.ToArray<bool>();

  protected override bool[] getFieldTargetValue(NetArray<bool, NetBool> field)
  {
    return field.ToArray<bool>();
  }
}
