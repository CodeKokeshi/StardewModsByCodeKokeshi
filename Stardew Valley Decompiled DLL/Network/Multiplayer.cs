// Decompiled with JetBrains decompiler
// Type: StardewValley.Multiplayer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Netcode;
using StardewValley.Buildings;
using StardewValley.GameData.LocationContexts;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable
namespace StardewValley;

public class Multiplayer
{
  public static readonly long AllPlayers = 0;
  public const byte farmerDelta = 0;
  public const byte serverIntroduction = 1;
  public const byte playerIntroduction = 2;
  public const byte locationIntroduction = 3;
  public const byte forceEvent = 4;
  public const byte warpFarmer = 5;
  public const byte locationDelta = 6;
  public const byte locationSprites = 7;
  public const byte characterWarp = 8;
  public const byte availableFarmhands = 9;
  public const byte chatMessage = 10;
  public const byte connectionMessage = 11;
  public const byte worldDelta = 12;
  public const byte teamDelta = 13;
  public const byte newDaySync = 14;
  public const byte chatInfoMessage = 15;
  public const byte userNameUpdate = 16 /*0x10*/;
  public const byte farmerGainExperience = 17;
  public const byte serverToClientsMessage = 18;
  public const byte disconnecting = 19;
  public const byte sharedAchievement = 20;
  public const byte globalMessage = 21;
  public const byte partyWideMail = 22;
  public const byte forceKick = 23;
  public const byte removeLocationFromLookup = 24;
  public const byte farmerKilledMonster = 25;
  public const byte requestGrandpaReevaluation = 26;
  public const byte digBuriedNut = 27;
  public const byte requestPassout = 28;
  public const byte passout = 29;
  public const byte startNewDaySync = 30;
  public const byte readySync = 31 /*0x1F*/;
  public const byte chestHitSync = 32 /*0x20*/;
  public const byte dedicatedServerSync = 33;
  /// <summary>A compressed message, which must be decompressed to read the actual message.</summary>
  public const byte compressed = 127 /*0x7F*/;
  public const byte WARP_FLAG_STRUCTURE = 1;
  public const byte WARP_FLAG_FORCED = 2;
  public const byte WARP_FLAG_NEEDS_INFO = 4;
  public const byte WARP_FLAG_FACE_UP = 8;
  public const byte WARP_FLAG_FACE_RIGHT = 16 /*0x10*/;
  public const byte WARP_FLAG_FACE_DOWN = 32 /*0x20*/;
  public const byte WARP_FLAG_FACE_LEFT = 64 /*0x40*/;
  /// <summary>A token prefix for messages sent via <see cref="M:StardewValley.Multiplayer.sendChatInfoMessage(System.String,System.String[])" /> that shows the result of <see cref="M:StardewValley.Utility.AOrAn(System.String)" /> for a tokenizable input.</summary>
  public const 
  #nullable disable
  string chat_token_aOrAn = "aOrAn:";
  public int defaultInterpolationTicks = 15;
  public int farmerDeltaBroadcastPeriod = 3;
  public int locationDeltaBroadcastPeriod = 3;
  public int worldStateDeltaBroadcastPeriod = 3;
  public int playerLimit = 4;
  public static string kicked = "KICKED";
  /// <summary>The override value for <see cref="P:StardewValley.Multiplayer.protocolVersion" />, if set manually in the build settings.</summary>
  internal static string protocolVersionOverride;
  public readonly NetLogger logging = new NetLogger();
  protected List<long> disconnectingFarmers = new List<long>();
  public ulong latestID;
  public Dictionary<string, CachedMultiplayerMap> cachedMultiplayerMaps = new Dictionary<string, CachedMultiplayerMap>();
  protected HashSet<GameLocation> _updatedRoots = new HashSet<GameLocation>();
  public const string MSG_START_FESTIVAL_EVENT = "festivalEvent";
  public const string MSG_END_FESTIVAL = "endFest";
  public const string MSG_TRAIN_APPROACH = "trainApproach";

  /// <summary>A version string sent by the server to new connections. Clients disconnect with an error if it doesn't match their own protocol version, to prevent accidental connection of incompatible games.</summary>
  public static string protocolVersion
  {
    get
    {
      return Multiplayer.protocolVersionOverride != null ? Multiplayer.protocolVersionOverride : Game1.version + (Game1.versionLabel != null ? "+" + new string(Game1.versionLabel.Where<char>(new Func<char, bool>(char.IsLetterOrDigit)).ToArray<char>()) : "");
    }
  }

  public Multiplayer() => this.playerLimit = 8;

  public virtual long getNewID()
  {
    ulong num1 = (ulong) (((long) this.latestID & (long) byte.MaxValue) + 1L) & (ulong) byte.MaxValue;
    ulong uniqueMultiplayerId = (ulong) Game1.player.UniqueMultiplayerID;
    ulong num2 = uniqueMultiplayerId >> 32 /*0x20*/ ^ uniqueMultiplayerId & (ulong) uint.MaxValue;
    this.latestID = (ulong) ((long) ((ulong) DateTime.UtcNow.Ticks / 10000UL) << 24 | (long) ((num2 >> 16 /*0x10*/ ^ num2 & (ulong) ushort.MaxValue) & (ulong) ushort.MaxValue) << 8) | num1;
    return (long) this.latestID;
  }

  public virtual int MaxPlayers => Game1.server == null ? 1 : this.playerLimit;

  public virtual bool isDisconnecting(Farmer farmer)
  {
    return this.isDisconnecting(farmer.UniqueMultiplayerID);
  }

  public virtual bool isDisconnecting(long uid) => this.disconnectingFarmers.Contains(uid);

  public virtual bool isClientBroadcastType(byte messageType)
  {
    switch (messageType)
    {
      case 0:
      case 2:
      case 4:
      case 6:
      case 7:
      case 12:
      case 13:
      case 14:
      case 15:
      case 19:
      case 20:
      case 21:
      case 22:
      case 24:
      case 26:
        return true;
      default:
        return false;
    }
  }

  public virtual bool allowSyncDelay() => !Game1.newDaySync.hasInstance();

  public virtual int interpolationTicks()
  {
    if (!this.allowSyncDelay())
      return 0;
    return LocalMultiplayer.IsLocalMultiplayer(true) ? 4 : this.defaultInterpolationTicks;
  }

