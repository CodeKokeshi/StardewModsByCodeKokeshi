// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetReady.ReadySynchronizer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network.NetReady.Internal;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network.NetReady;

/// <summary>Manages and synchronizes ready checks, which ensure all players are ready before proceeding (e.g. before sleeping).</summary>
public class ReadySynchronizer
{
  /// <summary>The active ready checks by ID.</summary>
  private readonly Dictionary<string, BaseReadyCheck> ReadyChecks = new Dictionary<string, BaseReadyCheck>();

  /// <summary>Set the players that are needed for this ready check to pass.</summary>
  /// <param name="id">The ready check ID.</param>
  /// <param name="requiredFarmers">The required player IDs.</param>
  public void SetLocalRequiredFarmers(string id, List<Farmer> requiredFarmers)
  {
    List<long> farmerIds = new List<long>();
    foreach (Farmer requiredFarmer in requiredFarmers)
      farmerIds.Add(requiredFarmer.UniqueMultiplayerID);
    this.GetOrCreate(id).SetRequiredFarmers(farmerIds);
  }

  /// <summary>Set whether the local player is ready to proceed.</summary>
  /// <param name="id">The ready check ID.</param>
  /// <param name="ready">Whether the local player is ready.</param>
  public void SetLocalReady(string id, bool ready) => this.GetOrCreate(id).SetLocalReady(ready);

  /// <summary>Get whether all required players are ready to proceed.</summary>
  /// <param name="id">The ready check ID.</param>
  public bool IsReady(string id)
  {
    BaseReadyCheck ifExists = this.GetIfExists(id);
    return ifExists != null && ifExists.IsReady;
  }

  /// <summary>Get whether we can still cancel our acceptance of a ready check.</summary>
  /// <param name="id">The ready check ID.</param>
  public bool IsReadyCheckCancelable(string id)
  {
    BaseReadyCheck ifExists = this.GetIfExists(id);
    return ifExists != null && ifExists.IsCancelable;
  }

  /// <summary>Get the number of players that are ready to proceed.</summary>
  /// <param name="id">The ready check ID.</param>
  public int GetNumberReady(string id)
  {
    BaseReadyCheck ifExists = this.GetIfExists(id);
    return ifExists == null ? 0 : ifExists.NumberReady;
  }

  /// <summary>Get the number of players that are required to proceed.</summary>
  /// <param name="id">The ready check ID.</param>
  public int GetNumberRequired(string id)
  {
    BaseReadyCheck ifExists = this.GetIfExists(id);
    return ifExists == null ? 0 : ifExists.NumberRequired;
  }

  /// <summary>Update all ready checks.</summary>
  public void Update()
  {
    foreach (BaseReadyCheck baseReadyCheck in this.ReadyChecks.Values)
      baseReadyCheck.Update();
  }

  /// <summary>Clear all ready checks.</summary>
  public void Reset() => this.ReadyChecks.Clear();

  /// <summary>Process an incoming ready check sync message.</summary>
  /// <param name="message">The incoming sync message.</param>
  public void ProcessMessage(IncomingMessage message)
  {
    string id = message.Reader.ReadString();
    ReadyCheckMessageType messageType = (ReadyCheckMessageType) message.Reader.ReadByte();
    this.GetOrCreate(id).ProcessMessage(messageType, message);
  }

  /// <summary>Get a ready check by ID, or <c>null</c> if it doesn't exist.</summary>
  /// <param name="id">The ready check ID.</param>
  private BaseReadyCheck GetIfExists(string id)
  {
    BaseReadyCheck baseReadyCheck;
    return id == null || !this.ReadyChecks.TryGetValue(id, out baseReadyCheck) ? (BaseReadyCheck) null : baseReadyCheck;
  }

  /// <summary>Get a ready check by ID, creating it if needed.</summary>
  /// <param name="id">The ready check ID.</param>
  private BaseReadyCheck GetOrCreate(string id)
  {
    BaseReadyCheck baseReadyCheck1;
    if (this.ReadyChecks.TryGetValue(id, out baseReadyCheck1))
      return baseReadyCheck1;
    BaseReadyCheck baseReadyCheck2 = Game1.IsMasterGame ? (BaseReadyCheck) new ServerReadyCheck(id) : (BaseReadyCheck) new ClientReadyCheck(id);
    this.ReadyChecks.Add(id, baseReadyCheck2);
    return baseReadyCheck2;
  }
}
