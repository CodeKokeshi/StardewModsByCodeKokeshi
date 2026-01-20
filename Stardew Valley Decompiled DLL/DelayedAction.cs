// Decompiled with JetBrains decompiler
// Type: StardewValley.DelayedAction
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace StardewValley;

/// <summary>An action that will be performed after a delay.</summary>
public class DelayedAction
{
  /// <summary>The number of milliseconds until the action is invoked.</summary>
  public int timeUntilAction;
  /// <summary>An arbitrary int value saved for the action, if applicable.</summary>
  public int intData;
  /// <summary>An arbitrary float value saved for the action, if applicable.</summary>
  public float floatData;
  /// <summary>An arbitrary string value saved for the action, if applicable.</summary>
  public string stringData;
  /// <summary>An arbitrary point value saved for the action, if applicable.</summary>
  public Point pointData;
  /// <summary>An arbitrary NPC value saved for the action, if applicable.</summary>
  public NPC character;
  /// <summary>An arbitrary location value saved for the action, if applicable.</summary>
  public GameLocation location;
  /// <summary>The action to invoke.</summary>
  public Action behavior;
  /// <summary>The action to invoke after the screen is fully faded to black, if applicable.</summary>
  public Game1.afterFadeFunction afterFadeBehavior;
  /// <summary>Whether to only decrement the delay timer when there's no open menu.</summary>
  public bool waitUntilMenusGone;
  /// <summary>An arbitrary temporary animated sprite saved for the action, if applicable.</summary>
  public TemporaryAnimatedSprite temporarySpriteData;

  /// <summary>Construct an instance.</summary>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public DelayedAction(int delay) => this.timeUntilAction = delay;

  /// <summary>Construct an instance.</summary>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="behavior">The action to invoke.</param>
  public DelayedAction(int delay, Action behavior)
  {
    this.timeUntilAction = delay;
    this.behavior = behavior;
  }

  /// <summary>Decrement the timer and invoke the action, if applicable in the current context.</summary>
  /// <param name="time">The current game time.</param>
  public bool update(GameTime time)
  {
    if (!this.waitUntilMenusGone || Game1.activeClickableMenu == null)
    {
      this.timeUntilAction -= time.ElapsedGameTime.Milliseconds;
      if (this.timeUntilAction <= 0)
        this.behavior();
    }
    return this.timeUntilAction <= 0;
  }

