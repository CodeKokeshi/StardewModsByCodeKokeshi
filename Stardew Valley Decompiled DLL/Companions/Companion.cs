// Decompiled with JetBrains decompiler
// Type: StardewValley.Companions.Companion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;

#nullable disable
namespace StardewValley.Companions;

public class Companion : INetObject<NetFields>
{
  public readonly NetInt direction = new NetInt();
  protected readonly NetPosition _position = new NetPosition();
  protected readonly NetFarmerRef _owner = new NetFarmerRef();
  public readonly NetInt whichVariant = new NetInt();
  public float lerp = -1f;
  public Vector2 startPosition;
  public Vector2 endPosition;
  public float height;
  public float gravity;
  public NetEvent1Field<float, NetFloat> hopEvent = new NetEvent1Field<float, NetFloat>();

  public NetFields NetFields { get; } = new NetFields(nameof (Companion));

  public Farmer Owner
  {
    get => this._owner.Value;
    set => this._owner.Value = value;
  }

  public Vector2 Position
  {
    get => this._position.Value;
    set => this._position.Value = value;
  }

  public Vector2 OwnerPosition => Utility.PointToVector2(this.Owner.GetBoundingBox().Center);

  public bool IsLocal => this.Owner.IsLocalPlayer;

  public Companion()
  {
    this.InitNetFields();
    this.direction.Value = 1;
  }

  public virtual void InitializeCompanion(Farmer farmer)
  {
    this._owner.Value = farmer;
    this._position.Value = farmer.Position;
  }

  public virtual void CleanupCompanion() => this._owner.Value = (Farmer) null;

  public virtual void InitNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this._owner.NetFields, "_owner.NetFields").AddField((INetSerializable) this._position.NetFields, "_position.NetFields").AddField((INetSerializable) this.hopEvent, "hopEvent").AddField((INetSerializable) this.direction, "direction").AddField((INetSerializable) this.whichVariant, "whichVariant");
    this.hopEvent.onEvent += new AbstractNetEvent1<float>.Event(this.Hop);
  }

  public virtual void Hop(float amount)
  {
    this.height = 0.0f;
    this.gravity = amount;
  }

  public virtual void Update(GameTime time, GameLocation location)
  {
    TimeSpan elapsedGameTime;
    if (this.IsLocal)
    {
      if ((double) this.lerp < 0.0)
      {
        Vector2 vector2 = this.OwnerPosition - this.Position;
        if ((double) vector2.Length() > 768.0)
        {
          Utility.addRainbowStarExplosion(location, this.Position + new Vector2(0.0f, -this.height), 1);
          this.Position = this.Owner.Position;
          this.lerp = -1f;
        }
        vector2 = this.OwnerPosition - this.Position;
        if ((double) vector2.Length() > 80.0)
        {
          this.startPosition = this.Position;
          float num = 0.33f;
          this.endPosition = this.OwnerPosition + new Vector2(Utility.RandomFloat(-64f, 64f) * num, Utility.RandomFloat(-64f, 64f) * num);
          if (location.isCollidingPosition(new Rectangle((int) this.endPosition.X - 8, (int) this.endPosition.Y - 8, 16 /*0x10*/, 16 /*0x10*/), Game1.viewport, false, 0, false, (Character) null, true, ignoreCharacterRequirement: true))
            this.endPosition = this.OwnerPosition;
          this.lerp = 0.0f;
          this.hopEvent.Fire(1f);
          if ((double) Math.Abs(this.OwnerPosition.X - this.Position.X) > 8.0)
          {
            if ((double) this.OwnerPosition.X > (double) this.Position.X)
              this.direction.Value = 1;
            else
              this.direction.Value = 3;
          }
        }
      }
      if ((double) this.lerp >= 0.0)
      {
        double lerp = (double) this.lerp;
        elapsedGameTime = time.ElapsedGameTime;
        double num = elapsedGameTime.TotalSeconds / 0.40000000596046448;
        this.lerp = (float) (lerp + num);
        if ((double) this.lerp > 1.0)
          this.lerp = 1f;
        this.Position = new Vector2(Utility.Lerp(this.startPosition.X, this.endPosition.X, this.lerp), Utility.Lerp(this.startPosition.Y, this.endPosition.Y, this.lerp));
        if ((double) this.lerp == 1.0)
          this.lerp = -1f;
      }
    }
    this.hopEvent.Poll();
    if ((double) this.gravity == 0.0 && (double) this.height == 0.0)
      return;
    this.height += this.gravity;
    double gravity = (double) this.gravity;
    elapsedGameTime = time.ElapsedGameTime;
    double num1 = elapsedGameTime.TotalSeconds * 6.0;
    this.gravity = (float) (gravity - num1);
    if ((double) this.height > 0.0)
      return;
    this.height = 0.0f;
    this.gravity = 0.0f;
  }

  public virtual void Draw(SpriteBatch b)
  {
  }

  public virtual void OnOwnerWarp() => this._position.Value = this._owner.Value.Position;
}
