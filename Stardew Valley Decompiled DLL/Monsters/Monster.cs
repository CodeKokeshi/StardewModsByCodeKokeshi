// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.Monster
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Layers;

#nullable disable
namespace StardewValley.Monsters;

[XmlInclude(typeof (AngryRoger))]
[XmlInclude(typeof (Bat))]
[XmlInclude(typeof (BigSlime))]
[XmlInclude(typeof (BlueSquid))]
[XmlInclude(typeof (Bug))]
[XmlInclude(typeof (DinoMonster))]
[XmlInclude(typeof (Duggy))]
[XmlInclude(typeof (DustSpirit))]
[XmlInclude(typeof (DwarvishSentry))]
[XmlInclude(typeof (Fly))]
[XmlInclude(typeof (Ghost))]
[XmlInclude(typeof (GreenSlime))]
[XmlInclude(typeof (Grub))]
[XmlInclude(typeof (HotHead))]
[XmlInclude(typeof (LavaLurk))]
[XmlInclude(typeof (Leaper))]
[XmlInclude(typeof (MetalHead))]
[XmlInclude(typeof (Mummy))]
[XmlInclude(typeof (RockCrab))]
[XmlInclude(typeof (RockGolem))]
[XmlInclude(typeof (Serpent))]
[XmlInclude(typeof (ShadowBrute))]
[XmlInclude(typeof (ShadowGirl))]
[XmlInclude(typeof (ShadowGuy))]
[XmlInclude(typeof (ShadowShaman))]
[XmlInclude(typeof (Shooter))]
[XmlInclude(typeof (Skeleton))]
[XmlInclude(typeof (Spiker))]
[XmlInclude(typeof (SquidKid))]
public class Monster : NPC
{
  public const int index_health = 0;
  public const int index_damageToFarmer = 1;
  public const int index_isGlider = 4;
  public const int index_drops = 6;
  public const int index_resilience = 7;
  public const int index_jitteriness = 8;
  public const int index_distanceThresholdToMoveTowardsPlayer = 9;
  public const int index_speed = 10;
  public const int index_missChance = 11;
  public const int index_isMineMonster = 12;
  public const int index_experiencePoints = 13;
  public const int index_displayName = 14;
  public const int defaultInvincibleCountdown = 450;
  public float timeBeforeAIMovementAgain;
  [XmlElement("damageToFarmer")]
  public readonly NetInt damageToFarmer;
  [XmlElement("health")]
  public readonly NetIntDelta health;
  [XmlElement("maxHealth")]
  public readonly NetInt maxHealth;
  [XmlElement("resilience")]
  public readonly NetInt resilience;
  [XmlElement("slipperiness")]
  public readonly NetInt slipperiness;
  [XmlElement("experienceGained")]
  public readonly NetInt experienceGained;
  [XmlElement("jitteriness")]
  public readonly NetDouble jitteriness;
  [XmlElement("missChance")]
  public readonly NetDouble missChance;
  [XmlElement("isGlider")]
  public readonly NetBool isGlider;
  [XmlElement("mineMonster")]
  public readonly NetBool mineMonster;
  [XmlElement("hasSpecialItem")]
  public readonly NetBool hasSpecialItem;
  [XmlIgnore]
  public readonly NetFloat synchedRotation;
  [XmlArrayItem("int")]
  public readonly NetStringList objectsToDrop;
  [XmlIgnore]
  public int skipHorizontal;
  [XmlIgnore]
  public int invincibleCountdown;
  [XmlIgnore]
  public readonly NetInt defaultAnimationInterval;
  public readonly NetInt stunTime;
  [XmlElement("initializedForLocation")]
  public bool initializedForLocation;
  [XmlIgnore]
  public readonly NetBool netFocusedOnFarmers;
  [XmlIgnore]
  public readonly NetBool netWildernessFarmMonster;
  private readonly NetEvent1<ParryEventArgs> parryEvent;
  private readonly NetEvent1Field<Vector2, NetVector2> trajectoryEvent;
  [XmlIgnore]
  private readonly NetEvent0 deathAnimEvent;
  [XmlElement("ignoreDamageLOS")]
  public readonly NetBool ignoreDamageLOS;
  [XmlIgnore]
  public Monster.collisionBehavior onCollision;
  [XmlElement("isHardModeMonster")]
  public NetBool isHardModeMonster;
  private int slideAnimationTimer;

  [XmlIgnore]
  public Farmer Player => this.findPlayer();

  [XmlIgnore]
  public int DamageToFarmer
  {
    get => this.damageToFarmer.Value;
    set => this.damageToFarmer.Value = value;
  }

  [XmlIgnore]
  public int Health
  {
    get => this.health.Value;
    set => this.health.Value = value;
  }

  [XmlIgnore]
  public int MaxHealth
  {
    get => this.maxHealth.Value;
    set => this.maxHealth.Value = value;
  }

