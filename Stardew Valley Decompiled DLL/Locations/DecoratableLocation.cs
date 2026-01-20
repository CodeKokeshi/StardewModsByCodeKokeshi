// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.DecoratableLocation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class DecoratableLocation : GameLocation
{
  /// <summary>Obsolete.</summary>
  public readonly DecorationFacade wallPaper;
  [XmlIgnore]
  public readonly NetStringList wallpaperIDs;
  public readonly NetStringDictionary<string, NetString> appliedWallpaper;
  [XmlIgnore]
  public readonly Dictionary<string, List<Vector3>> wallpaperTiles;
  /// <summary>Obsolete.</summary>
  public readonly DecorationFacade floor;
  [XmlIgnore]
  public readonly NetStringList floorIDs;
  public readonly NetStringDictionary<string, NetString> appliedFloor;
  [XmlIgnore]
  public readonly Dictionary<string, List<Vector3>> floorTiles;
  protected Dictionary<string, TileSheet> _wallAndFloorTileSheets;
  protected Map _wallAndFloorTileSheetMap;
  /// <summary>Whether to log troubleshooting warnings for wallpaper and flooring issues.</summary>
  public static bool LogTroubleshootingInfo;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.appliedWallpaper, "appliedWallpaper").AddField((INetSerializable) this.appliedFloor, "appliedFloor").AddField((INetSerializable) this.floorIDs, "floorIDs").AddField((INetSerializable) this.wallpaperIDs, "wallpaperIDs");
    this.appliedWallpaper.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this.UpdateWallpaper(key));
    this.appliedWallpaper.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this.UpdateWallpaper(key));
    this.appliedWallpaper.OnValueTargetUpdated += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsUpdateEvent) ((key, old_value, new_value) =>
    {
      NetString netString;
      if (this.appliedWallpaper.FieldDict.TryGetValue(key, out netString))
        netString.CancelInterpolation();
      this.UpdateWallpaper(key);
    });
    this.appliedFloor.OnValueAdded += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsChangeEvent) ((key, value) => this.UpdateFloor(key));
    this.appliedFloor.OnConflictResolve += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ConflictResolveEvent) ((key, rejected, accepted) => this.UpdateFloor(key));
    this.appliedFloor.OnValueTargetUpdated += (NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.ContentsUpdateEvent) ((key, old_value, new_value) =>
    {
      NetString netString;
      if (this.appliedFloor.FieldDict.TryGetValue(key, out netString))
        netString.CancelInterpolation();
      this.UpdateFloor(key);
    });
  }

  public DecoratableLocation()
  {
    NetStringDictionary<string, NetString> stringDictionary1 = new NetStringDictionary<string, NetString>();
    stringDictionary1.InterpolationWait = false;
    this.appliedWallpaper = stringDictionary1;
    this.wallpaperTiles = new Dictionary<string, List<Vector3>>();
    this.floor = new DecorationFacade();
    this.floorIDs = new NetStringList();
    NetStringDictionary<string, NetString> stringDictionary2 = new NetStringDictionary<string, NetString>();
    stringDictionary2.InterpolationWait = false;
    this.appliedFloor = stringDictionary2;
    this.floorTiles = new Dictionary<string, List<Vector3>>();
    this._wallAndFloorTileSheets = new Dictionary<string, TileSheet>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public DecoratableLocation(string mapPath, string name)
  {
    NetStringDictionary<string, NetString> stringDictionary1 = new NetStringDictionary<string, NetString>();
    stringDictionary1.InterpolationWait = false;
    this.appliedWallpaper = stringDictionary1;
    this.wallpaperTiles = new Dictionary<string, List<Vector3>>();
    this.floor = new DecorationFacade();
    this.floorIDs = new NetStringList();
    NetStringDictionary<string, NetString> stringDictionary2 = new NetStringDictionary<string, NetString>();
    stringDictionary2.InterpolationWait = false;
    this.appliedFloor = stringDictionary2;
    this.floorTiles = new Dictionary<string, List<Vector3>>();
    this._wallAndFloorTileSheets = new Dictionary<string, TileSheet>();
    // ISSUE: explicit constructor call
    base.\u002Ector(mapPath, name);
  }

  public override void updateLayout()
  {
    base.updateLayout();
    if (!Game1.IsMasterGame)
      return;
    this.setWallpapers();
    this.setFloors();
  }

  public virtual void ReadWallpaperAndFloorTileData()
  {
    this.updateMap();
    this.wallpaperTiles.Clear();
    this.floorTiles.Clear();
    this.wallpaperIDs.Clear();
    this.floorIDs.Clear();
    string str1 = "0";
    string str2 = "0";
    if (this is FarmHouse farmHouse && farmHouse.upgradeLevel < 3)
    {
      Farm locationFromName = Game1.getLocationFromName("Farm", false) as Farm;
      str1 = FarmHouse.GetStarterWallpaper(locationFromName) ?? "0";
      str2 = FarmHouse.GetStarterFlooring(locationFromName) ?? "0";
    }
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    string propertyValue1;
    if (this.TryGetMapProperty("WallIDs", out propertyValue1))
    {
      foreach (string str3 in propertyValue1.Split(','))
      {
        string[] strArray = ArgUtility.SplitBySpace(str3);
        if (strArray.Length >= 1)
          this.wallpaperIDs.Add(strArray[0]);
        if (strArray.Length >= 2)
          dictionary[strArray[0]] = strArray[1];
      }
    }
    KeyValuePair<string, int> keyValuePair;
    if (this.wallpaperIDs.Count == 0)
    {
      List<Microsoft.Xna.Framework.Rectangle> walls = this.getWalls();
      for (int index = 0; index < walls.Count; ++index)
      {
        string key = "Wall_" + index.ToString();
        this.wallpaperIDs.Add(key);
        Microsoft.Xna.Framework.Rectangle rect = walls[index];
        if (!this.wallpaperTiles.ContainsKey(index.ToString()))
          this.wallpaperTiles[key] = new List<Vector3>();
        foreach (Point point in rect.GetPoints())
          this.wallpaperTiles[key].Add(new Vector3((float) point.X, (float) point.Y, (float) (point.Y - rect.Top)));
      }
    }
    else
    {
      for (int index1 = 0; index1 < this.map.Layers[0].LayerWidth; ++index1)
      {
        for (int index2 = 0; index2 < this.map.Layers[0].LayerHeight; ++index2)
        {
          string key = this.doesTileHaveProperty(index1, index2, "WallID", "Back");
          if (key != null)
          {
            if (!this.wallpaperIDs.Contains(key))
              this.wallpaperIDs.Add(key);
            string str4;
            if (this.appliedWallpaper.TryAdd(key, str1) && dictionary.TryGetValue(key, out str4))
            {
              string str5;
              if (this.appliedWallpaper.TryGetValue(str4, out str5))
              {
                this.appliedWallpaper[key] = str5;
              }
              else
              {
                keyValuePair = this.GetWallpaperSource(str4);
                if (keyValuePair.Value >= 0)
                  this.appliedWallpaper[key] = str4;
              }
            }
            List<Vector3> vector3List;
            if (!this.wallpaperTiles.TryGetValue(key, out vector3List))
              this.wallpaperTiles[key] = vector3List = new List<Vector3>();
            vector3List.Add(new Vector3((float) index1, (float) index2, 0.0f));
            if (this.IsFloorableOrWallpaperableTile(index1, index2 + 1, "Back"))
              vector3List.Add(new Vector3((float) index1, (float) (index2 + 1), 1f));
            if (this.IsFloorableOrWallpaperableTile(index1, index2 + 2, "Buildings"))
              vector3List.Add(new Vector3((float) index1, (float) (index2 + 2), 2f));
            else if (this.IsFloorableOrWallpaperableTile(index1, index2 + 2, "Back") && !this.IsFloorableTile(index1, index2 + 2, "Back"))
              vector3List.Add(new Vector3((float) index1, (float) (index2 + 2), 2f));
          }
        }
      }
    }
    dictionary.Clear();
    string propertyValue2;
    if (this.TryGetMapProperty("FloorIDs", out propertyValue2))
    {
      foreach (string str6 in propertyValue2.Split(','))
      {
        string[] strArray = ArgUtility.SplitBySpace(str6);
        if (strArray.Length >= 1)
          this.floorIDs.Add(strArray[0]);
        if (strArray.Length >= 2)
          dictionary[strArray[0]] = strArray[1];
      }
    }
    if (this.floorIDs.Count == 0)
    {
      List<Microsoft.Xna.Framework.Rectangle> floors = this.getFloors();
      for (int index = 0; index < floors.Count; ++index)
      {
        string key = "Floor_" + index.ToString();
        this.floorIDs.Add(key);
        Microsoft.Xna.Framework.Rectangle rect = floors[index];
        if (!this.floorTiles.ContainsKey(index.ToString()))
          this.floorTiles[key] = new List<Vector3>();
        foreach (Point point in rect.GetPoints())
          this.floorTiles[key].Add(new Vector3((float) point.X, (float) point.Y, 0.0f));
      }
    }
    else
    {
      for (int index3 = 0; index3 < this.map.Layers[0].LayerWidth; ++index3)
      {
        for (int index4 = 0; index4 < this.map.Layers[0].LayerHeight; ++index4)
        {
          string key = this.doesTileHaveProperty(index3, index4, "FloorID", "Back");
          if (key != null)
          {
            if (!this.floorIDs.Contains(key))
              this.floorIDs.Add(key);
            string str7;
            if (this.appliedFloor.TryAdd(key, str2) && dictionary.TryGetValue(key, out str7))
            {
              string str8;
              if (this.appliedFloor.TryGetValue(str7, out str8))
              {
                this.appliedFloor[key] = str8;
              }
              else
              {
                keyValuePair = this.GetFloorSource(str7);
                if (keyValuePair.Value >= 0)
                  this.appliedFloor[key] = str7;
              }
            }
            List<Vector3> vector3List;
            if (!this.floorTiles.TryGetValue(key, out vector3List))
              this.floorTiles[key] = vector3List = new List<Vector3>();
            vector3List.Add(new Vector3((float) index3, (float) index4, 0.0f));
          }
        }
      }
    }
    this.setFloors();
    this.setWallpapers();
  }

  public virtual TileSheet GetWallAndFloorTilesheet(string id)
  {
    if (this.map != this._wallAndFloorTileSheetMap)
    {
      this._wallAndFloorTileSheets.Clear();
      this._wallAndFloorTileSheetMap = this.map;
    }
    TileSheet andFloorTilesheet;
    if (this._wallAndFloorTileSheets.TryGetValue(id, out andFloorTilesheet))
      return andFloorTilesheet;
    try
    {
      foreach (ModWallpaperOrFlooring wallpaperOrFlooring in DataLoader.AdditionalWallpaperFlooring(Game1.content))
      {
        if (!(wallpaperOrFlooring.Id != id))
        {
          Texture2D texture2D = Game1.content.Load<Texture2D>(wallpaperOrFlooring.Texture);
          if (texture2D.Width != 256 /*0x0100*/)
            Game1.log.Warn($"The tilesheet for wallpaper/floor '{wallpaperOrFlooring.Id}' is {texture2D.Width} pixels wide, but it must be exactly {256 /*0x0100*/} pixels wide.");
          TileSheet tileSheet = new TileSheet("x_WallsAndFloors_" + id, this.map, wallpaperOrFlooring.Texture, new Size(texture2D.Width / 16 /*0x10*/, texture2D.Height / 16 /*0x10*/), new Size(16 /*0x10*/, 16 /*0x10*/));
          this.map.AddTileSheet(tileSheet);
          this.map.LoadTileSheets(Game1.mapDisplayDevice);
          this._wallAndFloorTileSheets[id] = tileSheet;
          return tileSheet;
        }
      }
      Game1.log.Error($"The tilesheet for wallpaper/floor '{id}' could not be loaded: no such ID found in Data/AdditionalWallpaperFlooring.");
      this._wallAndFloorTileSheets[id] = (TileSheet) null;
      return (TileSheet) null;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"The tilesheet for wallpaper/floor '{id}' could not be loaded.", ex);
      this._wallAndFloorTileSheets[id] = (TileSheet) null;
      return (TileSheet) null;
    }
  }

  public virtual KeyValuePair<string, int> GetFloorSource(string pattern_id)
  {
    int result;
    if (pattern_id.Contains(':'))
    {
      string[] strArray = pattern_id.Split(':');
      TileSheet andFloorTilesheet = this.GetWallAndFloorTilesheet(strArray[0]);
      if (int.TryParse(strArray[1], out result) && andFloorTilesheet != null)
        return new KeyValuePair<string, int>(andFloorTilesheet.Id, result);
    }
    return int.TryParse(pattern_id, out result) ? new KeyValuePair<string, int>("walls_and_floors", result) : new KeyValuePair<string, int>((string) null, -1);
  }

  public virtual KeyValuePair<string, int> GetWallpaperSource(string pattern_id)
  {
    int result;
    if (pattern_id.Contains(':'))
    {
      string[] strArray = pattern_id.Split(':');
      TileSheet andFloorTilesheet = this.GetWallAndFloorTilesheet(strArray[0]);
      if (int.TryParse(strArray[1], out result) && andFloorTilesheet != null)
        return new KeyValuePair<string, int>(andFloorTilesheet.Id, result);
    }
    return int.TryParse(pattern_id, out result) ? new KeyValuePair<string, int>("walls_and_floors", result) : new KeyValuePair<string, int>((string) null, -1);
  }

  public virtual void UpdateFloor(string floorId)
  {
    this.updateMap();
    string pattern_id;
    List<Vector3> vector3List;
    if (!this.appliedFloor.TryGetValue(floorId, out pattern_id) || !this.floorTiles.TryGetValue(floorId, out vector3List))
      return;
    bool flag = false;
    HashSet<string> values = (HashSet<string>) null;
    foreach (Vector3 vector3 in vector3List)
    {
      int x = (int) vector3.X;
      int y = (int) vector3.Y;
      KeyValuePair<string, int> floorSource = this.GetFloorSource(pattern_id);
      if (floorSource.Value < 0)
      {
        if (DecoratableLocation.LogTroubleshootingInfo)
        {
          values = values ?? new HashSet<string>();
          values.Add($"floor pattern '{pattern_id}' doesn't match any known floor set");
        }
      }
      else
      {
        string key = floorSource.Key;
        int num = floorSource.Value;
        int sheetWidth = this.map.RequireTileSheet(key).SheetWidth;
        int base_tile_sheet = num * 2 + num / (sheetWidth / 2) * sheetWidth;
        if (key == "walls_and_floors")
          base_tile_sheet += this.GetFirstFlooringTile();
        string reasonInvalid;
        if (!this.IsFloorableOrWallpaperableTile(x, y, "Back", out reasonInvalid))
        {
          if (DecoratableLocation.LogTroubleshootingInfo)
          {
            values = values ?? new HashSet<string>();
            values.Add(reasonInvalid);
          }
        }
        else
        {
          this.setMapTile(x, y, this.GetFlooringIndex(base_tile_sheet, x, y), "Back", key);
          flag = true;
        }
      }
    }
    // ISSUE: explicit non-virtual call
    if (flag || values == null || __nonvirtual (values.Count) <= 0)
      return;
    Game1.log.Warn($"Couldn't apply floors for area ID '{floorId}' ({string.Join("; ", (IEnumerable<string>) values)})");
  }

  public virtual void UpdateWallpaper(string wallpaperId)
  {
    this.updateMap();
    string pattern_id;
    List<Vector3> vector3List;
    if (!this.appliedWallpaper.TryGetValue(wallpaperId, out pattern_id) || !this.wallpaperTiles.TryGetValue(wallpaperId, out vector3List))
      return;
    bool flag = false;
    HashSet<string> values = (HashSet<string>) null;
    foreach (Vector3 vector3 in vector3List)
    {
      int x = (int) vector3.X;
      int y = (int) vector3.Y;
      int z = (int) vector3.Z;
      KeyValuePair<string, int> wallpaperSource = this.GetWallpaperSource(pattern_id);
      if (wallpaperSource.Value < 0)
      {
        if (DecoratableLocation.LogTroubleshootingInfo)
        {
          values = values ?? new HashSet<string>();
          values.Add($"wallpaper pattern '{pattern_id}' doesn't match any known wallpaper set");
        }
      }
      else
      {
        string key = wallpaperSource.Key;
        int num = wallpaperSource.Value;
        TileSheet tileSheet = this.map.RequireTileSheet(key);
        int sheetWidth = tileSheet.SheetWidth;
        string str = z != 2 || !this.IsFloorableOrWallpaperableTile(x, y, "Buildings", out string _) ? "Back" : "Buildings";
        string reasonInvalid;
        if (!this.IsFloorableOrWallpaperableTile(x, y, str, out reasonInvalid))
        {
          if (DecoratableLocation.LogTroubleshootingInfo)
          {
            values = values ?? new HashSet<string>();
            values.Add(reasonInvalid);
          }
        }
        else
        {
          this.setMapTile(x, y, num / sheetWidth * sheetWidth * 3 + num % sheetWidth + z * sheetWidth, str, tileSheet.Id);
          flag = true;
        }
      }
    }
    // ISSUE: explicit non-virtual call
    if (flag || values == null || __nonvirtual (values.Count) <= 0)
      return;
    Game1.log.Warn($"Couldn't apply wallpaper for area ID '{wallpaperId}' ({string.Join("; ", (IEnumerable<string>) values)})");
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (this.wasUpdated)
      return;
    base.UpdateWhenCurrentLocation(time);
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    if (!(this is FarmHouse))
    {
      this.ReadWallpaperAndFloorTileData();
      this.setWallpapers();
      this.setFloors();
    }
    if (!this.hasTileAt(Game1.player.TilePoint, "Buildings"))
      return;
    Game1.player.position.Y += 64f;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (!Game1.player.mailReceived.Add("button_tut_1"))
      return;
    Game1.onScreenMenus.Add((IClickableMenu) new ButtonTutorialMenu(0));
  }

  public override bool CanFreePlaceFurniture() => true;

  public virtual bool isTileOnWall(int x, int y)
  {
    foreach (string key in this.wallpaperTiles.Keys)
    {
      foreach (Vector3 vector3 in this.wallpaperTiles[key])
      {
        if ((int) vector3.X == x && (int) vector3.Y == y)
          return true;
      }
    }
    return false;
  }

  public int GetWallTopY(int x, int y)
  {
    foreach (string key in this.wallpaperTiles.Keys)
    {
      foreach (Vector3 vector3 in this.wallpaperTiles[key])
      {
        if ((int) vector3.X == x && (int) vector3.Y == y)
          return y - (int) vector3.Z;
      }
    }
    return -1;
  }

  public virtual void setFloors()
  {
    foreach (KeyValuePair<string, string> pair in this.appliedFloor.Pairs)
      this.UpdateFloor(pair.Key);
  }

  public virtual void setWallpapers()
  {
    foreach (KeyValuePair<string, string> pair in this.appliedWallpaper.Pairs)
      this.UpdateWallpaper(pair.Key);
  }

  public void SetFloor(string which, string which_room)
  {
    if (which_room == null)
    {
      foreach (string floorId in (NetList<string, NetString>) this.floorIDs)
        this.appliedFloor[floorId] = which;
    }
    else
      this.appliedFloor[which_room] = which;
  }

  public void SetWallpaper(string which, string which_room)
  {
    if (which_room == null)
    {
      foreach (string wallpaperId in (NetList<string, NetString>) this.wallpaperIDs)
        this.appliedWallpaper[wallpaperId] = which;
    }
    else
      this.appliedWallpaper[which_room] = which;
  }

  public void OverrideSpecificWallpaper(
    string which,
    string which_room,
    string wallpaperStyleToOverride)
  {
    if (which_room == null)
    {
      foreach (string wallpaperId in (NetList<string, NetString>) this.wallpaperIDs)
      {
        string str;
        if (this.appliedWallpaper.TryGetValue(wallpaperId, out str) && str == wallpaperStyleToOverride)
          this.appliedWallpaper[wallpaperId] = which;
      }
    }
    else
    {
      if (!(this.appliedWallpaper[which_room] == wallpaperStyleToOverride))
        return;
      this.appliedWallpaper[which_room] = which;
    }
  }

  public void OverrideSpecificFlooring(
    string which,
    string which_room,
    string flooringStyleToOverride)
  {
    if (which_room == null)
    {
      foreach (string floorId in (NetList<string, NetString>) this.floorIDs)
      {
        string str;
        if (this.appliedFloor.TryGetValue(floorId, out str) && str == flooringStyleToOverride)
          this.appliedFloor[floorId] = which;
      }
    }
    else
    {
      if (!(this.appliedFloor[which_room] == flooringStyleToOverride))
        return;
      this.appliedFloor[which_room] = which;
    }
  }

  public string GetFloorID(int x, int y)
  {
    foreach (string key in this.floorTiles.Keys)
    {
      foreach (Vector3 vector3 in this.floorTiles[key])
      {
        if ((int) vector3.X == x && (int) vector3.Y == y)
          return key;
      }
    }
    return (string) null;
  }

  public string GetWallpaperID(int x, int y)
  {
    foreach (string key in this.wallpaperTiles.Keys)
    {
      foreach (Vector3 vector3 in this.wallpaperTiles[key])
      {
        if ((int) vector3.X == x && (int) vector3.Y == y)
          return key;
      }
    }
    return (string) null;
  }

  protected bool IsFloorableTile(int x, int y, string layer_name)
  {
    switch (this.getTileIndexAt(x, y, "Buildings", "untitled tile sheet"))
    {
      case 197:
      case 198:
      case 199:
        return false;
      default:
        return this.IsFloorableOrWallpaperableTile(x, y, layer_name);
    }
  }

  public bool IsWallAndFloorTilesheet(string tilesheet_id)
  {
    return tilesheet_id == "walls_and_floors" || tilesheet_id.Contains("walls_and_floors") || tilesheet_id.StartsWith("x_WallsAndFloors_");
  }

  protected bool IsFloorableOrWallpaperableTile(int x, int y, string layerName)
  {
    return this.IsFloorableOrWallpaperableTile(x, y, layerName, out string _);
  }

  protected bool IsFloorableOrWallpaperableTile(
    int x,
    int y,
    string layerName,
    out string reasonInvalid)
  {
    Layer layer = this.map.GetLayer(layerName);
    if (layer == null)
    {
      reasonInvalid = $"layer '{layerName}' not found";
      return false;
    }
    if (x < 0 || x >= layer.LayerWidth || y < 0 || y >= layer.LayerHeight)
    {
      reasonInvalid = $"tile ({x}, {y}) is out of bounds for the layer";
      return false;
    }
    Tile tile = layer.Tiles[x, y];
    if (tile == null)
    {
      reasonInvalid = $"tile ({x}, {y}) not found";
      return false;
    }
    TileSheet tileSheet = tile.TileSheet;
    if (tileSheet == null)
    {
      reasonInvalid = $"tile ({x}, {y}) has unknown tilesheet";
      return false;
    }
    if (!this.IsWallAndFloorTilesheet(tileSheet.Id))
    {
      reasonInvalid = $"tilesheet '{tileSheet.Id}' isn't a wall and floor tilesheet, expected tilesheet ID containing 'walls_and_floors' or starting with 'x_WallsAndFloors_'";
      return false;
    }
    reasonInvalid = (string) null;
    return true;
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is DecoratableLocation decoratableLocation)
    {
      NetDictionary<string, string, NetString, SerializableDictionary<string, string>, NetStringDictionary<string, NetString>>.KeysCollection keys = decoratableLocation.appliedWallpaper.Keys;
      if (!keys.Any())
      {
        keys = decoratableLocation.appliedFloor.Keys;
        if (!keys.Any())
        {
          this.ReadWallpaperAndFloorTileData();
          for (int index = 0; index < decoratableLocation.wallPaper.Count; ++index)
          {
            try
            {
              this.appliedWallpaper[this.wallpaperIDs[index]] = decoratableLocation.wallPaper[index].ToString();
            }
            catch (Exception ex)
            {
            }
          }
          for (int index = 0; index < decoratableLocation.floor.Count; ++index)
          {
            try
            {
              this.appliedFloor[this.floorIDs[index]] = decoratableLocation.floor[index].ToString();
            }
            catch (Exception ex)
            {
            }
          }
          goto label_23;
        }
      }
      keys = decoratableLocation.appliedWallpaper.Keys;
      foreach (string key in keys)
        this.appliedWallpaper[key] = decoratableLocation.appliedWallpaper[key];
      foreach (string key in decoratableLocation.appliedFloor.Keys)
        this.appliedFloor[key] = decoratableLocation.appliedFloor[key];
    }
