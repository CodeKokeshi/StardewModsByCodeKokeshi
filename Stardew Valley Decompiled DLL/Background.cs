// Decompiled with JetBrains decompiler
// Type: StardewValley.Background
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.Locations;
using System;
using xTile.Layers;

#nullable disable
namespace StardewValley;

public class Background
{
  public int defaultChunkIndex;
  public int numChunksInSheet;
  public double chanceForDeviationFromDefault;
  protected Texture2D backgroundImage;
  protected Texture2D cloudsTexture;
  protected Vector2 position = Vector2.Zero;
  protected int chunksWide;
  protected int chunksHigh;
  protected int chunkWidth;
  protected int chunkHeight;
  protected int[] chunks;
  protected float zoom;
  public Color c;
  protected bool summitBG;
  protected bool onlyMapBG;
  public int yOffset;
  public TemporaryAnimatedSpriteList tempSprites;
  protected int initialViewportY;
  public bool cursed;
  /// <summary>The location for which to render a background.</summary>
  protected GameLocation location;

  /// <summary>constructor for summit background</summary>
  public Background(Summit location)
  {
    this.location = (GameLocation) location;
    this.summitBG = true;
    this.c = Color.White;
    this.initialViewportY = Game1.viewport.Y;
    this.cloudsTexture = Game1.content.Load<Texture2D>("Minigames\\Clouds");
  }

  public Background(GameLocation location, Color color, bool onlyMapBG)
  {
    this.location = location;
    this.c = color;
    this.onlyMapBG = onlyMapBG;
    this.tempSprites = new TemporaryAnimatedSpriteList();
  }

  public Background(
    GameLocation location,
    Texture2D bgImage,
    int seedValue,
    int chunksWide,
    int chunksHigh,
    int chunkWidth,
    int chunkHeight,
    float zoom,
    int defaultChunkIndex,
    int numChunksInSheet,
    double chanceForDeviation,
    Color c)
  {
    this.location = location;
    this.backgroundImage = bgImage;
    this.chunksWide = chunksWide;
    this.chunksHigh = chunksHigh;
    this.zoom = zoom;
    this.chunkWidth = chunkWidth;
    this.chunkHeight = chunkHeight;
    this.defaultChunkIndex = defaultChunkIndex;
    this.numChunksInSheet = numChunksInSheet;
    this.chanceForDeviationFromDefault = chanceForDeviation;
    this.c = c;
    Random random = Utility.CreateRandom((double) seedValue);
    this.chunks = new int[chunksWide * chunksHigh];
    for (int index = 0; index < chunksHigh * chunksWide; ++index)
      this.chunks[index] = random.NextDouble() >= this.chanceForDeviationFromDefault ? defaultChunkIndex : random.Next(numChunksInSheet);
  }

  public virtual void update(xTile.Dimensions.Rectangle viewport)
  {
    Layer layer = Game1.currentLocation.map.RequireLayer("Back");
    this.position.X = (float) -((double) (viewport.X + viewport.Width / 2) / ((double) layer.LayerWidth * 64.0) * ((double) (this.chunksWide * this.chunkWidth) * (double) this.zoom - (double) viewport.Width));
    this.position.Y = (float) -((double) (viewport.Y + viewport.Height / 2) / ((double) layer.LayerHeight * 64.0) * ((double) (this.chunksHigh * this.chunkHeight) * (double) this.zoom - (double) viewport.Height));
  }

