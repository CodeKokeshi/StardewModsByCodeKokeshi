// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.NetLeaderboardsEntry
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;

#nullable disable
namespace StardewValley.Minigames;

public class NetLeaderboardsEntry : INetObject<NetFields>
{
  public readonly NetString name = new NetString("");
  public readonly NetInt score = new NetInt(0);

  public NetFields NetFields { get; } = new NetFields(nameof (NetLeaderboardsEntry));

  public void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.name, "name").AddField((INetSerializable) this.score, "score");
  }

  public NetLeaderboardsEntry() => this.InitNetFields();

  public NetLeaderboardsEntry(string new_name, int new_score)
  {
    this.InitNetFields();
    this.name.Value = new_name;
    this.score.Value = new_score;
  }
}
