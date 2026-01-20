// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Tent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class Tent : LargeTerrainFeature
{
  public readonly NetInt health = new NetInt(5);
  private int invincTimer;
  private Vector2 shakeOffset;
  private bool goingToSleep;
  public static Vector2 lastTentTouchedByPlayer = Vector2.Zero;

  public Tent()
    : base(true)
  {
    this.isDestroyedByNPCTrample = true;
  }

  public override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.health, "health");
  }

  public Tent(Vector2 tileLocation)
    : base(true)
  {
    this.Tile = tileLocation;
    this.isDestroyedByNPCTrample = true;
  }

  public override Rectangle getBoundingBox()
  {
    Vector2 tile = this.Tile;
    return new Rectangle((int) ((double) tile.X - 1.0) * 64 /*0x40*/, (int) ((double) tile.Y - 1.0) * 64 /*0x40*/, 192 /*0xC0*/, 128 /*0x80*/);
  }

  /// <inheritdoc />
  public override bool isPassable(Character c = null) => c != null;

  public override bool performToolAction(Tool t, int damage, Vector2 tileLocation)
  {
    if (this.invincTimer <= 0)
    {
      --this.health.Value;
      this.invincTimer = 400;
      Game1.playSound("weed_cut");
    }
    return base.performToolAction(t, damage, tileLocation);
  }

  public override void dayUpdate()
  {
    this.health.Value = 0;
    Game1.displayFarmer = true;
    base.dayUpdate();
  }

  public override bool performUseAction(Vector2 tileLocation)
  {
    Vector2 tile = this.Tile;
    Vector2 grabTile = Game1.player.GetGrabTile();
    if ((grabTile == tile || (double) grabTile.X == (double) tile.X && (double) grabTile.Y >= (double) tile.Y) && !Game1.newDay && Game1.shouldTimePass() && Game1.player.hasMoved && !Game1.player.passedOut)
    {
      Tent.lastTentTouchedByPlayer = tile;
      this.Location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep"), this.Location.createYesNoResponses(), "SleepTent", (Object) null);
    }
    return base.performUseAction(tileLocation);
  }

  public override void onDestroy()
  {
    GameLocation location = this.Location;
    Vector2 tile = this.Tile;
    Game1.playSound("cut");
    Utility.addDirtPuffs(location, (int) tile.X - 1, (int) tile.Y - 1, 3, 2, 3);
    for (int index = 0; index < 16 /*0x10*/; ++index)
      location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors_1_6", new Rectangle(112 /*0x70*/ + Game1.random.Next(4) * 8, 248, 8, 8), 9999f, 1, 1, Utility.getRandomPositionInThisRectangle(this.getBoundingBox(), Game1.random), false, false, (float) ((double) tile.Y * 64.0 / 10000.0), 0.02f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        motion = new Vector2((float) Game1.random.Next(-1, 2), -5f),
        acceleration = new Vector2(0.0f, 0.16f)
      });
  }

  public override bool tickUpdate(GameTime time)
  {
    if (this.invincTimer > 0)
    {
      this.invincTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
      this.shakeOffset = new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      if (this.invincTimer <= 0)
        this.shakeOffset = Vector2.Zero;
    }
    if (this.health.Value > 0 || this.goingToSleep)
      return base.tickUpdate(time);
    this.onDestroy();
    return true;
  }

  public override void draw(SpriteBatch spriteBatch)
  {
    Vector2 tile = this.Tile;
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(tile * 64f + new Vector2(-2f, -1f) * 64f), new Rectangle?(new Rectangle(48 /*0x30*/, 208 /*0xD0*/, 64 /*0x40*/, 48 /*0x30*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
    spriteBatch.Draw(Game1.mouseCursors_1_6, Game1.GlobalToLocal(tile * 64f + new Vector2(-1f, -3f) * 64f) + this.shakeOffset, new Rectangle?(new Rectangle(0, 192 /*0xC0*/, 48 /*0x30*/, 64 /*0x40*/)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) tile.Y * 64.0 / 10000.0));
  }
}
