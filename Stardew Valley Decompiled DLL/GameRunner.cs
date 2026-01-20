// Decompiled with JetBrains decompiler
// Type: StardewValley.GameRunner
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

#nullable enable
namespace StardewValley;

public class GameRunner : Game
{
  public static 
  #nullable disable
  GameRunner instance;
  public List<Game1> gameInstances = new List<Game1>();
  public List<Game1> gameInstancesToRemove = new List<Game1>();
  public Game1 gamePtr;
  public bool shouldLoadContent;
  protected bool _initialized;
  protected bool _windowSizeChanged;
  public List<int> startButtonState = new List<int>();
  public List<KeyValuePair<Game1, IEnumerator<int>>> activeNewDayProcesses = new List<KeyValuePair<Game1, IEnumerator<int>>>();
  public int nextInstanceId;
  public static int MaxTextureSize = 4096 /*0x1000*/;

  public GameRunner()
  {
    Program.sdk.EarlyInitialize();
    if (!Program.releaseBuild)
      this.InactiveSleepTime = new TimeSpan(0L);
    Game1.graphics = new GraphicsDeviceManager((Game) this);
    Game1.graphics.PreparingDeviceSettings += (EventHandler<PreparingDeviceSettingsEventArgs>) ((sender, args) => args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents);
    Game1.graphics.PreferredBackBufferWidth = 1280 /*0x0500*/;
    Game1.graphics.PreferredBackBufferHeight = 720;
    this.Content.RootDirectory = "Content";
    SpriteBatch.TextureTuckAmount = 1f / 1000f;
    LocalMultiplayer.Initialize();
    ItemRegistry.RegisterItemTypes();
    GameRunner.MaxTextureSize = int.MaxValue;
    this.Window.AllowUserResizing = true;
    this.SubscribeClientSizeChange();
    this.Exiting += (EventHandler<EventArgs>) ((sender, args) =>
    {
      this.ExecuteForInstances((Action<Game1>) (instance => instance.exitEvent(sender, args)));
      Process.GetCurrentProcess().Kill();
    });
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    LocalizedContentManager.OnLanguageChange += (LocalizedContentManager.LanguageChangedHandler) (code => this.ExecuteForInstances((Action<Game1>) (instance => instance.TranslateFields())));
    DebugTools.GameConstructed((Game) this);
  }

  protected override void OnActivated(object sender, EventArgs args)
  {
    this.ExecuteForInstances((Action<Game1>) (instance => instance.Instance_OnActivated(sender, args)));
  }

