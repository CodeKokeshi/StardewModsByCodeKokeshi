// Decompiled with JetBrains decompiler
// Type: StardewValley.Proposal
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Network;

#nullable disable
namespace StardewValley;

public class Proposal : INetObject<NetFields>
{
  public readonly NetFarmerRef sender = new NetFarmerRef();
  public readonly NetFarmerRef receiver = new NetFarmerRef();
  public readonly NetEnum<ProposalType> proposalType = new NetEnum<ProposalType>(ProposalType.Gift);
  public readonly NetEnum<ProposalResponse> response = new NetEnum<ProposalResponse>(ProposalResponse.None);
  public readonly NetString responseMessageKey = new NetString();
  public readonly NetRef<Item> gift = new NetRef<Item>();
  public readonly NetBool canceled = new NetBool();
  public readonly NetBool cancelConfirmed = new NetBool();

  public NetFields NetFields { get; } = new NetFields(nameof (Proposal));

  public Proposal()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.sender.NetFields, "sender.NetFields").AddField((INetSerializable) this.receiver.NetFields, "receiver.NetFields").AddField((INetSerializable) this.proposalType, nameof (proposalType)).AddField((INetSerializable) this.response, nameof (response)).AddField((INetSerializable) this.responseMessageKey, nameof (responseMessageKey)).AddField((INetSerializable) this.gift, nameof (gift)).AddField((INetSerializable) this.canceled, nameof (canceled)).AddField((INetSerializable) this.cancelConfirmed, nameof (cancelConfirmed));
  }
}
