// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.PlayerStatusList
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

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class PlayerStatusList : INetObject<NetFields>
{
  protected readonly NetLongDictionary<string, NetString> _statusList;
  protected readonly Dictionary<long, string> _formattedStatusList;
  protected readonly Dictionary<string, Texture2D> _iconSprites;
  protected readonly List<Farmer> _sortedFarmers;
  public int iconAnimationFrames;
  public int largestSpriteWidth;
  public int largestSpriteHeight;
  public PlayerStatusList.SortMode sortMode;
  public PlayerStatusList.DisplayMode displayMode;
  protected Dictionary<string, KeyValuePair<string, Rectangle>> _iconDefinitions;

  public NetFields NetFields { get; }

  public PlayerStatusList()
  {
    NetLongDictionary<string, NetString> netLongDictionary = new NetLongDictionary<string, NetString>();
    netLongDictionary.InterpolationWait = false;
    this._statusList = netLongDictionary;
    this._formattedStatusList = new Dictionary<long, string>();
    this._iconSprites = new Dictionary<string, Texture2D>();
    this._sortedFarmers = new List<Farmer>();
    this.iconAnimationFrames = 1;
    this._iconDefinitions = new Dictionary<string, KeyValuePair<string, Rectangle>>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.InitNetFields();
  }

  public void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this._statusList, "_statusList");
    this._statusList.OnValueRemoved += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ContentsChangeEvent) ((_param1, _param2) => this._OnValueChanged());
    this._statusList.OnValueAdded += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ContentsChangeEvent) ((_param1, _param2) => this._OnValueChanged());
    this._statusList.OnConflictResolve += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ConflictResolveEvent) ((_param1, _param2, _param3) => this._OnValueChanged());
    this._statusList.OnValueTargetUpdated += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ContentsUpdateEvent) ((key, value, targetValue) =>
    {
      NetString netString;
      if (this._statusList.FieldDict.TryGetValue(key, out netString))
        netString.CancelInterpolation();
      this._OnValueChanged();
    });
  }

  public void AddSpriteDefinition(string key, string file, int x, int y, int width, int height)
  {
    Texture2D texture2D;
    if (!this._iconSprites.TryGetValue(file, out texture2D) || texture2D.IsDisposed)
      this._iconSprites[file] = Game1.content.Load<Texture2D>(file);
    this._iconDefinitions[key] = new KeyValuePair<string, Rectangle>(file, new Rectangle(x, y, width, height));
    if (width > this.largestSpriteWidth)
      this.largestSpriteWidth = width;
    if (height <= this.largestSpriteHeight)
      return;
    this.largestSpriteHeight = height;
  }

  public void UpdateState(string newState)
  {
    string str;
    if (this._statusList.TryGetValue(Game1.player.UniqueMultiplayerID, out str) && !(str != newState))
      return;
    this._statusList[Game1.player.UniqueMultiplayerID] = newState;
  }

  public void WithdrawState() => this._statusList.Remove(Game1.player.UniqueMultiplayerID);

  protected void _OnValueChanged()
  {
    foreach (long key in this._statusList.Keys)
      this._formattedStatusList[key] = this.GetStatusText(key);
    this._ResortList();
  }

  protected void _ResortList()
  {
    this._sortedFarmers.Clear();
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      this._sortedFarmers.Add(onlineFarmer);
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      if (Game1.IsMasterGame && !this._sortedFarmers.Contains(allFarmer))
        this._statusList.Remove(allFarmer.UniqueMultiplayerID);
      if (!this._statusList.ContainsKey(allFarmer.UniqueMultiplayerID))
        this._sortedFarmers.Remove(allFarmer);
    }
    switch (this.sortMode)
    {
      case PlayerStatusList.SortMode.NumberSort:
      case PlayerStatusList.SortMode.NumberSortDescending:
        this._sortedFarmers.Sort((Comparison<Farmer>) ((a, b) => this.GetStatusInt(a.UniqueMultiplayerID).CompareTo(this.GetStatusInt(b.UniqueMultiplayerID))));
        if (this.sortMode != PlayerStatusList.SortMode.NumberSortDescending)
          break;
        this._sortedFarmers.Reverse();
        break;
      case PlayerStatusList.SortMode.AlphaSort:
      case PlayerStatusList.SortMode.AlphaSortDescending:
        this._sortedFarmers.Sort((Comparison<Farmer>) ((a, b) => this.GetStatusText(a.UniqueMultiplayerID).CompareTo(this.GetStatusText(b.UniqueMultiplayerID))));
        if (this.sortMode != PlayerStatusList.SortMode.AlphaSortDescending)
          break;
        this._sortedFarmers.Reverse();
        break;
    }
  }

  /// <summary>Try to get the status text for a player.</summary>
  /// <param name="id">The unique multiplayer ID for the player whose status to get.</param>
  /// <param name="statusText">The status text if found, else <c>null</c>.</param>
  /// <returns>Whether the status was found.</returns>
  public bool TryGetStatusText(long id, out string statusText)
  {
    if (this._statusList.TryGetValue(id, out statusText))
    {
      if (this.displayMode == PlayerStatusList.DisplayMode.LocalizedText)
        statusText = Game1.content.LoadString(statusText);
      return true;
    }
    statusText = (string) null;
    return false;
  }

  /// <summary>Get the string representation of a player's status.</summary>
  /// <param name="id">The unique multiplayer ID for the player whose status to get.</param>
  /// <param name="fallback">The value to return if no status is found for the player.</param>
  /// <returns>The string representation of the player's status, or <paramref name="fallback" /> if not found.</returns>
  public string GetStatusText(long id, string fallback = "")
  {
    string statusText;
    return !this.TryGetStatusText(id, out statusText) ? fallback : statusText;
  }

  /// <summary>Get the integer representation of a player's status (e.g. number of eggs found at the Egg Festival).</summary>
  /// <param name="id">The unique multiplayer ID for the player whose status to get.</param>
  /// <param name="fallback">The value to return if no status is found for the player.</param>
  /// <returns>The integer representation of the player's status, or <paramref name="fallback" /> if not found.</returns>
  public int GetStatusInt(long id, int fallback = 0)
  {
    string statusText;
    int result;
    return !this.TryGetStatusText(id, out statusText) || !int.TryParse(statusText, out result) ? fallback : result;
  }

  public void Draw(
    SpriteBatch b,
    Vector2 draw_position,
    float draw_scale = 4f,
    float draw_layer = 0.45f,
    PlayerStatusList.HorizontalAlignment horizontal_origin = PlayerStatusList.HorizontalAlignment.Left,
    PlayerStatusList.VerticalAlignment vertical_origin = PlayerStatusList.VerticalAlignment.Top)
  {
    float num1 = 12f;
    if (this.displayMode == PlayerStatusList.DisplayMode.Icons && (double) this.largestSpriteHeight > (double) num1)
      num1 = (float) this.largestSpriteHeight;
    if (horizontal_origin == PlayerStatusList.HorizontalAlignment.Right)
    {
      float num2 = 0.0f;
      if (this.displayMode == PlayerStatusList.DisplayMode.Icons)
      {
        draw_position.X -= (float) this.largestSpriteWidth * draw_scale;
      }
      else
      {
        foreach (Farmer sortedFarmer in this._sortedFarmers)
        {
          string text;
          if (!sortedFarmer.IsDedicatedPlayer && this._formattedStatusList.TryGetValue(sortedFarmer.UniqueMultiplayerID, out text))
          {
            float x = Game1.dialogueFont.MeasureString(text).X;
            if ((double) num2 < (double) x)
              num2 = x;
          }
        }
        draw_position.X -= (num2 + 16f) * draw_scale;
      }
    }
    if (vertical_origin == PlayerStatusList.VerticalAlignment.Bottom)
      draw_position.Y -= num1 * (float) this._statusList.Length * draw_scale;
    foreach (Farmer sortedFarmer in this._sortedFarmers)
    {
      if (!sortedFarmer.IsDedicatedPlayer)
      {
        float num3 = Game1.isUsingBackToFrontSorting ? -1f : 1f;
        string key;
        if (this._formattedStatusList.TryGetValue(sortedFarmer.UniqueMultiplayerID, out key))
        {
          Vector2 zero = Vector2.Zero;
          sortedFarmer.FarmerRenderer.drawMiniPortrat(b, draw_position, draw_layer, draw_scale * 0.75f, 2, sortedFarmer);
          KeyValuePair<string, Rectangle> keyValuePair;
          if (this.displayMode == PlayerStatusList.DisplayMode.Icons && this._iconDefinitions.TryGetValue(key, out keyValuePair))
          {
            zero.X += 12f * draw_scale;
            Rectangle rectangle = keyValuePair.Value with
            {
              Y = (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % (double) (this.iconAnimationFrames * 100) / 100.0) * 16 /*0x10*/
            };
            b.Draw(this._iconSprites[keyValuePair.Key], draw_position + zero, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, draw_scale, SpriteEffects.None, draw_layer - 0.0001f * num3);
          }
          else
          {
            zero.X += 16f * draw_scale;
            zero.Y += 2f * draw_scale;
            string text = key;
            b.DrawString(Game1.dialogueFont, text, draw_position + zero + Vector2.One * draw_scale, Color.Black, 0.0f, Vector2.Zero, draw_scale / 4f, SpriteEffects.None, draw_layer - 0.0001f * num3);
            b.DrawString(Game1.dialogueFont, text, draw_position + zero, Color.White, 0.0f, Vector2.Zero, draw_scale / 4f, SpriteEffects.None, draw_layer);
          }
          draw_position.Y += num1 * draw_scale;
        }
      }
    }
  }

  public enum SortMode
  {
    None,
    NumberSort,
    NumberSortDescending,
    AlphaSort,
    AlphaSortDescending,
  }

  public enum DisplayMode
  {
    Text,
    LocalizedText,
    Icons,
  }

  public enum VerticalAlignment
  {
    Top,
    Bottom,
  }

  public enum HorizontalAlignment
  {
    Left,
    Right,
  }
}
