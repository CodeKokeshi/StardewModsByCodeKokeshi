// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.MilkPail
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

public class MilkPail : Tool
{
  [XmlIgnore]
  private readonly NetEvent0 finishEvent = new NetEvent0();
  /// <summary>The farm animal the milk pail is being used on, if any.</summary>
  [XmlIgnore]
  public FarmAnimal animal;

  public MilkPail()
    : base("Milk Pail", -1, 6, 6, false)
  {
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new MilkPail();

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
    if (this.animal?.currentProduce.Value != null && this.animal.isAdult() && this.animal.CanGetProduceWithTool((Tool) this) && who.couldInventoryAcceptThisItem(this.animal.currentProduce.Value, 1))
    {
      this.animal.pauseTimer = 1500;
      this.animal.doEmote(20);
      if (this.PlayUseSounds)
        who.playNearbySoundLocal("Milking");
    }
    else if (this.animal?.currentProduce.Value != null && this.animal.isAdult())
    {
      if (who == Game1.player)
      {
        if (!this.animal.CanGetProduceWithTool((Tool) this))
        {
          string harvestTool = this.animal.GetAnimalData()?.HarvestTool;
          if (harvestTool != null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\Tools:MilkPail_Name", (object) harvestTool));
        }
        else if (!who.couldInventoryAcceptThisItem(this.animal.currentProduce.Value, this.animal.hasEatenAnimalCracker.Value ? 2 : 1))
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      }
    }
    else if (who == Game1.player)
    {
      if (this.PlayUseSounds)
      {
        DelayedAction.playSoundAfterDelay("fishingRodBend", 300);
        DelayedAction.playSoundAfterDelay("fishingRodBend", 1200);
      }
      string dialogue = (string) null;
      if (this.animal != null)
        dialogue = this.animal.CanGetProduceWithTool((Tool) this) ? (this.animal.isBaby() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14176", (object) this.animal.displayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14177", (object) this.animal.displayName)) : Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14175", (object) this.animal.displayName);
      if (dialogue != null)
        DelayedAction.showDialogueAfterDelay(dialogue, 1000);
    }
    who.Halt();
    int currentFrame = who.FarmerSprite.CurrentFrame;
    who.FarmerSprite.animateOnce(287 + who.FacingDirection, 50f, 4);
    who.FarmerSprite.oldFrame = currentFrame;
    who.UsingTool = true;
    who.CanMove = false;
    return true;
  }

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
    this.CurrentParentTileIndex = 6;
    this.IndexOfMenuItemView = 6;
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
        if (this.PlayUseSounds)
          Game1.playSound("coin");
        this.animal.currentProduce.Value = (string) null;
        this.animal.friendshipTowardFarmer.Value = Math.Min(1000, this.animal.friendshipTowardFarmer.Value + 5);
        this.animal.ReloadTextureIfNeeded();
        who.gainExperience(0, 5);
      }
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
