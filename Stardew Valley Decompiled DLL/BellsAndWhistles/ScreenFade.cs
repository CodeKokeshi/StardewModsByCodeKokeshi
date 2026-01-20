// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.ScreenFade
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace StardewValley.BellsAndWhistles;

public class ScreenFade
{
  public bool globalFade;
  public bool fadeIn = true;
  public bool fadeToBlack;
  public bool nonWarpFade;
  public float fadeToBlackAlpha;
  public float globalFadeSpeed;
  private const float fadeToFudge = 0.1f;
  private Game1.afterFadeFunction afterFade;
  private Func<bool> onFadeToBlackComplete;
  private Action onFadedBackInComplete;

  public ScreenFade(Func<bool> onFadeToBlack, Action onFadeIn)
  {
    this.onFadeToBlackComplete = onFadeToBlack;
    this.onFadedBackInComplete = onFadeIn;
  }

  public bool UpdateFade(GameTime time)
  {
    if (this.fadeToBlack && ((double) Game1.pauseTime == 0.0 || Game1.eventUp))
    {
      if ((double) this.fadeToBlackAlpha > 1.1000000238418579 && !Game1.messagePause)
      {
        this.fadeToBlackAlpha = 1f;
        if (this.onFadeToBlackComplete())
          return true;
        this.nonWarpFade = false;
        this.fadeIn = false;
        if (this.afterFade != null)
        {
          Game1.afterFadeFunction afterFade = this.afterFade;
          this.afterFade = (Game1.afterFadeFunction) null;
          afterFade();
        }
        this.globalFade = false;
      }
      if ((double) this.fadeToBlackAlpha < -0.10000000149011612)
      {
        this.fadeToBlackAlpha = 0.0f;
        this.fadeToBlack = false;
        this.onFadedBackInComplete();
      }
      this.UpdateFadeAlpha(time);
    }
    return false;
  }

  public void UpdateFadeAlpha(GameTime time)
  {
    if (this.fadeIn)
    {
      this.fadeToBlackAlpha += (Game1.eventUp || Game1.farmEvent != null ? 0.0008f : 0.0019f) * (float) time.ElapsedGameTime.Milliseconds;
      this.fadeToBlackAlpha = Game1.IsDedicatedHost ? 1.2f : this.fadeToBlackAlpha;
    }
    else
    {
      if (Game1.messagePause || Game1.dialogueUp)
        return;
      this.fadeToBlackAlpha -= (Game1.eventUp || Game1.farmEvent != null ? 0.0008f : 0.0019f) * (float) time.ElapsedGameTime.Milliseconds;
      this.fadeToBlackAlpha = Game1.IsDedicatedHost ? -0.2f : this.fadeToBlackAlpha;
    }
  }

  public void FadeScreenToBlack(float startAlpha = 0.0f, bool stopMovement = true)
  {
    this.globalFade = false;
    this.fadeToBlack = true;
    this.fadeIn = true;
    this.fadeToBlackAlpha = startAlpha;
    if (!stopMovement)
      return;
    Game1.player.CanMove = false;
  }

  public void FadeClear(float startAlpha = 1f)
  {
    this.globalFade = false;
    this.fadeIn = false;
    this.fadeToBlack = true;
    this.fadeToBlackAlpha = startAlpha;
  }

  public void GlobalFadeToBlack(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f)
  {
    if (this.fadeToBlack && !this.fadeIn)
      this.onFadedBackInComplete();
    this.fadeToBlack = false;
    this.globalFade = true;
    this.fadeIn = false;
    this.afterFade = afterFade;
    this.globalFadeSpeed = fadeSpeed;
    this.fadeToBlackAlpha = 0.0f;
  }

  public void GlobalFadeToClear(Game1.afterFadeFunction afterFade = null, float fadeSpeed = 0.02f)
  {
    if (this.fadeToBlack && this.fadeIn)
    {
      int num = this.onFadeToBlackComplete() ? 1 : 0;
    }
    this.fadeToBlack = false;
    this.globalFade = true;
    this.fadeIn = true;
    this.afterFade = afterFade;
    this.globalFadeSpeed = fadeSpeed;
    this.fadeToBlackAlpha = 1f;
  }

  public void UpdateGlobalFade()
  {
    if (this.fadeIn)
    {
      if ((double) this.fadeToBlackAlpha <= 0.0)
      {
        this.globalFade = false;
        if (this.afterFade != null)
        {
          Game1.afterFadeFunction afterFade = this.afterFade;
          this.afterFade();
          if (this.afterFade != null && this.afterFade.Equals((object) afterFade))
            this.afterFade = (Game1.afterFadeFunction) null;
          if (Game1.nonWarpFade)
            this.fadeToBlack = false;
        }
      }
      this.fadeToBlackAlpha = Game1.IsDedicatedHost ? 0.0f : Math.Max(0.0f, this.fadeToBlackAlpha - this.globalFadeSpeed);
    }
    else
    {
      if ((double) this.fadeToBlackAlpha >= 1.0)
      {
        this.globalFade = false;
        if (this.afterFade != null)
        {
          Game1.afterFadeFunction afterFade = this.afterFade;
          this.afterFade();
          if (this.afterFade != null && this.afterFade.Equals((object) afterFade))
            this.afterFade = (Game1.afterFadeFunction) null;
          if (Game1.nonWarpFade)
            this.fadeToBlack = false;
        }
      }
      this.fadeToBlackAlpha = Game1.IsDedicatedHost ? 1f : Math.Min(1f, this.fadeToBlackAlpha + this.globalFadeSpeed);
    }
  }
}
