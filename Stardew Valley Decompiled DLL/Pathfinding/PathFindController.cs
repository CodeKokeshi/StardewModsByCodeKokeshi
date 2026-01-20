// Decompiled with JetBrains decompiler
// Type: StardewValley.Pathfinding.PathFindController
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Audio;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Threading;
using xTile.Tiles;

#nullable disable
namespace StardewValley.Pathfinding;

/// This class finds a path from one point to another using the A* pathfinding algorithm. Then it will guide the given character along that path.
///             Can only be used on maps where the tile width and height are each 127 or less.
[InstanceStatics]
public class PathFindController
{
  public const byte impassable = 255 /*0xFF*/;
  public const int timeToWaitBeforeCancelling = 5000;
  private Character character;
  public GameLocation location;
  public Stack<Point> pathToEndPoint;
  public Point endPoint;
  public int finalFacingDirection;
  public int pausedTimer;
  public PathFindController.endBehavior endBehaviorFunction;
  public bool nonDestructivePathing;
  public bool allowPlayerPathingInEvent;
  public bool NPCSchedule;
  protected static readonly sbyte[,] Directions = new sbyte[4, 2]
  {
    {
      (sbyte) -1,
      (sbyte) 0
    },
    {
      (sbyte) 1,
      (sbyte) 0
    },
    {
      (sbyte) 0,
      (sbyte) 1
    },
    {
      (sbyte) 0,
      (sbyte) -1
    }
  };
  protected static PriorityQueue _openList = new PriorityQueue();
  protected static HashSet<int> _closedList = new HashSet<int>();
  protected static int _counter = 0;
  public int timerSinceLastCheckPoint;