label_23:
    this.setWallpapers();
    this.setFloors();
    base.TransferDataFromSavedLocation(l);
  }

  public Furniture getRandomFurniture(Random r)
  {
    return r.ChooseFrom<Furniture>((IList<Furniture>) this.furniture);
  }

  public virtual string getFloorRoomIdAt(Point p)
  {
    foreach (string key in this.floorTiles.Keys)
    {
      foreach (Vector3 vector3 in this.floorTiles[key])
      {
        if ((int) vector3.X == p.X && (int) vector3.Y == p.Y)
          return key;
      }
    }
    return (string) null;
  }

  public virtual int GetFirstFlooringTile() => 336;

  public virtual int GetFlooringIndex(int base_tile_sheet, int tile_x, int tile_y)
  {
    if (!this.hasTileAt(tile_x, tile_y, "Back"))
      return 0;
    TileSheet tileSheet = this.map.GetTileSheet(this.getTileSheetIDAt(tile_x, tile_y, "Back"));
    int num1 = 16 /*0x10*/;
    if (tileSheet != null)
      num1 = tileSheet.SheetWidth;
    int num2 = tile_x % 2;
    int num3 = tile_y % 2;
    return base_tile_sheet + num2 + num1 * num3;
  }

  public virtual List<Microsoft.Xna.Framework.Rectangle> getFloors() => new List<Microsoft.Xna.Framework.Rectangle>();
}