  [XmlIgnore]
  public int ExperienceGained
  {
    get => this.experienceGained.Value;
    set => this.experienceGained.Value = value;
  }

  [XmlIgnore]
  public int Slipperiness
  {
    get => this.slipperiness.Value;
    set => this.slipperiness.Value = value;
  }

  [XmlIgnore]
  public bool focusedOnFarmers
  {
    get => this.netFocusedOnFarmers.Value;
    set => this.netFocusedOnFarmers.Value = value;
  }

  [XmlIgnore]
  public bool wildernessFarmMonster
  {
    get => this.netWildernessFarmMonster.Value;
    set => this.netWildernessFarmMonster.Value = value;
  }

  public Monster()
  {
    NetEvent1<ParryEventArgs> netEvent1 = new NetEvent1<ParryEventArgs>();
    netEvent1.InterpolationWait = false;
    this.parryEvent = netEvent1;
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.trajectoryEvent = netEvent1Field;
    this.deathAnimEvent = new NetEvent0();
    this.ignoreDamageLOS = new NetBool();
    this.isHardModeMonster = new NetBool(false);
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public Monster(string name, Vector2 position)
    : this(name, position, 2)
  {
    this.Breather = false;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.damageToFarmer, "damageToFarmer").AddField((INetSerializable) this.health, "health").AddField((INetSerializable) this.maxHealth, "maxHealth").AddField((INetSerializable) this.resilience, "resilience").AddField((INetSerializable) this.slipperiness, "slipperiness").AddField((INetSerializable) this.experienceGained, "experienceGained").AddField((INetSerializable) this.jitteriness, "jitteriness").AddField((INetSerializable) this.missChance, "missChance").AddField((INetSerializable) this.isGlider, "isGlider").AddField((INetSerializable) this.mineMonster, "mineMonster").AddField((INetSerializable) this.hasSpecialItem, "hasSpecialItem").AddField((INetSerializable) this.objectsToDrop, "objectsToDrop").AddField((INetSerializable) this.defaultAnimationInterval, "defaultAnimationInterval").AddField((INetSerializable) this.netFocusedOnFarmers, "netFocusedOnFarmers").AddField((INetSerializable) this.netWildernessFarmMonster, "netWildernessFarmMonster").AddField((INetSerializable) this.deathAnimEvent, "deathAnimEvent").AddField((INetSerializable) this.parryEvent, "parryEvent").AddField((INetSerializable) this.trajectoryEvent, "trajectoryEvent").AddField((INetSerializable) this.ignoreDamageLOS, "ignoreDamageLOS").AddField((INetSerializable) this.synchedRotation, "synchedRotation").AddField((INetSerializable) this.isHardModeMonster, "isHardModeMonster").AddField((INetSerializable) this.stunTime, "stunTime");
    this.position.Field.AxisAlignedMovement = false;
    this.parryEvent.onEvent += new AbstractNetEvent1<ParryEventArgs>.Event(this.handleParried);
    this.deathAnimEvent.onEvent += new NetEvent0.Event(this.localDeathAnimation);
    this.trajectoryEvent.onEvent += new AbstractNetEvent1<Vector2>.Event(this.doSetTrajectory);
  }

  protected override Farmer findPlayer()
  {
    if (this.currentLocation == null)
      return Game1.player;
    Farmer player = Game1.player;
    double num = double.MaxValue;
    foreach (Farmer farmer in this.currentLocation.farmers)
    {
      if (!farmer.hidden.Value)
      {
        double playerPriority = this.findPlayerPriority(farmer);
        if (playerPriority < num)
        {
          num = playerPriority;
          player = farmer;
        }
      }
    }
    return player;
  }

  protected virtual double findPlayerPriority(Farmer f)
  {
    return (double) (f.Position - this.Position).LengthSquared();
  }

  public virtual void onDealContactDamage(Farmer who)
  {
  }

  public virtual List<Item> getExtraDropItems() => new List<Item>();

  public override bool withinPlayerThreshold()
  {
    return this.focusedOnFarmers || this.withinPlayerThreshold(this.moveTowardPlayerThreshold.Value);
  }

  /// <inheritdoc />
  public override bool IsMonster => true;

  /// <inheritdoc />
  [XmlIgnore]
  public override bool IsVillager => false;

  public Monster(string name, Vector2 position, int facingDir)
  {
    NetEvent1<ParryEventArgs> netEvent1 = new NetEvent1<ParryEventArgs>();
    netEvent1.InterpolationWait = false;
    this.parryEvent = netEvent1;
    NetEvent1Field<Vector2, NetVector2> netEvent1Field = new NetEvent1Field<Vector2, NetVector2>();
    netEvent1Field.InterpolationWait = false;
    this.trajectoryEvent = netEvent1Field;
    this.deathAnimEvent = new NetEvent0();
    this.ignoreDamageLOS = new NetBool();
    this.isHardModeMonster = new NetBool(false);
    // ISSUE: explicit constructor call
    base.\u002Ector(new AnimatedSprite("Characters\\Monsters\\" + name), position, facingDir, name);
    this.parseMonsterInfo(name);
    this.Breather = false;
  }

