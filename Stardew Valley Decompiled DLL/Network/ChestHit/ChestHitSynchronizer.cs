// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.ChestHit.ChestHitSynchronizer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network.ChestHit;

/// <summary>Prevents race conditions when multiplayer players hit a chest.</summary>
public sealed class ChestHitSynchronizer
{
  /// <summary>The queue of chest hit events to process during the next <see cref="M:StardewValley.Network.ChestHit.ChestHitSynchronizer.Update" />, maintained by the host.</summary>
  private readonly Queue<ChestHitArgs> EventQueue = new Queue<ChestHitArgs>();
  /// <summary>A map of chests and their first tool hit timers, maintained by each farmhand.</summary>
  internal readonly Dictionary<string, Dictionary<ulong, ChestHitTimer>> SavedTimers = new Dictionary<string, Dictionary<ulong, ChestHitTimer>>();

  /// <summary>Resets the chest hit synchronizer, used in <see cref="M:StardewValley.FarmerTeam.NewDay" />.</summary>
  public void Reset()
  {
    this.EventQueue.Clear();
    this.SavedTimers.Clear();
  }

  /// <summary>Processes all of the chest hit events in <see cref="F:StardewValley.Network.ChestHit.ChestHitSynchronizer.EventQueue" />, used in <see cref="M:StardewValley.FarmerTeam.Update" />.</summary>
  public void Update()
  {
    if (!Game1.IsMasterGame)
      return;
    while (this.EventQueue.Count > 0)
    {
      ChestHitArgs args = this.EventQueue.Dequeue();
      if (args == null)
        break;
      if (args.Location?.getObjectAtTile(args.ChestTile.X, args.ChestTile.Y, true) is Chest objectAtTile)
        objectAtTile.HandleChestHit(args);
    }
  }

  /// <summary>Synchronizes a player hitting a chest.</summary>
  /// <param name="args">The arguments for the chest hit event.</param>
  public void Sync(ChestHitArgs args)
  {
    if (!(args.Location?.getObjectAtTile(args.ChestTile.X, args.ChestTile.Y, true) is Chest objectAtTile))
      return;
    if (Game1.IsMasterGame)
    {
      this.EventQueue.Enqueue(args);
    }
    else
    {
      if (objectAtTile.hitTimerInstance != null)
      {
        ChestHitTimer hitTimerInstance = objectAtTile.hitTimerInstance;
        GameTime currentGameTime = Game1.currentGameTime;
        int num = currentGameTime != null ? (int) currentGameTime.TotalGameTime.TotalMilliseconds : -999;
        hitTimerInstance.SavedTime = num;
        Dictionary<ulong, ChestHitTimer> dictionary;
        if (!this.SavedTimers.TryGetValue(args.Location.NameOrUniqueName, out dictionary))
        {
          dictionary = new Dictionary<ulong, ChestHitTimer>();
          this.SavedTimers.Add(args.Location.NameOrUniqueName, dictionary);
        }
        dictionary[ChestHitSynchronizer.HashPosition(args.ChestTile.X, args.ChestTile.Y)] = objectAtTile.hitTimerInstance;
      }
      Game1.client?.sendMessage(new OutgoingMessage((byte) 32 /*0x20*/, Game1.player, new object[12]
      {
        (object) (byte) 0,
        (object) args.Location.isStructure.Value,
        (object) args.Location.NameOrUniqueName,
        (object) args.ChestTile.X,
        (object) args.ChestTile.Y,
        (object) args.ToolPosition,
        (object) args.StandingPixel.X,
        (object) args.StandingPixel.Y,
        (object) args.Direction,
        (object) args.HoldDownClick,
        (object) args.ToolCanHit,
        (object) args.RecentlyHit
      }));
    }
  }

  /// <summary>Signals that a chest has been moved.</summary>
  /// <param name="location">The parent location of the moved chest.</param>
  /// <param name="sourceTileX">The old, pre-move x-coordinate of the chest.</param>
  /// <param name="sourceTileY">The old, pre-move y-coordinate of the chest.</param>
  /// <param name="destTileX">The new, post-move x-coordinate of the chest.</param>
  /// <param name="destTileY">The new, post-move y-coordinate of the chest.</param>
  public void SignalMove(
    GameLocation location,
    int sourceTileX,
    int sourceTileY,
    int destTileX,
    int destTileY)
  {
    if (Game1.server == null || location == null)
      return;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      Game1.server.sendMessage(farmer.UniqueMultiplayerID, new OutgoingMessage((byte) 32 /*0x20*/, Game1.player, new object[6]
      {
        (object) (byte) 1,
        (object) location.NameOrUniqueName,
        (object) sourceTileX,
        (object) sourceTileY,
        (object) destTileX,
        (object) destTileY
      }));
  }

  /// <summary>Signals that a chest has been deleted.</summary>
  /// <param name="location">The parent location of the deleted chest.</param>
  /// <param name="tileX">The x-coordinate of the deleted chest.</param>
  /// <param name="tileY">The y-coordinate of the deleted chest.</param>
  public void SignalDelete(GameLocation location, int tileX, int tileY)
  {
    if (Game1.server == null || location == null)
      return;
    foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      Game1.server.sendMessage(farmer.UniqueMultiplayerID, new OutgoingMessage((byte) 32 /*0x20*/, Game1.player, new object[4]
      {
        (object) (byte) 2,
        (object) location.NameOrUniqueName,
        (object) tileX,
        (object) tileY
      }));
  }

