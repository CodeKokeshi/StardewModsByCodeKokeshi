// Decompiled with JetBrains decompiler
// Type: StardewValley.ConsoleAsync.GenericOp
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Threading.Tasks;

#nullable disable
namespace StardewValley.ConsoleAsync;

public sealed class GenericOp : AsyncTaskOperation
{
  public Action DoneCallback;

  public override bool Done => this.Task.Status >= TaskStatus.RanToCompletion;

  public override void Conclude()
  {
    Action doneCallback = this.DoneCallback;
    if (doneCallback == null)
      return;
    doneCallback();
  }

  /// <summary>
  /// Returns true if successful
  /// 
  /// Otherwise will throw the tasks exception.
  /// This should be called from within the Action callback.
  /// </summary>
  public bool Result
  {
    get
    {
      if (this.Task.Status < TaskStatus.RanToCompletion)
        return false;
      if (this.Task.IsFaulted)
      {
        Exception baseException = this.Task.Exception.GetBaseException();
        Console.WriteLine((object) baseException);
        Console.WriteLine("Task failed with exception: {0}.", (object) baseException.Message);
        throw baseException;
      }
      return true;
    }
  }
}
