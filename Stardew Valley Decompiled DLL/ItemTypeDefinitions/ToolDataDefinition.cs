// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.ToolDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.GameData.Tools;
using StardewValley.TokenizableStrings;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for tool items.</summary>
public class ToolDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(T)";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) Game1.toolData.Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.toolData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    ToolData rawData = this.GetRawData(itemId);
    return rawData == null ? (ParsedItemData) null : new ParsedItemData((IItemDataDefinition) this, itemId, rawData.MenuSpriteIndex > -1 ? rawData.MenuSpriteIndex : rawData.SpriteIndex, rawData.Texture, itemId, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), -99, (string) null, (object) rawData);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return texture != null ? Game1.getSquareSourceRectForNonStandardTileSheet(texture, 16 /*0x10*/, 16 /*0x10*/, spriteIndex) : throw new ArgumentNullException(nameof (texture));
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    ToolData toolData = data != null ? this.GetRawData(data.ItemId) : throw new ArgumentNullException(nameof (data));
    Tool toolInstance = this.CreateToolInstance(data, toolData);
    if (toolInstance == null)
      return (Item) this.GetErrorTool(data);
    toolInstance.ItemId = data.ItemId;
    toolInstance.SetSpriteIndex(toolData.SpriteIndex);
    if (toolData.MenuSpriteIndex > -1)
      toolInstance.IndexOfMenuItemView = toolData.MenuSpriteIndex;
    toolInstance.Name = toolData.Name;
    if (toolData.UpgradeLevel > -1)
      toolInstance.UpgradeLevel = toolData.UpgradeLevel;
    if (toolData.AttachmentSlots > -1)
      toolInstance.AttachmentSlotsCount = toolData.AttachmentSlots;
    if (toolData.SetProperties != null)
    {
      Type type = toolInstance.GetType();
      foreach (KeyValuePair<string, string> setProperty in toolData.SetProperties)
        this.TrySetProperty(type, toolInstance, setProperty.Key, setProperty.Value);
    }
    if (toolData.ModData != null)
    {
      foreach (KeyValuePair<string, string> keyValuePair in toolData.ModData)
        toolInstance.modData[keyValuePair.Key] = keyValuePair.Value;
    }
    return (Item) toolInstance;
  }

  /// <summary>Get the raw data fields for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  protected ToolData GetRawData(string itemId)
  {
    ToolData toolData;
    return itemId == null || !Game1.toolData.TryGetValue(itemId, out toolData) ? (ToolData) null : toolData;
  }

  /// <summary>Create an empty instance of a tool's type, if valid.</summary>
  /// <param name="itemData">The parsed item data.</param>
  /// <param name="toolData">The tool data.</param>
  /// <remarks>Note for mods: this method deliberately doesn't allow custom types that aren't part of the game code because that will cause crashes in multiplayer or when saving the game. If you patch this logic to allow a custom class type, you should be aware of the consequences and avoid permanently breaking players' saves when your mod is removed.</remarks>
  protected Tool CreateToolInstance(ParsedItemData itemData, ToolData toolData)
  {
    if (itemData != null && toolData != null)
    {
      Type type = typeof (Tool).Assembly.GetType("StardewValley.Tools." + toolData.ClassName);
      if (type != (Type) null)
      {
        Tool instance = (Tool) Activator.CreateInstance(type);
        if (instance != null)
          return instance;
      }
    }
    return this.GetErrorTool(itemData);
  }

  /// <summary>Create an Error Item tool, for use when we don't have a class to initialize.</summary>
  /// <param name="data">The item data.</param>
  protected Tool GetErrorTool(ParsedItemData data) => (Tool) new ErrorTool(data.ItemId);

  /// <summary>Set a tool property.</summary>
  /// <param name="type">The tool type.</param>
  /// <param name="tool">The tool instance.</param>
  /// <param name="name">The property name.</param>
  /// <param name="rawValue">The raw property value.</param>
  protected void TrySetProperty(Type type, Tool tool, string name, string rawValue)
  {
    MemberInfo memberInfo = (MemberInfo) type.GetProperty(name);
    if ((object) memberInfo == null)
      memberInfo = (MemberInfo) type.GetField(name);
    MemberInfo info = memberInfo;
    if (info == (MemberInfo) null)
    {
      Game1.log.Error($"Can't set field or property '{name}' for tool '{tool.QualifiedItemId}': the {type.FullName} class has none public with that name");
    }
    else
    {
      string error;
      if (info.TrySetValueFromString((object) tool, rawValue, (object[]) null, out error))
        return;
      Game1.log.Error($"Can't set {((object) (info as FieldInfo) != null ? "field" : "property")} '{name}' for tool '{tool.QualifiedItemId}': {error}.");
    }
  }
}
