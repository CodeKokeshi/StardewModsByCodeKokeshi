// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ParryEventArgs
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.IO;

#nullable disable
namespace StardewValley.Monsters;

internal class ParryEventArgs : NetEventArg
{
  public int damage;
  private long farmerId;

  public Farmer who
  {
    get => Game1.GetPlayer(this.farmerId) ?? Game1.MasterPlayer;
    set => this.farmerId = value.UniqueMultiplayerID;
  }

  public ParryEventArgs()
  {
  }

  public ParryEventArgs(int damage, Farmer who)
  {
    this.damage = damage;
    this.who = who;
  }

  public void Read(BinaryReader reader)
  {
    this.damage = reader.ReadInt32();
    this.farmerId = reader.ReadInt64();
  }

  public void Write(BinaryWriter writer)
  {
    writer.Write(this.damage);
    writer.Write(this.farmerId);
  }
}
