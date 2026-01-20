// Decompiled with JetBrains decompiler
// Type: StardewValley.Companions.FlyingCompanion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using System;

#nullable disable
namespace StardewValley.Companions;

public class FlyingCompanion : Companion
{
  public const int VARIANT_FAIRY = 0;
  public const int VARIANT_PARROT = 1;
  private float flitTimer;
  private Vector2 extraPosition;
  private Vector2 extraPositionMotion;
  private Vector2 extraPositionAcceleration;
  private bool floatup;
  private int flapAnimationLength = 4;
  private int currentSidewaysFlap;
  private bool hasLight = true;
  private string lightId;
  private NetInt whichSubVariant = new NetInt(-1);
  private NetInt startingYForVariant = new NetInt(0);
  private bool perching;
  private float timeSinceLastZeroLerp;
  private float parrot_squawkTimer;
  private float parrot_squatTimer;

  public FlyingCompanion() => this.lightId = $"{nameof (FlyingCompanion)}_{Game1.random.Next()}";

  public FlyingCompanion(int whichVariant, int whichSubVariant = -1)
    : this()
  {
    this.whichVariant.Value = whichVariant;
    this.whichSubVariant.Value = whichSubVariant;
    if (whichVariant != 1)
      return;
    this.startingYForVariant.Value = 160 /*0xA0*/;
    this.hasLight = false;
  }

  public override void InitNetFields()
  {
    base.InitNetFields();
    this.NetFields.AddField((INetSerializable) this.whichSubVariant, "whichSubVariant").AddField((INetSerializable) this.startingYForVariant, "startingYForVariant");
  }