  public virtual IEnumerable<NetFarmerRoot> farmerRoots()
  {
    if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null)
      yield return Game1.serverHost;
    foreach (NetRoot<Farmer> netRoot in Game1.otherFarmers.Roots.Values)
    {
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || (NetFieldBase<Farmer, NetRef<Farmer>>) netRoot != (NetRef<Farmer>) Game1.serverHost)
        yield return netRoot as NetFarmerRoot;
    }
  }

  public virtual NetFarmerRoot farmerRoot(long id)
  {
    if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && id == Game1.serverHost.Value.UniqueMultiplayerID)
      return Game1.serverHost;
    NetRoot<Farmer> netRoot;
    return Game1.otherFarmers.Roots.TryGetValue(id, out netRoot) ? netRoot as NetFarmerRoot : (NetFarmerRoot) null;
  }

  public virtual void broadcastFarmerDeltas()
  {
    foreach (NetFarmerRoot farmerRoot in this.farmerRoots())
    {
      if (farmerRoot.Dirty && Game1.player.UniqueMultiplayerID == farmerRoot.Value.UniqueMultiplayerID)
        this.broadcastFarmerDelta(farmerRoot.Value, this.writeObjectDeltaBytes<Farmer>((NetRoot<Farmer>) farmerRoot));
    }
    if (!Game1.player.teamRoot.Dirty)
      return;
    this.broadcastTeamDelta(this.writeObjectDeltaBytes<FarmerTeam>(Game1.player.teamRoot));
  }

  protected virtual void broadcastTeamDelta(byte[] delta)
  {
    if (Game1.IsServer)
    {
      foreach (Farmer farmer in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
      {
        if (farmer != Game1.player)
          Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 13, Game1.player, (object) delta);
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 13, (object) delta);
    }
  }

  protected virtual void broadcastFarmerDelta(Farmer farmer, byte[] delta)
  {
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
    {
      if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
        otherFarmer.Value.queueMessage((byte) 0, farmer, (object) farmer.UniqueMultiplayerID, (object) delta);
    }
  }

  public void updateRoot<T>(T root) where T : INetRoot
  {
    foreach (long disconnectingFarmer in this.disconnectingFarmers)
      root.Disconnect(disconnectingFarmer);
    root.TickTree();
  }

  public virtual void updateRoots()
  {
    this.updateRoot<NetRoot<NetWorldState>>(Game1.netWorldState);
    foreach (NetFarmerRoot farmerRoot in this.farmerRoots())
    {
      farmerRoot.Clock.InterpolationTicks = this.interpolationTicks();
      this.updateRoot<NetFarmerRoot>(farmerRoot);
    }
    Game1.player.teamRoot.Clock.InterpolationTicks = this.interpolationTicks();
    this.updateRoot<NetRoot<FarmerTeam>>(Game1.player.teamRoot);
    if (Game1.IsClient)
    {
      foreach (GameLocation activeLocation in this.activeLocations())
      {
        if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeLocation.Root != (NetRef<GameLocation>) null && this._updatedRoots.Add(activeLocation.Root.Value))
        {
          activeLocation.Root.Clock.InterpolationTicks = this.interpolationTicks();
          this.updateRoot<NetRoot<GameLocation>>(activeLocation.Root);
        }
      }
    }
    else
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null)
        {
          location.Root.Clock.InterpolationTicks = this.interpolationTicks();
          this.updateRoot<NetRoot<GameLocation>>(location.Root);
        }
        return true;
      }), false, true);
    this._updatedRoots.Clear();
  }

  public virtual void broadcastLocationDeltas()
  {
    if (Game1.IsClient)
    {
      foreach (GameLocation activeLocation in this.activeLocations())
      {
        if (!((NetFieldBase<GameLocation, NetRef<GameLocation>>) activeLocation.Root == (NetRef<GameLocation>) null) && activeLocation.Root.Dirty)
          this.broadcastLocationDelta(activeLocation);
      }
    }
    else
      Utility.ForEachLocation((Func<GameLocation, bool>) (location =>
      {
        if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null && location.Root.Dirty)
          this.broadcastLocationDelta(location);
        return true;
      }), false, true);
  }

  public virtual void broadcastLocationDelta(GameLocation loc)
  {
    if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) loc.Root == (NetRef<GameLocation>) null || !loc.Root.Dirty)
      return;
    byte[] bytes = this.writeObjectDeltaBytes<GameLocation>(loc.Root);
    this.broadcastLocationBytes(loc, (byte) 6, bytes);
  }

  protected virtual void broadcastLocationBytes(GameLocation loc, byte messageType, byte[] bytes)
  {
    OutgoingMessage message = new OutgoingMessage(messageType, Game1.player, new object[3]
    {
      (object) loc.isStructure.Value,
      (object) loc.NameOrUniqueName,
      (object) bytes
    });
    this.broadcastLocationMessage(loc, message);
  }

  protected virtual void broadcastLocationMessage(GameLocation loc, OutgoingMessage message)
  {
    if (Game1.IsClient)
      Game1.client.sendMessage(message);
    else if (this.isAlwaysActiveLocation(loc))
    {
      foreach (Farmer f in (IEnumerable<Farmer>) Game1.otherFarmers.Values)
        TellFarmer(f);
    }
    else
    {
      foreach (Farmer farmer in loc.farmers)
        TellFarmer(farmer);
      foreach (Building building in loc.buildings)
      {
        GameLocation indoors = building.GetIndoors();
        if (indoors != null)
        {
          foreach (Farmer farmer in indoors.farmers)
            TellFarmer(farmer);
        }
      }
    }

    void TellFarmer(Farmer f)
    {
      if (f == Game1.player)
        return;
      Game1.server.sendMessage(f.UniqueMultiplayerID, message);
    }
  }

  public virtual void broadcastSprites(GameLocation location, TemporaryAnimatedSpriteList sprites)
  {
    this.broadcastSprites(location, sprites.ToArray<TemporaryAnimatedSprite>());
  }

  public virtual void broadcastSprites(
    GameLocation location,
    params TemporaryAnimatedSprite[] sprites)
  {
    location.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) sprites);
    if (sprites.Length == 0 || !Game1.IsMultiplayer)
      return;
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
      {
        writer.Push("TemporaryAnimatedSprites");
        writer.Write(sprites.Length);
        foreach (TemporaryAnimatedSprite sprite in sprites)
          sprite.Write(writer, location);
        writer.Pop();
      }
      this.broadcastLocationBytes(location, (byte) 7, memoryStream.ToArray());
    }
  }

  public virtual void broadcastWorldStateDeltas()
  {
    if (!Game1.netWorldState.Dirty)
      return;
    byte[] numArray = this.writeObjectDeltaBytes<NetWorldState>(Game1.netWorldState);
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
    {
      if (otherFarmer.Value != Game1.player)
        otherFarmer.Value.queueMessage((byte) 12, Game1.player, (object) numArray);
    }
  }

  public virtual void receiveWorldState(BinaryReader msg)
  {
    Game1.netWorldState.Clock.InterpolationTicks = 0;
    this.readObjectDelta<NetWorldState>(msg, Game1.netWorldState);
    Game1.netWorldState.TickTree();
    int timeOfDay = Game1.timeOfDay;
    Game1.netWorldState.Value.WriteToGame1();
    if (Game1.IsServer || timeOfDay == Game1.timeOfDay || Game1.currentLocation == null || Game1.newDaySync.hasInstance())
      return;
    Game1.performTenMinuteClockUpdate();
  }

  public virtual void requestCharacterWarp(
    NPC character,
    GameLocation targetLocation,
    Vector2 position)
  {
    if (!Game1.IsClient)
      return;
    GameLocation currentLocation = character.currentLocation;
    if (currentLocation == null)
      throw new ArgumentException("In warpCharacter, the character's currentLocation must not be null");
    Guid guid = currentLocation.characters.GuidOf(character);
    if (guid == Guid.Empty)
      throw new ArgumentException("In warpCharacter, the character must be in its currentLocation");
    OutgoingMessage message = new OutgoingMessage((byte) 8, Game1.player, new object[6]
    {
      (object) currentLocation.isStructure.Value,
      (object) currentLocation.NameOrUniqueName,
      (object) guid,
      (object) targetLocation.isStructure.Value,
      (object) targetLocation.NameOrUniqueName,
      (object) position
    });
    Game1.serverHost.Value.queueMessage(message);
  }

  public virtual NetRoot<GameLocation> locationRoot(GameLocation location)
  {
    if ((NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root == (NetRef<GameLocation>) null && Game1.IsMasterGame)
    {
      new NetRoot<GameLocation>().Set(location);
      location.Root.Clock.InterpolationTicks = this.interpolationTicks();
      location.Root.MarkClean();
    }
    return location.Root;
  }

  public virtual void sendPassoutRequest()
  {
    object[] objArray = new object[1]
    {
      (object) Game1.player.UniqueMultiplayerID
    };
    if (Game1.IsMasterGame)
      this._receivePassoutRequest(Game1.player);
    else
      Game1.client.sendMessage((byte) 28, objArray);
  }

  public virtual void receivePassoutRequest(IncomingMessage msg)
  {
    if (!Game1.IsServer)
      return;
    Farmer player = Game1.GetPlayer(msg.Reader.ReadInt64());
    if (player == null)
      return;
    this._receivePassoutRequest(player);
  }

  protected virtual void _receivePassoutRequest(Farmer farmer)
  {
    if (!Game1.IsMasterGame)
      return;
    GameLocation locationFromName1 = farmer.lastSleepLocation.Value == null || !Game1.isLocationAccessible(farmer.lastSleepLocation.Value) ? (GameLocation) null : Game1.getLocationFromName(farmer.lastSleepLocation.Value);
    bool? nullable = locationFromName1?.CanWakeUpHere(farmer);
    if (nullable.HasValue && nullable.GetValueOrDefault() && locationFromName1.GetLocationContextId() == farmer.currentLocation.GetLocationContextId())
    {
      if (Game1.IsServer && farmer != Game1.player)
      {
        object[] source = new object[4]
        {
          (object) farmer.lastSleepLocation.Value,
          (object) farmer.lastSleepPoint.X,
          (object) farmer.lastSleepPoint.Y,
          (object) true
        };
        Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 29, Game1.player, ((IEnumerable<object>) source).ToArray<object>());
      }
      else
        Farmer.performPassoutWarp(farmer, farmer.lastSleepLocation.Value, farmer.lastSleepPoint.Value, true);
    }
    else
    {
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(farmer);
      string bed_location_name = homeOfFarmer.NameOrUniqueName;
      Point bed_point = homeOfFarmer.GetPlayerBedSpot();
      bool has_bed = homeOfFarmer.GetPlayerBed() != null;
      List<ReviveLocation> reviveLocationList = farmer.currentLocation?.GetLocationContext().PassOutLocations ?? StardewValley.LocationContexts.Default.PassOutLocations;
      if (reviveLocationList != null)
      {
        foreach (ReviveLocation reviveLocation in reviveLocationList)
        {
          if (GameStateQuery.CheckConditions(reviveLocation.Condition, farmer.currentLocation, farmer))
          {
            GameLocation locationFromName2 = Game1.getLocationFromName(reviveLocation.Location);
            if (locationFromName2 != null)
            {
              bed_location_name = reviveLocation.Location;
              bed_point = reviveLocation.Position;
              has_bed = false;
              using (List<Furniture>.Enumerator enumerator = locationFromName2.furniture.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  if (enumerator.Current is BedFurniture current && current.bedType != BedFurniture.BedType.Child)
                  {
                    bed_point = current.GetBedSpot();
                    has_bed = true;
                    break;
                  }
                }
                break;
              }
            }
            break;
          }
        }
      }
      if (Game1.IsServer && farmer != Game1.player)
      {
        object[] source = new object[4]
        {
          (object) bed_location_name,
          (object) bed_point.X,
          (object) bed_point.Y,
          (object) has_bed
        };
        Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 29, Game1.player, ((IEnumerable<object>) source).ToArray<object>());
      }
      else
        Farmer.performPassoutWarp(farmer, bed_location_name, bed_point, has_bed);
    }
  }

  public virtual void receivePassout(IncomingMessage msg)
  {
    if (msg.SourceFarmer != Game1.serverHost.Value)
      return;
    Farmer.performPassoutWarp(Game1.player, msg.Reader.ReadString(), new Point(msg.Reader.ReadInt32(), msg.Reader.ReadInt32()), msg.Reader.ReadBoolean());
  }

  public virtual object[] generateForceEventMessage(
    string eventId,
    GameLocation location,
    int tileX,
    int tileY,
    bool use_local_farmer,
    bool notify_when_done)
  {
    return new object[7]
    {
      (object) eventId,
      (object) use_local_farmer,
      (object) notify_when_done,
      (object) tileX,
      (object) tileY,
      (object) (byte) (location.isStructure.Value ? 1 : 0),
      (object) location.NameOrUniqueName
    };
  }

  public virtual void broadcastEvent(
    Event evt,
    GameLocation location,
    Vector2 positionBeforeEvent,
    bool use_local_farmer = true,
    bool notify_when_done = false)
  {
    if (string.IsNullOrEmpty(evt.id) || evt.id == "-1")
      return;
    object[] forceEventMessage = this.generateForceEventMessage(evt.id, location, (int) positionBeforeEvent.X, (int) positionBeforeEvent.Y, use_local_farmer, notify_when_done);
    if (Game1.IsServer)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 4, Game1.dedicatedServer.FakeFarmer, forceEventMessage);
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 4, forceEventMessage);
    }
  }

  protected virtual void receiveRequestGrandpaReevaluation(IncomingMessage msg)
  {
    Game1.getFarm()?.requestGrandpaReevaluation();
  }

  protected virtual void receiveFarmerKilledMonster(IncomingMessage msg)
  {
    if (msg.SourceFarmer != Game1.serverHost.Value)
      return;
    string name = msg.Reader.ReadString();
    if (name == null)
      return;
    Game1.stats.monsterKilled(name);
  }

  public virtual void broadcastRemoveLocationFromLookup(GameLocation location)
  {
    List<object> objectList = new List<object>();
    objectList.Add((object) location.NameOrUniqueName);
    if (Game1.IsServer)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 24, Game1.player, objectList.ToArray());
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 24, objectList.ToArray());
    }
  }

  public virtual void broadcastNutDig(GameLocation location, Point point)
  {
    if (Game1.IsMasterGame)
      this._performNutDig(location, point);
    else
      Game1.client.sendMessage((byte) 27, new List<object>()
      {
        (object) location.NameOrUniqueName,
        (object) point.X,
        (object) point.Y
      }.ToArray());
  }

  protected virtual void receiveNutDig(IncomingMessage msg)
  {
    if (!Game1.IsMasterGame)
      return;
    string name = msg.Reader.ReadString();
    Point point = new Point(msg.Reader.ReadInt32(), msg.Reader.ReadInt32());
    this._performNutDig(Game1.getLocationFromName(name), point);
  }

  protected virtual void _performNutDig(GameLocation location, Point point)
  {
    if (!(location is IslandLocation location1) || !location1.IsBuriedNutLocation(point))
      return;
    string str = $"{location.NameOrUniqueName}_{point.X.ToString()}_{point.Y.ToString()}";
    if (!Game1.netWorldState.Value.FoundBuriedNuts.Add(str))
      return;
    Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2((float) point.X, (float) point.Y) * 64f, -1, (GameLocation) location1);
  }

  public virtual void broadcastPartyWideMail(
    string mail_key,
    Multiplayer.PartyWideMessageQueue message_queue = Multiplayer.PartyWideMessageQueue.MailForTomorrow,
    bool no_letter = false)
  {
    mail_key = mail_key.Trim();
    mail_key = mail_key.Replace(Environment.NewLine, "");
    List<object> objectList = new List<object>();
    objectList.Add((object) mail_key);
    objectList.Add((object) (int) message_queue);
    objectList.Add((object) no_letter);
    this._performPartyWideMail(mail_key, message_queue, no_letter);
    if (Game1.IsServer)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 22, Game1.player, objectList.ToArray());
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 22, objectList.ToArray());
    }
  }

  public virtual void broadcastGrandpaReevaluation()
  {
    Game1.getFarm().requestGrandpaReevaluation();
    if (Game1.IsServer)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 26, Game1.player);
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 26);
    }
  }

  /// <summary>Broadcast a global popup message.</summary>
  /// <param name="translationKey">The translation key for the message text.</param>
  /// <param name="onlyShowIfEmpty">Whether to show the message only when no other messages are showing.</param>
  /// <param name="location">The location where players will see the message, or <see langword="null" /> to show it everywhere.</param>
  /// <param name="substitutions">The token substitutions for placeholders in the translation text, if any.</param>
  public virtual void broadcastGlobalMessage(
    string translationKey,
    bool onlyShowIfEmpty = false,
    GameLocation location = null,
    params string[] substitutions)
  {
    if ((!onlyShowIfEmpty || Game1.hudMessages.Count == 0) && (location == null || location.NameOrUniqueName == Game1.player.currentLocation?.NameOrUniqueName))
    {
      string[] strArray = new string[substitutions.Length];
      for (int index = 0; index < substitutions.Length; ++index)
        strArray[index] = TokenParser.ParseText(substitutions[index]);
      Game1.showGlobalMessage(Game1.content.LoadString(translationKey, (object[]) strArray));
    }
    List<object> objectList = new List<object>()
    {
      (object) translationKey,
      (object) onlyShowIfEmpty,
      (object) (location?.NameOrUniqueName ?? ""),
      (object) substitutions.Length
    };
    objectList.AddRange((IEnumerable<object>) substitutions);
    if (Game1.IsServer)
    {
      foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      {
        if (otherFarmer.Value.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
          Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 21, Game1.player, objectList.ToArray());
      }
    }
    else
    {
      if (!Game1.IsClient)
        return;
      Game1.client.sendMessage((byte) 21, objectList.ToArray());
    }
  }

  public virtual NetRoot<T> readObjectFull<T>(BinaryReader reader) where T : class, INetObject<INetSerializable>
  {
    NetRoot<T> netRoot = NetRoot<T>.Connect(reader);
    netRoot.Clock.InterpolationTicks = this.defaultInterpolationTicks;
    return netRoot;
  }

  protected virtual BinaryWriter createWriter(Stream stream)
  {
    BinaryWriter writer = new BinaryWriter(stream);
    if (this.logging.IsLogging)
      writer = (BinaryWriter) new LoggingBinaryWriter(writer);
    return writer;
  }

  public virtual void writeObjectFull<T>(BinaryWriter writer, NetRoot<T> root, long? peer) where T : class, INetObject<INetSerializable>
  {
    root.CreateConnectionPacket(writer, peer);
  }

  public virtual byte[] writeObjectFullBytes<T>(NetRoot<T> root, long? peer) where T : class, INetObject<INetSerializable>
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
      {
        root.CreateConnectionPacket(writer, peer);
        return memoryStream.ToArray();
      }
    }
  }

  public virtual void readObjectDelta<T>(BinaryReader reader, NetRoot<T> root) where T : class, INetObject<INetSerializable>
  {
    root.Read(reader);
  }

  public virtual void writeObjectDelta<T>(BinaryWriter writer, NetRoot<T> root) where T : class, INetObject<INetSerializable>
  {
    root.Write(writer);
  }

  public virtual byte[] writeObjectDeltaBytes<T>(NetRoot<T> root) where T : class, INetObject<INetSerializable>
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      using (BinaryWriter writer = this.createWriter((Stream) memoryStream))
      {
        root.Write(writer);
        return memoryStream.ToArray();
      }
    }
  }

  public virtual NetFarmerRoot readFarmer(BinaryReader reader)
  {
    NetFarmerRoot netFarmerRoot = new NetFarmerRoot();
    netFarmerRoot.ReadConnectionPacket(reader);
    netFarmerRoot.Clock.InterpolationTicks = this.defaultInterpolationTicks;
    return netFarmerRoot;
  }

  public virtual void addPlayer(NetFarmerRoot f)
  {
    long uniqueMultiplayerId = f.Value.UniqueMultiplayerID;
    f.Value.teamRoot = Game1.player.teamRoot;
    Game1.otherFarmers.Roots[uniqueMultiplayerId] = (NetRoot<Farmer>) f;
    this.disconnectingFarmers.Remove(uniqueMultiplayerId);
    if (Game1.chatBox == null)
      return;
    string sub1 = ChatBox.formattedUserNameLong(f.Value);
    Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_PlayerJoined", (object) sub1));
  }

  public virtual void receivePlayerIntroduction(BinaryReader reader)
  {
    this.addPlayer(this.readFarmer(reader));
  }

  public virtual void broadcastPlayerIntroduction(NetFarmerRoot farmerRoot)
  {
    if (Game1.server == null)
      return;
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
    {
      if (farmerRoot.Value.UniqueMultiplayerID != otherFarmer.Value.UniqueMultiplayerID)
        Game1.server.sendMessage(otherFarmer.Value.UniqueMultiplayerID, (byte) 2, farmerRoot.Value, (object) Game1.server.getUserName(farmerRoot.Value.UniqueMultiplayerID), (object) this.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) farmerRoot, new long?(otherFarmer.Value.UniqueMultiplayerID)));
    }
  }

  public virtual void broadcastUserName(long farmerId, string userName)
  {
    if (Game1.server != null)
      return;
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
    {
      Farmer farmer = otherFarmer.Value;
      if (farmer.UniqueMultiplayerID != farmerId)
        Game1.server.sendMessage(farmer.UniqueMultiplayerID, (byte) 16 /*0x10*/, Game1.serverHost.Value, (object) farmerId, (object) userName);
    }
  }

  public virtual string getUserName(long id)
  {
    if (id == Game1.player.UniqueMultiplayerID)
      return Game1.content.LoadString("Strings\\UI:Chat_SelfPlayerID");
    if (Game1.server != null)
      return Game1.server.getUserName(id);
    return Game1.client != null ? Game1.client.getUserName(id) : "?";
  }

  public virtual void playerDisconnected(long id)
  {
    NetRoot<Farmer> netRoot;
    if (!Game1.otherFarmers.Roots.TryGetValue(id, out netRoot) || this.disconnectingFarmers.Contains(id))
      return;
    NetFarmerRoot farmhand = netRoot as NetFarmerRoot;
    if (farmhand.Value.mount != null && Game1.IsMasterGame)
      farmhand.Value.mount.dismount();
    if (Game1.IsMasterGame)
    {
      farmhand.TargetValue.handleDisconnect();
      farmhand.TargetValue.companions.Clear();
      this.saveFarmhand(farmhand);
      farmhand.Value.handleDisconnect();
    }
    if (Game1.player.dancePartner.Value is Farmer && ((Farmer) Game1.player.dancePartner.Value).UniqueMultiplayerID == farmhand.Value.UniqueMultiplayerID)
      Game1.player.dancePartner.Value = (Character) null;
    if (Game1.chatBox != null)
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_PlayerLeft", (object) ChatBox.formattedUserNameLong(Game1.otherFarmers[id])));
    this.disconnectingFarmers.Add(id);
  }

  protected virtual void removeDisconnectedFarmers()
  {
    foreach (long disconnectingFarmer in this.disconnectingFarmers)
      Game1.otherFarmers.Remove(disconnectingFarmer);
    this.disconnectingFarmers.Clear();
  }

  public virtual void sendFarmhand()
  {
    (Game1.player.NetFields.Root as NetFarmerRoot).MarkReassigned();
  }

  protected virtual void saveFarmhand(NetFarmerRoot farmhand)
  {
    Game1.netWorldState.Value.SaveFarmhand(farmhand);
  }

  public virtual void saveFarmhands()
  {
    if (!Game1.IsMasterGame)
      return;
    foreach (NetRoot<Farmer> farmhand in Game1.otherFarmers.Roots.Values)
      this.saveFarmhand(farmhand as NetFarmerRoot);
  }

  public virtual void clientRemotelyDisconnected(Multiplayer.DisconnectType disconnectType)
  {
    Multiplayer.LogDisconnect(disconnectType);
    this.returnToMainMenu();
  }

  private void returnToMainMenu()
  {
    if (!Game1.game1.IsMainInstance)
      GameRunner.instance.RemoveGameInstance(Game1.game1);
    else
      Game1.ExitToTitle((Action) (() =>
      {
        (Game1.activeClickableMenu as TitleMenu).skipToTitleButtons();
        TitleMenu.subMenu = (IClickableMenu) new ConfirmationDialog(Game1.content.LoadString("Strings\\UI:Client_RemotelyDisconnected"), (ConfirmationDialog.behavior) null)
        {
          okButton = {
            visible = false
          }
        };
      }));
  }

  public static bool ShouldLogDisconnect(Multiplayer.DisconnectType disconnectType)
  {
    switch (disconnectType)
    {
      case Multiplayer.DisconnectType.ClosedGame:
      case Multiplayer.DisconnectType.ExitedToMainMenu:
      case Multiplayer.DisconnectType.ExitedToMainMenu_FromFarmhandSelect:
      case Multiplayer.DisconnectType.ServerOfflineMode:
      case Multiplayer.DisconnectType.ServerFull:
      case Multiplayer.DisconnectType.AcceptedOtherInvite:
        return false;
      default:
        return true;
    }
  }

  public static bool IsTimeout(Multiplayer.DisconnectType disconnectType)
  {
    switch (disconnectType)
    {
      case Multiplayer.DisconnectType.ClientTimeout:
      case Multiplayer.DisconnectType.LidgrenTimeout:
      case Multiplayer.DisconnectType.GalaxyTimeout:
        return true;
      default:
        return false;
    }
  }

  public static void LogDisconnect(Multiplayer.DisconnectType disconnectType)
  {
    if (Multiplayer.ShouldLogDisconnect(disconnectType))
    {
      string message = $"Disconnected at : {DateTime.Now.ToLongTimeString()} - {disconnectType.ToString()}";
      if (Game1.client != null)
        message = $"{message} Ping: {Game1.client.GetPingToHost().ToString("0.#")}" + (Game1.client is LidgrenClient ? " ip" : " friend/invite");
      Program.WriteLog(Program.LogType.Disconnect, message, true);
    }
    Game1.log.Verbose("Disconnected: " + disconnectType.ToString());
  }

  public virtual void sendSharedAchievementMessage(int achievement)
  {
    if (Game1.IsClient)
    {
      Game1.client.sendMessage((byte) 20, (object) achievement);
    }
    else
    {
      if (!Game1.IsServer)
        return;
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
        Game1.server.sendMessage(key, (byte) 20, Game1.player, (object) achievement);
    }
  }

  public virtual void sendServerToClientsMessage(string message)
  {
    if (!Game1.IsServer)
      return;
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      otherFarmer.Value.queueMessage((byte) 18, Game1.player, (object) message);
  }

  public virtual void sendChatMessage(
    LocalizedContentManager.LanguageCode language,
    string message,
    long recipientID)
  {
    if (Game1.IsClient)
    {
      Game1.client.sendMessage((byte) 10, (object) recipientID, (object) language, (object) message);
    }
    else
    {
      if (!Game1.IsServer)
        return;
      if (recipientID == Multiplayer.AllPlayers)
      {
        foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
          Game1.server.sendMessage(key, (byte) 10, Game1.player, (object) recipientID, (object) language, (object) message);
      }
      else
        Game1.server.sendMessage(recipientID, (byte) 10, Game1.player, (object) recipientID, (object) language, (object) message);
    }
  }

  public virtual void receiveChatMessage(
    Farmer sourceFarmer,
    long recipientID,
    LocalizedContentManager.LanguageCode language,
    string message)
  {
    if (Game1.chatBox == null)
      return;
    int chatKind = 0;
    message = Program.sdk.FilterDirtyWords(message);
    if (recipientID != Multiplayer.AllPlayers)
      chatKind = 3;
    Game1.chatBox.receiveChatMessage(sourceFarmer.UniqueMultiplayerID, chatKind, language, message);
  }

  /// <summary>In multiplayer, send a chat messages to all connected players including the current player. In single-player, do nothing.</summary>
  /// <inheritdoc cref="M:StardewValley.Multiplayer.receiveChatInfoMessage(StardewValley.Farmer,System.String,System.String[])" />
  public virtual void globalChatInfoMessage(string messageKey, params string[] args)
  {
    if (!Game1.IsMultiplayer && Game1.multiplayerMode == (byte) 0)
      return;
    this.receiveChatInfoMessage(Game1.player, messageKey, args);
    this.sendChatInfoMessage(messageKey, args);
  }

  /// <summary>Send a chat messages to all connected players including the current player.</summary>
  /// <inheritdoc cref="M:StardewValley.Multiplayer.receiveChatInfoMessage(StardewValley.Farmer,System.String,System.String[])" />
  public void globalChatInfoMessageEvenInSinglePlayer(string messageKey, params string[] args)
  {
    this.receiveChatInfoMessage(Game1.player, messageKey, args);
    this.sendChatInfoMessage(messageKey, args);
  }

  /// <summary>Send a chat messages to all connected players, excluding the current player.</summary>
  /// <inheritdoc cref="M:StardewValley.Multiplayer.receiveChatInfoMessage(StardewValley.Farmer,System.String,System.String[])" />
  protected virtual void sendChatInfoMessage(string messageKey, params string[] args)
  {
    if (Game1.IsClient)
    {
      Game1.client.sendMessage((byte) 15, (object) messageKey, (object) args);
    }
    else
    {
      if (!Game1.IsServer)
        return;
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
        Game1.server.sendMessage(key, (byte) 15, Game1.player, (object) messageKey, (object) args);
    }
  }

  /// <summary>Receive a chat message sent via a method like <see cref="M:StardewValley.Multiplayer.globalChatInfoMessage(System.String,System.String[])" /> or <see cref="M:StardewValley.Multiplayer.sendChatInfoMessage(System.String,System.String[])" />.</summary>
  /// <param name="sourceFarmer">The player who sent the message.</param>
  /// <param name="messageKey">The translation key to show. This is prefixed with <c>Strings\UI:Chat_</c> automatically.</param>
  /// <param name="args">The values with which to replace placeholders in the translation text. Localizable values should be <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenized strings</see> or special tokens like <see cref="F:StardewValley.Multiplayer.chat_token_aOrAn" />, since other players may not be playing in the same language.</param>
  protected virtual void receiveChatInfoMessage(
    Farmer sourceFarmer,
    string messageKey,
    string[] args)
  {
    if (Game1.chatBox == null)
      return;
    try
    {
      string[] array = ((IEnumerable<string>) args).Select<string, string>((Func<string, string>) (arg => arg.StartsWith("aOrAn:") ? Utility.AOrAn(TokenParser.ParseText(arg.Substring("aOrAn:".Length))) : TokenParser.ParseText(arg))).ToArray<string>();
      Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, (object[]) array));
    }
    catch (ContentLoadException ex)
    {
    }
    catch (FormatException ex)
    {
    }
    catch (OverflowException ex)
    {
    }
    catch (KeyNotFoundException ex)
    {
    }
  }

  public virtual void parseServerToClientsMessage(string message)
  {
    if (!Game1.IsClient)
      return;
    switch (message)
    {
      case "festivalEvent":
        if (Game1.currentLocation.currentEvent == null)
          break;
        Game1.currentLocation.currentEvent.forceFestivalContinue();
        break;
      case "endFest":
        if (Game1.CurrentEvent == null)
          break;
        Game1.CurrentEvent.forceEndFestival(Game1.player);
        break;
      case "trainApproach":
        if (!(Game1.getLocationFromName("Railroad") is Railroad locationFromName))
          break;
        locationFromName.PlayTrainApproach();
        break;
    }
  }

  public virtual IEnumerable<GameLocation> activeLocations()
  {
    if (Game1.currentLocation != null)
      yield return Game1.currentLocation;
    foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
    {
      if (this.isAlwaysActiveLocation(location))
      {
        foreach (GameLocation gameLocation in this._GetActiveLocationsHere(location))
          yield return gameLocation;
      }
    }
  }

  protected virtual IEnumerable<GameLocation> _GetActiveLocationsHere(GameLocation location)
  {
    if (location != Game1.currentLocation)
      yield return location;
    foreach (Building building in location.buildings)
    {
      GameLocation indoors = building.GetIndoors();
      if (indoors != null && (!indoors.isAlwaysActive.Value || building.GetIndoorsType() != IndoorsType.Global))
      {
        foreach (GameLocation gameLocation in this._GetActiveLocationsHere(indoors))
          yield return gameLocation;
      }
    }
  }

  public virtual bool isAlwaysActiveLocation(GameLocation location)
  {
    return (NetFieldBase<GameLocation, NetRef<GameLocation>>) location.Root != (NetRef<GameLocation>) null && location.Root.Value != location && this.isAlwaysActiveLocation(location.Root.Value) || location.isAlwaysActive.Value;
  }

  protected virtual void readActiveLocation(IncomingMessage msg)
  {
    bool flag = msg.Reader.ReadBoolean();
    NetRoot<GameLocation> netRoot = this.readObjectFull<GameLocation>(msg.Reader);
    if (this.isAlwaysActiveLocation(netRoot.Value))
    {
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        GameLocation location = Game1.locations[index];
        if (location.Equals(netRoot.Value))
        {
          if (location != netRoot.Value)
          {
            if (location != null)
            {
              if (Game1.currentLocation == location)
                Game1.currentLocation = netRoot.Value;
              if (Game1.player.currentLocation == location)
                Game1.player.currentLocation = netRoot.Value;
              Game1.removeLocationFromLocationLookup(location);
              location.OnRemoved();
            }
            Game1.locations[index] = netRoot.Value;
            break;
          }
          break;
        }
      }
    }
    if (!(Game1.locationRequest != null | flag))
      return;
    if (Game1.locationRequest != null)
      Game1.currentLocation = Game1.findStructure(netRoot.Value, Game1.locationRequest.Name) ?? netRoot.Value;
    else if (flag)
      Game1.currentLocation = netRoot.Value;
    if (Game1.locationRequest != null)
    {
      Game1.locationRequest.Location = Game1.currentLocation;
      Game1.locationRequest.Loaded(Game1.currentLocation);
    }
    if (Game1.client != null || !(Game1.activeClickableMenu is TitleMenu) || (TitleMenu.subMenu is FarmhandMenu subMenu ? subMenu.client : (Client) null) == null)
      Game1.currentLocation.resetForPlayerEntry();
    Game1.player.currentLocation = Game1.currentLocation;
    Game1.locationRequest?.Warped(Game1.currentLocation);
    Game1.currentLocation.updateSeasonalTileSheets();
    Game1.locationRequest = (LocationRequest) null;
  }

  public virtual bool isActiveLocation(GameLocation location)
  {
    if (Game1.IsMasterGame)
      return true;
    if (location?.Root == null)
      return false;
    return Game1.currentLocation != null && (NetFieldBase<GameLocation, NetRef<GameLocation>>) Game1.currentLocation.Root != (NetRef<GameLocation>) null && Game1.currentLocation.Root.Value == location.Root.Value || this.isAlwaysActiveLocation(location);
  }

  protected virtual GameLocation readLocation(BinaryReader reader)
  {
    bool isStructure = reader.ReadByte() > (byte) 0;
    GameLocation locationFromName = Game1.getLocationFromName(reader.ReadString(), isStructure);
    if (locationFromName == null || (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.locationRoot(locationFromName) == (NetRef<GameLocation>) null)
      return (GameLocation) null;
    return !this.isActiveLocation(locationFromName) ? (GameLocation) null : locationFromName;
  }

  protected virtual LocationRequest readLocationRequest(BinaryReader reader)
  {
    bool isStructure = reader.ReadByte() > (byte) 0;
    return Game1.getLocationRequest(reader.ReadString(), isStructure);
  }

  protected virtual NPC readNPC(BinaryReader reader)
  {
    NPC npc;
    return !this.readLocation(reader).characters.TryGetValue(reader.ReadGuid(), out npc) ? (NPC) null : npc;
  }

  public virtual void readSprites(
    BinaryReader reader,
    GameLocation location,
    Action<TemporaryAnimatedSprite> assignSprite)
  {
    int length = reader.ReadInt32();
    TemporaryAnimatedSprite[] temporaryAnimatedSpriteArray = new TemporaryAnimatedSprite[length];
    for (int index = 0; index < length; ++index)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = TemporaryAnimatedSprite.GetTemporaryAnimatedSprite();
      temporaryAnimatedSprite.Read(reader, location);
      temporaryAnimatedSprite.ticksBeforeAnimationStart += this.interpolationTicks();
      temporaryAnimatedSpriteArray[index] = temporaryAnimatedSprite;
      assignSprite(temporaryAnimatedSprite);
    }
  }

  protected virtual void receiveTeamDelta(BinaryReader msg)
  {
    this.readObjectDelta<FarmerTeam>(msg, Game1.player.teamRoot);
  }

  protected virtual void receiveNewDaySync(IncomingMessage msg)
  {
    if (!Game1.newDaySync.hasInstance() && msg.SourceFarmer == Game1.serverHost.Value)
      Game1.NewDay(0.0f);
    if (!Game1.newDaySync.hasInstance())
      return;
    Game1.newDaySync.receiveMessage(msg);
  }

  protected virtual void receiveFarmerGainExperience(IncomingMessage msg)
  {
    if (msg.SourceFarmer != Game1.serverHost.Value)
      return;
    Game1.player.gainExperience(msg.Reader.ReadInt32(), msg.Reader.ReadInt32());
  }

  protected virtual void receiveSharedAchievement(IncomingMessage msg)
  {
    Game1.getAchievement(msg.Reader.ReadInt32(), false);
  }

  protected virtual void receiveRemoveLocationFromLookup(IncomingMessage msg)
  {
    Game1.removeLocationFromLocationLookup(msg.Reader.ReadString());
  }

  protected virtual void receivePartyWideMail(IncomingMessage msg)
  {
    this._performPartyWideMail(msg.Reader.ReadString(), (Multiplayer.PartyWideMessageQueue) msg.Reader.ReadInt32(), msg.Reader.ReadBoolean());
  }

  protected void _performPartyWideMail(
    string mail_key,
    Multiplayer.PartyWideMessageQueue message_queue,
    bool no_letter)
  {
    switch (message_queue)
    {
      case Multiplayer.PartyWideMessageQueue.MailForTomorrow:
        Game1.addMailForTomorrow(mail_key, no_letter);
        break;
      case Multiplayer.PartyWideMessageQueue.SeenMail:
        Game1.addMail(mail_key, no_letter);
        break;
    }
    if (no_letter)
      mail_key += "%&NL&%";
    switch (message_queue)
    {
      case Multiplayer.PartyWideMessageQueue.MailForTomorrow:
        mail_key = "%&MFT&%" + mail_key;
        break;
      case Multiplayer.PartyWideMessageQueue.SeenMail:
        mail_key = "%&SM&%" + mail_key;
        break;
    }
    if (!Game1.IsMasterGame || Game1.player.team.broadcastedMail.Contains(mail_key))
      return;
    Game1.player.team.broadcastedMail.Add(mail_key);
  }

  protected void receiveForceKick()
  {
    if (Game1.IsServer)
      return;
    this.Disconnect(Multiplayer.DisconnectType.Kicked);
    this.returnToMainMenu();
  }

  protected virtual void receiveGlobalMessage(IncomingMessage msg)
  {
    string path = msg.Reader.ReadString();
    int num = msg.Reader.ReadBoolean() ? 1 : 0;
    string str = msg.Reader.ReadString();
    if (num != 0 && Game1.hudMessages.Count > 0 || !string.IsNullOrEmpty(str) && str != Game1.player.currentLocation?.NameOrUniqueName)
      return;
    int length = msg.Reader.ReadInt32();
    object[] objArray = new object[length];
    for (int index = 0; index < length; ++index)
      objArray[index] = (object) TokenParser.ParseText(msg.Reader.ReadString());
    Game1.showGlobalMessage(Game1.content.LoadString(path, objArray));
  }

  protected void receiveStartNewDaySync() => Game1.newDaySync.flagServerReady();

  protected void receiveReadySync(IncomingMessage msg) => Game1.netReady.ProcessMessage(msg);

  protected void receiveChestHitSync(IncomingMessage msg)
  {
    Game1.player.team.chestHit.ProcessMessage(msg);
  }

  protected void receiveDedicatedServerSync(IncomingMessage msg)
  {
    Game1.dedicatedServer.ProcessMessage(msg);
  }

  public virtual void processIncomingMessage(IncomingMessage msg)
  {
    GameLocation location;
    switch (msg.MessageType)
    {
      case 0:
        NetFarmerRoot root = this.farmerRoot(msg.Reader.ReadInt64());
        if (!((NetFieldBase<Farmer, NetRef<Farmer>>) root != (NetRef<Farmer>) null))
          break;
        this.readObjectDelta<Farmer>(msg.Reader, (NetRoot<Farmer>) root);
        break;
      case 2:
        this.receivePlayerIntroduction(msg.Reader);
        break;
      case 3:
        this.readActiveLocation(msg);
        break;
      case 4:
        string eventId = msg.Reader.ReadString();
        bool flag = msg.Reader.ReadBoolean();
        bool notify_when_done = msg.Reader.ReadBoolean();
        int tileX = msg.Reader.ReadInt32();
        int tileY = msg.Reader.ReadInt32();
        LocationRequest request = this.readLocationRequest(msg.Reader);
        GameLocation location_for_event_check = Game1.getLocationFromName(request.Name);
        if (location_for_event_check?.findEventById(eventId) == null)
        {
          Game1.log.Warn($"Couldn't find event {eventId} for broadcast event!");
          break;
        }
        Farmer farmerActor = flag ? (Game1.player.NetFields.Root as NetRoot<Farmer>).Clone().Value : (msg.SourceFarmer.NetFields.Root as NetRoot<Farmer>).Clone().Value;
        Point oldTile = Game1.player.TilePoint;
        string oldLocation = Game1.player.currentLocation.NameOrUniqueName;
        int direction = Game1.player.facingDirection.Value;
        Game1.player.locationBeforeForcedEvent.Value = oldLocation;
        request.OnWarp += (LocationRequest.Callback) (() =>
        {
          farmerActor.currentLocation = Game1.currentLocation;
          farmerActor.completelyStopAnimatingOrDoingAction();
          farmerActor.UsingTool = false;
          farmerActor.Items.Clear();
          farmerActor.hidden.Value = false;
          Event eventById = Game1.currentLocation.findEventById(eventId, farmerActor);
          eventById.notifyWhenDone = notify_when_done;
          eventById.notifyLocationName = location_for_event_check.NameOrUniqueName;
          eventById.notifyLocationIsStructure = (byte) request.IsStructure;
          Game1.currentLocation.startEvent(eventById);
          farmerActor.Position = Game1.player.Position;
          Game1.warpingForForcedRemoteEvent = false;
          string str = Game1.player.locationBeforeForcedEvent.Value;
          Game1.player.locationBeforeForcedEvent.Value = (string) null;
          eventById.setExitLocation(oldLocation, oldTile.X, oldTile.Y);
          Game1.player.locationBeforeForcedEvent.Value = str;
          Game1.player.orientationBeforeEvent = direction;
        });
        Game1.remoteEventQueue.Add(new Action(PerformForcedEvent));
        break;

        void PerformForcedEvent()
        {
          Game1.warpingForForcedRemoteEvent = true;
          Game1.player.completelyStopAnimatingOrDoingAction();
          Game1.warpFarmer(request, tileX, tileY, Game1.player.FacingDirection);
        }
      case 6:
        location = this.readLocation(msg.Reader);
        if (location == null)
          break;
        this.readObjectDelta<GameLocation>(msg.Reader, location.Root);
        break;
      case 7:
        location = this.readLocation(msg.Reader);
        if (location == null)
          break;
        this.readSprites(msg.Reader, location, (Action<TemporaryAnimatedSprite>) (sprite => location.temporarySprites.Add(sprite)));
        break;
      case 8:
        NPC character = this.readNPC(msg.Reader);
        location = this.readLocation(msg.Reader);
        if (character == null || location == null)
          break;
        Game1.warpCharacter(character, location, msg.Reader.ReadVector2());
        break;
      case 10:
        long recipientID = msg.Reader.ReadInt64();
        LocalizedContentManager.LanguageCode language = msg.Reader.ReadEnum<LocalizedContentManager.LanguageCode>();
        string message = msg.Reader.ReadString();
        this.receiveChatMessage(msg.SourceFarmer, recipientID, language, message);
        break;
      case 12:
        this.receiveWorldState(msg.Reader);
        break;
      case 13:
        this.receiveTeamDelta(msg.Reader);
        break;
      case 14:
        this.receiveNewDaySync(msg);
        break;
      case 15:
        string messageKey = msg.Reader.ReadString();
        string[] args = new string[(int) msg.Reader.ReadByte()];
        for (int index = 0; index < args.Length; ++index)
          args[index] = msg.Reader.ReadString();
        this.receiveChatInfoMessage(msg.SourceFarmer, messageKey, args);
        break;
      case 17:
        this.receiveFarmerGainExperience(msg);
        break;
      case 18:
        this.parseServerToClientsMessage(msg.Reader.ReadString());
        break;
      case 19:
        this.playerDisconnected(msg.SourceFarmer.UniqueMultiplayerID);
        break;
      case 20:
        this.receiveSharedAchievement(msg);
        break;
      case 21:
        this.receiveGlobalMessage(msg);
        break;
      case 22:
        this.receivePartyWideMail(msg);
        break;
      case 23:
        this.receiveForceKick();
        break;
      case 24:
        this.receiveRemoveLocationFromLookup(msg);
        break;
      case 25:
        this.receiveFarmerKilledMonster(msg);
        break;
      case 26:
        this.receiveRequestGrandpaReevaluation(msg);
        break;
      case 27:
        this.receiveNutDig(msg);
        break;
      case 28:
        this.receivePassoutRequest(msg);
        break;
      case 29:
        this.receivePassout(msg);
        break;
      case 30:
        this.receiveStartNewDaySync();
        break;
      case 31 /*0x1F*/:
        this.receiveReadySync(msg);
        break;
      case 32 /*0x20*/:
        this.receiveChestHitSync(msg);
        break;
      case 33:
        this.receiveDedicatedServerSync(msg);
        break;
      case 127 /*0x7F*/:
        Game1.log.Warn("Unexpectedly received a compressed multiplayer message that wasn't decompressed by the net client.");
        break;
    }
  }

  public virtual void StartLocalMultiplayerServer()
  {
    Game1.server = (IGameServer) new GameServer(true);
    Game1.server.startServer();
  }

  public virtual void StartServer()
  {
    Game1.server = (IGameServer) new GameServer();
    Game1.server.startServer();
  }

  public virtual void Disconnect(Multiplayer.DisconnectType disconnectType)
  {
    if (Game1.server != null)
    {
      Game1.server.stopServer();
      Game1.server = (IGameServer) null;
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
        this.playerDisconnected(key);
    }
    if (Game1.client != null)
    {
      this.sendFarmhand();
      this.UpdateLate(true);
      Game1.client.disconnect();
      Game1.client = (Client) null;
    }
    Game1.otherFarmers.Clear();
    Multiplayer.LogDisconnect(disconnectType);
  }

  protected virtual void updatePendingConnections()
  {
    switch (Game1.multiplayerMode)
    {
      case 1:
        if (Game1.client == null || Game1.client.readyToPlay)
          break;
        Game1.client.receiveMessages();
        break;
      case 2:
        if (Game1.server != null || !Game1.options.enableServer)
          break;
        this.StartServer();
        break;
    }
  }

  public void UpdateLoading()
  {
    this.updatePendingConnections();
    if (Game1.server == null)
      return;
    Game1.server.receiveMessages();
  }

  public virtual void UpdateEarly()
  {
    this.updatePendingConnections();
    if (Game1.multiplayerMode == (byte) 2 && (NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null && Game1.options.enableServer)
      Game1.server.initializeHost();
    if (Game1.server != null)
      Game1.server.receiveMessages();
    else if (Game1.client != null)
      Game1.client.receiveMessages();
    this.updateRoots();
    if (Game1.CurrentEvent != null)
      return;
    this.removeDisconnectedFarmers();
  }

  public virtual void UpdateLate(bool forceSync = false)
  {
    if (Game1.multiplayerMode != (byte) 0)
    {
      if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.farmerDeltaBroadcastPeriod == 0)
        this.broadcastFarmerDeltas();
      if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.locationDeltaBroadcastPeriod == 0)
        this.broadcastLocationDeltas();
      if (!this.allowSyncDelay() | forceSync || Game1.ticks % this.worldStateDeltaBroadcastPeriod == 0)
        this.broadcastWorldStateDeltas();
    }
    if (Game1.server != null)
      Game1.server.sendMessages();
    if (Game1.client == null)
      return;
    Game1.client.sendMessages();
  }

  public virtual void inviteAccepted()
  {
    if (!(Game1.activeClickableMenu is TitleMenu activeClickableMenu))
      return;
    switch (TitleMenu.subMenu)
    {
      case null:
        activeClickableMenu.performButtonAction("Invite");
        break;
      case FarmhandMenu _:
      case CoopMenu _:
        TitleMenu.subMenu = (IClickableMenu) new FarmhandMenu();
        break;
    }
  }

  public virtual Client InitClient(Client client) => client;

  public virtual Server InitServer(Server server) => server;

  public enum PartyWideMessageQueue
  {
    MailForTomorrow,
    SeenMail,
  }

  public enum DisconnectType
  {
    None,
    ClosedGame,
    ExitedToMainMenu,
    ExitedToMainMenu_FromFarmhandSelect,
    HostLeft,
    ServerOfflineMode,
    ServerFull,
    Kicked,
    AcceptedOtherInvite,
    ClientTimeout,
    LidgrenTimeout,
    GalaxyTimeout,
    Timeout_FarmhandSelection,
    LidgrenDisconnect_Unknown,
  }
}
