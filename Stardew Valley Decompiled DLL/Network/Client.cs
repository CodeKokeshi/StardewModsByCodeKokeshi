// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.Client
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public abstract class Client : IBandwidthMonitor
{
  public const int connectionTimeout = 45000;
  public bool hasHandshaked;
  public bool readyToPlay;
  public bool timedOut;
  public bool connectionStarted;
  public string serverName = "???";
  public string connectionMessage;
  public Multiplayer.DisconnectType pendingDisconnect;
  protected BandwidthLogger bandwidthLogger;
  protected long? timeoutTime;
  public List<Farmer> availableFarmhands;
  public Dictionary<long, string> userNames = new Dictionary<long, string>();

  protected abstract void connectImpl();

  public abstract void disconnect(bool neatly = true);

  protected abstract void receiveMessagesImpl();

  public abstract void sendMessage(OutgoingMessage message);

  public abstract string getUserID();

  protected abstract string getHostUserName();

  public virtual float GetPingToHost() => 0.0f;

  public virtual string getUserName(long farmerId)
  {
    return farmerId != Game1.serverHost.Value.UniqueMultiplayerID ? this.userNames.GetValueOrDefault<long, string>(farmerId, "?") : this.getHostUserName();
  }

  public virtual void connect()
  {
    Game1.log.Verbose("Starting client. Protocol version: " + Multiplayer.protocolVersion);
    this.connectionMessage = (string) null;
    if (this.connectionStarted)
      return;
    this.connectionStarted = true;
    this.connectImpl();
    this.timeoutTime = new long?(DateTime.UtcNow.Ticks / 10000L + 45000L);
  }

  public virtual void receiveMessages()
  {
    this.receiveMessagesImpl();
    if (this.hasHandshaked)
      this.timeoutTime = new long?();
    if (this.timeoutTime.HasValue && DateTime.UtcNow.Ticks / 10000L >= this.timeoutTime.Value)
    {
      this.pendingDisconnect = Multiplayer.DisconnectType.ClientTimeout;
      this.timedOut = true;
      this.disconnect(false);
      Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ClientTimeout);
    }
    this.bandwidthLogger?.Update();
  }

  protected virtual void processIncomingMessage(IncomingMessage message)
  {
    byte messageType = message.MessageType;
    if (messageType <= (byte) 9)
    {
      switch ((int) messageType - 1)
      {
        case 0:
          this.receiveServerIntroduction(message.Reader);
          return;
        case 1:
          this.userNames[message.FarmerID] = message.Reader.ReadString();
          Game1.multiplayer.processIncomingMessage(message);
          return;
        case 2:
          Game1.multiplayer.processIncomingMessage(message);
          return;
        default:
          if (messageType == (byte) 9)
          {
            this.receiveAvailableFarmhands(message.Reader);
            return;
          }
          break;
      }
    }
    else if (messageType != (byte) 11)
    {
      if (messageType == (byte) 16 /*0x10*/)
      {
        if (message.FarmerID != Game1.serverHost.Value.UniqueMultiplayerID)
          return;
        this.receiveUserNameUpdate(message.Reader);
        return;
      }
    }
    else
    {
      this.connectionMessage = Game1.content.LoadString(message.Reader.ReadString());
      return;
    }
    Game1.multiplayer.processIncomingMessage(message);
  }

  protected virtual void receiveUserNameUpdate(BinaryReader msg)
  {
    this.userNames[msg.ReadInt64()] = msg.ReadString();
  }

  protected virtual void receiveAvailableFarmhands(BinaryReader msg)
  {
    int num1 = msg.ReadInt32();
    int num2 = msg.ReadInt32();
    int num3 = msg.ReadInt32();
    int num4 = (int) msg.ReadByte();
    this.availableFarmhands = new List<Farmer>();
    while (this.availableFarmhands.Count < num4)
    {
      NetFarmerRoot netFarmerRoot = new NetFarmerRoot();
      netFarmerRoot.ReadFull(msg, new NetVersion());
      netFarmerRoot.MarkReassigned();
      netFarmerRoot.MarkClean();
      Farmer farmer = netFarmerRoot.Value;
      this.availableFarmhands.Add(farmer);
      farmer.yearForSaveGame = new int?(num1);
      farmer.seasonForSaveGame = new int?(num2);
      farmer.dayOfMonthForSaveGame = new int?(num3);
    }
    this.hasHandshaked = true;
    this.connectionMessage = (string) null;
    switch (Game1.activeClickableMenu)
    {
      case TitleMenu _:
        break;
      case FarmhandMenu _:
        break;
      default:
        using (List<Farmer>.Enumerator enumerator = this.availableFarmhands.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            Game1.player = enumerator.Current;
            this.sendPlayerIntroduction();
            break;
          }
        }
        Game1.multiplayer.Disconnect(Multiplayer.DisconnectType.ServerFull);
        break;
    }
  }

  public virtual bool PopulatePlatformData(Farmer farmer) => false;

  public virtual void sendPlayerIntroduction()
  {
    if (this.getUserID() != "")
    {
      string userId = this.getUserID();
      Game1.log.Verbose("sendPlayerIntroduction " + userId);
      Game1.player.userID.Value = userId;
    }
    this.PopulatePlatformData(Game1.player);
    (Game1.player.NetFields.Root as NetRoot<Farmer>).MarkClean();
    this.sendMessage((byte) 2, (object) Game1.multiplayer.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) (Game1.player.NetFields.Root as NetFarmerRoot), new long?()));
  }

  protected virtual void setUpGame()
  {
    Game1.flushLocationLookup();
    Game1.player.updateFriendshipGifts(Game1.Date);
    Game1.gameMode = (byte) 3;
    Game1.stats.checkForAchievements();
    Game1.multiplayerMode = (byte) 1;
    Game1.client = this;
    this.readyToPlay = true;
    BedFurniture.ApplyWakeUpPosition(Game1.player);
    Game1.fadeClear();
    Game1.currentLocation.updateSeasonalTileSheets();
    Game1.currentLocation.resetForPlayerEntry();
    Game1.player.sleptInTemporaryBed.Value = false;
    Game1.initializeVolumeLevels();
    if (Game1.MasterPlayer.eventsSeen.Contains("558291"))
      Game1.player.songsHeard.Add("grandpas_theme");
    Game1.AddNPCs();
    Game1.AddModNPCs();
    Utility.ForEachVillager((Func<NPC, bool>) (villager =>
    {
      villager.ChooseAppearance();
      return true;
    }));
    Game1.exitActiveMenu();
    if (!Game1.player.isCustomized.Value)
      Game1.activeClickableMenu = (IClickableMenu) new CharacterCustomization(CharacterCustomization.Source.NewFarmhand);
    Game1.player.team.AddAnyBroadcastedMail();
    if (Game1.shouldPlayMorningSong(true))
      Game1.playMorningSong();
    for (int index = 1; index < Game1.netWorldState.Value.HighestPlayerLimit; ++index)
    {
      int num = index + 1;
      if (Game1.getLocationFromName("Cellar" + num.ToString()) == null)
      {
        GameLocation gameLocation = Game1.CreateGameLocation("Cellar");
        if (gameLocation == null)
        {
          Game1.log.Error("Couldn't create 'Cellar' location. Was it removed from Data/Locations?");
        }
        else
        {
          NetString name = gameLocation.name;
          string str1 = name.Value;
          num = index + 1;
          string str2 = num.ToString();
          name.Value = str1 + str2;
          Game1.locations.Add(gameLocation);
        }
      }
    }
    Game1.player.showToolUpgradeAvailability();
    Game1.dayTimeMoneyBox.questsDirty = true;
    Game1.player.ReequipEnchantments();
    foreach (Item obj in Game1.player.Items)
    {
      if (obj is StardewValley.Object @object)
        @object.reloadSprite();
    }
    Game1.player.companions.Clear();
    Game1.player.resetAllTrinketEffects();
    Game1.player.isSitting.Value = false;
    Game1.player.mount?.dismount();
    Game1.player.forceCanMove();
    Game1.player.viewingLocation.Value = (string) null;
    Game1.player.timeWentToBed.Value = 0;
  }

  protected virtual void receiveServerIntroduction(BinaryReader msg)
  {
    Game1.otherFarmers.Roots[Game1.player.UniqueMultiplayerID] = (NetRoot<Farmer>) (Game1.player.NetFields.Root as NetFarmerRoot);
    NetFarmerRoot netFarmerRoot = Game1.multiplayer.readFarmer(msg);
    long uniqueMultiplayerId = netFarmerRoot.Value.UniqueMultiplayerID;
    Game1.serverHost = netFarmerRoot;
    Game1.serverHost.Value.teamRoot = Game1.multiplayer.readObjectFull<FarmerTeam>(msg);
    Game1.otherFarmers.Roots.Add(uniqueMultiplayerId, (NetRoot<Farmer>) netFarmerRoot);
    Game1.player.teamRoot = Game1.serverHost.Value.teamRoot;
    Game1.netWorldState = Game1.multiplayer.readObjectFull<NetWorldState>(msg);
    Game1.netWorldState.Clock.InterpolationTicks = 0;
    Game1.netWorldState.Value.WriteToGame1(true);
    this.setUpGame();
    if (Game1.chatBox == null)
      return;
    Game1.chatBox.listPlayers();
  }

  public virtual void sendMessages()
  {
    if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null)
      return;
    foreach (OutgoingMessage message in (IEnumerable<OutgoingMessage>) Game1.serverHost.Value.messageQueue)
      this.sendMessage(message);
    foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
      otherFarmer.Value.messageQueue.Clear();
  }

  public virtual void sendMessage(byte which, params object[] data)
  {
    this.sendMessage(new OutgoingMessage(which, Game1.player, data));
  }

  public BandwidthLogger BandwidthLogger => this.bandwidthLogger;

  public bool LogBandwidth
  {
    get => this.bandwidthLogger != null;
    set => this.bandwidthLogger = value ? new BandwidthLogger() : (BandwidthLogger) null;
  }
}