  public virtual bool ShouldMonsterBeRemoved() => this.Health <= 0;

  public virtual void drawAboveAllLayers(SpriteBatch b)
  {
  }

  public override void draw(SpriteBatch b)
  {
    if (this.isGlider.Value)
      return;
    base.draw(b);
  }

  public virtual bool isInvincible() => this.invincibleCountdown > 0;

  public void setInvincibleCountdown(int time)
  {
    this.invincibleCountdown = time;
    this.startGlowing(new Color((int) byte.MaxValue, 0, 0), false, 0.25f);
    this.glowingTransparency = 1f;
  }

  protected int maxTimesReachedMineBottom()
  {
    int val1 = 0;
    foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      val1 = Math.Max(val1, onlineFarmer.timesReachedMineBottom);
    return val1;
  }

  public virtual Debris ModifyMonsterLoot(Debris debris) => debris;

  public virtual int GetBaseDifficultyLevel() => 0;

  public virtual void BuffForAdditionalDifficulty(int additional_difficulty)
  {
    if (this.DamageToFarmer != 0)
    {
      this.DamageToFarmer = (int) ((double) this.DamageToFarmer * (1.0 + (double) additional_difficulty * 0.25));
      int b = 20 + (additional_difficulty - 1) * 20;
      if (this.DamageToFarmer < b)
        this.DamageToFarmer = (int) Utility.Lerp((float) this.DamageToFarmer, (float) b, 0.5f);
    }
    this.MaxHealth = (int) ((double) this.MaxHealth * (1.0 + (double) additional_difficulty * 0.5));
    int b1 = 500 + (additional_difficulty - 1) * 300;
    if (this.MaxHealth < b1)
      this.MaxHealth = (int) Utility.Lerp((float) this.MaxHealth, (float) b1, 0.5f);
    this.Health = this.MaxHealth;
    this.resilience.Value += additional_difficulty * this.resilience.Value;
    this.isHardModeMonster.Value = true;
  }

  protected void parseMonsterInfo(string name)
  {
    string[] strArray1 = DataLoader.Monsters(Game1.content)[name].Split('/');
    this.Health = Convert.ToInt32(strArray1[0]);
    this.MaxHealth = this.Health;
    this.DamageToFarmer = Convert.ToInt32(strArray1[1]);
    this.isGlider.Value = Convert.ToBoolean(strArray1[4]);
    string[] strArray2 = ArgUtility.SplitBySpace(strArray1[6]);
    this.objectsToDrop.Clear();
    for (int index = 0; index < strArray2.Length; index += 2)
    {
      if (Game1.random.NextDouble() < Convert.ToDouble(strArray2[index + 1]))
        this.objectsToDrop.Add(strArray2[index]);
    }
    this.resilience.Value = Convert.ToInt32(strArray1[7]);
    this.jitteriness.Value = Convert.ToDouble(strArray1[8]);
    this.willDestroyObjectsUnderfoot = false;
    this.moveTowardPlayer(Convert.ToInt32(strArray1[9]));
    this.speed = Convert.ToInt32(strArray1[10]);
    this.missChance.Value = Convert.ToDouble(strArray1[11]);
    this.mineMonster.Value = Convert.ToBoolean(strArray1[12]);
    if (this.maxTimesReachedMineBottom() >= 1 && this.mineMonster.Value)
    {
      this.resilience.Value += this.resilience.Value / 2;
      this.missChance.Value *= 2.0;
      this.Health += Game1.random.Next(0, this.Health);
      this.DamageToFarmer += Game1.random.Next(0, this.DamageToFarmer / 2);
    }
    try
    {
      this.ExperienceGained = Convert.ToInt32(strArray1[13]);
    }
    catch (Exception ex)
    {
      this.ExperienceGained = 1;
    }
    this.displayName = strArray1[14];
  }

  /// <summary>Get the translated display name for a monster from the underlying data, if any.</summary>
  /// <param name="name">The monster's internal name.</param>
  public new static string GetDisplayName(string name)
  {
    string str;
    return name == null || !DataLoader.Monsters(Game1.content).TryGetValue(name, out str) ? name : str.Split('/')[14];
  }

