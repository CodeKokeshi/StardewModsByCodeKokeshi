// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.MiniJukebox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Objects;

public class MiniJukebox : Object
{
  private bool showNote;

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  public MiniJukebox()
  {
  }

  public MiniJukebox(Vector2 position)
    : base(position, "209")
  {
    this.Name = "Mini-Jukebox";
    this.type.Value = "Crafting";
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    GameLocation location = this.Location;
    if (!location.IsFarm && !location.IsGreenhouse)
    {
      switch (location)
      {
        case Cellar _:
        case IslandWest _:
          break;
        default:
          Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_NotFarmPlay"));
          goto label_8;
      }
    }
    if (location.IsOutdoors && location.IsRainingHere())
    {
      Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_OutdoorRainy"));
    }
    else
    {
      List<string> jukeboxTracks = Utility.GetJukeboxTracks(Game1.player, Game1.player.currentLocation);
      jukeboxTracks.Insert(0, "turn_off");
      jukeboxTracks.Add("random");
      Game1.activeClickableMenu = (IClickableMenu) new ChooseFromListMenu(jukeboxTracks, new ChooseFromListMenu.actionOnChoosingListOption(this.OnSongChosen), true, location.miniJukeboxTrack.Value);
    }
label_8:
    return true;
  }

  public void RegisterToLocation() => this.Location?.OnMiniJukeboxAdded();

  public override void performRemoveAction()
  {
    this.Location?.OnMiniJukeboxRemoved();
    base.performRemoveAction();
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    GameLocation location = this.Location;
    if (location != null && location.IsMiniJukeboxPlaying())
    {
      this.showNextIndex.Value = true;
      if (this.showNote)
      {
        this.showNote = false;
        for (int index = 0; index < 4; ++index)
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(516, 1916, 7, 10), 9999f, 1, 1, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(48 /*0x30*/), -80f), false, false, (float) (((double) this.tileLocation.Value.Y + 1.0) * 64.0 / 10000.0), 0.01f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            xPeriodic = true,
            xPeriodicLoopTime = 1200f,
            xPeriodicRange = 8f,
            motion = new Vector2((float) Game1.random.Next(-10, 10) / 100f, -1f),
            delayBeforeAnimationStart = 1200 + 300 * index
          });
      }
    }
    else
      this.showNextIndex.Value = false;
    base.updateWhenCurrentLocation(time);
  }

  public void OnSongChosen(string selection)
  {
    GameLocation location = this.Location;
    if (location == null)
      return;
    if (selection == "turn_off")
    {
      location.miniJukeboxTrack.Value = "";
    }
    else
    {
      if (selection != location.miniJukeboxTrack.Value)
      {
        this.showNote = true;
        this.shakeTimer = 1000;
      }
      location.miniJukeboxTrack.Value = selection;
      if (!(selection == "random"))
        return;
      location.SelectRandomMiniJukeboxTrack();
    }
  }
}
