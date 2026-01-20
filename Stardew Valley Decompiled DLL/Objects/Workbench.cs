// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Workbench
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Workbench : StardewValley.Object
{
  [XmlIgnore]
  public readonly NetMutex mutex = new NetMutex();

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.mutex.NetFields, "mutex.NetFields");
  }

  public Workbench()
  {
  }

  public Workbench(Vector2 position)
    : base(position, "208")
  {
    this.Name = nameof (Workbench);
    this.type.Value = "Crafting";
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (justCheckingForActivity)
      return true;
    List<Chest> chestList = new List<Chest>();
    Point? fridgePosition = location.GetFridgePosition();
    Vector2[] vector2Array = new Vector2[8]
    {
      new Vector2(-1f, 1f),
      new Vector2(0.0f, 1f),
      new Vector2(1f, 1f),
      new Vector2(-1f, 0.0f),
      new Vector2(1f, 0.0f),
      new Vector2(-1f, -1f),
      new Vector2(0.0f, -1f),
      new Vector2(1f, -1f)
    };
    for (int index = 0; index < vector2Array.Length; ++index)
    {
      Vector2 key = new Vector2((float) (int) ((double) this.tileLocation.X + (double) vector2Array[index].X), (float) (int) ((double) this.tileLocation.Y + (double) vector2Array[index].Y));
      int x1 = (int) this.tileLocation.X;
      int? x2 = fridgePosition?.X;
      int valueOrDefault = x2.GetValueOrDefault();
      if (x1 == valueOrDefault & x2.HasValue && (int) this.tileLocation.Y == fridgePosition.Value.Y)
      {
        Chest fridge = location.GetFridge();
        if (fridge != null)
          chestList.Add(fridge);
      }
      StardewValley.Object @object;
      if (location.objects.TryGetValue(key, out @object) && @object is Chest chest && (chest.SpecialChestType == Chest.SpecialChestTypes.None || chest.SpecialChestType == Chest.SpecialChestTypes.BigChest))
        chestList.Add(chest);
    }
    List<NetMutex> mutexes = new List<NetMutex>();
    List<IInventory> inventories = new List<IInventory>();
    foreach (Chest chest in chestList)
    {
      mutexes.Add(chest.mutex);
      inventories.Add((IInventory) chest.Items);
    }
    if (!this.mutex.IsLocked())
    {
      MultipleMutexRequest multipleMutexRequest = new MultipleMutexRequest(mutexes, (Action<MultipleMutexRequest>) (request => this.mutex.RequestLock((Action) (() =>
      {
        Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
        Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, standaloneMenu: true, materialContainers: inventories);
        Game1.activeClickableMenu.exitFunction = (IClickableMenu.onExit) (() =>
        {
          this.mutex.ReleaseLock();
          request.ReleaseLocks();
        });
      }), new Action(request.ReleaseLocks))), (Action<MultipleMutexRequest>) (request => Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Workbench_Chest_Warning"))));
    }
    return true;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location = this.Location;
    if (location != null)
      this.mutex.Update(location);
    base.updateWhenCurrentLocation(time);
  }
}
