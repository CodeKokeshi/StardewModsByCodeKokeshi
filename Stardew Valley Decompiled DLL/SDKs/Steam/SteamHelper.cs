// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.SteamHelper
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Galaxy.Api;
using StardewValley.Menus;
using StardewValley.SDKs.GogGalaxy.Listeners;
using Steamworks;
using System;

#nullable disable
namespace StardewValley.SDKs.Steam;

public class SteamHelper : SDKHelper
{
  private Callback<GameOverlayActivated_t> gameOverlayActivated;
  private CallResult<EncryptedAppTicketResponse_t> encryptedAppTicketResponse;
  private Callback<GamepadTextInputDismissed_t> gamepadTextInputDismissed;
  private GalaxyAuthListener galaxyAuthListener;
  private GalaxyOperationalStateChangeListener galaxyStateChangeListener;
  private GalaxySpecificUserDataListener galaxySpecificUserDataListener;
  public bool active;
  private SDKNetHelper networking;
  private TextBox _keyboardTextBox;
  protected bool _runningOnSteamDeck;

  public SDKNetHelper Networking => this.networking;

  public bool ConnectionFinished { get; private set; }

  public int ConnectionProgress { get; private set; }

  public bool GalaxyConnected { get; private set; }

  public string Name { get; } = "Steam";

  public void EarlyInitialize()
  {
  }

  public virtual bool IsRunningOnSteamDeck() => this._runningOnSteamDeck;

