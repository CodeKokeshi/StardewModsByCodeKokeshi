// Decompiled with JetBrains decompiler
// Type: StardewValley.Friendship
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class Friendship : INetObject<NetFields>
{
  private readonly NetInt points = new NetInt();
  private readonly NetInt giftsThisWeek = new NetInt();
  private readonly NetInt giftsToday = new NetInt();
  private readonly NetRef<WorldDate> lastGiftDate = new NetRef<WorldDate>();
  private readonly NetBool talkedToToday = new NetBool();
  private readonly NetBool proposalRejected = new NetBool();
  private readonly NetRef<WorldDate> weddingDate = new NetRef<WorldDate>();
  private readonly NetRef<WorldDate> nextBirthingDate = new NetRef<WorldDate>();
  private readonly NetEnum<FriendshipStatus> status = new NetEnum<FriendshipStatus>(FriendshipStatus.Friendly);
  private readonly NetLong proposer = new NetLong();
  private readonly NetBool roommateMarriage = new NetBool(false);

  [XmlIgnore]
  public NetFields NetFields { get; } = new NetFields(nameof (Friendship));

  public int Points
  {
    get => this.points.Value;
    set => this.points.Value = value;
  }

  public int GiftsThisWeek
  {
    get => this.giftsThisWeek.Value;
    set => this.giftsThisWeek.Value = value;
  }

  public int GiftsToday
  {
    get => this.giftsToday.Value;
    set => this.giftsToday.Value = value;
  }

  public WorldDate LastGiftDate
  {
    get => this.lastGiftDate.Value;
    set => this.lastGiftDate.Value = value;
  }

  public bool TalkedToToday
  {
    get => this.talkedToToday.Value;
    set => this.talkedToToday.Value = value;
  }

  public bool ProposalRejected
  {
    get => this.proposalRejected.Value;
    set => this.proposalRejected.Value = value;
  }

  public WorldDate WeddingDate
  {
    get => this.weddingDate.Value;
    set => this.weddingDate.Value = value;
  }

  public WorldDate NextBirthingDate
  {
    get => this.nextBirthingDate.Value;
    set => this.nextBirthingDate.Value = value;
  }

  public FriendshipStatus Status
  {
    get => this.status.Value;
    set => this.status.Value = value;
  }

  public long Proposer
  {
    get => this.proposer.Value;
    set => this.proposer.Value = value;
  }

  public bool RoommateMarriage
  {
    get => this.roommateMarriage.Value;
    set => this.roommateMarriage.Value = value;
  }

  public int DaysMarried
  {
    get
    {
      return this.WeddingDate == (WorldDate) null || this.WeddingDate.TotalDays > Game1.Date.TotalDays ? 0 : Game1.Date.TotalDays - this.WeddingDate.TotalDays;
    }
  }

  public int CountdownToWedding
  {
    get
    {
      return this.WeddingDate == (WorldDate) null || this.WeddingDate.TotalDays < Game1.Date.TotalDays ? 0 : this.WeddingDate.TotalDays - Game1.Date.TotalDays;
    }
  }

  public int DaysUntilBirthing
  {
    get
    {
      return this.NextBirthingDate == (WorldDate) null ? -1 : Math.Max(0, this.NextBirthingDate.TotalDays - Game1.Date.TotalDays);
    }
  }

  public Friendship()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.points, nameof (points)).AddField((INetSerializable) this.giftsThisWeek, nameof (giftsThisWeek)).AddField((INetSerializable) this.giftsToday, nameof (giftsToday)).AddField((INetSerializable) this.lastGiftDate, nameof (lastGiftDate)).AddField((INetSerializable) this.talkedToToday, nameof (talkedToToday)).AddField((INetSerializable) this.proposalRejected, nameof (proposalRejected)).AddField((INetSerializable) this.weddingDate, nameof (weddingDate)).AddField((INetSerializable) this.nextBirthingDate, nameof (nextBirthingDate)).AddField((INetSerializable) this.status, nameof (status)).AddField((INetSerializable) this.proposer, nameof (proposer)).AddField((INetSerializable) this.roommateMarriage, nameof (roommateMarriage));
  }

  public Friendship(int startingPoints)
    : this()
  {
    this.Points = startingPoints;
  }

  public void Clear()
  {
    this.points.Value = 0;
    this.giftsThisWeek.Value = 0;
    this.giftsToday.Value = 0;
    this.lastGiftDate.Value = (WorldDate) null;
    this.talkedToToday.Value = false;
    this.proposalRejected.Value = false;
    this.roommateMarriage.Value = false;
    this.weddingDate.Value = (WorldDate) null;
    this.nextBirthingDate.Value = (WorldDate) null;
    this.status.Value = FriendshipStatus.Friendly;
    this.proposer.Value = 0L;
  }

  public bool IsDating()
  {
    return this.Status == FriendshipStatus.Dating || this.Status == FriendshipStatus.Engaged || this.Status == FriendshipStatus.Married;
  }

  public bool IsEngaged() => this.Status == FriendshipStatus.Engaged;

  public bool IsMarried() => this.Status == FriendshipStatus.Married;

  public bool IsDivorced() => this.Status == FriendshipStatus.Divorced;

  public bool IsRoommate() => this.IsMarried() && this.roommateMarriage.Value;
}
