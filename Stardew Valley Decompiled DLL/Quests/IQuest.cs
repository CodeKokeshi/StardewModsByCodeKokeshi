// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.IQuest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Quests;

public interface IQuest
{
  string GetName();

  string GetDescription();

  List<string> GetObjectiveDescriptions();

  bool CanBeCancelled();

  void MarkAsViewed();

  bool ShouldDisplayAsNew();

  bool ShouldDisplayAsComplete();

  bool IsTimedQuest();

  int GetDaysLeft();

  bool IsHidden();

  bool HasReward();

  bool HasMoneyReward();

  int GetMoneyReward();

  void OnMoneyRewardClaimed();

  bool OnLeaveQuestPage();
}