  /// <summary>Process an incoming chest hit sync message.</summary>
  /// <param name="message">The incoming sync message.</param>
  public void ProcessMessage(IncomingMessage message)
  {
    switch (message.Reader.ReadByte())
    {
      case 0:
        this.ProcessSync(message);
        break;
      case 1:
        this.ProcessMove(message);
        break;
      case 2:
        this.ProcessDelete(message);
        break;
    }
  }

  /// <summary>Packs the two integer components of the tile coordinate into an unsigned long.</summary>
  /// <param name="x">The x-component of the tile coordinate.</param>
  /// <param name="y">The y-component of the tile coordinate.</param>
  internal static ulong HashPosition(int x, int y)
  {
    return (ulong) (uint) x << 32 /*0x20*/ | (ulong) (uint) y;
  }

  private static GameLocation ReadLocation(IncomingMessage message)
  {
    bool isStructure = message.Reader.ReadBoolean();
    GameLocation locationFromName = Game1.getLocationFromName(message.Reader.ReadString(), isStructure);
    return locationFromName == null || Game1.multiplayer.locationRoot(locationFromName) == null ? (GameLocation) null : locationFromName;
  }

  /// <summary>Handle a request to synchronize a player hitting a chest.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.ChestHit.Internal.ChestHitMessageType.Sync" /> message.</param>
  private void ProcessSync(IncomingMessage message)
  {
    if (!Game1.IsMasterGame)
    {
      Game1.log.Warn("Unexpectedly received a chest hit sync message as a farmhand.");
    }
    else
    {
      ChestHitArgs chestHitArgs = new ChestHitArgs();
      bool isStructure = message.Reader.ReadBoolean();
      string name = message.Reader.ReadString();
      chestHitArgs.Location = Game1.getLocationFromName(name, isStructure);
      if (chestHitArgs.Location == null || Game1.multiplayer.locationRoot(chestHitArgs.Location) == null)
        return;
      chestHitArgs.ChestTile.X = message.Reader.ReadInt32();
      chestHitArgs.ChestTile.Y = message.Reader.ReadInt32();
      chestHitArgs.ToolPosition.X = message.Reader.ReadSingle();
      chestHitArgs.ToolPosition.Y = message.Reader.ReadSingle();
      chestHitArgs.StandingPixel.X = message.Reader.ReadInt32();
      chestHitArgs.StandingPixel.Y = message.Reader.ReadInt32();
      chestHitArgs.Direction = message.Reader.ReadInt32();
      chestHitArgs.HoldDownClick = message.Reader.ReadBoolean();
      chestHitArgs.ToolCanHit = message.Reader.ReadBoolean();
      chestHitArgs.RecentlyHit = message.Reader.ReadBoolean();
      this.EventQueue.Enqueue(chestHitArgs);
    }
  }

  /// <summary>Handle an incoming signal that a chest has moved.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.ChestHit.Internal.ChestHitMessageType.Move" /> message.</param>
  private void ProcessMove(IncomingMessage message)
  {
    if (Game1.IsMasterGame)
    {
      Game1.log.Warn("Unexpectedly received a chest move message as the host.");
    }
    else
    {
      string key1 = message.Reader.ReadString();
      if (key1 == null)
        return;
      int x1 = message.Reader.ReadInt32();
      int y1 = message.Reader.ReadInt32();
      int x2 = message.Reader.ReadInt32();
      int y2 = message.Reader.ReadInt32();
      Dictionary<ulong, ChestHitTimer> dictionary;
      if (!this.SavedTimers.TryGetValue(key1, out dictionary))
        return;
      ulong key2 = ChestHitSynchronizer.HashPosition(x1, y1);
      ChestHitTimer chestHitTimer;
      if (!dictionary.TryGetValue(key2, out chestHitTimer))
        return;
      dictionary.Remove(key2);
      dictionary.TryAdd(ChestHitSynchronizer.HashPosition(x2, y2), chestHitTimer);
    }
  }

  /// <summary>Handle an incoming signal that a chest has been deleted.</summary>
  /// <param name="message">The incoming <see cref="F:StardewValley.Network.ChestHit.Internal.ChestHitMessageType.Delete" /> message.</param>
  private void ProcessDelete(IncomingMessage message)
  {
    if (Game1.IsMasterGame)
    {
      Game1.log.Warn("Unexpectedly received a chest delete message as the host.");
    }
    else
    {
      string key = message.Reader.ReadString();
      if (key == null)
        return;
      int x = message.Reader.ReadInt32();
      int y = message.Reader.ReadInt32();
      Dictionary<ulong, ChestHitTimer> dictionary;
      if (!this.SavedTimers.TryGetValue(key, out dictionary))
        return;
      dictionary.Remove(ChestHitSynchronizer.HashPosition(x, y));
    }
  }
}
