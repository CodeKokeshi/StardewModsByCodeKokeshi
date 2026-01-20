// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.IMinigame
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley.Minigames;

public interface IMinigame
{
  /// <summary>game tick for minigame</summary>
  /// <returns>true if finished</returns>
  bool tick(GameTime time);

  bool overrideFreeMouseMovement();

  bool doMainGameUpdates();

  void receiveLeftClick(int x, int y, bool playSound = true);

  void leftClickHeld(int x, int y);

  void receiveRightClick(int x, int y, bool playSound = true);

  void releaseLeftClick(int x, int y);

  void releaseRightClick(int x, int y);

  void receiveKeyPress(Keys k);

  void receiveKeyRelease(Keys k);

  void draw(SpriteBatch b);

  void changeScreenSize();

  void unload();

  void receiveEventPoke(int data);

  string minigameId();

  bool forceQuit();
}
