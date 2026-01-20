// Decompiled with JetBrains decompiler
// Type: StardewValley.DebugTimings
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Diagnostics;

#nullable disable
namespace StardewValley;

public class DebugTimings
{
  private static readonly Vector2 DrawPos = Vector2.One * 12f;
  private readonly Stopwatch StopwatchDraw = new Stopwatch();
  private readonly Stopwatch StopwatchUpdate = new Stopwatch();
  private double LastTimingDraw;
  private double LastTimingUpdate;
  private float DrawTextWidth = -1f;
  private bool Active;

  public bool Toggle()
  {
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      return false;
    this.Active = !this.Active;
    return this.Active;
  }

  public void StartDrawTimer()
  {
    if (!this.Active)
      return;
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      return;
    this.StopwatchDraw.Restart();
  }

  public void StopDrawTimer()
  {
    if (!this.Active)
      return;
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      return;
    this.StopwatchDraw.Stop();
    this.LastTimingDraw = this.StopwatchDraw.Elapsed.TotalMilliseconds;
  }

  public void StartUpdateTimer()
  {
    if (!this.Active)
      return;
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      return;
    this.StopwatchUpdate.Restart();
  }

  public void StopUpdateTimer()
  {
    if (!this.Active)
      return;
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault())
      return;
    this.StopwatchUpdate.Stop();
    this.LastTimingUpdate = this.StopwatchUpdate.Elapsed.TotalMilliseconds;
  }

  public void Draw()
  {
    if (!this.Active)
      return;
    bool? isMainInstance = Game1.game1?.IsMainInstance;
    if (!isMainInstance.HasValue || !isMainInstance.GetValueOrDefault() || Game1.spriteBatch == null || Game1.dialogueFont == null)
      return;
    if ((double) this.DrawTextWidth <= 0.0)
      this.DrawTextWidth = Game1.dialogueFont.MeasureString($"Draw time: {0:00.00} ms  ").X;
    Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, 64 /*0x40*/), Color.Black * 0.5f);
    Game1.spriteBatch.DrawString(Game1.dialogueFont, $"Draw time: {this.LastTimingDraw:00.00} ms  ", DebugTimings.DrawPos, Color.White);
    Game1.spriteBatch.DrawString(Game1.dialogueFont, $"Update time: {this.LastTimingUpdate:00.00} ms", new Vector2(DebugTimings.DrawPos.X + this.DrawTextWidth, DebugTimings.DrawPos.Y), Color.White);
  }
}