  public virtual void draw(SpriteBatch b)
  {
    if (this.summitBG)
    {
      if (Game1.viewport.X <= -1000)
        return;
      Season seasonForLocation = Game1.GetSeasonForLocation(this.location);
      bool flag = seasonForLocation == Season.Winter;
      int num1;
      switch (seasonForLocation)
      {
        case Season.Fall:
          num1 = 1;
          break;
        case Season.Winter:
          num1 = 2;
          break;
        default:
          num1 = 0;
          break;
      }
      int num2 = -Game1.viewport.Y / 4 + this.initialViewportY / 4;
      float num3 = 1f;
      float num4 = 1f;
      Color color = Color.White;
      int num5 = (int) ((double) (Game1.timeOfDay - Game1.timeOfDay % 100) + (double) (Game1.timeOfDay % 100 / 10) * 16.659999847412109);
      int num6 = flag ? 30 : 0;
      if (Game1.timeOfDay >= 1800)
      {
        this.c = new Color((float) byte.MaxValue, (float) byte.MaxValue - Math.Max(100f, (float) ((double) num5 + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727 - 1800.0)), (float) byte.MaxValue - Math.Max(100f, (float) (((double) num5 + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727 - 1800.0) / 2.0)));
        color = flag ? Color.Black * 0.5f : Color.Blue * 0.5f;
        num3 = Math.Max(0.0f, Math.Min(1f, (float) ((2000.0 - ((double) num5 + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727)) / 200.0)));
        num4 = Math.Max(0.0f, Math.Min(1f, (float) ((2200.0 - ((double) num5 + (double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727)) / 400.0)));
        Game1.ambientLight = new Color((int) Utility.Lerp(0.0f, 30f, 1f - num3), (int) Utility.Lerp(0.0f, 60f, 1f - num3), (int) Utility.Lerp(0.0f, 15f, 1f - num3));
      }
      b.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(639, 858, 1, 144 /*0x90*/)), this.c * num4, 0.0f, Vector2.Zero, SpriteEffects.None, 5E-08f);
      b.Draw(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(seasonForLocation == Season.Fall ? new Microsoft.Xna.Framework.Rectangle(639, 1051, 1, 400) : new Microsoft.Xna.Framework.Rectangle(639 + (num1 + 1), 1051, 1, 400)), this.c * num3, 0.0f, Vector2.Zero, SpriteEffects.None, 1E-07f);
      if (Game1.timeOfDay >= 1800)
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height / 2 - 780)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 1453, 638, 195)), Color.White * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
      if (Game1.dayOfMonth == 28 && Game1.timeOfDay > 1900)
        b.Draw(Game1.mouseCursors, new Vector2((float) ((double) ((float) num5 + (float) ((double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727)) / 2600.0 * (double) Game1.viewport.Width / 4.0), (float) (Game1.viewport.Height / 2 + 176 /*0xB0*/) - (float) ((double) ((float) (num5 - 1900) + (float) ((double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727)) / 700.0 * (double) Game1.viewport.Height / 2.0)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(642, 834, 43, 44)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 5E-08f);
      if (!flag && (Game1.currentLocation.IsDebrisWeatherHere() || Game1.currentLocation.IsRainingHere()))
        b.Draw(this.cloudsTexture, new Vector2((float) Game1.viewport.Width - ((float) num5 + (float) ((double) Game1.gameTimeInterval / (double) Game1.realMilliSecondsPerGameTenMinutes * 16.600000381469727)) / 2600f * (float) (Game1.viewport.Width + 2048 /*0x0800*/), (float) (Game1.viewport.Height - 584 - 600 + num2 / 2 + Game1.dayOfMonth * 6)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 512 /*0x0200*/, 340)), Color.White * num3, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 5.6E-08f);
      if (!this.cursed)
      {
        b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(0, Game1.viewport.Height - 584 + num2 / 2, Game1.viewport.Width, Game1.viewport.Height / 2), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), new Color((int) ((double) num6 + 60.0 * (double) num4), (int) ((double) (num6 + 10) + 170.0 * (double) num4), (int) ((double) (num6 + 20) + 205.0 * (double) num4)), 0.0f, Vector2.Zero, SpriteEffects.None, 2E-07f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), Color.White * Math.Max((float) this.c.A, 0.5f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(2556f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), color * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 2E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), Color.White * Math.Max((float) this.c.A, 0.5f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (Game1.viewport.Height - 596 + num2)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 736 + num1 * 149, 639, 149)), color * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 2E-06f);
        foreach (TemporaryAnimatedSprite temporarySprite in Game1.currentLocation.temporarySprites)
          temporarySprite.draw(b);
        b.Draw(this.cloudsTexture, new Vector2(0.0f, (float) (Game1.viewport.Height - 568) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 554 + num1 * 153, 164, 142)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(this.cloudsTexture, new Vector2((float) (Game1.viewport.Width - 488), (float) (Game1.viewport.Height - 612) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(390, 543 + num1 * 153, 122, 153)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
        b.Draw(this.cloudsTexture, new Vector2(0.0f, (float) (Game1.viewport.Height - 568) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 554 + num1 * 153, 164, 142)), Color.Black * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        b.Draw(this.cloudsTexture, new Vector2((float) (Game1.viewport.Width - 488), (float) (Game1.viewport.Height - 612) + (float) num2 * 2f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(390, 543 + num1 * 153, 122, 153)), Color.Black * (1f - num3), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      }
      else
      {
        if (!Game1.options.screenFlash)
          return;
        Random random = new Random((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds - Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0));
        for (int index = 0; index < 20; ++index)
        {
          Texture2D texture = random.Choose<Texture2D>(Game1.mouseCursors, Game1.mouseCursors2, Game1.objectSpriteSheet, Game1.menuTexture, Game1.uncoloredMenuTexture, Game1.mouseCursors_1_6, Game1.bigCraftableSpriteSheet, Game1.cropSpriteSheet);
          b.Draw(texture, new Vector2((float) (random.Next(Game1.viewport.Width) - 100), (float) (random.Next(Game1.viewport.Height) - 100)) + new Vector2((float) (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0) * 0.03f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(random.Next(texture.Width / 16 /*0x10*/) * 16 /*0x10*/, random.Next(texture.Height / 16 /*0x10*/) * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/)), Utility.getRandomRainbowColor(random), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-06f);
        }
      }
    }
    else if (this.backgroundImage == null)
    {
      Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
      if (this.onlyMapBG)
      {
        destinationRectangle.X = Math.Max(0, -Game1.viewport.X);
        destinationRectangle.Y = Math.Max(0, -Game1.viewport.Y);
        destinationRectangle.Width = Math.Min(Game1.viewport.Width, Game1.currentLocation.map.DisplayWidth);
        destinationRectangle.Height = Math.Min(Game1.viewport.Height, Game1.currentLocation.map.DisplayHeight);
      }
      b.Draw(Game1.staminaRect, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Game1.staminaRect.Bounds), this.c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
      for (int index = this.tempSprites.Count - 1; index >= 0; --index)
      {
        if (this.tempSprites[index].update(Game1.currentGameTime))
          this.tempSprites.RemoveAt(index);
        else
          this.tempSprites[index].draw(b);
      }
    }
    else
    {
      Vector2 zero = Vector2.Zero;
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, this.chunkWidth, this.chunkHeight);
      for (int index = 0; index < this.chunks.Length; ++index)
      {
        zero.X = this.position.X + (float) (index * this.chunkWidth % (this.chunksWide * this.chunkWidth)) * this.zoom;
        zero.Y = this.position.Y + (float) (index * this.chunkWidth / (this.chunksWide * this.chunkWidth) * this.chunkHeight) * this.zoom;
        if (this.backgroundImage == null)
        {
          b.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) zero.X, (int) zero.Y, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle?(rectangle), this.c, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }
        else
        {
          rectangle.X = this.chunks[index] * this.chunkWidth % this.backgroundImage.Width;
          rectangle.Y = this.chunks[index] * this.chunkWidth / this.backgroundImage.Width * this.chunkHeight;
          b.Draw(this.backgroundImage, zero, new Microsoft.Xna.Framework.Rectangle?(rectangle), this.c, 0.0f, Vector2.Zero, this.zoom, SpriteEffects.None, 0.0f);
        }
      }
    }
  }
}