  /// <summary>Warp the local player to another location after a delay.</summary>
  /// <param name="targetLocation">The internal name of the target location.</param>
  /// <param name="targetTile">The target tile position.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void warpAfterDelay(string targetLocation, Point targetTile, int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyWarp);
    delayedAction.stringData = targetLocation;
    delayedAction.pointData = targetTile;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Add a temporary animated sprite to the current location after a delay.</summary>
  /// <param name="sprite">The temporary sprite to add.</param>
  /// <param name="location">The location to which to add the sprite.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="waitUntilMenusGone">Whether to only decrement the delay timer when there's no open menu.</param>
  public static void addTemporarySpriteAfterDelay(
    TemporaryAnimatedSprite sprite,
    GameLocation location,
    int delay,
    bool waitUntilMenusGone = false)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyTempSprite);
    delayedAction.temporarySpriteData = sprite;
    delayedAction.location = location;
    delayedAction.waitUntilMenusGone = waitUntilMenusGone;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Play a sound after a delay, either for all player in the location (if <paramref name="location" /> is specified) else for the current player.</summary>
  /// <param name="soundName">The cue ID for the sound to play.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="location">The location in which to play the sound. If specified, it's played for all players in the location; otherwise it's only played for the current player.</param>
  /// <param name="position">The tile position from which to play the sound, or <c>null</c> if it should be played throughout the location.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
  /// <param name="local">Whether the sound should only be played for the local player. Ignored if <paramref name="location" /> is null.</param>
  public static void playSoundAfterDelay(
    string soundName,
    int delay,
    GameLocation location = null,
    Vector2? position = null,
    int pitch = -1,
    bool local = false)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = !local ? new Action(delayedAction.ApplySound) : new Action(delayedAction.ApplySoundLocal);
    delayedAction.stringData = soundName;
    delayedAction.location = location;
    delayedAction.intData = pitch;
    if (position.HasValue)
      delayedAction.pointData = Utility.Vector2ToPoint(position.Value);
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Remove a temporary animated sprite from the current location after a delay.</summary>
  /// <param name="location">The location from which to remove the sprite.</param>
  /// <param name="idOfTempSprite">The ID of the temporary sprite to remove.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void removeTemporarySpriteAfterDelay(
    GameLocation location,
    int idOfTempSprite,
    int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyRemoveTemporarySprite);
    delayedAction.location = location;
    delayedAction.intData = idOfTempSprite;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Start a music track after a delay.</summary>
  /// <param name="musicName">The cue ID for the music to play.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="interruptable">Whether the music can be overridden by a jukebox.</param>
  public static DelayedAction playMusicAfterDelay(string musicName, int delay, bool interruptable = true)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyMusicTrack);
    delayedAction.stringData = musicName;
    delayedAction.intData = interruptable ? 1 : 0;
    Game1.delayedActions.Add(delayedAction);
    return delayedAction;
  }

  /// <summary>Show text in a speech bubble over an NPC's head after a delay.</summary>
  /// <param name="text">The literal text to display.</param>
  /// <param name="who">The NPC over which to show the text.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void textAboveHeadAfterDelay(string text, NPC who, int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyTextAboveHead);
    delayedAction.stringData = text;
    delayedAction.character = who;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Disable a glowing effect applied to the current player after a delay.</summary>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void stopFarmerGlowing(int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyStopGlowing);
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Show a generic dialogue message without an NPC speaker after a delay.</summary>
  /// <param name="dialogue">The dialogue text to show.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void showDialogueAfterDelay(string dialogue, int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyDialogue);
    delayedAction.stringData = dialogue;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Show a screen flash after a delay.</summary>
  /// <param name="intensity">The intensity of the flash, as a value between 0 (transparent) and 1 (fully opaque).</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="sound">The cue ID for the sound to play.</param>
  public static void screenFlashAfterDelay(float intensity, int delay, string sound = null)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyScreenFlash);
    delayedAction.stringData = sound;
    delayedAction.floatData = intensity;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Remove a tile from a location's map after a delay.</summary>
  /// <param name="x">The tile's X tile position.</param>
  /// <param name="y">The tile's Y tile position.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  /// <param name="location">The location from whose map to remove the tile.</param>
  /// <param name="whichLayer">The map layer ID from which to remove the tile.</param>
  public static void removeTileAfterDelay(
    int x,
    int y,
    int delay,
    GameLocation location,
    string whichLayer)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyRemoveMapTile);
    delayedAction.pointData = new Point(x, y);
    delayedAction.location = location;
    delayedAction.stringData = whichLayer;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Fade the screen to black after a delay.</summary>
  /// <param name="behaviorAfterFade">The action to invoke after the screen is fully faded to black.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static void fadeAfterDelay(Game1.afterFadeFunction behaviorAfterFade, int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = new Action(delayedAction.ApplyFade);
    delayedAction.afterFadeBehavior = behaviorAfterFade;
    Game1.delayedActions.Add(delayedAction);
  }

  /// <summary>Invoke a callback after a delay.</summary>
  /// <param name="func">The action to invoke.</param>
  /// <param name="delay">The number of milliseconds until the action is invoked.</param>
  public static DelayedAction functionAfterDelay(Action func, int delay)
  {
    DelayedAction delayedAction = new DelayedAction(delay);
    delayedAction.behavior = func;
    Game1.delayedActions.Add(delayedAction);
    return delayedAction;
  }

  /// <summary>Apply the delayed screen fade to black.</summary>
  private void ApplyFade() => Game1.globalFadeToBlack(this.afterFadeBehavior);

  /// <summary>Apply the delayed text over an NPC's head.</summary>
  private void ApplyTextAboveHead()
  {
    string stringData = this.stringData;
    if (stringData == null)
      return;
    this.character?.showTextAboveHead(stringData);
  }

  /// <summary>Apply the delayed temporary animated sprite addition.</summary>
  private void ApplyTempSprite()
  {
    if (this.temporarySpriteData == null)
      return;
    this.location?.TemporarySprites.Add(this.temporarySpriteData);
  }

  /// <summary>Apply the delayed player glow disable.</summary>
  private void ApplyStopGlowing()
  {
    Game1.player.stopGlowing();
    Game1.player.stopJittering();
    Game1.screenGlowHold = false;
    if (!Game1.isFestival() || !Game1.IsFall)
      return;
    Game1.changeMusicTrack("fallFest");
  }

  /// <summary>Apply the delayed dialogue without NPC.</summary>
  private void ApplyDialogue() => Game1.drawObjectDialogue(this.stringData);

  /// <summary>Apply the delayed warp action.</summary>
  private void ApplyWarp()
  {
    string stringData = this.stringData;
    Point pointData = this.pointData;
    if (stringData == null)
      return;
    Game1.warpFarmer(stringData, pointData.X, pointData.Y, false);
  }

  /// <summary>Apply the delayed map tile removal.</summary>
  private void ApplyRemoveMapTile()
  {
    string stringData = this.stringData;
    Point pointData = this.pointData;
    if (stringData == null)
      return;
    this.location?.removeTile(pointData.X, pointData.Y, stringData);
  }

  /// <summary>Apply the delayed temporary animated sprite removal.</summary>
  private void ApplyRemoveTemporarySprite()
  {
    this.location?.removeTemporarySpritesWithID(this.intData);
  }

  /// <summary>Helper to apply the delayed sound action.</summary>
  private void ApplySoundHelper(bool local)
  {
    string stringData = this.stringData;
    int? pitch = this.intData > -1 ? new int?(this.intData) : new int?();
    Vector2? position = this.pointData != Point.Zero ? new Vector2?(Utility.PointToVector2(this.pointData)) : new Vector2?();
    if (stringData == null)
      return;
    if (this.location == null)
      Game1.playSound(stringData, pitch);
    else if (local)
      this.location.localSound(stringData, position, pitch);
    else
      this.location.playSound(stringData, position, pitch);
  }

  /// <summary>Apply the delayed sound action.</summary>
  private void ApplySound() => this.ApplySoundHelper(false);

  /// <summary>Apply the delayed sound action for the local player.</summary>
  private void ApplySoundLocal() => this.ApplySoundHelper(true);

  /// <summary>Apply the delayed music action.</summary>
  private void ApplyMusicTrack()
  {
    string stringData = this.stringData;
    bool track_interruptable = this.intData > 0;
    if (stringData == null)
      return;
    Game1.changeMusicTrack(stringData, track_interruptable);
  }

  /// <summary>Apply the delayed screen flash.</summary>
  private void ApplyScreenFlash()
  {
    double floatData = (double) this.floatData;
    string stringData = this.stringData;
    if (!string.IsNullOrEmpty(stringData))
      Game1.playSound(stringData);
    Game1.flashAlpha = (float) floatData;
  }
}
