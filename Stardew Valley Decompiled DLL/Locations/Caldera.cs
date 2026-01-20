// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.Caldera
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class Caldera : IslandLocation
{
  [XmlIgnore]
  public Texture2D mapBaseTilesheet;
  [XmlElement("visited")]
  public NetBool visited = new NetBool();

  public Caldera()
  {
  }

  public Caldera(string filename, string locationName)
    : base(filename, locationName)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.visited, "visited");
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (!this.visited.Value)
      this.visited.Value = true;
    if (!Game1.MasterPlayer.hasOrWillReceiveMail("reachedCaldera"))
      Game1.addMailForTomorrow("reachedCaldera", true, true);
    this.mapBaseTilesheet = Game1.temporaryContent.Load<Texture2D>(this.map.RequireTileSheet(0, "dungeon").ImageSource);
    this.waterColor.Value = Color.White;
    if (Game1.player.mailReceived.Contains("CalderaTreasure"))
      this.overlayObjects.Remove(new Vector2(25f, 28f));
    else if (!this.objects.ContainsKey(new Vector2(25f, 28f)))
    {
      Chest chest = new Chest(false, "227");
      chest.addItem(ItemRegistry.Create("(O)74"));
      chest.synchronized.Value = false;
      chest.type.Value = "interactive";
      chest.Fragility = 2;
      chest.SetBigCraftableSpriteIndex(227);
      this.overlayObjects.Add(new Vector2(25f, 28f), (Object) chest);
    }
    if (!Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") || Game1.player.mailReceived.Contains("gotCAMask"))
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\critters", new Microsoft.Xna.Framework.Rectangle(15, 333, 13, 12), new Vector2(908.8f, 1792f), false, 0.0f, Color.White)
    {
      scale = 4f,
      interval = 99999f,
      totalNumberOfLoops = 99999,
      yPeriodic = true,
      yPeriodicRange = 2f,
      yPeriodicLoopTime = 2500f
    });
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    this.critters = new List<Critter>();
    if (Game1.random.NextDouble() < 0.17)
      this.addCritter((Critter) new CalderaMonkey(new Vector2(12f, 21.3f) * 64f));
    if (Game1.random.NextDouble() < 0.17)
      this.addCritter((Critter) new CalderaMonkey(new Vector2(33f, 21.3f) * 64f));
    if (Game1.random.NextDouble() >= 0.17)
      return;
    this.addCritter((Critter) new CalderaMonkey(new Vector2(18f, 17.3f) * 64f));
  }

  public override bool CanRefillWateringCanOnTile(int tileX, int tileY) => false;

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    if (!this.visited.Value || Game1.player.hasOrWillReceiveMail("volcanoShortcutUnlocked"))
      return;
    Game1.addMailForTomorrow("volcanoShortcutUnlocked", true);
  }

  public override void drawWaterTile(SpriteBatch b, int x, int y)
  {
    int num1 = y == this.map.Layers[0].LayerHeight - 1 ? 1 : (!this.waterTiles[x, y + 1] ? 1 : 0);
    bool flag = y == 0 || !this.waterTiles[x, y - 1];
    int num2 = 0;
    int num3 = 320;
    b.Draw(this.mapBaseTilesheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - (!flag ? (int) this.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 + this.waterAnimationIndex * 16 /*0x10*/, num3 + ((x + y) % 2 == 0 ? (this.waterTileFlip ? 32 /*0x20*/ : 0) : (this.waterTileFlip ? 0 : 32 /*0x20*/)) + (flag ? (int) this.waterPosition / 4 : 0), 16 /*0x10*/, 16 /*0x10*/ + (flag ? (int) -(double) this.waterPosition / 4 : 0))), this.waterColor.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
    if (num1 == 0)
      return;
    b.Draw(this.mapBaseTilesheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) ((y + 1) * 64 /*0x40*/ - (int) this.waterPosition))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(num2 + this.waterAnimationIndex * 16 /*0x10*/, num3 + ((x + (y + 1)) % 2 == 0 ? (this.waterTileFlip ? 32 /*0x20*/ : 0) : (this.waterTileFlip ? 0 : 32 /*0x20*/)), 16 /*0x10*/, 16 /*0x10*/ - (int) (16.0 - (double) this.waterPosition / 4.0) - 1)), this.waterColor.Value, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && !Game1.player.mailReceived.Contains("gotCAMask") && tileLocation.X == 14 && tileLocation.Y == 28)
    {
      Game1.playSound("monkey1");
      who.addItemByMenuIfNecessaryElseHoldUp(ItemRegistry.Create("(H)92"));
      Game1.player.mailReceived.Add("gotCAMask");
    }
    switch (this.getTileIndexAt(tileLocation, "Buildings", "untitled tile sheet"))
    {
      case 123:
      case 124:
      case 133:
      case 134:
      case 156:
      case 157:
        Game1.activeClickableMenu = (IClickableMenu) new ForgeMenu();
        return true;
      default:
        return base.checkAction(tileLocation, viewport, who);
    }
  }

  public override bool isActionableTile(int xTile, int yTile, Farmer who)
  {
    return yTile == 21 && (xTile == 22 || xTile == 23) || Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && !Game1.player.mailReceived.Contains("gotCAMask") && xTile == 14 && yTile == 28 || base.isActionableTile(xTile, yTile, who);
  }

  public override void drawBackground(SpriteBatch b)
  {
    base.drawBackground(b);
    this.DrawParallaxHorizon(b, false);
  }

  public override bool performToolAction(Tool t, int tileX, int tileY)
  {
    if (t is WateringCan && this.isTileOnMap(new Vector2((float) tileX, (float) tileY)) && this.waterTiles[tileX, tileY])
    {
      for (int index = 0; index < 10; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1965, 8, 8), new Vector2((float) tileX + 0.5f, (float) tileY + 0.5f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-16, 16 /*0x10*/)), false, 0.02f, Color.White)
        {
          scale = 3f,
          animationLength = 7,
          totalNumberOfLoops = 10,
          interval = 90f,
          motion = new Vector2((float) Game1.random.Next(-10, 11) / 8f, -3f),
          acceleration = new Vector2(0.0f, 0.08f),
          delayBeforeAnimationStart = index * 50
        });
      for (int index = 0; index < 5; ++index)
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) tileX, (float) tileY - 0.5f) * 64f + new Vector2((float) Game1.random.Next(64 /*0x40*/), (float) Game1.random.Next(64 /*0x40*/)), false, 0.007f, Color.White)
        {
          alpha = 0.75f,
          motion = new Vector2(0.0f, -1f),
          acceleration = new Vector2(1f / 500f, 0.0f),
          interval = 99999f,
          layerDepth = 1f,
          scale = 4f,
          scaleChange = 0.02f,
          rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
          delayBeforeAnimationStart = index * 35
        });
      DelayedAction.playSoundAfterDelay("fireball", 200);
      Game1.playSound("steam");
    }
    return base.performToolAction(t, tileX, tileY);
  }
}
