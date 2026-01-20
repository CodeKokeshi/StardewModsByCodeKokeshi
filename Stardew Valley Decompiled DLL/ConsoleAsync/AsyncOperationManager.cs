// Decompiled with JetBrains decompiler
// Type: StardewValley.ConsoleAsync.AsyncOperationManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable disable
namespace StardewValley.ConsoleAsync;

public class AsyncOperationManager
{
  private static AsyncOperationManager _instance;
  private List<IAsyncOperation> _pendingOps;
  private List<IAsyncOperation> _tempOps;
  private List<IAsyncOperation> _doneOps;

  public static AsyncOperationManager Use => AsyncOperationManager._instance;

  public static void Init() => AsyncOperationManager._instance = new AsyncOperationManager();

  private AsyncOperationManager()
  {
    this._pendingOps = new List<IAsyncOperation>();
    this._tempOps = new List<IAsyncOperation>();
    this._doneOps = new List<IAsyncOperation>();
  }

  public void AddPending(Task task, Action<GenericResult> doneAction)
  {
    GenericOp op = new GenericOp();
    op.DoneCallback = new Action(OnDone);
    op.Task = task;
    if (task.Status > TaskStatus.Created)
      op.TaskStarted = true;
    this.AddPending((IAsyncOperation) op);

    void OnDone()
    {
      GenericResult genericResult = new GenericResult();
      genericResult.Ex = (Exception) op.Task.Exception;
      if (genericResult.Ex != null)
        genericResult.Ex = genericResult.Ex.GetBaseException();
      genericResult.Failed = genericResult.Ex != null;
      genericResult.Success = genericResult.Ex == null;
      doneAction(genericResult);
    }
  }

  public void AddPending(Action workAction, Action<GenericResult> doneAction)
  {
    GenericOp op = new GenericOp();
    op.DoneCallback = new Action(OnDone);
    Task task = new Task(workAction);
    op.Task = task;
    this.AddPending((IAsyncOperation) op);

    void OnDone()
    {
      GenericResult genericResult = new GenericResult();
      genericResult.Ex = (Exception) op.Task.Exception;
      if (genericResult.Ex != null)
        genericResult.Ex = genericResult.Ex.GetBaseException();
      genericResult.Failed = genericResult.Ex != null;
      genericResult.Success = genericResult.Ex == null;
      doneAction(genericResult);
    }
  }

  public void AddPending(IAsyncOperation op)
  {
    lock (this._pendingOps)
      this._pendingOps.Add(op);
  }

  public void Update()
  {
    lock (this._pendingOps)
    {
      this._doneOps.Clear();
      this._tempOps.Clear();
      this._tempOps.AddRange((IEnumerable<IAsyncOperation>) this._pendingOps);
      this._pendingOps.Clear();
      bool flag = false;
      for (int index = 0; index < this._tempOps.Count; ++index)
      {
        IAsyncOperation tempOp = this._tempOps[index];
        if (flag)
        {
          this._pendingOps.Add(tempOp);
        }
        else
        {
          flag = true;
          if (!tempOp.Started)
          {
            tempOp.Begin();
            this._pendingOps.Add(tempOp);
          }
          else if (tempOp.Done)
            this._doneOps.Add(tempOp);
          else
            this._pendingOps.Add(tempOp);
        }
      }
      this._tempOps.Clear();
    }
    for (int index = 0; index < this._doneOps.Count; ++index)
      this._doneOps[index].Conclude();
    this._doneOps.Clear();
  }
}
