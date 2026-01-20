// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.AbandonedJojaMart
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Locations;

public class AbandonedJojaMart : GameLocation
{
  [XmlIgnore]
  private readonly NetEvent0 restoreAreaCutsceneEvent = new NetEvent0();
  [XmlIgnore]
  public NetMutex bundleMutex = new NetMutex();

  public AbandonedJojaMart()
  {
  }

  public AbandonedJojaMart(string mapPath, string name)
    : base(mapPath, name)
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.restoreAreaCutsceneEvent, "restoreAreaCutsceneEvent").AddField((INetSerializable) this.bundleMutex.NetFields, "bundleMutex.NetFields");
    this.restoreAreaCutsceneEvent.onEvent += new NetEvent0.Event(this.doRestoreAreaCutscene);
  }

  public void checkBundle()
  {
    this.bundleMutex.RequestLock((Action) (() => Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(6, Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundlesDict())));
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    this.bundleMutex.Update((GameLocation) this);
    if (this.bundleMutex.IsLockHeld() && Game1.activeClickableMenu == null)
      this.bundleMutex.ReleaseLock();
    this.restoreAreaCutsceneEvent.Poll();
  }

  public void restoreAreaCutscene() => this.restoreAreaCutsceneEvent.Fire();

  private void doRestoreAreaCutscene()
  {
    if (Game1.currentLocation == this)
    {
      Game1.player.freezePause = 1000;
      DelayedAction.removeTileAfterDelay(8, 8, 100, Game1.currentLocation, "Buildings");
      Game1.RequireLocation(nameof (AbandonedJojaMart)).startEvent(new StardewValley.Event(Game1.content.Load<Dictionary<string, string>>("Data\\Events\\AbandonedJojaMart")["missingBundleComplete"], "Data\\Events\\AbandonedJojaMart", "192393"));
    }
    Game1.addMailForTomorrow("ccMovieTheater", true, true);
    if (Game1.player.team.theaterBuildDate.Value >= 0L)
      return;
    Game1.player.team.theaterBuildDate.Set((long) (Game1.Date.TotalDays + 1));
  }

  protected override void resetSharedState()
  {
    bool[] flagArray;
    if (Game1.netWorldState.Value.Bundles.TryGetValue(36, out flagArray) && !this.bundleMutex.IsLocked() && !Game1.eventUp && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
    {
      foreach (bool flag in flagArray)
      {
        if (!flag)
          return;
      }
      this.restoreAreaCutscene();
    }
    base.resetSharedState();
  }
}
