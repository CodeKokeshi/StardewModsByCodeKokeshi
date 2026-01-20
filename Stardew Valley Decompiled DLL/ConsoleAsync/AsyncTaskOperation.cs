// Decompiled with JetBrains decompiler
// Type: StardewValley.ConsoleAsync.AsyncTaskOperation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Threading.Tasks;

#nullable disable
namespace StardewValley.ConsoleAsync;

public abstract class AsyncTaskOperation : IAsyncOperation
{
  public Task Task;
  public bool TaskStarted;

  bool IAsyncOperation.Started => this.TaskStarted;

  public abstract bool Done { get; }

  void IAsyncOperation.Begin()
  {
    DebugTools.Assert(!this.TaskStarted, "AsyncTaskOperation.Begin called but TaskStarted already is true!");
    this.TaskStarted = true;
    this.Task.Start();
  }

  public abstract void Conclude();
}
