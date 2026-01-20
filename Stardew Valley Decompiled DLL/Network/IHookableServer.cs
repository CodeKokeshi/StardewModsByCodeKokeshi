// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.IHookableServer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Network;

/// <summary>A net sync server which allows intercepting received messages.</summary>
public interface IHookableServer
{
  /// <summary>A callback to raise when receiving a message. This receives the incoming message, a method to send a message, and a callback to run the default logic.</summary>
  Action<IncomingMessage, Action<OutgoingMessage>, Action> OnProcessingMessage { get; set; }
}
