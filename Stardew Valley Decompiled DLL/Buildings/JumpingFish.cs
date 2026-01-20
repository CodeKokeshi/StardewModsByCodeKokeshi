// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.JumpingFish
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using System;

#nullable disable
namespace StardewValley.Buildings;

public class JumpingFish
{
  public Vector2 startPosition;
  public Vector2 endPosition;
  protected float _age;
  public float jumpTime = 1f;
  protected FishPond _pond;
  protected StardewValley.Object _fishObject;
  protected bool _flipped;
  public Vector2 position;
  public float jumpHeight;
  public float angularVelocity;
  public float angle;

  public JumpingFish(FishPond pond, Vector2 start_position, Vector2 end_position)
  {
    this.angularVelocity = (float) ((double) Utility.RandomFloat(20f, 40f) * 3.1415927410125732 / 180.0);
    this.startPosition = start_position;
    this.endPosition = end_position;
    this.position = this.startPosition;
    this._pond = pond;
    this._fishObject = pond.GetFishObject();
    if ((double) this.startPosition.X > (double) this.endPosition.X)
      this._flipped = true;
    this.jumpHeight = Utility.RandomFloat(75f, 100f);
    this.Splash();
  }

  public void Splash()
  {
    if (this._pond == null || !Game1.currentLocation.buildings.Contains((Building) this._pond))
      return;
    Game1.playSound("dropItemInWater");
    Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.position + new Vector2(-0.5f, -0.5f) * 64f, false, false)
    {
      delayBeforeAnimationStart = 0,
      layerDepth = this.startPosition.Y / 10000f
    });
  }

  public bool Update(float time)
  {
    this._age += time;
    this.angle += this.angularVelocity * time;
    if ((double) this._age >= (double) this.jumpTime)
    {
      this._age = time;
      this.Splash();
      return true;
    }
    this.position.X = Utility.Lerp(this.startPosition.X, this.endPosition.X, this._age / this.jumpTime);
    this.position.Y = Utility.Lerp(this.startPosition.Y, this.endPosition.Y, this._age / this.jumpTime);
    return false;
  }

  public void Draw(SpriteBatch b)
  {
    float angle = this.angle;
    SpriteEffects effects = SpriteEffects.None;
    if (this._flipped)
    {
      effects = SpriteEffects.FlipHorizontally;
      angle *= -1f;
    }
    float num = 1f;
    Vector2 globalPosition = this.position + new Vector2(0.0f, (float) Math.Sin((double) this._age / (double) this.jumpTime * Math.PI) * -this.jumpHeight);
    Vector2 origin = new Vector2(8f, 8f);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this._fishObject.QualifiedItemId);
    b.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, globalPosition), new Rectangle?(dataOrErrorItem.GetSourceRect()), Color.White, angle, origin, 4f * num, effects, (float) ((double) this.position.Y / 10000.0 + 9.9999999747524271E-07));
    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.position), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.5f, 0.0f, new Vector2((float) (Game1.shadowTexture.Bounds.Width / 2), (float) (Game1.shadowTexture.Bounds.Height / 2)), 2f, effects, (float) ((double) this.position.Y / 10000.0 + 9.9999999747524271E-07));
  }
}
