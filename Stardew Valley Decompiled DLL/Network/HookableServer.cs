// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.HookableServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Network;

/// <inheritdoc cref="T:StardewValley.Network.IHookableServer" />
public abstract class HookableServer : Server, IHookableServer
{
  /// <inheritdoc />
  public Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage { get; set; }

  /// <summary>Construct an instance.</summary>
  /// <param name="gameServer">The underlying game server.</param>
  public HookableServer(IGameServer gameServer)
    : base(gameServer)
  {
    this.OnProcessingMessage = new Action<IncomingMessage, Action<OutgoingMessage>, Action>(this.OnServerProcessingMessage);
  }

  private void OnServerProcessingMessage(
    IncomingMessage message,
    Action<OutgoingMessage> sendMessage,
    Action resume)
  {
    resume();
  }
}
