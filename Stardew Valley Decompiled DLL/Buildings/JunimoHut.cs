// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.JunimoHut
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Buildings;

public class JunimoHut(Vector2 tileLocation) : Building("Junimo Hut", tileLocation)
{
  public int cropHarvestRadius = 8;
  /// <summary>Obsolete. This is only kept to preserve data from old save files. Use <see cref="M:StardewValley.Buildings.JunimoHut.GetOutputChest" /> instead.</summary>
  [XmlElement("output")]
  public Chest obsolete_output;
  [XmlElement("noHarvest")]
  public readonly NetBool noHarvest = new NetBool();
  [XmlElement("wasLit")]
  public readonly NetBool wasLit = new NetBool(false);
  private int junimoSendOutTimer;
  [XmlIgnore]
  public List<JunimoHarvester> myJunimos = new List<JunimoHarvester>();
  [XmlIgnore]
  public Point lastKnownCropLocation = Point.Zero;
  public NetInt raisinDays = new NetInt();
  [XmlElement("shouldSendOutJunimos")]
  public NetBool shouldSendOutJunimos = new NetBool(false);
  private Rectangle lightInteriorRect = new Rectangle(195, 0, 18, 17);
  private Rectangle bagRect = new Rectangle(208 /*0xD0*/, 51, 15, 13);

