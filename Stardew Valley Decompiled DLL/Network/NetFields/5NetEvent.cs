// Decompiled with JetBrains decompiler
// Type: Netcode.NetEvent1Field`2
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public class NetEvent1Field<T, TField> : AbstractNetEvent1<T> where TField : NetField<T, TField>, new()
{
  protected override T readEventArg(BinaryReader reader, NetVersion version)
  {
    TField field = new TField();
    field.ReadFull(reader, version);
    return field.Value;
  }

  protected override void writeEventArg(BinaryWriter writer, T eventArg)
  {
    TField field = new TField();
    field.Value = eventArg;
    field.WriteFull(writer);
  }
}
