// Decompiled with JetBrains decompiler
// Type: StardewValley.NPCController
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class NPCController
{
  public Character puppet;
  private bool loop;
  private bool destroyAtNextTurn;
  private List<Vector2> path;
  private Vector2 target;
  private int pathIndex;
  private int pauseTime = -1;
  private int speed;
  private NPCController.endBehavior behaviorAtEnd;

  private int CurrentPathX
  {
    get => this.pathIndex >= this.path.Count ? 0 : (int) this.path[this.pathIndex].X;
  }

  private int CurrentPathY
  {
    get => this.pathIndex >= this.path.Count ? 0 : (int) this.path[this.pathIndex].Y;
  }

  private bool MovingHorizontally => this.CurrentPathX != 0;

  public NPCController(
    Character n,
    List<Vector2> path,
    bool loop,
    NPCController.endBehavior endBehavior = null)
  {
    if (n == null)
      return;
    this.speed = n.speed;
    this.loop = loop;
    this.puppet = n;
    this.path = path;
    this.setMoving(true);
    this.behaviorAtEnd = endBehavior;
  }

  public void destroyAtNextCrossroad() => this.destroyAtNextTurn = true;

  private bool setMoving(bool newTarget)
  {
    if (this.puppet == null || this.pathIndex >= this.path.Count)
      return false;
    int direction = 2;
    if (this.CurrentPathX > 0)
      direction = 1;
    else if (this.CurrentPathX < 0)
      direction = 3;
    else if (this.CurrentPathY < 0)
      direction = 0;
    else if (this.CurrentPathY > 0)
      direction = 2;
    this.puppet.Halt();
    this.puppet.faceDirection(direction);
    if (this.CurrentPathX != 0 && this.CurrentPathY != 0)
    {
      this.pauseTime = this.CurrentPathY;
      this.puppet.faceDirection(this.CurrentPathX % 4);
      return true;
    }
    this.puppet.setMovingInFacingDirection();
    if (newTarget)
      this.target = new Vector2(this.puppet.Position.X + (float) (this.CurrentPathX * 64 /*0x40*/), this.puppet.Position.Y + (float) (this.CurrentPathY * 64 /*0x40*/));
    return true;
  }

  public bool update(GameTime time, GameLocation location, List<NPCController> allControllers)
  {
    this.puppet.speed = this.speed;
    bool flag = false;
    foreach (NPCController allController in allControllers)
    {
      if (allController.puppet != null)
      {
        if (allController.puppet.Equals((object) this.puppet))
          flag = true;
        if (allController.puppet.FacingDirection == this.puppet.FacingDirection && !allController.puppet.Equals((object) this.puppet) && allController.puppet.GetBoundingBox().Intersects(this.puppet.nextPosition(this.puppet.FacingDirection)))
        {
          if (!flag)
            return false;
          break;
        }
      }
    }
    if (this.puppet is Farmer puppet)
    {
      puppet.setRunning(false, true);
      puppet.speed = 2;
      puppet.ignoreCollisions = true;
      if (Game1.CurrentEvent != null && Game1.CurrentEvent.farmer != this.puppet)
        puppet.updateMovementAnimation(time);
    }
    this.puppet.MovePosition(time, Game1.viewport, location);
    if (this.pauseTime < 0 && !this.puppet.isMoving())
      this.setMoving(false);
    if (this.pauseTime < 0 && (double) Math.Abs(Vector2.Distance(this.puppet.Position, this.target)) <= (double) this.puppet.Speed)
    {
      ++this.pathIndex;
      if (this.destroyAtNextTurn)
        return true;
      if (!this.setMoving(true))
      {
        if (this.loop)
        {
          this.pathIndex = 0;
          this.setMoving(true);
        }
        else if (Game1.currentMinigame == null)
        {
          NPCController.endBehavior behaviorAtEnd = this.behaviorAtEnd;
          if (behaviorAtEnd != null)
            behaviorAtEnd();
          return true;
        }
      }
    }
    else if (this.pauseTime >= 0)
    {
      this.pauseTime -= time.ElapsedGameTime.Milliseconds;
      if (this.pauseTime < 0)
      {
        ++this.pathIndex;
        if (this.destroyAtNextTurn)
          return true;
        if (!this.setMoving(true))
        {
          if (this.loop)
          {
            this.pathIndex = 0;
            this.setMoving(true);
          }
          else if (Game1.currentMinigame == null)
          {
            NPCController.endBehavior behaviorAtEnd = this.behaviorAtEnd;
            if (behaviorAtEnd != null)
              behaviorAtEnd();
            return true;
          }
        }
      }
    }
    return false;
  }

  public delegate void endBehavior();
}
