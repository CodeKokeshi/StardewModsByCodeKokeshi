// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.FarmCave
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;

#nullable disable
namespace StardewValley.Locations;

public class FarmCave : GameLocation
{
  public FarmCave()
  {
  }

  public FarmCave(string map, string name)
    : base(map, name)
  {
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.MasterPlayer.caveChoice.Value != 1)
      return;
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(0.0f, 0.0f), false, 0.0f, Color.White)
    {
      interval = 3000f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      layerDepth = 1f,
      lightId = "FarmCave_1",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(8f, 0.0f), false, 0.0f, Color.White)
    {
      interval = 3000f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(320f, -64f), false, 0.0f, Color.White)
    {
      interval = 2000f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 500,
      layerDepth = 1f,
      lightId = "FarmCave_2",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(328f, -64f), false, 0.0f, Color.White)
    {
      interval = 2000f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 500,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(128f, (float) (this.map.Layers[0].LayerHeight * 64 /*0x40*/ - 64 /*0x40*/)), false, 0.0f, Color.White)
    {
      interval = 1600f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 250,
      layerDepth = 1f,
      lightId = "FarmCave_3",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(136f, (float) (this.map.Layers[0].LayerHeight * 64 /*0x40*/ - 64 /*0x40*/)), false, 0.0f, Color.White)
    {
      interval = 1600f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 250,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 /*0x40*/ + 4), 192f), false, 0.0f, Color.White)
    {
      interval = 2800f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f,
      lightId = "FarmCave_4",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 /*0x40*/ + 12), 192f), false, 0.0f, Color.White)
    {
      interval = 2800f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 /*0x40*/ + 4), 576f), false, 0.0f, Color.White)
    {
      interval = 2200f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f,
      lightId = "FarmCave_5",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2((float) ((this.map.Layers[0].LayerWidth + 1) * 64 /*0x40*/ + 12), 576f), false, 0.0f, Color.White)
    {
      interval = 2200f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-60f, 128f), false, 0.0f, Color.White)
    {
      interval = 2600f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f,
      lightId = "FarmCave_6",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-52f, 128f), false, 0.0f, Color.White)
    {
      interval = 2600f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 750,
      layerDepth = 1f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-64f, 384f), false, 0.0f, Color.White)
    {
      interval = 3400f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 650,
      layerDepth = 1f,
      lightId = "FarmCave_7",
      lightRadius = 0.5f
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(374, 358, 1, 1), new Vector2(-52f, 384f), false, 0.0f, Color.White)
    {
      interval = 3400f,
      animationLength = 3,
      totalNumberOfLoops = 99999,
      scale = 4f,
      delayBeforeAnimationStart = 650,
      layerDepth = 1f
    });
    Game1.ambientLight = new Color(70, 90, 0);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    if (Game1.MasterPlayer.caveChoice.Value != 1)
      return;
    if (Game1.random.NextDouble() < 0.002 && Game1.currentLocation == this)
    {
      this.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(640, 1664, 16 /*0x10*/, 16 /*0x10*/), 80f, 4, 9999, new Vector2((float) Game1.random.Next(this.map.Layers[0].LayerWidth), (float) this.map.Layers[0].LayerHeight) * 64f, false, false, 1f, 0.0f, Color.Black, 4f, 0.0f, 0.0f, 0.0f)
      {
        xPeriodic = true,
        xPeriodicLoopTime = 2000f,
        xPeriodicRange = 64f,
        motion = new Vector2(0.0f, -8f)
      });
      if (Game1.random.NextDouble() < 0.15 && Game1.currentLocation == this)
        this.localSound("batScreech");
      for (int index = 1; index < 5; ++index)
        DelayedAction.playSoundAfterDelay("batFlap", 320 * index - 80 /*0x50*/);
    }
    else
    {
      if (Game1.random.NextDouble() >= 0.005)
        return;
      this.temporarySprites.Add((TemporaryAnimatedSprite) new BatTemporarySprite(new Vector2(Game1.random.NextBool() ? 0.0f : (float) (this.map.DisplayWidth - 64 /*0x40*/), (float) (this.map.DisplayHeight - 64 /*0x40*/))));
    }
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
  }

  public override void performTenMinuteUpdate(int timeOfDay)
  {
    if (Game1.currentLocation == this)
      this.UpdateReadyFlag();
    base.performTenMinuteUpdate(timeOfDay);
  }

  public override void cleanupBeforePlayerExit()
  {
    base.cleanupBeforePlayerExit();
    this.UpdateReadyFlag();
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    if (Game1.MasterPlayer.caveChoice.Value == 1)
    {
      while (Game1.random.NextDouble() < 0.66)
      {
        string str;
        switch (Game1.random.Next(5))
        {
          case 0:
            str = "296";
            break;
          case 1:
            str = "396";
            break;
          case 2:
            str = "406";
            break;
          case 3:
            str = "410";
            break;
          default:
            str = Game1.random.NextDouble() < 0.1 ? "613" : Game1.random.Next(634, 639).ToString();
            break;
        }
        Vector2 vector2 = new Vector2((float) Game1.random.Next(1, this.map.Layers[0].LayerWidth - 1), (float) Game1.random.Next(1, this.map.Layers[0].LayerHeight - 4));
        Object o = ItemRegistry.Create<Object>("(O)" + str);
        o.IsSpawnedObject = true;
        if (this.CanItemBePlacedHere(vector2))
          this.setObject(vector2, o);
      }
    }
    this.UpdateReadyFlag();
  }

  public virtual void UpdateReadyFlag()
  {
    bool flag = false;
    foreach (Object @object in this.objects.Values)
    {
      if (@object.isSpawnedObject.Value)
      {
        flag = true;
        break;
      }
      if (@object.bigCraftable.Value && @object.heldObject.Value != null && @object.MinutesUntilReady <= 0 && @object.QualifiedItemId == "(BC)128")
      {
        flag = true;
        break;
      }
    }
    Game1.getFarm().farmCaveReady.Value = flag;
  }

  public void setUpMushroomHouse()
  {
    int[] numArray1 = new int[2]{ 5, 7 };
    foreach (int y in numArray1)
    {
      int[] numArray2 = new int[3]{ 4, 6, 8 };
      foreach (int x in numArray2)
      {
        Object o = ItemRegistry.Create<Object>("(BC)128");
        o.fragility.Value = 2;
        this.setObject(new Vector2((float) x, (float) y), o);
      }
    }
    this.setObject(new Vector2(10f, 5f), ItemRegistry.Create<Object>("(BC)Dehydrator"));
  }
}
