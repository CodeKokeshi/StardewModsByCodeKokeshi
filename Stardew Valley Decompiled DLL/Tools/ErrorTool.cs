// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.ErrorTool
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Tools;

/// <summary>A broken tool used when we can't create a specific tool type.</summary>
public class ErrorTool : Tool
{
  public ErrorTool()
    : base("Error Item", 0, 0, 0, false)
  {
  }

  public ErrorTool(string itemId, int upgradeLevel = 0, int numAttachmentSlots = 0)
    : base("Error Item", upgradeLevel, 0, 0, false, numAttachmentSlots)
  {
    this.ItemId = itemId;
    this.Name = "Error Item";
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    return (Item) new ErrorTool(this.ItemId, this.UpgradeLevel, this.numAttachmentSlots.Value);
  }

  protected override string loadDescription()
  {
    return ItemRegistry.RequireTypeDefinition("(T)").GetErrorData(this.ItemId).Description;
  }

  protected override string loadDisplayName()
  {
    return ItemRegistry.RequireTypeDefinition("(T)").GetErrorData(this.ItemId).DisplayName;
  }
}
