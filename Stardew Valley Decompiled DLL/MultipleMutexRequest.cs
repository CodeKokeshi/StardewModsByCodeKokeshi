// Decompiled with JetBrains decompiler
// Type: StardewValley.MultipleMutexRequest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class MultipleMutexRequest
{
  protected int _reportedCount;
  protected List<NetMutex> _acquiredLocks;
  protected List<NetMutex> _mutexList;
  protected Action<MultipleMutexRequest> _onSuccess;
  protected Action<MultipleMutexRequest> _onFailure;

  public MultipleMutexRequest(
    List<NetMutex> mutexes,
    Action<MultipleMutexRequest> success_callback = null,
    Action<MultipleMutexRequest> failure_callback = null)
  {
    this._onSuccess = success_callback;
    this._onFailure = failure_callback;
    this._acquiredLocks = new List<NetMutex>();
    this._mutexList = new List<NetMutex>((IEnumerable<NetMutex>) mutexes);
    this._RequestMutexes();
  }

  public MultipleMutexRequest(
    NetMutex[] mutexes,
    Action<MultipleMutexRequest> success_callback = null,
    Action<MultipleMutexRequest> failure_callback = null)
  {
    this._onSuccess = success_callback;
    this._onFailure = failure_callback;
    this._acquiredLocks = new List<NetMutex>();
    this._mutexList = new List<NetMutex>((IEnumerable<NetMutex>) mutexes);
    this._RequestMutexes();
  }

  protected void _RequestMutexes()
  {
    if (this._mutexList == null)
    {
      Action<MultipleMutexRequest> onFailure = this._onFailure;
      if (onFailure == null)
        return;
      onFailure(this);
    }
    else if (this._mutexList.Count == 0)
    {
      Action<MultipleMutexRequest> onSuccess = this._onSuccess;
      if (onSuccess == null)
        return;
      onSuccess(this);
    }
    else
    {
      for (int index = 0; index < this._mutexList.Count; ++index)
      {
        if (this._mutexList[index].IsLocked())
        {
          Action<MultipleMutexRequest> onFailure = this._onFailure;
          if (onFailure == null)
            return;
          onFailure(this);
          return;
        }
      }
      for (int index = 0; index < this._mutexList.Count; ++index)
      {
        NetMutex mutex = this._mutexList[index];
        mutex.RequestLock((Action) (() => this._OnLockAcquired(mutex)), (Action) (() => this._OnLockFailed(mutex)));
      }
    }
  }

  protected void _OnLockAcquired(NetMutex mutex)
  {
    ++this._reportedCount;
    this._acquiredLocks.Add(mutex);
    if (this._reportedCount < this._mutexList.Count)
      return;
    this._Finalize();
  }

  protected void _OnLockFailed(NetMutex mutex)
  {
    ++this._reportedCount;
    if (this._reportedCount < this._mutexList.Count)
      return;
    this._Finalize();
  }

  protected void _Finalize()
  {
    if (this._acquiredLocks.Count < this._mutexList.Count)
    {
      this.ReleaseLocks();
      this._onFailure(this);
    }
    else
      this._onSuccess(this);
  }

  public void ReleaseLocks()
  {
    for (int index = 0; index < this._acquiredLocks.Count; ++index)
      this._acquiredLocks[index].ReleaseLock();
    this._acquiredLocks.Clear();
  }
}
