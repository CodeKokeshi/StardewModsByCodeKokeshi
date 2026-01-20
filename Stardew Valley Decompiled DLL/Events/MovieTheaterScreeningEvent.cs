// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.MovieTheaterScreeningEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace StardewValley.Events;

/// <summary>Generates the event that plays when watching a movie at the <see cref="T:StardewValley.Locations.MovieTheater" />.</summary>
public class MovieTheaterScreeningEvent
{
  public int currentResponse;
  public 
  #nullable disable
  List<List<Character>> playerAndGuestAudienceGroups;
  public Dictionary<int, Character> _responseOrder = new Dictionary<int, Character>();
  protected Dictionary<Character, Character> _whiteListDependencyLookup;
  protected Dictionary<Character, string> _characterResponses;
  public MovieData movieData;
  protected List<Farmer> _farmers;
  protected Dictionary<Character, MovieConcession> _concessionsData;

  public Event getMovieEvent(
    string movieId,
    List<List<Character>> player_and_guest_audience_groups,
    List<List<Character>> npcOnlyAudienceGroups,
    Dictionary<Character, MovieConcession> concessions_data = null)
  {
    this._concessionsData = concessions_data;
    this._responseOrder = new Dictionary<int, Character>();
    this._whiteListDependencyLookup = new Dictionary<Character, Character>();
    this._characterResponses = new Dictionary<Character, string>();
    this.movieData = MovieTheater.GetMovieDataById()[movieId];
    this.playerAndGuestAudienceGroups = player_and_guest_audience_groups;
    this.currentResponse = 0;
    StringBuilder sb = new StringBuilder();
    Random theaterRandom = Utility.CreateDaySaveRandom();
    sb.Append("movieScreenAmbience/-2000 -2000/");
    string str1 = "farmer" + Utility.getFarmerNumberFromFarmer(Game1.player).ToString();
    string str2 = "";
    bool flag1 = false;
    foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
    {
      if (guestAudienceGroup.Contains((Character) Game1.player))
      {
        for (int index = 0; index < guestAudienceGroup.Count; ++index)
        {
          if (!(guestAudienceGroup[index] is Farmer))
          {
            str2 = guestAudienceGroup[index].name.Value;
            flag1 = true;
            break;
          }
        }
      }
    }
    this._farmers = new List<Farmer>();
    foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
    {
      foreach (Character character in guestAudienceGroup)
      {
        if (character is Farmer farmer && !this._farmers.Contains(farmer))
          this._farmers.Add(farmer);
      }
    }
    List<Character> list = this.playerAndGuestAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).ToList<Character>();
    if (list.Count <= 12)
      list.AddRange((IEnumerable<Character>) npcOnlyAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).ToList<Character>());
    bool flag2 = true;
    foreach (Character character in list)
    {
      if (character != null)
      {
        if (!flag2)
          sb.Append(' ');
        if (character is Farmer who)
          sb.Append("farmer").Append(Utility.getFarmerNumberFromFarmer(who));
        else
          sb.Append(character.name.Value);
        sb.Append(" -1000 -1000 0");
        flag2 = false;
      }
    }
    sb.Append("/changeToTemporaryMap MovieTheaterScreen false/specificTemporarySprite movieTheater_setup/ambientLight 0 0 0/");
    string[] strArray1 = new string[8];
    string[] strArray2 = new string[6];
    string[] strArray3 = new string[4];
    this.playerAndGuestAudienceGroups = this.playerAndGuestAudienceGroups.OrderBy<List<Character>, int>((Func<List<Character>, int>) (x => theaterRandom.Next())).ToList<List<Character>>();
    int num1 = theaterRandom.Next(8 - Math.Min(this.playerAndGuestAudienceGroups.SelectMany<List<Character>, Character>((Func<List<Character>, IEnumerable<Character>>) (x => (IEnumerable<Character>) x)).Count<Character>(), 8) + 1);
    int index1 = 0;
    int numberFromFarmer;
    if (this.playerAndGuestAudienceGroups.Count > 0)
    {
      for (int index2 = 0; index2 < 8; ++index2)
      {
        int num2 = (index2 + num1) % 8;
        if (this.playerAndGuestAudienceGroups[index1].Count == 2 && (num2 == 3 || num2 == 7))
        {
          ++index2;
          num2 = (num2 + 1) % 8;
        }
        for (int index3 = 0; index3 < this.playerAndGuestAudienceGroups[index1].Count && num2 + index3 < strArray1.Length; ++index3)
        {
          string[] strArray4 = strArray1;
          int index4 = num2 + index3;
          string str3;
          if (!(this.playerAndGuestAudienceGroups[index1][index3] is Farmer))
          {
            str3 = this.playerAndGuestAudienceGroups[index1][index3].name.Value;
          }
          else
          {
            numberFromFarmer = Utility.getFarmerNumberFromFarmer(this.playerAndGuestAudienceGroups[index1][index3] as Farmer);
            str3 = "farmer" + numberFromFarmer.ToString();
          }
          strArray4[index4] = str3;
          if (index3 > 0)
            ++index2;
        }
        ++index1;
        if (index1 >= this.playerAndGuestAudienceGroups.Count)
          break;
      }
    }
    else
      Game1.log.Warn("The movie audience somehow has no players. This is likely a bug.");
    bool flag3 = false;
    if (index1 < this.playerAndGuestAudienceGroups.Count)
    {
      int num3 = 0;
      for (int index5 = 0; index5 < 4; ++index5)
      {
        int num4 = (index5 + num3) % 4;
        for (int index6 = 0; index6 < this.playerAndGuestAudienceGroups[index1].Count && num4 + index6 < strArray3.Length; ++index6)
        {
          string[] strArray5 = strArray3;
          int index7 = num4 + index6;
          string str4;
          if (!(this.playerAndGuestAudienceGroups[index1][index6] is Farmer))
          {
            str4 = this.playerAndGuestAudienceGroups[index1][index6].name.Value;
          }
          else
          {
            numberFromFarmer = Utility.getFarmerNumberFromFarmer(this.playerAndGuestAudienceGroups[index1][index6] as Farmer);
            str4 = "farmer" + numberFromFarmer.ToString();
          }
          strArray5[index7] = str4;
          if (index6 > 0)
            ++index5;
        }
        ++index1;
        if (index1 >= this.playerAndGuestAudienceGroups.Count)
          break;
      }
      if (index1 < this.playerAndGuestAudienceGroups.Count)
      {
        flag3 = true;
        int num5 = 0;
        for (int index8 = 0; index8 < 6; ++index8)
        {
          int num6 = (index8 + num5) % 6;
          if (this.playerAndGuestAudienceGroups[index1].Count == 2 && num6 == 2)
          {
            ++index8;
            num6 = (num6 + 1) % 8;
          }
          for (int index9 = 0; index9 < this.playerAndGuestAudienceGroups[index1].Count && num6 + index9 < strArray2.Length; ++index9)
          {
            string[] strArray6 = strArray2;
            int index10 = num6 + index9;
            string str5;
            if (!(this.playerAndGuestAudienceGroups[index1][index9] is Farmer))
            {
              str5 = this.playerAndGuestAudienceGroups[index1][index9].name.Value;
            }
            else
            {
              numberFromFarmer = Utility.getFarmerNumberFromFarmer(this.playerAndGuestAudienceGroups[index1][index9] as Farmer);
              str5 = "farmer" + numberFromFarmer.ToString();
            }
            strArray6[index10] = str5;
            if (index9 > 0)
              ++index8;
          }
          ++index1;
          if (index1 >= this.playerAndGuestAudienceGroups.Count)
            break;
        }
      }
    }
    if (!flag3)
    {
      for (int index11 = 0; index11 < npcOnlyAudienceGroups.Count; ++index11)
      {
        int num7 = theaterRandom.Next(3 - npcOnlyAudienceGroups[index11].Count + 1) + index11 * 3;
        for (int index12 = 0; index12 < npcOnlyAudienceGroups[index11].Count; ++index12)
          strArray2[num7 + index12] = npcOnlyAudienceGroups[index11][index12].name.Value;
      }
    }
    int num8 = 0;
    int num9 = 0;
    for (int index13 = 0; index13 < strArray1.Length; ++index13)
    {
      if (!string.IsNullOrEmpty(strArray1[index13]) && strArray1[index13] != str1 && strArray1[index13] != str2)
      {
        ++num8;
        if (num8 >= 2)
        {
          ++num9;
          Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index13);
          sb.Append("warp ").Append(strArray1[index13]).Append(' ').Append(seatTileFromIndex.X).Append(' ').Append(seatTileFromIndex.Y).Append("/positionOffset ").Append(strArray1[index13]).Append(" 0 -10/");
          if (num9 == 2)
          {
            num9 = 0;
            if (theaterRandom.NextBool() && strArray1[index13] != str2 && strArray1[index13 - 1] != str2 && strArray1[index13 - 1] != null)
            {
              sb.Append("faceDirection ").Append(strArray1[index13]).Append(" 3 true/");
              sb.Append("faceDirection ").Append(strArray1[index13 - 1]).Append(" 1 true/");
            }
          }
        }
      }
    }
    int num10 = 0;
    int num11 = 0;
    for (int index14 = 0; index14 < strArray2.Length; ++index14)
    {
      if (!string.IsNullOrEmpty(strArray2[index14]) && strArray2[index14] != str1 && strArray2[index14] != str2)
      {
        ++num10;
        if (num10 >= 2)
        {
          ++num11;
          Point seatTileFromIndex = this.getMidRowSeatTileFromIndex(index14);
          sb.Append("warp ").Append(strArray2[index14]).Append(' ').Append(seatTileFromIndex.X).Append(' ').Append(seatTileFromIndex.Y).Append("/positionOffset ").Append(strArray2[index14]).Append(" 0 -10/");
          if (num11 == 2)
          {
            num11 = 0;
            if (index14 != 3 && theaterRandom.NextBool() && strArray2[index14 - 1] != null)
            {
              sb.Append("faceDirection ").Append(strArray2[index14]).Append(" 3 true/");
              sb.Append("faceDirection ").Append(strArray2[index14 - 1]).Append(" 1 true/");
            }
          }
        }
      }
    }
    int num12 = 0;
    int num13 = 0;
    for (int index15 = 0; index15 < strArray3.Length; ++index15)
    {
      if (!string.IsNullOrEmpty(strArray3[index15]) && strArray3[index15] != str1 && strArray3[index15] != str2)
      {
        ++num12;
        if (num12 >= 2)
        {
          ++num13;
          Point seatTileFromIndex = this.getFrontRowSeatTileFromIndex(index15);
          sb.Append("warp ").Append(strArray3[index15]).Append(' ').Append(seatTileFromIndex.X).Append(' ').Append(seatTileFromIndex.Y).Append("/positionOffset ").Append(strArray3[index15]).Append(" 0 -10/");
          if (num13 == 2)
          {
            num13 = 0;
            if (theaterRandom.NextBool() && strArray3[index15 - 1] != null)
            {
              sb.Append("faceDirection ").Append(strArray3[index15]).Append(" 3 true/");
              sb.Append("faceDirection ").Append(strArray3[index15 - 1]).Append(" 1 true/");
            }
          }
        }
      }
    }
    Point point = new Point(1, 15);
    int num14 = 0;
    for (int index16 = 0; index16 < strArray1.Length; ++index16)
    {
      if (!string.IsNullOrEmpty(strArray1[index16]) && strArray1[index16] != str1 && strArray1[index16] != str2)
      {
        Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index16);
        if (num14 == 1)
          sb.Append("warp ").Append(strArray1[index16]).Append(' ').Append(seatTileFromIndex.X - 1).Append(" 10").Append("/advancedMove ").Append(strArray1[index16]).Append(" false 1 ").Append(200).Append(" 1 0 4 1000/").Append("positionOffset ").Append(strArray1[index16]).Append(" 0 -10/");
        else
          sb.Append("warp ").Append(strArray1[index16]).Append(" 1 12").Append("/advancedMove ").Append(strArray1[index16]).Append(" false 1 200 ").Append("0 -2 ").Append(seatTileFromIndex.X - 1).Append(" 0 4 1000/").Append("positionOffset ").Append(strArray1[index16]).Append(" 0 -10/");
        ++num14;
      }
      if (num14 >= 2)
        break;
    }
    int num15 = 0;
    for (int index17 = 0; index17 < strArray2.Length; ++index17)
    {
      if (!string.IsNullOrEmpty(strArray2[index17]) && strArray2[index17] != str1 && strArray2[index17] != str2)
      {
        Point seatTileFromIndex = this.getMidRowSeatTileFromIndex(index17);
        if (num15 == 1)
          sb.Append("warp ").Append(strArray2[index17]).Append(' ').Append(seatTileFromIndex.X - 1).Append(" 8").Append("/advancedMove ").Append(strArray2[index17]).Append(" false 1 ").Append(400).Append(" 1 0 4 1000/");
        else
          sb.Append("warp ").Append(strArray2[index17]).Append(" 2 9").Append("/advancedMove ").Append(strArray2[index17]).Append(" false 1 300 ").Append("0 -1 ").Append(seatTileFromIndex.X - 2).Append(" 0 4 1000/");
        ++num15;
      }
      if (num15 >= 2)
        break;
    }
    int num16 = 0;
    for (int index18 = 0; index18 < strArray3.Length; ++index18)
    {
      if (!string.IsNullOrEmpty(strArray3[index18]) && strArray3[index18] != str1 && strArray3[index18] != str2)
      {
        Point seatTileFromIndex = this.getFrontRowSeatTileFromIndex(index18);
        if (num16 == 1)
          sb.Append("warp ").Append(strArray3[index18]).Append(' ').Append(seatTileFromIndex.X - 1).Append(" 6").Append("/advancedMove ").Append(strArray3[index18]).Append(" false 1 ").Append(400).Append(" 1 0 4 1000/");
        else
          sb.Append("warp ").Append(strArray3[index18]).Append(" 3 7").Append("/advancedMove ").Append(strArray3[index18]).Append(" false 1 300 ").Append("0 -1 ").Append(seatTileFromIndex.X - 3).Append(" 0 4 1000/");
        ++num16;
      }
      if (num16 >= 2)
        break;
    }
    sb.Append("viewport 6 8 true/pause 500/");
    for (int index19 = 0; index19 < strArray1.Length; ++index19)
    {
      if (!string.IsNullOrEmpty(strArray1[index19]))
      {
        Point seatTileFromIndex = this.getBackRowSeatTileFromIndex(index19);
        if (strArray1[index19] == str1 || strArray1[index19] == str2)
          sb.Append("warp ").Append(strArray1[index19]).Append(' ').Append(point.X).Append(' ').Append(point.Y).Append("/advancedMove ").Append(strArray1[index19]).Append(" false 0 -5 ").Append(seatTileFromIndex.X - point.X).Append(" 0 4 1000/").Append("pause ").Append(1000).Append("/");
      }
    }
    for (int index20 = 0; index20 < strArray2.Length; ++index20)
    {
      if (!string.IsNullOrEmpty(strArray2[index20]))
      {
        Point seatTileFromIndex = this.getMidRowSeatTileFromIndex(index20);
        if (strArray2[index20] == str1 || strArray2[index20] == str2)
          sb.Append("warp ").Append(strArray2[index20]).Append(' ').Append(point.X).Append(' ').Append(point.Y).Append("/advancedMove ").Append(strArray2[index20]).Append(" false 0 -7 ").Append(seatTileFromIndex.X - point.X).Append(" 0 4 1000/").Append("pause ").Append(1000).Append("/");
      }
    }
    for (int index21 = 0; index21 < strArray3.Length; ++index21)
    {
      if (!string.IsNullOrEmpty(strArray3[index21]))
      {
        Point seatTileFromIndex = this.getFrontRowSeatTileFromIndex(index21);
        if (strArray3[index21] == str1 || strArray3[index21] == str2)
          sb.Append("warp ").Append(strArray3[index21]).Append(' ').Append(point.X).Append(' ').Append(point.Y).Append("/advancedMove ").Append(strArray3[index21]).Append(" false 0 -7 1 0 0 -1 1 0 0 -1 ").Append(seatTileFromIndex.X - 3).Append(" 0 4 1000/").Append("pause ").Append(1000).Append("/");
      }
    }
    sb.Append("pause 3000");
    if (flag1)
      sb.Append("/proceedPosition ").Append(str2);
    sb.Append("/pause 1000");
    if (!flag1)
      sb.Append("/proceedPosition farmer");
    sb.Append("/waitForAllStationary/pause 100");
    foreach (Character c in list)
    {
      string eventName = MovieTheaterScreeningEvent.getEventName(c);
      if (eventName != str1 && eventName != str2)
      {
        if (c is Farmer)
          sb.Append("/faceDirection ").Append(eventName).Append(" 0 true/positionOffset ").Append(eventName).Append(" 0 42 true");
        else
          sb.Append("/faceDirection ").Append(eventName).Append(" 0 true/positionOffset ").Append(eventName).Append(" 0 12 true");
        if (theaterRandom.NextDouble() < 0.2)
          sb.Append("/pause 100");
      }
    }
    sb.Append("/positionOffset ").Append(str1).Append(" 0 32");
    if (flag1)
      sb.Append("/positionOffset ").Append(str2).Append(" 0 8");
    sb.Append("/ambientLight 210 210 120 true/pause 500/viewport move 0 -1 4000/pause 5000");
    List<Character> characterList = new List<Character>();
    foreach (List<Character> guestAudienceGroup in this.playerAndGuestAudienceGroups)
    {
      foreach (Character character in guestAudienceGroup)
      {
        if (!(character is Farmer) && !characterList.Contains(character))
          characterList.Add(character);
      }
    }
    for (int index22 = 0; index22 < characterList.Count; ++index22)
    {
      int index23 = theaterRandom.Next(characterList.Count);
      Character character = characterList[index22];
      characterList[index22] = characterList[index23];
      characterList[index23] = character;
    }
    int key1 = 0;
    foreach (MovieScene scene in this.movieData.Scenes)
    {
      if (scene.ResponsePoint != null)
      {
        bool flag4 = false;
        for (int index24 = 0; index24 < characterList.Count; ++index24)
        {
          MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(characterList[index24] as NPC);
          if (reactionsForCharacter != null)
          {
            foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
            {
              if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(characterList[index24] as NPC)) && reaction.SpecialResponses?.DuringMovie != null && (reaction.SpecialResponses.DuringMovie.ResponsePoint == scene.ResponsePoint || reaction.Whitelist.Count > 0))
              {
                if (!this._whiteListDependencyLookup.ContainsKey(characterList[index24]))
                {
                  this._responseOrder[key1] = characterList[index24];
                  if (reaction.Whitelist != null)
                  {
                    for (int index25 = 0; index25 < reaction.Whitelist.Count; ++index25)
                    {
                      Character characterFromName = (Character) Game1.getCharacterFromName(reaction.Whitelist[index25]);
                      if (characterFromName != null)
                      {
                        this._whiteListDependencyLookup[characterFromName] = characterList[index24];
                        foreach (int key2 in this._responseOrder.Keys)
                        {
                          if (this._responseOrder[key2] == characterFromName)
                            this._responseOrder.Remove(key2);
                        }
                      }
                    }
                  }
                }
                characterList.RemoveAt(index24);
                --index24;
                flag4 = true;
                break;
              }
            }
            if (flag4)
              break;
          }
        }
        if (!flag4)
        {
          for (int index26 = 0; index26 < characterList.Count; ++index26)
          {
            MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(characterList[index26] as NPC);
            if (reactionsForCharacter != null)
            {
              foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
              {
                if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(characterList[index26] as NPC)) && reaction.SpecialResponses?.DuringMovie != null && reaction.SpecialResponses.DuringMovie.ResponsePoint == key1.ToString())
                {
                  if (!this._whiteListDependencyLookup.ContainsKey(characterList[index26]))
                  {
                    this._responseOrder[key1] = characterList[index26];
                    if (reaction.Whitelist != null)
                    {
                      for (int index27 = 0; index27 < reaction.Whitelist.Count; ++index27)
                      {
                        Character characterFromName = (Character) Game1.getCharacterFromName(reaction.Whitelist[index27]);
                        if (characterFromName != null)
                        {
                          this._whiteListDependencyLookup[characterFromName] = characterList[index26];
                          foreach (int key3 in this._responseOrder.Keys)
                          {
                            if (this._responseOrder[key3] == characterFromName)
                              this._responseOrder.Remove(key3);
                          }
                        }
                      }
                    }
                  }
                  characterList.RemoveAt(index26);
                  --index26;
                  flag4 = true;
                  break;
                }
              }
              if (flag4)
                break;
            }
          }
        }
        ++key1;
      }
    }
    int key4 = 0;
    for (int index28 = 0; index28 < characterList.Count; ++index28)
    {
      if (!this._whiteListDependencyLookup.ContainsKey(characterList[index28]))
      {
        while (this._responseOrder.ContainsKey(key4))
          ++key4;
        this._responseOrder[key4] = characterList[index28];
        ++key4;
      }
    }
    foreach (MovieScene scene in this.movieData.Scenes)
      this._ParseScene(sb, scene);
    while (this.currentResponse < this._responseOrder.Count)
      this._ParseResponse(sb);
    sb.Append("/stopMusic");
    sb.Append("/fade/viewport -1000 -1000");
    sb.Append("/pause 500/message \"").Append(Game1.content.LoadString("Strings\\Locations:Theater_MovieEnd")).Append("\"/pause 500");
    sb.Append("/requestMovieEnd");
    return new Event(sb.ToString(), (string) null, "MovieTheaterScreening");
  }

  protected void _ParseScene(StringBuilder sb, MovieScene scene)
  {
    if (!string.IsNullOrWhiteSpace(scene.Sound))
      sb.Append("/playSound ").Append(scene.Sound);
    if (!string.IsNullOrWhiteSpace(scene.Music))
      sb.Append("/playMusic ").Append(scene.Music);
    if (scene.MessageDelay > 0)
      sb.Append("/pause ").Append(scene.MessageDelay);
    if (scene.Image >= 0)
    {
      sb.Append("/specificTemporarySprite movieTheater_screen ").Append(this.movieData.Id).Append(' ').Append(scene.Image).Append(' ').Append(scene.Shake);
      if (this.movieData.Texture != null)
        sb.Append(" \"").Append(ArgUtility.EscapeQuotes(this.movieData.Texture)).Append('"');
    }
    if (!string.IsNullOrWhiteSpace(scene.Script))
      sb.Append(TokenParser.ParseText(scene.Script));
    if (!string.IsNullOrWhiteSpace(scene.Text))
      sb.Append("/message \"").Append(ArgUtility.EscapeQuotes(TokenParser.ParseText(scene.Text))).Append('"');
    if (scene.ResponsePoint == null)
      return;
    this._ParseResponse(sb, scene);
  }

  protected void _ParseResponse(StringBuilder sb, MovieScene scene = null)
  {
    Character character;
    if (this._responseOrder.TryGetValue(this.currentResponse, out character))
    {
      sb.Append("/pause 500");
      bool ignoreScript = false;
      if (!this._whiteListDependencyLookup.ContainsKey(character))
      {
        MovieCharacterReaction reactionsForCharacter = MovieTheater.GetReactionsForCharacter(character as NPC);
        if (reactionsForCharacter != null)
        {
          foreach (MovieReaction reaction in reactionsForCharacter.Reactions)
          {
            if (reaction.ShouldApplyToMovie(this.movieData, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(character as NPC)) && reaction.SpecialResponses?.DuringMovie != null && (string.IsNullOrEmpty(reaction.SpecialResponses.DuringMovie.ResponsePoint) || scene != null && reaction.SpecialResponses.DuringMovie.ResponsePoint == scene.ResponsePoint || reaction.SpecialResponses.DuringMovie.ResponsePoint == this.currentResponse.ToString() || reaction.Whitelist.Count > 0))
            {
              string text1 = TokenParser.ParseText(reaction.SpecialResponses.DuringMovie.Script);
              string text2 = TokenParser.ParseText(reaction.SpecialResponses.DuringMovie.Text);
              if (!string.IsNullOrWhiteSpace(text1))
              {
                sb.Append(text1);
                ignoreScript = true;
              }
              if (!string.IsNullOrWhiteSpace(text2))
              {
                sb.Append("/speak ").Append(character.name.Value).Append(" \"").Append(text2).Append('"');
                break;
              }
              break;
            }
          }
        }
      }
      this._ParseCharacterResponse(sb, character, ignoreScript);
      foreach (Character key in this._whiteListDependencyLookup.Keys)
      {
        if (this._whiteListDependencyLookup[key] == character)
          this._ParseCharacterResponse(sb, key);
      }
    }
    ++this.currentResponse;
  }

  protected void _ParseCharacterResponse(
    StringBuilder sb,
    Character responding_character,
    bool ignoreScript = false)
  {
    string responseForMovie = MovieTheater.GetResponseForMovie(responding_character as NPC);
    Character character;
    if (this._whiteListDependencyLookup.TryGetValue(responding_character, out character))
      responseForMovie = MovieTheater.GetResponseForMovie(character as NPC);
    switch (responseForMovie)
    {
      case "love":
        sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(200);
        if (!ignoreScript)
        {
          sb.Append("/playSound reward/emote ").Append(responding_character.name.Value).Append(' ').Append(20).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_LoveMovie", (object) responding_character.displayName)).Append('"');
          break;
        }
        break;
      case "like":
        sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(100);
        if (!ignoreScript)
        {
          sb.Append("/playSound give_gift/emote ").Append(responding_character.name.Value).Append(' ').Append(56).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_LikeMovie", (object) responding_character.displayName)).Append('"');
          break;
        }
        break;
      case "dislike":
        sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(0);
        if (!ignoreScript)
        {
          sb.Append("/playSound newArtifact/emote ").Append(responding_character.name.Value).Append(' ').Append(24).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_DislikeMovie", (object) responding_character.displayName)).Append('"');
          break;
        }
        break;
    }
    MovieConcession concession;
    if (this._concessionsData != null && this._concessionsData.TryGetValue(responding_character, out concession))
    {
      string tasteForCharacter = MovieTheater.GetConcessionTasteForCharacter(responding_character, concession);
      string str1 = "";
      CharacterData data;
      if (NPC.TryGetData(responding_character.name.Value, out data))
      {
        switch (data.Gender)
        {
          case Gender.Male:
            str1 = "_Male";
            break;
          case Gender.Female:
            str1 = "_Female";
            break;
        }
      }
      string str2 = "eat";
      if (concession.Tags != null && concession.Tags.Contains("Drink"))
        str2 = "gulp";
      switch (tasteForCharacter)
      {
        case "love":
          sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(50);
          sb.Append("/tossConcession ").Append(responding_character.Name).Append(' ').Append(concession.Id).Append("/pause 1000");
          sb.Append("/playSound ").Append(str2).Append("/shake ").Append(responding_character.Name).Append(" 500/pause 1000");
          sb.Append("/playSound reward/emote ").Append(responding_character.name.Value).Append(' ').Append(20).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_LoveConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName)).Append('"');
          break;
        case "like":
          sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(25);
          sb.Append("/tossConcession ").Append(responding_character.Name).Append(' ').Append(concession.Id).Append("/pause 1000");
          sb.Append("/playSound ").Append(str2).Append("/shake ").Append(responding_character.Name).Append(" 500/pause 1000");
          sb.Append("/playSound give_gift/emote ").Append(responding_character.name.Value).Append(' ').Append(56).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_LikeConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName)).Append('"');
          break;
        case "dislike":
          sb.Append("/friendship ").Append(responding_character.Name).Append(' ').Append(0);
          sb.Append("/playSound croak/pause 1000");
          sb.Append("/playSound newArtifact/emote ").Append(responding_character.name.Value).Append(' ').Append(40).Append("/message \"").Append(Game1.content.LoadString("Strings\\Characters:MovieTheater_DislikeConcession" + str1, (object) responding_character.displayName, (object) concession.DisplayName)).Append('"');
          break;
      }
    }
    this._characterResponses[responding_character] = responseForMovie;
  }

  public Dictionary<Character, string> GetCharacterResponses() => this._characterResponses;

  private static string getEventName(Character c)
  {
    return c is Farmer who ? "farmer" + Utility.getFarmerNumberFromFarmer(who).ToString() : c.name.Value;
  }

  private Point getBackRowSeatTileFromIndex(int index)
  {
    switch (index)
    {
      case 0:
        return new Point(2, 10);
      case 1:
        return new Point(3, 10);
      case 2:
        return new Point(4, 10);
      case 3:
        return new Point(5, 10);
      case 4:
        return new Point(8, 10);
      case 5:
        return new Point(9, 10);
      case 6:
        return new Point(10, 10);
      case 7:
        return new Point(11, 10);
      default:
        return new Point(4, 12);
    }
  }

  private Point getMidRowSeatTileFromIndex(int index)
  {
    switch (index)
    {
      case 0:
        return new Point(3, 8);
      case 1:
        return new Point(4, 8);
      case 2:
        return new Point(5, 8);
      case 3:
        return new Point(8, 8);
      case 4:
        return new Point(9, 8);
      case 5:
        return new Point(10, 8);
      default:
        return new Point(4, 12);
    }
  }

  private Point getFrontRowSeatTileFromIndex(int index)
  {
    switch (index)
    {
      case 0:
        return new Point(4, 6);
      case 1:
        return new Point(5, 6);
      case 2:
        return new Point(8, 6);
      case 3:
        return new Point(9, 6);
      default:
        return new Point(4, 12);
    }
  }
}