  public virtual void InitializeForLocation(GameLocation location)
  {
    if (this.initializedForLocation)
      return;
    if (this.mineMonster.Value && this.maxTimesReachedMineBottom() >= 1)
    {
      double num = 0.0;
      if (location is MineShaft mineShaft)
        num = (double) mineShaft.GetAdditionalDifficulty() * 0.001;
      if (Game1.random.NextDouble() < 0.001 + num)
        this.objectsToDrop.Add(Game1.random.Choose<string>("72", "74"));
    }
    if (Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS") && Game1.random.NextDouble() < (this.name.Value == "Dust Spirit" ? 0.02 : 0.05))
      this.objectsToDrop.Add("890");
    if (location is MineShaft mineShaft1 && mineShaft1.mineLevel > 120 && !mineShaft1.isSideBranch())
    {
      int num1 = mineShaft1.mineLevel - 121;
      if (Utility.GetDayOfPassiveFestival("DesertFestival") > 0)
      {
        float chance = 0.02f + (float) (Game1.player.team.calicoEggSkullCavernRating.Value * 5 + 1 + num1) * (1f / 500f);
        if ((double) chance > 0.5)
          chance = 0.5f;
        if (Game1.random.NextBool(chance))
        {
          int num2 = Game1.random.Next(1, 4);
          for (int index = 0; index < num2; ++index)
            this.objectsToDrop.Add("CalicoEgg");
        }
      }
    }
    this.initializedForLocation = true;
  }

  /// <inheritdoc />
  public override void reloadSprite(bool onlyAppearance = false)
  {
    this.Sprite = new AnimatedSprite("Characters\\Monsters\\" + this.Name, 0, 16 /*0x10*/, 16 /*0x10*/);
  }

  /// <inheritdoc />
  public override void ChooseAppearance(LocalizedContentManager content = null)
  {
    if (this.Sprite?.Texture != null)
      return;
    this.reloadSprite(true);
  }

  public virtual void shedChunks(int number) => this.shedChunks(number, 0.75f);

  public virtual void shedChunks(int number, float scale)
  {
    if (this.Sprite.Texture.Height <= this.Sprite.getHeight() * 4)
      return;
    Point standingPixel = this.StandingPixel;
    Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, new Microsoft.Xna.Framework.Rectangle(0, this.Sprite.getHeight() * 4 + 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 8, standingPixel.X, standingPixel.Y, number, this.TilePoint.Y, Color.White, 4f * scale);
  }

  public void deathAnimation()
  {
    this.sharedDeathAnimation();
    this.deathAnimEvent.Fire();
  }

  protected virtual void sharedDeathAnimation() => this.shedChunks(Game1.random.Next(4, 9), 0.75f);

  protected virtual void localDeathAnimation()
  {
  }

  public void parried(int damage, Farmer who)
  {
    this.parryEvent.Fire(new ParryEventArgs(damage, who));
  }

  private void handleParried(ParryEventArgs args)
  {
    int damage = args.damage;
    Farmer who = args.who;
    if (Game1.IsMasterGame)
    {
      float xVelocity = this.xVelocity;
      float yVelocity = this.yVelocity;
      if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
        this.currentLocation.damageMonster(this.GetBoundingBox(), damage / 2, damage / 2 + 1, false, 0.0f, 0, 0.0f, 0.0f, false, who);
      this.xVelocity = -xVelocity;
      this.yVelocity = -yVelocity;
      this.xVelocity *= this.isGlider.Value ? 2f : 3.5f;
      this.yVelocity *= this.isGlider.Value ? 2f : 3.5f;
    }
    this.setInvincibleCountdown(450);
  }

  public virtual int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    Farmer who)
  {
    return this.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, "hitEnemy");
  }

  public int takeDamage(
    int damage,
    int xTrajectory,
    int yTrajectory,
    bool isBomb,
    double addedPrecision,
    string hitSound)
  {
    int damage1 = Math.Max(1, damage - this.resilience.Value);
    this.slideAnimationTimer = 0;
    if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
    {
      damage1 = -1;
    }
    else
    {
      this.Health -= damage1;
      this.currentLocation.playSound(hitSound);
      this.setTrajectory(xTrajectory / 3, yTrajectory / 3);
      if (this.Health <= 0)
        this.deathAnimation();
    }
    return damage1;
  }

  public override void setTrajectory(Vector2 trajectory) => this.trajectoryEvent.Fire(trajectory);

  private void doSetTrajectory(Vector2 trajectory)
  {
    if (!Game1.IsMasterGame)
      return;
    if ((double) Math.Abs(trajectory.X) > (double) Math.Abs(this.xVelocity))
      this.xVelocity = trajectory.X;
    if ((double) Math.Abs(trajectory.Y) <= (double) Math.Abs(this.yVelocity))
      return;
    this.yVelocity = trajectory.Y;
  }

  public virtual void behaviorAtGameTick(GameTime time)
  {
    if ((double) this.timeBeforeAIMovementAgain > 0.0)
      this.timeBeforeAIMovementAgain -= (float) time.ElapsedGameTime.Milliseconds;
    if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
      return;
    this.IsWalkingTowardPlayer = false;
    Point standingPixel1 = this.StandingPixel;
    Point standingPixel2 = this.Player.StandingPixel;
    if (Math.Abs(standingPixel2.Y - standingPixel1.Y) > 192 /*0xC0*/)
    {
      if (standingPixel2.X - standingPixel1.X > 0)
        this.SetMovingLeft(true);
      else
        this.SetMovingRight(true);
    }
    else if (standingPixel2.Y - standingPixel1.Y > 0)
      this.SetMovingUp(true);
    else
      this.SetMovingDown(true);
    this.MovePosition(time, Game1.viewport, this.currentLocation);
  }

  public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

  public override void update(GameTime time, GameLocation location)
  {
    if (Game1.IsMasterGame && !this.initializedForLocation && location != null)
    {
      this.InitializeForLocation(location);
      this.initializedForLocation = true;
    }
    this.parryEvent.Poll();
    this.trajectoryEvent.Poll();
    this.deathAnimEvent.Poll();
    this.position.UpdateExtrapolation((float) this.speed + this.addedSpeed);
    TimeSpan elapsedGameTime;
    if (this.invincibleCountdown > 0)
    {
      int invincibleCountdown = this.invincibleCountdown;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.invincibleCountdown = invincibleCountdown - milliseconds;
      if (this.invincibleCountdown <= 0)
        this.stopGlowing();
    }
    if (!location.farmers.Any())
      return;
    if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
      base.update(time, location);
    if (Game1.IsMasterGame)
    {
      if (this.stunTime.Value <= 0)
      {
        this.behaviorAtGameTick(time);
      }
      else
      {
        NetInt stunTime = this.stunTime;
        int num = stunTime.Value;
        elapsedGameTime = time.ElapsedGameTime;
        int totalMilliseconds = (int) elapsedGameTime.TotalMilliseconds;
        stunTime.Value = num - totalMilliseconds;
        if (this.stunTime.Value < 0)
          this.stunTime.Value = 0;
      }
    }
    this.updateAnimation(time);
    if (Game1.IsMasterGame)
      this.synchedRotation.Value = this.rotation;
    else
      this.rotation = this.synchedRotation.Value;
    Layer layer = location.map.RequireLayer("Back");
    if (this.controller != null && this.withinPlayerThreshold(3))
      this.controller = (PathFindController) null;
    if (!this.isGlider.Value && ((double) this.Position.X < 0.0 || (double) this.Position.X > (double) (layer.LayerWidth * 64 /*0x40*/) || (double) this.Position.Y < 0.0 || (double) this.Position.Y > (double) (layer.LayerHeight * 64 /*0x40*/)))
    {
      location.characters.Remove((NPC) this);
    }
    else
    {
      if (!this.isGlider.Value || (double) this.Position.X >= -2000.0)
        return;
      this.Health = -500;
    }
  }

  protected void resetAnimationSpeed()
  {
    if (this.ignoreMovementAnimations)
      return;
    this.Sprite.interval = (float) this.defaultAnimationInterval.Value - (float) (((double) this.speed + (double) this.addedSpeed - 2.0) * 20.0);
  }

  protected virtual void updateAnimation(GameTime time)
  {
    if (!Game1.IsMasterGame)
      this.updateMonsterSlaveAnimation(time);
    this.resetAnimationSpeed();
  }

  protected override void updateSlaveAnimation(GameTime time)
  {
  }

  protected virtual void updateMonsterSlaveAnimation(GameTime time)
  {
    this.Sprite.animateOnce(time);
  }

  public virtual bool ShouldActuallyMoveAwayFromPlayer() => false;

  private void checkHorizontalMovement(
    ref bool success,
    ref bool setMoving,
    ref bool scootSuccess,
    Farmer who,
    GameLocation location)
  {
    Vector2 position;
    if ((double) who.Position.X > (double) this.Position.X + 16.0)
    {
      if (this.ShouldActuallyMoveAwayFromPlayer())
        this.SetMovingOnlyLeft();
      else
        this.SetMovingOnlyRight();
      setMoving = true;
      if (!location.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
      {
        success = true;
      }
      else
      {
        this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
        position = this.Position;
        if (!position.Equals(this.lastPosition))
          scootSuccess = true;
      }
    }
    if (success || (double) who.Position.X >= (double) this.Position.X - 16.0)
      return;
    if (this.ShouldActuallyMoveAwayFromPlayer())
      this.SetMovingOnlyRight();
    else
      this.SetMovingOnlyLeft();
    setMoving = true;
    if (!location.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
    {
      success = true;
    }
    else
    {
      this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
      position = this.Position;
      if (position.Equals(this.lastPosition))
        return;
      scootSuccess = true;
    }
  }

  private void checkVerticalMovement(
    ref bool success,
    ref bool setMoving,
    ref bool scootSuccess,
    Farmer who,
    GameLocation location)
  {
    if (!success && (double) who.Position.Y < (double) this.Position.Y - 16.0)
    {
      if (this.ShouldActuallyMoveAwayFromPlayer())
        this.SetMovingOnlyDown();
      else
        this.SetMovingOnlyUp();
      setMoving = true;
      if (!location.isCollidingPosition(this.nextPosition(0), Game1.viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
      {
        success = true;
      }
      else
      {
        this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
        if (!this.Position.Equals(this.lastPosition))
          scootSuccess = true;
      }
    }
    if (success || (double) who.Position.Y <= (double) this.Position.Y + 16.0)
      return;
    if (this.ShouldActuallyMoveAwayFromPlayer())
      this.SetMovingOnlyUp();
    else
      this.SetMovingOnlyDown();
    setMoving = true;
    if (!location.isCollidingPosition(this.nextPosition(2), Game1.viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
    {
      success = true;
    }
    else
    {
      this.MovePosition(Game1.currentGameTime, Game1.viewport, location);
      if (this.Position.Equals(this.lastPosition))
        return;
      scootSuccess = true;
    }
  }

  public override void updateMovement(GameLocation location, GameTime time)
  {
    if (this.IsWalkingTowardPlayer)
    {
      if ((this.moveTowardPlayerThreshold.Value == -1 || this.withinPlayerThreshold()) && (double) this.timeBeforeAIMovementAgain <= 0.0 && this.IsMonster && !this.isGlider.Value)
      {
        xTile.Tiles.Tile tile = location.map.RequireLayer("Back").Tiles[this.Player.TilePoint.X, this.Player.TilePoint.Y];
        if (tile == null || tile.Properties.ContainsKey("NPCBarrier"))
          return;
        if (this.skipHorizontal <= 0)
        {
          if (this.lastPosition.Equals(this.Position) && Game1.random.NextDouble() < 0.001)
          {
            switch (this.FacingDirection)
            {
              case 0:
              case 2:
                if (Game1.random.NextBool())
                {
                  this.SetMovingOnlyRight();
                  break;
                }
                this.SetMovingOnlyLeft();
                break;
              case 1:
              case 3:
                if (Game1.random.NextBool())
                {
                  this.SetMovingOnlyUp();
                  break;
                }
                this.SetMovingOnlyDown();
                break;
            }
            this.skipHorizontal = 700;
            return;
          }
          bool success = false;
          bool setMoving = false;
          bool scootSuccess = false;
          if ((double) this.lastPosition.X == (double) this.Position.X)
          {
            this.checkHorizontalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
            this.checkVerticalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
          }
          else
          {
            this.checkVerticalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
            this.checkHorizontalMovement(ref success, ref setMoving, ref scootSuccess, this.Player, location);
          }
          if (success)
            this.skipHorizontal = 500;
          else if (!setMoving)
          {
            this.Halt();
            this.faceGeneralDirection(this.Player.getStandingPosition());
          }
          if (scootSuccess)
            return;
        }
        else
          this.skipHorizontal -= time.ElapsedGameTime.Milliseconds;
      }
    }
    else
      this.defaultMovementBehavior(time);
    this.MovePosition(time, Game1.viewport, location);
    if (!this.Position.Equals(this.lastPosition) || !this.IsWalkingTowardPlayer || !this.withinPlayerThreshold())
      return;
    this.noMovementProgressNearPlayerBehavior();
  }

  public virtual void noMovementProgressNearPlayerBehavior()
  {
    this.Halt();
    this.faceGeneralDirection(this.Player.getStandingPosition());
  }

  public virtual void defaultMovementBehavior(GameTime time)
  {
    if (Game1.random.NextDouble() >= this.jitteriness.Value * 1.8 || this.skipHorizontal > 0)
      return;
    switch (Game1.random.Next(6))
    {
      case 0:
        this.SetMovingOnlyUp();
        break;
      case 1:
        this.SetMovingOnlyRight();
        break;
      case 2:
        this.SetMovingOnlyDown();
        break;
      case 3:
        this.SetMovingOnlyLeft();
        break;
      default:
        this.Halt();
        break;
    }
  }

  public virtual bool TakesDamageFromHitbox(Microsoft.Xna.Framework.Rectangle area_of_effect)
  {
    return this.GetBoundingBox().Intersects(area_of_effect);
  }

  public virtual bool OverlapsFarmerForDamage(Farmer who)
  {
    return this.GetBoundingBox().Intersects(who.GetBoundingBox());
  }

  public override void Halt()
  {
    int speed = this.speed;
    base.Halt();
    this.speed = speed;
  }

  public override void MovePosition(
    GameTime time,
    xTile.Dimensions.Rectangle viewport,
    GameLocation currentLocation)
  {
    if (this.stunTime.Value > 0)
      return;
    this.lastPosition = this.Position;
    if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
    {
      if (double.IsNaN((double) this.xVelocity) || double.IsNaN((double) this.yVelocity))
      {
        this.xVelocity = 0.0f;
        this.yVelocity = 0.0f;
      }
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      int x = boundingBox.X;
      int y = boundingBox.Y;
      int b1 = boundingBox.X + (int) this.xVelocity;
      int b2 = boundingBox.Y - (int) this.yVelocity;
      int val1 = 1;
      bool flag1 = false;
      bool flag2 = this is SquidKid;
      if (!this.isGlider.Value | flag2)
      {
        if (boundingBox.Width > 0 && Math.Abs((int) this.xVelocity) > boundingBox.Width)
          val1 = (int) Math.Max((double) val1, Math.Ceiling((double) Math.Abs((int) this.xVelocity) / (double) boundingBox.Width));
        if (boundingBox.Height > 0 && Math.Abs((int) this.yVelocity) > boundingBox.Height)
          val1 = (int) Math.Max((double) val1, Math.Ceiling((double) Math.Abs((int) this.yVelocity) / (double) boundingBox.Height));
      }
      for (int index = 1; index <= val1; ++index)
      {
        boundingBox.X = (int) Utility.Lerp((float) x, (float) b1, (float) index / (float) val1);
        boundingBox.Y = (int) Utility.Lerp((float) y, (float) b2, (float) index / (float) val1);
        bool glider = this.isGlider.Value && !flag2;
        if (currentLocation != null && currentLocation.isCollidingPosition(boundingBox, viewport, false, this.DamageToFarmer, glider, (Character) this))
        {
          flag1 = true;
          break;
        }
      }
      if (!flag1)
      {
        this.position.X += this.xVelocity;
        this.position.Y -= this.yVelocity;
        if (this.Slipperiness < 1000)
        {
          this.xVelocity -= this.xVelocity / (float) this.Slipperiness;
          this.yVelocity -= this.yVelocity / (float) this.Slipperiness;
          if ((double) Math.Abs(this.xVelocity) <= 0.05000000074505806)
            this.xVelocity = 0.0f;
          if ((double) Math.Abs(this.yVelocity) <= 0.05000000074505806)
            this.yVelocity = 0.0f;
        }
        if (!this.isGlider.Value && this.invincibleCountdown > 0)
        {
          this.slideAnimationTimer -= time.ElapsedGameTime.Milliseconds;
          if (this.slideAnimationTimer < 0 && ((double) Math.Abs(this.xVelocity) >= 3.0 || (double) Math.Abs(this.yVelocity) >= 3.0))
          {
            this.slideAnimationTimer = 100 - (int) ((double) Math.Abs(this.xVelocity) * 2.0 + (double) Math.Abs(this.yVelocity) * 2.0);
            Game1.multiplayer.broadcastSprites(currentLocation, new TemporaryAnimatedSprite(6, this.getStandingPosition() + new Vector2(-32f, -32f), Color.White * 0.75f, flipped: Game1.random.NextBool(), animationInterval: 20f)
            {
              scale = 0.75f
            });
          }
        }
      }
      else if (this.isGlider.Value || this.Slipperiness >= 8)
      {
        if (this.isGlider.Value)
        {
          bool[] flagArray = Utility.horizontalOrVerticalCollisionDirections(boundingBox, (Character) this);
          if (flagArray[0])
          {
            this.xVelocity = -this.xVelocity;
            this.position.X += (float) Math.Sign(this.xVelocity);
            this.rotation += (float) (Math.PI + (double) Game1.random.Next(-10, 11) * Math.PI / 500.0);
          }
          if (flagArray[1])
          {
            this.yVelocity = -this.yVelocity;
            this.position.Y -= (float) Math.Sign(this.yVelocity);
            this.rotation += (float) (Math.PI + (double) Game1.random.Next(-10, 11) * Math.PI / 500.0);
          }
        }
        if (this.Slipperiness < 1000)
        {
          this.xVelocity -= (float) ((double) this.xVelocity / (double) this.Slipperiness / 4.0);
          this.yVelocity -= (float) ((double) this.yVelocity / (double) this.Slipperiness / 4.0);
          if ((double) Math.Abs(this.xVelocity) <= 0.05000000074505806)
            this.xVelocity = 0.0f;
          if ((double) Math.Abs(this.yVelocity) <= 0.050999999046325684)
            this.yVelocity = 0.0f;
        }
      }
      else
      {
        this.xVelocity -= this.xVelocity / (float) this.Slipperiness;
        this.yVelocity -= this.yVelocity / (float) this.Slipperiness;
        if ((double) Math.Abs(this.xVelocity) <= 0.05000000074505806)
          this.xVelocity = 0.0f;
        if ((double) Math.Abs(this.yVelocity) <= 0.05000000074505806)
          this.yVelocity = 0.0f;
      }
      if (this.isGlider.Value)
        return;
    }
    if (this.moveUp)
    {
      if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this) || this.isCharging)
      {
        this.position.Y -= (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimations)
          this.Sprite.AnimateUp(time);
        this.FacingDirection = 0;
        this.faceDirection(0);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle position = this.nextPosition(0);
        position.Width /= 4;
        bool flag3 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        position.X += position.Width * 3;
        bool flag4 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        if (flag3 && !flag4 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag4 && !flag3 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
          this.Halt();
        else if (this.willDestroyObjectsUnderfoot)
        {
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
          {
            currentLocation.playSound("stoneCrack");
            this.position.Y -= (float) this.speed + this.addedSpeed;
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
        Monster.collisionBehavior onCollision = this.onCollision;
        if (onCollision != null)
          onCollision(currentLocation);
      }
    }
    else if (this.moveRight)
    {
      if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this) || this.isCharging)
      {
        this.position.X += (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimations)
          this.Sprite.AnimateRight(time);
        this.FacingDirection = 1;
        this.faceDirection(1);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle position = this.nextPosition(1);
        position.Height /= 4;
        bool flag5 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        position.Y += position.Height * 3;
        bool flag6 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        if (flag5 && !flag6 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag6 && !flag5 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
          this.Halt();
        else if (this.willDestroyObjectsUnderfoot)
        {
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
          {
            currentLocation.playSound("stoneCrack");
            this.position.X += (float) this.speed + this.addedSpeed;
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
        Monster.collisionBehavior onCollision = this.onCollision;
        if (onCollision != null)
          onCollision(currentLocation);
      }
    }
    else if (this.moveDown)
    {
      if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this) || this.isCharging)
      {
        this.position.Y += (float) this.speed + this.addedSpeed;
        if (!this.ignoreMovementAnimations)
          this.Sprite.AnimateDown(time);
        this.FacingDirection = 2;
        this.faceDirection(2);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle position = this.nextPosition(2);
        position.Width /= 4;
        bool flag7 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        position.X += position.Width * 3;
        bool flag8 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        if (flag7 && !flag8 && !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.X += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag8 && !flag7 && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.X -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
          this.Halt();
        else if (this.willDestroyObjectsUnderfoot)
        {
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
          {
            currentLocation.playSound("stoneCrack");
            this.position.Y += (float) this.speed + this.addedSpeed;
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
        Monster.collisionBehavior onCollision = this.onCollision;
        if (onCollision != null)
          onCollision(currentLocation);
      }
    }
    else if (this.moveLeft)
    {
      if ((!Game1.eventUp || Game1.IsMultiplayer) && !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this) || this.isCharging)
      {
        this.position.X -= (float) this.speed + this.addedSpeed;
        this.FacingDirection = 3;
        if (!this.ignoreMovementAnimations)
          this.Sprite.AnimateLeft(time);
        this.faceDirection(3);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle position = this.nextPosition(3);
        position.Height /= 4;
        bool flag9 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        position.Y += position.Height * 3;
        bool flag10 = currentLocation.isCollidingPosition(position, viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this);
        if (flag9 && !flag10 && !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.Y += (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        else if (flag10 && !flag9 && !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, this.DamageToFarmer, this.isGlider.Value, (Character) this))
          this.position.Y -= (float) this.speed * ((float) time.ElapsedGameTime.Milliseconds / 64f);
        if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
          this.Halt();
        else if (this.willDestroyObjectsUnderfoot)
        {
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
          {
            currentLocation.playSound("stoneCrack");
            this.position.X -= (float) this.speed + this.addedSpeed;
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
        Monster.collisionBehavior onCollision = this.onCollision;
        if (onCollision != null)
          onCollision(currentLocation);
      }
    }
    else if (!this.ignoreMovementAnimations)
    {
      if (this.moveUp)
        this.Sprite.AnimateUp(time);
      else if (this.moveRight)
        this.Sprite.AnimateRight(time);
      else if (this.moveDown)
        this.Sprite.AnimateDown(time);
      else if (this.moveLeft)
        this.Sprite.AnimateLeft(time);
    }
    if (this.blockedInterval >= 5000)
    {
      this.speed = 4;
      this.isCharging = true;
      this.blockedInterval = 0;
    }
    if (this.DamageToFarmer <= 0 || Game1.random.NextDouble() >= 0.00033333333333333332)
      return;
    switch (this.Name)
    {
      case "Shadow Guy":
        if (Game1.random.NextDouble() >= 0.3)
          break;
        if (Game1.random.NextBool())
        {
          currentLocation.playSound("grunt");
          break;
        }
        currentLocation.playSound("shadowpeep");
        break;
      case "Ghost":
        currentLocation.playSound("ghost");
        break;
    }
  }

  /// <summary>Auto-generate a default light source ID for this monster.</summary>
  /// <param name="identifier">A unique ID for this monster instance.</param>
  protected virtual string GenerateLightSourceId(int identifier)
  {
    return $"{this.GetType().Name}_{identifier}";
  }

  public delegate void collisionBehavior(GameLocation location);
}
