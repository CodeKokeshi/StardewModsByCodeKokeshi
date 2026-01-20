// Decompiled with JetBrains decompiler
// Type: StardewValley.Stats
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Stats
{
  /// <summary>The number of each monster type killed, prefixed by the monster's internal name.</summary>
  public StatsDictionary<int> specificMonstersKilled = new StatsDictionary<int>();
  /// <summary>The numeric metrics tracked by the game.</summary>
  /// <remarks>Most code should use methods like <see cref="M:StardewValley.Stats.Get(System.String)" /> or <see cref="M:StardewValley.Stats.Set(System.String,System.UInt32)" /> instead of calling this directly.</remarks>
  public StatsDictionary<uint> Values = new StatsDictionary<uint>();
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Stats.Values" /> instead.</summary>
  [XmlElement("stat_dictionary")]
  public SerializableDictionary<string, uint> obsolete_stat_dictionary;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.AverageBedtime" /> instead.</summary>
  [XmlElement("averageBedtime")]
  public uint? obsolete_averageBedtime;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.BeveragesMade" /> instead.</summary>
  [XmlElement("beveragesMade")]
  public uint? obsolete_beveragesMade;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.CaveCarrotsFound" /> instead.</summary>
  [XmlElement("caveCarrotsFound")]
  public uint? obsolete_caveCarrotsFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.CheeseMade" /> instead.</summary>
  [XmlElement("cheeseMade")]
  public uint? obsolete_cheeseMade;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.ChickenEggsLayed" /> instead.</summary>
  [XmlElement("chickenEggsLayed")]
  public uint? obsolete_chickenEggsLayed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.CopperFound" /> instead.</summary>
  [XmlElement("copperFound")]
  public uint? obsolete_copperFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.CowMilkProduced" /> instead.</summary>
  [XmlElement("cowMilkProduced")]
  public uint? obsolete_cowMilkProduced;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.CropsShipped" /> instead.</summary>
  [XmlElement("cropsShipped")]
  public uint? obsolete_cropsShipped;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.DaysPlayed" /> instead.</summary>
  [XmlElement("daysPlayed")]
  public uint? obsolete_daysPlayed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.DiamondsFound" /> instead.</summary>
  [XmlElement("diamondsFound")]
  public uint? obsolete_diamondsFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.DirtHoed" /> instead.</summary>
  [XmlElement("dirtHoed")]
  public uint? obsolete_dirtHoed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.DuckEggsLayed" /> instead.</summary>
  [XmlElement("duckEggsLayed")]
  public uint? obsolete_duckEggsLayed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.FishCaught" /> instead.</summary>
  [XmlElement("fishCaught")]
  public uint? obsolete_fishCaught;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GeodesCracked" /> instead.</summary>
  [XmlElement("geodesCracked")]
  public uint? obsolete_geodesCracked;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GiftsGiven" /> instead.</summary>
  [XmlElement("giftsGiven")]
  public uint? obsolete_giftsGiven;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GoatCheeseMade" /> instead.</summary>
  [XmlElement("goatCheeseMade")]
  public uint? obsolete_goatCheeseMade;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GoatMilkProduced" /> instead.</summary>
  [XmlElement("goatMilkProduced")]
  public uint? obsolete_goatMilkProduced;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GoldFound" /> instead.</summary>
  [XmlElement("goldFound")]
  public uint? obsolete_goldFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.GoodFriends" /> instead.</summary>
  [XmlElement("goodFriends")]
  public uint? obsolete_goodFriends;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.IndividualMoneyEarned" /> instead.</summary>
  [XmlElement("individualMoneyEarned")]
  public uint? obsolete_individualMoneyEarned;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.IridiumFound" /> instead.</summary>
  [XmlElement("iridiumFound")]
  public uint? obsolete_iridiumFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.IronFound" /> instead.</summary>
  [XmlElement("ironFound")]
  public uint? obsolete_ironFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.ItemsCooked" /> instead.</summary>
  [XmlElement("itemsCooked")]
  public uint? obsolete_itemsCooked;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.ItemsCrafted" /> instead.</summary>
  [XmlElement("itemsCrafted")]
  public uint? obsolete_itemsCrafted;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.ItemsForaged" /> instead.</summary>
  [XmlElement("itemsForaged")]
  public uint? obsolete_itemsForaged;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.ItemsShipped" /> instead.</summary>
  [XmlElement("itemsShipped")]
  public uint? obsolete_itemsShipped;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.MonstersKilled" /> instead.</summary>
  [XmlElement("monstersKilled")]
  public uint? obsolete_monstersKilled;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.MysticStonesCrushed" /> instead.</summary>
  [XmlElement("mysticStonesCrushed")]
  public uint? obsolete_mysticStonesCrushed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.NotesFound" /> instead.</summary>
  [XmlElement("notesFound")]
  public uint? obsolete_notesFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.OtherPreciousGemsFound" /> instead.</summary>
  [XmlElement("otherPreciousGemsFound")]
  public uint? obsolete_otherPreciousGemsFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.PiecesOfTrashRecycled" /> instead.</summary>
  [XmlElement("piecesOfTrashRecycled")]
  public uint? obsolete_piecesOfTrashRecycled;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.PreservesMade" /> instead.</summary>
  [XmlElement("preservesMade")]
  public uint? obsolete_preservesMade;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.PrismaticShardsFound" /> instead.</summary>
  [XmlElement("prismaticShardsFound")]
  public uint? obsolete_prismaticShardsFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.QuestsCompleted" /> instead.</summary>
  [XmlElement("questsCompleted")]
  public uint? obsolete_questsCompleted;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.RabbitWoolProduced" /> instead.</summary>
  [XmlElement("rabbitWoolProduced")]
  public uint? obsolete_rabbitWoolProduced;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.RocksCrushed" /> instead.</summary>
  [XmlElement("rocksCrushed")]
  public uint? obsolete_rocksCrushed;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.SheepWoolProduced" /> instead.</summary>
  [XmlElement("sheepWoolProduced")]
  public uint? obsolete_sheepWoolProduced;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.SlimesKilled" /> instead.</summary>
  [XmlElement("slimesKilled")]
  public uint? obsolete_slimesKilled;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.StepsTaken" /> instead.</summary>
  [XmlElement("stepsTaken")]
  public uint? obsolete_stepsTaken;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.StoneGathered" /> instead.</summary>
  [XmlElement("stoneGathered")]
  public uint? obsolete_stoneGathered;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.StumpsChopped" /> instead.</summary>
  [XmlElement("stumpsChopped")]
  public uint? obsolete_stumpsChopped;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.TimesFished" /> instead.</summary>
  [XmlElement("timesFished")]
  public uint? obsolete_timesFished;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.TimesUnconscious" /> instead.</summary>
  [XmlElement("timesUnconscious")]
  public uint? obsolete_timesUnconscious;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="F:StardewValley.Constants.StatKeys.TotalMoneyGifted" /> instead.</summary>
  [XmlElement("totalMoneyGifted")]
  public uint? obsolete_totalMoneyGifted;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.TrufflesFound" /> instead.</summary>
  [XmlElement("trufflesFound")]
  public uint? obsolete_trufflesFound;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.WeedsEliminated" /> instead.</summary>
  [XmlElement("weedsEliminated")]
  public uint? obsolete_weedsEliminated;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="P:StardewValley.Stats.SeedsSown" /> instead.</summary>
  [XmlElement("seedsSown")]
  public uint? obsolete_seedsSown;

  /// <summary>Whether platform achievements can be unlocked retroactively overnight or when loading the save.</summary>
  /// <remarks>Certification requirements on some platforms prohibit us from unlocking trophies without the player doing something. On those platforms, we instead unlock missed achievements when the player performs a relevant action.</remarks>
  public static bool AllowRetroactiveAchievements => Program.sdk.RetroactiveAchievementsAllowed();

  [XmlIgnore]
  public uint AverageBedtime
  {
    get => this.Get("averageBedtime");
    set
    {
      uint num = this.Get("averageBedtime");
      uint val2 = this.Get("daysPlayed");
      this.Set("averageBedtime", (num * (val2 - 1U) + value) / Math.Max(1U, val2));
    }
  }

  [XmlIgnore]
  public uint DaysPlayed
  {
    get => this.Get("daysPlayed");
    set => this.Set("daysPlayed", value);
  }

  [XmlIgnore]
  public uint IndividualMoneyEarned
  {
    get => this.Get("individualMoneyEarned");
    set
    {
      uint num = this.Get("individualMoneyEarned");
      this.Set("individualMoneyEarned", value);
      if (num < 1000000U && value >= 1000000U)
        Game1.multiplayer.globalChatInfoMessage("SoloEarned1mil_" + (Game1.player.IsMale ? "Male" : "Female"), Game1.player.Name);
      else if (num < 100000U && value >= 100000U)
        Game1.multiplayer.globalChatInfoMessage("SoloEarned100k_" + (Game1.player.IsMale ? "Male" : "Female"), Game1.player.Name);
      else if (num < 10000U && value >= 10000U)
      {
        Game1.multiplayer.globalChatInfoMessage("SoloEarned10k_" + (Game1.player.IsMale ? "Male" : "Female"), Game1.player.Name);
      }
      else
      {
        if (num >= 1000U || value < 1000U)
          return;
        Game1.multiplayer.globalChatInfoMessage("SoloEarned1k_" + (Game1.player.IsMale ? "Male" : "Female"), Game1.player.Name);
      }
    }
  }

  [XmlIgnore]
  public uint ItemsCooked
  {
    get => this.Get("itemsCooked");
    set => this.Set("itemsCooked", value);
  }

  [XmlIgnore]
  public uint ItemsCrafted
  {
    get => this.Get("itemsCrafted");
    set
    {
      this.Set("itemsCrafted", value);
      this.checkForCraftingAchievements();
    }
  }

  [XmlIgnore]
  public uint ItemsForaged
  {
    get => this.Get("itemsForaged");
    set => this.Set("itemsForaged", value);
  }

  [XmlIgnore]
  public uint ItemsShipped
  {
    get => this.Get("itemsShipped");
    set => this.Set("itemsShipped", value);
  }

  [XmlIgnore]
  public uint NotesFound
  {
    get => this.Get("notesFound");
    set => this.Set("notesFound", value);
  }

  [XmlIgnore]
  public uint StepsTaken
  {
    get => this.Get("stepsTaken");
    set => this.Set("stepsTaken", value);
  }

  [XmlIgnore]
  public uint StumpsChopped
  {
    get => this.Get("stumpsChopped");
    set => this.Set("stumpsChopped", value);
  }

  [XmlIgnore]
  public uint TimesUnconscious
  {
    get => this.Get("timesUnconscious");
    set => this.Set("timesUnconscious", value);
  }

  [XmlIgnore]
  public uint BeveragesMade
  {
    get => this.Get("beveragesMade");
    set => this.Set("beveragesMade", value);
  }

  [XmlIgnore]
  public uint CheeseMade
  {
    get => this.Get("cheeseMade");
    set => this.Set("cheeseMade", value);
  }

  [XmlIgnore]
  public uint ChickenEggsLayed
  {
    get => this.Get("chickenEggsLayed");
    set => this.Set("chickenEggsLayed", value);
  }

  [XmlIgnore]
  public uint CowMilkProduced
  {
    get => this.Get("cowMilkProduced");
    set => this.Set("cowMilkProduced", value);
  }

  [XmlIgnore]
  public uint CropsShipped
  {
    get => this.Get("cropsShipped");
    set => this.Set("cropsShipped", value);
  }

  [XmlIgnore]
  public uint DirtHoed
  {
    get => this.Get("dirtHoed");
    set => this.Set("dirtHoed", value);
  }

  [XmlIgnore]
  public uint DuckEggsLayed
  {
    get => this.Get("duckEggsLayed");
    set => this.Set("duckEggsLayed", value);
  }

  [XmlIgnore]
  public uint GoatCheeseMade
  {
    get => this.Get("goatCheeseMade");
    set => this.Set("goatCheeseMade", value);
  }

  [XmlIgnore]
  public uint GoatMilkProduced
  {
    get => this.Get("goatMilkProduced");
    set => this.Set("goatMilkProduced", value);
  }

  [XmlIgnore]
  public uint PiecesOfTrashRecycled
  {
    get => this.Get("piecesOfTrashRecycled");
    set => this.Set("piecesOfTrashRecycled", value);
  }

  [XmlIgnore]
  public uint PreservesMade
  {
    get => this.Get("preservesMade");
    set => this.Set("preservesMade", value);
  }

  [XmlIgnore]
  public uint RabbitWoolProduced
  {
    get => this.Get("rabbitWoolProduced");
    set => this.Set("rabbitWoolProduced", value);
  }

  [XmlIgnore]
  public uint SeedsSown
  {
    get => this.Get("seedsSown");
    set => this.Set("seedsSown", value);
  }

  [XmlIgnore]
  public uint SheepWoolProduced
  {
    get => this.Get("sheepWoolProduced");
    set => this.Set("sheepWoolProduced", value);
  }

  [XmlIgnore]
  public uint TrufflesFound
  {
    get => this.Get("trufflesFound");
    set => this.Set("trufflesFound", value);
  }

  [XmlIgnore]
  public uint WeedsEliminated
  {
    get => this.Get("weedsEliminated");
    set => this.Set("weedsEliminated", value);
  }

  [XmlIgnore]
  public uint MonstersKilled
  {
    get => this.Get("monstersKilled");
    set => this.Set("monstersKilled", value);
  }

  [XmlIgnore]
  public uint SlimesKilled
  {
    get => this.Get("slimesKilled");
    set => this.Set("slimesKilled", value);
  }

  [XmlIgnore]
  public uint FishCaught
  {
    get => this.Get("fishCaught");
    set => this.Set("fishCaught", value);
  }

  [XmlIgnore]
  public uint TimesFished
  {
    get => this.Get("timesFished");
    set => this.Set("timesFished", value);
  }

  [XmlIgnore]
  public uint CaveCarrotsFound
  {
    get => this.Get("caveCarrotsFound");
    set => this.Set("caveCarrotsFound", value);
  }

  [XmlIgnore]
  public uint CopperFound
  {
    get => this.Get("copperFound");
    set => this.Set("copperFound", value);
  }

  [XmlIgnore]
  public uint DiamondsFound
  {
    get => this.Get("diamondsFound");
    set => this.Set("diamondsFound", value);
  }

  [XmlIgnore]
  public uint GeodesCracked
  {
    get => this.Get("geodesCracked");
    set => this.Set("geodesCracked", value);
  }

  [XmlIgnore]
  public uint GoldFound
  {
    get => this.Get("goldFound");
    set => this.Set("goldFound", value);
  }

  [XmlIgnore]
  public uint IridiumFound
  {
    get => this.Get("iridiumFound");
    set => this.Set("iridiumFound", value);
  }

  [XmlIgnore]
  public uint IronFound
  {
    get => this.Get("ironFound");
    set => this.Set("ironFound", value);
  }

  [XmlIgnore]
  public uint MysticStonesCrushed
  {
    get => this.Get("mysticStonesCrushed");
    set => this.Set("mysticStonesCrushed", value);
  }

  [XmlIgnore]
  public uint OtherPreciousGemsFound
  {
    get => this.Get("otherPreciousGemsFound");
    set => this.Set("otherPreciousGemsFound", value);
  }

  [XmlIgnore]
  public uint PrismaticShardsFound
  {
    get => this.Get("prismaticShardsFound");
    set => this.Set("prismaticShardsFound", value);
  }

  [XmlIgnore]
  public uint RocksCrushed
  {
    get => this.Get("rocksCrushed");
    set => this.Set("rocksCrushed", value);
  }

  [XmlIgnore]
  public uint StoneGathered
  {
    get => this.Get("stoneGathered");
    set => this.Set("stoneGathered", value);
  }

  [XmlIgnore]
  public uint GiftsGiven
  {
    get => this.Get("giftsGiven");
    set => this.Set("giftsGiven", value);
  }

  [XmlIgnore]
  public uint GoodFriends
  {
    get => this.Get("goodFriends");
    set => this.Set("goodFriends", value);
  }

  [XmlIgnore]
  public uint QuestsCompleted
  {
    get => this.Get("questsCompleted");
    set
    {
      this.Set("questsCompleted", value);
      this.checkForQuestAchievements();
    }
  }

  /// <summary>Get the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  public uint Get(string key)
  {
    uint num;
    return !this.Values.TryGetValue(key, out num) ? 0U : num;
  }

  /// <summary>Set the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  /// <param name="value">The new value to set.</param>
  public void Set(string key, uint value)
  {
    if (value != 0U)
      this.Values[key] = value;
    else
      this.Values.Remove(key);
  }

  /// <summary>Set the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  /// <param name="value">The new value to set.</param>
  /// <remarks>The minimum stat value is zero. Setting a negative value is equivalent to setting zero.</remarks>
  public void Set(string key, int value)
  {
    if (value <= 0)
      this.Set(key, 0U);
    else
      this.Set(key, (uint) value);
  }

  /// <summary>Decrease the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  /// <param name="amount">The amount by which to decrease the stat.</param>
  /// <remarks>The minimum stat value is zero. Decrementing past zero is equivalent to setting zero.</remarks>
  public uint Decrement(string key, uint amount = 1)
  {
    uint num1 = this.Get(key);
    uint num2 = amount >= num1 ? 0U : num1 - amount;
    this.Set(key, num2);
    return num2;
  }

  /// <summary>Increase the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  /// <param name="amount">The amount by which to increase the stat.</param>
  /// <returns>Returns the new stat value.</returns>
  public uint Increment(string key, uint amount = 1)
  {
    uint num = this.Get(key) + amount;
    this.Set(key, num);
    return num;
  }

  /// <summary>Increase the value of a tracked stat.</summary>
  /// <param name="key">The unique stat key, usually matching a <see cref="T:StardewValley.Constants.StatKeys" /> field.</param>
  /// <param name="amount">The amount by which to increase the stat. If this is set to a negative value, the stat will be decremented instead (up to a minimum of zero).</param>
  /// <returns>Returns the new stat value.</returns>
  public uint Increment(string key, int amount)
  {
    return amount >= 0 ? this.Increment(key, (uint) amount) : this.Decrement(key, (uint) -amount);
  }

  /// <summary>Update the stats when a monster is killed.</summary>
  /// <param name="name">The monster's internal name.</param>
  public void monsterKilled(string name)
  {
    if (AdventureGuild.willThisKillCompleteAMonsterSlayerQuest(name))
    {
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Stats.cs.5129"));
      Game1.multiplayer.globalChatInfoMessage("MonsterSlayer" + Game1.random.Next(4).ToString(), Game1.player.Name, TokenStringBuilder.MonsterName(name));
    }
    this.specificMonstersKilled[name] = this.getMonstersKilled(name) + 1;
    this.checkForMonsterSlayerAchievement(true);
  }

  /// <summary>Get the number of a given monster type that the player has killed.</summary>
  /// <param name="name">The monster's internal name.</param>
  public int getMonstersKilled(string name)
  {
    return this.specificMonstersKilled.GetValueOrDefault<string, int>(name);
  }

  public void onMoneyGifted(uint amount)
  {
    uint num1 = this.Get("totalMoneyGifted");
    uint num2 = this.Increment("totalMoneyGifted", amount);
    if (num1 <= 1000000U && num2 > 1000000U)
      Game1.multiplayer.globalChatInfoMessage("Gifted1mil", Game1.player.Name);
    else if (num1 <= 100000U && num2 > 100000U)
      Game1.multiplayer.globalChatInfoMessage("Gifted100k", Game1.player.Name);
    else if (num1 <= 10000U && num2 > 10000U)
    {
      Game1.multiplayer.globalChatInfoMessage("Gifted10k", Game1.player.Name);
    }
    else
    {
      if (num1 > 1000U || num2 <= 1000U)
        return;
      Game1.multiplayer.globalChatInfoMessage("Gifted1k", Game1.player.Name);
    }
  }

  public void takeStep()
  {
    switch (this.Increment("stepsTaken"))
    {
      case 10000:
        Game1.multiplayer.globalChatInfoMessage("Walked10k", Game1.player.Name);
        break;
      case 100000:
        Game1.multiplayer.globalChatInfoMessage("Walked100k", Game1.player.Name);
        break;
      case 1000000:
        Game1.multiplayer.globalChatInfoMessage("Walked1m", Game1.player.Name);
        break;
      case 10000000:
        Game1.multiplayer.globalChatInfoMessage("Walked10m", Game1.player.Name);
        break;
    }
  }

  /// <summary>Unlock the 'Well Read' achievement if its criteria has been met.</summary>
  public void checkForBooksReadAchievement()
  {
    if (Game1.player.stats.Get("Book_Trash") <= 0U || Game1.player.stats.Get("Book_Crabbing") <= 0U || Game1.player.stats.Get("Book_Bombs") <= 0U || Game1.player.stats.Get("Book_Roe") <= 0U || Game1.player.stats.Get("Book_WildSeeds") <= 0U || Game1.player.stats.Get("Book_Woodcutting") <= 0U || Game1.player.stats.Get("Book_Defense") <= 0U || Game1.player.stats.Get("Book_Friendship") <= 0U || Game1.player.stats.Get("Book_Void") <= 0U || Game1.player.stats.Get("Book_Speed") <= 0U || Game1.player.stats.Get("Book_Marlon") <= 0U || Game1.player.stats.Get("Book_PriceCatalogue") <= 0U || Game1.player.stats.Get("Book_Diamonds") <= 0U || Game1.player.stats.Get("Book_Mystery") <= 0U || Game1.player.stats.Get("Book_AnimalCatalogue") <= 0U || Game1.player.stats.Get("Book_Speed2") <= 0U || Game1.player.stats.Get("Book_Artifact") <= 0U || Game1.player.stats.Get("Book_Horse") <= 0U || Game1.player.stats.Get("Book_Grass") <= 0U)
      return;
    Game1.getAchievement(35);
  }

  /// <summary>Unlock the cooking-related achievements if their criteria have been met.</summary>
  public void checkForCookingAchievements()
  {
    Dictionary<string, string> cookingRecipes = CraftingRecipe.cookingRecipes;
    int num1 = 0;
    int num2 = 0;
    foreach (KeyValuePair<string, string> keyValuePair in cookingRecipes)
    {
      int num3;
      if (Game1.player.cookingRecipes.ContainsKey(keyValuePair.Key) && Game1.player.recipesCooked.TryGetValue(ArgUtility.SplitBySpaceAndGet(keyValuePair.Value.Split('/')[2], 0), out num3))
      {
        num2 += num3;
        ++num1;
      }
    }
    this.Set("itemsCooked", num2);
    if (num1 >= cookingRecipes.Count)
      Game1.getAchievement(17);
    if (num1 >= 25)
      Game1.getAchievement(16 /*0x10*/);
    if (num1 < 10)
      return;
    Game1.getAchievement(15);
  }

  /// <summary>Unlock the crafting-related achievements if their criteria have been met.</summary>
  public void checkForCraftingAchievements()
  {
    Dictionary<string, string> craftingRecipes = CraftingRecipe.craftingRecipes;
    int num1 = 0;
    int num2 = 0;
    foreach (string key in craftingRecipes.Keys)
    {
      int num3;
      if (!(key == "Wedding Ring") && Game1.player.craftingRecipes.TryGetValue(key, out num3))
      {
        num2 += num3;
        if (Game1.player.craftingRecipes[key] > 0)
          ++num1;
      }
    }
    this.Set("itemsCrafted", num2);
    if (num1 >= craftingRecipes.Count - 1)
      Game1.getAchievement(22);
    if (num1 >= 30)
      Game1.getAchievement(21);
    if (num1 < 15)
      return;
    Game1.getAchievement(20);
  }

  /// <summary>Unlock the shipping-related achievements if their criteria have been met.</summary>
  public void checkForShippingAchievements()
  {
    bool flag1 = true;
    bool flag2 = false;
    foreach (CropData cropData in (IEnumerable<CropData>) Game1.cropData.Values)
    {
      if (cropData.CountForPolyculture)
        flag1 = flag1 && DidFarmerShip(cropData.HarvestItemId, 15);
      if (cropData.CountForMonoculture)
        flag2 = flag2 || DidFarmerShip(cropData.HarvestItemId, 300);
    }
    if (flag1)
      Game1.getAchievement(31 /*0x1F*/);
    if (flag2)
      Game1.getAchievement(32 /*0x20*/);
    if (!Utility.hasFarmerShippedAllItems())
      return;
    Game1.getAchievement(34);

    static bool DidFarmerShip(string itemId, int number)
    {
      return Game1.player.basicShipped.GetValueOrDefault(itemId) >= number;
    }
  }

  /// <summary>Unlock the fishing-related achievements if their criteria have been met.</summary>
  public void checkForFishingAchievements()
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    foreach (ParsedItemData parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
    {
      if (parsedItemData.ObjectType == "Fish" && (!(parsedItemData.RawData is ObjectData rawData) || !rawData.ExcludeFromFishingCollection))
      {
        ++num3;
        int[] numArray;
        if (Game1.player.fishCaught.TryGetValue(parsedItemData.QualifiedItemId, out numArray))
        {
          num1 += numArray[0];
          ++num2;
        }
      }
    }
    this.Set("fishCaught", num1);
    if (num1 >= 100)
      Game1.getAchievement(27);
    if (num2 >= num3)
    {
      Game1.getAchievement(26);
      if (!Game1.player.hasOrWillReceiveMail("CF_Fish"))
        Game1.addMailForTomorrow("CF_Fish");
    }
    if (num2 >= 24)
      Game1.getAchievement(25);
    if (num2 < 10)
      return;
    Game1.getAchievement(24);
  }

  /// <summary>Unlock the artifact donation-related achievements if their criteria have been met.</summary>
  public void checkForArchaeologyAchievements()
  {
    int length = Game1.netWorldState.Value.MuseumPieces.Length;
    if (length >= LibraryMuseum.totalArtifacts)
      Game1.getAchievement(5);
    if (length < 40)
      return;
    Game1.getAchievement(28);
  }

  /// <summary>Unlock achievements related to the items held by the player.</summary>
  public void checkForHeldItemAchievements()
  {
    if (!Game1.player.Items.ContainsId("(W)62") && !Game1.player.Items.ContainsId("(W)63") && !Game1.player.Items.ContainsId("(W)64"))
      return;
    Game1.getAchievement(42);
  }

  /// <summary>Unlock the money-related achievements if their criteria have been met.</summary>
  public void checkForMoneyAchievements()
  {
    if (Game1.player.totalMoneyEarned >= 10000000U)
      Game1.getAchievement(4);
    if (Game1.player.totalMoneyEarned >= 1000000U)
      Game1.getAchievement(3);
    if (Game1.player.totalMoneyEarned >= 250000U)
      Game1.getAchievement(2);
    if (Game1.player.totalMoneyEarned >= 50000U)
      Game1.getAchievement(1);
    if (Game1.player.totalMoneyEarned < 15000U)
      return;
    Game1.getAchievement(0);
  }

  /// <summary>Unlock the farmhouse upgrade-related achievements if their criteria have been met.</summary>
  public void checkForBuildingUpgradeAchievements()
  {
    if (Game1.player.HouseUpgradeLevel >= 2)
      Game1.getAchievement(19);
    if (Game1.player.HouseUpgradeLevel < 1)
      return;
    Game1.getAchievement(18);
  }

  /// <summary>Unlock the quest-related achievements if their criteria have been met.</summary>
  public void checkForQuestAchievements()
  {
    if (this.QuestsCompleted >= 40U)
    {
      Game1.getAchievement(30);
      Game1.addMailForTomorrow("quest35");
    }
    if (this.QuestsCompleted < 10U)
      return;
    Game1.getAchievement(29);
    Game1.addMailForTomorrow("quest10");
  }

  /// <summary>Unlock the friendship-related achievements if their criteria have been met.</summary>
  public void checkForFriendshipAchievements()
  {
    uint num1 = 0;
    uint num2 = 0;
    uint num3 = 0;
    foreach (Friendship friendship in Game1.player.friendshipData.Values)
    {
      if (friendship.Points >= 2500)
        ++num3;
      if (friendship.Points >= 2000)
        ++num2;
      if (friendship.Points >= 1250)
        ++num1;
    }
    this.GoodFriends = num2;
    if (num1 >= 20U)
      Game1.getAchievement(13);
    if (num1 >= 10U)
      Game1.getAchievement(12);
    if (num1 >= 4U)
      Game1.getAchievement(11);
    if (num1 >= 1U)
      Game1.getAchievement(6);
    if (num3 >= 8U)
      Game1.getAchievement(9);
    if (num3 >= 1U)
      Game1.getAchievement(7);
    foreach (KeyValuePair<string, string> cookingRecipe in CraftingRecipe.cookingRecipes)
    {
      string key1 = cookingRecipe.Key;
      string[] array = ArgUtility.SplitBySpace(ArgUtility.Get(cookingRecipe.Value.Split('/'), 3));
      if (!(ArgUtility.Get(array, 0) != "f"))
      {
        string key2 = ArgUtility.Get(array, 1);
        int num4 = ArgUtility.GetInt(array, 2);
        Friendship friendship;
        if (key2 != null && Game1.player.friendshipData.TryGetValue(key2, out friendship) && friendship.Points >= num4 * 250 && !Game1.player.cookingRecipes.ContainsKey(key1) && !Game1.player.hasOrWillReceiveMail(key2 + "Cooking"))
          Game1.addMailForTomorrow(key2 + "Cooking");
      }
    }
    foreach (KeyValuePair<string, string> craftingRecipe in CraftingRecipe.craftingRecipes)
    {
      string key3 = craftingRecipe.Key;
      string[] array = ArgUtility.SplitBySpace(ArgUtility.Get(craftingRecipe.Value.Split('/'), 4));
      if (!(ArgUtility.Get(array, 0) != "f"))
      {
        string key4 = ArgUtility.Get(array, 1);
        int num5 = ArgUtility.GetInt(array, 2);
        Friendship friendship;
        if (key4 != null && Game1.player.friendshipData.TryGetValue(key4, out friendship) && friendship.Points >= num5 * 250 && !Game1.player.craftingRecipes.ContainsKey(key3) && !Game1.player.hasOrWillReceiveMail(key4 + "Crafting"))
          Game1.addMailForTomorrow(key4 + "Crafting");
      }
    }
  }

  /// <summary>Unlock the achievements for completing the community center or Joja path if their criteria have been met.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForCommunityCenterOrJojaAchievements(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock))
      return;
    if (Game1.player.eventsSeen.Contains("191393"))
      Game1.getSteamAchievement("Achievement_LocalLegend");
    if (!Game1.player.eventsSeen.Contains("502261"))
      return;
    Game1.getSteamAchievement("Achievement_Joja");
  }

  /// <summary>Unlock the mini-game-related achievements if their criteria have been met.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForMiniGameAchievements(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock))
      return;
    if (Game1.player.stats.Get("completedPrairieKing") > 0U)
      Game1.getSteamAchievement("Achievement_PrairieKing");
    if (Game1.player.stats.Get("completedPrairieKingWithoutDying") <= 0U)
      return;
    Game1.getSteamAchievement("Achievement_FectorsChallenge");
  }

  /// <summary>Unlock the 'Full House' achievement if the player is married with two children.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForFullHouseAchievement(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock) || !Game1.player.isMarriedOrRoommates() || Game1.player.getChildrenCount() < 2)
      return;
    Game1.getSteamAchievement("Achievement_FullHouse");
  }

  /// <summary>Unlock the 'The Bottom' achievement if the player has reached the bottom of the mines.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  /// <param name="assumeDeepestLevel">Unlock the achievement regardless of the <see cref="P:StardewValley.Farmer.deepestMineLevel" />.</param>
  public void checkForMineAchievement(bool isDirectUnlock, bool assumeDeepestLevel = false)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock) || !assumeDeepestLevel && Game1.player.deepestMineLevel < 120)
      return;
    Game1.getSteamAchievement("Achievement_TheBottom");
  }

  /// <summary>Unlock the 'Protector of the Valley' achievement if the player has completed all monster slayer goals.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForMonsterSlayerAchievement(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock) || !AdventureGuild.areAllMonsterSlayerQuestsComplete())
      return;
    Game1.player.hasCompletedAllMonsterSlayerQuests.Value = true;
    Game1.getSteamAchievement("Achievement_KeeperOfTheMysticRings");
  }

  /// <summary>Unlock the skill-related achievements if their criteria have been met.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForSkillAchievements(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock))
      return;
    NetInt[] netIntArray = new NetInt[5]
    {
      Game1.player.farmingLevel,
      Game1.player.miningLevel,
      Game1.player.fishingLevel,
      Game1.player.foragingLevel,
      Game1.player.combatLevel
    };
    bool flag1 = false;
    bool flag2 = true;
    foreach (NetFieldBase<int, NetInt> netFieldBase in netIntArray)
    {
      if (netFieldBase.Value >= 10)
        flag1 = true;
      else
        flag2 = false;
    }
    if (!flag1)
      return;
    Game1.getSteamAchievement("Achievement_SingularTalent");
    if (!flag2)
      return;
    Game1.getSteamAchievement("Achievement_MasterOfTheFiveWays");
  }

  /// <summary>Unlock the 'Mystery Of The Stardrops' achievement if the player has found all stardrops.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  public void checkForStardropAchievement(bool isDirectUnlock)
  {
    if (!this.CanUnlockPlatformAchievements(isDirectUnlock) || !Utility.foundAllStardrops())
      return;
    Game1.getSteamAchievement("Achievement_Stardrop");
  }

  public bool isSharedAchievement(int which)
  {
    switch (which)
    {
      case 0:
      case 1:
      case 2:
      case 3:
      case 4:
      case 5:
      case 28:
        return true;
      default:
        return false;
    }
  }

  /// <summary>Unlock all achievements whose criteria have been met.</summary>
  public void checkForAchievements()
  {
    this.checkForBooksReadAchievement();
    this.checkForCookingAchievements();
    this.checkForCraftingAchievements();
    this.checkForShippingAchievements();
    this.checkForFishingAchievements();
    this.checkForArchaeologyAchievements();
    this.checkForHeldItemAchievements();
    this.checkForMoneyAchievements();
    this.checkForBuildingUpgradeAchievements();
    this.checkForQuestAchievements();
    this.checkForFriendshipAchievements();
    this.checkForCommunityCenterOrJojaAchievements(false);
    this.checkForMiniGameAchievements(false);
    this.checkForFullHouseAchievement(false);
    this.checkForMineAchievement(false);
    this.checkForMonsterSlayerAchievement(false);
    this.checkForSkillAchievements(false);
    this.checkForStardropAchievement(false);
  }

  /// <summary>Get whether platform achievements can be unlocked now based on platform restrictions.</summary>
  /// <param name="isDirectUnlock">Whether we're unlocking the achievements at the point where they normally trigger (i.e. not retroactively).</param>
  /// <remarks>See remarks on <see cref="P:StardewValley.Stats.AllowRetroactiveAchievements" />.</remarks>
  public bool CanUnlockPlatformAchievements(bool isDirectUnlock)
  {
    return Stats.AllowRetroactiveAchievements | isDirectUnlock;
  }
}
