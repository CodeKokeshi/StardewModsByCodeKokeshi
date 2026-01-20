// Decompiled with JetBrains decompiler
// Type: StardewValley.SlimeHutch
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class SlimeHutch : DecoratableLocation
{
  [XmlElement("slimeMatingsLeft")]
  public readonly NetInt slimeMatingsLeft = new NetInt();
  public readonly NetArray<bool, NetBool> waterSpots = new NetArray<bool, NetBool>(4);
  protected int _slimeCapacity = -1;

  public SlimeHutch()
  {
  }

  public SlimeHutch(string m, string name)
    : base(m, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.slimeMatingsLeft, "slimeMatingsLeft").AddField((INetSerializable) this.waterSpots, "waterSpots");
  }

  /// <inheritdoc />
  public override void OnParentBuildingUpgraded(Building building)
  {
    base.OnParentBuildingUpgraded(building);
    this._slimeCapacity = -1;
  }

  public bool isFull()
  {
    if (this._slimeCapacity < 0)
      this._slimeCapacity = this.ParentBuilding?.GetData()?.MaxOccupants ?? 20;
    return this.characters.Count >= this._slimeCapacity;
  }

  public override bool canSlimeMateHere()
  {
    int num = this.slimeMatingsLeft.Value;
    --this.slimeMatingsLeft.Value;
    return !this.isFull() && num > 0;
  }

  public override bool canSlimeHatchHere() => !this.isFull();

  public override void DayUpdate(int dayOfMonth)
  {
    int val2 = 0;
    int num1 = Game1.random.Next(this.waterSpots.Length);
    for (int index = 0; index < this.waterSpots.Length; ++index)
    {
      if (this.waterSpots[(index + num1) % this.waterSpots.Length] && val2 * 5 < this.characters.Count)
      {
        ++val2;
        this.waterSpots[(index + num1) % this.waterSpots.Length] = false;
      }
    }
    foreach (Object @object in this.objects.Values)
    {
      if (@object.IsSprinkler())
      {
        foreach (Vector2 sprinklerTile in @object.GetSprinklerTiles())
        {
          if ((double) sprinklerTile.X == 16.0 && (double) sprinklerTile.Y >= 6.0 && (double) sprinklerTile.Y <= 9.0)
            this.waterSpots[(int) sprinklerTile.Y - 6] = true;
        }
      }
    }
    for (int index = Math.Min(this.characters.Count / 5, val2); index > 0; --index)
    {
      int num2 = 50;
      Vector2 randomTile;
      for (randomTile = this.getRandomTile(); (!this.CanItemBePlacedHere(randomTile, ignorePassables: CollisionMask.None) || this.doesTileHaveProperty((int) randomTile.X, (int) randomTile.Y, "NPCBarrier", "Back") != null || (double) randomTile.Y >= 12.0) && num2 > 0; --num2)
        randomTile = this.getRandomTile();
      if (num2 > 0)
      {
        Object @object = ItemRegistry.Create<Object>("(BC)56");
        @object.fragility.Value = 2;
        this.objects.Add(randomTile, @object);
      }
    }
    NetInt slimeMatingsLeft;
    for (; this.slimeMatingsLeft.Value > 0; --slimeMatingsLeft.Value)
    {
      if (this.characters.Count > 1 && !this.isFull() && this.characters[Game1.random.Next(this.characters.Count)] is GreenSlime character && character.ageUntilFullGrown.Value <= 0)
      {
        for (int index = 1; index < 10; ++index)
        {
          GreenSlime mateToPursue = (GreenSlime) Utility.checkForCharacterWithinArea(character.GetType(), character.Position, (GameLocation) this, new Rectangle((int) character.Position.X - 64 /*0x40*/ * index, (int) character.Position.Y - 64 /*0x40*/ * index, 64 /*0x40*/ * (index * 2 + 1), 64 /*0x40*/ * (index * 2 + 1)));
          if (mateToPursue != null && mateToPursue.cute.Value != character.cute.Value && mateToPursue.ageUntilFullGrown.Value <= 0)
          {
            character.mateWith(mateToPursue, (GameLocation) this);
            break;
          }
        }
      }
      slimeMatingsLeft = this.slimeMatingsLeft;
    }
    this.slimeMatingsLeft.Value = this.characters.Count / 5 + 1;
    base.DayUpdate(dayOfMonth);
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is SlimeHutch slimeHutch)
    {
      for (int index = 0; index < this.waterSpots.Length; ++index)
      {
        if (index < slimeHutch.waterSpots.Count)
          this.waterSpots[index] = slimeHutch.waterSpots[index];
      }
    }
    base.TransferDataFromSavedLocation(l);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    Object @object;
    if (!this.objects.TryGetValue(new Vector2(1f, 4f), out @object))
      return;
    @object.Fragility = 0;
  }

  public override bool performToolAction(Tool t, int tileX, int tileY)
  {
    if (t is WateringCan && tileX == 16 /*0x10*/ && tileY >= 6 && tileY <= 9)
      this.waterSpots[tileY - 6] = true;
    return false;
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    for (int index1 = 0; index1 < this.waterSpots.Length; ++index1)
    {
      int index2 = this.waterSpots[index1] ? 2135 : 2134;
      this.setMapTile(16 /*0x10*/, 6 + index1, index2, "Buildings", "untitled tile sheet");
    }
  }
}
