// Decompiled with JetBrains decompiler
// Type: StardewValley.Delegates.ChatCommandHandlerDelegate
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Menus;

#nullable disable
namespace StardewValley.Delegates;

/// <summary>Handles a chat command.</summary>
/// <param name="command">The full chat command split by spaces, including the command name.</param>
/// <param name="chat">The chat box through which the command was entered.</param>
public delegate void ChatCommandHandlerDelegate(string[] command, ChatBox chat);
