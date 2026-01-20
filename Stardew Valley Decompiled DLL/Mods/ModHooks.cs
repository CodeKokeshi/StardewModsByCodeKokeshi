// Decompiled with JetBrains decompiler
// Type: StardewValley.Mods.ModHooks
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Events;
using StardewValley.Menus;
using System;
using System.Threading.Tasks;
using xTile.Dimensions;

#nullable disable
namespace StardewValley.Mods;

public class ModHooks
{
  public virtual void OnGame1_PerformTenMinuteClockUpdate(Action action) => action();

  public virtual void OnGame1_NewDayAfterFade(Action action) => action();

  public virtual void OnGame1_ShowEndOfNightStuff(Action action) => action();

  public virtual void OnGame1_UpdateControlInput(
    ref KeyboardState keyboardState,
    ref MouseState mouseState,
    ref GamePadState gamePadState,
    Action action)
  {
    action();
  }

  public virtual void OnGameLocation_ResetForPlayerEntry(GameLocation location, Action action)
  {
    action();
  }

  public virtual bool OnGameLocation_CheckAction(
    GameLocation location,
    Location tileLocation,
    xTile.Dimensions.Rectangle viewport,
    Farmer who,
    Func<bool> action)
  {
    return action();
  }

  public virtual FarmEvent OnUtility_PickFarmEvent(Func<FarmEvent> action) => action();

  public virtual void AfterNewDayBarrier(string barrier_id)
  {
  }

  public virtual void CreatedInitialLocations()
  {
  }

  public virtual void SaveAddedLocations()
  {
  }

  public virtual bool OnRendering(
    RenderSteps step,
    SpriteBatch sb,
    GameTime time,
    RenderTarget2D target_screen)
  {
    return true;
  }

  public virtual void OnRendered(
    RenderSteps step,
    SpriteBatch sb,
    GameTime time,
    RenderTarget2D target_screen)
  {
  }

  public virtual bool TryDrawMenu(IClickableMenu menu, Action draw_menu_action)
  {
    if (draw_menu_action != null)
      draw_menu_action();
    return true;
  }

  public virtual Task StartTask(Task task, string id)
  {
    task.Start();
    return task;
  }

  public virtual Task<T> StartTask<T>(Task<T> task, string id)
  {
    task.Start();
    return task;
  }
}
