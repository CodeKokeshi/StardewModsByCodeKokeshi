// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemContextTagManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Machines;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

/// <summary>Handles parsing and caching item context tags.</summary>
public static class ItemContextTagManager
{
  /// <summary>A cache of the base context tags by qualified item ID, excluding context tags added dynamically by the item instance.</summary>
  private static readonly Dictionary<string, HashSet<string>> BaseTagsCache = new Dictionary<string, HashSet<string>>();

  /// <summary>Get the base context tags for an item ID, excluding context tags added dynamically by the item instance.</summary>
  /// <param name="itemId">The qualified or unqualified item ID.</param>
  public static HashSet<string> GetBaseContextTags(string itemId)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
    HashSet<string> set;
    if (!ItemContextTagManager.BaseTagsCache.TryGetValue(dataOrErrorItem.QualifiedItemId, out set))
    {
      IItemDataDefinition itemType = dataOrErrorItem.ItemType;
      set = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      set.Add(ItemContextTagManager.SanitizeContextTag("id_" + dataOrErrorItem.QualifiedItemId));
      if (itemType.StandardDescriptor != null)
      {
        string str = ItemContextTagManager.SanitizeContextTag($"id_{dataOrErrorItem.ItemType.StandardDescriptor}_{dataOrErrorItem.ItemId}");
        set.Add(str);
      }
      switch (itemType.Identifier)
      {
        case "(BC)":
          if (dataOrErrorItem.RawData is BigCraftableData rawData1)
          {
            List<string> contextTags = rawData1.ContextTags;
            // ISSUE: explicit non-virtual call
            if ((contextTags != null ? (__nonvirtual (contextTags.Count) > 0 ? 1 : 0) : 0) != 0)
            {
              using (List<string>.Enumerator enumerator = rawData1.ContextTags.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  string current = enumerator.Current;
                  set.Add(current);
                }
                break;
              }
            }
            break;
          }
          break;
        case "(F)":
          if (dataOrErrorItem.RawData is string[] rawData2)
          {
            string str = ArgUtility.Get(rawData2, 11);
            set.AddRange<string>((IEnumerable<string>) ArgUtility.SplitBySpace(str));
            break;
          }
          break;
        case "(O)":
          if (dataOrErrorItem.RawData is ObjectData rawData3)
          {
            List<string> contextTags = rawData3.ContextTags;
            // ISSUE: explicit non-virtual call
            if ((contextTags != null ? (__nonvirtual (contextTags.Count) > 0 ? 1 : 0) : 0) != 0)
            {
              foreach (string contextTag in rawData3.ContextTags)
                set.Add(contextTag);
            }
            if (!rawData3.GeodeDropsDefaultItems)
            {
              List<ObjectGeodeDropData> geodeDrops = rawData3.GeodeDrops;
              // ISSUE: explicit non-virtual call
              if ((geodeDrops != null ? (__nonvirtual (geodeDrops.Count) > 0 ? 1 : 0) : 0) == 0)
                goto label_23;
            }
            set.Add("geode");
label_23:
            if (!rawData3.CanBeGivenAsGift)
            {
              set.Add("not_giftable");
              break;
            }
            break;
          }
          break;
        case "(H)":
          if (dataOrErrorItem.RawData is string[] rawData4)
          {
            string str = ArgUtility.Get(rawData4, 4);
            set.AddRange<string>((IEnumerable<string>) ArgUtility.SplitBySpace(str));
            break;
          }
          break;
      }
      if (dataOrErrorItem.InternalName != null)
        set.Add("item_" + ItemContextTagManager.SanitizeContextTag(dataOrErrorItem.InternalName));
      if (dataOrErrorItem.ObjectType != null)
        set.Add("item_type_" + ItemContextTagManager.SanitizeContextTag(dataOrErrorItem.ObjectType));
      MachineData machineData;
      if (DataLoader.Machines(Game1.content).TryGetValue(dataOrErrorItem.QualifiedItemId, out machineData))
      {
        set.Add("is_machine");
        int num;
        if (!machineData.HasOutput)
        {
          List<MachineOutputRule> outputRules = machineData.OutputRules;
          // ISSUE: explicit non-virtual call
          num = outputRules != null ? (__nonvirtual (outputRules.Count) > 0 ? 1 : 0) : 0;
        }
        else
          num = 1;
        bool flag1 = num != 0;
        bool flag2 = machineData.HasInput;
        if (!flag2)
        {
          List<MachineOutputRule> outputRules = machineData.OutputRules;
          // ISSUE: explicit non-virtual call
          if ((outputRules != null ? (__nonvirtual (outputRules.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            foreach (MachineOutputRule outputRule in machineData.OutputRules)
            {
              if (outputRule.Triggers != null)
              {
                foreach (MachineOutputTriggerRule trigger in outputRule.Triggers)
                {
                  if (trigger.Trigger.HasFlag((Enum) MachineOutputTrigger.ItemPlacedInMachine))
                  {
                    flag2 = true;
                    break;
                  }
                }
                if (flag2)
                  break;
              }
            }
          }
        }
        if (flag1)
          set.Add("machine_output");
        if (flag2)
          set.Add("machine_input");
      }
      string str1;
      if (dataOrErrorItem.Category == -4 && DataLoader.Fish(Game1.content).TryGetValue(dataOrErrorItem.ItemId, out str1))
      {
        string[] strArray = str1.Split('/');
        if (strArray[1] == "trap")
        {
          set.Add("fish_trap_location_" + strArray[4]);
        }
        else
        {
          set.Add("fish_motion_" + strArray[2]);
          int int32 = Convert.ToInt32(strArray[1]);
          if (int32 <= 33)
            set.Add("fish_difficulty_easy");
          else if (int32 <= 66)
            set.Add("fish_difficulty_medium");
          else if (int32 <= 100)
            set.Add("fish_difficulty_hard");
          else
            set.Add("fish_difficulty_extremely_hard");
          set.Add("fish_favor_weather_" + strArray[7]);
        }
      }
      switch (dataOrErrorItem.Category)
      {
        case -999:
          set.Add("category_litter");
          break;
        case -101:
          set.Add("category_trinket");
          break;
        case -100:
          set.Add("category_clothing");
          break;
        case -99:
          set.Add("category_tool");
          break;
        case -98:
          set.Add("category_weapon");
          break;
        case -97:
          set.Add("category_boots");
          break;
        case -96:
          set.Add("category_ring");
          break;
        case -95:
          set.Add("category_hat");
          break;
        case -81:
          set.Add("category_greens");
          break;
        case -80:
          set.Add("category_flowers");
          break;
        case -79:
          set.Add("category_fruits");
          break;
        case -75:
          set.Add("category_vegetable");
          break;
        case -74:
          set.Add("category_seeds");
          break;
        case -29:
          set.Add("category_equipment");
          break;
        case -28:
          set.Add("category_monster_loot");
          break;
        case -27:
          set.Add("category_syrup");
          break;
        case -26:
          set.Add("category_artisan_goods");
          break;
        case -25:
          set.Add("category_ingredients");
          break;
        case -24:
          set.Add("category_furniture");
          break;
        case -23:
          set.Add("category_sell_at_fish_shop");
          break;
        case -22:
          set.Add("category_tackle");
          break;
        case -21:
          set.Add("category_bait");
          break;
        case -20:
          set.Add("category_junk");
          break;
        case -19:
          set.Add("category_fertilizer");
          break;
        case -18:
          set.Add("category_sell_at_pierres_and_marnies");
          break;
        case -17:
          set.Add("category_sell_at_pierres");
          break;
        case -16:
          set.Add("category_building_resources");
          break;
        case -15:
          set.Add("category_metal_resources");
          break;
        case -14:
          set.Add("category_meat");
          break;
        case -12:
          set.Add("category_minerals");
          break;
        case -9:
          set.Add("category_big_craftable");
          break;
        case -8:
          set.Add("category_crafting");
          break;
        case -7:
          set.Add("category_cooking");
          break;
        case -6:
          set.Add("category_milk");
          break;
        case -5:
          set.Add("category_egg");
          break;
        case -4:
          set.Add("category_fish");
          break;
        case -2:
          set.Add("category_gem");
          break;
      }
      ItemContextTagManager.BaseTagsCache[dataOrErrorItem.QualifiedItemId] = set;
    }
    return set;
  }

  /// <summary>Get whether an item has a given base context tag, excluding context tags added dynamically by the item instance.</summary>
  /// <param name="itemId">The qualified or unqualified item ID.</param>
  /// <param name="tag">The tag to match.</param>
  public static bool HasBaseTag(string itemId, string tag)
  {
    return ItemContextTagManager.GetBaseContextTags(itemId).Contains(tag);
  }

  /// <summary>Get whether a tag query string (containing one or more context tags) matches the given item tags.</summary>
  /// <param name="tagQueryString">The comma-delimited list of context tags. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
  /// <param name="tags">The context tags for the item to check.</param>
  public static bool DoesTagQueryMatch(string tagQueryString, HashSet<string> tags)
  {
    return ItemContextTagManager.DoAllTagsMatch((IList<string>) tagQueryString?.Split(','), tags);
  }

  /// <summary>Get whether each tag matches the actual item tags.</summary>
  /// <param name="requiredTags">The tag values to match against the actual tag. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
  /// <param name="actualTags">The actual tags for the item being checked.</param>
  public static bool DoAllTagsMatch(IList<string> requiredTags, HashSet<string> actualTags)
  {
    if (requiredTags == null || requiredTags.Count == 0)
      return false;
    foreach (string requiredTag in (IEnumerable<string>) requiredTags)
    {
      if (!ItemContextTagManager.DoesTagMatch(requiredTag, actualTags))
        return false;
    }
    return true;
  }

  /// <summary>Get whether any tag matches the actual item tags.</summary>
  /// <param name="requiredTags">The tag values to match against the actual tag. Each tag can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
  /// <param name="actualTags">The actual tags for the item being checked.</param>
  public static bool DoAnyTagsMatch(IList<string> requiredTags, HashSet<string> actualTags)
  {
    if (requiredTags != null && requiredTags.Count > 0)
    {
      foreach (string requiredTag in (IEnumerable<string>) requiredTags)
      {
        if (requiredTag != null && requiredTag.Length > 0 && ItemContextTagManager.DoesTagMatch(requiredTag, actualTags))
          return true;
      }
    }
    return false;
  }

  /// <summary>Get whether a single-tag query matches the given item tags.</summary>
  /// <param name="tag">The tag to match. This can be negated by prefixing with <c>!</c> (like <c>!wine_item</c> to check if the tags <em>don't</em> contain <c>wine_item</c>).</param>
  /// <param name="tags">The list of tags to search for a match to <paramref name="tag" />.</param>
  public static bool DoesTagMatch(string tag, HashSet<string> tags)
  {
    if (tag == null)
      return false;
    tag = tag.Trim();
    bool flag = true;
    if (tag.StartsWith('!'))
    {
      tag = tag.Substring(1).TrimStart();
      flag = false;
    }
    return tag.Length > 0 && tags.Contains(tag) == flag;
  }

  /// <summary>Get a tag value with invalid characters (like spaces) escaped.</summary>
  /// <param name="tag">The raw tag value to sanitize.</param>
  public static string SanitizeContextTag(string tag)
  {
    return tag.Trim().ToLower().Replace(' ', '_').Replace("'", "");
  }

  /// <summary>Get the color of an item based on its <c>color_*</c> context tag, if any.</summary>
  /// <param name="item">The item whose context tags to check.</param>
  public static Color? GetColorFromTags(Item item)
  {
    foreach (string contextTag in item.GetContextTags())
    {
      if (contextTag.StartsWithIgnoreCase("color_"))
      {
        string lowerInvariant = contextTag.ToLowerInvariant();
        if (lowerInvariant != null)
        {
          switch (lowerInvariant.Length)
          {
            case 9:
              if (lowerInvariant == "color_red")
                return new Color?(new Color(220, 0, 0));
              continue;
            case 10:
              switch (lowerInvariant[6])
              {
                case 'b':
                  if (lowerInvariant == "color_blue")
                    return new Color?(new Color(46, 85, 183));
                  continue;
                case 'c':
                  if (lowerInvariant == "color_cyan")
                    return new Color?(Color.Cyan);
                  continue;
                case 'g':
                  switch (lowerInvariant)
                  {
                    case "color_gray":
                      return new Color?(Color.Gray);
                    case "color_gold":
                      return new Color?(Color.Gold);
                    default:
                      continue;
                  }
                case 'i':
                  if (lowerInvariant == "color_iron")
                    return new Color?(new Color(197, 213, 224 /*0xE0*/));
                  continue;
                case 'j':
                  if (lowerInvariant == "color_jade")
                    return new Color?(new Color(130, 158, 93));
                  continue;
                case 'l':
                  if (lowerInvariant == "color_lime")
                    return new Color?(Color.Lime);
                  continue;
                case 'p':
                  if (lowerInvariant == "color_pink")
                    return new Color?(new Color((int) byte.MaxValue, 163, 186));
                  continue;
                case 's':
                  if (lowerInvariant == "color_sand")
                    return new Color?(Color.NavajoWhite);
                  continue;
                default:
                  continue;
              }
            case 11:
              switch (lowerInvariant[8])
              {
                case 'a':
                  if (lowerInvariant == "color_black")
                    return new Color?(new Color(45, 45, 45));
                  continue;
                case 'e':
                  if (lowerInvariant == "color_green")
                    return new Color?(new Color(10, 143, 0));
                  continue;
                case 'i':
                  if (lowerInvariant == "color_white")
                    return new Color?(Color.White);
                  continue;
                case 'o':
                  if (lowerInvariant == "color_brown")
                    return new Color?(new Color(130, 73, 37));
                  continue;
                default:
                  continue;
              }
            case 12:
              switch (lowerInvariant[6])
              {
                case 'c':
                  if (lowerInvariant == "color_copper")
                    return new Color?(new Color(179, 85, 0));
                  continue;
                case 'o':
                  if (lowerInvariant == "color_orange")
                    return new Color?(new Color((int) byte.MaxValue, 128 /*0x80*/, 0));
                  continue;
                case 'p':
                  if (lowerInvariant == "color_purple")
                    return new Color?(new Color(115, 41, 181));
                  continue;
                case 's':
                  if (lowerInvariant == "color_salmon")
                    return new Color?(new Color((int) byte.MaxValue, 85, 95));
                  continue;
                case 'y':
                  if (lowerInvariant == "color_yellow")
                    return new Color?(new Color((int) byte.MaxValue, 230, 0));
                  continue;
                default:
                  continue;
              }
            case 13:
              if (lowerInvariant == "color_iridium")
                return new Color?(new Color(105, 15, (int) byte.MaxValue));
              continue;
            case 14:
              if (lowerInvariant == "color_dark_red")
                return new Color?(Color.DarkRed);
              continue;
            case 15:
              switch (lowerInvariant[11])
              {
                case 'b':
                  if (lowerInvariant == "color_dark_blue")
                    return new Color?(Color.DarkBlue);
                  continue;
                case 'c':
                  if (lowerInvariant == "color_dark_cyan")
                    return new Color?(Color.DarkCyan);
                  continue;
                case 'g':
                  if (lowerInvariant == "color_dark_gray")
                    return new Color?(Color.DarkGray);
                  continue;
                case 'p':
                  if (lowerInvariant == "color_dark_pink")
                    return new Color?(Color.DeepPink);
                  continue;
                case 'r':
                  if (lowerInvariant == "color_sea_green")
                    return new Color?(Color.SeaGreen);
                  continue;
                case 's':
                  if (lowerInvariant == "color_poppyseed")
                    return new Color?(new Color(82, 47, 153));
                  continue;
                default:
                  continue;
              }
            case 16 /*0x10*/:
              switch (lowerInvariant[11])
              {
                case '_':
                  if (lowerInvariant == "color_light_cyan")
                    return new Color?(new Color(180, (int) byte.MaxValue, (int) byte.MaxValue));
                  continue;
                case 'a':
                  if (lowerInvariant == "color_aquamarine")
                    return new Color?(Color.Aquamarine);
                  continue;
                case 'b':
                  if (lowerInvariant == "color_dark_brown")
                    return new Color?(Color.SaddleBrown);
                  continue;
                case 'g':
                  if (lowerInvariant == "color_dark_green")
                    return new Color?(Color.DarkGreen);
                  continue;
                default:
                  continue;
              }
            case 17:
              switch (lowerInvariant[11])
              {
                case 'o':
                  if (lowerInvariant == "color_dark_orange")
                    return new Color?(Color.DarkOrange);
                  continue;
                case 'p':
                  if (lowerInvariant == "color_dark_purple")
                    return new Color?(Color.DarkViolet);
                  continue;
                case 'y':
                  if (lowerInvariant == "color_dark_yellow")
                    return new Color?(Color.DarkGoldenrod);
                  continue;
                default:
                  continue;
              }
            case 18:
              if (lowerInvariant == "color_yellow_green")
                return new Color?(Color.GreenYellow);
              continue;
            case 21:
              if (lowerInvariant == "color_pale_violet_red")
                return new Color?(Color.PaleVioletRed);
              continue;
            default:
              continue;
          }
        }
      }
    }
    return new Color?();
  }

  /// <summary>Reset all cached item context tags.</summary>
  /// <remarks>This is called from <see cref="M:StardewValley.ItemRegistry.RebuildCache" /> and generally shouldn't be called directly by other code.</remarks>
  internal static void ResetCache() => ItemContextTagManager.BaseTagsCache.Clear();
}
