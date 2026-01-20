// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.CrabPot
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.Locations;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class CrabPot : StardewValley.Object
{
  public const int lidFlapTimerInterval = 60;
  [XmlIgnore]
  public float yBob;
  [XmlElement("directionOffset")]
  public readonly NetVector2 directionOffset = new NetVector2();
  [XmlElement("bait")]
  public readonly NetRef<StardewValley.Object> bait = new NetRef<StardewValley.Object>();
  public int tileIndexToShow;
  [XmlIgnore]
  public bool lidFlapping;
  [XmlIgnore]
  public bool lidClosing;
  [XmlIgnore]
  public float lidFlapTimer;
  [XmlIgnore]
  public float shakeTimer;
  [XmlIgnore]
  public Vector2 shake;
  [XmlIgnore]
  private int ignoreRemovalTimer;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.directionOffset, "directionOffset").AddField((INetSerializable) this.bait, "bait");
  }

  public CrabPot()
    : base("710", 1)
  {
    this.CanBeGrabbed = false;
    this.type.Value = "interactive";
    this.tileIndexToShow = this.ParentSheetIndex;
  }

  /// <summary>Get whether the crab pot currently needs to be filled with bait before it can catch something.</summary>
  /// <param name="player">The player checking the crab pot.</param>
  public bool NeedsBait(Farmer player)
  {
    return this.bait.Value == null && !(Game1.GetPlayer(this.owner.Value) ?? player ?? Game1.player).professions.Contains(11);
  }

  public List<Vector2> getOverlayTiles()
  {
    List<Vector2> tiles = new List<Vector2>();
    if (this.Location != null)
    {
      if ((double) this.directionOffset.Y < 0.0)
        this.addOverlayTilesIfNecessary((int) this.TileLocation.X, (int) this.tileLocation.Y, tiles);
      this.addOverlayTilesIfNecessary((int) this.TileLocation.X, (int) this.tileLocation.Y + 1, tiles);
      if ((double) this.directionOffset.X < 0.0)
        this.addOverlayTilesIfNecessary((int) this.TileLocation.X - 1, (int) this.tileLocation.Y + 1, tiles);
      if ((double) this.directionOffset.X > 0.0)
        this.addOverlayTilesIfNecessary((int) this.TileLocation.X + 1, (int) this.tileLocation.Y + 1, tiles);
    }
    return tiles;
  }

  protected void addOverlayTilesIfNecessary(int tile_x, int tile_y, List<Vector2> tiles)
  {
    GameLocation location = this.Location;
    if (location == null || location != Game1.currentLocation || !location.hasTileAt(tile_x, tile_y, "Buildings") || location.isWaterTile(tile_x, tile_y + 1))
      return;
    tiles.Add(new Vector2((float) tile_x, (float) tile_y));
  }

  /// <summary>Add any tiles that might overlap with this crab pot incorrectly to the <see cref="F:StardewValley.Game1.crabPotOverlayTiles" /> dictionary.</summary>
  public void addOverlayTiles()
  {
    GameLocation location = this.Location;
    if (location == null || location != Game1.currentLocation)
      return;
    foreach (Vector2 overlayTile in this.getOverlayTiles())
    {
      int num;
      if (!Game1.crabPotOverlayTiles.TryGetValue(overlayTile, out num))
        Game1.crabPotOverlayTiles[overlayTile] = num = 0;
      Game1.crabPotOverlayTiles[overlayTile] = num + 1;
    }
  }

  /// <summary>Remove any tiles that might overlap with this crab pot incorrectly from the <see cref="F:StardewValley.Game1.crabPotOverlayTiles" /> dictionary.</summary>
  public void removeOverlayTiles()
  {
    if (this.Location == null || this.Location != Game1.currentLocation)
      return;
    foreach (Vector2 overlayTile in this.getOverlayTiles())
    {
      int num;
      if (Game1.crabPotOverlayTiles.TryGetValue(overlayTile, out num))
      {
        --num;
        if (num <= 0)
          Game1.crabPotOverlayTiles.Remove(overlayTile);
        else
          Game1.crabPotOverlayTiles[overlayTile] = num;
      }
    }
  }

  public static bool IsValidCrabPotLocationTile(GameLocation location, int x, int y)
  {
    switch (location)
    {
      case Caldera _:
      case VolcanoDungeon _:
      case MineShaft _:
        return false;
      default:
        Vector2 key = new Vector2((float) x, (float) y);
        bool flag = location.isWaterTile(x + 1, y) && location.isWaterTile(x - 1, y) || location.isWaterTile(x, y + 1) && location.isWaterTile(x, y - 1);
        return !location.objects.ContainsKey(key) && flag && location.isWaterTile((int) key.X, (int) key.Y) && location.doesTileHaveProperty((int) key.X, (int) key.Y, "Passable", "Buildings") == null;
    }
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    this.updateOffset();
    this.addOverlayTiles();
    base.actionOnPlayerEntry();
  }

  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    Vector2 vector2 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    if (who != null)
      this.owner.Value = who.UniqueMultiplayerID;
    if (!CrabPot.IsValidCrabPotLocationTile(location, (int) vector2.X, (int) vector2.Y))
      return false;
    this.TileLocation = vector2;
    location.objects.Add(this.tileLocation.Value, (StardewValley.Object) this);
    location.playSound("waterSlosh");
    DelayedAction.playSoundAfterDelay("slosh", 150);
    this.updateOffset();
    this.addOverlayTiles();
    return true;
  }

  public void updateOffset()
  {
    Vector2 zero = Vector2.Zero;
    if (this.checkLocation(this.tileLocation.X - 1f, this.tileLocation.Y))
      zero += new Vector2(32f, 0.0f);
    if (this.checkLocation(this.tileLocation.X + 1f, this.tileLocation.Y))
      zero += new Vector2(-32f, 0.0f);
    if ((double) zero.X != 0.0 && this.checkLocation(this.tileLocation.X + (float) Math.Sign(zero.X), this.tileLocation.Y + 1f))
      zero += new Vector2(0.0f, -42f);
    if (this.checkLocation(this.tileLocation.X, this.tileLocation.Y - 1f))
      zero += new Vector2(0.0f, 32f);
    if (this.checkLocation(this.tileLocation.X, this.tileLocation.Y + 1f))
      zero += new Vector2(0.0f, -42f);
    this.directionOffset.Value = zero;
  }

  protected bool checkLocation(float tile_x, float tile_y)
  {
    GameLocation location = this.Location;
    return !location.isWaterTile((int) tile_x, (int) tile_y) || location.doesTileHaveProperty((int) tile_x, (int) tile_y, "Passable", "Buildings") != null;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new StardewValley.Object(this.ItemId, 1);

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    GameLocation location = this.Location;
    if (location == null || !(dropInItem is StardewValley.Object @object) || @object.Category != -21 || !this.NeedsBait(who))
      return false;
    if (!probe)
    {
      if (who != null)
        this.owner.Value = who.UniqueMultiplayerID;
      this.bait.Value = @object.getOne() as StardewValley.Object;
      location.playSound("Ship");
      this.lidFlapping = true;
      this.lidFlapTimer = 60f;
    }
    return true;
  }

  /// <inheritdoc />
  public override bool AttemptAutoLoad(IInventory inventory, Farmer who)
  {
    StardewValley.Object @object = this.bait.Value;
    if (!base.AttemptAutoLoad(inventory, who) || @object == this.bait.Value)
      return false;
    inventory.ReduceId(this.bait.Value.QualifiedItemId, this.bait.Value.Stack);
    return true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (this.tileIndexToShow == 714)
    {
      if (justCheckingForActivity)
        return true;
      StardewValley.Object @object = this.heldObject.Value;
      if (@object != null)
      {
        int stack = @object.Stack;
        if (Utility.CreateDaySaveRandom((double) Game1.uniqueIDForThisGame, (double) (Game1.stats.DaysPlayed * 77U), (double) this.tileLocation.X * 777.0 + (double) this.tileLocation.Y).NextDouble() < 0.25 && Game1.player.stats.Get("Book_Crabbing") > 0U && who.couldInventoryAcceptThisItem(@object.QualifiedItemId, stack * 2, @object.Quality))
          stack *= 2;
        @object.Stack = stack;
        this.heldObject.Value = (StardewValley.Object) null;
        if (who.IsLocalPlayer && !who.addItemToInventoryBool((Item) @object))
        {
          this.heldObject.Value = @object;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          return false;
        }
        string str;
        if (DataLoader.Fish(Game1.content).TryGetValue(@object.ItemId, out str))
        {
          string[] strArray = str.Split('/');
          int minValue = strArray.Length > 5 ? Convert.ToInt32(strArray[5]) : 1;
          int num = strArray.Length > 5 ? Convert.ToInt32(strArray[6]) : 10;
          who.caughtFish(@object.QualifiedItemId, Game1.random.Next(minValue, num + 1), numberCaught: stack);
        }
        who.gainExperience(1, 5);
      }
      this.readyForHarvest.Value = false;
      this.tileIndexToShow = 710;
      this.lidFlapping = true;
      this.lidFlapTimer = 60f;
      this.bait.Value = (StardewValley.Object) null;
      who.animateOnce(279 + who.FacingDirection);
      location.playSound("fishingRodBend");
      DelayedAction.playSoundAfterDelay("coin", 500);
      this.shake = Vector2.Zero;
      this.shakeTimer = 0.0f;
      this.ignoreRemovalTimer = 750;
      return true;
    }
    if (this.bait.Value == null && this.ignoreRemovalTimer <= 0)
    {
      if (justCheckingForActivity)
        return true;
      if (Game1.didPlayerJustClickAtAll(true))
      {
        if (Game1.player.addItemToInventoryBool(this.getOne()))
        {
          if (who.isMoving())
            Game1.haltAfterCheck = false;
          Game1.playSound("coin");
          location.objects.Remove(this.tileLocation.Value);
          return true;
        }
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      }
    }
    return false;
  }

  public override void performRemoveAction()
  {
    this.removeOverlayTiles();
    base.performRemoveAction();
  }

  public override void DayUpdate()
  {
    GameLocation location = this.Location;
    Farmer player = Game1.GetPlayer(this.owner.Value) ?? Game1.MasterPlayer;
    bool flag1 = player.professions.Contains(10);
    if (this.NeedsBait(player) || this.heldObject.Value != null)
      return;
    this.tileIndexToShow = 714;
    this.readyForHarvest.Value = true;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) this.tileLocation.X * 1000.0, (double) this.tileLocation.Y * (double) byte.MaxValue, (double) this.directionOffset.X * 1000.0 + (double) this.directionOffset.Y);
    List<string> options = new List<string>();
    string id;
    FishAreaData data;
    if (!location.TryGetFishAreaForTile(this.tileLocation.Value, out id, out data))
      data = (FishAreaData) null;
    double chance = flag1 ? 0.0 : (double) data?.CrabPotJunkChance ?? 0.2;
    int amount = 1;
    int quality = 0;
    string str1 = (string) null;
    id = this.bait.Value?.QualifiedItemId;
    switch (id)
    {
      case "(O)DeluxeBait":
        quality = 1;
        chance /= 2.0;
        break;
      case "(O)774":
        chance /= 2.0;
        if (daySaveRandom.NextBool(0.25))
        {
          amount = 2;
          break;
        }
        break;
      case "(O)SpecificBait":
        if (this.bait.Value.preservedParentSheetIndex.Value != null && this.bait.Value.preserve.Value.HasValue)
        {
          str1 = this.bait.Value.preservedParentSheetIndex.Value;
          chance /= 2.0;
          break;
        }
        break;
    }
    if (!daySaveRandom.NextBool(chance))
    {
      IList<string> crabPotFishForTile = location.GetCrabPotFishForTile(this.tileLocation.Value);
      foreach (KeyValuePair<string, string> keyValuePair in DataLoader.Fish(Game1.content))
      {
        if (keyValuePair.Value.Contains("trap"))
        {
          string[] strArray1 = keyValuePair.Value.Split('/');
          string[] strArray2 = ArgUtility.SplitBySpace(strArray1[4]);
          bool flag2 = false;
          foreach (string str2 in strArray2)
          {
            foreach (string str3 in (IEnumerable<string>) crabPotFishForTile)
            {
              if (str2 == str3)
              {
                flag2 = true;
                break;
              }
            }
          }
          if (flag2)
          {
            if (flag1)
            {
              options.Add(keyValuePair.Key);
            }
            else
            {
              double num = Convert.ToDouble(strArray1[2]);
              if (str1 != null && str1 == keyValuePair.Key)
                num *= num < 0.1 ? 4.0 : (num < 0.2 ? 3.0 : 2.0);
              if (daySaveRandom.NextDouble() < num)
              {
                this.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)" + keyValuePair.Key, amount, quality);
                break;
              }
            }
          }
        }
      }
    }
    if (this.heldObject.Value != null)
      return;
    if (flag1 && options.Count > 0)
      this.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)" + daySaveRandom.ChooseFrom<string>((IList<string>) options), amount, quality);
    else
      this.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)" + daySaveRandom.Next(168, 173).ToString());
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.lidFlapping)
    {
      this.lidFlapTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.lidFlapTimer <= 0.0)
      {
        this.tileIndexToShow += this.lidClosing ? -1 : 1;
        if (this.tileIndexToShow >= 713 && !this.lidClosing)
        {
          this.lidClosing = true;
          --this.tileIndexToShow;
        }
        else if (this.tileIndexToShow <= 709 && this.lidClosing)
        {
          this.lidClosing = false;
          ++this.tileIndexToShow;
          this.lidFlapping = false;
          if (this.bait.Value != null)
            this.tileIndexToShow = 713;
        }
        this.lidFlapTimer = 60f;
      }
    }
    if (this.readyForHarvest.Value && this.heldObject.Value != null)
    {
      this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.shakeTimer < 0.0)
        this.shakeTimer = (float) Game1.random.Next(2800, 3200);
    }
    this.shake.X = (double) this.shakeTimer <= 2000.0 ? 0.0f : (float) Game1.random.Next(-1, 2);
    if (this.ignoreRemovalTimer <= 0)
      return;
    this.ignoreRemovalTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (this.heldObject.Value != null)
      this.tileIndexToShow = 714;
    else if (this.tileIndexToShow == 0)
      this.tileIndexToShow = this.ParentSheetIndex;
    TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;
    this.yBob = (float) (Math.Sin(totalGameTime.TotalMilliseconds / 500.0 + (double) (x * 64 /*0x40*/)) * 8.0 + 8.0);
    if ((double) this.yBob <= 1.0 / 1000.0)
      location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), 150f, 8, 0, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/ + 4), (float) (y * 64 /*0x40*/ + 32 /*0x20*/)), false, Game1.random.NextBool(), 1f / 1000f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
    spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ + (int) this.yBob))) + this.shake, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.tileIndexToShow, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (((double) (y * 64 /*0x40*/) + (double) this.directionOffset.Y + (double) (x % 4)) / 10000.0));
    if (location.waterTiles != null && x < location.waterTiles.waterTiles.GetLength(0) && y < location.waterTiles.waterTiles.GetLength(1) && location.waterTiles.waterTiles[x, y].isWater)
    {
      if (location.waterTiles.waterTiles[x, y].isVisible)
      {
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/ + 4), (float) (y * 64 /*0x40*/ + 48 /*0x30*/))) + this.shake, new Rectangle?(new Rectangle(location.waterAnimationIndex * 64 /*0x40*/, 2112 + ((x + y) % 2 == 0 ? (location.waterTileFlip ? 128 /*0x80*/ : 0) : (location.waterTileFlip ? 0 : 128 /*0x80*/)), 56, 16 /*0x10*/ + (int) this.yBob)), location.waterColor.Value, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) (((double) (y * 64 /*0x40*/) + (double) this.directionOffset.Y + (double) (x % 4)) / 9999.0));
      }
      else
      {
        Color color = Utility.MultiplyColor(new Color(135, 135, 135, 215), location.waterColor.Value);
        spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/ + 4), (float) (y * 64 /*0x40*/ + 48 /*0x30*/))) + this.shake, new Rectangle?(), color, 0.0f, Vector2.Zero, new Vector2(56f, (float) (16 /*0x10*/ + (int) this.yBob)), SpriteEffects.None, (float) (((double) (y * 64 /*0x40*/) + (double) this.directionOffset.Y + (double) (x % 4)) / 9999.0));
      }
    }
    if (!this.readyForHarvest.Value || this.heldObject.Value == null)
      return;
    totalGameTime = Game1.currentGameTime.TotalGameTime;
    float num = (float) (4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));
    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/ - 8), (float) (y * 64 /*0x40*/ - 96 /*0x60*/ - 16 /*0x10*/) + num)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999999747524271E-07 + (double) this.tileLocation.X / 10000.0));
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.heldObject.Value.QualifiedItemId);
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, this.directionOffset.Value + new Vector2((float) (x * 64 /*0x40*/ + 32 /*0x20*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/ - 8) + num)), new Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) ((double) ((y + 1) * 64 /*0x40*/) / 10000.0 + 9.9999997473787516E-06 + (double) this.tileLocation.X / 10000.0));
  }
}
