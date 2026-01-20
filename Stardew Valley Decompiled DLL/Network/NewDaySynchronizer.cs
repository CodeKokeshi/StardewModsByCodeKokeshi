// Decompiled with JetBrains decompiler
// Type: StardewValley.NewDaySynchronizer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace StardewValley;

public class NewDaySynchronizer : NetSynchronizer
{
  /// <summary>A flag that clients use during <see cref="M:StardewValley.NewDaySynchronizer.start" /> to determine if they need to wait for the server.</summary>
  private bool ServerReady;
  /// <summary>A flag that used by <see cref="M:StardewValley.NewDaySynchronizer.hasInstance" /> that determines if the <see cref="T:StardewValley.NewDaySynchronizer" /> has a useable signaling context.</summary>
  private bool Instantiated;

  public NewDaySynchronizer()
  {
    this.ServerReady = false;
    this.Instantiated = false;
  }

  /// <summary>Determines if the <see cref="T:StardewValley.NewDaySynchronizer" /> object has a context that can actively be used for synchronization.</summary>
  /// <returns><see langword="true" /> if <see cref="T:StardewValley.NewDaySynchronizer" /> object has a signaling context, <see langword="false" /> otherwise.</returns>
  public bool hasInstance() => this.Instantiated;

  /// <summary>Creates a synchronizer context that can be used for signaling. <see cref="M:StardewValley.NewDaySynchronizer.hasInstance" /> will return <see langword="true" /> after this call.</summary>
  public void create() => this.Instantiated = true;

  /// <summary>Destroys the synchronizer context, such that it can no longer be used for signaling. <see cref="M:StardewValley.NewDaySynchronizer.hasInstance" /> will return <see langword="false" /> after this call.</summary>
  public void destroy()
  {
    this.Instantiated = false;
    this.ServerReady = false;
    this.reset();
  }

  /// <summary>Notifies a client that the server has reached the <see cref="M:StardewValley.NewDaySynchronizer.start" />. <see cref="M:StardewValley.NewDaySynchronizer.start" /> will unblock after this call.</summary>
  public void flagServerReady()
  {
    if (Game1.IsMasterGame)
      return;
    this.ServerReady = true;
  }

  public void start()
  {
    Game1.multiplayer.UpdateEarly();
    if (Game1.IsMasterGame)
    {
      this.ServerReady = true;
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        Game1.server.sendMessage(farmer.UniqueMultiplayerID, new OutgoingMessage((byte) 30, Game1.player, Array.Empty<object>()));
    }
    else
    {
      while (!this.ServerReady)
      {
        this.processMessages();
        if (this.shouldAbort())
        {
          this.ServerReady = false;
          throw new AbortNetSynchronizerException();
        }
        if (LocalMultiplayer.IsLocalMultiplayer())
          break;
      }
    }
  }

  /// <summary>Check if the server has started the synchronization context, so the calling task can <see langword="yield" /> otherwise.</summary>
  public bool hasStarted()
  {
    if (this.ServerReady)
      return true;
    this.processMessages();
    return false;
  }

  public bool readyForFinish()
  {
    Game1.netReady.SetLocalReady("wakeup", true);
    Game1.player.team.Update();
    Game1.multiplayer.UpdateLate();
    Game1.multiplayer.UpdateEarly();
    return Game1.netReady.IsReady("wakeup");
  }

  public bool readyForSave()
  {
    Game1.netReady.SetLocalReady("ready_for_save", true);
    Game1.player.team.Update();
    Game1.multiplayer.UpdateLate();
    Game1.multiplayer.UpdateEarly();
    return Game1.netReady.IsReady("ready_for_save");
  }

  public int numReadyForSave() => Game1.netReady.GetNumberReady("ready_for_save");

  public void finish()
  {
    if (Game1.IsServer)
      this.sendVar<NetBool, bool>("finished", true);
    Game1.multiplayer.UpdateLate();
  }

  public bool hasFinished() => this.hasVar("finished");

  public void flagSaved()
  {
    if (Game1.IsServer)
      this.sendVar<NetBool, bool>("saved", true);
    Game1.multiplayer.UpdateLate();
  }

  public bool hasSaved() => this.hasVar("saved");

  public override void processMessages()
  {
    Game1.multiplayer.UpdateLate();
    Thread.Sleep(16 /*0x10*/);
    Program.sdk.Update();
    Game1.multiplayer.UpdateEarly();
  }

  protected override void sendMessage(params object[] data)
  {
    OutgoingMessage message = new OutgoingMessage((byte) 14, Game1.player, data);
    if (Game1.IsServer)
    {
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        Game1.server.sendMessage(farmer.UniqueMultiplayerID, message);
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage(message);
    }
  }
}
