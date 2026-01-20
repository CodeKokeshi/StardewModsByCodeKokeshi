// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetDancePartner
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;

#nullable disable
namespace StardewValley.Network;

public class NetDancePartner : INetObject<NetFields>
{
  private readonly NetFarmerRef farmer = new NetFarmerRef();
  private readonly NetString villager = new NetString();

  public Character Value
  {
    get => this.GetCharacter();
    set => this.SetCharacter(value);
  }

  public NetFields NetFields { get; } = new NetFields(nameof (NetDancePartner));

  public NetDancePartner()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.farmer.NetFields, "farmer.NetFields").AddField((INetSerializable) this.villager, nameof (villager));
  }

  public NetDancePartner(Farmer farmer) => this.farmer.Value = farmer;

  public NetDancePartner(string villagerName) => this.villager.Value = villagerName;

  public Character GetCharacter()
  {
    if (this.farmer.Value != null)
      return (Character) this.farmer.Value;
    return Game1.CurrentEvent != null && this.villager.Value != null ? (Character) Game1.CurrentEvent.getActorByName(this.villager.Value) : (Character) null;
  }

  public void SetCharacter(Character value)
  {
    switch (value)
    {
      case null:
        this.farmer.Value = (Farmer) null;
        this.villager.Value = (string) null;
        break;
      case Farmer farmer:
        this.farmer.Value = farmer;
        this.villager.Value = (string) null;
        break;
      case NPC npc:
        if (!npc.IsVillager)
          throw new ArgumentException(value.ToString());
        this.farmer.Value = (Farmer) null;
        this.villager.Value = npc.Name;
        break;
      default:
        throw new ArgumentException(value.ToString());
    }
  }

  public NPC TryGetVillager()
  {
    if (this.farmer.Value != null)
      return (NPC) null;
    return Game1.CurrentEvent != null && this.villager.Value != null ? Game1.CurrentEvent.getActorByName(this.villager.Value) : (NPC) null;
  }

  public Farmer TryGetFarmer() => this.farmer.Value;

  public bool IsFarmer() => this.TryGetFarmer() != null;

  public bool IsVillager() => this.TryGetVillager() != null;

  public Gender GetGender()
  {
    if (this.IsFarmer())
      return this.TryGetFarmer().Gender;
    return this.IsVillager() ? this.TryGetVillager().Gender : Gender.Undefined;
  }
}
