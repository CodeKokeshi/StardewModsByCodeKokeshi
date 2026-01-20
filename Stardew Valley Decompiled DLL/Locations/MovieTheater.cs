// Decompiled with JetBrains decompiler
// Type: StardewValley.MovieInvitation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Network;

#nullable disable
namespace StardewValley;

public class MovieInvitation : INetObject<NetFields>
{
  private NetFarmerRef _farmer = new NetFarmerRef();
  protected NetString _invitedNPCName = new NetString();
  protected NetBool _fulfilled = new NetBool(false);

  public NetFields NetFields { get; } = new NetFields(nameof (MovieInvitation));

  public Farmer farmer
  {
    get => this._farmer.Value;
    set => this._farmer.Value = value;
  }

  public NPC invitedNPC
  {
    get => Game1.getCharacterFromName(this._invitedNPCName.Value);
    set
    {
      if (value == null)
        this._invitedNPCName.Set((string) null);
      else
        this._invitedNPCName.Set(value.name.Value);
    }
  }

  public bool fulfilled
  {
    get => this._fulfilled.Value;
    set => this._fulfilled.Set(value);
  }

  public MovieInvitation()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this._farmer.NetFields, "_farmer.NetFields").AddField((INetSerializable) this._invitedNPCName, nameof (_invitedNPCName)).AddField((INetSerializable) this._fulfilled, nameof (_fulfilled));
  }
}
