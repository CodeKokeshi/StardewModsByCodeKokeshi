// Decompiled with JetBrains decompiler
// Type: Netcode.INetSerializable
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.IO;

#nullable disable
namespace Netcode;

public interface INetSerializable
{
  uint DirtyTick { get; set; }

  bool Dirty { get; }

  bool NeedsTick { get; set; }

  bool ChildNeedsTick { get; set; }

  /// <summary>A name for this net field, used for troubleshooting network sync.</summary>
  string Name { get; set; }

  INetSerializable Parent { get; set; }

  INetRoot Root { get; }

  void MarkDirty();

  void MarkClean();

  bool Tick();

  void Read(BinaryReader reader, NetVersion version);

  void Write(BinaryWriter writer);

  void ReadFull(BinaryReader reader, NetVersion version);

  void WriteFull(BinaryWriter writer);
}
