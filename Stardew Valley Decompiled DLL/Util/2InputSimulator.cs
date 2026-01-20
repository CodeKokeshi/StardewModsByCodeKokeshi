// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.IInputSimulator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Util;

public interface IInputSimulator
{
  void SimulateInput(
    ref bool actionButtonPressed,
    ref bool switchToolButtonPressed,
    ref bool useToolButtonPressed,
    ref bool useToolButtonReleased,
    ref bool addItemToInventoryButtonPressed,
    ref bool cancelButtonPressed,
    ref bool moveUpPressed,
    ref bool moveRightPressed,
    ref bool moveLeftPressed,
    ref bool moveDownPressed,
    ref bool moveUpReleased,
    ref bool moveRightReleased,
    ref bool moveLeftReleased,
    ref bool moveDownReleased,
    ref bool moveUpHeld,
    ref bool moveRightHeld,
    ref bool moveLeftHeld,
    ref bool moveDownHeld);
}
