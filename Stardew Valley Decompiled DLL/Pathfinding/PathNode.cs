// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.PathNode
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Pathfinding;

public class PathNode : IEquatable<PathNode>
{
  public readonly int x;
  public readonly int y;
  public readonly int id;
  public byte g;
  public PathNode parent;

  public PathNode(int x, int y, PathNode parent)
  {
    this.x = x;
    this.y = y;
    this.parent = parent;
    this.id = PathNode.ComputeHash(x, y);
  }

  public PathNode(int x, int y, byte g, PathNode parent)
  {
    this.x = x;
    this.y = y;
    this.g = g;
    this.parent = parent;
    this.id = PathNode.ComputeHash(x, y);
  }

  public bool Equals(PathNode obj) => obj != null && this.x == obj.x && this.y == obj.y;

  public override bool Equals(object obj)
  {
    return obj is PathNode pathNode && this.x == pathNode.x && this.y == pathNode.y;
  }

  public override int GetHashCode() => this.id;

  public static int ComputeHash(int x, int y) => 100000 * x + y;
}