  public void Initialize()
  {
    try
    {
      this.active = SteamAPI.Init();
      Game1.log.Verbose("Steam logged on: " + SteamUser.BLoggedOn().ToString());
      if (this.active)
      {
        this._runningOnSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
        Game1.log.Verbose("Initializing GalaxySDK");
        this.encryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.onEncryptedAppTicketResponse));
        Game1.log.Verbose("Requesting Steam app ticket");
        this.encryptedAppTicketResponse.Set(SteamUser.RequestEncryptedAppTicket(LegacyShims.EmptyArray<byte>(), 0));
        ++this.ConnectionProgress;
        SteamNetworkingUtils.InitRelayNetworkAccess();
      }
    }
    catch (Exception ex)
    {
      Game1.log.Error("Error connecting to Steam.", ex);
      this.active = false;
      this.ConnectionFinished = true;
    }
    if (!this.active)
      return;
    try
    {
      GalaxyInstance.Init(new InitParams("48767653913349277", "58be5c2e55d7f535cf8c4b6bbc09d185de90b152c8c42703cc13502465f0d04a", "."));
      this.galaxyAuthListener = new GalaxyAuthListener(new Action(this.onGalaxyAuthSuccess), new Action<IAuthListener.FailureReason>(this.onGalaxyAuthFailure), new Action(this.onGalaxyAuthLost));
      this.galaxyStateChangeListener = new GalaxyOperationalStateChangeListener(new Action<uint>(this.onGalaxyStateChange));
    }
    catch (Exception ex)
    {
      Game1.log.Error("Error initializing the Galaxy API.", ex);
    }
    this.gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.onGameOverlayActivated));
    this.gamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(new Callback<GamepadTextInputDismissed_t>.DispatchDelegate(this.OnKeyboardDismissed));
  }

  public void CancelKeyboard() => this._keyboardTextBox = (TextBox) null;

  public void ShowKeyboard(TextBox text_box)
  {
    this._keyboardTextBox = text_box;
    SteamUtils.ShowGamepadTextInput(text_box.PasswordBox ? EGamepadTextInputMode.k_EGamepadTextInputModePassword : EGamepadTextInputMode.k_EGamepadTextInputModeNormal, EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, "", text_box.textLimit < 0 ? 100U : (uint) text_box.textLimit, text_box.Text);
  }

  public void OnKeyboardDismissed(GamepadTextInputDismissed_t callback)
  {
    if (this._keyboardTextBox == null)
      return;
    if (!callback.m_bSubmitted)
    {
      this._keyboardTextBox = (TextBox) null;
    }
    else
    {
      uint gamepadTextLength = SteamUtils.GetEnteredGamepadTextLength();
      string pchText;
      if (!SteamUtils.GetEnteredGamepadTextInput(out pchText, gamepadTextLength))
      {
        this._keyboardTextBox = (TextBox) null;
      }
      else
      {
        this._keyboardTextBox.RecieveTextInput(pchText);
        this._keyboardTextBox = (TextBox) null;
      }
    }
  }

  private void onSetGalaxyProfileName(GalaxyID userID)
  {
    try
    {
      if (userID != GalaxyInstance.User().GetGalaxyID())
        return;
    }
    catch (Exception ex)
    {
      return;
    }
    Game1.log.Verbose("Successfully set GOG Galaxy profile name.");
    this.galaxySpecificUserDataListener?.Dispose();
    this.galaxySpecificUserDataListener = (GalaxySpecificUserDataListener) null;
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
    this.networking = (SDKNetHelper) new SteamNetHelper();
    ++this.ConnectionProgress;
    this.ConnectionFinished = true;
    this.GalaxyConnected = true;
    try
    {
      this.galaxySpecificUserDataListener = new GalaxySpecificUserDataListener(new Action<GalaxyID>(this.onSetGalaxyProfileName));
      GalaxyInstance.User().SetUserData("StardewDisplayName", SteamFriends.GetPersonaName());
    }
    catch (Exception ex)
    {
      Game1.log.Error("Failed to set GOG Galaxy profile name.", ex);
    }
  }

  private void onGalaxyAuthSuccess()
  {
    Game1.log.Verbose("Galaxy auth success");
    ++this.ConnectionProgress;
  }

  private void onGalaxyAuthFailure(IAuthListener.FailureReason reason)
  {
    if (this.networking == null)
      this.networking = (SDKNetHelper) new SteamNetHelper();
    Game1.log.Error("Galaxy auth failure: " + reason.ToString());
    this.ConnectionFinished = true;
    this.GalaxyConnected = false;
  }

  private void onGalaxyAuthLost()
  {
    if (this.networking == null)
      this.networking = (SDKNetHelper) new SteamNetHelper();
    Game1.log.Error("Galaxy auth lost");
    this.ConnectionFinished = true;
    this.GalaxyConnected = false;
  }

  private void onEncryptedAppTicketResponse(EncryptedAppTicketResponse_t response, bool ioFailure)
  {
    if (response.m_eResult == EResult.k_EResultOK)
    {
      byte[] numArray = new byte[1024 /*0x0400*/];
      uint pcbTicket;
      SteamUser.GetEncryptedAppTicket(numArray, 1024 /*0x0400*/, out pcbTicket);
      ++this.ConnectionProgress;
      Game1.log.Verbose("Signing into GalaxySDK");
      try
      {
        GalaxyInstance.User().SignInSteam(numArray, pcbTicket, SteamFriends.GetPersonaName());
      }
      catch (Exception ex)
      {
        Game1.log.Error("Galaxy SignInSteam failed with an exception:", ex);
      }
    }
    else
    {
      Game1.log.Error($"Failed to retrieve encrypted app ticket: {response.m_eResult.ToString()}, {ioFailure.ToString()}");
      this.ConnectionFinished = true;
    }
  }

  private void onGameOverlayActivated(GameOverlayActivated_t pCallback)
  {
    if (!this.active)
      return;
    if (pCallback.m_bActive != (byte) 0)
      Game1.paused = !Game1.IsMultiplayer;
    else
      Game1.paused = false;
  }

  /// <inheritdoc />
  public bool RetroactiveAchievementsAllowed() => true;

  public void GetAchievement(string achieve)
  {
    if (!this.active || !SteamAPI.IsSteamRunning())
      return;
    if (achieve.Equals("0"))
      achieve = "a0";
    try
    {
      SteamUserStats.SetAchievement(achieve);
      SteamUserStats.StoreStats();
    }
    catch (Exception ex)
    {
    }
  }

  public void ResetAchievements()
  {
    if (!this.active)
      return;
    if (!SteamAPI.IsSteamRunning())
      return;
    try
    {
      SteamUserStats.ResetAllStats(true);
    }
    catch (Exception ex)
    {
    }
  }

  public void Update()
  {
    if (this.active)
    {
      SteamAPI.RunCallbacks();
      try
      {
        GalaxyInstance.ProcessData();
      }
      catch (Exception ex)
      {
      }
    }
    Game1.game1.IsMouseVisible = Game1.paused || Game1.options.hardwareCursor;
  }

  public void Shutdown() => SteamAPI.Shutdown();

  public void DebugInfo()
  {
    if (SteamAPI.IsSteamRunning())
    {
      Game1.debugOutput = SteamUser.BLoggedOn() ? "steam is running, user logged on" : "steam is running";
    }
    else
    {
      Game1.debugOutput = "steam is not running";
      SteamAPI.Init();
    }
  }

  public string FilterDirtyWords(string words) => words;

  public bool HasOverlay => false;

  public bool IsJapaneseRegionRelease => false;

  public bool IsEnterButtonAssignmentFlipped => false;
}
