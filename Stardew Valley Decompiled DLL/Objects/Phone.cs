// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Phone
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Objects;

[InstanceStatics]
public class Phone : StardewValley.Object
{
  /// <summary>The methods which handle incoming phone calls.</summary>
  public static List<IPhoneHandler> PhoneHandlers = new List<IPhoneHandler>()
  {
    (IPhoneHandler) new DefaultPhoneHandler()
  };
  /// <summary>While the phone is ringing, how long each ring sound should last in milliseconds.</summary>
  public const int RING_DURATION = 600;
  /// <summary>While the phone is ringing, the delay between each ring sound in milliseconds.</summary>
  public const int RING_CYCLE_TIME = 1800;
  public static Random r;
  protected static bool _phoneSoundPlaying = false;
  public static int ringingTimer;
  public static string whichPhoneCall = (string) null;
  public static long lastRunTick = -1;
  public static long lastMinutesElapsedTick = -1;
  public static int intervalsToRing = 0;

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  public Phone()
  {
  }

  public Phone(Vector2 position)
    : base(position, "214")
  {
    this.Name = "Telephone";
    this.type.Value = "Crafting";
    this.bigCraftable.Value = true;
    this.canBeSetDown.Value = true;
  }

  /// <inheritdoc />
  public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
  {
    if (justCheckingForActivity)
      return true;
    string whichPhoneCall = Phone.whichPhoneCall;
    Phone.StopRinging();
    if (whichPhoneCall == null)
      Game1.game1.ShowTelephoneMenu();
    else if (!Phone.HandleIncomingCall(whichPhoneCall))
      Phone.HangUp();
    return true;
  }

  /// <summary>Handle an incoming phone call when the player interacts with the phone, if applicable.</summary>
  /// <param name="callId">The unique ID for the incoming call.</param>
  /// <remarks>For custom calls, add a new handler to <see cref="F:StardewValley.Objects.Phone.PhoneHandlers" /> instead.</remarks>
  public static bool HandleIncomingCall(string callId)
  {
    Action incomingCallAction = Phone.GetIncomingCallAction(callId);
    if (incomingCallAction == null)
      return false;
    Game1.playSound("openBox");
    Game1.player.freezePause = 500;
    DelayedAction.functionAfterDelay(incomingCallAction, 500);
    int num;
    if (!Game1.player.callsReceived.TryGetValue(callId, out num))
      num = 0;
    Game1.player.callsReceived[callId] = num + 1;
    return true;
  }

  public override void updateWhenCurrentLocation(GameTime time)
  {
    if (this.Location != Game1.currentLocation)
      return;
    if ((long) Game1.ticks != Phone.lastRunTick)
    {
      if (Game1.eventUp)
        return;
      Phone.lastRunTick = (long) Game1.ticks;
      if (Phone.whichPhoneCall != null && Game1.shouldTimePass())
      {
        if (Phone.ringingTimer == 0)
        {
          Game1.playSound("phone");
          Phone._phoneSoundPlaying = true;
        }
        Phone.ringingTimer += (int) time.ElapsedGameTime.TotalMilliseconds;
        if (Phone.ringingTimer >= 1800)
        {
          Phone.ringingTimer = 0;
          Phone._phoneSoundPlaying = false;
        }
      }
    }
    base.updateWhenCurrentLocation(time);
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    Phone.r = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
    Phone._phoneSoundPlaying = false;
    Phone.ringingTimer = 0;
    Phone.whichPhoneCall = (string) null;
    Phone.intervalsToRing = 0;
  }

  /// <inheritdoc />
  public override bool minutesElapsed(int minutes)
  {
    if (!Game1.IsMasterGame)
      return false;
    if (Phone.lastMinutesElapsedTick != (long) Game1.ticks)
    {
      Phone.lastMinutesElapsedTick = (long) Game1.ticks;
      if (Phone.intervalsToRing == 0)
      {
        if (Phone.r == null)
          Phone.r = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.stats.DaysPlayed);
        foreach (IPhoneHandler phoneHandler in Phone.PhoneHandlers)
        {
          string str = phoneHandler.CheckForIncomingCall(Phone.r);
          if (str != null)
          {
            Phone.intervalsToRing = 3;
            Game1.player.team.ringPhoneEvent.Fire(str);
            break;
          }
        }
      }
      else
      {
        --Phone.intervalsToRing;
        if (Phone.intervalsToRing <= 0)
          Game1.player.team.ringPhoneEvent.Fire((string) null);
      }
    }
    return base.minutesElapsed(minutes);
  }

  /// <summary>Get whether the phone is currently ringing.</summary>
  public static bool IsRinging() => Phone._phoneSoundPlaying;

  /// <summary>Start ringing the phone for an incoming call.</summary>
  /// <param name="callId">The unique ID for the incoming call.</param>
  public static void Ring(string callId)
  {
    if (string.IsNullOrWhiteSpace(callId))
    {
      Phone.StopRinging();
    }
    else
    {
      if (Phone.GetIncomingCallAction(callId) == null)
        return;
      Phone.whichPhoneCall = callId;
      Phone.ringingTimer = 0;
      Phone._phoneSoundPlaying = false;
    }
  }

  /// <summary>Stop ringing the phone and discard the incoming call, if any.</summary>
  public static void StopRinging()
  {
    Phone.whichPhoneCall = (string) null;
    Phone.ringingTimer = 0;
    Phone.intervalsToRing = 0;
    if (!Phone.IsRinging())
      return;
    Game1.soundBank.GetCue("phone").Stop(AudioStopOptions.Immediate);
    Phone._phoneSoundPlaying = false;
  }

  /// <summary>Hang up the phone.</summary>
  public static void HangUp()
  {
    Phone.StopRinging();
    Game1.currentLocation.playSound("openBox");
  }

  /// <summary>Get the action to call when the player answers the phone, if the call ID is valid.</summary>
  /// <param name="callId">The unique ID for the incoming call.</param>
  public static Action GetIncomingCallAction(string callId)
  {
    foreach (IPhoneHandler phoneHandler in Phone.PhoneHandlers)
    {
      Action showDialogue;
      if (phoneHandler.TryHandleIncomingCall(callId, out showDialogue))
        return showDialogue;
    }
    return (Action) null;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    if (this.isTemporarilyInvisible)
      return;
    base.draw(spriteBatch, x, y, alpha);
    bool flag = Phone.ringingTimer > 0 && Phone.ringingTimer < 600;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
    Rectangle destinationRectangle = new Rectangle((int) local.X + (flag || this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) local.Y + (flag || this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), 64 /*0x40*/, 128 /*0x80*/);
    float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 /*0x40*/ - 20) / 10000f) + (float) x * 1E-05f;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    spriteBatch.Draw(dataOrErrorItem.GetTexture(), destinationRectangle, new Rectangle?(dataOrErrorItem.GetSourceRect(1, new int?(this.ParentSheetIndex))), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
  }
}
