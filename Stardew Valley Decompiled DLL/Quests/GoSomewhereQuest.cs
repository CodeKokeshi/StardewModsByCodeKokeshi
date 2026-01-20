// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.GoSomewhereQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

public class GoSomewhereQuest : Quest
{
  [XmlElement("whereToGo")]
  public readonly NetString whereToGo = new NetString();

  public GoSomewhereQuest()
  {
  }

  public GoSomewhereQuest(string where) => this.whereToGo.Value = where;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.whereToGo, "whereToGo");
  }

  /// <inheritdoc />
  public override bool OnWarped(GameLocation location, bool probe = false)
  {
    bool flag = base.OnWarped(location, probe);
    if (!(location?.NameOrUniqueName == this.whereToGo.Value))
      return flag;
    if (!probe)
      this.questComplete();
    return true;
  }
}
