// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.PriorityQueue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections.Generic;

#nullable disable
namespace StardewValley.Pathfinding;

public class PriorityQueue
{
  private int total_size;
  private SortedDictionary<int, Queue<PathNode>> nodes;

  public PriorityQueue()
  {
    this.nodes = new SortedDictionary<int, Queue<PathNode>>();
    this.total_size = 0;
  }

  public bool IsEmpty() => this.total_size == 0;

  public void Clear()
  {
    this.total_size = 0;
    foreach (KeyValuePair<int, Queue<PathNode>> node in this.nodes)
      node.Value.Clear();
  }

  public bool Contains(PathNode p, int priority)
  {
    Queue<PathNode> pathNodeQueue;
    return this.nodes.TryGetValue(priority, out pathNodeQueue) && pathNodeQueue.Contains(p);
  }

  public PathNode Dequeue()
  {
    if (!this.IsEmpty())
    {
      foreach (Queue<PathNode> pathNodeQueue in this.nodes.Values)
      {
        if (pathNodeQueue.Count > 0)
        {
          --this.total_size;
          return pathNodeQueue.Dequeue();
        }
      }
    }
    return (PathNode) null;
  }

  public object Peek()
  {
    if (!this.IsEmpty())
    {
      foreach (Queue<PathNode> pathNodeQueue in this.nodes.Values)
      {
        if (pathNodeQueue.Count > 0)
          return (object) pathNodeQueue.Peek();
      }
    }
    return (object) null;
  }

  public object Dequeue(int priority)
  {
    --this.total_size;
    return (object) this.nodes[priority].Dequeue();
  }

  public void Enqueue(PathNode item, int priority)
  {
    Queue<PathNode> pathNodeQueue;
    if (!this.nodes.TryGetValue(priority, out pathNodeQueue))
    {
      this.nodes.Add(priority, new Queue<PathNode>());
      this.Enqueue(item, priority);
    }
    else
    {
      pathNodeQueue.Enqueue(item);
      ++this.total_size;
    }
  }
}
