// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.GenericTool
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Tools;

/// <summary>A generic tool instance with no logic of its own, used for cases where the logic is applied elsewhere.</summary>
public class GenericTool : Tool
{
  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new GenericTool();
}
