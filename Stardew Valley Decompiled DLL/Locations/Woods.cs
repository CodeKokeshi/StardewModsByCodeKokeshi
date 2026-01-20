// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Woods
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Locations;

public class Woods : GameLocation
{
  public const int numBaubles = 25;
  private List<Vector2> baubles;
  private List<WeatherDebris> weatherDebris;
  [XmlElement("hasUnlockedStatue")]
  public readonly NetBool hasUnlockedStatue = new NetBool();
  [XmlElement("addedSlimesToday")]
  private readonly NetBool addedSlimesToday = new NetBool();
  [XmlIgnore]
  private readonly NetEvent0 statueAnimationEvent = new NetEvent0();
  protected Color _ambientLightColor = Color.White;
  private int statueTimer;

  public Woods()
  {
  }

  public Woods(string map, string name)
    : base(map, name)
  {
    this.isOutdoors.Value = true;
    this.ignoreDebrisWeather.Value = true;
    this.ignoreOutdoorLighting.Value = true;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.addedSlimesToday, "addedSlimesToday").AddField((INetSerializable) this.statueAnimationEvent, "statueAnimationEvent").AddField((INetSerializable) this.hasUnlockedStatue, "hasUnlockedStatue");
    this.statueAnimationEvent.onEvent += new NetEvent0.Event(this.doStatueAnimation);
  }

  /// <summary>Reset the crow shop which contains lost unique items.</summary>
  public static void ResetLostItemsShop()
  {
    IInventory itemsShopInventory = Woods.GetLostItemsShopInventory();
    itemsShopInventory.Clear();
    Dictionary<string, int> itemsInSave = new Dictionary<string, int>();
    Utility.ForEachItem((Func<Item, bool>) (item =>
    {
      itemsInSave[item.QualifiedItemId] = itemsInSave.GetValueOrDefault<string, int>(item.QualifiedItemId) + item.Stack;
      return true;
    }));
    Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
    Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
    foreach (Farmer allFarmer in Game1.getAllFarmers())
    {
      foreach (string key in (NetHashSet<string>) allFarmer.eventsSeen)
        dictionary1[key] = dictionary1.GetValueOrDefault<string, int>(key) + 1;
      foreach (string key in (NetHashSet<string>) allFarmer.mailReceived)
        dictionary2[key] = dictionary2.GetValueOrDefault<string, int>(key) + 1;
    }
    foreach (LostItem lostItem in DataLoader.LostItemsShop(Game1.content))
    {
      int valueOrDefault1;
      if (lostItem.RequireMailReceived != null)
        valueOrDefault1 = dictionary2.GetValueOrDefault<string, int>(lostItem.RequireMailReceived);
      else if (lostItem.RequireEventSeen != null)
        valueOrDefault1 = dictionary1.GetValueOrDefault<string, int>(lostItem.RequireEventSeen);
      else
        continue;
      int valueOrDefault2 = itemsInSave.GetValueOrDefault<string, int>(lostItem.ItemId);
      int num = valueOrDefault1 - valueOrDefault2;
      if (num > 0)
      {
        for (int index = 0; index < num; ++index)
          itemsShopInventory.Add(ItemRegistry.Create(lostItem.ItemId));
      }
    }
  }

  public bool localPlayerHasFoundStardrop() => Game1.player.hasOrWillReceiveMail("CF_Statue");

  public void statueAnimation(Farmer who)
  {
    if (this.hasUnlockedStatue.Value)
      return;
    who.reduceActiveItemByOne();
    this.hasUnlockedStatue.Value = true;
    this.statueAnimationEvent.Fire();
  }

  private void doStatueAnimation()
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 7f) * 64f, Color.White, 9, animationInterval: 50f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 7f) * 64f, Color.Orange, 9, animationInterval: 70f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 6f) * 64f, Color.White, 9, animationInterval: 60f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 6f) * 64f, Color.OrangeRed, 9, animationInterval: 120f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(8f, 5f) * 64f, Color.Red, 9));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(9f, 5f) * 64f, Color.White, 9, animationInterval: 170f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 464f), Color.Orange, 9, animationInterval: 40f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 464f), Color.White, 9, animationInterval: 90f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 400f), Color.OrangeRed, 9, animationInterval: 190f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 400f), Color.White, 9, animationInterval: 80f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(544f, 336f), Color.Red, 9, animationInterval: 69f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(608f, 336f), Color.OrangeRed, 9, animationInterval: 130f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Orange, 9, animationInterval: 40f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, animationInterval: 90f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(480f, 464f), Color.Red, 9, animationInterval: 30f));
    this.temporarySprites.Add(new TemporaryAnimatedSprite(11, new Vector2(672f, 368f), Color.White, 9, animationInterval: 180f));
    this.localSound("secret1");
    this.updateStatueEyes();
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (who.IsLocalPlayer)
    {
      switch (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet"))
      {
        case 1140:
        case 1141:
          if (!this.hasUnlockedStatue.Value)
          {
            if (who.ActiveObject?.QualifiedItemId == "(O)417")
            {
              this.statueTimer = 1000;
              who.freezePause = 1000;
              Game1.changeMusicTrack("none");
              this.playSound("newArtifact");
            }
            else
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Woods_Statue").Replace('\n', '^'));
          }
          if (this.hasUnlockedStatue.Value && !this.localPlayerHasFoundStardrop() && who.freeSpotsInInventory() > 0)
          {
            who.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(O)434"));
            Game1.player.mailReceived.Add("CF_Statue");
          }
          return true;
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    Woods.GetLostItemShopMutex().ReleaseLock();
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Monster));
    this.addedSlimesToday.Value = false;
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.baubles?.Clear();
    this.weatherDebris?.Clear();
  }

  protected override void resetSharedState()
  {
    if (!this.addedSlimesToday.Value)
    {
      this.addedSlimesToday.Value = true;
      Random random = Utility.CreateRandom((double) Game1.stats.DaysPlayed, (double) Game1.uniqueIDForThisGame, 12.0);
      for (int index = 50; index > 0; --index)
      {
        Vector2 randomTile = this.getRandomTile();
        if (random.NextDouble() < 0.25 && this.CanItemBePlacedHere(randomTile))
        {
          switch (this.GetSeason())
          {
            case Season.Spring:
              this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 0));
              continue;
            case Season.Summer:
              this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 0));
              continue;
            case Season.Fall:
              this.characters.Add((NPC) new GreenSlime(randomTile * 64f, random.Choose<int>(0, 40)));
              continue;
            case Season.Winter:
              this.characters.Add((NPC) new GreenSlime(randomTile * 64f, 40));
              continue;
            default:
              continue;
          }
        }
      }
    }
    base.resetSharedState();
  }

  protected void _updateWoodsLighting()
  {
    if (Game1.currentLocation != this)
      return;
    int minutes1 = Utility.ConvertTimeToMinutes(Game1.getStartingToGetDarkTime((GameLocation) this));
    int minutes2 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime((GameLocation) this));
    int minutes3 = Utility.ConvertTimeToMinutes(Game1.getModeratelyDarkTime((GameLocation) this));
    int minutes4 = Utility.ConvertTimeToMinutes(Game1.getTrulyDarkTime((GameLocation) this));
    double num = (double) Utility.ConvertTimeToMinutes(Game1.timeOfDay) + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameMinute;
    float t1 = Utility.Clamp(((float) num - (float) minutes1) / (float) (minutes2 - minutes1), 0.0f, 1f);
    float t2 = Utility.Clamp(((float) num - (float) minutes3) / (float) (minutes4 - minutes3), 0.0f, 1f);
    Game1.ambientLight.R = (byte) Utility.Lerp((float) this._ambientLightColor.R, (float) Math.Max(this._ambientLightColor.R, Game1.isRaining ? Game1.ambientLight.R : Game1.outdoorLight.R), t1);
    Game1.ambientLight.G = (byte) Utility.Lerp((float) this._ambientLightColor.G, (float) Math.Max(this._ambientLightColor.G, Game1.isRaining ? Game1.ambientLight.G : Game1.outdoorLight.G), t1);
    Game1.ambientLight.B = (byte) Utility.Lerp((float) this._ambientLightColor.B, (float) Math.Max(this._ambientLightColor.B, Game1.isRaining ? Game1.ambientLight.B : Game1.outdoorLight.B), t1);
    Game1.ambientLight.A = (byte) Utility.Lerp((float) this._ambientLightColor.A, (float) Math.Max(this._ambientLightColor.A, Game1.isRaining ? Game1.ambientLight.A : Game1.outdoorLight.A), t1);
    Color black = Color.Black with
    {
      A = (byte) Utility.Lerp((float) byte.MaxValue, 0.0f, t2)
    };
    foreach (LightSource lightSource in Game1.currentLightSources.Values)
    {
      if (lightSource.lightContext.Value == LightSource.LightContext.MapLight)
        lightSource.color.Value = black;
    }
  }

  public override void MakeMapModifications(bool force = false)
  {
    base.MakeMapModifications(force);
    this.UpdateLostItemsShopTile();
    this.updateStatueEyes();
  }

  protected override void resetLocalState()
  {
    this._ambientLightColor = new Color(150, 120, 50);
    this.ignoreOutdoorLighting.Value = false;
    Game1.player.mailReceived.Add("beenToWoods");
    base.resetLocalState();
    this._updateWoodsLighting();
    int num1 = 25 + Utility.CreateDaySaveRandom().Next(0, 75);
    if (!this.IsRainingHere())
    {
      this.baubles = new List<Vector2>();
      for (int index = 0; index < num1; ++index)
        this.baubles.Add(new Vector2((float) Game1.random.Next(0, this.map.DisplayWidth), (float) Game1.random.Next(0, this.map.DisplayHeight)));
      Season season = this.GetSeason();
      if (season != Season.Winter)
      {
        this.weatherDebris = new List<WeatherDebris>();
        int maxValue = 192 /*0xC0*/;
        int which = 1;
        if (season == Season.Fall)
          which = 2;
        for (int index = 0; index < num1; ++index)
        {
          List<WeatherDebris> weatherDebris1 = this.weatherDebris;
          int num2 = index * maxValue;
          Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
          int width1 = viewport.Width;
          double x = (double) (num2 % width1 + Game1.random.Next(maxValue));
          int num3 = index * maxValue;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int width2 = viewport.Width;
          int num4 = num3 / width2 * maxValue;
          viewport = Game1.graphics.GraphicsDevice.Viewport;
          int height = viewport.Height;
          double y = (double) (num4 % height + Game1.random.Next(maxValue));
          WeatherDebris weatherDebris2 = new WeatherDebris(new Vector2((float) x, (float) y), which, (float) Game1.random.Next(15) / 500f, (float) Game1.random.Next(-10, 0) / 50f, (float) Game1.random.Next(10) / 50f);
          weatherDebris1.Add(weatherDebris2);
        }
      }
    }
    if (Game1.timeOfDay < 1200)
      return;
    int endTime = Utility.ModifyTime(1920, Utility.CreateDaySaveRandom(15.0).Next(390));
    int num5 = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, endTime) * Game1.realMilliSecondsPerGameMinute;
    if (num5 <= 0)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(0.0f, 0.0f), false, 0.0f, Color.White)
    {
      animationLength = 1,
      totalNumberOfLoops = 1,
      interval = (float) num5,
      endFunction = (TemporaryAnimatedSprite.endBehavior) (x =>
      {
        bool flag = true;
        foreach (Farmer farmer in this.farmers)
        {
          if ((double) farmer.position.X < 640.0 || (double) farmer.position.Y > 1280.0)
            flag = false;
        }
        if (!flag)
          return;
        foreach (LightSource lightSource in this.sharedLights.Values)
        {
          if ((double) lightSource.position.X < 1600.0 && (double) lightSource.position.Y > 1184.0)
          {
            flag = false;
            break;
          }
        }
        if (!flag)
          return;
        this.temporarySprites.Add(new TemporaryAnimatedSprite("Characters\\asldkfjsquaskutanfsldk", new Microsoft.Xna.Framework.Rectangle(0, 0, 32 /*0x20*/, 48 /*0x30*/), new Vector2(22f, 24.3f) * 64f, true, 0.0f, Color.White)
        {
          animationLength = 8,
          totalNumberOfLoops = 88,
          interval = 90f,
          motion = new Vector2(-7f, 0.0f),
          scale = 5.5f,
          layerDepth = 0.176f
        });
      })
    });
  }

  /// <summary>Add or remove the crow shop as needed.</summary>
  private void UpdateLostItemsShopTile()
  {
    IInventory itemsShopInventory = Woods.GetLostItemsShopInventory();
    itemsShopInventory.RemoveWhere<Item>((Predicate<Item>) (item => item == null || item.Stack <= 0));
    if (itemsShopInventory.HasAny())
    {
      if (this.Map.GetTileSheet("lostItemsShop") == null)
      {
        Texture2D texture2D = Game1.content.Load<Texture2D>("Characters\\Crow");
        this.map.AddTileSheet(new TileSheet("lostItemsShop", this.map, "Characters\\Crow", new Size(texture2D.Width / 16 /*0x10*/, texture2D.Height / 16 /*0x10*/), new Size(16 /*0x10*/)));
      }
      this.setAnimatedMapTile(12, 4, Enumerable.Range(0, 32 /*0x20*/).ToArray<int>(), 100L, "Front", "lostItemsShop");
      this.setAnimatedMapTile(12, 5, Enumerable.Range(32 /*0x20*/, 32 /*0x20*/).ToArray<int>(), 100L, "Buildings", "lostItemsShop", "LostItemsShop");
      for (int index = 0; index < 3; ++index)
        this.setTileProperty(11 + index, 6, "Buildings", "Action", "LostItemsShop");
      this.setMapTile(10, 4, 0, "Buildings", "untitled tile sheet");
      this.setMapTile(14, 5, 0, "Buildings", "untitled tile sheet");
    }
    else
    {
      this.removeMapTile(12, 4, "Front");
      this.removeMapTile(12, 5, "Buildings");
      for (int index = 0; index < 3; ++index)
        this.removeTileProperty(11, 6 + index, "Buildings", "Action");
      this.removeMapTile(10, 4, "Buildings");
      this.removeMapTile(14, 5, "Buildings");
    }
  }

  private void updateStatueEyes()
  {
    Layer layer = this.map.RequireLayer("Front");
    if (this.hasUnlockedStatue.Value && !this.localPlayerHasFoundStardrop())
    {
      layer.Tiles[8, 6].TileIndex = 1117;
      layer.Tiles[9, 6].TileIndex = 1118;
    }
    else
    {
      layer.Tiles[8, 6].TileIndex = 1115;
      layer.Tiles[9, 6].TileIndex = 1116;
    }
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool skipWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, skipWasUpdatedFlush);
    this.statueAnimationEvent.Poll();
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this._updateWoodsLighting();
    if (this.statueTimer > 0)
    {
      this.statueTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.statueTimer <= 0)
        this.statueAnimation(Game1.player);
    }
    if (this.baubles != null)
    {
      for (int index = 0; index < this.baubles.Count; ++index)
      {
        Vector2 vector2 = new Vector2();
        vector2.X = (float) ((double) this.baubles[index].X - (double) Math.Max(0.4f, Math.Min(1f, (float) index * 0.01f)) - (double) index * 0.0099999997764825821 * Math.Sin(2.0 * Math.PI * (double) time.TotalGameTime.Milliseconds / 8000.0));
        vector2.Y = this.baubles[index].Y + Math.Max(0.5f, Math.Min(1.2f, (float) index * 0.02f));
        if ((double) vector2.Y > (double) this.map.DisplayHeight || (double) vector2.X < 0.0)
        {
          vector2.X = (float) Game1.random.Next(0, this.map.DisplayWidth);
          vector2.Y = -64f;
        }
        this.baubles[index] = vector2;
      }
    }
    if (this.weatherDebris == null)
      return;
    foreach (WeatherDebris weatherDebri in this.weatherDebris)
      weatherDebri.update();
    Game1.updateDebrisWeatherForMovement(this.weatherDebris);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    if (this.baubles != null)
    {
      for (int index = 0; index < this.baubles.Count; ++index)
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.baubles[index]), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(346 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double) (index * 25)) % 600.0) / 150 * 5, 1971, 5, 5)), Color.White, (float) index * 0.3926991f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }
    if (this.weatherDebris == null || this.currentEvent != null)
      return;
    foreach (WeatherDebris weatherDebri in this.weatherDebris)
      weatherDebri.draw(b);
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    if (!(ArgUtility.Get(action, 0) == "LostItemsShop"))
      return base.performAction(action, who, tileLocation);
    Woods.GetLostItemShopMutex().RequestLock((Action) (() =>
    {
      if (!Utility.TryOpenShopMenu("LostItems", (string) null) || !(Game1.activeClickableMenu is ShopMenu activeClickableMenu2))
        return;
      activeClickableMenu2.behaviorBeforeCleanup = new Action<IClickableMenu>(this.OnLostItemsShopClosed);
    }));
    return true;
  }

  /// <summary>Get the items sold in the lost items shop.</summary>
  public static IInventory GetLostItemsShopInventory()
  {
    return (IInventory) Game1.player.team.GetOrCreateGlobalInventory("LostItemsShop");
  }

  /// <summary>Get the mutex which locks access to the lost items shop.</summary>
  public static NetMutex GetLostItemShopMutex()
  {
    return Game1.player.team.GetOrCreateGlobalInventoryMutex("LostItemsShop");
  }

  /// <summary>Handle the player closing the lost items shop.</summary>
  /// <param name="shopMenu">The shop menu.</param>
  private void OnLostItemsShopClosed(IClickableMenu shopMenu)
  {
    Woods.GetLostItemShopMutex().ReleaseLock();
  }
}
