// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.HaveBuildingQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

/// <summary>A quest which completes when a building is constructed.</summary>
public class HaveBuildingQuest : Quest
{
  /// <summary>The building type to construct, matching the key in <see cref="M:StardewValley.DataLoader.Buildings(StardewValley.LocalizedContentManager)" />.</summary>
  [XmlElement("buildingType")]
  public readonly NetString buildingType = new NetString();

  /// <summary>Construct an instance.</summary>
  public HaveBuildingQuest() => this.questType.Value = 8;

  /// <summary>Construct an instance.</summary>
  /// <param name="buildingType">The building type to construct, matching the key in <see cref="M:StardewValley.DataLoader.Buildings(StardewValley.LocalizedContentManager)" />.</param>
  public HaveBuildingQuest(string buildingType)
    : this()
  {
    this.buildingType.Value = buildingType;
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.buildingType, "buildingType");
  }

  /// <inheritdoc />
  public override bool OnBuildingExists(string buildingType, bool probe = false)
  {
    bool flag = base.OnBuildingExists(buildingType, probe);
    if (!(buildingType == this.buildingType.Value))
      return flag;
    if (!probe)
      this.questComplete();
    return true;
  }
}
