// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.SoundInTheNightEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using xTile.Layers;

#nullable disable
namespace StardewValley.Events;

public class SoundInTheNightEvent : BaseFarmEvent
{
  public const int cropCircle = 0;
  public const int meteorite = 1;
  public const int dogs = 2;
  public const int owl = 3;
  public const int earthquake = 4;
  public const int raccoonStump = 5;
  private readonly NetInt behavior = new NetInt();
  private float timer;
  private float timeUntilText = 7000f;
  private string soundName;
  private string message;
  private bool playedSound;
  private bool showedMessage;
  private bool finished;
  private Vector2 targetLocation;
  private Building targetBuilding;

  public SoundInTheNightEvent()
    : this(0)
  {
  }

  public SoundInTheNightEvent(int which) => this.behavior.Value = which;

  /// <inheritdoc />
  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.behavior, "behavior");
  }

  /// <inheritdoc />
  public override bool setUp()
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
    Farm farm = Game1.getFarm();
    farm.updateMap();
    this.timer = 0.0f;
    switch (this.behavior.Value)
    {
      case 0:
        this.soundName = "UFO";
        this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_UFO");
        int num1 = 50;
        Layer layer1 = farm.map.RequireLayer("Back");
        for (; num1 > 0; --num1)
        {
          this.targetLocation = new Vector2((float) random.Next(5, layer1.LayerWidth - 4), (float) random.Next(5, layer1.LayerHeight - 4));
          if (farm.CanItemBePlacedHere(this.targetLocation))
            break;
        }
        if (num1 <= 0)
          return true;
        break;
      case 1:
        this.soundName = "Meteorite";
        this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_Meteorite");
        Layer layer2 = farm.map.RequireLayer("Back");
        this.targetLocation = new Vector2((float) random.Next(5, layer2.LayerWidth - 20), (float) random.Next(5, layer2.LayerHeight - 4));
        for (int x = (int) this.targetLocation.X; (double) x <= (double) this.targetLocation.X + 1.0; ++x)
        {
          for (int y = (int) this.targetLocation.Y; (double) y <= (double) this.targetLocation.Y + 1.0; ++y)
          {
            Vector2 tile = new Vector2((float) x, (float) y);
            if (!farm.isTileOpenBesidesTerrainFeatures(tile) || !farm.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X + 1f, tile.Y)) || !farm.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X + 1f, tile.Y - 1f)) || !farm.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X, tile.Y - 1f)) || farm.isWaterTile((int) tile.X, (int) tile.Y) || farm.isWaterTile((int) tile.X + 1, (int) tile.Y))
              return true;
          }
        }
        break;
      case 2:
        this.soundName = "dogs";
        if (random.NextBool())
          return true;
        foreach (Building building in farm.buildings)
        {
          if (building.GetIndoors() is AnimalHouse indoors && !building.animalDoorOpen.Value && indoors.animalsThatLiveHere.Count > indoors.animals.Length && random.NextDouble() < 1.0 / (double) farm.buildings.Count)
          {
            this.targetBuilding = building;
            break;
          }
        }
        return this.targetBuilding == null;
      case 3:
        this.soundName = "owl";
        int num2 = 50;
        Layer layer3 = farm.map.RequireLayer("Back");
        for (; num2 > 0; --num2)
        {
          this.targetLocation = new Vector2((float) random.Next(5, layer3.LayerWidth - 4), (float) random.Next(5, layer3.LayerHeight - 4));
          if (farm.CanItemBePlacedHere(this.targetLocation))
            break;
        }
        if (num2 <= 0)
          return true;
        break;
      case 4:
        this.soundName = "thunder_small";
        this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_Earthquake");
        break;
      case 5:
        this.soundName = "windstorm";
        this.message = Game1.content.LoadString("Strings\\1_6_Strings:windstorm");
        this.timeUntilText = 14000f;
        Game1.player.mailReceived.Add("raccoonTreeFallen");
        break;
    }
    Game1.freezeControls = true;
    return false;
  }

  /// <inheritdoc />
  public override bool tickUpdate(GameTime time)
  {
    this.timer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.timer > 1500.0 && !this.playedSound)
    {
      if (!string.IsNullOrEmpty(this.soundName))
      {
        Game1.playSound(this.soundName);
        this.playedSound = true;
      }
      if (!this.playedSound && this.message != null)
      {
        Game1.drawObjectDialogue(this.message);
        Game1.globalFadeToClear();
        this.showedMessage = true;
        if (this.message == null)
          this.finished = true;
        else
          Game1.afterDialogues = (Game1.afterFadeFunction) (() => this.finished = true);
      }
    }
    if ((double) this.timer > (double) this.timeUntilText && !this.showedMessage)
    {
      Game1.pauseThenMessage(10, this.message);
      this.showedMessage = true;
      if (this.message == null)
        this.finished = true;
      else
        Game1.afterDialogues = (Game1.afterFadeFunction) (() => this.finished = true);
    }
    if (!this.finished)
      return false;
    Game1.freezeControls = false;
    return true;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    SpriteBatch spriteBatch = b;
    Texture2D staminaRect = Game1.staminaRect;
    Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
    int width = viewport.Width;
    viewport = Game1.graphics.GraphicsDevice.Viewport;
    int height = viewport.Height;
    Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
    Color black = Color.Black;
    spriteBatch.Draw(staminaRect, destinationRectangle, black);
    if (this.showedMessage)
      return;
    b.Draw(Game1.mouseCursors_1_6, new Vector2(12f, (float) (Game1.viewport.Height - 12 - 76)), new Rectangle?(new Rectangle(256 /*0x0100*/ + (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 600.0 / 100.0) * 19, 413, 19, 19)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
  }

  /// <inheritdoc />
  public override void makeChangesToLocation()
  {
    if (!Game1.IsMasterGame)
      return;
    Farm farm = Game1.getFarm();
    switch (this.behavior.Value)
    {
      case 0:
        StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>("(BC)96");
        @object.MinutesUntilReady = 24000 - Game1.timeOfDay;
        farm.objects.Add(this.targetLocation, @object);
        break;
      case 1:
        farm.terrainFeatures.Remove(this.targetLocation);
        farm.terrainFeatures.Remove(this.targetLocation + new Vector2(1f, 0.0f));
        farm.terrainFeatures.Remove(this.targetLocation + new Vector2(1f, 1f));
        farm.terrainFeatures.Remove(this.targetLocation + new Vector2(0.0f, 1f));
        farm.resourceClumps.Add(new ResourceClump(622, 2, 2, this.targetLocation));
        break;
      case 2:
        AnimalHouse indoors = (AnimalHouse) this.targetBuilding.GetIndoors();
        long key1 = 0;
        foreach (long key2 in (NetList<long, NetLong>) indoors.animalsThatLiveHere)
        {
          if (!indoors.animals.ContainsKey(key2))
          {
            key1 = key2;
            break;
          }
        }
        if (!Game1.getFarm().animals.Remove(key1))
          break;
        indoors.animalsThatLiveHere.Remove(key1);
        using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = Game1.getFarm().animals.Pairs.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Value.moodMessage.Value = 5;
          break;
        }
      case 3:
        farm.objects.Add(this.targetLocation, ItemRegistry.Create<StardewValley.Object>("(BC)95"));
        break;
    }
  }
}
