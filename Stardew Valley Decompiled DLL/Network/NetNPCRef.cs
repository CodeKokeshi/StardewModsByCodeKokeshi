// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetNPCRef
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;

#nullable disable
namespace StardewValley.Network;

public class NetNPCRef : INetObject<NetFields>
{
  private readonly NetGuid guid = new NetGuid();

  public NetFields NetFields { get; } = new NetFields(nameof (NetNPCRef));

  public NetNPCRef()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.guid, nameof (guid));
  }

  public NPC Get(GameLocation location)
  {
    NPC npc;
    return !(this.guid.Value != Guid.Empty) || location == null || !location.characters.TryGetValue(this.guid.Value, out npc) ? (NPC) null : npc;
  }

  public void Set(GameLocation location, NPC npc)
  {
    if (npc == null)
    {
      this.guid.Value = Guid.Empty;
    }
    else
    {
      Guid guid = location.characters.GuidOf(npc);
      this.guid.Value = !(guid == Guid.Empty) ? guid : throw new ArgumentException();
    }
  }

  public void Clear() => this.guid.Value = Guid.Empty;
}
