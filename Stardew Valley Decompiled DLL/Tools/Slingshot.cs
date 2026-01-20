// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Slingshot
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Projectiles;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class Slingshot : Tool
{
  public const int basicDamage = 5;
  public const string basicSlingshotId = "32";
  public const string masterSlingshotId = "33";
  public const string galaxySlingshotId = "34";
  public const int drawBackSoundThreshold = 8;
  [XmlIgnore]
  public int lastClickX;
  [XmlIgnore]
  public int lastClickY;
  [XmlIgnore]
  public int mouseDragAmount;
  [XmlIgnore]
  public double pullStartTime = -1.0;
  [XmlIgnore]
  public float nextAutoFire = -1f;
  [XmlIgnore]
  public bool canPlaySound;
  [XmlIgnore]
  private readonly NetEvent0 finishEvent = new NetEvent0();
  [XmlIgnore]
  public readonly NetPoint aimPos = new NetPoint().Interpolated(true, true);

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(W)";

  public Slingshot()
    : this("32")
  {
  }

  /// <inheritdoc />
  protected override void MigrateLegacyItemId()
  {
    this.ItemId = this.InitialParentTileIndex.ToString();
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Slingshot(this.ItemId);

  protected override string loadDisplayName()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).DisplayName;
  }

  protected override string loadDescription()
  {
    return ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId).Description;
  }

  public override bool doesShowTileLocationMarker() => false;

  public Slingshot(string itemId = "32")
  {
    itemId = this.ValidateUnqualifiedItemId(itemId);
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(W)" + itemId);
    this.ItemId = itemId;
    this.Name = dataOrErrorItem.InternalName;
    this.InitialParentTileIndex = dataOrErrorItem.SpriteIndex;
    this.CurrentParentTileIndex = dataOrErrorItem.SpriteIndex;
    this.IndexOfMenuItemView = dataOrErrorItem.SpriteIndex;
    this.numAttachmentSlots.Value = 1;
    this.attachments.SetCount(1);
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.finishEvent, "finishEvent").AddField((INetSerializable) this.aimPos, "aimPos");
    this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
  }

  public int GetBackArmDistance(Farmer who)
  {
    if (this.CanAutoFire() && (double) this.nextAutoFire > 0.0)
      return (int) Utility.Lerp(20f, 0.0f, this.nextAutoFire / this.GetAutoFireRate());
    return !Game1.options.useLegacySlingshotFiring ? (int) (20.0 * (double) this.GetSlingshotChargeTime()) : Math.Min(20, (int) Vector2.Distance(who.getStandingPosition(), new Vector2((float) this.aimPos.X, (float) this.aimPos.Y)) / 20);
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    this.IndexOfMenuItemView = this.InitialParentTileIndex;
    if (!this.CanAutoFire())
      this.PerformFire(location, who);
    this.finish();
  }

  public virtual void PerformFire(GameLocation location, Farmer who)
  {
    StardewValley.Object attachment = this.attachments[0];
    if (attachment != null)
    {
      this.updateAimPos();
      int x = this.aimPos.X;
      int y = this.aimPos.Y;
      int backArmDistance = this.GetBackArmDistance(who);
      Vector2 shootOrigin = this.GetShootOrigin(who);
      Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(this.GetShootOrigin(who), this.AdjustForHeight(new Vector2((float) x, (float) y)), (float) (15 + Game1.random.Next(4, 6)) * (1f + who.buffs.WeaponSpeedMultiplier));
      if (backArmDistance > 4 && !this.canPlaySound)
      {
        StardewValley.Object one = (StardewValley.Object) attachment.getOne();
        if (attachment.ConsumeStack(1) == null)
          this.attachments[0] = (StardewValley.Object) null;
        float num;
        switch (this.ItemId)
        {
          case "33":
            num = 2f;
            break;
          case "34":
            num = 4f;
            break;
          default:
            num = 1f;
            break;
        }
        int ammoDamage = this.GetAmmoDamage(one);
        string ammoCollisionSound = this.GetAmmoCollisionSound(one);
        BasicProjectile.onCollisionBehavior collisionBehavior = this.GetAmmoCollisionBehavior(one);
        if (!Game1.options.useLegacySlingshotFiring)
        {
          velocityTowardPoint.X *= -1f;
          velocityTowardPoint.Y *= -1f;
        }
        NetCollection<Projectile> projectiles = location.projectiles;
        BasicProjectile basicProjectile = new BasicProjectile((int) ((double) num * (double) (ammoDamage + Game1.random.Next(-(ammoDamage / 2), ammoDamage + 2)) * (1.0 + (double) who.buffs.AttackMultiplier)), -1, 0, 0, (float) (Math.PI / (64.0 + (double) Game1.random.Next(-63, 64 /*0x40*/))), -velocityTowardPoint.X, -velocityTowardPoint.Y, shootOrigin - new Vector2(32f, 32f), ammoCollisionSound, damagesMonsters: true, location: location, firer: (Character) who, collisionBehavior: collisionBehavior, shotItemId: one.ItemId);
        basicProjectile.IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null;
        projectiles.Add((Projectile) basicProjectile);
      }
    }
    else
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
    this.canPlaySound = true;
  }

  /// <summary>Get the damage inflicted by shooting an ammunition item, excluding the slingshot upgrade level multiplier.</summary>
  /// <param name="ammunition">The item that was shot.</param>
  public virtual int GetAmmoDamage(StardewValley.Object ammunition)
  {
    string qualifiedItemId = ammunition?.QualifiedItemId;
    if (qualifiedItemId != null && qualifiedItemId.Length == 6)
    {
      switch (qualifiedItemId[5])
      {
        case '0':
          switch (qualifiedItemId)
          {
            case "(O)390":
              return 5;
            case "(O)380":
              return 20;
          }
          break;
        case '1':
          if (qualifiedItemId == "(O)441")
            return 20;
          break;
        case '2':
          if (qualifiedItemId == "(O)382")
            return 15;
          break;
        case '4':
          if (qualifiedItemId == "(O)384")
            return 30;
          break;
        case '6':
          if (qualifiedItemId == "(O)386")
            return 50;
          break;
        case '8':
          switch (qualifiedItemId)
          {
            case "(O)388":
              return 2;
            case "(O)378":
              return 10;
          }
          break;
      }
    }
    return 1;
  }

  /// <summary>Get the sound to play when the ammunition item hits.</summary>
  /// <param name="ammunition">The item that was shot.</param>
  public virtual string GetAmmoCollisionSound(StardewValley.Object ammunition)
  {
    if (ammunition?.QualifiedItemId == "(O)441")
      return "explosion";
    return ammunition != null && ammunition.Category == -5 ? "slimedead" : "hammer";
  }

  /// <summary>Get the logic to apply when the ammunition item hits.</summary>
  /// <param name="ammunition">The item that was shot.</param>
  public virtual BasicProjectile.onCollisionBehavior GetAmmoCollisionBehavior(StardewValley.Object ammunition)
  {
    return ammunition.QualifiedItemId == "(O)441" ? new BasicProjectile.onCollisionBehavior(BasicProjectile.explodeOnImpact) : (BasicProjectile.onCollisionBehavior) null;
  }

  public Vector2 GetShootOrigin(Farmer who)
  {
    return this.AdjustForHeight(who.getStandingPosition(), false);
  }

  public Vector2 AdjustForHeight(Vector2 position, bool for_cursor = true)
  {
    return !Game1.options.useLegacySlingshotFiring & for_cursor ? new Vector2(position.X, position.Y) : new Vector2(position.X, (float) ((double) position.Y - 32.0 - 8.0));
  }

  public void finish() => this.finishEvent.Fire();

  private void doFinish()
  {
    if (this.lastUser == null)
      return;
    this.lastUser.usingSlingshot = false;
    this.lastUser.canReleaseTool = true;
    this.lastUser.UsingTool = false;
    this.lastUser.canMove = true;
    this.lastUser.Halt();
    if (this.lastUser != Game1.player || !Game1.options.gamepadControls)
      return;
    Game1.game1.controllerSlingshotSafeTime = 0.2f;
  }

  /// <inheritdoc />
  protected override bool canThisBeAttached(StardewValley.Object o, int slot)
  {
    string qualifiedItemId = o.QualifiedItemId;
    if (qualifiedItemId != null && qualifiedItemId.Length == 6)
    {
      switch (qualifiedItemId[5])
      {
        case '0':
          if (qualifiedItemId == "(O)380" || qualifiedItemId == "(O)390")
            break;
          goto label_9;
        case '1':
          if (!(qualifiedItemId == "(O)441"))
            goto label_9;
          break;
        case '2':
          if (qualifiedItemId == "(O)382")
            break;
          goto label_9;
        case '4':
          if (qualifiedItemId == "(O)384")
            break;
          goto label_9;
        case '6':
          if (qualifiedItemId == "(O)386")
            break;
          goto label_9;
        case '8':
          if (qualifiedItemId == "(O)378" || qualifiedItemId == "(O)388")
            break;
          goto label_9;
        default:
          goto label_9;
      }
      return true;
    }
label_9:
    if (o.bigCraftable.Value)
      return false;
    return o.Category == -5 || o.Category == -79 || o.Category == -75;
  }

  public override string getHoverBoxText(Item hoveredItem)
  {
    if (hoveredItem is StardewValley.Object o && this.canThisBeAttached(o))
      return Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14256", (object) this.DisplayName, (object) o.DisplayName);
    return hoveredItem == null && this.attachments?[0] != null ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14258", (object) this.attachments[0].DisplayName) : (string) null;
  }

  public override bool onRelease(GameLocation location, int x, int y, Farmer who)
  {
    this.DoFunction(location, x, y, 1, who);
    return true;
  }

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    who.usingSlingshot = true;
    who.canReleaseTool = false;
    this.mouseDragAmount = 0;
    int num = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
    who.FarmerSprite.setCurrentFrame(42 + num);
    if (!who.IsLocalPlayer)
      return true;
    Game1.oldMouseState = Game1.input.GetMouseState();
    Game1.lastMousePositionBeforeFade = Game1.getMousePosition();
    this.lastClickX = Game1.getOldMouseX() + Game1.viewport.X;
    this.lastClickY = Game1.getOldMouseY() + Game1.viewport.Y;
    this.pullStartTime = Game1.currentGameTime.TotalGameTime.TotalSeconds;
    if (this.CanAutoFire())
      this.nextAutoFire = -1f;
    this.updateAimPos();
    return true;
  }

  public virtual float GetAutoFireRate() => 0.3f;

  public virtual bool CanAutoFire() => false;

  private void updateAimPos()
  {
    if (this.lastUser == null || !this.lastUser.IsLocalPlayer)
      return;
    Point point = Game1.getMousePosition();
    if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
    {
      Vector2 vector2 = Game1.oldPadState.ThumbSticks.Left;
      if ((double) vector2.Length() < 0.25)
      {
        vector2.X = 0.0f;
        vector2.Y = 0.0f;
        GamePadDPad dpad = Game1.oldPadState.DPad;
        if (dpad.Down == ButtonState.Pressed)
        {
          vector2.Y = -1f;
        }
        else
        {
          dpad = Game1.oldPadState.DPad;
          if (dpad.Up == ButtonState.Pressed)
            vector2.Y = 1f;
        }
        dpad = Game1.oldPadState.DPad;
        if (dpad.Left == ButtonState.Pressed)
          vector2.X = -1f;
        dpad = Game1.oldPadState.DPad;
        if (dpad.Right == ButtonState.Pressed)
          vector2.X = 1f;
        if ((double) vector2.X != 0.0 && (double) vector2.Y != 0.0)
        {
          vector2.Normalize();
          vector2 *= 1f;
        }
      }
      Vector2 shootOrigin = this.GetShootOrigin(this.lastUser);
      if (!Game1.options.useLegacySlingshotFiring && (double) vector2.Length() < 0.25)
      {
        switch (this.lastUser.FacingDirection)
        {
          case 0:
            vector2 = new Vector2(0.0f, 1f);
            break;
          case 1:
            vector2 = new Vector2(1f, 0.0f);
            break;
          case 2:
            vector2 = new Vector2(0.0f, -1f);
            break;
          case 3:
            vector2 = new Vector2(-1f, 0.0f);
            break;
        }
      }
      point = Utility.Vector2ToPoint(shootOrigin + new Vector2(vector2.X, -vector2.Y) * 600f);
      point.X -= Game1.viewport.X;
      point.Y -= Game1.viewport.Y;
    }
    int num1 = point.X + Game1.viewport.X;
    int num2 = point.Y + Game1.viewport.Y;
    this.aimPos.X = num1;
    this.aimPos.Y = num2;
  }

  public override void tickUpdate(GameTime time, Farmer who)
  {
    this.lastUser = who;
    this.finishEvent.Poll();
    if (!who.usingSlingshot)
      return;
    if (who.IsLocalPlayer)
    {
      this.updateAimPos();
      int x = this.aimPos.X;
      int y = this.aimPos.Y;
      ++this.mouseDragAmount;
      if (!Game1.options.useLegacySlingshotFiring)
      {
        Vector2 shootOrigin = this.GetShootOrigin(who);
        Vector2 vector2 = this.AdjustForHeight(new Vector2((float) x, (float) y)) - shootOrigin;
        if ((double) Math.Abs(vector2.X) > (double) Math.Abs(vector2.Y))
        {
          if ((double) vector2.X < 0.0)
            who.faceDirection(3);
          if ((double) vector2.X > 0.0)
            who.faceDirection(1);
        }
        else
        {
          if ((double) vector2.Y < 0.0)
            who.faceDirection(0);
          if ((double) vector2.Y > 0.0)
            who.faceDirection(2);
        }
      }
      else
        who.faceGeneralDirection(new Vector2((float) x, (float) y), opposite: true);
      if (!Game1.options.useLegacySlingshotFiring)
      {
        if (this.canPlaySound && (double) this.GetSlingshotChargeTime() >= 1.0)
        {
          if (this.PlayUseSounds)
            who.playNearbySoundAll("slingshot");
          this.canPlaySound = false;
        }
      }
      else if (this.canPlaySound && (Math.Abs(x - this.lastClickX) > 8 || Math.Abs(y - this.lastClickY) > 8) && this.mouseDragAmount > 4)
      {
        if (this.PlayUseSounds)
          who.playNearbySoundAll("slingshot");
        this.canPlaySound = false;
      }
      if (!this.CanAutoFire())
      {
        this.lastClickX = x;
        this.lastClickY = y;
      }
      if (Game1.options.useLegacySlingshotFiring)
        Game1.mouseCursor = Game1.cursor_none;
      if (this.CanAutoFire())
      {
        bool flag = false;
        if (this.GetBackArmDistance(who) >= 20 && (double) this.nextAutoFire < 0.0)
        {
          this.nextAutoFire = 0.0f;
          flag = true;
        }
        if ((double) this.nextAutoFire > 0.0 | flag)
        {
          this.nextAutoFire -= (float) time.ElapsedGameTime.TotalSeconds;
          if ((double) this.nextAutoFire <= 0.0)
          {
            this.PerformFire(who.currentLocation, who);
            this.nextAutoFire = this.GetAutoFireRate();
          }
        }
      }
    }
    int num = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
    who.FarmerSprite.setCurrentFrame(42 + num);
  }

  /// <inheritdoc />
  protected override void GetAttachmentSlotSprite(
    int slot,
    out Texture2D texture,
    out Rectangle sourceRect)
  {
    base.GetAttachmentSlotSprite(slot, out texture, out sourceRect);
    if (this.attachments[0] != null)
      return;
    sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 43);
  }

  public float GetSlingshotChargeTime()
  {
    return this.pullStartTime < 0.0 ? 0.0f : Utility.Clamp((float) (Game1.currentGameTime.TotalGameTime.TotalSeconds - this.pullStartTime) / this.GetRequiredChargeTime(), 0.0f, 1f);
  }

  public float GetRequiredChargeTime() => 0.3f;

  public override void draw(SpriteBatch b)
  {
    if (!this.lastUser.usingSlingshot || !this.lastUser.IsLocalPlayer)
      return;
    int x = this.aimPos.X;
    int y = this.aimPos.Y;
    Vector2 shootOrigin = this.GetShootOrigin(this.lastUser);
    Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(shootOrigin, this.AdjustForHeight(new Vector2((float) x, (float) y)), 256f);
    double num1 = Math.Sqrt((double) velocityTowardPoint.X * (double) velocityTowardPoint.X + (double) velocityTowardPoint.Y * (double) velocityTowardPoint.Y) - 181.0;
    double num2 = (double) velocityTowardPoint.X / 256.0;
    double num3 = (double) velocityTowardPoint.Y / 256.0;
    int num4 = (int) ((double) velocityTowardPoint.X - num1 * num2);
    int num5 = (int) ((double) velocityTowardPoint.Y - num1 * num3);
    if (!Game1.options.useLegacySlingshotFiring)
    {
      num4 *= -1;
      num5 *= -1;
    }
    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(shootOrigin.X - (float) num4, shootOrigin.Y - (float) num5)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43)), Color.White, 0.0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0.999999f);
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
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    if (this.IndexOfMenuItemView == 0 || this.IndexOfMenuItemView == 21 || this.ItemId == "47")
    {
      switch (this.Name)
      {
        case nameof (Slingshot):
          this.CurrentParentTileIndex = int.Parse("32");
          break;
        case "Master Slingshot":
          this.CurrentParentTileIndex = int.Parse("33");
          break;
        case "Galaxy Slingshot":
          this.CurrentParentTileIndex = int.Parse("34");
          break;
      }
      this.IndexOfMenuItemView = this.CurrentParentTileIndex;
    }
    spriteBatch.Draw(Tool.weaponsTexture, location + new Vector2(32f, 29f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Tool.weaponsTexture, this.IndexOfMenuItemView, 16 /*0x10*/, 16 /*0x10*/)), color * transparency, 0.0f, new Vector2(8f, 8f), scaleSize * 4f, SpriteEffects.None, layerDepth);
    if (drawStackNumber != StackDrawType.Hide && this.attachments?[0] != null)
      Utility.drawTinyDigits(this.attachments[0].Stack, spriteBatch, location + new Vector2((float) (64 /*0x40*/ - Utility.getWidthOfTinyDigitString(this.attachments[0].Stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }
}
