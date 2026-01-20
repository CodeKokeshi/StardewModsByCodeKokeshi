// Decompiled with JetBrains decompiler
// Type: StardewValley.BundleGenerator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Extensions;
using StardewValley.GameData.Bundles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#nullable disable
namespace StardewValley;

public class BundleGenerator
{
  public List<RandomBundleData> randomBundleData;
  public Dictionary<string, string> bundleData;
  public Random random;

  public Dictionary<string, string> Generate(List<RandomBundleData> bundle_data, Random rng)
  {
    this.random = rng;
    this.randomBundleData = bundle_data;
    this.bundleData = new Dictionary<string, string>((IDictionary<string, string>) DataLoader.Bundles(Game1.content));
    foreach (RandomBundleData randomBundleData in this.randomBundleData)
    {
      List<int> intList = new List<int>();
      string[] strArray1 = ArgUtility.SplitBySpace(randomBundleData.Keys);
      Dictionary<int, BundleData> dictionary = new Dictionary<int, BundleData>();
      foreach (string s in strArray1)
        intList.Add(int.Parse(s));
      BundleSetData bundleSetData = this.random.ChooseFrom<BundleSetData>((IList<BundleSetData>) randomBundleData.BundleSets);
      if (bundleSetData != null)
      {
        foreach (BundleData bundle in bundleSetData.Bundles)
          dictionary[bundle.Index] = bundle;
      }
      List<BundleData> bundleDataList = new List<BundleData>();
      foreach (BundleData bundle in randomBundleData.Bundles)
        bundleDataList.Add(bundle);
      for (int key = 0; key < intList.Count; ++key)
      {
        if (!dictionary.ContainsKey(key))
        {
          List<BundleData> options = new List<BundleData>();
          foreach (BundleData bundleData in bundleDataList)
          {
            if (bundleData.Index == key)
              options.Add(bundleData);
          }
          if (options.Count > 0)
          {
            BundleData bundleData = this.random.ChooseFrom<BundleData>((IList<BundleData>) options);
            bundleDataList.Remove(bundleData);
            dictionary[key] = bundleData;
          }
          else
          {
            foreach (BundleData bundleData in bundleDataList)
            {
              if (bundleData.Index == -1)
                options.Add(bundleData);
            }
            if (options.Count > 0)
            {
              BundleData bundleData = this.random.ChooseFrom<BundleData>((IList<BundleData>) options);
              bundleDataList.Remove(bundleData);
              dictionary[key] = bundleData;
            }
          }
        }
      }
      foreach (int key1 in dictionary.Keys)
      {
        BundleData bundleData1 = dictionary[key1];
        StringBuilder builder = new StringBuilder();
        builder.Append(bundleData1.Name);
        builder.Append("/");
        string str1 = bundleData1.Reward;
        if (str1.Length > 0)
        {
          try
          {
            if (char.IsDigit(str1[0]))
            {
              string[] strArray2 = ArgUtility.SplitBySpace(str1);
              int stack_count = int.Parse(strArray2[0]);
              Item obj = Utility.fuzzyItemSearch(string.Join(" ", strArray2, 1, strArray2.Length - 1), stack_count);
              if (obj != null)
                str1 = Utility.getStandardDescriptionFromItem(obj, obj.Stack);
            }
          }
          catch (Exception ex)
          {
            Game1.log.Error("ERROR: Malformed reward string in bundle: " + str1, ex);
            str1 = bundleData1.Reward;
          }
        }
        builder.Append(str1);
        builder.Append("/");
        int color1 = 0;
        string color2 = bundleData1.Color;
        int length;
        if (color2 != null)
        {
          length = color2.Length;
          switch (length)
          {
            case 3:
              if (color2 == "Red")
              {
                color1 = 4;
                break;
              }
              break;
            case 4:
              switch (color2[0])
              {
                case 'B':
                  if (color2 == "Blue")
                  {
                    color1 = 5;
                    break;
                  }
                  break;
                case 'T':
                  if (color2 == "Teal")
                  {
                    color1 = 6;
                    break;
                  }
                  break;
              }
              break;
            case 5:
              if (color2 == "Green")
              {
                color1 = 0;
                break;
              }
              break;
            case 6:
              switch (color2[0])
              {
                case 'O':
                  if (color2 == "Orange")
                  {
                    color1 = 2;
                    break;
                  }
                  break;
                case 'P':
                  if (color2 == "Purple")
                  {
                    color1 = 1;
                    break;
                  }
                  break;
                case 'Y':
                  if (color2 == "Yellow")
                  {
                    color1 = 3;
                    break;
                  }
                  break;
              }
              break;
          }
        }
        this.ParseItemList(builder, bundleData1.Items, bundleData1.Pick, bundleData1.RequiredItems, color1);
        builder.Append("/");
        builder.Append(bundleData1.Sprite);
        builder.Append('/');
        builder.Append(bundleData1.Name);
        Dictionary<string, string> bundleData2 = this.bundleData;
        string areaName = randomBundleData.AreaName;
        length = intList[key1];
        string str2 = length.ToString();
        string key2 = $"{areaName}/{str2}";
        string str3 = builder.ToString();
        bundleData2[key2] = str3;
      }
    }
    return this.bundleData;
  }