  public JunimoHut()
    : this(Vector2.Zero)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.noHarvest, "noHarvest").AddField((INetSerializable) this.wasLit, "wasLit").AddField((INetSerializable) this.shouldSendOutJunimos, "shouldSendOutJunimos").AddField((INetSerializable) this.raisinDays, "raisinDays");
    this.wasLit.fieldChangeVisibleEvent += (FieldChange<NetBool, bool>) ((field, old_value, new_value) => this.updateLightState());
  }

  public override Rectangle getRectForAnimalDoor(BuildingData data)
  {
    return new Rectangle((1 + this.tileX.Value) * 64 /*0x40*/, (this.tileY.Value + 1) * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
  }

  public override Rectangle? getSourceRectForMenu()
  {
    return new Rectangle?(new Rectangle(Game1.GetSeasonIndexForLocation(this.GetParentLocation()) * 48 /*0x30*/, 0, 48 /*0x30*/, 64 /*0x40*/));
  }

  public Chest GetOutputChest() => this.GetBuildingChest("Output");

  public override void dayUpdate(int dayOfMonth)
  {
    base.dayUpdate(dayOfMonth);
    this.myJunimos.Clear();
    this.wasLit.Value = false;
    this.shouldSendOutJunimos.Value = true;
    if (this.raisinDays.Value > 0 && !Game1.IsWinter)
      --this.raisinDays.Value;
    if (this.raisinDays.Value == 0 && !Game1.IsWinter)
    {
      Chest outputChest = this.GetOutputChest();
      if (outputChest.Items.CountId("(O)Raisins") > 0)
      {
        this.raisinDays.Value += 7;
        outputChest.Items.ReduceId("(O)Raisins", 1);
      }
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (allFarmer.isActive() && allFarmer.currentLocation != null && (allFarmer.currentLocation is FarmHouse || allFarmer.currentLocation.isStructure.Value))
        this.shouldSendOutJunimos.Value = false;
    }
  }

  public void sendOutJunimos() => this.junimoSendOutTimer = 1000;

  /// <inheritdoc />
  public override void performActionOnConstruction(GameLocation location, Farmer who)
  {
    base.performActionOnConstruction(location, who);
    this.sendOutJunimos();
  }

  public override void resetLocalState()
  {
    base.resetLocalState();
    this.updateLightState();
  }

  public void updateLightState()
  {
    if (!this.IsInCurrentLocation())
      return;
    string str = $"{nameof (JunimoHut)}_{this.tileX}_{this.tileY}";
    if (this.wasLit.Value)
    {
      if (Utility.getLightSource(str) == null)
        Game1.currentLightSources.Add(new LightSource(str, 4, new Vector2((float) (this.tileX.Value + 1), (float) (this.tileY.Value + 1)) * 64f + new Vector2(32f, 32f), 0.5f, onlyLocation: this.parentLocationName.Value));
      AmbientLocationSounds.addSound(new Vector2((float) (this.tileX.Value + 1), (float) (this.tileY.Value + 1)), 1);
    }
    else
    {
      Utility.removeLightSource(str);
      AmbientLocationSounds.removeSound(new Vector2((float) (this.tileX.Value + 1), (float) (this.tileY.Value + 1)));
    }
  }

  public int getUnusedJunimoNumber()
  {
    for (int unusedJunimoNumber = 0; unusedJunimoNumber < 3; ++unusedJunimoNumber)
    {
      if (unusedJunimoNumber >= this.myJunimos.Count)
        return unusedJunimoNumber;
      bool flag = false;
      foreach (JunimoHarvester junimo in this.myJunimos)
      {
        if (junimo.whichJunimoFromThisHut == unusedJunimoNumber)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return unusedJunimoNumber;
    }
    return 2;
  }

  public override void updateWhenFarmNotCurrentLocation(GameTime time)
  {
    base.updateWhenFarmNotCurrentLocation(time);
    GameLocation parentLocation = this.GetParentLocation();
    Chest outputChest = this.GetOutputChest();
    if (outputChest?.mutex != null)
    {
      outputChest.mutex.Update(parentLocation);
      if (outputChest.mutex.IsLockHeld() && Game1.activeClickableMenu == null)
        outputChest.mutex.ReleaseLock();
    }
    if (!Game1.IsMasterGame || this.junimoSendOutTimer <= 0 || !this.shouldSendOutJunimos.Value)
      return;
    this.junimoSendOutTimer -= time.ElapsedGameTime.Milliseconds;
    if (this.junimoSendOutTimer > 0 || this.myJunimos.Count >= 3 || parentLocation.IsWinterHere() || parentLocation.IsRainingHere() || !this.areThereMatureCropsWithinRadius() || !(parentLocation.NameOrUniqueName != "Farm") && Game1.farmEvent != null)
      return;
    int unusedJunimoNumber = this.getUnusedJunimoNumber();
    bool isPrismatic = false;
    Color? gemColor = this.getGemColor(ref isPrismatic);
    JunimoHarvester junimoHarvester = new JunimoHarvester(parentLocation, new Vector2((float) (this.tileX.Value + 1), (float) (this.tileY.Value + 1)) * 64f + new Vector2(0.0f, 32f), this, unusedJunimoNumber, gemColor);
    junimoHarvester.isPrismatic.Value = isPrismatic;
    parentLocation.characters.Add((NPC) junimoHarvester);
    this.myJunimos.Add(junimoHarvester);
    this.junimoSendOutTimer = 1000;
    if (!Utility.isOnScreen(Utility.Vector2ToPoint(new Vector2((float) (this.tileX.Value + 1), (float) (this.tileY.Value + 1))), 64 /*0x40*/, parentLocation))
      return;
    try
    {
      parentLocation.playSound("junimoMeep1");
    }
    catch (Exception ex)
    {
    }
  }

  public override void Update(GameTime time)
  {
    if (!this.shouldSendOutJunimos.Value)
      this.shouldSendOutJunimos.Value = true;
    base.Update(time);
  }

  private Color? getGemColor(ref bool isPrismatic)
  {
    List<Color> colorList = new List<Color>();
    foreach (Item dye_object in this.GetOutputChest().Items)
    {
      if (dye_object != null && (dye_object.Category == -12 || dye_object.Category == -2))
      {
        Color? dyeColor = TailoringMenu.GetDyeColor(dye_object);
        if (dye_object.QualifiedItemId == "(O)74")
          isPrismatic = true;
        if (dyeColor.HasValue)
          colorList.Add(dyeColor.Value);
      }
    }
    return colorList.Count > 0 ? new Color?(colorList[Game1.random.Next(colorList.Count)]) : new Color?();
  }

  public bool areThereMatureCropsWithinRadius()
  {
    GameLocation parentLocation = this.GetParentLocation();
    for (int index1 = this.tileX.Value + 1 - this.cropHarvestRadius; index1 < this.tileX.Value + 2 + this.cropHarvestRadius; ++index1)
    {
      for (int index2 = this.tileY.Value - this.cropHarvestRadius + 1; index2 < this.tileY.Value + 2 + this.cropHarvestRadius; ++index2)
      {
        TerrainFeature terrainFeature;
        if (parentLocation.terrainFeatures.TryGetValue(new Vector2((float) index1, (float) index2), out terrainFeature))
        {
          if (parentLocation.isCropAtTile(index1, index2) && ((HoeDirt) terrainFeature).readyForHarvest())
          {
            this.lastKnownCropLocation = new Point(index1, index2);
            return true;
          }
          if (terrainFeature is Bush bush && bush.readyForHarvest())
          {
            this.lastKnownCropLocation = new Point(index1, index2);
            return true;
          }
        }
      }
    }
    this.lastKnownCropLocation = Point.Zero;
    return false;
  }

  public override void performTenMinuteAction(int timeElapsed)
  {
    base.performTenMinuteAction(timeElapsed);
    GameLocation parentLocation = this.GetParentLocation();
    if (this.myJunimos.Count > 0)
    {
      for (int index = this.myJunimos.Count - 1; index >= 0; --index)
      {
        if (!parentLocation.characters.Contains((NPC) this.myJunimos[index]))
          this.myJunimos.RemoveAt(index);
        else
          this.myJunimos[index].pokeToHarvest();
      }
    }
    if (this.myJunimos.Count < 3 && Game1.timeOfDay < 1900)
      this.junimoSendOutTimer = 1;
    if (Game1.timeOfDay >= 2000 && Game1.timeOfDay < 2400)
    {
      if (parentLocation.IsWinterHere() || Game1.random.NextDouble() >= 0.2)
        return;
      this.wasLit.Value = true;
    }
    else
    {
      if (Game1.timeOfDay != 2400 || parentLocation.IsWinterHere())
        return;
      this.wasLit.Value = false;
    }
  }

  public override bool doAction(Vector2 tileLocation, Farmer who)
  {
    if (who.ActiveObject != null && who.ActiveObject.IsFloorPathItem() && who.currentLocation != null && !who.currentLocation.terrainFeatures.ContainsKey(tileLocation))
      return false;
    if (!this.occupiesTile(tileLocation))
      return base.doAction(tileLocation, who);
    Chest output = this.GetOutputChest();
    if (output.Items.Count > 36)
      output.clearNulls();
    output.mutex.RequestLock((Action) (() => Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) output.Items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(output.grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(output.grabItemFromChest), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, whichSpecialButton: 1, context: (object) this)));
    return true;
  }

  public override void drawInMenu(SpriteBatch b, int x, int y)
  {
    this.drawShadow(b, x, y);
    b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Rectangle?(new Rectangle(0, 0, 48 /*0x30*/, 64 /*0x40*/)), this.color, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 0.89f);
  }

  public override void draw(SpriteBatch b)
  {
    if (this.isMoving)
      return;
    if (this.daysOfConstructionLeft.Value > 0)
    {
      this.drawInConstruction(b);
    }
    else
    {
      this.drawShadow(b);
      Rectangle rectangle = this.getSourceRectForMenu() ?? this.getSourceRect();
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/))), new Rectangle?(rectangle), this.color * this.alpha, 0.0f, new Vector2(0.0f, (float) this.texture.Value.Bounds.Height), 4f, SpriteEffects.None, (float) ((this.tileY.Value + this.tilesHigh.Value - 1) * 64 /*0x40*/) / 10000f);
      if (this.raisinDays.Value > 0 && !Game1.IsWinter)
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 12), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ + 20))), new Rectangle?(new Rectangle(246, 46, 10, 18)), this.color * this.alpha, 0.0f, new Vector2(0.0f, 18f), 4f, SpriteEffects.None, (float) ((this.tileY.Value + this.tilesHigh.Value - 1) * 64 /*0x40*/ + 2) / 10000f);
      bool flag = false;
      Chest outputChest = this.GetOutputChest();
      if (outputChest != null)
      {
        foreach (Item obj in outputChest.Items)
        {
          if (obj != null && obj.Category != -12 && obj.Category != -2)
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 128 /*0x80*/ + 12), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 32 /*0x20*/))), new Rectangle?(this.bagRect), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.tileY.Value + this.tilesHigh.Value - 1) * 64 /*0x40*/ + 1) / 10000f);
      if (Game1.timeOfDay < 2000 || Game1.timeOfDay >= 2400 || !this.wasLit.Value || this.GetParentLocation().IsWinterHere())
        return;
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.tileX.Value * 64 /*0x40*/ + 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/ - 64 /*0x40*/))), new Rectangle?(this.lightInteriorRect), this.color * this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((this.tileY.Value + this.tilesHigh.Value - 1) * 64 /*0x40*/ + 1) / 10000f);
    }
  }
}
