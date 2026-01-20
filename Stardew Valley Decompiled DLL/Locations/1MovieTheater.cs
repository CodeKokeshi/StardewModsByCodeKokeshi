// Decompiled with JetBrains decompiler
// Type: StardewValley.StartMovieEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley;

public class StartMovieEvent : NetEventArg
{
  public long uid;
  public List<List<Character>> playerGroups;
  public List<List<Character>> npcGroups;

  public StartMovieEvent()
  {
  }

  public StartMovieEvent(
    long farmer_uid,
    List<List<Character>> player_groups,
    List<List<Character>> npc_groups)
  {
    this.uid = farmer_uid;
    this.playerGroups = player_groups;
    this.npcGroups = npc_groups;
  }

  public void Read(BinaryReader reader)
  {
    this.uid = reader.ReadInt64();
    this.playerGroups = this.ReadCharacterList(reader);
    this.npcGroups = this.ReadCharacterList(reader);
  }

  public void Write(BinaryWriter writer)
  {
    writer.Write(this.uid);
    this.WriteCharacterList(writer, this.playerGroups);
    this.WriteCharacterList(writer, this.npcGroups);
  }

  public List<List<Character>> ReadCharacterList(BinaryReader reader)
  {
    List<List<Character>> characterListList = new List<List<Character>>();
    int num1 = reader.ReadInt32();
    for (int index1 = 0; index1 < num1; ++index1)
    {
      List<Character> characterList = new List<Character>();
      int num2 = reader.ReadInt32();
      for (int index2 = 0; index2 < num2; ++index2)
      {
        Character character = reader.ReadInt32() == 1 ? (Character) Game1.GetPlayer(reader.ReadInt64(), true) ?? (Character) Game1.MasterPlayer : (Character) Game1.getCharacterFromName(reader.ReadString());
        characterList.Add(character);
      }
      characterListList.Add(characterList);
    }
    return characterListList;
  }

  public void WriteCharacterList(BinaryWriter writer, List<List<Character>> group_list)
  {
    writer.Write(group_list.Count);
    foreach (List<Character> group in group_list)
    {
      writer.Write(group.Count);
      foreach (Character character in group)
      {
        if (character is Farmer farmer)
        {
          writer.Write(1);
          writer.Write(farmer.UniqueMultiplayerID);
        }
        else
        {
          writer.Write(0);
          writer.Write(character.Name);
        }
      }
    }
  }
}
