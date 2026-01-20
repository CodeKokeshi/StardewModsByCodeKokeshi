// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.ChestHit.ChestHitTimer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Network.ChestHit;

public sealed class ChestHitTimer
{
  /// <summary>The amount of milliseconds remaining until a chest must be hit twice to move it.</summary>
  public int Milliseconds;
  /// <summary>The time when this timer was saved in <see cref="F:StardewValley.Network.ChestHit.ChestHitSynchronizer.SavedTimers" />.</summary>
  public int SavedTime = -1;

  /// <summary>Ticks down the timer.</summary>
  /// <param name="gameTime">Provides a snapshot of timing values.</param>
  public void Update(GameTime time)
  {
    if (this.Milliseconds <= 0)
      return;
    this.Milliseconds -= (int) time.ElapsedGameTime.TotalMilliseconds;
  }
}