  public PathFindController(
    Character c,
    GameLocation location,
    Point endPoint,
    int finalFacingDirection)
    : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, (PathFindController.endBehavior) null, 10000, endPoint)
  {
  }

  public PathFindController(
    Character c,
    GameLocation location,
    Point endPoint,
    int finalFacingDirection,
    PathFindController.endBehavior endBehaviorFunction)
    : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, (PathFindController.endBehavior) null, 10000, endPoint)
  {
    this.endPoint = endPoint;
    this.endBehaviorFunction = endBehaviorFunction;
  }

  public PathFindController(
    Character c,
    GameLocation location,
    Point endPoint,
    int finalFacingDirection,
    PathFindController.endBehavior endBehaviorFunction,
    int limit)
    : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, (PathFindController.endBehavior) null, limit, endPoint)
  {
    this.endPoint = endPoint;
    this.endBehaviorFunction = endBehaviorFunction;
  }

  public PathFindController(
    Character c,
    GameLocation location,
    Point endPoint,
    int finalFacingDirection,
    bool clearMarriageDialogues = true)
    : this(c, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), finalFacingDirection, (PathFindController.endBehavior) null, 10000, endPoint, clearMarriageDialogues)
  {
  }

  public static bool isAtEndPoint(
    PathNode currentNode,
    Point endPoint,
    GameLocation location,
    Character c)
  {
    return currentNode.x == endPoint.X && currentNode.y == endPoint.Y;
  }

  public PathFindController(
    Stack<Point> pathToEndPoint,
    GameLocation location,
    Character c,
    Point endPoint)
  {
    this.pathToEndPoint = pathToEndPoint;
    this.location = location;
    this.character = c;
    this.endPoint = endPoint;
  }

  public PathFindController(Stack<Point> pathToEndPoint, Character c, GameLocation l)
  {
    this.pathToEndPoint = pathToEndPoint;
    this.character = c;
    this.location = l;
    this.NPCSchedule = true;
  }

  public PathFindController(
    Character c,
    GameLocation location,
    PathFindController.isAtEnd endFunction,
    int finalFacingDirection,
    PathFindController.endBehavior endBehaviorFunction,
    int limit,
    Point endPoint,
    bool clearMarriageDialogues = true)
  {
    this.character = c;
    if (c is NPC npc && npc.CurrentDialogue.Count > 0 && npc.CurrentDialogue.Peek().removeOnNextMove)
      npc.CurrentDialogue.Pop();
    if (npc != null & clearMarriageDialogues)
    {
      if (npc.currentMarriageDialogue.Count > 0)
        npc.currentMarriageDialogue.Clear();
      npc.shouldSayMarriageDialogue.Value = false;
    }
    this.location = location;
    this.endBehaviorFunction = endBehaviorFunction;
    if (endPoint == Point.Zero)
      endPoint = c.TilePoint;
    this.finalFacingDirection = finalFacingDirection;
    if (!(this.character is NPC) && !this.isPlayerPresent() && endFunction == new PathFindController.isAtEnd(PathFindController.isAtEndPoint) && endPoint.X > 0 && endPoint.Y > 0)
      this.character.Position = new Vector2((float) (endPoint.X * 64 /*0x40*/), (float) (endPoint.Y * 64 /*0x40*/ - 32 /*0x20*/));
    else
      this.pathToEndPoint = PathFindController.findPath(c.TilePoint, endPoint, endFunction, location, this.character, limit);
  }

  public bool isPlayerPresent() => this.location.farmers.Any();

  public virtual bool update(GameTime time)
  {
    if (this.pathToEndPoint == null || this.pathToEndPoint.Count == 0)
      return true;
    if (!this.NPCSchedule && !this.isPlayerPresent() && this.endPoint.X > 0 && this.endPoint.Y > 0)
    {
      this.character.Position = new Vector2((float) (this.endPoint.X * 64 /*0x40*/), (float) (this.endPoint.Y * 64 /*0x40*/ - 32 /*0x20*/));
      return true;
    }
    if (Game1.activeClickableMenu == null || Game1.IsMultiplayer)
    {
      int sinceLastCheckPoint = this.timerSinceLastCheckPoint;
      TimeSpan elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.timerSinceLastCheckPoint = sinceLastCheckPoint + milliseconds1;
      Vector2 position = this.character.Position;
      this.moveCharacter(time);
      if (this.character.Position.Equals(position))
      {
        int pausedTimer = this.pausedTimer;
        elapsedGameTime = time.ElapsedGameTime;
        int milliseconds2 = elapsedGameTime.Milliseconds;
        this.pausedTimer = pausedTimer + milliseconds2;
      }
      else
        this.pausedTimer = 0;
      if (!this.NPCSchedule && this.pausedTimer > 5000)
        return true;
    }
    return false;
  }

  public static Stack<Point> findPath(
    Point startPoint,
    Point endPoint,
    PathFindController.isAtEnd endPointFunction,
    GameLocation location,
    Character character,
    int limit)
  {
    if (Interlocked.Increment(ref PathFindController._counter) != 1)
      throw new Exception();
    try
    {
      bool flag = character is FarmAnimal farmAnimal && farmAnimal.CanSwim() && farmAnimal.isSwimming.Value;
      PathFindController._openList.Clear();
      PathFindController._closedList.Clear();
      PriorityQueue openList = PathFindController._openList;
      HashSet<int> closedList = PathFindController._closedList;
      int num1 = 0;
      openList.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
      int layerWidth = location.map.Layers[0].LayerWidth;
      int layerHeight = location.map.Layers[0].LayerHeight;
      while (!openList.IsEmpty())
      {
        PathNode pathNode1 = openList.Dequeue();
        if (endPointFunction(pathNode1, endPoint, location, character))
          return PathFindController.reconstructPath(pathNode1);
        closedList.Add(pathNode1.id);
        int num2 = (int) (byte) ((uint) pathNode1.g + 1U);
        for (int index = 0; index < 4; ++index)
        {
          int x = pathNode1.x + (int) PathFindController.Directions[index, 0];
          int y = pathNode1.y + (int) PathFindController.Directions[index, 1];
          int hash = PathNode.ComputeHash(x, y);
          if (!closedList.Contains(hash))
          {
            if ((x != endPoint.X || y != endPoint.Y) && (x < 0 || y < 0 || x >= layerWidth || y >= layerHeight))
            {
              closedList.Add(hash);
            }
            else
            {
              PathNode pathNode2 = new PathNode(x, y, pathNode1);
              pathNode2.g = (byte) ((uint) pathNode1.g + 1U);
              if (!flag && location.isCollidingPosition(new Rectangle(pathNode2.x * 64 /*0x40*/ + 1, pathNode2.y * 64 /*0x40*/ + 1, 62, 62), Game1.viewport, character is Farmer, 0, false, character, true))
              {
                closedList.Add(hash);
              }
              else
              {
                int priority = num2 + (Math.Abs(endPoint.X - x) + Math.Abs(endPoint.Y - y));
                closedList.Add(hash);
                openList.Enqueue(pathNode2, priority);
              }
            }
          }
        }
        ++num1;
        if (num1 >= limit)
          return (Stack<Point>) null;
      }
      return (Stack<Point>) null;
    }
    finally
    {
      if (Interlocked.Decrement(ref PathFindController._counter) != 0)
        throw new Exception();
    }
  }

  public static Stack<Point> reconstructPath(PathNode finalNode)
  {
    Stack<Point> pointStack = new Stack<Point>();
    pointStack.Push(new Point(finalNode.x, finalNode.y));
    for (PathNode parent = finalNode.parent; parent != null; parent = parent.parent)
      pointStack.Push(new Point(parent.x, parent.y));
    return pointStack;
  }

  protected virtual void moveCharacter(GameTime time)
  {
    Point point = this.pathToEndPoint.Peek();
    Rectangle rectangle = new Rectangle(point.X * 64 /*0x40*/, point.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    rectangle.Inflate(-2, 0);
    Rectangle boundingBox = this.character.GetBoundingBox();
    if ((rectangle.Contains(boundingBox) || boundingBox.Width > rectangle.Width && rectangle.Contains(boundingBox.Center)) && rectangle.Bottom - boundingBox.Bottom >= 2)
    {
      this.timerSinceLastCheckPoint = 0;
      this.pathToEndPoint.Pop();
      this.character.stopWithoutChangingFrame();
      if (this.pathToEndPoint.Count != 0)
        return;
      this.character.Halt();
      if (this.finalFacingDirection != -1)
        this.character.faceDirection(this.finalFacingDirection);
      if (this.NPCSchedule)
      {
        NPC character = this.character as NPC;
        character.DirectionsToNewLocation = (SchedulePathDescription) null;
        character.endOfRouteMessage.Value = character.nextEndOfRouteMessage;
      }
      PathFindController.endBehavior behaviorFunction = this.endBehaviorFunction;
      if (behaviorFunction == null)
        return;
      behaviorFunction(this.character, this.location);
    }
    else
    {
      if (this.character is Farmer character1)
        character1.movementDirections.Clear();
      else if (!(this.location is MovieTheater))
      {
        string name = this.character.Name;
        for (int index = 0; index < this.location.characters.Count; ++index)
        {
          NPC character = this.location.characters[index];
          if (!character.Equals((object) this.character) && character.GetBoundingBox().Intersects(boundingBox) && character.isMoving() && string.Compare(character.Name, name, StringComparison.Ordinal) < 0)
          {
            this.character.Halt();
            return;
          }
        }
      }
      if (boundingBox.Left < rectangle.Left && boundingBox.Right < rectangle.Right)
        this.character.SetMovingRight(true);
      else if (boundingBox.Right > rectangle.Right && boundingBox.Left > rectangle.Left)
        this.character.SetMovingLeft(true);
      else if (boundingBox.Top <= rectangle.Top)
        this.character.SetMovingDown(true);
      else if (boundingBox.Bottom >= rectangle.Bottom - 2)
        this.character.SetMovingUp(true);
      this.character.MovePosition(time, Game1.viewport, this.location);
      if (this.nonDestructivePathing)
      {
        if (rectangle.Intersects(this.character.nextPosition(this.character.FacingDirection)))
        {
          Vector2 vector2 = this.character.nextPositionVector2();
          StardewValley.Object objectAt = this.location.getObjectAt((int) vector2.X, (int) vector2.Y);
          if (objectAt != null)
          {
            if (objectAt is Fence fence && fence.isGate.Value)
              fence.toggleGate(true);
            else if (!objectAt.isPassable())
            {
              this.character.Halt();
              this.character.controller = (PathFindController) null;
              return;
            }
          }
        }
        this.handleWarps(this.character.nextPosition(this.character.getDirection()));
      }
      else
      {
        if (!this.NPCSchedule)
          return;
        this.handleWarps(this.character.nextPosition(this.character.getDirection()));
      }
    }
  }

  public void handleWarps(Rectangle position)
  {
    Warp warp = this.location.isCollidingWithWarpOrDoor(position, this.character);
    if (warp == null)
      return;
    if (warp.TargetName == "Trailer" && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
      warp = new Warp(warp.X, warp.Y, "Trailer_Big", 13, 24, false);
    if (this.character is NPC character1 && character1.isMarried() && character1.followSchedule)
    {
      GameLocation location = this.location;
      if (!(location is FarmHouse))
      {
        if (location is BusStop && warp.X <= 9)
        {
          GameLocation home = character1.getHome();
          Point entryLocation = ((FarmHouse) home).getEntryLocation();
          warp = new Warp(warp.X, warp.Y, home.name.Value, entryLocation.X, entryLocation.Y, false);
        }
      }
      else
        warp = new Warp(warp.X, warp.Y, "BusStop", 10, 23, false);
      if (character1.temporaryController != null && character1.controller != null)
        character1.controller.location = Game1.RequireLocation(warp.TargetName);
    }
    string str1 = warp.TargetName;
    foreach (string activePassiveFestival in (IEnumerable<string>) Game1.netWorldState.Value.ActivePassiveFestivals)
    {
      PassiveFestivalData data;
      string str2;
      if (Utility.TryGetPassiveFestivalData(activePassiveFestival, out data) && data.MapReplacements != null && data.MapReplacements.TryGetValue(str1, out str2))
      {
        str1 = str2;
        break;
      }
    }
    if (this.character is NPC character2 && (warp.TargetName == "FarmHouse" || warp.TargetName == "Cabin") && character2.isMarried() && character2.getSpouse() != null)
    {
      this.location = (GameLocation) Utility.getHomeOfFarmer(character2.getSpouse());
      Point entryLocation = ((FarmHouse) this.location).getEntryLocation();
      warp = new Warp(warp.X, warp.Y, this.location.name.Value, entryLocation.X, entryLocation.Y, false);
      if (character2.temporaryController != null && character2.controller != null)
        character2.controller.location = this.location;
      Game1.warpCharacter(character2, this.location, new Vector2((float) warp.TargetX, (float) warp.TargetY));
    }
    else
    {
      this.location = Game1.RequireLocation(str1);
      Game1.warpCharacter(this.character as NPC, warp.TargetName, new Vector2((float) warp.TargetX, (float) warp.TargetY));
    }
    if (this.isPlayerPresent() && this.location.doors.ContainsKey(new Point(warp.X, warp.Y)))
      this.location.playSound("doorClose", new Vector2?(new Vector2((float) warp.X, (float) warp.Y)), context: SoundContext.NPC);
    if (this.isPlayerPresent() && this.location.doors.ContainsKey(new Point(warp.TargetX, warp.TargetY - 1)))
      this.location.playSound("doorClose", new Vector2?(new Vector2((float) warp.TargetX, (float) warp.TargetY)), context: SoundContext.NPC);
    if (this.pathToEndPoint.Count > 0)
      this.pathToEndPoint.Pop();
    Point tilePoint = this.character.TilePoint;
    while (this.pathToEndPoint.Count > 0 && (Math.Abs(this.pathToEndPoint.Peek().X - tilePoint.X) > 1 || Math.Abs(this.pathToEndPoint.Peek().Y - tilePoint.Y) > 1))
      this.pathToEndPoint.Pop();
  }

  [Obsolete("Use findPathForNPCSchedules overload with 'npc' parameter.")]
  public static Stack<Point> findPathForNPCSchedules(
    Point startPoint,
    Point endPoint,
    GameLocation location,
    int limit)
  {
    return PathFindController.findPathForNPCSchedules(startPoint, endPoint, location, limit, (Character) null);
  }

  public static Stack<Point> findPathForNPCSchedules(
    Point startPoint,
    Point endPoint,
    GameLocation location,
    int limit,
    Character npc)
  {
    PriorityQueue priorityQueue = new PriorityQueue();
    HashSet<int> intSet = new HashSet<int>();
    int num = 0;
    priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, (byte) 0, (PathNode) null), Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
    PathNode pathNode1 = (PathNode) priorityQueue.Peek();
    int layerWidth = location.map.Layers[0].LayerWidth;
    int layerHeight = location.map.Layers[0].LayerHeight;
    while (!priorityQueue.IsEmpty())
    {
      PathNode pathNode2 = priorityQueue.Dequeue();
      if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
        return PathFindController.reconstructPath(pathNode2);
      intSet.Add(pathNode2.id);
      for (int index = 0; index < 4; ++index)
      {
        int x = pathNode2.x + (int) PathFindController.Directions[index, 0];
        int y = pathNode2.y + (int) PathFindController.Directions[index, 1];
        int hash = PathNode.ComputeHash(x, y);
        if (!intSet.Contains(hash))
        {
          PathNode p = new PathNode(x, y, pathNode2);
          p.g = (byte) ((uint) pathNode2.g + 1U);
          if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 && p.x < layerWidth && p.y < layerHeight && !PathFindController.isPositionImpassableForNPCSchedule(location, p.x, p.y, npc))
          {
            int priority = (int) p.g + PathFindController.getPreferenceValueForTerrainType(location, p.x, p.y) + (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y) + (p.x == pathNode2.x && p.x == pathNode1.x || p.y == pathNode2.y && p.y == pathNode1.y ? -2 : 0));
            if (!priorityQueue.Contains(p, priority))
              priorityQueue.Enqueue(p, priority);
          }
        }
      }
      pathNode1 = pathNode2;
      ++num;
      if (num >= limit)
        return (Stack<Point>) null;
    }
    return (Stack<Point>) null;
  }

  protected static bool isPositionImpassableForNPCSchedule(
    GameLocation loc,
    int x,
    int y,
    Character npc)
  {
    Tile tile = loc.Map.RequireLayer("Buildings").Tiles[x, y];
    if (tile != null && tile.TileIndex != -1)
    {
      string str;
      if (tile.TileIndexProperties.TryGetValue("Action", out str) || tile.Properties.TryGetValue("Action", out str))
      {
        if (str.StartsWith("LockedDoorWarp") || !str.Contains("Door") && !str.Contains("Passable"))
          return true;
      }
      else if (loc.doesTileHaveProperty(x, y, "Passable", "Buildings") == null && loc.doesTileHaveProperty(x, y, "NPCPassable", "Buildings") == null)
        return true;
    }
    if (loc.doesTileHaveProperty(x, y, "NoPath", "Back") != null)
      return true;
    foreach (Warp warp in (NetList<Warp, NetRef<Warp>>) loc.warps)
    {
      if (warp.X == x && warp.Y == y)
        return true;
    }
    bool? nullable = loc.terrainFeatures.GetValueOrDefault(new Vector2((float) x, (float) y))?.isPassable(npc);
    if (!nullable.HasValue || nullable.GetValueOrDefault())
    {
      nullable = loc.getLargeTerrainFeatureAt(x, y)?.isPassable(npc);
      if (!nullable.HasValue || nullable.GetValueOrDefault())
        return false;
    }
    return true;
  }

  /// <summary>Get the precedence value for a tile position when choosing a path, where lower values are preferred.</summary>
  /// <param name="l">The location to check.</param>
  /// <param name="x">The X tile position.</param>
  /// <param name="y">The Y tile position.</param>
  protected static int getPreferenceValueForTerrainType(GameLocation l, int x, int y)
  {
    switch (l.doesTileHaveProperty(x, y, "Type", "Back")?.ToLower())
    {
      case "stone":
        return -7;
      case "wood":
        return -4;
      case "dirt":
        return -2;
      case "grass":
        return -1;
      default:
        return 0;
    }
  }

  public delegate bool isAtEnd(
    PathNode currentNode,
    Point endPoint,
    GameLocation location,
    Character c);

  public delegate void endBehavior(Character c, GameLocation location);
}
