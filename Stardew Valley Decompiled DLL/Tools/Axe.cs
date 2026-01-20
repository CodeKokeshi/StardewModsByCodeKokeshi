// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Axe
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using System;

#nullable disable
namespace StardewValley.Tools;

public class Axe : Tool
{
  public NetInt additionalPower = new NetInt(0);

  public Axe()
    : base(nameof (Axe), 0, 189, 215, false)
  {
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.additionalPower, "additionalPower");
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    switch (this.UpgradeLevel)
    {
      case 0:
        this.ItemId = nameof (Axe);
        break;
      case 1:
        this.ItemId = "CopperAxe";
        break;
      case 2:
        this.ItemId = "SteelAxe";
        break;
      case 3:
        this.ItemId = "GoldAxe";
        break;
      case 4:
        this.ItemId = "IridiumAxe";
        break;
      default:
        this.ItemId = nameof (Axe);
        break;
    }
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Axe();

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    this.Update(who.FacingDirection, 0, who);
    who.EndUsingTool();
    return true;
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    if (!this.isEfficient.Value)
      who.Stamina -= (float) (2 * power) - (float) who.ForagingLevel * 0.1f;
    int num1 = x / 64 /*0x40*/;
    int num2 = y / 64 /*0x40*/;
    Rectangle tileRect = new Rectangle(num1 * 64 /*0x40*/, num2 * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    Vector2 tile = new Vector2((float) num1, (float) num2);
    if (location.Map.RequireLayer("Buildings").Tiles[num1, num2] != null && location.Map.RequireLayer("Buildings").Tiles[num1, num2].TileIndexProperties.ContainsKey("TreeStump"))
    {
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.14023"));
    }
    else
    {
      this.upgradeLevel.Value += this.additionalPower.Value;
      location.performToolAction((Tool) this, num1, num2);
      TerrainFeature terrainFeature;
      if (location.terrainFeatures.TryGetValue(tile, out terrainFeature) && terrainFeature.performToolAction((Tool) this, 0, tile))
        location.terrainFeatures.Remove(tile);
      location.largeTerrainFeatures?.RemoveWhere((Func<LargeTerrainFeature, bool>) (largeFeature => largeFeature.getBoundingBox().Intersects(tileRect) && largeFeature.performToolAction((Tool) this, 0, tile)));
      Vector2 key = new Vector2((float) num1, (float) num2);
      StardewValley.Object @object;
      if (location.Objects.TryGetValue(key, out @object) && @object.Type != null && @object.performToolAction((Tool) this))
      {
        if (@object.Type == "Crafting" && @object.fragility.Value != 2)
          location.debris.Add(new Debris(@object.QualifiedItemId, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel)));
        @object.performRemoveAction();
        location.Objects.Remove(key);
      }
      this.upgradeLevel.Value -= this.additionalPower.Value;
    }
  }
}
