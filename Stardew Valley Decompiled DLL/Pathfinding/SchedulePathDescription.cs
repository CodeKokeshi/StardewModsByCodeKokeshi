// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.SchedulePathDescription
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Pathfinding;

public class SchedulePathDescription
{
  public Stack<Point> route;
  public int time;
  public int facingDirection;
  public string endOfRouteBehavior;
  public string endOfRouteMessage;
  public string targetLocationName;
  public Point targetTile;

  public SchedulePathDescription(
    Stack<Point> route,
    int facingDirection,
    string endBehavior,
    string endMessage,
    string targetLocationName,
    Point targetTile)
  {
    this.endOfRouteMessage = endMessage;
    this.route = route;
    this.facingDirection = facingDirection;
    this.endOfRouteBehavior = endBehavior;
    this.targetLocationName = targetLocationName;
    this.targetTile = targetTile;
  }
}