  public void SubscribeClientSizeChange()
  {
    this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.OnWindowSizeChange);
  }

  public void OnWindowSizeChange(object sender, EventArgs args)
  {
    this.Window.ClientSizeChanged -= new EventHandler<EventArgs>(this.OnWindowSizeChange);
    this._windowSizeChanged = true;
  }

  protected override void Draw(GameTime gameTime)
  {
    if (this._windowSizeChanged)
    {
      this.ExecuteForInstances((Action<Game1>) (instance => instance.Window_ClientSizeChanged((object) null, (EventArgs) null)));
      this._windowSizeChanged = false;
      this.SubscribeClientSizeChange();
    }
    foreach (Game1 gameInstance in this.gameInstances)
    {
      GameRunner.LoadInstance((InstanceGame) gameInstance);
      Viewport viewport = this.GraphicsDevice.Viewport;
      Game1.graphics.GraphicsDevice.Viewport = new Viewport(0, 0, Math.Min(gameInstance.localMultiplayerWindow.Width, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth), Math.Min(gameInstance.localMultiplayerWindow.Height, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight));
      gameInstance.Instance_Draw(gameTime);
      this.GraphicsDevice.Viewport = viewport;
      GameRunner.SaveInstance((InstanceGame) gameInstance);
    }
    if (LocalMultiplayer.IsLocalMultiplayer())
    {
      this.GraphicsDevice.Clear(Game1.bgColor);
      foreach (Game1 gameInstance in this.gameInstances)
      {
        Game1.isRenderingScreenBuffer = true;
        gameInstance.DrawSplitScreenWindow();
        Game1.isRenderingScreenBuffer = false;
      }
    }
    base.Draw(gameTime);
  }

  public int GetNewInstanceID() => this.nextInstanceId++;

  protected override void Initialize()
  {
    DebugTools.BeforeGameInitialize((Game) this);
    this.InitializeMainInstance();
    this.IsFixedTimeStep = true;
    base.Initialize();
    Game1.graphics.SynchronizeWithVerticalRetrace = true;
    Program.sdk.Initialize();
  }

  public bool WasWindowSizeChanged() => this._windowSizeChanged;

  public int GetMaxSimultaneousPlayers() => 4;

  public void InitializeMainInstance()
  {
    this.gameInstances = new List<Game1>();
    this.AddGameInstance(PlayerIndex.One);
  }

  public virtual void ExecuteForInstances(Action<Game1> action)
  {
    Game1 game1 = Game1.game1;
    if (game1 != null)
      GameRunner.SaveInstance((InstanceGame) game1);
    foreach (Game1 gameInstance in this.gameInstances)
    {
      GameRunner.LoadInstance((InstanceGame) gameInstance);
      action(gameInstance);
      GameRunner.SaveInstance((InstanceGame) gameInstance);
    }
    if (game1 != null)
      GameRunner.LoadInstance((InstanceGame) game1);
    else
      Game1.game1 = (Game1) null;
  }

  public virtual void RemoveGameInstance(Game1 instance)
  {
    if (!this.gameInstances.Contains(instance) || this.gameInstancesToRemove.Contains(instance))
      return;
    this.gameInstancesToRemove.Add(instance);
  }

  public virtual void AddGameInstance(PlayerIndex player_index)
  {
    Game1 game1 = Game1.game1;
    if (game1 != null)
      GameRunner.SaveInstance((InstanceGame) game1, true);
    if (this.gameInstances.Count > 0)
    {
      Game1 gameInstance = this.gameInstances[0];
      GameRunner.LoadInstance((InstanceGame) gameInstance);
      Game1.StartLocalMultiplayerIfNecessary();
      GameRunner.SaveInstance((InstanceGame) gameInstance, true);
    }
    Game1 instance = this.gameInstances.Count == 0 ? this.CreateGameInstance() : this.CreateGameInstance(player_index, this.gameInstances.Count);
    this.gameInstances.Add(instance);
    if (this.gamePtr == null)
      this.gamePtr = instance;
    if (this.gameInstances.Count > 0)
    {
      instance.staticVarHolder = Activator.CreateInstance(LocalMultiplayer.StaticVarHolderType);
      GameRunner.SetInstanceDefaults((InstanceGame) instance);
      GameRunner.LoadInstance((InstanceGame) instance);
    }
    Game1.game1 = instance;
    instance.Instance_Initialize();
    if (this.shouldLoadContent)
      instance.Instance_LoadContent();
    GameRunner.SaveInstance((InstanceGame) instance);
    if (game1 != null)
      GameRunner.LoadInstance((InstanceGame) game1);
    else
      Game1.game1 = (Game1) null;
    this._windowSizeChanged = true;
  }

  public virtual Game1 CreateGameInstance(PlayerIndex player_index = PlayerIndex.One, int index = 0)
  {
    return new Game1(player_index, index);
  }

  protected override void LoadContent()
  {
    Game1.graphics.PreferredBackBufferWidth = 1280 /*0x0500*/;
    Game1.graphics.PreferredBackBufferHeight = 720;
    Game1.graphics.ApplyChanges();
    GameRunner.LoadInstance((InstanceGame) this.gamePtr);
    this.gamePtr.Instance_LoadContent();
    GameRunner.SaveInstance((InstanceGame) this.gamePtr);
    DebugTools.GameLoadContent((Game) this);
    foreach (Game1 gameInstance in this.gameInstances)
    {
      if (gameInstance != this.gamePtr)
      {
        GameRunner.LoadInstance((InstanceGame) gameInstance);
        gameInstance.Instance_LoadContent();
        GameRunner.SaveInstance((InstanceGame) gameInstance);
      }
    }
    this.shouldLoadContent = true;
    base.LoadContent();
  }

  protected override void UnloadContent()
  {
    this.gamePtr.Instance_UnloadContent();
    base.UnloadContent();
  }

  protected override void Update(GameTime gameTime)
  {
    GameStateQuery.Update();
    for (int index = 0; index < this.activeNewDayProcesses.Count; ++index)
    {
      KeyValuePair<Game1, IEnumerator<int>> activeNewDayProcess = this.activeNewDayProcesses[index];
      Game1 key = this.activeNewDayProcesses[index].Key;
      GameRunner.LoadInstance((InstanceGame) key);
      if (!activeNewDayProcess.Value.MoveNext())
      {
        key.isLocalMultiplayerNewDayActive = false;
        this.activeNewDayProcesses.RemoveAt(index);
        --index;
        Utility.CollectGarbage();
      }
      GameRunner.SaveInstance((InstanceGame) key);
    }
    while (this.startButtonState.Count < 4)
      this.startButtonState.Add(-1);
    for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; ++playerIndex)
    {
      if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.Start))
      {
        if (this.startButtonState[(int) playerIndex] >= 0)
          this.startButtonState[(int) playerIndex]++;
      }
      else
        this.startButtonState[(int) playerIndex] = 0;
    }
    for (int index1 = 0; index1 < this.gameInstances.Count; ++index1)
    {
      Game1 gameInstance1 = this.gameInstances[index1];
      GameRunner.LoadInstance((InstanceGame) gameInstance1);
      if (index1 == 0)
      {
        PlayerIndex playerIndex = PlayerIndex.Two;
        if (gameInstance1.instanceOptions.gamepadMode == Options.GamepadModes.ForceOff)
          playerIndex = PlayerIndex.One;
        for (PlayerIndex index2 = playerIndex; index2 <= PlayerIndex.Four; ++index2)
        {
          bool flag = false;
          foreach (Game1 gameInstance2 in this.gameInstances)
          {
            if (gameInstance2.instancePlayerOneIndex == index2)
            {
              flag = true;
              break;
            }
          }
          if (!flag && gameInstance1.IsLocalCoopJoinable() && this.IsStartDown(index2) && gameInstance1.ShowLocalCoopJoinMenu())
            this.InvalidateStartPress(index2);
        }
      }
      else
        Game1.options.gamepadMode = Options.GamepadModes.ForceOn;
      Game1.debugTimings.StartUpdateTimer();
      gameInstance1.Instance_Update(gameTime);
      Game1.debugTimings.StopUpdateTimer();
      GameRunner.SaveInstance((InstanceGame) gameInstance1);
    }
    if (this.gameInstancesToRemove.Count > 0)
    {
      foreach (Game1 instance in this.gameInstancesToRemove)
      {
        GameRunner.LoadInstance((InstanceGame) instance);
        instance.exitEvent((object) null, (EventArgs) null);
        this.gameInstances.Remove(instance);
        Game1.game1 = (Game1) null;
      }
      for (int index = 0; index < this.gameInstances.Count; ++index)
        this.gameInstances[index].instanceIndex = index;
      if (this.gameInstances.Count == 1)
      {
        Game1 gameInstance = this.gameInstances[0];
        GameRunner.LoadInstance((InstanceGame) gameInstance, true);
        gameInstance.staticVarHolder = (object) null;
        Game1.EndLocalMultiplayer();
      }
      bool flag = false;
      if (this.gameInstances.Count > 0)
      {
        foreach (Game1 gameInstance in this.gameInstances)
        {
          if (gameInstance.instancePlayerOneIndex == PlayerIndex.One)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          this.gameInstances[0].instancePlayerOneIndex = PlayerIndex.One;
      }
      this.gameInstancesToRemove.Clear();
      this._windowSizeChanged = true;
    }
    base.Update(gameTime);
  }

  public virtual void InvalidateStartPress(PlayerIndex index)
  {
    if (index < PlayerIndex.One || index >= (PlayerIndex) this.startButtonState.Count)
      return;
    this.startButtonState[(int) index] = -1;
  }

  public virtual bool IsStartDown(PlayerIndex index)
  {
    return index >= PlayerIndex.One && index < (PlayerIndex) this.startButtonState.Count && this.startButtonState[(int) index] == 1;
  }

  private static void SetInstanceDefaults(InstanceGame instance)
  {
    for (int index = 0; index < LocalMultiplayer.staticDefaults.Count; ++index)
    {
      object staticDefault = LocalMultiplayer.staticDefaults[index];
      object obj = staticDefault != null ? staticDefault.DeepClone<object>() : (object) null;
      LocalMultiplayer.staticFields[index].SetValue((object) null, obj);
    }
    GameRunner.SaveInstance(instance);
  }

  public static void SaveInstance(InstanceGame instance, bool force = false)
  {
    if (!force && !LocalMultiplayer.IsLocalMultiplayer())
      return;
    if (instance.staticVarHolder == null)
      instance.staticVarHolder = Activator.CreateInstance(LocalMultiplayer.StaticVarHolderType);
    LocalMultiplayer.StaticSave(instance.staticVarHolder);
  }

  public static void LoadInstance(InstanceGame instance, bool force = false)
  {
    Game1.game1 = instance as Game1;
    if (!force && !LocalMultiplayer.IsLocalMultiplayer() || instance.staticVarHolder == null)
      return;
    LocalMultiplayer.StaticLoad(instance.staticVarHolder);
    Options options;
    if (Game1.player == null || !Game1.player.isCustomized.Value || !Game1.splitscreenOptions.TryGetValue(Game1.player.UniqueMultiplayerID, out options))
      return;
    Game1.options = options;
  }
}
