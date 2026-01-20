// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Furniture
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

[XmlInclude(typeof (BedFurniture))]
[XmlInclude(typeof (RandomizedPlantFurniture))]
[XmlInclude(typeof (StorageFurniture))]
[XmlInclude(typeof (TV))]
public class Furniture : StardewValley.Object, ISittable
{
  public const int chair = 0;
  public const int bench = 1;
  public const int couch = 2;
  public const int armchair = 3;
  public const int dresser = 4;
  public const int longTable = 5;
  public const int painting = 6;
  public const int lamp = 7;
  public const int decor = 8;
  public const int other = 9;
  public const int bookcase = 10;
  public const int table = 11;
  public const int rug = 12;
  public const int window = 13;
  public const int fireplace = 14;
  public const int bed = 15;
  public const int torch = 16 /*0x10*/;
  public const int sconce = 17;
  public const string furnitureTextureName = "TileSheets\\furniture";
  [XmlElement("furniture_type")]
  public readonly NetInt furniture_type = new NetInt();
  [XmlElement("rotations")]
  public readonly NetInt rotations = new NetInt();
  [XmlElement("currentRotation")]
  public readonly NetInt currentRotation = new NetInt();
  [XmlElement("sourceIndexOffset")]
  private readonly NetInt sourceIndexOffset = new NetInt();
  [XmlElement("drawPosition")]
  protected readonly NetVector2 drawPosition = new NetVector2();
  [XmlElement("sourceRect")]
  public readonly NetRectangle sourceRect = new NetRectangle();
  [XmlElement("defaultSourceRect")]
  public readonly NetRectangle defaultSourceRect = new NetRectangle();
  [XmlElement("defaultBoundingBox")]
  public readonly NetRectangle defaultBoundingBox = new NetRectangle();
  [XmlElement("drawHeldObjectLow")]
  public readonly NetBool drawHeldObjectLow = new NetBool();
  [XmlIgnore]
  public NetLongDictionary<int, NetInt> sittingFarmers = new NetLongDictionary<int, NetInt>();
  [XmlIgnore]
  public Vector2? lightGlowPosition;
  /// <summary>Whether this furniture can be removed if other checks pass.</summary>
  /// <remarks>This value only applies for the current instance, it's not synced in multiplayer or written to the save file.</remarks>
  [XmlIgnore]
  public bool AllowLocalRemoval = true;
  public static bool isDrawingLocationFurniture;
  protected static Dictionary<string, string> _frontTextureName;
  [XmlIgnore]
  private int _placementRestriction = -1;
  [XmlIgnore]
  private string _description;

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.furniture_type, "furniture_type").AddField((INetSerializable) this.rotations, "rotations").AddField((INetSerializable) this.currentRotation, "currentRotation").AddField((INetSerializable) this.sourceIndexOffset, "sourceIndexOffset").AddField((INetSerializable) this.drawPosition, "drawPosition").AddField((INetSerializable) this.sourceRect, "sourceRect").AddField((INetSerializable) this.defaultSourceRect, "defaultSourceRect").AddField((INetSerializable) this.defaultBoundingBox, "defaultBoundingBox").AddField((INetSerializable) this.drawHeldObjectLow, "drawHeldObjectLow").AddField((INetSerializable) this.sittingFarmers, "sittingFarmers");
  }

  [XmlIgnore]
  public int placementRestriction
  {
    get
    {
      if (this._placementRestriction < 0)
      {
        bool flag = true;
        string[] data = this.getData();
        if (data != null && data.Length > 6 && int.TryParse(data[6], out this._placementRestriction) && this._placementRestriction >= 0)
          flag = false;
        if (flag)
          this._placementRestriction = !this.name.Contains("TV") ? (this.IsTable() || this.furniture_type.Value == 1 || this.furniture_type.Value == 0 || this.furniture_type.Value == 8 || this.furniture_type.Value == 16 /*0x10*/ ? 2 : 0) : 0;
      }
      return this._placementRestriction;
    }
  }

  [XmlIgnore]
  public string description
  {
    get
    {
      if (this._description == null)
        this._description = this.loadDescription();
      return this._description;
    }
  }

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(F)";

  public Furniture()
  {
    this.updateDrawPosition();
    this.isOn.Value = false;
  }

  public Furniture(string itemId, Vector2 tile, int initialRotations)
    : this(itemId, tile)
  {
    for (int index = 0; index < initialRotations; ++index)
      this.rotate();
    this.isOn.Value = false;
  }

  public virtual void OnAdded(GameLocation loc, Vector2 tilePos)
  {
    if (this.IntersectsForCollision(Game1.player.GetBoundingBox()))
      Game1.player.TemporaryPassableTiles.Add(this.GetBoundingBoxAt((int) tilePos.X, (int) tilePos.Y));
    if (this.furniture_type.Value == 13)
    {
      if (loc != null && loc.IsRainingHere())
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 1;
      }
      else
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 0;
        this.AddLightGlow();
      }
    }
    this.minutesElapsed(1);
  }

  public void OnRemoved(GameLocation loc, Vector2 tilePos) => this.RemoveLightGlow();

  public override bool IsHeldOverHead() => false;

  /// <summary>Whether this is a table, which can have items placed on it.</summary>
  public virtual bool IsTable()
  {
    int num = this.furniture_type.Value;
    return num == 11 || num == 5;
  }

  public static Rectangle GetDefaultSourceRect(string itemId, Texture2D texture = null)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(F)" + itemId);
    string[] data = Furniture.getData(itemId);
    if (data == null)
      return dataOrErrorItem.GetSourceRect();
    if (data[2].Equals("-1"))
      return Furniture.getDefaultSourceRectForType(dataOrErrorItem, Furniture.getTypeNumberFromName(data[1]), texture);
    string[] strArray = ArgUtility.SplitBySpace(data[2]);
    int int32_1 = Convert.ToInt32(strArray[0]);
    int int32_2 = Convert.ToInt32(strArray[1]);
    return Furniture.getDefaultSourceRect(dataOrErrorItem, int32_1, int32_2, texture);
  }

  /// <summary>Set the furniture's position and rotation, and update all related data.</summary>
  /// <param name="x">The tile X position.</param>
  /// <param name="y">The tile X position.</param>
  /// <param name="rotations">The number of times to rotate the furniture, starting from its current rotation.</param>
  /// <returns>Returns the furniture instance for chaining.</returns>
  public Furniture SetPlacement(int x, int y, int rotations = 0)
  {
    return this.SetPlacement(new Vector2((float) x, (float) y), rotations);
  }

  /// <summary>Set the furniture's position and rotation, and update all related data.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="rotations">The number of times to rotate the furniture, starting from its current rotation.</param>
  /// <returns>Returns the furniture instance for chaining.</returns>
  public Furniture SetPlacement(Point tile, int rotations = 0)
  {
    return this.SetPlacement(Utility.PointToVector2(tile), rotations);
  }

  /// <summary>Set the furniture's position and rotation, and update all related data.</summary>
  /// <param name="tile">The tile position.</param>
  /// <param name="rotations">The number of times to rotate the furniture, starting from its current rotation.</param>
  /// <returns>Returns the furniture instance for chaining.</returns>
  public Furniture SetPlacement(Vector2 tile, int rotations = 0)
  {
    this.InitializeAtTile(tile);
    for (int index = 0; index < rotations; ++index)
      this.rotate();
    return this;
  }

  /// <summary>Set the held object.</summary>
  /// <param name="obj">The object to hold.</param>
  /// <returns>Returns the furniture instance for chaining.</returns>
  public Furniture SetHeldObject(StardewValley.Object obj)
  {
    this.heldObject.Value = obj;
    if (obj != null)
    {
      if (obj is Furniture furniture)
        furniture.InitializeAtTile(this.TileLocation);
      else
        obj.TileLocation = this.TileLocation;
    }
    return this;
  }

  /// <summary>Set the furniture's tile position and update all position-related data.</summary>
  /// <param name="tile">The tile position.</param>
  public void InitializeAtTile(Vector2 tile)
  {
    Texture2D texture = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).GetTexture();
    string[] data = this.getData();
    if (data != null)
    {
      this.furniture_type.Value = Furniture.getTypeNumberFromName(data[1]);
      this.defaultSourceRect.Value = new Rectangle(this.ParentSheetIndex * 16 /*0x10*/ % texture.Width, this.ParentSheetIndex * 16 /*0x10*/ / texture.Width * 16 /*0x10*/, 1, 1);
      this.drawHeldObjectLow.Value = this.Name.ContainsIgnoreCase("tea");
      this.sourceRect.Value = Furniture.GetDefaultSourceRect(this.ItemId);
      this.defaultSourceRect.Value = this.sourceRect.Value;
      this.rotations.Value = Convert.ToInt32(data[4]);
      this.price.Value = Convert.ToInt32(data[5]);
    }
    else
      this.defaultSourceRect.Value = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).GetSourceRect();
    if (tile != this.TileLocation)
      this.TileLocation = tile;
    else
      this.RecalculateBoundingBox(data);
  }

  public Furniture(string itemId, Vector2 tile)
  {
    this.isOn.Value = false;
    this.ItemId = itemId;
    this.ResetParentSheetIndex();
    this.name = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).InternalName;
    this.InitializeAtTile(tile);
  }

  /// <inheritdoc />
  public override void RecalculateBoundingBox() => this.RecalculateBoundingBox(this.getData());

  /// <summary>Recalculate the item's bounding box based on its current position.</summary>
  /// <param name="data">The furniture data to apply.</param>
  private void RecalculateBoundingBox(string[] data)
  {
    Rectangle rectangle;
    switch (ArgUtility.Get(data, 3))
    {
      case null:
        rectangle = new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
        break;
      case "-1":
        rectangle = this.getDefaultBoundingBoxForType(this.furniture_type.Value);
        break;
      default:
        string[] strArray = ArgUtility.SplitBySpace(data[3]);
        rectangle = new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, Convert.ToInt32(strArray[0]) * 64 /*0x40*/, Convert.ToInt32(strArray[1]) * 64 /*0x40*/);
        break;
    }
    this.defaultBoundingBox.Value = rectangle;
    this.boundingBox.Value = rectangle;
    this.updateRotation();
  }

  protected string[] getData() => Furniture.getData(this.ItemId);

  protected static string[] getData(string itemId)
  {
    string str;
    return !DataLoader.Furniture(Game1.content).TryGetValue(itemId, out str) ? (string[]) null : str.Split('/');
  }

  /// <inheritdoc />
  protected override string loadDisplayName()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).DisplayName;
  }

  protected virtual string loadDescription()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    if (dataOrErrorItem.IsErrorItem)
      return dataOrErrorItem.Description;
    string qualifiedItemId = this.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 7:
          switch (qualifiedItemId[4])
          {
            case '2':
              if (qualifiedItemId == "(F)1226")
                return Game1.parseText(Game1.content.LoadString("Strings\\Objects:FurnitureCatalogueDescription"), Game1.smallFont, 320);
              break;
            case '3':
              if (qualifiedItemId == "(F)1308")
                return Game1.parseText(Game1.content.LoadString("Strings\\Objects:CatalogueDescription"), Game1.smallFont, 320);
              break;
          }
          break;
        case 16 /*0x10*/:
          if (qualifiedItemId == "(F)JojaCatalogue")
            return Game1.content.LoadString("Strings\\1_6_Strings:JojaCatalogueDescription");
          break;
        case 17:
          switch (qualifiedItemId[3])
          {
            case 'R':
              if (qualifiedItemId == "(F)RetroCatalogue")
                return Game1.content.LoadString("Strings\\1_6_Strings:RetroCatalogueDescription");
              break;
            case 'T':
              if (qualifiedItemId == "(F)TrashCatalogue")
                return Game1.content.LoadString("Strings\\1_6_Strings:TrashCatalogueDescription");
              break;
          }
          break;
        case 18:
          switch (qualifiedItemId[3])
          {
            case 'J':
              if (qualifiedItemId == "(F)JunimoCatalogue")
                return Game1.content.LoadString("Strings\\1_6_Strings:JunimoCatalogueDescription");
              break;
            case 'W':
              if (qualifiedItemId == "(F)WizardCatalogue")
                return Game1.content.LoadString("Strings\\1_6_Strings:WizardCatalogueDescription");
              break;
          }
          break;
      }
    }
    switch (this.placementRestriction)
    {
      case 0:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_NotOutdoors");
      case 1:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors_Description");
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Decoration_Description");
      default:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture.cs.12623");
    }
  }

  public override string getDescription()
  {
    return Game1.parseText(this.description, Game1.smallFont, this.getDescriptionWidth());
  }

  /// <inheritdoc />
  public override Color getCategoryColor() => new Color(100, 25, 190);

  /// <inheritdoc />
  public override bool performDropDownAction(Farmer who)
  {
    this.actionOnPlayerEntryOrPlacement(this.Location, true);
    return false;
  }

  public override void hoverAction()
  {
    base.hoverAction();
    if (Game1.player.isInventoryFull())
      return;
    Game1.mouseCursor = Game1.cursor_grab;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    if (justCheckingForActivity)
      return true;
    string qualifiedItemId = this.QualifiedItemId;
    if (qualifiedItemId != null)
    {
      switch (qualifiedItemId.Length)
      {
        case 6:
          switch (qualifiedItemId[4])
          {
            case '0':
              if (qualifiedItemId == "(F)704" || qualifiedItemId == "(F)709")
                break;
              break;
            case '1':
              if (qualifiedItemId == "(F)714" || qualifiedItemId == "(F)719")
                break;
              break;
          }
          break;
        case 7:
          switch (qualifiedItemId[6])
          {
            case '2':
              if (qualifiedItemId == "(F)1402")
              {
                Game1.activeClickableMenu = (IClickableMenu) new Billboard();
                return true;
              }
              break;
            case '6':
              if (qualifiedItemId == "(F)1226")
              {
                Utility.TryOpenShopMenu("Furniture Catalogue", location);
                return true;
              }
              break;
            case '8':
              if (qualifiedItemId == "(F)1308")
              {
                Utility.TryOpenShopMenu("Catalogue", location);
                return true;
              }
              break;
            case '9':
              if (qualifiedItemId == "(F)1309")
              {
                Game1.playSound("openBox");
                this.shakeTimer = 500;
                if (Game1.getMusicTrackName().Equals("sam_acoustic1"))
                  Game1.changeMusicTrack("none", true);
                else
                  Game1.changeMusicTrack("sam_acoustic1");
                return true;
              }
              break;
          }
          break;
        case 11:
          if (qualifiedItemId == "(F)Cauldron")
          {
            this.IsOn = !this.IsOn;
            this.SpecialVariable = this.IsOn ? 388859 : 0;
            if (this.IsOn)
            {
              location.playSound("fireball");
              location.playSound("bubbles");
              for (int index = 0; index < 13; ++index)
                this.addCauldronBubbles((float) (-0.5 - (double) index * 0.20000000298023224));
              break;
            }
            break;
          }
          break;
        case 16 /*0x10*/:
          if (qualifiedItemId == "(F)JojaCatalogue")
          {
            if (!Game1.player.mailReceived.Contains("JojaThriveTerms"))
            {
              Game1.player.mailReceived.Add("JojaThriveTerms");
              Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString("Strings\\1_6_Strings:JojaCatalogueDescriptionTerms"))
              {
                whichBG = 4
              };
            }
            else
              Utility.TryOpenShopMenu("JojaFurnitureCatalogue", location);
            return true;
          }
          break;
        case 17:
          switch (qualifiedItemId[3])
          {
            case 'R':
              if (qualifiedItemId == "(F)RetroCatalogue")
              {
                Utility.TryOpenShopMenu("RetroFurnitureCatalogue", location);
                break;
              }
              break;
            case 'T':
              if (qualifiedItemId == "(F)TrashCatalogue")
              {
                Utility.TryOpenShopMenu("TrashFurnitureCatalogue", location);
                break;
              }
              break;
          }
          break;
        case 18:
          switch (qualifiedItemId[3])
          {
            case 'J':
              if (qualifiedItemId == "(F)JunimoCatalogue")
              {
                Utility.TryOpenShopMenu("JunimoFurnitureCatalogue", location);
                break;
              }
              break;
            case 'W':
              if (qualifiedItemId == "(F)WizardCatalogue")
              {
                if (!Game1.player.mailReceived.Contains("WizardCatalogue"))
                {
                  Game1.player.mailReceived.Add("WizardCatalogue");
                  Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(Game1.content.LoadString("Strings\\1_6_Strings:WizardCatalogueLetter"))
                  {
                    whichBG = 2
                  };
                }
                else
                  Utility.TryOpenShopMenu("WizardFurnitureCatalogue", location);
                return true;
              }
              break;
          }
          break;
      }
    }
    if (this.furniture_type.Value == 14 || this.furniture_type.Value == 16 /*0x10*/)
    {
      this.isOn.Value = !this.isOn.Value;
      this.initializeLightSource(this.tileLocation.Value);
      this.setFireplace(broadcast: true);
      return true;
    }
    if (this.GetSeatCapacity() <= 0)
      return this.clicked(who);
    who.BeginSitting((ISittable) this);
    return true;
  }

  public virtual void setFireplace(bool playSound = true, bool broadcast = false)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (this.isOn.Value)
    {
      if (this.lightSource == null)
        this.initializeLightSource(this.tileLocation.Value);
      if (this.lightSource != null && this.isOn.Value && !location.hasLightSource(this.lightSource.Id))
        location.sharedLights.AddLight(this.lightSource.Clone());
      if (playSound)
        location.localSound("fireball");
      AmbientLocationSounds.addSound(new Vector2(this.tileLocation.X, this.tileLocation.Y), 1);
    }
    else
    {
      if (playSound)
        location.localSound("fireball");
      base.performRemoveAction();
      AmbientLocationSounds.removeSound(new Vector2(this.tileLocation.X, this.tileLocation.Y));
    }
  }

  public virtual void AttemptRemoval(Action<Furniture> removal_action)
  {
    if (removal_action == null)
      return;
    removal_action(this);
  }

  public virtual bool canBeRemoved(Farmer who)
  {
    if (!this.AllowLocalRemoval)
      return false;
    GameLocation location = this.Location;
    if (location == null || this.HasSittingFarmers() || this.heldObject.Value != null)
      return false;
    Rectangle boundingBox = this.GetBoundingBox();
    if (this.isPassable())
    {
      for (int x = boundingBox.Left / 64 /*0x40*/; x < boundingBox.Right / 64 /*0x40*/; ++x)
      {
        for (int y = boundingBox.Top / 64 /*0x40*/; y < boundingBox.Bottom / 64 /*0x40*/; ++y)
        {
          Furniture furnitureAt = location.GetFurnitureAt(new Vector2((float) x, (float) y));
          if (furnitureAt != null && furnitureAt != this || location.objects.ContainsKey(new Vector2((float) x, (float) y)))
            return false;
        }
      }
    }
    return true;
  }

  public override bool clicked(Farmer who)
  {
    Game1.haltAfterCheck = false;
    if (this.furniture_type.Value == 11 && who.ActiveObject != null && this.heldObject.Value == null || this.heldObject.Value == null)
      return false;
    StardewValley.Object @object = this.heldObject.Value;
    this.heldObject.Value = (StardewValley.Object) null;
    if (who.addItemToInventoryBool((Item) @object))
    {
      @object.performRemoveAction();
      Game1.playSound("coin");
      return true;
    }
    this.heldObject.Value = @object;
    return false;
  }

  public virtual int GetSeatCapacity()
  {
    if (this.QualifiedItemId.Equals("(F)UprightPiano") || this.QualifiedItemId.Equals("(F)DarkPiano"))
      return 1;
    switch (this.furniture_type.Value)
    {
      case 0:
        return 1;
      case 1:
        return 2;
      case 2:
        return this.defaultBoundingBox.Width / 64 /*0x40*/ - 1;
      case 3:
        return 1;
      default:
        return 0;
    }
  }

  public virtual bool IsSeatHere(GameLocation location) => location.furniture.Contains(this);

  public virtual bool IsSittingHere(Farmer who)
  {
    return this.sittingFarmers.ContainsKey(who.UniqueMultiplayerID);
  }

  public virtual Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false)
  {
    int index;
    return this.sittingFarmers.TryGetValue(who.UniqueMultiplayerID, out index) ? new Vector2?(this.GetSeatPositions(ignore_offsets)[index]) : new Vector2?();
  }

  public virtual bool HasSittingFarmers() => this.sittingFarmers.Length > 0;

  public virtual void RemoveSittingFarmer(Farmer farmer)
  {
    this.sittingFarmers.Remove(farmer.UniqueMultiplayerID);
  }

  public virtual int GetSittingFarmerCount() => this.sittingFarmers.Length;

  public virtual Rectangle GetSeatBounds()
  {
    Rectangle boundingBox = this.GetBoundingBox();
    return new Rectangle(boundingBox.X / 64 /*0x40*/, boundingBox.Y / 64 /*0x40*/, boundingBox.Width / 64 /*0x40*/, boundingBox.Height / 64 /*0x40*/);
  }

  public virtual int GetSittingDirection()
  {
    if (this.Name.Contains("Stool"))
      return Game1.player.FacingDirection;
    if (this.QualifiedItemId.Equals("(F)UprightPiano") || this.QualifiedItemId.Equals("(F)DarkPiano"))
      return 0;
    switch (this.currentRotation.Value)
    {
      case 0:
        return 2;
      case 1:
        return 1;
      case 2:
        return 0;
      case 3:
        return 3;
      default:
        return 2;
    }
  }

  public virtual Vector2? AddSittingFarmer(Farmer who)
  {
    List<Vector2> seatPositions = this.GetSeatPositions(false);
    int num1 = -1;
    Vector2? nullable = new Vector2?();
    float num2 = 96f;
    Vector2 standingPosition = who.getStandingPosition();
    for (int index = 0; index < seatPositions.Count; ++index)
    {
      if (!this.sittingFarmers.Values.Contains<int>(index))
      {
        float num3 = ((seatPositions[index] + new Vector2(0.5f, 0.5f)) * 64f - standingPosition).Length();
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          nullable = new Vector2?(seatPositions[index]);
          num1 = index;
        }
      }
    }
    if (nullable.HasValue)
      this.sittingFarmers[who.UniqueMultiplayerID] = num1;
    return nullable;
  }

  public virtual List<Vector2> GetSeatPositions(bool ignore_offsets = false)
  {
    List<Vector2> seatPositions = new List<Vector2>();
    if (this.QualifiedItemId.Equals("(F)UprightPiano") || this.QualifiedItemId.Equals("(F)DarkPiano"))
      seatPositions.Add(this.TileLocation + new Vector2(1.5f, 0.0f));
    switch (this.furniture_type.Value)
    {
      case 0:
        seatPositions.Add(this.TileLocation);
        break;
      case 1:
        for (int x = 0; x < this.getTilesWide(); ++x)
        {
          for (int y = 0; y < this.getTilesHigh(); ++y)
            seatPositions.Add(this.TileLocation + new Vector2((float) x, (float) y));
        }
        break;
      case 2:
        int num = this.defaultBoundingBox.Width / 64 /*0x40*/ - 1;
        switch (this.currentRotation.Value)
        {
          case 0:
          case 2:
            seatPositions.Add(this.TileLocation + new Vector2(0.5f, 0.0f));
            for (int index = 1; index < num - 1; ++index)
              seatPositions.Add(this.TileLocation + new Vector2((float) index + 0.5f, 0.0f));
            seatPositions.Add(this.TileLocation + new Vector2((float) (num - 1) + 0.5f, 0.0f));
            break;
          case 1:
            for (int y = 0; y < num; ++y)
              seatPositions.Add(this.TileLocation + new Vector2(1f, (float) y));
            break;
          default:
            for (int y = 0; y < num; ++y)
              seatPositions.Add(this.TileLocation + new Vector2(0.0f, (float) y));
            break;
        }
        break;
      case 3:
        if (this.currentRotation.Value == 0 || this.currentRotation.Value == 2)
        {
          seatPositions.Add(this.TileLocation + new Vector2(0.5f, 0.0f));
          break;
        }
        if (this.currentRotation.Value == 1)
        {
          seatPositions.Add(this.TileLocation + new Vector2(1f, 0.0f));
          break;
        }
        seatPositions.Add(this.TileLocation + new Vector2(0.0f, 0.0f));
        break;
    }
    return seatPositions;
  }

  public bool timeToTurnOnLights()
  {
    if (this.Location == null)
      return false;
    return this.Location.IsRainingHere() || Game1.timeOfDay >= Game1.getTrulyDarkTime(this.Location) - 100;
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    this.sittingFarmers.Clear();
    if (this.Location.IsRainingHere())
      this.addLights();
    else if (!this.timeToTurnOnLights() || Game1.newDay)
      this.removeLights();
    else
      this.addLights();
    this.RemoveLightGlow();
    if (!Game1.IsMasterGame || Game1.season != Season.Winter || Game1.dayOfMonth != 25 || this.furniture_type.Value != 11 && this.furniture_type.Value != 5 || this.heldObject.Value == null)
      return;
    if (this.heldObject.Value.QualifiedItemId == "(O)223" && !Game1.player.mailReceived.Contains("CookiePresent_year" + Game1.year.ToString()))
    {
      this.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)MysteryBox");
      Game1.player.mailReceived.Add("CookiePresent_year" + Game1.year.ToString());
    }
    else
    {
      if (this.heldObject.Value.Category != -6 || Game1.player.mailReceived.Contains("MilkPresent_year" + Game1.year.ToString()))
        return;
      this.heldObject.Value = ItemRegistry.Create<StardewValley.Object>("(O)MysteryBox");
      Game1.player.mailReceived.Add("MilkPresent_year" + Game1.year.ToString());
    }
  }

  public virtual void AddLightGlow()
  {
    GameLocation location = this.Location;
    if (location == null || this.lightGlowPosition.HasValue)
      return;
    Vector2 vector2 = new Vector2((float) (this.boundingBox.X + 32 /*0x20*/), (float) (this.boundingBox.Y + 64 /*0x40*/));
    if (location.lightGlows.Contains(vector2))
      return;
    this.lightGlowPosition = new Vector2?(vector2);
    location.lightGlows.Add(vector2);
  }

  public virtual void RemoveLightGlow()
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (this.lightGlowPosition.HasValue && location.lightGlows.Contains(this.lightGlowPosition.Value))
      location.lightGlows.Remove(this.lightGlowPosition.Value);
    location.lightGlowLayerCache.Clear();
    this.lightGlowPosition = new Vector2?();
  }

  /// <inheritdoc />
  public override void actionOnPlayerEntry()
  {
    base.actionOnPlayerEntry();
    this.actionOnPlayerEntryOrPlacement(this.Location, false);
    if (this.Location == null || !this.QualifiedItemId.Equals("(F)BirdHouse") || !this.Location.isOutdoors.Value || Game1.isRaining || Game1.timeOfDay >= Game1.getStartingToGetDarkTime(this.Location))
      return;
    Random daySaveRandom = Utility.CreateDaySaveRandom((double) this.TileLocation.X * 74797.0, (double) this.TileLocation.Y * 77.0, (double) (Game1.timeOfDay * 99));
    int num1 = (int) Game1.stats.Get("childrenTurnedToDoves");
    if (daySaveRandom.NextDouble() >= 0.06)
      return;
    this.Location.instantiateCrittersList();
    int startingIndex = Game1.season == Season.Fall ? 45 : 25;
    int num2 = 0;
    if (Game1.random.NextBool() && Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal"))
      startingIndex = Game1.season == Season.Fall ? 135 : 125;
    if (startingIndex == 25 && Game1.random.NextDouble() < 0.05)
      startingIndex = 165;
    if (daySaveRandom.NextDouble() < (double) num1 * 0.08)
    {
      startingIndex = 175;
      num2 = 12;
    }
    this.Location.critters.Add((Critter) new Birdie(this.TileLocation * 64f + new Vector2(32f, (float) (64 /*0x40*/ + Game1.random.Next(3) * 4 + num2)), -160f, startingIndex, true));
  }

  /// <summary>Handle the player entering the location containing the object, or the furniture being placed.</summary>
  /// <param name="environment">The location containing the object.</param>
  /// <param name="dropDown">Whether the item was just placed (instead of the player entering the location with it already placed).</param>
  public virtual void actionOnPlayerEntryOrPlacement(GameLocation environment, bool dropDown)
  {
    if (this.Location == null)
      this.Location = environment;
    this.RemoveLightGlow();
    this.removeLights();
    if (this.furniture_type.Value == 14 || this.furniture_type.Value == 16 /*0x10*/)
      this.setFireplace(false);
    if (this.timeToTurnOnLights())
    {
      this.addLights();
      if (this.heldObject.Value is Furniture furniture)
        furniture.addLights();
    }
    if (!(this.QualifiedItemId == "(F)1971") || dropDown)
      return;
    environment.instantiateCrittersList();
    environment.addCritter((Critter) new Butterfly(environment, environment.getRandomTile()).setStayInbounds(true));
    while (Game1.random.NextBool())
      environment.addCritter((Critter) new Butterfly(environment, environment.getRandomTile()).setStayInbounds(true));
  }

  /// <inheritdoc />
  public override bool performObjectDropInAction(
    Item dropInItem,
    bool probe,
    Farmer who,
    bool returnFalseIfItemConsumed = false)
  {
    GameLocation location = this.Location;
    if (location == null || !(dropInItem is StardewValley.Object @object) || !this.IsTable() || this.heldObject.Value != null || @object.bigCraftable.Value || @object is Wallpaper || @object is Furniture furniture && (furniture.getTilesWide() != 1 || furniture.getTilesHigh() != 1))
      return false;
    if (!probe)
    {
      this.heldObject.Value = (StardewValley.Object) @object.getOne();
      this.heldObject.Value.Location = this.Location;
      this.heldObject.Value.TileLocation = this.tileLocation.Value;
      this.heldObject.Value.boundingBox.X = this.boundingBox.X;
      this.heldObject.Value.boundingBox.Y = this.boundingBox.Y;
      this.heldObject.Value.performDropDownAction(who);
      location.playSound("woodyStep");
      if (who != null)
      {
        who.reduceActiveItemByOne();
        if (returnFalseIfItemConsumed)
          return false;
      }
    }
    return true;
  }

  /// <summary>Auto-generate a default light source ID for this furniture when placed.</summary>
  protected virtual string GenerateLightSourceId()
  {
    return this.GenerateLightSourceId(this.tileLocation.Value);
  }

  private bool isLampStyleLightSource()
  {
    return this.furniture_type.Value == 7 || this.furniture_type.Value == 17 || this.QualifiedItemId == "(F)1369";
  }

  public virtual void addLights()
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (this.heldObject.Value is Furniture furniture)
    {
      this.heldObject.Value.Location = this.Location;
      furniture.addLights();
    }
    if (this.isLampStyleLightSource())
    {
      this.sourceRect.Value = this.defaultSourceRect.Value;
      this.sourceIndexOffset.Value = 1;
      if (this.lightSource != null)
        return;
      this.lightSource = new LightSource(this.GenerateLightSourceId(), 4, new Vector2((float) (this.boundingBox.X + 32 /*0x20*/), (float) (this.boundingBox.Y + (this.furniture_type.Value == 7 ? -64 : 64 /*0x40*/))), this.furniture_type.Value == 7 ? 2f : 1f, this.QualifiedItemId == "(F)1369" ? Color.RoyalBlue * 0.7f : Color.Black, onlyLocation: location.NameOrUniqueName);
      location.sharedLights.AddLight(this.lightSource.Clone());
    }
    else if (this.QualifiedItemId == "(F)1440")
    {
      this.lightSource = new LightSource(this.GenerateLightSourceId(), 4, new Vector2((float) (this.boundingBox.X + 96 /*0x60*/), (float) this.boundingBox.Y - 32f), 1.5f, Color.Black, onlyLocation: location.NameOrUniqueName);
      location.sharedLights.AddLight(this.lightSource.Clone());
    }
    else if (this.furniture_type.Value == 13)
    {
      this.sourceRect.Value = this.defaultSourceRect.Value;
      this.sourceIndexOffset.Value = 1;
      this.RemoveLightGlow();
    }
    else
    {
      if (!(this is FishTankFurniture) || this.lightSource != null)
        return;
      string lightSourceId = this.GenerateLightSourceId();
      Vector2 position = new Vector2((float) ((double) this.tileLocation.X * 64.0 + 32.0 + 2.0), (float) ((double) this.tileLocation.Y * 64.0 + 12.0));
      for (int index = 0; index < this.getTilesWide(); ++index)
      {
        this.lightSource = new LightSource($"{lightSourceId}_tile{index}", 8, position, 2f, Color.Black, onlyLocation: location.NameOrUniqueName);
        location.sharedLights.AddLight(this.lightSource.Clone());
        position.X += 64f;
      }
    }
  }

  public virtual void removeLights()
  {
    GameLocation location = this.Location;
    if (this.heldObject.Value is Furniture furniture)
      furniture.removeLights();
    if (this.isLampStyleLightSource() || this.QualifiedItemId == "(F)1440")
    {
      this.sourceRect.Value = this.defaultSourceRect.Value;
      this.sourceIndexOffset.Value = 0;
      location?.removeLightSource(this.GenerateLightSourceId());
      this.lightSource = (LightSource) null;
    }
    else if (this.furniture_type.Value == 13)
    {
      if (location != null && location.IsRainingHere())
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 1;
      }
      else
      {
        this.sourceRect.Value = this.defaultSourceRect.Value;
        this.sourceIndexOffset.Value = 0;
        this.AddLightGlow();
      }
    }
    else
    {
      if (!(this is FishTankFurniture))
        return;
      string lightSourceId = this.GenerateLightSourceId();
      for (int index = 0; index < this.getTilesWide(); ++index)
      {
        if (location != null)
          location.removeLightSource($"{lightSourceId}_tile{index}");
      }
      this.lightSource = (LightSource) null;
    }
  }

  /// <inheritdoc />
  public override bool minutesElapsed(int minutes)
  {
    if (this.Location == null)
      return false;
    if (this.timeToTurnOnLights())
      this.addLights();
    else
      this.removeLights();
    return false;
  }

  public override void performRemoveAction()
  {
    this.removeLights();
    if (this.Location == null)
      return;
    if (this.furniture_type.Value == 14 || this.furniture_type.Value == 16 /*0x10*/)
    {
      this.isOn.Value = false;
      this.setFireplace(false);
    }
    this.RemoveLightGlow();
    base.performRemoveAction();
    if (this.furniture_type.Value == 14 || this.furniture_type.Value == 16 /*0x10*/)
      this.lightSource = (LightSource) null;
    if (this.QualifiedItemId == "(F)1309" && Game1.getMusicTrackName().Equals("sam_acoustic1"))
      Game1.changeMusicTrack("none", true);
    this.sittingFarmers.Clear();
  }

  public virtual void rotate()
  {
    if (this.rotations.Value < 2)
      return;
    this.currentRotation.Value += this.rotations.Value == 4 ? 1 : 2;
    this.currentRotation.Value %= 4;
    this.updateRotation();
  }

  public virtual void updateRotation()
  {
    this.flipped.Value = false;
    if (this.currentRotation.Value > 0)
    {
      Point point1;
      switch (this.furniture_type.Value)
      {
        case 2:
          point1 = new Point(-1, 1);
          break;
        case 3:
          point1 = new Point(-1, 1);
          break;
        case 5:
          point1 = new Point(-1, 0);
          break;
        default:
          point1 = Point.Zero;
          break;
      }
      bool flag1 = (this.IsTable() || this.furniture_type.Value == 12 || this.QualifiedItemId == "(F)724" || this.QualifiedItemId == "(F)727") && !this.name.Contains("End Table") && !this.name.Contains("EndTable");
      bool flag2 = this.defaultBoundingBox.Width != this.defaultBoundingBox.Height;
      if (flag1 && this.currentRotation.Value == 2)
        this.currentRotation.Value = 1;
      if (flag2)
      {
        int height = this.boundingBox.Height;
        switch (this.currentRotation.Value)
        {
          case 0:
          case 2:
            this.boundingBox.Height = this.defaultBoundingBox.Height;
            this.boundingBox.Width = this.defaultBoundingBox.Width;
            break;
          case 1:
          case 3:
            this.boundingBox.Height = this.boundingBox.Width + point1.X * 64 /*0x40*/;
            this.boundingBox.Width = height + point1.Y * 64 /*0x40*/;
            break;
        }
      }
      Point point2 = this.furniture_type.Value == 12 ? new Point(1, -1) : Point.Zero;
      if (flag2)
      {
        switch (this.currentRotation.Value)
        {
          case 0:
            this.sourceRect.Value = this.defaultSourceRect.Value;
            break;
          case 1:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 /*0x10*/ + point1.Y * 16 /*0x10*/ + point2.X * 16 /*0x10*/, this.defaultSourceRect.Width + 16 /*0x10*/ + point1.X * 16 /*0x10*/ + point2.Y * 16 /*0x10*/);
            break;
          case 2:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width + this.defaultSourceRect.Height - 16 /*0x10*/ + point1.Y * 16 /*0x10*/ + point2.X * 16 /*0x10*/, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
            break;
          case 3:
            this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Height - 16 /*0x10*/ + point1.Y * 16 /*0x10*/ + point2.X * 16 /*0x10*/, this.defaultSourceRect.Width + 16 /*0x10*/ + point1.X * 16 /*0x10*/ + point2.Y * 16 /*0x10*/);
            this.flipped.Value = true;
            break;
        }
      }
      else
      {
        this.flipped.Value = this.currentRotation.Value == 3;
        if (this.rotations.Value == 2)
          this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + (this.currentRotation.Value == 2 ? 1 : 0) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
        else
          this.sourceRect.Value = new Rectangle(this.defaultSourceRect.X + (this.currentRotation.Value == 3 ? 1 : this.currentRotation.Value) * this.defaultSourceRect.Width, this.defaultSourceRect.Y, this.defaultSourceRect.Width, this.defaultSourceRect.Height);
      }
      if (flag1 && this.currentRotation.Value == 1)
        this.currentRotation.Value = 2;
    }
    else
    {
      this.sourceRect.Value = this.defaultSourceRect.Value;
      this.boundingBox.Value = this.defaultBoundingBox.Value;
    }
    this.updateDrawPosition();
  }

  public virtual bool isGroundFurniture()
  {
    return this.furniture_type.Value != 13 && this.furniture_type.Value != 6 && this.furniture_type.Value != 17 && this.furniture_type.Value != 13;
  }

  /// <inheritdoc />
  public override bool canBeGivenAsGift() => false;

  public static Furniture GetFurnitureInstance(string itemId, Vector2? position = null)
  {
    if (!position.HasValue)
      position = new Vector2?(Vector2.Zero);
    if (itemId == "1466" || itemId == "1468" || itemId == "1680" || itemId == "2326" || itemId == "RetroTV")
      return (Furniture) new TV(itemId, position.Value);
    string str = ArgUtility.Get(Furniture.getData(itemId), 1);
    bool? nullable1;
    switch (str)
    {
      case "fishtank":
        return (Furniture) new FishTankFurniture(itemId, position.Value);
      case "dresser":
        return (Furniture) new StorageFurniture(itemId, position.Value);
      case "randomized_plant":
        return (Furniture) new RandomizedPlantFurniture(itemId, position.Value);
      case null:
        nullable1 = new bool?();
        break;
      default:
        nullable1 = new bool?(str.StartsWith("bed"));
        break;
    }
    bool? nullable2 = nullable1;
    return nullable2.HasValue && nullable2.GetValueOrDefault() ? (Furniture) new BedFurniture(itemId, position.Value) : new Furniture(itemId, position.Value);
  }

  public virtual bool IsCloseEnoughToFarmer(Farmer f, int? override_tile_x = null, int? override_tile_y = null)
  {
    Rectangle rectangle = new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, this.getTilesWide() * 64 /*0x40*/, this.getTilesHigh() * 64 /*0x40*/);
    if (override_tile_x.HasValue)
      rectangle.X = override_tile_x.Value * 64 /*0x40*/;
    if (override_tile_y.HasValue)
      rectangle.Y = override_tile_y.Value * 64 /*0x40*/;
    rectangle.Inflate(96 /*0x60*/, 96 /*0x60*/);
    return rectangle.Contains(Game1.player.StandingPixel);
  }

  public virtual int GetModifiedWallTilePosition(GameLocation l, int tile_x, int tile_y)
  {
    if (this.isGroundFurniture() || l == null || !(l is DecoratableLocation decoratableLocation))
      return tile_y;
    int wallTopY = decoratableLocation.GetWallTopY(tile_x, tile_y);
    return wallTopY != -1 ? wallTopY : tile_y;
  }

  public override bool canBePlacedHere(
    GameLocation l,
    Vector2 tile,
    CollisionMask collisionMask = CollisionMask.All,
    bool showError = false)
  {
    if (!l.CanPlaceThisFurnitureHere(this))
      return false;
    if (!this.isGroundFurniture())
      tile.Y = (float) this.GetModifiedWallTilePosition(l, (int) tile.X, (int) tile.Y);
    CollisionMask ignorePassables = CollisionMask.Buildings | CollisionMask.Flooring | CollisionMask.TerrainFeatures;
    bool itemIsPassable = this.isPassable();
    if (itemIsPassable)
      ignorePassables |= CollisionMask.Characters | CollisionMask.Farmers;
    collisionMask &= CollisionMask.Buildings | CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring | CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
    int tilesWide = this.getTilesWide();
    int tilesHigh = this.getTilesHigh();
    for (int index1 = 0; index1 < tilesWide; ++index1)
    {
      for (int index2 = 0; index2 < tilesHigh; ++index2)
      {
        Vector2 vector2_1 = new Vector2(tile.X + (float) index1, tile.Y + (float) index2);
        Vector2 vector2_2 = new Vector2(vector2_1.X + 0.5f, vector2_1.Y + 0.5f) * 64f;
        if (!l.isTilePlaceable(vector2_1, itemIsPassable))
          return false;
        foreach (Furniture furniture in l.furniture)
        {
          if (furniture.furniture_type.Value == 11 && furniture.GetBoundingBox().Contains((int) vector2_2.X, (int) vector2_2.Y) && furniture.heldObject.Value == null && tilesWide == 1 && tilesHigh == 1)
            return true;
          if ((furniture.furniture_type.Value != 12 || this.furniture_type.Value == 12) && furniture.GetBoundingBox().Contains((int) vector2_2.X, (int) vector2_2.Y) && !furniture.AllowPlacementOnThisTile((int) tile.X + index1, (int) tile.Y + index2))
            return false;
        }
        StardewValley.Object @object;
        if (l.objects.TryGetValue(vector2_1, out @object) && (!@object.isPassable() || !this.isPassable()))
          return false;
        if (!this.isGroundFurniture())
        {
          if (l.IsTileOccupiedBy(vector2_1, collisionMask, ignorePassables))
            return false;
        }
        else if (this.furniture_type.Value == 15 && index2 == 0)
        {
          if (l.IsTileOccupiedBy(vector2_1, collisionMask, ignorePassables))
            return false;
        }
        else if (l.IsTileBlockedBy(vector2_1, collisionMask, ignorePassables) || l.terrainFeatures.GetValueOrDefault(vector2_1) is HoeDirt valueOrDefault && valueOrDefault.crop != null)
          return false;
      }
    }
    return this.GetAdditionalFurniturePlacementStatus(l, (int) tile.X * 64 /*0x40*/, (int) tile.Y * 64 /*0x40*/) == 0;
  }

  public virtual void updateDrawPosition()
  {
    this.drawPosition.Value = new Vector2((float) this.boundingBox.X, (float) (this.boundingBox.Y - (this.sourceRect.Height * 4 - this.boundingBox.Height)));
  }

  public virtual int getTilesWide() => this.boundingBox.Width / 64 /*0x40*/;

  public virtual int getTilesHigh() => this.boundingBox.Height / 64 /*0x40*/;

  /// <inheritdoc />
  public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
  {
    if (!this.isGroundFurniture())
      y = this.GetModifiedWallTilePosition(location, x / 64 /*0x40*/, y / 64 /*0x40*/) * 64 /*0x40*/;
    if (this.GetAdditionalFurniturePlacementStatus(location, x, y, who) != 0)
      return false;
    Vector2 vector2 = new Vector2((float) (x / 64 /*0x40*/), (float) (y / 64 /*0x40*/));
    if (this.TileLocation != vector2)
      this.TileLocation = vector2;
    else
      this.RecalculateBoundingBox();
    foreach (Furniture furniture in location.furniture)
    {
      if (furniture.furniture_type.Value == 11 && furniture.heldObject.Value == null && furniture.GetBoundingBox().Intersects(this.boundingBox.Value))
      {
        furniture.performObjectDropInAction((Item) this, false, who ?? Game1.player, false);
        return true;
      }
    }
    return base.placementAction(location, x, y, who);
  }

  /// <summary>Get the reason the furniture can't be placed at a given position, if applicable.</summary>
  /// <param name="location">The location in which the furniture is being placed.</param>
  /// <param name="x">The X pixel position at which the furniture is being placed.</param>
  /// <param name="y">The Y pixel position at which the furniture is being placed.</param>
  /// <param name="who">The player placing the furniture, if applicable.</param>
  /// <returns>
  ///   Returns one of these values:
  ///   <list type="bullet">
  ///     <item><description>0: valid placement.</description></item>
  ///     <item><description>1: the object is a wall placed object but isn't being placed on a wall.</description></item>
  ///     <item><description>2: the object can't be placed here due to the tile being marked as not furnishable.</description></item>
  ///     <item><description>3: the object isn't a wall placed object, but is trying to be placed on a wall.</description></item>
  ///     <item><description>4: the current location isn't decorable.</description></item>
  ///     <item><description>-1: general fail condition.</description></item>
  ///   </list>
  /// </returns>
  public virtual int GetAdditionalFurniturePlacementStatus(
    GameLocation location,
    int x,
    int y,
    Farmer who = null)
  {
    if (!location.CanPlaceThisFurnitureHere(this))
      return 4;
    Point point = new Point(x / 64 /*0x40*/, y / 64 /*0x40*/);
    this.tileLocation.Value = new Vector2((float) point.X, (float) point.Y);
    bool flag1 = false;
    if (this.furniture_type.Value == 6 || this.furniture_type.Value == 17 || this.furniture_type.Value == 13 || this.QualifiedItemId == "(F)1293")
    {
      int num = this.QualifiedItemId == "(F)1293" ? 3 : 0;
      bool flag2 = false;
      if (location is DecoratableLocation decoratableLocation)
      {
        if ((this.furniture_type.Value == 6 || this.furniture_type.Value == 17 || this.furniture_type.Value == 13 || num != 0) && decoratableLocation.isTileOnWall(point.X, point.Y - num) && decoratableLocation.GetWallTopY(point.X, point.Y - num) + num == point.Y)
          flag2 = true;
        else if (!this.isGroundFurniture() && decoratableLocation.isTileOnWall(point.X, point.Y - 1) && decoratableLocation.GetWallTopY(point.X, point.Y) + 1 == point.Y)
          flag2 = true;
      }
      if (!flag2)
        return 1;
      flag1 = true;
    }
    int num1 = this.getTilesHigh();
    if (this.furniture_type.Value == 6 && num1 > 2)
      num1 = 2;
    for (int x1 = point.X; x1 < point.X + this.getTilesWide(); ++x1)
    {
      for (int y1 = point.Y; y1 < point.Y + num1; ++y1)
      {
        if (location.doesTileHaveProperty(x1, y1, "NoFurniture", "Back") != null)
          return 2;
        if (!flag1 && location is DecoratableLocation decoratableLocation && decoratableLocation.isTileOnWall(x1, y1))
        {
          if (!(this is BedFurniture) || y1 != point.Y)
            return 3;
        }
        else
        {
          int tileIndexAt = location.getTileIndexAt(x1, y1, "Buildings");
          if (tileIndexAt != -1 && (!(location is IslandFarmHouse) || tileIndexAt < 192 /*0xC0*/ || tileIndexAt > 194 || !(location.getTileSheetIDAt(x1, y1, "Buildings") == "untitled tile sheet")))
            return -1;
        }
      }
    }
    return 0;
  }

  public override bool isPassable() => this.furniture_type.Value == 12 || base.isPassable();

  public override bool isPlaceable() => true;

  public virtual bool AllowPlacementOnThisTile(int tile_x, int tile_y) => false;

  /// <inheritdoc />
  public override Rectangle GetBoundingBoxAt(int x, int y)
  {
    return this.isTemporarilyInvisible ? Rectangle.Empty : this.boundingBox.Value;
  }

  protected static Rectangle getDefaultSourceRectForType(
    ParsedItemData itemData,
    int type,
    Texture2D texture = null)
  {
    int spriteWidth;
    int spriteHeight;
    switch (type)
    {
      case 0:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
      case 1:
        spriteWidth = 2;
        spriteHeight = 2;
        break;
      case 2:
        spriteWidth = 3;
        spriteHeight = 2;
        break;
      case 3:
        spriteWidth = 2;
        spriteHeight = 2;
        break;
      case 4:
        spriteWidth = 2;
        spriteHeight = 2;
        break;
      case 5:
        spriteWidth = 5;
        spriteHeight = 3;
        break;
      case 6:
        spriteWidth = 2;
        spriteHeight = 2;
        break;
      case 7:
        spriteWidth = 1;
        spriteHeight = 3;
        break;
      case 8:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
      case 10:
        spriteWidth = 2;
        spriteHeight = 3;
        break;
      case 11:
        spriteWidth = 2;
        spriteHeight = 3;
        break;
      case 12:
        spriteWidth = 3;
        spriteHeight = 2;
        break;
      case 13:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
      case 14:
        spriteWidth = 2;
        spriteHeight = 5;
        break;
      case 16 /*0x10*/:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
      case 17:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
      default:
        spriteWidth = 1;
        spriteHeight = 2;
        break;
    }
    return Furniture.getDefaultSourceRect(itemData, spriteWidth, spriteHeight, texture);
  }

  protected static Rectangle getDefaultSourceRect(
    ParsedItemData itemData,
    int spriteWidth,
    int spriteHeight,
    Texture2D texture = null)
  {
    texture = texture ?? itemData.GetTexture();
    return new Rectangle(itemData.SpriteIndex * 16 /*0x10*/ % texture.Width, itemData.SpriteIndex * 16 /*0x10*/ / texture.Width * 16 /*0x10*/, spriteWidth * 16 /*0x10*/, spriteHeight * 16 /*0x10*/);
  }

  protected virtual Rectangle getDefaultBoundingBoxForType(int type)
  {
    int num1;
    int num2;
    switch (type)
    {
      case 0:
        num1 = 1;
        num2 = 1;
        break;
      case 1:
        num1 = 2;
        num2 = 1;
        break;
      case 2:
        num1 = 3;
        num2 = 1;
        break;
      case 3:
        num1 = 2;
        num2 = 1;
        break;
      case 4:
        num1 = 2;
        num2 = 1;
        break;
      case 5:
        num1 = 5;
        num2 = 2;
        break;
      case 6:
        num1 = 2;
        num2 = 2;
        break;
      case 7:
        num1 = 1;
        num2 = 1;
        break;
      case 8:
        num1 = 1;
        num2 = 1;
        break;
      case 10:
        num1 = 2;
        num2 = 1;
        break;
      case 11:
        num1 = 2;
        num2 = 2;
        break;
      case 12:
        num1 = 3;
        num2 = 2;
        break;
      case 13:
        num1 = 1;
        num2 = 2;
        break;
      case 14:
        num1 = 2;
        num2 = 1;
        break;
      case 16 /*0x10*/:
        num1 = 1;
        num2 = 1;
        break;
      case 17:
        num1 = 1;
        num2 = 2;
        break;
      default:
        num1 = 1;
        num2 = 1;
        break;
    }
    return new Rectangle((int) this.tileLocation.X * 64 /*0x40*/, (int) this.tileLocation.Y * 64 /*0x40*/, num1 * 64 /*0x40*/, num2 * 64 /*0x40*/);
  }

  public static int getTypeNumberFromName(string typeName)
  {
    if (typeName.StartsWithIgnoreCase("bed"))
      return 15;
    string lower = typeName.ToLower();
    if (lower != null)
    {
      switch (lower.Length)
      {
        case 3:
          if (lower == "rug")
            return 12;
          break;
        case 4:
          if (lower == "lamp")
            return 7;
          break;
        case 5:
          switch (lower[2])
          {
            case 'a':
              if (lower == "chair")
                return 0;
              break;
            case 'b':
              if (lower == "table")
                return 11;
              break;
            case 'c':
              if (lower == "decor")
                return 8;
              break;
            case 'n':
              if (lower == "bench")
                return 1;
              break;
            case 'r':
              if (lower == "torch")
                return 16 /*0x10*/;
              break;
            case 'u':
              if (lower == "couch")
                return 2;
              break;
          }
          break;
        case 6:
          switch (lower[0])
          {
            case 's':
              if (lower == "sconce")
                return 17;
              break;
            case 'w':
              if (lower == "window")
                return 13;
              break;
          }
          break;
        case 7:
          if (lower == "dresser")
            return 4;
          break;
        case 8:
          switch (lower[0])
          {
            case 'a':
              if (lower == "armchair")
                return 3;
              break;
            case 'b':
              if (lower == "bookcase")
                return 10;
              break;
            case 'p':
              if (lower == "painting")
                return 6;
              break;
          }
          break;
        case 9:
          if (lower == "fireplace")
            return 14;
          break;
        case 10:
          if (lower == "long table")
            return 5;
          break;
      }
    }
    return 9;
  }

  /// <inheritdoc />
  public override int salePrice(bool ignoreProfitMargins = false) => this.price.Value;

  public override int maximumStackSize() => 1;

  /// <inheritdoc />
  public override string Name => this.name;

  protected virtual float getScaleSize()
  {
    int num1 = this.defaultSourceRect.Width / 16 /*0x10*/;
    int num2 = this.defaultSourceRect.Height / 16 /*0x10*/;
    if (num1 >= 7)
      return 0.5f;
    if (num1 >= 6)
      return 0.66f;
    if (num1 >= 5)
      return 0.75f;
    if (num2 >= 5)
      return 0.8f;
    if (num2 >= 3)
      return 1f;
    if (num1 <= 2)
      return 2f;
    return num1 <= 4 ? 1f : 0.1f;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.Location == null)
      return;
    if (Game1.IsMasterGame && this.sittingFarmers.Length > 0)
    {
      List<long> longList = (List<long>) null;
      foreach (long key in this.sittingFarmers.Keys)
      {
        if (!Game1.player.team.playerIsOnline(key))
        {
          if (longList == null)
            longList = new List<long>();
          longList.Add(key);
        }
      }
      if (longList != null)
      {
        foreach (long key in longList)
          this.sittingFarmers.Remove(key);
      }
    }
    TimeSpan elapsedGameTime;
    if (this.shakeTimer > 0)
    {
      int shakeTimer = this.shakeTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.shakeTimer = shakeTimer - milliseconds;
    }
    if (!this.IsOn || this.SpecialVariable != 388859)
      return;
    int noteBlockSoundTime = this.lastNoteBlockSoundTime;
    elapsedGameTime = time.ElapsedGameTime;
    int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
    this.lastNoteBlockSoundTime = noteBlockSoundTime + totalMilliseconds;
    if (this.lastNoteBlockSoundTime <= 500)
      return;
    this.lastNoteBlockSoundTime = 0;
    this.addCauldronBubbles();
  }

  private void addCauldronBubbles(float speed = -0.5f)
  {
    this.Location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), this.TileLocation * 64f + new Vector2(41.6f, -21f) + new Vector2((float) Game1.random.Next(-12, 21), (float) Game1.random.Next(16 /*0x10*/)), false, 1f / 500f, Color.Lime)
    {
      alphaFade = (float) (1.0 / 1000.0 - (double) speed / 300.0),
      alpha = 0.75f,
      motion = new Vector2(0.0f, speed),
      acceleration = new Vector2(0.0f, 0.0f),
      interval = 99999f,
      layerDepth = (float) (this.boundingBox.Bottom - 3 - Game1.random.Next(5)) / 10000f,
      scale = 3f,
      scaleChange = 0.01f,
      rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0)
    });
  }

  public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
  {
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), location + new Vector2(32f, 32f), new Rectangle?(dataOrErrorItem.GetSourceRect()), color * transparency, 0.0f, new Vector2((float) (sourceRect.Width / 2), (float) (sourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize, SpriteEffects.None, layerDepth);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    Rectangle sourceRect = this.sourceRect.Value;
    sourceRect.X += sourceRect.Width * this.sourceIndexOffset.Value;
    ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Texture2D texture1 = dataOrErrorItem1.GetTexture();
    string textureName = dataOrErrorItem1.TextureName;
    if (dataOrErrorItem1.IsErrorItem)
      sourceRect = dataOrErrorItem1.GetSourceRect();
    if (Furniture._frontTextureName == null)
      Furniture._frontTextureName = new Dictionary<string, string>();
    if (Furniture.isDrawingLocationFurniture)
    {
      string assetName;
      if (!Furniture._frontTextureName.TryGetValue(textureName, out assetName))
      {
        assetName = textureName + "Front";
        Furniture._frontTextureName[textureName] = assetName;
      }
      Texture2D texture2 = (Texture2D) null;
      if (this.HasSittingFarmers() || this.SpecialVariable == 388859)
      {
        try
        {
          texture2 = Game1.content.Load<Texture2D>(assetName);
        }
        catch
        {
          texture2 = (Texture2D) null;
        }
      }
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.drawPosition.Value + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero));
      SpriteEffects effects = this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
      Color color = Color.White * alpha;
      if (this.HasSittingFarmers())
      {
        spriteBatch.Draw(texture1, local, new Rectangle?(sourceRect), color, 0.0f, Vector2.Zero, 4f, effects, (float) (this.boundingBox.Value.Top + 16 /*0x10*/) / 10000f);
        if (texture2 != null && sourceRect.Right <= texture2.Width && sourceRect.Bottom <= texture2.Height)
          spriteBatch.Draw(texture2, local, new Rectangle?(sourceRect), color, 0.0f, Vector2.Zero, 4f, effects, (float) (this.boundingBox.Value.Bottom - 8) / 10000f);
      }
      else
      {
        spriteBatch.Draw(texture1, local, new Rectangle?(sourceRect), color, 0.0f, Vector2.Zero, 4f, effects, this.furniture_type.Value == 12 ? (float) (1.9999999434361371E-09 + (double) this.tileLocation.Y / 100000.0) : (float) (this.boundingBox.Value.Bottom - (this.furniture_type.Value == 6 || this.furniture_type.Value == 17 || this.furniture_type.Value == 13 ? 48 /*0x30*/ : 8)) / 10000f);
        if (this.SpecialVariable == 388859 && texture2 != null && sourceRect.Right <= texture2.Width && sourceRect.Bottom <= texture2.Height)
          spriteBatch.Draw(texture2, local, new Rectangle?(sourceRect), color, 0.0f, Vector2.Zero, 4f, effects, (float) (this.boundingBox.Value.Bottom - 2) / 10000f);
      }
    }
    else
      spriteBatch.Draw(texture1, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/ + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 /*0x40*/ - (sourceRect.Height * 4 - this.boundingBox.Height) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)))), new Rectangle?(sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.furniture_type.Value == 12 ? (float) (1.9999999434361371E-09 + (double) this.tileLocation.Y / 100000.0) : (float) (this.boundingBox.Value.Bottom - (this.furniture_type.Value == 6 || this.furniture_type.Value == 17 || this.furniture_type.Value == 13 ? 48 /*0x30*/ : 8)) / 10000f);
    if (this.heldObject.Value != null)
    {
      if (this.heldObject.Value is Furniture furniture)
      {
        furniture.drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 /*0x20*/), (float) (this.boundingBox.Center.Y - furniture.sourceRect.Height * 4 - (this.drawHeldObjectLow.Value ? -16 : 16 /*0x10*/)))), (float) (this.boundingBox.Bottom - 7) / 10000f, alpha);
      }
      else
      {
        ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem(this.heldObject.Value.QualifiedItemId);
        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 /*0x20*/), (float) (this.boundingBox.Center.Y - (this.drawHeldObjectLow.Value ? 32 /*0x20*/ : 85)))) + new Vector2(32f, 53f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float) this.boundingBox.Bottom / 10000f);
        if (this.heldObject.Value is ColoredObject)
          this.heldObject.Value.drawInMenu(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 /*0x20*/), (float) (this.boundingBox.Center.Y - (this.drawHeldObjectLow.Value ? 32 /*0x20*/ : 85)))), 1f, 1f, (float) (this.boundingBox.Bottom + 1) / 10000f, StackDrawType.Hide, Color.White, false);
        else
          spriteBatch.Draw(dataOrErrorItem2.GetTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 /*0x20*/), (float) (this.boundingBox.Center.Y - (this.drawHeldObjectLow.Value ? 32 /*0x20*/ : 85)))), new Rectangle?(dataOrErrorItem2.GetSourceRect()), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (this.boundingBox.Bottom + 1) / 10000f);
      }
    }
    if (this.isOn.Value && this.furniture_type.Value == 14)
    {
      Rectangle boundingBoxAt = this.GetBoundingBoxAt(x, y);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 12), (float) (this.boundingBox.Center.Y - 64 /*0x40*/))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (boundingBoxAt.Bottom - 2) / 10000f);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 32 /*0x20*/ - 4), (float) (this.boundingBox.Center.Y - 64 /*0x40*/))), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 2047 /*0x07FF*/) + (double) (y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (boundingBoxAt.Bottom - 1) / 10000f);
    }
    else if (this.isOn.Value && this.furniture_type.Value == 16 /*0x10*/)
    {
      Rectangle boundingBoxAt = this.GetBoundingBoxAt(x, y);
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (this.boundingBox.Center.X - 20), (float) this.boundingBox.Center.Y - 105.6f)), new Rectangle?(new Rectangle(276 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (x * 3047) + (double) (y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) (boundingBoxAt.Bottom - 2) / 10000f);
    }
    if (!Game1.debugMode)
      return;
    spriteBatch.DrawString(Game1.smallFont, this.QualifiedItemId, Game1.GlobalToLocal(Game1.viewport, this.drawPosition.Value), Color.Yellow, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
  }

  public virtual void drawAtNonTileSpot(
    SpriteBatch spriteBatch,
    Vector2 location,
    float layerDepth,
    float alpha = 1f)
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    Rectangle sourceRect = this.sourceRect.Value;
    sourceRect.X += sourceRect.Width * this.sourceIndexOffset.Value;
    if (dataOrErrorItem.IsErrorItem)
      sourceRect = dataOrErrorItem.GetSourceRect();
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), location, new Rectangle?(sourceRect), Color.White * alpha, 0.0f, Vector2.Zero, 4f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
  }

  public virtual int GetAdditionalTilePropertyRadius() => 0;

  public virtual bool DoesTileHaveProperty(
    int tile_x,
    int tile_y,
    string property_name,
    string layer_name,
    ref string property_value)
  {
    return false;
  }

  public virtual bool IntersectsForCollision(Rectangle rect)
  {
    return this.GetBoundingBox().Intersects(rect);
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Furniture(this.ItemId, this.tileLocation.Value);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Furniture furniture))
      return;
    this.drawPosition.Value = furniture.drawPosition.Value;
    this.defaultBoundingBox.Value = furniture.defaultBoundingBox.Value;
    this.boundingBox.Value = furniture.boundingBox.Value;
    this.isOn.Value = false;
    this.rotations.Value = furniture.rotations.Value;
    this.currentRotation.Value = furniture.currentRotation.Value - (this.rotations.Value == 4 ? 1 : 2);
    this.rotate();
  }
}
