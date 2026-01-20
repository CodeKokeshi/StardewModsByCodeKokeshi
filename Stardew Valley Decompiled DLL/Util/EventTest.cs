// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.EventTest
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Util;

public class EventTest
{
  private int currentEventIndex;
  private int currentLocationIndex;
  private int aButtonTimer;
  private List<string> specificEventsToDo = new List<string>();
  private bool doingSpecifics;

  public EventTest(string startingLocationName = "", int startingEventIndex = 0)
  {
    this.currentLocationIndex = 0;
    if (startingLocationName.Length > 0)
    {
      for (int index = 0; index < Game1.locations.Count; ++index)
      {
        if (Game1.locations[index].Name.Equals(startingLocationName))
        {
          this.currentLocationIndex = index;
          break;
        }
      }
    }
    this.currentEventIndex = startingEventIndex;
  }

  public EventTest(string[] whichEvents)
  {
    for (int index = 1; index < whichEvents.Length; index += 2)
      this.specificEventsToDo.Add($"{whichEvents[index]} {whichEvents[index + 1]}");
    this.doingSpecifics = true;
    this.currentLocationIndex = -1;
  }

  public void update()
  {
    if (!Game1.eventUp && !Game1.fadeToBlack)
    {
      if (this.currentLocationIndex >= Game1.locations.Count)
        return;
      if (this.doingSpecifics && this.currentLocationIndex == -1)
      {
        if (this.specificEventsToDo.Count == 0)
          return;
        for (int index = 0; index < Game1.locations.Count; ++index)
        {
          string str = this.specificEventsToDo.Last<string>();
          string[] strArray = ArgUtility.SplitBySpace(str);
          if (Game1.locations[index].Name.Equals(strArray[0]))
          {
            this.currentLocationIndex = index;
            int num = -1;
            foreach (KeyValuePair<string, string> keyValuePair in Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + Game1.locations[index].Name))
            {
              ++num;
              int result;
              if (int.TryParse(keyValuePair.Key.Split('/')[0], out result) && result == Convert.ToInt32(strArray[1]))
              {
                this.currentEventIndex = num;
                break;
              }
            }
            this.specificEventsToDo.Remove(str);
            break;
          }
        }
      }
      GameLocation location = Game1.locations[this.currentLocationIndex];
      if (location.currentEvent != null)
        return;
      string locationName = location.name.Value;
      if (locationName == "Pool")
        locationName = "BathHouse_Pool";
      bool flag = true;
      Dictionary<string, string> source = (Dictionary<string, string>) null;
      try
      {
        source = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + locationName);
      }
      catch (Exception ex)
      {
        flag = false;
      }
      if (flag && this.currentEventIndex < source.Count)
      {
        KeyValuePair<string, string> keyValuePair = source.ElementAt<KeyValuePair<string, string>>(this.currentEventIndex);
        string key = keyValuePair.Key;
        string script = keyValuePair.Value;
        if (key.Contains('/') && !script.Equals("null"))
        {
          if (Game1.currentLocation.Name.Equals(locationName))
          {
            Game1.eventUp = true;
            Game1.currentLocation.currentEvent = new Event(script);
          }
          else
          {
            LocationRequest locationRequest = Game1.getLocationRequest(locationName);
            locationRequest.OnLoad += (LocationRequest.Callback) (() => Game1.currentLocation.currentEvent = new Event(script));
            Game1.warpFarmer(locationRequest, 8, 8, Game1.player.FacingDirection);
          }
        }
      }
      ++this.currentEventIndex;
      if (!flag || this.currentEventIndex >= source.Count)
      {
        this.currentEventIndex = 0;
        ++this.currentLocationIndex;
      }
      if (!this.doingSpecifics)
        return;
      this.currentLocationIndex = -1;
    }
    else
    {
      this.aButtonTimer -= (int) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
      if (this.aButtonTimer >= 0)
        return;
      this.aButtonTimer = 100;
      if (!(Game1.activeClickableMenu is DialogueBox activeClickableMenu))
        return;
      activeClickableMenu.performHoverAction(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height - 64 /*0x40*/ - Game1.random.Next(300));
      DialogueBox dialogueBox = activeClickableMenu;
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int x = viewport.Width / 2;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int y = viewport.Height - 64 /*0x40*/ - Game1.random.Next(300);
      dialogueBox.receiveLeftClick(x, y, true);
    }
  }
}
