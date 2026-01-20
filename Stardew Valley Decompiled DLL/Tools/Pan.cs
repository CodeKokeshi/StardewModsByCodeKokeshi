// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Pan
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Constants;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class Pan : Tool
{
  [XmlIgnore]
  private readonly NetEvent0 finishEvent = new NetEvent0();

  public Pan()
    : base("Copper Pan", 1, 12, 12, false)
  {
  }

  public Pan(int upgradeLevel)
    : base("Copper Pan", upgradeLevel, 12, 12, false)
  {
  }

  /// <inheritdoc />
  protected override Item GetOneNew()
  {
    if (this.upgradeLevel.Value == -1)
      this.UpgradeLevel = 1;
    return (Item) new Pan(this.UpgradeLevel);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.finishEvent, "finishEvent");
    this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
  }

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    if (this.upgradeLevel.Value <= 0)
      this.UpgradeLevel = 1;
    this.CurrentParentTileIndex = 12;
    this.IndexOfMenuItemView = 12;
    int num = 4;
    if (this.hasEnchantmentOfType<ReachingToolEnchantment>())
      ++num;
    bool flag = false;
    Rectangle rectangle = new Rectangle(location.orePanPoint.X * 64 /*0x40*/ - (int) (64.0 * ((double) num / 2.0)), location.orePanPoint.Y * 64 /*0x40*/ - (int) (64.0 * ((double) num / 2.0)), 64 /*0x40*/ * num, 64 /*0x40*/ * num);
    Point standingPixel = who.StandingPixel;
    if (rectangle.Contains(x, y) && (double) Utility.distance((float) standingPixel.X, (float) rectangle.Center.X, (float) standingPixel.Y, (float) rectangle.Center.Y) <= (double) (num * 64 /*0x40*/))
      flag = true;
    who.lastClick = Vector2.Zero;
    x = (int) who.GetToolLocation().X;
    y = (int) who.GetToolLocation().Y;
    who.lastClick = new Vector2((float) x, (float) y);
    if ((NetFieldBase<Point, NetPoint>) location.orePanPoint != (NetPoint) null && !location.orePanPoint.Equals((object) Point.Zero))
    {
      Rectangle boundingBox = who.GetBoundingBox();
      if (flag || boundingBox.Intersects(rectangle))
      {
        who.faceDirection(2);
        who.FarmerSprite.animateOnce(303, 50f, 4);
        return true;
      }
    }
    who.forceCanMove();
    return true;
  }

  public static void playSlosh(Farmer who) => who.playNearbySoundLocal("slosh");

  public override void tickUpdate(GameTime time, Farmer who)
  {
    this.lastUser = who;
    base.tickUpdate(time, who);
    this.finishEvent.Poll();
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    Vector2 toolLocation = who.GetToolLocation();
    x = (int) toolLocation.X;
    y = (int) toolLocation.Y;
    this.CurrentParentTileIndex = 12;
    this.IndexOfMenuItemView = 12;
    location.localSound("coin", new Vector2?(toolLocation / 64f));
    who.addItemsByMenuIfNecessary(this.getPanItems(location, who));
    location.orePanPoint.Value = Point.Zero;
    int num = 0;
    while (num < this.upgradeLevel.Value - 1 && !location.performOrePanTenMinuteUpdate(Game1.random) && (Game1.random.NextDouble() >= 0.5 || !location.performOrePanTenMinuteUpdate(Game1.random) || location is IslandNorth))
      ++num;
    this.finish();
  }

  private void finish() => this.finishEvent.Fire();

  private void doFinish()
  {
    this.lastUser.CanMove = true;
    this.lastUser.UsingTool = false;
    this.lastUser.canReleaseTool = true;
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
    this.IndexOfMenuItemView = 12;
    base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
  }

  public List<Item> getPanItems(GameLocation location, Farmer who)
  {
    List<Item> panItems = new List<Item>();
    string itemId1 = "378";
    int num1 = (int) who.stats.Increment("TimesPanned", 1);
    Random random = Utility.CreateRandom((double) location.orePanPoint.X, (double) location.orePanPoint.Y * 1000.0, (double) Game1.stats.DaysPlayed, (double) (who.stats.Get("TimesPanned") * 77U));
    double num2 = random.NextDouble() - (double) who.luckLevel.Value * 0.001 - who.DailyLuck - (double) (this.upgradeLevel.Value - 1) * 0.05;
    if (num2 < 0.01)
      itemId1 = "386";
    else if (num2 < 0.241)
      itemId1 = "384";
    else if (num2 < 0.6)
      itemId1 = "380";
    if (itemId1 != "386" && random.NextDouble() < 0.1 + (this.hasEnchantmentOfType<ArchaeologistEnchantment>() ? 0.1 : 0.0))
      itemId1 = "881";
    int num3 = random.Next(2, 7) + 1 + (int) ((random.NextDouble() + 0.1 + (double) who.luckLevel.Value / 10.0 + who.DailyLuck) * 2.0);
    int initialStack1 = random.Next(5) + 1 + (int) ((random.NextDouble() + 0.1 + (double) who.luckLevel.Value / 10.0) * 2.0);
    int initialStack2 = num3 + (this.upgradeLevel.Value - 1);
    double num4 = random.NextDouble() - who.DailyLuck;
    int num5 = this.upgradeLevel.Value;
    bool flag = false;
    double num6 = (double) (this.upgradeLevel.Value - 1) * 0.04;
    if (this.enchantments.Count > 0)
      num6 *= 1.25;
    if (this.hasEnchantmentOfType<GenerousEnchantment>())
      num5 += 2;
    for (; random.NextDouble() - who.DailyLuck < 0.4 + (double) who.LuckLevel * 0.04 + num6 && num5 > 0; --num5)
    {
      double num7 = random.NextDouble() - who.DailyLuck - (double) (this.upgradeLevel.Value - 1) * 0.005;
      string itemId2 = "382";
      if (num7 < 0.02 + (double) who.LuckLevel * 0.002 && random.NextDouble() < 0.75)
      {
        itemId2 = "72";
        initialStack1 = 1;
      }
      else if (num7 < 0.1 && random.NextDouble() < 0.75)
      {
        itemId2 = (60 + random.Next(5) * 2).ToString();
        initialStack1 = 1;
      }
      else if (num7 < 0.36)
      {
        itemId2 = "749";
        initialStack1 = Math.Max(1, initialStack1 / 2);
      }
      else if (num7 < 0.5)
      {
        itemId2 = random.Choose<string>("82", "84", "86");
        initialStack1 = 1;
      }
      if (num7 < (double) who.LuckLevel * 0.002 && !flag && random.NextDouble() < 0.33)
      {
        panItems.Add((Item) new Ring("859"));
        flag = true;
      }
      if (num7 < 0.01 && random.NextDouble() < 0.5)
        panItems.Add(Utility.getRandomCosmeticItem(random));
      if (random.NextDouble() < 0.1 && this.hasEnchantmentOfType<FisherEnchantment>())
      {
        Item fish = location.getFish(1f, (string) null, random.Next(1, 6), who, 0.0, who.Tile);
        if (fish != null && fish.Category == -4)
          panItems.Add(fish);
      }
      if (random.NextDouble() < 0.02 + (this.hasEnchantmentOfType<ArchaeologistEnchantment>() ? 0.05 : 0.0))
      {
        Item fromThisLocation = location.tryGetRandomArtifactFromThisLocation(who, random);
        if (fromThisLocation != null)
          panItems.Add(fromThisLocation);
      }
      if (Utility.tryRollMysteryBox(0.05, random))
        panItems.Add(ItemRegistry.Create(Game1.player.stats.Get(StatKeys.Mastery(2)) > 0U ? "(O)GoldenMysteryBox" : "(O)MysteryBox"));
      if (itemId2 != null)
        panItems.Add((Item) new StardewValley.Object(itemId2, initialStack1));
    }
    int amount = 0;
    while (random.NextDouble() < 0.05 + (this.hasEnchantmentOfType<ArchaeologistEnchantment>() ? 0.15 : 0.0))
      ++amount;
    if (amount > 0)
      panItems.Add(ItemRegistry.Create("(O)275", amount));
    panItems.Add((Item) new StardewValley.Object(itemId1, initialStack2));
    if (!(location is IslandNorth islandNorth))
    {
      if (location is IslandLocation && random.NextDouble() < 0.2)
        panItems.Add(ItemRegistry.Create("(O)831", random.Next(2, 6)));
    }
    else if (islandNorth.bridgeFixed.Value && random.NextDouble() < 0.2)
      panItems.Add(ItemRegistry.Create("(O)822"));
    if (who != null)
    {
      who.gainExperience(3, initialStack2 + initialStack1);
      who.gainExperience(2, panItems.Count * 7);
    }
    return panItems;
  }
}