  public string ParseRandomTags(string data)
  {
    int startIndex;
    do
    {
      startIndex = data.LastIndexOf('[');
      if (startIndex >= 0)
      {
        int num = data.IndexOf(']', startIndex);
        if (num == -1)
          return data;
        string str = this.random.ChooseFrom<string>((IList<string>) data.Substring(startIndex + 1, num - startIndex - 1).Split('|'));
        data = data.Remove(startIndex, num - startIndex + 1);
        data = data.Insert(startIndex, str);
      }
    }
    while (startIndex >= 0);
    return data;
  }

  public Item ParseItemString(string item_string)
  {
    string[] strArray = ArgUtility.SplitBySpace(item_string);
    int index = 0;
    int amount = int.Parse(strArray[index]);
    int startIndex = index + 1;
    int num = 0;
    switch (strArray[startIndex])
    {
      case "NQ":
        num = 0;
        ++startIndex;
        break;
      case "SQ":
        num = 1;
        ++startIndex;
        break;
      case "GQ":
        num = 2;
        ++startIndex;
        break;
      case "IQ":
        num = 3;
        ++startIndex;
        break;
    }
    string str = string.Join(" ", strArray, startIndex, strArray.Length - startIndex);
    if (char.IsDigit(str[0]))
    {
      Item itemString = ItemRegistry.Create("(O)" + str, amount);
      itemString.Quality = num;
      return itemString;
    }
    Item itemString1 = (Item) null;
    if (str.EndsWithIgnoreCase("category"))
    {
      try
      {
        FieldInfo field = typeof (Object).GetField(str);
        if (field != (FieldInfo) null)
          itemString1 = (Item) new Object(((int) field.GetValue((object) null)).ToString(), 1);
      }
      catch (Exception ex)
      {
      }
    }
    if (itemString1 == null)
    {
      itemString1 = Utility.fuzzyItemSearch(str);
      itemString1.Quality = num;
    }
    if (itemString1 == null)
      throw new Exception($"Invalid item name '{str}' encountered while generating a bundle.");
    itemString1.Stack = amount;
    return itemString1;
  }

  public void ParseItemList(
    StringBuilder builder,
    string item_list,
    int pick_count,
    int required_items,
    int color)
  {
    item_list = this.ParseRandomTags(item_list);
    string[] strArray = item_list.Split(',');
    List<string> stringList = new List<string>();
    for (int index = 0; index < strArray.Length; ++index)
    {
      Item itemString = this.ParseItemString(strArray[index]);
      stringList.Add($"{itemString.ItemId} {itemString.Stack.ToString()} {itemString.Quality.ToString()}");
    }
    if (pick_count < 0)
      pick_count = stringList.Count;
    if (required_items < 0)
      required_items = pick_count;
    while (stringList.Count > pick_count)
    {
      int index = this.random.Next(stringList.Count);
      stringList.RemoveAt(index);
    }
    for (int index = 0; index < stringList.Count; ++index)
    {
      builder.Append(stringList[index]);
      if (index < stringList.Count - 1)
        builder.Append(" ");
    }
    builder.Append("/");
    builder.Append(color);
    builder.Append("/");
    builder.Append(required_items);
  }
}
