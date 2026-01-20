// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DwarfGate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using xTile.ObjectModel;

#nullable disable
namespace StardewValley.Locations;

public class DwarfGate : INetObject<NetFields>
{
  public NetPoint tilePosition;
  public NetLocationRef locationRef;
  public bool triggeredOpen;
  public NetPointDictionary<bool, NetBool> switches;
  public Dictionary<Point, bool> localSwitches;
  public NetBool opened;
  public bool localOpened;
  public NetInt pressedSwitches;
  public int localPressedSwitches;
  public NetInt gateIndex;
  public NetEvent0 openEvent;
  public NetEvent1Field<Point, NetPoint> pressEvent;

  public NetFields NetFields { get; }

  public DwarfGate()
  {
    NetPointDictionary<bool, NetBool> netPointDictionary = new NetPointDictionary<bool, NetBool>();
    netPointDictionary.InterpolationWait = false;
    this.switches = netPointDictionary;
    this.localSwitches = new Dictionary<Point, bool>();
    this.opened = new NetBool(false);
    NetInt netInt = new NetInt(0);
    netInt.InterpolationWait = false;
    this.pressedSwitches = netInt;
    this.gateIndex = new NetInt(0);
    this.openEvent = new NetEvent0();
    NetEvent1Field<Point, NetPoint> netEvent1Field = new NetEvent1Field<Point, NetPoint>();
    netEvent1Field.InterpolationWait = false;
    this.pressEvent = netEvent1Field;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.InitNetFields();
  }

  public DwarfGate(VolcanoDungeon location, int gate_index, int x, int y, int seed)
    : this()
  {
    this.locationRef.Value = (GameLocation) location;
    this.tilePosition.X = x;
    this.tilePosition.Y = y;
    this.gateIndex.Value = gate_index;
    Random random = Utility.CreateRandom((double) seed);
    List<Point> collection;
    if (location.possibleSwitchPositions.TryGetValue(gate_index, out collection))
    {
      int val2 = Math.Min(collection.Count, 3);
      if (gate_index > 0)
        val2 = 1;
      List<Point> list = new List<Point>((IEnumerable<Point>) collection);
      Utility.Shuffle<Point>(random, list);
      int num = Math.Min(random.Next(1, Math.Max(1, val2)), val2);
      if (location.isMonsterLevel())
        num = val2;
      for (int index = 0; index < num; ++index)
        this.switches[list[index]] = false;
    }
    this.UpdateLocalStates();
    this.ApplyTiles();
  }

  public virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.tilePosition, "tilePosition").AddField((INetSerializable) this.locationRef.NetFields, "locationRef.NetFields").AddField((INetSerializable) this.switches, "switches").AddField((INetSerializable) this.pressedSwitches, "pressedSwitches").AddField(this.openEvent.NetFields, "openEvent.NetFields").AddField((INetSerializable) this.opened, "opened").AddField(this.pressEvent.NetFields, "pressEvent.NetFields").AddField((INetSerializable) this.gateIndex, "gateIndex");
    this.pressEvent.onEvent += new AbstractNetEvent1<Point>.Event(this.OnPress);
    this.openEvent.onEvent += new NetEvent0.Event(this.OpenGate);
  }

  public virtual void OnPress(Point point)
  {
    bool flag;
    if (Game1.IsMasterGame && this.switches.TryGetValue(point, out flag) && !flag)
    {
      this.switches[point] = true;
      ++this.pressedSwitches.Value;
    }
    if (Game1.currentLocation == this.locationRef.Value)
      Game1.playSound("openBox");
    this.localSwitches[point] = true;
    this.ApplyTiles();
  }

  public virtual void OpenGate()
  {
    if (Game1.currentLocation == this.locationRef.Value)
      Game1.playSound("cowboy_gunload");
    if (Game1.IsMasterGame)
    {
      if (this.gateIndex.Value == -1 && !Game1.MasterPlayer.hasOrWillReceiveMail("volcanoShortcutUnlocked"))
        Game1.addMailForTomorrow("volcanoShortcutUnlocked", true);
      this.opened.Value = true;
    }
    this.localOpened = true;
    this.ApplyTiles();
  }

  public virtual void ResetLocalState()
  {
    this.UpdateLocalStates();
    this.ApplyTiles();
  }

  public virtual void UpdateLocalStates()
  {
    this.localOpened = this.opened.Value;
    this.localPressedSwitches = this.pressedSwitches.Value;
    foreach (Point key in this.switches.Keys)
      this.localSwitches[key] = this.switches[key];
  }

  public virtual void Draw(SpriteBatch b)
  {
    if (this.localOpened)
      return;
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) this.tilePosition.X, (float) this.tilePosition.Y) * 64f + new Vector2(1f, -5f) * 4f), new Rectangle?(new Rectangle(178, 189, 14, 34)), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, (float) ((this.tilePosition.Y + 2) * 64 /*0x40*/) / 10000f);
  }

  public virtual void UpdateWhenCurrentLocation(GameTime time, GameLocation location)
  {
    this.openEvent.Poll();
    this.pressEvent.Poll();
    if (this.localPressedSwitches != this.pressedSwitches.Value)
    {
      this.localPressedSwitches = this.pressedSwitches.Value;
      this.ApplyTiles();
    }
    if (!this.localOpened && this.opened.Value)
    {
      this.localOpened = true;
      this.ApplyTiles();
    }
    foreach (Point key in this.switches.Keys)
    {
      if (this.switches[key] && !this.localSwitches[key])
      {
        this.localSwitches[key] = true;
        this.ApplyTiles();
      }
    }
  }

  public virtual void ApplyTiles()
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    foreach (Point key in this.localSwitches.Keys)
    {
      ++num1;
      if (this.switches[key])
        ++num3;
      if (this.localSwitches[key])
      {
        ++num2;
        ((IDictionary<string, PropertyValue>) this.locationRef.Value.setMapTile(key.X, key.Y, VolcanoDungeon.GetTileIndex(1, 31 /*0x1F*/), "Back", "dungeon").Properties).Remove("TouchAction");
      }
      else
        this.locationRef.Value.setMapTile(key.X, key.Y, VolcanoDungeon.GetTileIndex(0, 31 /*0x1F*/), "Back", "dungeon").Properties["TouchAction"] = (PropertyValue) "DwarfSwitch";
    }
    switch (num1)
    {
      case 1:
        this.locationRef.Value.setMapTile(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(10 + num2, 23), "Buildings", "dungeon");
        break;
      case 2:
        this.locationRef.Value.setMapTile(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(12 + num2, 23), "Buildings", "dungeon");
        break;
      case 3:
        this.locationRef.Value.setMapTile(this.tilePosition.X - 1, this.tilePosition.Y, VolcanoDungeon.GetTileIndex(10 + num2, 22), "Buildings", "dungeon");
        break;
    }
    if (!this.triggeredOpen && num3 >= num1)
    {
      this.triggeredOpen = true;
      if (Game1.IsMasterGame)
        DelayedAction.functionAfterDelay(new Action(this.openEvent.Fire), 500);
    }
    if (this.localOpened)
      this.locationRef.Value.removeTile(this.tilePosition.X, this.tilePosition.Y + 1, "Buildings");
    else
      this.locationRef.Value.setMapTile(this.tilePosition.X, this.tilePosition.Y + 1, 0, "Buildings", "dungeon");
  }
}
