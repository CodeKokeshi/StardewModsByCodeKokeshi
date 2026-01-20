// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerRoot
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.SaveSerialization;

#nullable disable
namespace StardewValley.Network;

public class NetFarmerRoot : NetRoot<Farmer>
{
  public NetFarmerRoot() => this.Serializer = SaveSerializer.GetSerializer(typeof (Farmer));

  public NetFarmerRoot(Farmer value)
    : base(value)
  {
    this.Serializer = SaveSerializer.GetSerializer(typeof (Farmer));
  }

  public override NetRoot<Farmer> Clone()
  {
    NetRoot<Farmer> netRoot = base.Clone();
    if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && netRoot.Value != null)
      netRoot.Value.teamRoot = Game1.serverHost.Value.teamRoot;
    return netRoot;
  }
}
