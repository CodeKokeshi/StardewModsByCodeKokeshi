// Decompiled with JetBrains decompiler
// Type: StardewValley.Warp
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Warp : INetObject<NetFields>
{
  [XmlElement("x")]
  private readonly NetInt x = new NetInt();
  [XmlElement("y")]
  private readonly NetInt y = new NetInt();
  [XmlElement("targetX")]
  private readonly NetInt targetX = new NetInt();
  [XmlElement("targetY")]
  private readonly NetInt targetY = new NetInt();
  [XmlElement("flipFarmer")]
  public readonly NetBool flipFarmer = new NetBool();
  [XmlElement("targetName")]
  private readonly NetString targetName = new NetString();
  [XmlElement("npcOnly")]
  public readonly NetBool npcOnly = new NetBool();

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (Warp));

  public int X => this.x.Value;

  public int Y => this.y.Value;

  public int TargetX
  {
    get => this.targetX.Value;
    set => this.targetX.Value = value;
  }

  public int TargetY
  {
    get => this.targetY.Value;
    set => this.targetY.Value = value;
  }

  public string TargetName
  {
    get => this.targetName.Value;
    set => this.targetName.Value = value;
  }

  public Warp()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.x, "this.x").AddField((INetSerializable) this.y, "this.y").AddField((INetSerializable) this.targetX, "this.targetX").AddField((INetSerializable) this.targetY, "this.targetY").AddField((INetSerializable) this.targetName, "this.targetName").AddField((INetSerializable) this.flipFarmer, "this.flipFarmer").AddField((INetSerializable) this.npcOnly, "this.npcOnly");
  }

  public Warp(
    int x,
    int y,
    string targetName,
    int targetX,
    int targetY,
    bool flipFarmer,
    bool npcOnly = false)
    : this()
  {
    this.x.Value = x;
    this.y.Value = y;
    this.targetX.Value = targetX;
    this.targetY.Value = targetY;
    this.targetName.Value = targetName;
    this.flipFarmer.Value = flipFarmer;
    this.npcOnly.Value = npcOnly;
  }
}
