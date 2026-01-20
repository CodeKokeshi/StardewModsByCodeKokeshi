// Decompiled with JetBrains decompiler
// Type: StardewValley.Companions.HungryFrogCompanion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;
using System;

#nullable disable
namespace StardewValley.Companions;

public class HungryFrogCompanion : HoppingCompanion
{
  private const int RANGE = 300;
  private const int FULLNESS_TIME = 12000;
  public float fullnessTime;
  private float monsterEatCheckTimer;
  private float tongueOutTimer;
  private readonly NetBool tongueOut = new NetBool(false);
  private readonly NetBool tongueReturn = new NetBool(false);
  private readonly NetPosition tonguePosition = new NetPosition();
  private readonly NetVector2 tongueVelocity = new NetVector2();
  private readonly NetNPCRef attachedMonsterField = new NetNPCRef();
  private readonly NetEvent0 fullnessTrigger = new NetEvent0();
  private float initialEquipDelay = 12000f;
  private float lastHopTimer;

  private Monster attachedMonster
  {
    get
    {
      return this.Owner != null ? this.attachedMonsterField.Get(this.Owner.currentLocation) as Monster : (Monster) null;
    }
    set => this.attachedMonsterField.Set(this.Owner.currentLocation, (NPC) value);
  }

  public HungryFrogCompanion()
  {
  }

  public HungryFrogCompanion(int variant) => this.whichVariant.Value = variant;

  public override void InitNetFields()
  {
    base.InitNetFields();
    this.NetFields.AddField((INetSerializable) this.tongueOut, "tongueOut").AddField((INetSerializable) this.tongueReturn, "tongueReturn").AddField((INetSerializable) this.tonguePosition.NetFields, "tonguePosition.NetFields").AddField((INetSerializable) this.tongueVelocity, "tongueVelocity").AddField((INetSerializable) this.attachedMonsterField.NetFields, "attachedMonsterField.NetFields").AddField((INetSerializable) this.fullnessTrigger, "fullnessTrigger");
    this.fullnessTrigger.onEvent += new NetEvent0.Event(this.triggerFullnessTimer);
  }

