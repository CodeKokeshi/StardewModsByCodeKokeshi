// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetCharacterRef
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;

#nullable disable
namespace StardewValley.Network;

public class NetCharacterRef : INetObject<NetFields>
{
  private readonly NetNPCRef npc = new NetNPCRef();
  private readonly NetFarmerRef farmer = new NetFarmerRef();

  public NetFields NetFields { get; } = new NetFields(nameof (NetCharacterRef));

  public NetCharacterRef()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.npc.NetFields, "npc.NetFields").AddField((INetSerializable) this.farmer.NetFields, "farmer.NetFields");
  }

  public Character Get(GameLocation location)
  {
    return (Character) this.npc.Get(location) ?? (Character) this.farmer.Value;
  }

  public void Set(GameLocation location, Character character)
  {
    switch (character)
    {
      case NPC npc:
        this.npc.Set(location, npc);
        this.farmer.Value = (Farmer) null;
        break;
      case Farmer farmer:
        this.npc.Clear();
        this.farmer.Value = farmer;
        break;
      default:
        throw new ArgumentException();
    }
  }

  public void Clear()
  {
    this.npc.Clear();
    this.farmer.Value = (Farmer) null;
  }
}
