// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.MoneyDial
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class MoneyDial
{
  public const int digitHeight = 8;
  public int numDigits;
  public int currentValue;
  public int previousTargetValue;
  public TemporaryAnimatedSpriteList animations = new TemporaryAnimatedSpriteList();
  private int speed;
  private int soundTimer;
  private int moneyMadeAccumulator;
  private int moneyShineTimer;
  private bool playSounds = true;
  public Action<int> onPlaySound;
  /// <summary>Whether to shake the money display in <see cref="F:StardewValley.Game1.dayTimeMoneyBox" /> when the money amount changes.</summary>
  public bool ShouldShakeMainMoneyBox = true;

  public MoneyDial(int numDigits, bool playSound = true)
  {
    this.numDigits = numDigits;
    this.playSounds = playSound;
    this.currentValue = 0;
    if (Game1.player != null)
      this.currentValue = Game1.player.Money;
    this.onPlaySound = new Action<int>(this.playDefaultSound);
  }

  public void playDefaultSound(int direction)
  {
    if (direction <= 0)
      return;
    Game1.playSound("moneyDial");
  }

  public void draw(SpriteBatch b, Vector2 position, int target)
  {
    if (this.previousTargetValue != target)
    {
      this.speed = (target - this.currentValue) / 100;
      this.previousTargetValue = target;
      this.soundTimer = Math.Max(6, 100 / (Math.Abs(this.speed) + 1));
    }
    if (this.moneyShineTimer > 0 && this.currentValue == target)
      this.moneyShineTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
    if (this.moneyMadeAccumulator > 0)
    {
      this.moneyMadeAccumulator -= (Math.Abs(this.speed / 2) + 1) * (this.animations.Count <= 0 ? 100 : 1);
      if (this.moneyMadeAccumulator <= 0)
        this.moneyShineTimer = this.numDigits * 60;
    }
    if (this.ShouldShakeMainMoneyBox && this.moneyMadeAccumulator > 2000)
      Game1.dayTimeMoneyBox.moneyShakeTimer = 100;
    if (this.currentValue != target)
    {
      this.currentValue += this.speed + (this.currentValue < target ? 1 : -1);
      if (this.currentValue < target)
        this.moneyMadeAccumulator += Math.Abs(this.speed);
      --this.soundTimer;
      if (Math.Abs(target - this.currentValue) <= this.speed + 1 || this.speed != 0 && Math.Sign(target - this.currentValue) != Math.Sign(this.speed))
        this.currentValue = target;
      if (this.soundTimer <= 0)
      {
        if (this.playSounds)
        {
          Action<int> onPlaySound = this.onPlaySound;
          if (onPlaySound != null)
            onPlaySound(Math.Sign(target - this.currentValue));
        }
        this.soundTimer = Math.Max(6, 100 / (Math.Abs(this.speed) + 1));
        if (Game1.random.NextDouble() < 0.4)
        {
          if (target > this.currentValue)
            this.animations.Add(TemporaryAnimatedSprite.GetTemporaryAnimatedSprite(Game1.random.Next(10, 12), position + new Vector2((float) Game1.random.Next(30, 190), (float) Game1.random.Next(-32, 48 /*0x30*/)), Color.Gold));
          else if (target < this.currentValue)
          {
            TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(356, 449, 1, 1), 999999f, 1, 44, position + new Vector2((float) Game1.random.Next(160 /*0xA0*/), (float) Game1.random.Next(-32, 32 /*0x20*/)), false, false, 1f, 0.01f, Color.White, (float) (Game1.random.Next(1, 3) * 4), -1f / 1000f, 0.0f, 0.0f);
            temporaryAnimatedSprite.motion = new Vector2((float) Game1.random.Next(-30, 40) / 10f, (float) Game1.random.Next(-30, -5) / 10f);
            temporaryAnimatedSprite.acceleration = new Vector2(0.0f, 0.25f);
            this.animations.Add(temporaryAnimatedSprite);
          }
        }
      }
    }
    for (int index = this.animations.Count - 1; index >= 0; --index)
    {
      if (this.animations[index].update(Game1.currentGameTime))
        this.animations.RemoveAt(index);
      else
        this.animations[index].draw(b, true);
    }
    int x = 0;
    int num1 = (int) Math.Pow(10.0, (double) (this.numDigits - 1));
    bool flag = false;
    for (int index = 0; index < this.numDigits; ++index)
    {
      int num2 = this.currentValue / num1 % 10;
      if (num2 > 0 || index == this.numDigits - 1)
        flag = true;
      if (flag)
        b.Draw(Game1.mouseCursors, position + new Vector2((float) x, !(Game1.activeClickableMenu is ShippingMenu) || this.currentValue < 1000000 ? 0.0f : (float) Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.53096771240234 + (double) index) * (float) (this.currentValue / 1000000)), new Rectangle?(new Rectangle(286, 502 - num2 * 8, 5, 8)), Color.Maroon, 0.0f, Vector2.Zero, (float) (4.0 + (this.moneyShineTimer / 60 == this.numDigits - index ? 0.30000001192092896 : 0.0)), SpriteEffects.None, 1f);
      x += 24;
      num1 /= 10;
    }
  }
}
