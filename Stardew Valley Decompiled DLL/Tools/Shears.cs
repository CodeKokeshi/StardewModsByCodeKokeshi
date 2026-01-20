// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Shears
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Tools;

public class Shears : Tool
{
  [XmlIgnore]
  private readonly NetEvent0 finishEvent = new NetEvent0();
  /// <summary>The farm animal the shears are being used on, if any.</summary>
  [XmlIgnore]
  public FarmAnimal animal;

  public Shears()
    : base(nameof (Shears), -1, 7, 7, false)
  {
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Shears();

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.finishEvent, "finishEvent");
    this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
  }

  public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
  {
    x = (int) who.GetToolLocation().X;
    y = (int) who.GetToolLocation().Y;
    Rectangle toolRect = new Rectangle(x - 32 /*0x20*/, y - 32 /*0x20*/, 64 /*0x40*/, 64 /*0x40*/);
    this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>) location.animals.Values, (Tool) this, toolRect);
    who.Halt();
    int currentFrame = who.FarmerSprite.CurrentFrame;
    who.FarmerSprite.animateOnce(283 + who.FacingDirection, 50f, 4);
    who.FarmerSprite.oldFrame = currentFrame;
    who.UsingTool = true;
    who.CanMove = false;
    return true;
  }

  public static void playSnip(Farmer who) => who.playNearbySoundAll("scissors");

  public override void tickUpdate(GameTime time, Farmer who)
  {
    this.lastUser = who;
    base.tickUpdate(time, who);
    this.finishEvent.Poll();
  }

  public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
  {
    base.DoFunction(location, x, y, power, who);
    who.Stamina -= 4f;
    if (this.PlayUseSounds)
      Shears.playSnip(who);
    this.CurrentParentTileIndex = 7;
    this.IndexOfMenuItemView = 7;
    if (this.animal?.currentProduce.Value != null && this.animal.isAdult() && this.animal.CanGetProduceWithTool((Tool) this))
    {
      StardewValley.Object @object = ItemRegistry.Create<StardewValley.Object>("(O)" + this.animal.currentProduce.Value);
      @object.CanBeSetDown = false;
      @object.Quality = this.animal.produceQuality.Value;
      if (this.animal.hasEatenAnimalCracker.Value)
        @object.Stack = 2;
      if (who.addItemToInventoryBool((Item) @object))
      {
        this.animal.HandleStatsOnProduceCollected((Item) @object, (uint) @object.Stack);
        this.animal.currentProduce.Value = (string) null;
        if (this.PlayUseSounds)
          Game1.playSound("coin");
        this.animal.friendshipTowardFarmer.Value = Math.Min(1000, this.animal.friendshipTowardFarmer.Value + 5);
        this.animal.ReloadTextureIfNeeded();
        who.gainExperience(0, 5);
      }
    }
    else
    {
      string dialogue = (string) null;
      if (this.animal != null)
        dialogue = this.animal.CanGetProduceWithTool((Tool) this) ? (this.animal.isBaby() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14246", (object) this.animal.displayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14247", (object) this.animal.displayName)) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14245", (object) this.animal.displayName);
      if (dialogue != null)
        Game1.drawObjectDialogue(dialogue);
    }
    this.finish();
  }

  private void finish() => this.finishEvent.Fire();

  private void doFinish()
  {
    this.animal = (FarmAnimal) null;
    this.lastUser.CanMove = true;
    this.lastUser.completelyStopAnimatingOrDoingAction();
    this.lastUser.UsingTool = false;
    this.lastUser.canReleaseTool = true;
  }
}
