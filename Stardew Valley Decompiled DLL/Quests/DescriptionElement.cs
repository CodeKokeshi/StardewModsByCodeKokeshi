// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.DescriptionElement
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Monsters;
using StardewValley.SaveSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Quests;

[XmlInclude(typeof (Item))]
[XmlInclude(typeof (Character))]
public class DescriptionElement : INetObject<NetFields>
{
  public static XmlSerializer serializer = SaveSerializer.GetSerializer(typeof (DescriptionElement));
  /// <summary>The translation key for the text to render.</summary>
  [XmlElement("xmlKey")]
  public string translationKey;
  /// <summary>The values to substitute for placeholders like <c>{0}</c> in the translation text.</summary>
  [XmlElement("param")]
  public List<object> substitutions;

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (DescriptionElement));

  /// <summary>Construct an instance for an empty text.</summary>
  public DescriptionElement()
    : this(string.Empty)
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="key">The translation key for the text to render.</param>
  /// <param name="substitutions">The values to substitute for placeholders like <c>{0}</c> in the translation text.</param>
  public DescriptionElement(string key, params object[] substitutions)
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this);
    this.translationKey = key;
    this.substitutions = new List<object>();
    this.substitutions.AddRange((IEnumerable<object>) substitutions);
  }

  public string loadDescriptionElement()
  {
    if (string.IsNullOrWhiteSpace(this.translationKey))
      return string.Empty;
    object[] array = this.substitutions.ToArray();
    for (int index = 0; index < array.Length; ++index)
    {
      switch (array[index])
      {
        case DescriptionElement descriptionElement2:
          array[index] = (object) descriptionElement2.loadDescriptionElement();
          break;
        case StardewValley.Object @object:
          array[index] = (object) ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId).DisplayName;
          break;
        case Monster monster:
          DescriptionElement descriptionElement1;
          if (monster.name.Value == "Frost Jelly")
          {
            descriptionElement1 = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13772", Array.Empty<object>());
            array[index] = (object) descriptionElement1.loadDescriptionElement();
          }
          else
          {
            descriptionElement1 = new DescriptionElement("Data\\Monsters:" + monster.name.Value, Array.Empty<object>());
            array[index] = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? (object) (((IEnumerable<string>) descriptionElement1.loadDescriptionElement().Split('/')).Last<string>() + "s") : (object) ((IEnumerable<string>) descriptionElement1.loadDescriptionElement().Split('/')).Last<string>();
          }
          array[index] = (object) ((IEnumerable<string>) descriptionElement1.loadDescriptionElement().Split('/')).Last<string>();
          break;
        case NPC npc:
          array[index] = (object) NPC.GetDisplayName(npc.name.Value);
          break;
      }
    }
    switch (array.Length)
    {
      case 0:
        return !this.translationKey.Contains("Dialogue.cs.7") && !this.translationKey.Contains("Dialogue.cs.8") ? Game1.content.LoadString(this.translationKey) : Game1.content.LoadString(this.translationKey).Replace("/", " ").TrimStart(' ');
      case 1:
        return Game1.content.LoadString(this.translationKey, array[0]);
      case 2:
        return Game1.content.LoadString(this.translationKey, array[0], array[1]);
      case 3:
        return Game1.content.LoadString(this.translationKey, array[0], array[1], array[2]);
      default:
        return Game1.content.LoadString(this.translationKey, array);
    }
  }
}
