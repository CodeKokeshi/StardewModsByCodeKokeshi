// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.GogGalaxy.GalaxyHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.SDKs.GogGalaxy.Listeners;
using System;

#nullable disable
namespace StardewValley.SDKs.GogGalaxy;

public class GalaxyHelper : SDKHelper
{
  public const string ClientID = "48767653913349277";
  public const string ClientSecret = "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a";
  /// <summary>The key we use to store the user's custom display name on the Galaxy API.</summary>
  public const string DisplayNameDataKey = "StardewDisplayName";
  public bool active;
  private GalaxyAuthListener authListener;
  private GalaxyOperationalStateChangeListener stateChangeListener;
  private GalaxyNetHelper networking;

  public string Name { get; } = "Galaxy";

  public bool ConnectionFinished { get; private set; }

  public int ConnectionProgress { get; private set; }

  public SDKNetHelper Networking => (SDKNetHelper) this.networking;

  public bool HasOverlay => false;

  public void EarlyInitialize()
  {
  }

  public void Initialize()
  {
    try
    {
      GalaxyInstance.Init(new InitParams("48767653913349277", "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a"));
      this.authListener = new GalaxyAuthListener(new Action(this.onGalaxyAuthSuccess), new Action<IAuthListener.FailureReason>(this.onGalaxyAuthFailure), new Action(this.onGalaxyAuthLost));
      this.stateChangeListener = new GalaxyOperationalStateChangeListener(new Action<uint>(this.onGalaxyStateChange));
      GalaxyInstance.User().SignInGalaxy(true);
      this.active = true;
      ++this.ConnectionProgress;
    }
    catch (Exception ex)
    {
      Game1.log.Error("Error initializing GalaxyHelper.", ex);
      this.ConnectionFinished = true;
    }
  }

  private void onGalaxyStateChange(uint operationalState)
  {
    if (this.networking != null)
      return;
    if (((int) operationalState & 1) != 0)
    {
      Game1.log.Verbose("Galaxy signed in");
      ++this.ConnectionProgress;
    }
    if (((int) operationalState & 2) == 0)
      return;
    Game1.log.Verbose("Galaxy logged on");
    this.networking = new GalaxyNetHelper();
    ++this.ConnectionProgress;
    this.ConnectionFinished = true;
  }

  private void onGalaxyAuthSuccess()
  {
    Game1.log.Verbose("Galaxy auth success");
    ++this.ConnectionProgress;
  }

  private void onGalaxyAuthFailure(IAuthListener.FailureReason reason)
  {
    Game1.log.Error("Galaxy auth failure: " + reason.ToString());
    this.ConnectionFinished = true;
  }

  private void onGalaxyAuthLost()
  {
    Game1.log.Error("Galaxy auth lost");
    this.ConnectionFinished = true;
  }

  /// <inheritdoc />
  public bool RetroactiveAchievementsAllowed() => true;

  public void GetAchievement(string achieve)
  {
  }

  public void ResetAchievements()
  {
    if (!this.active)
      return;
    GalaxyInstance.Stats().ResetStatsAndAchievements();
  }

  public void Update()
  {
    if (!this.active)
      return;
    GalaxyInstance.ProcessData();
  }

  public void Shutdown()
  {
  }

  public void DebugInfo()
  {
  }

  public string FilterDirtyWords(string words) => words;

  public bool IsJapaneseRegionRelease => false;

  public bool IsEnterButtonAssignmentFlipped => false;
}