  public override void Update(GameTime time, GameLocation location)
  {
    if (!this.tongueOut.Value)
      base.Update(time, location);
    if (!Game1.shouldTimePass())
      return;
    if ((double) this.fullnessTime > 0.0)
      this.fullnessTime -= (float) time.ElapsedGameTime.TotalMilliseconds;
    this.lastHopTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.initialEquipDelay > 0.0)
    {
      this.initialEquipDelay -= (float) time.ElapsedGameTime.TotalMilliseconds;
    }
    else
    {
      if (this.IsLocal)
      {
        this.monsterEatCheckTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
        if ((double) this.monsterEatCheckTimer >= 2000.0 && (double) this.fullnessTime <= 0.0 && !this.tongueOut.Value)
        {
          this.monsterEatCheckTimer = 0.0f;
          if (!(location is SlimeHutch))
          {
            Monster monsterWithinRange = Utility.findClosestMonsterWithinRange(location, this.Position, 300);
            if (monsterWithinRange != null)
            {
              if (monsterWithinRange is Bat && monsterWithinRange.Age == 789)
              {
                this.monsterEatCheckTimer = 0.0f;
                return;
              }
              if (monsterWithinRange.Name.Equals("Truffle Crab"))
              {
                this.monsterEatCheckTimer = 0.0f;
                return;
              }
              if (monsterWithinRange is GreenSlime greenSlime && greenSlime.prismatic.Value)
              {
                this.monsterEatCheckTimer = 0.0f;
                return;
              }
              this.height = 0.0f;
              Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(this.Position, monsterWithinRange.getStandingPosition(), 12f);
              this.tongueOut.Value = true;
              this.tongueReturn.Value = false;
              this.tonguePosition.Value = this.Position + new Vector2(-32f, -32f) + new Vector2(this.direction.Value != 3 ? 28f : 0.0f, -20f);
              this.tongueVelocity.Value = velocityTowardPoint;
              location.playSound("croak");
              this.direction.Value = (double) monsterWithinRange.Position.X < (double) this.Position.X ? 3 : 1;
            }
          }
          this.tongueOutTimer = 0.0f;
        }
        if (this.tongueOut.Value)
        {
          this.tongueOutTimer += (float) (time.ElapsedGameTime.TotalMilliseconds * (this.tongueReturn.Value ? -1.0 : 1.0));
          NetPosition tonguePosition = this.tonguePosition;
          tonguePosition.Value = tonguePosition.Value + this.tongueVelocity.Value;
          if (this.attachedMonster == null)
          {
            if ((double) Vector2.Distance(this.Position, this.tonguePosition.Value) >= 300.0)
            {
              this.tongueReachedMonster((Monster) null);
            }
            else
            {
              int num = 40;
              if (this.Owner.currentLocation.doesPositionCollideWithCharacter(new Rectangle((int) this.tonguePosition.X + 32 /*0x20*/ - num / 2, (int) this.tonguePosition.Y + 32 /*0x20*/ - num / 2, num, num)) is Monster m)
                this.tongueReachedMonster(m);
            }
          }
          if (this.attachedMonster != null)
          {
            this.attachedMonster.Position = this.tonguePosition.Value;
            this.attachedMonster.xVelocity = 0.0f;
            this.attachedMonster.yVelocity = 0.0f;
          }
          if (this.tongueReturn.Value)
          {
            Vector2 vector2 = Vector2.Subtract(this.Position + new Vector2(-32f, -32f) + new Vector2(this.direction.Value != 3 ? 28f : 0.0f, -20f), this.tonguePosition.Value);
            vector2.Normalize();
            this.tongueVelocity.Value = vector2 * 12f;
          }
          if (this.tongueReturn.Value && (double) Vector2.Distance(this.Position, this.tonguePosition.Value) <= 48.0 || (double) this.tongueOutTimer <= 0.0)
          {
            if (this.attachedMonster != null)
            {
              if (this.attachedMonster is HotHead attachedMonster && (double) attachedMonster.timeUntilExplode.Value > 0.0)
                attachedMonster.currentLocation?.netAudio.StopPlaying("fuse");
              if (this.attachedMonster.currentLocation != null)
                this.attachedMonster.currentLocation.characters.Remove((NPC) this.attachedMonster);
              else
                location.characters.Remove((NPC) this.attachedMonster);
              this.fullnessTrigger.Fire();
              this.attachedMonster = (Monster) null;
            }
            double num = (double) Vector2.Distance(this.Position, this.tonguePosition.Value);
            this.tongueOut.Value = false;
            this.tongueReturn.Value = false;
          }
        }
      }
      else if (this.tongueOut.Value && this.attachedMonster != null)
      {
        this.attachedMonster.Position = this.tonguePosition.Value;
        this.attachedMonster.position.Paused = true;
        this.attachedMonster.xVelocity = 0.0f;
        this.attachedMonster.yVelocity = 0.0f;
      }
      this.fullnessTrigger.Poll();
    }
  }

  public override void OnOwnerWarp()
  {
    this.attachedMonster = (Monster) null;
    this.tongueOut.Value = false;
    this.tongueReturn.Value = false;
    base.OnOwnerWarp();
  }

  public override void Hop(float amount)
  {
    base.Hop(amount);
    if ((double) this.fullnessTime > 0.0)
      this.Owner?.currentLocation.localSound("frog_slap");
    this.lastHopTimer = 0.0f;
  }

  private void triggerFullnessTimer() => this.fullnessTime = 12000f;

  public void tongueReachedMonster(Monster m)
  {
    this.tongueReturn.Value = true;
    this.tongueVelocity.Value = this.tongueVelocity.Value * -1f;
    this.attachedMonster = m;
    if (m == null)
      return;
    m.DamageToFarmer = 0;
    m.farmerPassesThrough = true;
    this.Owner?.currentLocation.localSound("fishSlap");
  }

  public override void Draw(SpriteBatch b)
  {
    if (this.Owner?.currentLocation == null || this.Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival())
      return;
    Texture2D texture = Game1.content.Load<Texture2D>("TileSheets\\companions");
    SpriteEffects effects = SpriteEffects.None;
    Rectangle r = new Rectangle((double) this.fullnessTime > 0.0 ? 128 /*0x80*/ : 0, 16 /*0x10*/ + this.whichVariant.Value * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/);
    Color color = this.whichVariant.Value == 7 ? Utility.GetPrismaticColor() : Color.White;
    if (this.direction.Value == 3)
      effects = SpriteEffects.FlipHorizontally;
    if (this.tongueOut.Value)
      b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(r, 112 /*0x70*/)), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
    else if ((double) this.height > 0.0)
    {
      if ((double) this.gravity > 0.0)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(r, 16 /*0x10*/)), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
      else if ((double) this.gravity > -0.15000000596046448)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(r, 32 /*0x20*/)), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
      else
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(r, 48 /*0x30*/)), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
    }
    else if ((double) this.lastHopTimer > 5000.0 && !this.tongueOut.Value)
      b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(Utility.translateRect(r, 80 /*0x50*/ + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 >= 200.0 ? 16 /*0x10*/ : 0))), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
    else
      b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0))), new Rectangle?(r), color, 0.0f, new Vector2(8f, 16f), 4f, effects, (float) (((double) this._position.Y - 12.0) / 10000.0));
    SpriteBatch spriteBatch = b;
    Texture2D shadowTexture = Game1.shadowTexture;
    Vector2 local1 = Game1.GlobalToLocal(this.Position + this.Owner.drawOffset);
    Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
    Color white = Color.White;
    Rectangle bounds = Game1.shadowTexture.Bounds;
    double x = (double) bounds.Center.X;
    bounds = Game1.shadowTexture.Bounds;
    double y = (double) bounds.Center.Y;
    Vector2 origin = new Vector2((float) x, (float) y);
    double scale = 3.0 * (double) Utility.Lerp(1f, 0.8f, Math.Min(this.height, 1f));
    spriteBatch.Draw(shadowTexture, local1, sourceRectangle, white, 0.0f, origin, (float) scale, SpriteEffects.None, 0.0f);
    if (!this.tongueOut.Value)
      return;
    Vector2 local2 = Game1.GlobalToLocal(this.tonguePosition.Value + new Vector2(32f));
    Vector2 local3 = Game1.GlobalToLocal(this.Position + new Vector2(-32f, -32f) + new Vector2(this.direction.Value != 3 ? 44f : 24f, 16f));
    Utility.drawLineWithScreenCoordinates((int) local3.X, (int) local3.Y, (int) local2.X, (int) local2.Y, b, Color.Red, thickness: 4);
    Texture2D projectileSheet = Projectile.projectileSheet;
    Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Projectile.projectileSheet, 19, 16 /*0x10*/, 16 /*0x10*/);
    b.Draw(projectileSheet, Game1.GlobalToLocal(this.tonguePosition.Value + new Vector2(32f, 32f)) + this.Owner.drawOffset, new Rectangle?(standardTileSheet), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 1f);
  }
}
