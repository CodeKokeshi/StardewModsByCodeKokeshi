// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.HookableClient
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Network;

/// <inheritdoc cref="T:StardewValley.Network.IHookableClient" />
public abstract class HookableClient : Client, IHookableClient
{
  /// <inheritdoc />
  public Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage { get; set; }

  /// <inheritdoc />
  public Action<OutgoingMessage, Action<OutgoingMessage>, Action> OnSendingMessage { get; set; }

  /// <summary>Construct an instance.</summary>
  public HookableClient()
  {
    this.OnProcessingMessage = new Action<IncomingMessage, Action<OutgoingMessage>, Action>(this.OnClientProcessingMessage);
    this.OnSendingMessage = new Action<OutgoingMessage, Action<OutgoingMessage>, Action>(this.OnClientSendingMessage);
  }

  private void OnClientProcessingMessage(
    IncomingMessage message,
    Action<OutgoingMessage> sendMessage,
    Action resume)
  {
    resume();
  }

  private void OnClientSendingMessage(
    OutgoingMessage message,
    Action<OutgoingMessage> sendMessage,
    Action resume)
  {
    resume();
  }
}
