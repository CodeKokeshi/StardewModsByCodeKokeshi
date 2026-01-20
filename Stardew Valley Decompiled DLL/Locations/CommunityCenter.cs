// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.CommunityCenter
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class CommunityCenter : GameLocation
{
  public const int AREA_Pantry = 0;
  public const int AREA_FishTank = 2;
  public const int AREA_CraftsRoom = 1;
  public const int AREA_BoilerRoom = 3;
  public const int AREA_Vault = 4;
  public const int AREA_Bulletin = 5;
  public const int AREA_AbandonedJojaMart = 6;
  public const int AREA_Bulletin2 = 7;
  public const int AREA_JunimoHut = 8;
  [XmlElement("warehouse")]
  private readonly NetBool warehouse = new NetBool();
  [XmlIgnore]
  public List<NetMutex> bundleMutexes = new List<NetMutex>();
  public readonly NetArray<bool, NetBool> areasComplete = new NetArray<bool, NetBool>(6);
  [XmlElement("numberOfStarsOnPlaque")]
  public readonly NetInt numberOfStarsOnPlaque = new NetInt();
  [XmlIgnore]
  private readonly NetEvent0 newJunimoNoteCheckEvent = new NetEvent0();
  [XmlIgnore]
  private readonly NetEvent1Field<int, NetInt> restoreAreaCutsceneEvent = new NetEvent1Field<int, NetInt>();
  [XmlIgnore]
  private readonly NetEvent1Field<int, NetInt> areaCompleteRewardEvent = new NetEvent1Field<int, NetInt>();
  private float messageAlpha;
  private List<int> junimoNotesViewportTargets;
  private Dictionary<int, List<int>> areaToBundleDictionary;
  private Dictionary<int, int> bundleToAreaDictionary;
  private Dictionary<string, List<List<int>>> bundlesIngredientsInfo;
  private bool _isWatchingJunimoGoodbye;
  private Vector2 missedRewardsChestTile = new Vector2(22f, 10f);
  private const string missedRewardsTileSheetId = "indoors2";
  [XmlIgnore]
  public readonly NetRef<Chest> missedRewardsChest = new NetRef<Chest>(new Chest(true));
  [XmlIgnore]
  public readonly NetBool missedRewardsChestVisible = new NetBool(false);
  [XmlIgnore]
  public readonly NetEvent1Field<bool, NetBool> showMissedRewardsChestEvent = new NetEvent1Field<bool, NetBool>();
  public const int PHASE_firstPause = 0;
  public const int PHASE_junimoAppear = 1;
  public const int PHASE_junimoDance = 2;
  public const int PHASE_restore = 3;
  private int restoreAreaTimer;
  private int restoreAreaPhase;
  private int restoreAreaIndex;
  private ICue buildUpSound;

  [XmlElement("bundles")]
  public NetBundles bundles => Game1.netWorldState.Value.Bundles;

  [XmlElement("bundleRewards")]
  public NetIntDictionary<bool, NetBool> bundleRewards => Game1.netWorldState.Value.BundleRewards;

  public CommunityCenter()
  {
    this.initAreaBundleConversions();
    this.refreshBundlesIngredientsInfo();
  }

  public CommunityCenter(string map_path, string name)
    : base(map_path, name)
  {
    this.initAreaBundleConversions();
    this.refreshBundlesIngredientsInfo();
  }

  public CommunityCenter(string name)
    : base("Maps\\CommunityCenter_Ruins", name)
  {
    this.initAreaBundleConversions();
    this.refreshBundlesIngredientsInfo();
  }

  public void refreshBundlesIngredientsInfo()
  {
    this.bundlesIngredientsInfo = new Dictionary<string, List<List<int>>>();
    Dictionary<string, string> bundleData = Game1.netWorldState.Value.BundleData;
    Dictionary<int, bool[]> dictionary = this.bundlesDict();
    foreach (KeyValuePair<string, string> keyValuePair in bundleData)
    {
      string[] strArray1 = keyValuePair.Key.Split('/');
      int int32_1 = Convert.ToInt32(strArray1[1]);
      string name = strArray1[0];
      string[] strArray2 = ArgUtility.SplitBySpace(keyValuePair.Value.Split('/')[2]);
      if (this.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(name)))
      {
        for (int index = 0; index < strArray2.Length; index += 3)
        {
          if (dictionary.ContainsKey(int32_1) && !dictionary[int32_1][index / 3])
          {
            int result;
            string key;
            if (int.TryParse(strArray2[index], out result) && result < 0)
            {
              key = result.ToString();
            }
            else
            {
              ParsedItemData data = ItemRegistry.GetData(strArray2[index]);
              key = data != null ? data.QualifiedItemId : "(O)" + strArray2[index];
            }
            int int32_2 = Convert.ToInt32(strArray2[index + 1]);
            int int32_3 = Convert.ToInt32(strArray2[index + 2]);
            List<List<int>> intListList;
            if (!this.bundlesIngredientsInfo.TryGetValue(key, out intListList))
              this.bundlesIngredientsInfo[key] = intListList = new List<List<int>>();
            intListList.Add(new List<int>()
            {
              int32_1,
              int32_2,
              int32_3
            });
          }
        }
      }
    }
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.warehouse, "warehouse").AddField((INetSerializable) this.areasComplete, "areasComplete").AddField((INetSerializable) this.numberOfStarsOnPlaque, "numberOfStarsOnPlaque").AddField((INetSerializable) this.newJunimoNoteCheckEvent, "newJunimoNoteCheckEvent").AddField((INetSerializable) this.restoreAreaCutsceneEvent, "restoreAreaCutsceneEvent").AddField((INetSerializable) this.areaCompleteRewardEvent, "areaCompleteRewardEvent").AddField((INetSerializable) this.missedRewardsChest, "missedRewardsChest").AddField((INetSerializable) this.showMissedRewardsChestEvent, "showMissedRewardsChestEvent").AddField((INetSerializable) this.missedRewardsChestVisible, "missedRewardsChestVisible");
    this.newJunimoNoteCheckEvent.onEvent += new NetEvent0.Event(this.doCheckForNewJunimoNotes);
    this.restoreAreaCutsceneEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doRestoreAreaCutscene);
    this.areaCompleteRewardEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doAreaCompleteReward);
    this.showMissedRewardsChestEvent.onEvent += new AbstractNetEvent1<bool>.Event(this.doShowMissedRewardsChest);
  }

  private void initAreaBundleConversions()
  {
    this.areaToBundleDictionary = new Dictionary<int, List<int>>();
    this.bundleToAreaDictionary = new Dictionary<int, int>();
    for (int key = 0; key < 7; ++key)
    {
      this.areaToBundleDictionary.Add(key, new List<int>());
      NetMutex netMutex = new NetMutex();
      this.bundleMutexes.Add(netMutex);
      this.NetFields.AddField((INetSerializable) netMutex.NetFields, "m.NetFields");
    }
    foreach (KeyValuePair<string, string> keyValuePair in Game1.netWorldState.Value.BundleData)
    {
      int int32 = Convert.ToInt32(keyValuePair.Key.Split('/')[1]);
      this.areaToBundleDictionary[CommunityCenter.getAreaNumberFromName(keyValuePair.Key.Split('/')[0])].Add(int32);
      this.bundleToAreaDictionary.Add(int32, CommunityCenter.getAreaNumberFromName(keyValuePair.Key.Split('/')[0]));
    }
  }

  public static int getAreaNumberFromName(string name)
  {
    if (name != null)
    {
      switch (name.Length)
      {
        case 5:
          if (name == "Vault")
            return 4;
          goto label_24;
        case 6:
          if (name == "Pantry")
            return 0;
          goto label_24;
        case 8:
          switch (name[0])
          {
            case 'B':
              if (name == "Bulletin")
                goto label_22;
              goto label_24;
            case 'F':
              if (name == "FishTank")
                goto label_19;
              goto label_24;
            default:
              goto label_24;
          }
        case 9:
          if (name == "Fish Tank")
            goto label_19;
          goto label_24;
        case 10:
          switch (name[0])
          {
            case 'B':
              if (name == "BoilerRoom")
                goto label_20;
              goto label_24;
            case 'C':
              if (name == "CraftsRoom")
                break;
              goto label_24;
            default:
              goto label_24;
          }
          break;
        case 11:
          switch (name[0])
          {
            case 'B':
              if (name == "Boiler Room")
                goto label_20;
              goto label_24;
            case 'C':
              if (name == "Crafts Room")
                break;
              goto label_24;
            default:
              goto label_24;
          }
          break;
        case 13:
          if (name == "BulletinBoard")
            goto label_22;
          goto label_24;
        case 14:
          if (name == "Bulletin Board")
            goto label_22;
          goto label_24;
        case 19:
          if (name == "Abandoned Joja Mart")
            return 6;
          goto label_24;
        default:
          goto label_24;
      }
      return 1;
label_19:
      return 2;
label_20:
      return 3;
label_22:
      return 5;
    }
label_24:
    return -1;
  }

  private Point getNotePosition(int area)
  {
    switch (area)
    {
      case 0:
        return new Point(14, 5);
      case 1:
        return new Point(14, 23);
      case 2:
        return new Point(40, 10);
      case 3:
        return new Point(63 /*0x3F*/, 14);
      case 4:
        return new Point(55, 6);
      case 5:
        return new Point(46, 11);
      default:
        return Point.Zero;
    }
  }

  public void addJunimoNote(int area)
  {
    Point notePosition = this.getNotePosition(area);
    if (notePosition.Equals((object) Vector2.Zero))
      return;
    StaticTile[] junimoNoteTileFrames = CommunityCenter.getJunimoNoteTileFrames(area, this.map);
    string layerId = area == 5 ? "Front" : "Buildings";
    this.map.RequireLayer(layerId).Tiles[notePosition.X, notePosition.Y] = (Tile) new AnimatedTile(this.map.RequireLayer(layerId), junimoNoteTileFrames, 70L);
    Game1.currentLightSources.Add(new LightSource($"{nameof (CommunityCenter)}_Area{area}", 4, new Vector2((float) (notePosition.X * 64 /*0x40*/), (float) (notePosition.Y * 64 /*0x40*/)), 1f, onlyLocation: this.NameOrUniqueName));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (notePosition.X * 64 /*0x40*/), (float) (notePosition.Y * 64 /*0x40*/)), Color.White)
    {
      layerDepth = 1f,
      interval = 50f,
      motion = new Vector2(1f, 0.0f),
      acceleration = new Vector2(-0.005f, 0.0f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (notePosition.X * 64 /*0x40*/ - 12), (float) (notePosition.Y * 64 /*0x40*/ - 12)), Color.White)
    {
      scale = 0.75f,
      layerDepth = 1f,
      interval = 50f,
      motion = new Vector2(1f, 0.0f),
      acceleration = new Vector2(-0.005f, 0.0f),
      delayBeforeAnimationStart = 50
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (notePosition.X * 64 /*0x40*/ - 12), (float) (notePosition.Y * 64 /*0x40*/ + 12)), Color.White)
    {
      layerDepth = 1f,
      interval = 50f,
      motion = new Vector2(1f, 0.0f),
      acceleration = new Vector2(-0.005f, 0.0f),
      delayBeforeAnimationStart = 100
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (notePosition.X * 64 /*0x40*/), (float) (notePosition.Y * 64 /*0x40*/)), Color.White)
    {
      layerDepth = 1f,
      scale = 0.75f,
      interval = 50f,
      motion = new Vector2(1f, 0.0f),
      acceleration = new Vector2(-0.005f, 0.0f),
      delayBeforeAnimationStart = 150
    });
  }

  public int numberOfCompleteBundles()
  {
    int num = 0;
    foreach (KeyValuePair<int, bool[]> pair in this.bundles.Pairs)
    {
      ++num;
      for (int index = 0; index < pair.Value.Length; ++index)
      {
        if (!pair.Value[index])
        {
          --num;
          break;
        }
      }
    }
    return num;
  }

  public void addStarToPlaque() => ++this.numberOfStarsOnPlaque.Value;

  private string getMessageForAreaCompletion()
  {
    int numberOfAreasComplete = this.getNumberOfAreasComplete();
    switch (numberOfAreasComplete)
    {
      case 1:
      case 2:
      case 3:
      case 4:
      case 5:
      case 6:
        return Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaCompletion" + numberOfAreasComplete.ToString(), (object) Game1.player.Name);
      default:
        return "";
    }
  }

  private int getNumberOfAreasComplete()
  {
    int numberOfAreasComplete = 0;
    for (int index = 0; index < this.areasComplete.Count; ++index)
    {
      if (this.areasComplete[index])
        ++numberOfAreasComplete;
    }
    return numberOfAreasComplete;
  }

  public Dictionary<int, bool[]> bundlesDict()
  {
    return this.bundles.Pairs.Select<KeyValuePair<int, bool[]>, KeyValuePair<int, bool[]>>((Func<KeyValuePair<int, bool[]>, KeyValuePair<int, bool[]>>) (kvp => new KeyValuePair<int, bool[]>(kvp.Key, ((IEnumerable<bool>) kvp.Value).ToArray<bool>()))).ToDictionary<KeyValuePair<int, bool[]>, int, bool[]>((Func<KeyValuePair<int, bool[]>, int>) (x => x.Key), (Func<KeyValuePair<int, bool[]>, bool[]>) (y => y.Value));
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (!who.IsLocalPlayer || !(ArgUtility.Get(action, 0) == "MissedRewards"))
      return base.performAction(action, who, tileLocation);
    this.missedRewardsChest.Value.mutex.RequestLock((Action) (() =>
    {
      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) this.missedRewardsChest.Value.Items, false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.rewardGrabbed), canBeExitedWithKey: true, context: (object) this);
      Game1.activeClickableMenu.exitFunction = (IClickableMenu.onExit) (() =>
      {
        this.missedRewardsChest.Value.mutex.ReleaseLock();
        this.checkForMissedRewards();
      });
    }));
    return true;
  }

  private void rewardGrabbed(Item item, Farmer who)
  {
    this.bundleRewards[item.SpecialVariable] = false;
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    switch (this.getTileIndexAt(tileLocation, "Buildings", "indoors"))
    {
      case 1799:
        if (this.numberOfCompleteBundles() > 2)
          this.checkBundle(5);
        return true;
      case 1824:
      case 1825:
      case 1826:
      case 1827:
      case 1828:
      case 1829:
      case 1830:
      case 1831:
      case 1832:
      case 1833:
        this.checkBundle(this.getAreaNumberFromLocation(who.Tile));
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  public void checkBundle(int area)
  {
    this.bundleMutexes[area].RequestLock((Action) (() => Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(area, this.bundlesDict())));
  }

  public void addJunimoNoteViewportTarget(int area)
  {
    if (this.junimoNotesViewportTargets == null)
      this.junimoNotesViewportTargets = new List<int>();
    if (this.junimoNotesViewportTargets.Contains(area))
      return;
    this.junimoNotesViewportTargets.Add(area);
  }

  public void checkForNewJunimoNotes() => this.newJunimoNoteCheckEvent.Fire();

  private void doCheckForNewJunimoNotes()
  {
    if (Game1.currentLocation != this)
      return;
    for (int area = 0; area < this.areasComplete.Count; ++area)
    {
      if (!this.isJunimoNoteAtArea(area) && this.shouldNoteAppearInArea(area))
        this.addJunimoNoteViewportTarget(area);
    }
  }

  public bool isJunimoNoteAtArea(int area)
  {
    Point notePosition = this.getNotePosition(area);
    return area == 5 ? this.map.RequireLayer("Front").Tiles[notePosition.X, notePosition.Y] != null : this.map.RequireLayer("Buildings").Tiles[notePosition.X, notePosition.Y] != null;
  }

  public bool shouldNoteAppearInArea(int area)
  {
    bool flag = true;
    for (int index1 = 0; index1 < this.areaToBundleDictionary[area].Count; ++index1)
    {
      foreach (int key in this.areaToBundleDictionary[area])
      {
        bool[] flagArray;
        if (this.bundles.TryGetValue(key, out flagArray))
        {
          int num = flagArray.Length / 3;
          for (int index2 = 0; index2 < num; ++index2)
          {
            if (!flagArray[index2])
            {
              flag = false;
              break;
            }
          }
        }
        if (!flag)
          break;
      }
    }
    if (area >= 0 && !flag)
    {
      switch (area)
      {
        case 0:
        case 2:
          if (this.numberOfCompleteBundles() > 0)
            return true;
          break;
        case 1:
          return true;
        case 3:
          if (this.numberOfCompleteBundles() > 1)
            return true;
          break;
        case 4:
          if (this.numberOfCompleteBundles() > 3)
            return true;
          break;
        case 5:
          if (this.numberOfCompleteBundles() > 2)
            return true;
          break;
        case 6:
          if (Utility.HasAnyPlayerSeenEvent("191393"))
            return true;
          break;
      }
    }
    return false;
  }

  public override void updateMap()
  {
    if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
    {
      this.warehouse.Value = true;
      this.mapPath.Value = "Maps\\CommunityCenter_Joja";
    }
    base.updateMap();
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (this.areAllAreasComplete())
    {
      this.mapPath.Value = "Maps\\CommunityCenter_Refurbished";
      this.updateMap();
    }
    base.TransferDataFromSavedLocation(l);
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.areAllAreasComplete())
    {
      this.mapPath.Value = "Maps\\CommunityCenter_Refurbished";
      this.addFishTank();
    }
    this._isWatchingJunimoGoodbye = false;
    if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !this.areAllAreasComplete())
    {
      for (int index = 0; index < this.areasComplete.Count; ++index)
      {
        if (this.shouldNoteAppearInArea(index))
          this.characters.Add((NPC) new Junimo(new Vector2((float) this.getNotePosition(index).X, (float) (this.getNotePosition(index).Y + 2)) * 64f, index));
      }
    }
    this.numberOfStarsOnPlaque.Value = 0;
    for (int index = 0; index < this.areasComplete.Count; ++index)
    {
      if (this.areasComplete[index])
        ++this.numberOfStarsOnPlaque.Value;
    }
    this.checkForMissedRewards();
  }

  private void doShowMissedRewardsChest(bool isVisible)
  {
    int x = (int) this.missedRewardsChestTile.X;
    int y = (int) this.missedRewardsChestTile.Y;
    this.removeMapTile(x, y, "Buildings");
    if (!isVisible)
      return;
    this.setMapTile(x, y, 5, "Buildings", "indoors2", "MissedRewards");
  }

  private void checkForMissedRewards()
  {
    HashSet<int> intSet = new HashSet<int>();
    bool flag = false;
    this.missedRewardsChest.Value.Items.Clear();
    List<Item> rewards = new List<Item>();
    foreach (int key in this.bundleRewards.Keys)
    {
      int bundleToArea = this.bundleToAreaDictionary[key];
      if (this.bundleRewards[key] && this.areasComplete.Count > bundleToArea && this.areasComplete[bundleToArea] && !intSet.Contains(bundleToArea))
      {
        intSet.Add(bundleToArea);
        flag = true;
        rewards.Clear();
        JunimoNoteMenu.GetBundleRewards(bundleToArea, rewards);
        foreach (Item obj in rewards)
          this.missedRewardsChest.Value.addItem(obj);
      }
    }
    if (flag == this.missedRewardsChestVisible.Value)
      return;
    this.showMissedRewardsChestEvent.Fire(flag);
    Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(Game1.random.Choose<int>(5, 46), this.missedRewardsChestTile * 64f + new Vector2(16f, 16f), Color.White)
    {
      layerDepth = 1f
    });
    this.missedRewardsChestVisible.Value = flag;
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (!Game1.MasterPlayer.mailReceived.Contains("JojaMember") && !this.areAllAreasComplete())
    {
      for (int index = 0; index < this.areasComplete.Count; ++index)
      {
        if (this.areasComplete[index])
          this.loadArea(index, false);
        else if (this.shouldNoteAppearInArea(index))
          this.addJunimoNote(index);
      }
    }
    this.doShowMissedRewardsChest(this.missedRewardsChestVisible.Value);
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.eventUp || this.areAllAreasComplete())
      return;
    Game1.changeMusicTrack("communityCenter");
  }

  private int getAreaNumberFromLocation(Vector2 tileLocation)
  {
    for (int area = 0; area < this.areasComplete.Count; ++area)
    {
      if (this.getAreaBounds(area).Contains((int) tileLocation.X, (int) tileLocation.Y))
        return area;
    }
    return -1;
  }

  private Microsoft.Xna.Framework.Rectangle getAreaBounds(int area)
  {
    switch (area)
    {
      case 0:
        return new Microsoft.Xna.Framework.Rectangle(0, 0, 22, 11);
      case 1:
        return new Microsoft.Xna.Framework.Rectangle(0, 12, 21, 17);
      case 2:
        return new Microsoft.Xna.Framework.Rectangle(35, 4, 9, 9);
      case 3:
        return new Microsoft.Xna.Framework.Rectangle(52, 9, 16 /*0x10*/, 12);
      case 4:
        return new Microsoft.Xna.Framework.Rectangle(45, 0, 15, 9);
      case 5:
        return new Microsoft.Xna.Framework.Rectangle(22, 13, 28, 9);
      case 7:
        return new Microsoft.Xna.Framework.Rectangle(44, 10, 6, 3);
      case 8:
        return new Microsoft.Xna.Framework.Rectangle(22, 4, 13, 9);
      default:
        return Microsoft.Xna.Framework.Rectangle.Empty;
    }
  }

  protected void removeJunimo()
  {
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Junimo));
  }

  public override void cleanupBeforeSave() => this.removeJunimo();

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    if (this.farmers.Count > 1)
      return;
    this.removeJunimo();
  }

  public bool isBundleComplete(int bundleIndex)
  {
    for (int index = 0; index < this.bundles[bundleIndex].Length; ++index)
    {
      if (!this.bundles[bundleIndex][index])
        return false;
    }
    return true;
  }

  public bool couldThisIngredienteBeUsedInABundle(StardewValley.Object o)
  {
    if (!o.bigCraftable.Value)
    {
      List<List<int>> intListList1;
      if (this.bundlesIngredientsInfo.TryGetValue(o.QualifiedItemId, out intListList1))
      {
        foreach (List<int> intList in intListList1)
        {
          if (o.Quality >= intList[2])
            return true;
        }
      }
      List<List<int>> intListList2;
      if (o.Category < 0 && this.bundlesIngredientsInfo.TryGetValue(o.Category.ToString(), out intListList2))
      {
        foreach (List<int> intList in intListList2)
        {
          if (o.Quality >= intList[2])
            return true;
        }
      }
    }
    return false;
  }

  public void areaCompleteReward(int whichArea) => this.areaCompleteRewardEvent.Fire(whichArea);

  private void doAreaCompleteReward(int whichArea)
  {
    string str = "";
    switch (whichArea)
    {
      case 0:
        str = "ccPantry";
        break;
      case 1:
        str = "ccCraftsRoom";
        break;
      case 2:
        str = "ccFishTank";
        break;
      case 3:
        str = "ccBoilerRoom";
        break;
      case 4:
        str = "ccVault";
        break;
      case 5:
        str = "ccBulletin";
        Game1.addMailForTomorrow("ccBulletinThankYou");
        break;
    }
    if (str.Length <= 0 || Game1.player.mailReceived.Contains(str))
      return;
    Game1.player.mailForTomorrow.Add(str + "%&NL&%");
  }

  public void loadArea(int area, bool showEffects = true)
  {
    Microsoft.Xna.Framework.Rectangle areaBounds = this.getAreaBounds(area);
    Map map = Game1.game1.xTileContent.Load<Map>("Maps\\CommunityCenter_Refurbished");
    this.ApplyMapOverride(map, $"CommunityCenter_Refurbished{area}", new Microsoft.Xna.Framework.Rectangle?(areaBounds), new Microsoft.Xna.Framework.Rectangle?(areaBounds));
    Layer layer1 = map.RequireLayer("Buildings");
    Layer layer2 = map.RequireLayer("Front");
    Layer layer3 = map.RequireLayer("Paths");
    foreach (Point point in areaBounds.GetPoints())
    {
      int x = point.X;
      int y = point.Y;
      Tile tile1 = layer1.Tiles[x, y];
      if (tile1 != null)
      {
        this.adjustMapLightPropertiesForLamp(tile1.TileIndex, x, y, "Buildings");
        if (Game1.player.currentLocation == this && Game1.player.TilePoint.X == x && Game1.player.TilePoint.Y == y)
          Game1.player.Position = new Vector2(2080f, 576f);
      }
      Tile tile2 = layer2.Tiles[x, y];
      if (tile2 != null)
        this.adjustMapLightPropertiesForLamp(tile2.TileIndex, x, y, "Front");
      Tile tile3 = layer3.Tiles[x, y];
      if (tile3 != null && tile3.TileIndex == 8)
        Game1.currentLightSources.Add(new LightSource($"{nameof (CommunityCenter)}_Area{area}_{point.X}_{point.Y}", 4, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)), 2f, onlyLocation: this.NameOrUniqueName));
      if (showEffects && Game1.random.NextDouble() < 0.58 && layer1.Tiles[x, y] == null)
        this.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)), Color.White)
        {
          layerDepth = 1f,
          interval = 50f,
          motion = new Vector2((float) Game1.random.Next(17) / 10f, 0.0f),
          acceleration = new Vector2(-0.005f, 0.0f),
          delayBeforeAnimationStart = Game1.random.Next(500)
        });
    }
    if ((area == 5 || area == 8) && this.missedRewardsChestVisible.Value)
      this.doShowMissedRewardsChest(true);
    switch (area)
    {
      case 2:
        this.addFishTank();
        break;
      case 5:
        this.loadArea(7);
        break;
    }
    this.addLightGlows();
  }

  public void addFishTank()
  {
    bool flag = false;
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture.QualifiedItemId == "(F)CCFishTank")
      {
        furniture.AllowLocalRemoval = false;
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    FishTankFurniture fishTankFurniture = new FishTankFurniture("CCFishTank", new Vector2(38f, 9f));
    fishTankFurniture.CanBeGrabbed = false;
    fishTankFurniture.AllowLocalRemoval = false;
    fishTankFurniture.Fragility = 2;
    fishTankFurniture.heldItems.Add(ItemRegistry.Create("(O)143"));
    fishTankFurniture.heldItems.Add(ItemRegistry.Create("(O)145"));
    fishTankFurniture.heldItems.Add(ItemRegistry.Create("(O)721"));
    this.furniture.Add((Furniture) fishTankFurniture);
  }

  public void restoreAreaCutscene(int whichArea) => this.restoreAreaCutsceneEvent.Fire(whichArea);

  public void markAreaAsComplete(int area)
  {
    if (Game1.currentLocation == this)
      this.areasComplete[area] = true;
    if (!this.areAllAreasComplete() || Game1.currentLocation != this)
      return;
    this._isWatchingJunimoGoodbye = true;
  }

  private void doRestoreAreaCutscene(int whichArea)
  {
    this.markAreaAsComplete(whichArea);
    this.restoreAreaIndex = whichArea;
    this.restoreAreaPhase = 0;
    this.restoreAreaTimer = 1000;
    if (Game1.player.currentLocation == this)
    {
      Game1.freezeControls = true;
      Game1.changeMusicTrack("none");
    }
    this.checkForMissedRewards();
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    this.restoreAreaCutsceneEvent.Poll();
    this.newJunimoNoteCheckEvent.Poll();
    this.areaCompleteRewardEvent.Poll();
    this.showMissedRewardsChestEvent.Poll();
    foreach (NetMutex bundleMutex in this.bundleMutexes)
    {
      bundleMutex.Update((GameLocation) this);
      if (bundleMutex.IsLockHeld() && Game1.activeClickableMenu == null)
        bundleMutex.ReleaseLock();
    }
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.missedRewardsChest.Value.updateWhenCurrentLocation(time);
    if (this.restoreAreaTimer > 0)
    {
      int restoreAreaTimer = this.restoreAreaTimer;
      this.restoreAreaTimer -= time.ElapsedGameTime.Milliseconds;
      switch (this.restoreAreaPhase)
      {
        case 0:
          if (this.restoreAreaTimer > 0)
            break;
          this.restoreAreaTimer = 3000;
          this.restoreAreaPhase = 1;
          if (Game1.player.currentLocation != this)
            break;
          Game1.player.faceDirection(2);
          Game1.player.jump();
          Game1.player.jitterStrength = 1f;
          Game1.player.showFrame(94);
          break;
        case 1:
          if (Game1.IsMasterGame && Game1.random.NextDouble() < 0.4)
          {
            Vector2 positionInThisRectangle = Utility.getRandomPositionInThisRectangle(this.getAreaBounds(this.restoreAreaIndex), Game1.random);
            Junimo junimo = new Junimo(positionInThisRectangle * 64f, this.restoreAreaIndex, true);
            if (!this.isCollidingPosition(junimo.GetBoundingBox(), Game1.viewport, (Character) junimo))
            {
              this.characters.Add((NPC) junimo);
              Game1.multiplayer.broadcastSprites((GameLocation) this, new TemporaryAnimatedSprite(Game1.random.Choose<int>(5, 46), positionInThisRectangle * 64f + new Vector2(16f, 16f), Color.White)
              {
                layerDepth = 1f
              });
              this.localSound("tinyWhip");
            }
          }
          if (this.restoreAreaTimer <= 0)
          {
            this.restoreAreaTimer = 999999;
            this.restoreAreaPhase = 2;
            if (Game1.player.currentLocation != this)
              break;
            Game1.screenGlowOnce(Color.White, true, maxAlpha: 1f);
            Game1.playSound("wind", out this.buildUpSound);
            this.buildUpSound.SetVariable("Volume", 0.0f);
            this.buildUpSound.SetVariable("Frequency", 0.0f);
            Game1.player.jitterStrength = 2f;
            Game1.player.stopShowingFrame();
          }
          Game1.drawLighting = false;
          break;
        case 2:
          if (this.buildUpSound != null)
          {
            this.buildUpSound.SetVariable("Volume", Game1.screenGlowAlpha * 150f);
            this.buildUpSound.SetVariable("Frequency", Game1.screenGlowAlpha * 150f);
          }
          if ((double) Game1.screenGlowAlpha >= (double) Game1.screenGlowMax)
          {
            this.messageAlpha += 0.008f;
            this.messageAlpha = Math.Min(this.messageAlpha, 1f);
          }
          if (((double) Game1.screenGlowAlpha == (double) Game1.screenGlowMax || Game1.currentLocation != this) && this.restoreAreaTimer > 5200)
            this.restoreAreaTimer = 5200;
          if (this.restoreAreaTimer < 5200 && Game1.random.NextDouble() < (double) (5200 - this.restoreAreaTimer) / 10000.0)
            this.localSound(Game1.random.Choose<string>("dustMeep", "junimoMeep1"));
          if (this.restoreAreaTimer > 0)
            break;
          this.restoreAreaTimer = 2000;
          this.messageAlpha = 0.0f;
          this.restoreAreaPhase = 3;
          if (Game1.IsMasterGame)
            this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Junimo junimo1 && junimo1.temporaryJunimo.Value));
          if (Game1.player.currentLocation != this)
          {
            if (!Game1.IsMasterGame)
              break;
            this.loadArea(this.restoreAreaIndex);
            this._mapSeatsDirty = true;
            break;
          }
          Game1.screenGlowHold = false;
          this.loadArea(this.restoreAreaIndex);
          if (Game1.IsMasterGame)
            this._mapSeatsDirty = true;
          this.buildUpSound?.Stop(AudioStopOptions.Immediate);
          this.localSound("wand");
          Game1.changeMusicTrack("junimoStarSong");
          this.localSound("woodyHit");
          Game1.flashAlpha = 1f;
          Game1.player.stopJittering();
          Game1.drawLighting = true;
          break;
        case 3:
          if (restoreAreaTimer > 1000 && this.restoreAreaTimer <= 1000)
          {
            Junimo junimoForArea = this.getJunimoForArea(this.restoreAreaIndex);
            if (junimoForArea != null && Game1.IsMasterGame)
            {
              if (!junimoForArea.holdingBundle.Value)
              {
                junimoForArea.Position = Utility.getRandomAdjacentOpenTile(Utility.PointToVector2(this.getNotePosition(this.restoreAreaIndex)), (GameLocation) this) * 64f;
                int num;
                for (num = 0; this.isCollidingPosition(junimoForArea.GetBoundingBox(), Game1.viewport, (Character) junimoForArea) && num < 20; ++num)
                {
                  Microsoft.Xna.Framework.Rectangle r = this.getAreaBounds(this.restoreAreaIndex);
                  if (this.restoreAreaIndex == 5)
                    r = new Microsoft.Xna.Framework.Rectangle(44, 13, 6, 2);
                  junimoForArea.Position = Utility.getRandomPositionInThisRectangle(r, Game1.random) * 64f;
                }
                if (num < 20)
                  junimoForArea.fadeBack();
              }
              junimoForArea.returnToJunimoHutToFetchStar((GameLocation) this);
            }
          }
          if (this.restoreAreaTimer > 0 || this._isWatchingJunimoGoodbye)
            break;
          Game1.freezeControls = false;
          break;
      }
    }
    else
    {
      if (Game1.activeClickableMenu != null)
        return;
      List<int> notesViewportTargets = this.junimoNotesViewportTargets;
      // ISSUE: explicit non-virtual call
      if ((notesViewportTargets != null ? (__nonvirtual (notesViewportTargets.Count) > 0 ? 1 : 0) : 0) == 0 || Game1.isViewportOnCustomPath())
        return;
      this.setViewportToNextJunimoNoteTarget();
    }
  }

  private void setViewportToNextJunimoNoteTarget()
  {
    if (this.junimoNotesViewportTargets.Count > 0)
    {
      Game1.freezeControls = true;
      Point notePosition = this.getNotePosition(this.junimoNotesViewportTargets[0]);
      Game1.moveViewportTo(new Vector2((float) notePosition.X, (float) notePosition.Y) * 64f, 5f, 2000, new Game1.afterFadeFunction(this.afterViewportGetsToJunimoNotePosition), new Game1.afterFadeFunction(this.setViewportToNextJunimoNoteTarget));
    }
    else
    {
      Game1.viewportFreeze = true;
      Game1.viewportHold = 10000;
      Game1.globalFadeToBlack(new Game1.afterFadeFunction(Game1.afterFadeReturnViewportToPlayer));
      Game1.freezeControls = false;
      Game1.afterViewport = (Game1.afterFadeFunction) null;
    }
  }

  private void afterViewportGetsToJunimoNotePosition()
  {
    int notesViewportTarget = this.junimoNotesViewportTargets[0];
    this.junimoNotesViewportTargets.RemoveAt(0);
    this.addJunimoNote(notesViewportTarget);
    this.localSound("reward");
  }

  public Junimo getJunimoForArea(int whichArea)
  {
    foreach (NPC character in this.characters)
    {
      if (character is Junimo junimoForArea && junimoForArea.whichArea.Value == whichArea)
        return junimoForArea;
    }
    Junimo character1 = new Junimo(Vector2.Zero, whichArea);
    this.addCharacter((NPC) character1);
    return character1;
  }

  public bool areAllAreasComplete()
  {
    foreach (bool flag in this.areasComplete)
    {
      if (!flag)
        return false;
    }
    return true;
  }

  public void junimoGoodbyeDance()
  {
    this.getJunimoForArea(0).Position = new Vector2(23f, 11f) * 64f;
    this.getJunimoForArea(1).Position = new Vector2(27f, 11f) * 64f;
    this.getJunimoForArea(2).Position = new Vector2(24f, 12f) * 64f;
    this.getJunimoForArea(4).Position = new Vector2(26f, 12f) * 64f;
    this.getJunimoForArea(3).Position = new Vector2(28f, 12f) * 64f;
    this.getJunimoForArea(5).Position = new Vector2(25f, 11f) * 64f;
    for (int whichArea = 0; whichArea < this.areasComplete.Count; ++whichArea)
    {
      this.getJunimoForArea(whichArea).stayStill();
      this.getJunimoForArea(whichArea).faceDirection(1);
      this.getJunimoForArea(whichArea).fadeBack();
      this.getJunimoForArea(whichArea).IsInvisible = false;
      this.getJunimoForArea(whichArea).setAlpha(1f);
    }
    Point standingPixel = Game1.player.StandingPixel;
    Game1.moveViewportTo(new Vector2((float) standingPixel.X, (float) standingPixel.Y), 2f, 5000, new Game1.afterFadeFunction(this.startGoodbyeDance), new Game1.afterFadeFunction(this.endGoodbyeDance));
    Game1.viewportFreeze = false;
    Game1.freezeControls = true;
  }

  public void prepareForJunimoDance()
  {
    for (int whichArea = 0; whichArea < this.areasComplete.Count; ++whichArea)
    {
      Junimo junimoForArea = this.getJunimoForArea(whichArea);
      junimoForArea.holdingBundle.Value = false;
      junimoForArea.holdingStar.Value = false;
      junimoForArea.controller = (PathFindController) null;
      junimoForArea.Halt();
      junimoForArea.IsInvisible = true;
    }
    this.numberOfStarsOnPlaque.Value = 0;
    for (int index = 0; index < this.areasComplete.Count; ++index)
    {
      if (this.areasComplete[index])
        ++this.numberOfStarsOnPlaque.Value;
    }
  }

  private void startGoodbyeDance()
  {
    Game1.freezeControls = true;
    this.getJunimoForArea(0).Position = new Vector2(23f, 11f) * 64f;
    this.getJunimoForArea(1).Position = new Vector2(27f, 11f) * 64f;
    this.getJunimoForArea(2).Position = new Vector2(24f, 12f) * 64f;
    this.getJunimoForArea(4).Position = new Vector2(26f, 12f) * 64f;
    this.getJunimoForArea(3).Position = new Vector2(28f, 12f) * 64f;
    this.getJunimoForArea(5).Position = new Vector2(25f, 11f) * 64f;
    for (int whichArea = 0; whichArea < this.areasComplete.Count; ++whichArea)
    {
      this.getJunimoForArea(whichArea).stayStill();
      this.getJunimoForArea(whichArea).faceDirection(1);
      this.getJunimoForArea(whichArea).fadeBack();
      this.getJunimoForArea(whichArea).IsInvisible = false;
      this.getJunimoForArea(whichArea).setAlpha(1f);
      this.getJunimoForArea(whichArea).sayGoodbye();
    }
  }

  private void endGoodbyeDance()
  {
    for (int whichArea = 0; whichArea < this.areasComplete.Count; ++whichArea)
      this.getJunimoForArea(whichArea).fadeAway();
    Game1.pauseThenDoFunction(3600, new Game1.afterFadeFunction(this.loadJunimoHut));
    Game1.freezeControls = true;
  }

  private void loadJunimoHut()
  {
    for (int whichArea = 0; whichArea < this.areasComplete.Count; ++whichArea)
      this.getJunimoForArea(whichArea).clearTextAboveHead();
    this.loadArea(8);
    Game1.flashAlpha = 1f;
    this.localSound("wand");
    Game1.freezeControls = false;
    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:CommunityCenter_JunimosReturned"));
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    for (int index = 0; index < this.numberOfStarsOnPlaque.Value; ++index)
    {
      switch (index)
      {
        case 0:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2136f, 324f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
        case 1:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2136f, 364f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
        case 2:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2096f, 384f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
        case 3:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2056f, 364f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
        case 4:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2056f, 324f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
        case 5:
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(2096f, 308f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(354, 401, 7, 7)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
          break;
      }
    }
    if (!Game1.eventUp)
      return;
    Furniture.isDrawingLocationFurniture = true;
    foreach (Furniture furniture in this.furniture)
    {
      if (furniture.QualifiedItemId == "(F)CCFishTank")
        furniture.draw(b, -1, -1, 1f);
    }
    Furniture.isDrawingLocationFurniture = false;
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    if ((double) this.messageAlpha <= 0.0)
      return;
    Junimo junimoForArea = this.getJunimoForArea(0);
    if (junimoForArea != null)
      b.Draw(junimoForArea.Sprite.Texture, new Vector2((float) (Game1.viewport.Width / 2 - 32 /*0x20*/), (float) ((double) (Game1.viewport.Height * 2) / 3.0 - 64.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800.0) / 100 * 16 /*0x10*/, 0, 16 /*0x10*/, 16 /*0x10*/)), Color.Lime * this.messageAlpha, 0.0f, new Vector2((float) (junimoForArea.Sprite.SpriteWidth * 4 / 2), (float) ((double) (junimoForArea.Sprite.SpriteHeight * 4) * 3.0 / 4.0)) / 4f, Math.Max(0.2f, 1f) * 4f, junimoForArea.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
    b.DrawString(Game1.dialogueFont, "\"" + Game1.parseText(this.getMessageForAreaCompletion() + "\"", Game1.dialogueFont, 640), new Vector2((float) (Game1.viewport.Width / 2 - 320), (float) (Game1.viewport.Height * 2) / 3f), Game1.textColor * this.messageAlpha * 0.6f);
  }

  public static string getAreaNameFromNumber(int areaNumber)
  {
    switch (areaNumber)
    {
      case 0:
        return "Pantry";
      case 1:
        return "Crafts Room";
      case 2:
        return "Fish Tank";
      case 3:
        return "Boiler Room";
      case 4:
        return "Vault";
      case 5:
        return "Bulletin Board";
      case 6:
        return "Abandoned Joja Mart";
      default:
        return "";
    }
  }

  public static string getAreaEnglishDisplayNameFromNumber(int areaNumber)
  {
    return Game1.content.LoadBaseString("Strings\\Locations:CommunityCenter_AreaName_" + CommunityCenter.getAreaNameFromNumber(areaNumber).Replace(" ", ""));
  }

  public static string getAreaDisplayNameFromNumber(int areaNumber)
  {
    return Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_" + CommunityCenter.getAreaNameFromNumber(areaNumber).Replace(" ", ""));
  }

  public static StaticTile[] getJunimoNoteTileFrames(int area, Map map)
  {
    TileSheet tileSheet = map.GetTileSheet("indoor") ?? map.RequireTileSheet(0, "indoors");
    if (area == 5)
    {
      Layer layer = map.RequireLayer("Front");
      return new StaticTile[13]
      {
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1741),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1773),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1805),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1805),
        new StaticTile(layer, tileSheet, BlendMode.Alpha, 1773)
      };
    }
    Layer layer1 = map.RequireLayer("Buildings");
    return new StaticTile[20]
    {
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1832),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1824),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1825),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1826),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1827),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1828),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1829),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1830),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1831),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1832),
      new StaticTile(layer1, tileSheet, BlendMode.Alpha, 1833)
    };
  }
}
