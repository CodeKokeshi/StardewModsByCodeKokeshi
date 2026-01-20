// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandShrine
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Locations;

public class IslandShrine : IslandForestLocation
{
  [XmlIgnore]
  public ItemPedestal northPedestal;
  [XmlIgnore]
  public ItemPedestal southPedestal;
  [XmlIgnore]
  public ItemPedestal eastPedestal;
  [XmlIgnore]
  public ItemPedestal westPedestal;
  [XmlIgnore]
  public NetEvent0 puzzleFinishedEvent = new NetEvent0();
  [XmlElement("puzzleFinished")]
  public NetBool puzzleFinished = new NetBool();

  public IslandShrine()
  {
  }

  public IslandShrine(string map, string name)
    : base(map, name)
  {
    this.AddMissingPedestals();
  }

  public override List<Vector2> GetAdditionalWalnutBushes()
  {
    return new List<Vector2>() { new Vector2(23f, 34f) };
  }

  public ItemPedestal AddOrUpdatePedestal(Vector2 position, string birdLocation)
  {
    ItemPedestal itemPedestal1 = this.getObjectAtTile((int) position.X, (int) position.Y) as ItemPedestal;
    string itemIndex = IslandGemBird.GetItemIndex(IslandGemBird.GetBirdTypeForLocation(birdLocation));
    if (itemPedestal1 == null || !itemPedestal1.isIslandShrinePedestal.Value)
    {
      OverlaidDictionary objects = this.objects;
      Vector2 key = position;
      ItemPedestal itemPedestal2 = new ItemPedestal(position, (Object) null, false, Color.White);
      itemPedestal2.Fragility = 2;
      itemPedestal2.isIslandShrinePedestal.Value = true;
      itemPedestal1 = itemPedestal2;
      objects[key] = (Object) itemPedestal2;
    }
    itemPedestal1.successColor.Value = Color.Transparent;
    if (itemPedestal1.requiredItem.Value?.ItemId != itemIndex)
    {
      itemPedestal1.requiredItem.Value = new Object(itemIndex, 1);
      if (itemPedestal1.heldObject.Value?.ItemId != itemIndex)
        itemPedestal1.heldObject.Value = (Object) null;
    }
    return itemPedestal1;
  }

  public virtual void AddMissingPedestals()
  {
    this.westPedestal = this.AddOrUpdatePedestal(new Vector2(21f, 27f), "IslandWest");
    this.eastPedestal = this.AddOrUpdatePedestal(new Vector2(27f, 27f), "IslandEast");
    this.southPedestal = this.AddOrUpdatePedestal(new Vector2(24f, 28f), "IslandSouth");
    this.northPedestal = this.AddOrUpdatePedestal(new Vector2(24f, 25f), "IslandNorth");
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.puzzleFinished, "puzzleFinished").AddField((INetSerializable) this.puzzleFinishedEvent, "puzzleFinishedEvent");
    this.puzzleFinishedEvent.onEvent += new NetEvent0.Event(this.OnPuzzleFinish);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (!Game1.IsMasterGame)
      return;
    this.AddMissingPedestals();
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (!this.puzzleFinished.Value)
      return;
    this.ApplyFinishedTiles();
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    if (!(l is IslandShrine islandShrine))
      return;
    this.northPedestal = islandShrine.getObjectAtTile((int) this.northPedestal.TileLocation.X, (int) this.northPedestal.TileLocation.Y) as ItemPedestal;
    this.southPedestal = islandShrine.getObjectAtTile((int) this.southPedestal.TileLocation.X, (int) this.southPedestal.TileLocation.Y) as ItemPedestal;
    this.eastPedestal = islandShrine.getObjectAtTile((int) this.eastPedestal.TileLocation.X, (int) this.eastPedestal.TileLocation.Y) as ItemPedestal;
    this.westPedestal = islandShrine.getObjectAtTile((int) this.westPedestal.TileLocation.X, (int) this.westPedestal.TileLocation.Y) as ItemPedestal;
    this.puzzleFinished.Value = islandShrine.puzzleFinished.Value;
  }

  public void OnPuzzleFinish()
  {
    if (Game1.IsMasterGame)
    {
      for (int index = 0; index < 5; ++index)
        Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(24f, 19f) * 64f, -1, (GameLocation) this);
    }
    if (Game1.currentLocation != this)
      return;
    Game1.playSound("boulderBreak");
    Game1.playSound("secret1");
    Game1.flashAlpha = 1f;
    this.ApplyFinishedTiles();
  }

  public virtual void ApplyFinishedTiles()
  {
    this.setMapTile(23, 19, 142, "AlwaysFront", "untitled tile sheet3");
    this.setMapTile(24, 19, 143, "AlwaysFront", "untitled tile sheet3");
    this.setMapTile(25, 19, 144 /*0x90*/, "AlwaysFront", "untitled tile sheet3");
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (!Game1.IsMasterGame || this.puzzleFinished.Value || !this.northPedestal.match.Value || !this.southPedestal.match.Value || !this.eastPedestal.match.Value || !this.westPedestal.match.Value)
      return;
    Game1.player.team.MarkCollectedNut("IslandShrinePuzzle");
    this.puzzleFinishedEvent.Fire();
    this.puzzleFinished.Value = true;
    this.northPedestal.locked.Value = true;
    this.northPedestal.heldObject.Value = (Object) null;
    this.southPedestal.locked.Value = true;
    this.southPedestal.heldObject.Value = (Object) null;
    this.eastPedestal.locked.Value = true;
    this.eastPedestal.heldObject.Value = (Object) null;
    this.westPedestal.locked.Value = true;
    this.westPedestal.heldObject.Value = (Object) null;
  }
}
