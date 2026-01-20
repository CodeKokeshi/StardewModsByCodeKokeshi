// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.IslandFieldOffice
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Locations;

public class IslandFieldOffice : IslandLocation
{
  public const int totalPieces = 11;
  public const int piece_Skeleton_Back_Leg = 0;
  public const int piece_Skeleton_Ribs = 1;
  public const int piece_Skeleton_Front_Leg = 2;
  public const int piece_Skeleton_Tail = 3;
  public const int piece_Skeleton_Spine = 4;
  public const int piece_Skeleton_Skull = 5;
  public const int piece_Snake_Tail = 6;
  public const int piece_Snake_Spine = 7;
  public const int piece_Snake_Skull = 8;
  public const int piece_Bat = 9;
  public const int piece_Frog = 10;
  [XmlElement("uncollectedRewards")]
  public NetList<Item, NetRef<Item>> uncollectedRewards;
  [XmlIgnore]
  public NetMutex safariGuyMutex;
  private NPC safariGuy;
  [XmlElement("piecesDonated")]
  public NetList<bool, NetBool> piecesDonated;
  [XmlElement("centerSkeletonRestored")]
  public readonly NetBool centerSkeletonRestored;
  [XmlElement("snakeRestored")]
  public readonly NetBool snakeRestored;
  [XmlElement("batRestored")]
  public readonly NetBool batRestored;
  [XmlElement("frogRestored")]
  public readonly NetBool frogRestored;
  [XmlElement("plantsRestoredLeft")]
  public readonly NetBool plantsRestoredLeft;
  [XmlElement("plantsRestoredRight")]
  public readonly NetBool plantsRestoredRight;
  public readonly NetBool hasFailedSurveyToday;
  private bool _shouldTriggerFinalCutscene;
  private float speakerTimer;

  public IslandFieldOffice()
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.centerSkeletonRestored = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.snakeRestored = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.batRestored = netBool3;
    NetBool netBool4 = new NetBool();
    netBool4.InterpolationWait = false;
    this.frogRestored = netBool4;
    NetBool netBool5 = new NetBool();
    netBool5.InterpolationWait = false;
    this.plantsRestoredLeft = netBool5;
    NetBool netBool6 = new NetBool();
    netBool6.InterpolationWait = false;
    this.plantsRestoredRight = netBool6;
    this.hasFailedSurveyToday = new NetBool();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public IslandFieldOffice(string map, string name)
  {
    NetBool netBool1 = new NetBool();
    netBool1.InterpolationWait = false;
    this.centerSkeletonRestored = netBool1;
    NetBool netBool2 = new NetBool();
    netBool2.InterpolationWait = false;
    this.snakeRestored = netBool2;
    NetBool netBool3 = new NetBool();
    netBool3.InterpolationWait = false;
    this.batRestored = netBool3;
    NetBool netBool4 = new NetBool();
    netBool4.InterpolationWait = false;
    this.frogRestored = netBool4;
    NetBool netBool5 = new NetBool();
    netBool5.InterpolationWait = false;
    this.plantsRestoredLeft = netBool5;
    NetBool netBool6 = new NetBool();
    netBool6.InterpolationWait = false;
    this.plantsRestoredRight = netBool6;
    this.hasFailedSurveyToday = new NetBool();
    // ISSUE: explicit constructor call
    base.\u002Ector(map, name);
    while (this.piecesDonated.Count < 11)
      this.piecesDonated.Add(false);
  }