  public override void Draw(SpriteBatch b)
  {
    if (this.Owner?.currentLocation == null || this.Owner.currentLocation.DisplayName == "Temp" && !Game1.isFestival())
      return;
    Texture2D texture = Game1.content.Load<Texture2D>("TileSheets\\companions");
    SpriteEffects effects = SpriteEffects.None;
    if (this.direction.Value == 1)
      effects = SpriteEffects.FlipHorizontally;
    if (this.perching)
    {
      if ((double) this.parrot_squatTimer > 0.0)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0)) + this.extraPosition), new Rectangle?(new Rectangle((int) ((double) this.parrot_squatTimer % 1000.0) / 500 * 16 /*0x10*/ + 128 /*0x80*/, this.startingYForVariant.Value, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, effects, this._position.Y / 10000f);
      else if ((double) this.parrot_squawkTimer > 0.0)
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0)) + this.extraPosition), new Rectangle?(new Rectangle(160 /*0xA0*/, this.startingYForVariant.Value, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, effects, this._position.Y / 10000f);
      else
        b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0)) + this.extraPosition), new Rectangle?(new Rectangle(128 /*0x80*/, this.startingYForVariant.Value, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, effects, this._position.Y / 10000f);
    }
    else
    {
      b.Draw(texture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(0.0f, (float) (-(double) this.height * 4.0)) + this.extraPosition), new Rectangle?(new Rectangle(this.whichSubVariant.Value * 64 /*0x40*/ + (int) ((double) this.flitTimer / (double) (500 / this.flapAnimationLength)) * 16 /*0x10*/, this.startingYForVariant.Value, 16 /*0x10*/, 16 /*0x10*/)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, effects, this._position.Y / 10000f);
      b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(this.Position + this.Owner.drawOffset + new Vector2(this.extraPosition.X, 0.0f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f * Utility.Lerp(1f, 0.8f, Math.Min(this.height, 1f)), SpriteEffects.None, (float) (((double) this._position.Y - 8.0) / 10000.0 - 1.9999999949504854E-06));
    }
  }

  public override void Update(GameTime time, GameLocation location)
  {
    base.Update(time, location);
    this.height = 32f;
    this.flitTimer += (float) time.ElapsedGameTime.TotalMilliseconds;
    if ((double) this.flitTimer > (double) (this.flapAnimationLength * 125))
    {
      this.flitTimer = 0.0f;
      this.extraPositionMotion = new Vector2(Game1.random.NextDouble() < 0.5 ? 0.1f : -0.1f, -2f);
      if ((double) this.extraPositionMotion.X < 0.0)
        --this.currentSidewaysFlap;
      else
        ++this.currentSidewaysFlap;
      if (this.currentSidewaysFlap < -4 || this.currentSidewaysFlap > 4)
        this.extraPositionMotion.X *= -1f;
      this.extraPositionAcceleration = new Vector2(0.0f, this.floatup ? 0.13f : 0.14f);
      if ((double) this.extraPosition.Y > 8.0)
        this.floatup = true;
      else if ((double) this.extraPosition.Y < -8.0)
        this.floatup = false;
    }
    if (!this.perching)
    {
      this.extraPosition += this.extraPositionMotion;
      this.extraPositionMotion += this.extraPositionAcceleration;
    }
    if (this.hasLight && location.Equals(Game1.currentLocation))
      Utility.repositionLightSource(this.lightId, this.Position - new Vector2(0.0f, this.height * 4f) + this.extraPosition);
    if (this.whichVariant.Value != 1)
      return;
    TimeSpan elapsedGameTime;
    if ((double) this.lerp <= 0.0)
    {
      double sinceLastZeroLerp = (double) this.timeSinceLastZeroLerp;
      elapsedGameTime = time.ElapsedGameTime;
      double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
      this.timeSinceLastZeroLerp = (float) (sinceLastZeroLerp + totalMilliseconds);
    }
    else
      this.timeSinceLastZeroLerp = 0.0f;
    this.whichSubVariant.Value = (double) this.timeSinceLastZeroLerp >= 100.0 ? 1 : 0;
    if ((double) this.timeSinceLastZeroLerp > 2000.0)
    {
      if (!this.perching && ((double) Math.Abs(this.OwnerPosition.X - (this.Position.X + this.extraPosition.X)) >= 8.0 || (double) Math.Abs(this.OwnerPosition.Y - (this.Position.Y + this.extraPosition.Y)) >= 8.0))
        return;
      if (this.perching && !(this.Owner.Position + new Vector2(32f, 20f)).Equals(this.Position))
      {
        this.perching = false;
        this.timeSinceLastZeroLerp = 0.0f;
        this.parrot_squatTimer = 0.0f;
        this.parrot_squawkTimer = 0.0f;
      }
      else
      {
        if ((double) this.parrot_squawkTimer > 0.0)
        {
          double parrotSquawkTimer = (double) this.parrot_squawkTimer;
          elapsedGameTime = time.ElapsedGameTime;
          double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
          this.parrot_squawkTimer = (float) (parrotSquawkTimer - totalMilliseconds);
        }
        if ((double) this.parrot_squatTimer > 0.0)
        {
          double parrotSquatTimer = (double) this.parrot_squatTimer;
          elapsedGameTime = time.ElapsedGameTime;
          double totalMilliseconds = elapsedGameTime.TotalMilliseconds;
          this.parrot_squatTimer = (float) (parrotSquatTimer - totalMilliseconds);
        }
        this.perching = true;
        this.Position = this.Owner.Position + new Vector2(32f, 20f);
        this.extraPosition = Vector2.Zero;
        this.endPosition = this.Position;
        if (Game1.random.NextDouble() < 0.0005 && (double) this.parrot_squawkTimer <= 0.0)
        {
          this.parrot_squawkTimer = 500f;
          location.localSound("parrot_squawk");
        }
        else
        {
          if (Game1.random.NextDouble() >= 0.0015 || (double) this.parrot_squatTimer > 0.0)
            return;
          this.parrot_squatTimer = (float) (Game1.random.Next(2, 6) * 1000);
        }
      }
    }
    else
      this.perching = false;
  }

  public override void InitializeCompanion(Farmer farmer)
  {
    base.InitializeCompanion(farmer);
    if (this.hasLight)
      Game1.currentLightSources.Add(new LightSource(this.lightId, 1, this.Position, 2f, Color.Black));
    if (this.whichSubVariant.Value != -1)
      return;
    Random random = Utility.CreateRandom((double) farmer.uniqueMultiplayerID.Value);
    this.whichSubVariant.Value = random.Next(4);
    if (this.whichVariant.Value != 0 || random.NextDouble() >= 0.5)
      return;
    this.startingYForVariant.Value += 176 /*0xB0*/;
  }

  public override void CleanupCompanion()
  {
    base.CleanupCompanion();
    if (!this.hasLight)
      return;
    Utility.removeLightSource(this.lightId);
  }

  public override void OnOwnerWarp()
  {
    base.OnOwnerWarp();
    this.extraPosition = Vector2.Zero;
    this.extraPositionMotion = Vector2.Zero;
    this.extraPositionAcceleration = Vector2.Zero;
    if (!this.hasLight)
      return;
    Game1.currentLightSources.Add(new LightSource(this.lightId, 1, this.Position, 2f, Color.Black));
  }

  public override void Hop(float amount)
  {
  }
}