  public NPC getSafariGuy() => this.safariGuy;

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.piecesDonated, "piecesDonated").AddField((INetSerializable) this.centerSkeletonRestored, "centerSkeletonRestored").AddField((INetSerializable) this.snakeRestored, "snakeRestored").AddField((INetSerializable) this.batRestored, "batRestored").AddField((INetSerializable) this.frogRestored, "frogRestored").AddField((INetSerializable) this.plantsRestoredLeft, "plantsRestoredLeft").AddField((INetSerializable) this.plantsRestoredRight, "plantsRestoredRight").AddField((INetSerializable) this.uncollectedRewards, "uncollectedRewards").AddField((INetSerializable) this.hasFailedSurveyToday, "hasFailedSurveyToday").AddField((INetSerializable) this.safariGuyMutex.NetFields, "safariGuyMutex.NetFields");
    this.centerSkeletonRestored.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplySkeletonRestore();
    });
    this.snakeRestored.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplySnakeRestore();
    });
    this.batRestored.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyBatRestore();
    });
    this.frogRestored.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyFrogRestore();
    });
    this.plantsRestoredLeft.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyPlantRestoreLeft();
    });
    this.plantsRestoredRight.fieldChangeEvent += (FieldChange<NetBool, bool>) ((f, oldValue, newValue) =>
    {
      if (!newValue || this.mapPath.Value == null)
        return;
      this.ApplyPlantRestoreRight();
    });
  }

  private void ApplyPlantRestoreLeft()
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f, new Color(0, 220, 150))
    {
      layerDepth = 1f,
      motion = new Vector2(1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 220, 150) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      layerDepth = 1f,
      motion = new Vector2(-1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 220, 150) * 0.75f)
    {
      scale = 0.75f,
      delayBeforeAnimationStart = 50,
      layerDepth = 1f,
      motion = new Vector2(1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 220, 150) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      delayBeforeAnimationStart = 100,
      layerDepth = 1f,
      motion = new Vector2(-1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(1.1f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(250, 100, 250) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      delayBeforeAnimationStart = 150,
      layerDepth = 1f,
      motion = new Vector2(0.0f, -3f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    if (Game1.gameMode == (byte) 6 || Utility.ShouldIgnoreValueChangeCallback())
      return;
    if (Game1.currentLocation == this)
    {
      Game1.playSound("leafrustle");
      DelayedAction.playSoundAfterDelay("leafrustle", 150);
    }
    if (!Game1.IsMasterGame)
      return;
    Game1.player.team.MarkCollectedNut("IslandLeftPlantRestored");
    if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130)
      return;
    Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(1.5f, 3.3f) * 64f, 1, (GameLocation) this, 256 /*0x0100*/);
  }

  private void ApplyPlantRestoreRight()
  {
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(7.5f, 3.3f) * 64f, new Color(0, 220, 150))
    {
      layerDepth = 1f,
      motion = new Vector2(1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 220, 150) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      layerDepth = 1f,
      motion = new Vector2(-1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8.3f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 200, 120) * 0.75f)
    {
      scale = 0.75f,
      delayBeforeAnimationStart = 50,
      layerDepth = 1f,
      motion = new Vector2(1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 220, 150) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      delayBeforeAnimationStart = 100,
      layerDepth = 1f,
      motion = new Vector2(-1f, -4f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    this.temporarySprites.Add(new TemporaryAnimatedSprite(50, new Vector2(8.5f, 3.3f) * 64f + new Vector2((float) Game1.random.Next(-16, 16 /*0x10*/), (float) Game1.random.Next(-48, 48 /*0x30*/)), new Color(0, 250, 180) * 0.75f)
    {
      scale = 0.75f,
      flipped = true,
      delayBeforeAnimationStart = 150,
      layerDepth = 1f,
      motion = new Vector2(0.0f, -3f),
      acceleration = new Vector2(0.0f, 0.1f)
    });
    if (Game1.gameMode == (byte) 6 || Utility.ShouldIgnoreValueChangeCallback())
      return;
    if (Game1.currentLocation == this)
    {
      Game1.playSound("leafrustle");
      DelayedAction.playSoundAfterDelay("leafrustle", 150);
    }
    if (!Game1.IsMasterGame)
      return;
    Game1.player.team.MarkCollectedNut("IslandRightPlantRestored");
    if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130)
      return;
    Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(7.5f, 3.3f) * 64f, 3, (GameLocation) this, 256 /*0x0100*/);
  }

  private void ApplyFrogRestore()
  {
    if (Game1.gameMode != (byte) 6 && !Utility.ShouldIgnoreValueChangeCallback() && Game1.currentLocation == this)
      Game1.playSound("dirtyHit");
    for (int index = 0; index < 3; ++index)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (6.5 + (double) Game1.random.Next(-10, 11) / 100.0), 3f) * 64f, false, 0.007f, Color.White)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -1f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 4f,
        scaleChange = 0.02f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
        delayBeforeAnimationStart = index * 100
      });
  }

  private void ApplyBatRestore()
  {
    if (Game1.gameMode != (byte) 6 && !Utility.ShouldIgnoreValueChangeCallback() && Game1.currentLocation == this)
      Game1.playSound("dirtyHit");
    for (int index = 0; index < 3; ++index)
      this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2((float) (2.5 + (double) Game1.random.Next(-10, 11) / 100.0), 3f) * 64f, false, 0.007f, Color.White)
      {
        alpha = 0.75f,
        motion = new Vector2(0.0f, -1f),
        acceleration = new Vector2(1f / 500f, 0.0f),
        interval = 99999f,
        layerDepth = 1f,
        scale = 4f,
        scaleChange = 0.02f,
        rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
        delayBeforeAnimationStart = index * 100
      });
  }

  private void ApplySnakeRestore()
  {
  }

  private void ApplySkeletonRestore()
  {
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    base.TransferDataFromSavedLocation(l);
    IslandFieldOffice islandFieldOffice = l as IslandFieldOffice;
    this.uncollectedRewards.Clear();
    this.uncollectedRewards.Set((IList<Item>) islandFieldOffice.uncollectedRewards);
    this.piecesDonated.Clear();
    this.piecesDonated.Set((IList<bool>) islandFieldOffice.piecesDonated);
    this.centerSkeletonRestored.Value = islandFieldOffice.centerSkeletonRestored.Value;
    this.snakeRestored.Value = islandFieldOffice.snakeRestored.Value;
    this.batRestored.Value = islandFieldOffice.batRestored.Value;
    this.frogRestored.Value = islandFieldOffice.frogRestored.Value;
    this.plantsRestoredLeft.Value = islandFieldOffice.plantsRestoredLeft.Value;
    this.plantsRestoredRight.Value = islandFieldOffice.plantsRestoredRight.Value;
    this.hasFailedSurveyToday.Value = islandFieldOffice.hasFailedSurveyToday.Value;
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    if (Game1.player.hasOrWillReceiveMail("islandNorthCaveOpened") && this.safariGuy == null)
    {
      this.safariGuy = new NPC(new AnimatedSprite("Characters\\SafariGuy", 0, 16 /*0x10*/, 32 /*0x20*/), new Vector2(8f, 6f) * 64f, "IslandFieldOFfice", 2, "Professor Snail", false, Game1.content.Load<Texture2D>("Portraits\\SafariGuy"));
      this.safariGuy.AllowDynamicAppearance = false;
      this.safariGuy.displayName = Game1.content.LoadString("Strings\\NPCNames:ProfessorSnail");
    }
    if (this.safariGuy != null && !Game1.player.hasOrWillReceiveMail("safariGuyIntro"))
    {
      this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Intro_Event")));
      Game1.player.mailReceived.Add("safariGuyIntro");
      Game1.player.Halt();
    }
    else
    {
      if (this.safariGuy != null)
      {
        Game1.changeMusicTrack("fieldofficeTentMusic");
        if (Game1.random.NextBool())
        {
          this.safariGuy.Halt();
          this.safariGuy.showTextAboveHead(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Welcome_" + Game1.random.Next(4).ToString()));
          this.safariGuy.faceTowardFarmerForPeriod(60000, 5, false, Game1.player);
        }
        else
          this.safariGuy.Sprite.CurrentAnimation = new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(18, 900, 0, false, false),
            new FarmerSprite.AnimationFrame(19, 900, 0, false, false)
          };
      }
      if (Game1.player.hasOrWillReceiveMail("fieldOfficeFinale") || !this.isRangeAllTrue(0, 11) || !this.plantsRestoredRight.Value || !this.plantsRestoredLeft.Value || this.currentEvent != null)
        return;
      this._StartFinaleEvent();
    }
  }

  /// <summary>returns true if a new uncollected reward was added.</summary>
  /// <param name="which"></param>
  /// <returns></returns>
  public bool donatePiece(int which)
  {
    this.piecesDonated[which] = true;
    if (!this.centerSkeletonRestored.Value && this.isRangeAllTrue(0, 6))
    {
      this.centerSkeletonRestored.Value = true;
      if (Game1.netWorldState.Value.GoldenWalnutsFound < 130)
        this.uncollectedRewards.Add(ItemRegistry.Create("(O)73", 6));
      this.uncollectedRewards.Add(ItemRegistry.Create("(O)69"));
      Game1.player.team.MarkCollectedNut("IslandCenterSkeletonRestored");
      return true;
    }
    if (!this.snakeRestored.Value && this.isRangeAllTrue(6, 9))
    {
      this.snakeRestored.Value = true;
      if (Game1.netWorldState.Value.GoldenWalnutsFound < 130)
        this.uncollectedRewards.Add(ItemRegistry.Create("(O)73", 3));
      this.uncollectedRewards.Add(ItemRegistry.Create("(O)835"));
      Game1.player.team.MarkCollectedNut("IslandSnakeRestored");
      return true;
    }
    if (!this.batRestored.Value && this.piecesDonated[9])
    {
      this.batRestored.Value = true;
      if (Game1.netWorldState.Value.GoldenWalnutsFound < 130)
        this.uncollectedRewards.Add(ItemRegistry.Create("(O)73"));
      else
        this.uncollectedRewards.Add(ItemRegistry.Create("(O)TentKit"));
      Game1.player.team.MarkCollectedNut("IslandBatRestored");
      return true;
    }
    if (this.frogRestored.Value || !this.piecesDonated[10])
      return false;
    this.frogRestored.Value = true;
    if (Game1.netWorldState.Value.GoldenWalnutsFound < 130)
      this.uncollectedRewards.Add(ItemRegistry.Create("(O)73"));
    else
      this.uncollectedRewards.Add(ItemRegistry.Create("(O)926"));
    Game1.player.team.MarkCollectedNut("IslandFrogRestored");
    return true;
  }

  public bool isRangeAllTrue(int low, int high)
  {
    for (int index = low; index < high; ++index)
    {
      if (!this.piecesDonated[index])
        return false;
    }
    return true;
  }

  public void triggerFinaleCutscene() => this._shouldTriggerFinalCutscene = true;

  private void _triggerFinaleCutsceneActual()
  {
    Game1.player.Halt();
    Game1.player.freezePause = 500;
    DelayedAction.functionAfterDelay((Action) (() =>
    {
      if (Game1.activeClickableMenu != null)
        Game1.activeClickableMenu = (IClickableMenu) null;
      Game1.globalFadeToBlack(new Game1.afterFadeFunction(this._StartFinaleEvent));
    }), 500);
    this._shouldTriggerFinalCutscene = false;
  }

  protected void _StartFinaleEvent()
  {
    this.safariGuy?.clearTextAboveHead();
    this.startEvent(new StardewValley.Event(Game1.content.LoadString("Strings\\Locations:FieldOfficeFinale")));
  }

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (this.safariGuy != null && !Game1.eventUp)
      this.safariGuy.draw(b);
    if (this.centerSkeletonRestored.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(3f, 4f) * 64f + new Vector2(0.0f, 4f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(210, 184, 46, 43)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0512f);
    if (this.snakeRestored.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(1f, 5f) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(195, 185, 14, 42)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448f);
    if (this.batRestored.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(2.5f, 2.7f) * 64f + new Vector2(1f, 1f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(212, 171, 16 /*0x10*/, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
    if (this.frogRestored.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(6f, 2f) * 64f + new Vector2(9f, 10f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(232, 169, 14, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0256f);
    if (this.plantsRestoredLeft.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(1f, 4f) * 64f + new Vector2(0.0f, -7f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194, 167, 16 /*0x10*/, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.032f);
    if (this.plantsRestoredRight.Value)
      b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(new Vector2(7f, 3f) * 64f + new Vector2(8f, 3f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(224 /*0xE0*/, 148, 32 /*0x20*/, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.032f);
    if (this.safariGuy == null || this.plantsRestoredLeft.Value && this.plantsRestoredRight.Value || Game1.eventUp)
      return;
    float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2));
    b.Draw(Game1.mouseCursors2, Game1.GlobalToLocal(Game1.viewport, new Vector2(324f, 144f + num)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(220, 160 /*0xA0*/, 3, 8)), Color.White, 0.0f, new Vector2(1f, 4f), 4f + Math.Max(0.0f, (float) (0.25 - (double) num / 16.0)), SpriteEffects.None, 1f);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    base.drawAboveAlwaysFrontLayer(b);
    this.safariGuy?.drawAboveAlwaysFrontLayer(b);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    base.UpdateWhenCurrentLocation(time);
    this.safariGuyMutex.Update((GameLocation) this);
    if (this.safariGuy != null)
    {
      this.safariGuy.update(time, (GameLocation) this);
      this.speakerTimer -= (float) time.ElapsedGameTime.TotalMilliseconds;
      if ((double) this.speakerTimer <= 0.0)
      {
        this.speakerTimer = 600f;
        this.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(211, 161, 5, 5), new Vector2(74.75f, 20.75f) * 4f, false, 0.0f, Color.White)
        {
          scale = 5f,
          scaleChange = -0.05f,
          motion = new Vector2(0.125f, 0.125f),
          animationLength = 1,
          totalNumberOfLoops = 1,
          interval = 400f,
          layerDepth = 1f
        });
      }
    }
    if (Game1.currentLocation != this || !this._shouldTriggerFinalCutscene || Game1.activeClickableMenu != null)
      return;
    this._triggerFinaleCutsceneActual();
  }

  public virtual void OnCollectReward(Item item, Farmer farmer)
  {
    if (!(Game1.activeClickableMenu is ItemGrabMenu activeClickableMenu) || activeClickableMenu.context != this)
      return;
    if (Game1.player.addItemToInventoryBool(activeClickableMenu.heldItem))
    {
      this.uncollectedRewards.Remove(item);
      activeClickableMenu.ItemsToGrabMenu.actualInventory = (IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards);
      activeClickableMenu.heldItem = (Item) null;
      if (!(item.QualifiedItemId != "(O)73"))
        return;
      Game1.playSound("coin");
    }
    else
    {
      Game1.playSound("cancel");
      Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      activeClickableMenu.ItemsToGrabMenu.actualInventory = (IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards);
      activeClickableMenu.heldItem = (Item) null;
    }
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    if (questionAndAnswer == null)
      return false;
    if (questionAndAnswer != null)
    {
      switch (questionAndAnswer.Length)
      {
        case 10:
          if (questionAndAnswer == "Survey_Yes")
          {
            if (!this.plantsRestoredLeft.Value)
            {
              List<Response> responseList = new List<Response>();
              for (int index = 18; index < 25; ++index)
                responseList.Add(new Response(index == 22 ? "Correct" : "Wrong", index.ToString() ?? ""));
              responseList.Add(new Response("No", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")).SetHotKey(Keys.Escape));
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Question"), responseList.ToArray(), "PurpleFlowerSurvey");
              break;
            }
            if (!this.plantsRestoredRight.Value)
            {
              List<Response> responseList = new List<Response>();
              for (int index = 11; index < 19; ++index)
                responseList.Add(new Response(index == 18 ? "Correct" : "Wrong", index.ToString() ?? ""));
              responseList.Add(new Response("No", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel")).SetHotKey(Keys.Escape));
              this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_PurpleStarfish_Question"), responseList.ToArray(), "PurpleStarfishSurvey");
              break;
            }
            break;
          }
          break;
        case 11:
          if (questionAndAnswer == "Safari_Hint")
          {
            int num = this.getRandomUnfoundBoneIndex();
            if (num == 823)
              num = 824;
            Game1.DrawDialogue(this.safariGuy, "Data\\ExtraDialogue:ProfessorSnail_Hint_" + num.ToString());
            break;
          }
          break;
        case 12:
          if (questionAndAnswer == "Safari_Leave")
          {
            this.safariGuyMutex.ReleaseLock();
            break;
          }
          break;
        case 13:
          if (questionAndAnswer == "Safari_Donate")
          {
            Game1.activeClickableMenu = (IClickableMenu) new FieldOfficeMenu(this);
            Game1.activeClickableMenu.exitFunction += new IClickableMenu.onExit(this.safariGuyMutex.ReleaseLock);
            break;
          }
          break;
        case 14:
          if (questionAndAnswer == "Safari_Collect")
          {
            Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new List<Item>((IEnumerable<Item>) this.uncollectedRewards), false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect(this.OnCollectReward), canBeExitedWithKey: true, playRightClickSound: false, allowRightClick: false, context: (object) this);
            Game1.activeClickableMenu.exitFunction += new IClickableMenu.onExit(this.safariGuyMutex.ReleaseLock);
            break;
          }
          break;
        case 24:
          if (questionAndAnswer == "PurpleFlowerSurvey_Wrong")
          {
            Game1.DrawDialogue(this.safariGuy, "Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Wrong");
            this.hasFailedSurveyToday.Value = true;
            break;
          }
          break;
        case 26:
          switch (questionAndAnswer[6])
          {
            case 'F':
              if (questionAndAnswer == "PurpleFlowerSurvey_Correct")
              {
                Game1.DrawDialogue(this.safariGuy, "Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Correct");
                this.plantsRestoredLeft.Value = true;
                Game1.multiplayer.globalChatInfoMessage("FinishedSurvey", Game1.player.name.Value);
                break;
              }
              break;
            case 'S':
              if (questionAndAnswer == "PurpleStarfishSurvey_Wrong")
              {
                Game1.DrawDialogue(this.safariGuy, "Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Wrong");
                this.hasFailedSurveyToday.Value = true;
                break;
              }
              break;
          }
          break;
        case 28:
          if (questionAndAnswer == "PurpleStarfishSurvey_Correct")
          {
            Game1.DrawDialogue(this.safariGuy, "Strings\\Locations:IslandFieldOffice_Survey_PurpleFlower_Correct");
            this.plantsRestoredRight.Value = true;
            Game1.multiplayer.globalChatInfoMessage("FinishedSurvey", Game1.player.name.Value);
            break;
          }
          break;
      }
    }
    if (!Game1.player.hasOrWillReceiveMail("fieldOfficeFinale") && this.isRangeAllTrue(0, 11) && this.plantsRestoredRight.Value && this.plantsRestoredLeft.Value)
      this.triggerFinaleCutscene();
    return base.answerDialogueAction(questionAndAnswer, questionParams);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    this.hasFailedSurveyToday.Value = false;
    base.DayUpdate(dayOfMonth);
  }

  public virtual void TalkToSafariGuy()
  {
    List<Response> responseList = new List<Response>();
    responseList.Add(new Response("Donate", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Donate")));
    if (this.uncollectedRewards.Count > 0)
      responseList.Add(new Response("Collect", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Collect")));
    if (this.getRandomUnfoundBoneIndex() != -1)
      responseList.Add(new Response("Hint", Game1.content.LoadString("Strings\\Locations:Hint")));
    responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ArchaeologyHouse_Gunther_Leave")));
    this.createQuestionDialogue("", responseList.ToArray(), "Safari");
  }

  private int getRandomUnfoundBoneIndex()
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
    for (int index = 0; index < 25; ++index)
    {
      int num = random.Next(11);
      if (!this.piecesDonated[num])
        return FieldOfficeMenu.getDonationPieceIndexNeededForSpot(num);
    }
    for (int index = 0; index < this.piecesDonated.Count; ++index)
    {
      if (!this.piecesDonated[index])
        return FieldOfficeMenu.getDonationPieceIndexNeededForSpot(index);
    }
    return -1;
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    switch (ArgUtility.Get(action, 0))
    {
      case "FieldOfficeDesk":
        if (this.safariGuy != null)
        {
          this.safariGuyMutex.RequestLock(new Action(this.TalkToSafariGuy));
          return true;
        }
        break;
      case "FieldOfficeSurvey":
        if (this.safariGuy != null)
        {
          if (this.hasFailedSurveyToday.Value)
          {
            Game1.DrawDialogue(this.safariGuy, "Strings\\Locations:IslandFieldOffice_Survey_Failed");
            return true;
          }
          if (!this.plantsRestoredLeft.Value)
          {
            Response[] answerChoices = new Response[2]
            {
              new Response("Yes", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Yes")),
              new Response("No", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Notyet"))
            };
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Prompt_LeftPlant"), answerChoices, "Survey");
            (Game1.activeClickableMenu as DialogueBox).aboveDialogueImage = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(194, 167, 16 /*0x10*/, 17), 1f, 1, 1, Vector2.Zero, false, false)
            {
              scale = 4f
            };
          }
          else if (!this.plantsRestoredRight.Value)
          {
            Response[] answerChoices = new Response[2]
            {
              new Response("Yes", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Yes")),
              new Response("No", Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Notyet"))
            };
            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:IslandFieldOffice_Survey_Prompt_RightPlant"), answerChoices, "Survey");
            (Game1.activeClickableMenu as DialogueBox).aboveDialogueImage = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(193, 150, 16 /*0x10*/, 16 /*0x10*/), 1f, 1, 1, Vector2.Zero, false, false)
            {
              scale = 4f
            };
          }
          return true;
        }
        break;
    }
    return base.performAction(action, who, tileLocation);
  }
}
