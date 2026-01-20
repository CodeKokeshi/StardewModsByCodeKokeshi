// Decompiled with JetBrains decompiler
// Type: StardewValley.ChatCommands
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Delegates;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace StardewValley;

/// <summary>The commands that can be executed through the in-game chat box.</summary>
/// <remarks>See also <see cref="T:StardewValley.DebugCommands" />.</remarks>
/// <summary>The commands that can be executed through the in-game chat box.</summary>
/// <remarks>See also <see cref="T:StardewValley.DebugCommands" />.</remarks>
public static class ChatCommands
{
  /// <summary>The supported commands and their handlers.</summary>
  private static readonly Dictionary<string, ChatCommands.ChatCommand> Handlers = new Dictionary<string, ChatCommands.ChatCommand>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  /// <summary>Alternate names for chat commands (e.g. shorthand or acronyms).</summary>
  private static readonly Dictionary<string, string> Aliases = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

  /// <summary>Whether debug cheat commands are enabled.</summary>
  public static bool AllowCheats
  {
    get => Program.enableCheats || Game1.player?.team?.allowChatCheats.Value.GetValueOrDefault();
  }

  /// <summary>Register the default chat commands, defined as <see cref="T:StardewValley.ChatCommands.DefaultHandlers" /> methods.</summary>
  static ChatCommands()
  {
    ChatCommands.Register("qi", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Qi), (Func<string, string>) null);
    ChatCommands.Register("concernedApe", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.ConcernedApe), (Func<string, string>) null, new string[2]
    {
      "ape",
      "ca"
    });
    ChatCommands.Register("cheat", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Cheat), (Func<string, string>) null, new string[5]
    {
      "showMeTheMoney",
      "imACheat",
      "cheats",
      "freeGold",
      "rosebud"
    });
    ChatCommands.Register("money", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Money), (Func<string, string>) null, cheatsOnly: true);
    ChatCommands.Register("help", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Help), (Func<string, string>) null, new string[1]
    {
      "h"
    });
    ChatCommands.Register("clear", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Clear), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Clear_Help", (object) name)));
    ChatCommands.Register("list", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.List), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_List_Help", (object) name)), new string[2]
    {
      "users",
      "players"
    });
    ChatCommands.Register("color", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Color), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Color_Help", (object) name)));
    ChatCommands.Register("color-list", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.ColorList), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_ColorList_Help", (object) name)));
    ChatCommands.Register("emote", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Emote), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Emote_Help", (object) name)), new string[1]
    {
      "e"
    });
    ChatCommands.Register("mapScreenshot", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.MapScreenshot), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_MapScreenshot_Help", (object) name)));
    ChatCommands.Register("pause", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Pause), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Pause_Help", (object) name)));
    ChatCommands.Register("resume", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Resume), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Resume_Help", (object) name)));
    ChatCommands.Register("message", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Message), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Message_Help", (object) name)), new string[3]
    {
      "dm",
      "pm",
      "whisper"
    }, multiplayerOnly: true);
    ChatCommands.Register("reply", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Reply), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Reply_Help", (object) name)), new string[1]
    {
      "r"
    }, multiplayerOnly: true);
    ChatCommands.Register("ping", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Ping), (Func<string, string>) null, multiplayerOnly: true);
    ChatCommands.Register("kick", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Kick), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Kick_Help", (object) name)), mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("ban", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Ban), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Ban_Help", (object) name)), mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("unban", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Unban), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_Help", (object) name)), mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("unbanAll", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.UnbanAll), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_UnbanAll_Help", (object) name)), mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("moveBuildingPermission", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.MoveBuildingPermission), (Func<string, string>) null, new string[2]
    {
      "mbp",
      "movePermission"
    }, true, true);
    ChatCommands.Register("sleepAnnounceMode", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.SleepAnnounceMode), (Func<string, string>) null, mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("unlinkPlayer", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.UnlinkPlayer), (Func<string, string>) (name => Game1.content.LoadString("Strings\\UI:ChatCommands_UnlinkPlayer_Help", (object) name)), mainOnly: true, multiplayerOnly: true);
    ChatCommands.Register("debug", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.Debug), (Func<string, string>) null, cheatsOnly: true);
    ChatCommands.Register("logFile", ChatCommands.GetDebugPassThrough("LogFile"), (Func<string, string>) null);
    ChatCommands.Register("printDiag", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.PrintDiag), (Func<string, string>) null);
    ChatCommands.Register("recountNuts", new ChatCommandHandlerDelegate(ChatCommands.DefaultHandlers.RecountNuts), (Func<string, string>) null);
    ChatCommands.Register("sdlVersion", ChatCommands.GetDebugPassThrough("SdlVersion"), (Func<string, string>) null, new string[1]
    {
      "sdlv"
    });
  }

  /// <summary>Get whether a chat command exists.</summary>
  /// <param name="commandName">The chat command name, like <c>help</c>.</param>
  public static bool Exists(string commandName)
  {
    if (commandName == null)
      return false;
    return ChatCommands.Handlers.ContainsKey(commandName) || ChatCommands.Aliases.ContainsKey(commandName);
  }

  /// <summary>Register a chat command handler.</summary>
  /// <param name="commandName">The chat command name, like <c>help</c>. This should only contain alphanumeric, underscore, dash, and dot characters. For custom chat command, this should be prefixed with your mod ID like <c>Example.ModId_Command</c>.</param>
  /// <param name="handler">The handler which processes the chat command entered by the player.</param>
  /// <param name="helpDescription">Get the text to show for this command when the player enter <c>/help</c>, or <c>null</c> to hide it from the help command. This receives the registered command name.</param>
  /// <param name="mainOnly">Whether the command can only be used by the main player.</param>
  /// <param name="multiplayerOnly">Whether the command can only be used in multiplayer mode.</param>
  /// <param name="cheatsOnly">Whether the command can only be used when cheats are enabled.</param>
  /// <param name="aliases">Alternate names for the command (e.g. shorthand or acronyms).</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="commandName" /> is null or whitespace-only.</exception>
  /// <exception cref="T:System.ArgumentNullException">The <paramref name="handler" /> is null.</exception>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="commandName" /> is already registered.</exception>
  public static void Register(
    string commandName,
    ChatCommandHandlerDelegate handler,
    Func<string, string> helpDescription,
    string[] aliases = null,
    bool mainOnly = false,
    bool multiplayerOnly = false,
    bool cheatsOnly = false)
  {
    commandName = commandName?.Trim();
    if (string.IsNullOrWhiteSpace(commandName))
      throw new ArgumentException("The chat command name can't be null or empty.", nameof (commandName));
    if (ChatCommands.Handlers.ContainsKey(commandName))
      throw new InvalidOperationException($"The chat command name '{commandName}' is already registered.");
    string str;
    if (ChatCommands.Aliases.TryGetValue(commandName, out str))
      throw new InvalidOperationException($"The chat command name '{commandName}' is already registered as an alias of '{str}'.");
    if (handler == null)
      throw new ArgumentNullException(nameof (handler));
    ChatCommands.Handlers[commandName] = new ChatCommands.ChatCommand(commandName, helpDescription, handler, mainOnly, multiplayerOnly, cheatsOnly);
    if (aliases == null || aliases.Length == 0)
      return;
    foreach (string alias in aliases)
      ChatCommands.RegisterAlias(alias, commandName);
  }

  /// <summary>Register an alternate name for a chat command.</summary>
  /// <param name="alias">The alias to register. This should only contain alphanumeric, underscore, and dot characters. For custom chat command, this should be prefixed with your mod ID like <c>Example.ModId_Command</c>.</param>
  /// <param name="commandName">The chat command name to map it to, like <c>help</c>. This should already be registered (e.g. via <see cref="M:StardewValley.ChatCommands.Register(System.String,StardewValley.Delegates.ChatCommandHandlerDelegate,System.Func{System.String,System.String},System.String[],System.Boolean,System.Boolean,System.Boolean)" />).</param>
  /// <exception cref="T:System.ArgumentException">The <paramref name="alias" /> or <paramref name="commandName" /> is null or whitespace-only.</exception>
  /// <exception cref="T:System.InvalidOperationException">The <paramref name="commandName" /> is already registered.</exception>
  public static void RegisterAlias(string alias, string commandName)
  {
    alias = alias?.Trim();
    if (string.IsNullOrWhiteSpace(alias))
      throw new ArgumentException("The alias can't be null or empty.", nameof (alias));
    if (ChatCommands.Handlers.ContainsKey(alias))
      throw new InvalidOperationException($"The alias '{alias}' is already registered as a chat command name.");
    string str;
    if (ChatCommands.Aliases.TryGetValue(alias, out str))
      throw new InvalidOperationException($"The alias '{alias}' is already registered for '{str}'.");
    if (string.IsNullOrWhiteSpace(commandName))
      throw new ArgumentException("The chat command name can't be null or empty.", nameof (alias));
    ChatCommands.Aliases[alias] = ChatCommands.Handlers.ContainsKey(commandName) ? commandName : throw new InvalidOperationException($"The alias '{alias}' can't be registered for '{commandName}' because there's no chat command with that name.");
  }

  /// <summary>Try to handle a chat command.</summary>
  /// <param name="command">The full chat command split by spaces, including the command name and arguments.</param>
  /// <param name="chat">The chat box through which the command was entered.</param>
  /// <returns>Returns whether the command was found and executed, regardless of whether the command logic succeeded.</returns>
  public static bool TryHandle(string[] command, ChatBox chat)
  {
    string key = ArgUtility.Get(command, 0);
    if (string.IsNullOrWhiteSpace(key))
      return false;
    string str;
    if (ChatCommands.Aliases.TryGetValue(key, out str))
      key = str;
    ChatCommands.ChatCommand chatCommand;
    if (!ChatCommands.Handlers.TryGetValue(key, out chatCommand))
      return false;
    if (chatCommand.IsMainPlayerOnly && !Game1.IsMasterGame)
    {
      chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_HostOnly"));
      return true;
    }
    if (chatCommand.IsMultiplayerOnly && !Game1.IsServer && !Game1.IsMultiplayer)
    {
      chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_MultiplayerOnly"));
      return true;
    }
    if (chatCommand.IsCheatsOnly && !ChatCommands.AllowCheats)
    {
      string name = chatCommand.Name;
      if (name == "cheat" || name == "debug" || name == "money")
      {
        chat.addNiceTryEasterEggMessage();
        return true;
      }
      chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_CheatsOnly"));
      return true;
    }
    try
    {
      chatCommand.Handler(command, chat);
      return true;
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Error running chat command '{string.Join(" ", command)}'.", ex);
      return false;
    }
  }

  /// <summary>Create a chat command handler which passes a chat command through to a debug command directly without checking if <see cref="P:StardewValley.ChatCommands.AllowCheats" /> is enabled, with the same arguments</summary>
  /// <param name="debugCommandName">The name of the debug command registered with <see cref="T:StardewValley.DebugCommands" />.</param>
  public static ChatCommandHandlerDelegate GetDebugPassThrough(string debugCommandName)
  {
    return new ChatCommandHandlerDelegate(Handle);

    void Handle(string[] command, ChatBox chat)
    {
      command[0] = debugCommandName;
      string command1 = ArgUtility.UnsplitQuoteAware(command, ' ');
      chat.cheat(command1);
    }
  }

  /// <summary>A chat command which can be invoked through the chat box.</summary>
  public class ChatCommand
  {
    /// <summary>The main chat command name, like <c>help</c>.</summary>
    public readonly string Name;
    /// <summary>The method which processes a command from the chat box.</summary>
    public readonly ChatCommandHandlerDelegate Handler;
    /// <summary>&gt;The text to show for this command when the player enters <c>/help</c>. This can be or return null to hide it from the help output. This receives the registered command name.</summary>
    public readonly Func<string, string> HelpDescription;
    /// <summary>Whether this command can only be used by the main player.</summary>
    public readonly bool IsMainPlayerOnly;
    /// <summary>Whether this command can only be used in multiplayer mode.</summary>
    public readonly bool IsMultiplayerOnly;
    /// <summary>Whether the command can only be used when cheats are enabled.</summary>
    public readonly bool IsCheatsOnly;

    /// <summary>Construct an instance.</summary>
    /// <param name="name">The main chat command name, like <c>help</c>.</param>
    /// <param name="handler">The method which processes a command from the chat box.</param>
    /// <param name="helpDescription">The text to show for this command when the player enters <c>/help</c>. This can be or return null to hide it from the help output.</param>
    /// <param name="isMainPlayerOnly">Whether this command can only be used by the main player.</param>
    /// <param name="isMultiplayerOnly">Whether this command can only be used in multiplayer mode.</param>
    /// <param name="isCheatsOnly">Whether the command can only be used when cheats are enabled.</param>
    public ChatCommand(
      string name,
      Func<string, string> helpDescription,
      ChatCommandHandlerDelegate handler,
      bool isMainPlayerOnly,
      bool isMultiplayerOnly,
      bool isCheatsOnly)
    {
      this.Name = name;
      this.HelpDescription = helpDescription;
      this.Handler = handler;
      this.IsMainPlayerOnly = isMainPlayerOnly;
      this.IsMultiplayerOnly = isMultiplayerOnly;
      this.IsCheatsOnly = isCheatsOnly;
    }

    /// <summary>Get whether this chat command can be used by the current player.</summary>
    public bool IsVisible()
    {
      return (!this.IsMainPlayerOnly || Game1.IsMasterGame) && (!this.IsMultiplayerOnly || Game1.IsServer || Game1.IsMultiplayer) && (!this.IsCheatsOnly || ChatCommands.AllowCheats);
    }
  }

  /// <summary>The low-level handlers for vanilla chat commands. Most code should call <see cref="M:StardewValley.ChatCommands.TryHandle(System.String[],StardewValley.Menus.ChatBox)" /> instead, which adds error-handling.</summary>
  public static class DefaultHandlers
  {
    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Ban(string[] command, ChatBox chat)
    {
      int matchingIndex = 0;
      Farmer matchingFarmer = chat.findMatchingFarmer(command, ref matchingIndex, true);
      if (matchingFarmer != null)
      {
        string key = Game1.server.ban(matchingFarmer.UniqueMultiplayerID);
        string str;
        if (key == null || !Game1.bannedUsers.TryGetValue(key, out str))
        {
          chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Ban_Failed"));
        }
        else
        {
          string sub1 = str != null ? $"{str} ({key})" : key;
          chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Ban_Done", (object) sub1));
        }
      }
      else
      {
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_NoSuchOnlinePlayer"));
        chat.listPlayers(true);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Cheat(string[] command, ChatBox chat) => chat.addNiceTryEasterEggMessage();

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Clear(string[] command, ChatBox chat) => chat.messages.Clear();

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Color(string[] command, ChatBox chat)
    {
      if (command.Length <= 1)
        return;
      Game1.player.defaultChatColor = command[1];
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void ConcernedApe(string[] command, ChatBox chat)
    {
      if (Game1.player.mailReceived.Add("apeChat1"))
        chat.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_ConcernedApe_1"), new Color(104, 214, (int) byte.MaxValue));
      else
        chat.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_ConcernedApe_2"), Color.Yellow);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void ColorList(string[] command, ChatBox chat)
    {
      chat.addMessage("white, red, blue, green, jade, yellowgreen, pink, purple, yellow, orange, brown, gray, cream, salmon, peach, aqua, jungle, plum", Color.White);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Debug(string[] command, ChatBox chat)
    {
      string command1 = ArgUtility.UnsplitQuoteAware(command, ' ', 1);
      if (string.IsNullOrWhiteSpace(command1))
        chat.addErrorMessage("invalid usage: requires a debug command to run");
      else
        chat.cheat(command1, true);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Emote(string[] command, ChatBox chat)
    {
      if (!Game1.player.CanEmote())
        return;
      bool flag = false;
      if (command.Length > 1)
      {
        string lowerInvariant = command[1].ToLowerInvariant();
        string emote_type = lowerInvariant.Substring(0, Math.Min(lowerInvariant.Length, 16 /*0x10*/));
        for (int index = 0; index < Farmer.EMOTES.Length; ++index)
        {
          if (emote_type == Farmer.EMOTES[index].emoteString)
          {
            flag = true;
            break;
          }
        }
        if (flag)
          Game1.player.netDoEmote(emote_type);
      }
      if (flag)
        return;
      string message = "";
      for (int index = 0; index < Farmer.EMOTES.Length; ++index)
      {
        if (!Farmer.EMOTES[index].hidden)
        {
          message += Farmer.EMOTES[index].emoteString;
          if (index < Farmer.EMOTES.Length - 1)
            message += ", ";
        }
      }
      chat.addMessage(message, Color.White);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Help(string[] command, ChatBox chat)
    {
      string str = ArgUtility.Get(command, 1);
      if (str != null)
      {
        ChatCommands.ChatCommand chatCommand;
        if (ChatCommands.Handlers.TryGetValue(str, out chatCommand))
        {
          Func<string, string> helpDescription = chatCommand.HelpDescription;
          string sub1 = helpDescription != null ? helpDescription(chatCommand.Name) : (string) null;
          if (sub1 != null)
          {
            chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Help_CommandDescription", (object) sub1));
            return;
          }
        }
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Help_NoSuchCommand", (object) str));
      }
      List<string> values1 = new List<string>();
      List<string> values2 = new List<string>();
      foreach (ChatCommands.ChatCommand chatCommand in ChatCommands.Handlers.Values)
      {
        if (chatCommand.IsVisible())
        {
          Func<string, string> helpDescription = chatCommand.HelpDescription;
          if ((helpDescription != null ? helpDescription(chatCommand.Name) : (string) null) != null)
          {
            if (chatCommand.IsMultiplayerOnly)
              values2.Add(chatCommand.Name);
            else
              values1.Add(chatCommand.Name);
          }
        }
      }
      chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Help_Intro"));
      chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Help_CommandList", (object) string.Join(", ", (IEnumerable<string>) values1)));
      if (values2.Count <= 0)
        return;
      chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Help_MultiplayerCommandList", (object) string.Join(", ", (IEnumerable<string>) values2)));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Kick(string[] command, ChatBox chat)
    {
      int matchingIndex = 0;
      Farmer matchingFarmer = chat.findMatchingFarmer(command, ref matchingIndex, true);
      if (matchingFarmer != null)
      {
        Game1.server.kick(matchingFarmer.UniqueMultiplayerID);
      }
      else
      {
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_NoSuchOnlinePlayer"));
        chat.listPlayers(true);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void List(string[] command, ChatBox chat) => chat.listPlayers();

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void MapScreenshot(string[] command, ChatBox chat)
    {
      if (!Game1.game1.CanTakeScreenshots())
        return;
      int result = 25;
      string screenshot_name = (string) null;
      if (command.Length > 2 && !int.TryParse(command[2], out result))
        result = 25;
      if (command.Length > 1)
        screenshot_name = command[1];
      if (result <= 10)
        result = 10;
      string mapScreenshot = Game1.game1.takeMapScreenshot(new float?((float) result / 100f), screenshot_name, (Action) null);
      if (mapScreenshot != null)
        chat.addMessage($"Wrote '{mapScreenshot}'.", Color.White);
      else
        chat.addMessage("Failed.", Color.Red);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Message(string[] command, ChatBox chat) => chat.sendPrivateMessage(command);

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Money(string[] command, ChatBox chat)
    {
      ChatCommands.GetDebugPassThrough(nameof (Money))(command, chat);
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void MoveBuildingPermission(string[] command, ChatBox chat)
    {
      if (command.Length <= 1)
      {
        chat.addMessage("off, owned, on", Color.White);
      }
      else
      {
        switch (command[1].ToLowerInvariant())
        {
          case "off":
            Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.Off;
            break;
          case "owned":
            Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.OwnedBuildings;
            break;
          case "on":
            Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.On;
            break;
        }
        chat.addMessage($"moveBuildingPermission {Game1.player.team.farmhandsCanMoveBuildings.Value}", Color.White);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    /// <remarks>See also <see cref="M:StardewValley.ChatCommands.DefaultHandlers.Resume(System.String[],StardewValley.Menus.ChatBox)" />.</remarks>
    public static void Pause(string[] command, ChatBox chat)
    {
      if (!Game1.IsMasterGame)
      {
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_HostOnly"));
      }
      else
      {
        Game1.netWorldState.Value.IsPaused = !Game1.netWorldState.Value.IsPaused;
        chat.globalInfoMessage(Game1.netWorldState.Value.IsPaused ? "Paused" : "Resumed");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Ping(string[] command, ChatBox chat)
    {
      if (!Game1.IsMultiplayer)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (Game1.IsServer)
      {
        foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
        {
          stringBuilder.Clear();
          stringBuilder.AppendFormat("Ping({0}) {1}ms ", (object) otherFarmer.Value.Name, (object) (int) Game1.server.getPingToClient(otherFarmer.Key));
          chat.addMessage(stringBuilder.ToString(), Color.White);
        }
      }
      else
      {
        stringBuilder.AppendFormat("Ping: {0}ms", (object) (int) Game1.client.GetPingToHost());
        chat.addMessage(stringBuilder.ToString(), Color.White);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void PrintDiag(string[] command, ChatBox chat)
    {
      StringBuilder sb = new StringBuilder();
      Program.AppendDiagnostics(sb);
      chat.addInfoMessage(sb.ToString());
      Game1.log.Info(sb.ToString());
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Qi(string[] command, ChatBox chat)
    {
      if (Game1.player.mailReceived.Add("QiChat1"))
      {
        chat.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Qi_1"), new Color(100, 50, (int) byte.MaxValue));
      }
      else
      {
        if (!Game1.player.mailReceived.Add("QiChat2"))
          return;
        chat.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Qi_2"), new Color(100, 50, (int) byte.MaxValue));
        chat.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Qi_3"), Color.Yellow);
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void RecountNuts(string[] command, ChatBox chat) => Game1.game1.RecountWalnuts();

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Reply(string[] command, ChatBox chat) => chat.replyPrivateMessage(command);

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    /// <remarks>See also <see cref="M:StardewValley.ChatCommands.DefaultHandlers.Pause(System.String[],StardewValley.Menus.ChatBox)" />.</remarks>
    public static void Resume(string[] command, ChatBox chat)
    {
      if (!Game1.IsMasterGame)
      {
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_HostOnly"));
      }
      else
      {
        if (!Game1.netWorldState.Value.IsPaused)
          return;
        Game1.netWorldState.Value.IsPaused = false;
        chat.globalInfoMessage("Resumed");
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void SleepAnnounceMode(string[] command, ChatBox chat)
    {
      if (command.Length <= 1)
        return;
      switch (command[1].ToLowerInvariant())
      {
        case "all":
          Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.All;
          break;
        case "first":
          Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.First;
          break;
        case "off":
          Game1.player.team.sleepAnnounceMode.Value = FarmerTeam.SleepAnnounceModes.Off;
          break;
      }
      Game1.multiplayer.globalChatInfoMessage("SleepAnnounceModeSet", TokenStringBuilder.LocalizedText($"Strings\\UI:ChatCommands_SleepAnnounceMode_{Game1.player.team.sleepAnnounceMode.Value}"));
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void Unban(string[] command, ChatBox chat)
    {
      if (Game1.bannedUsers.Count == 0)
      {
        chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_NoPlayersBanned"));
      }
      else
      {
        bool flag = false;
        if (command.Length > 1)
        {
          string key1 = command[1];
          string key2 = (string) null;
          string str;
          if (Game1.bannedUsers.TryGetValue(key1, out str))
          {
            key2 = key1;
          }
          else
          {
            foreach (KeyValuePair<string, string> bannedUser in (Dictionary<string, string>) Game1.bannedUsers)
            {
              if (bannedUser.Value == key1)
              {
                key2 = bannedUser.Key;
                str = bannedUser.Value;
                break;
              }
            }
          }
          if (key2 != null)
          {
            string sub1 = str != null ? $"{str} ({key2})" : key2;
            chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_Done", (object) sub1));
            Game1.bannedUsers.Remove(key2);
          }
          else
          {
            flag = true;
            chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_PlayerNotFound"));
          }
        }
        else
          flag = true;
        if (!flag)
          return;
        chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_PlayerList"));
        foreach (KeyValuePair<string, string> bannedUser in (Dictionary<string, string>) Game1.bannedUsers)
        {
          string message = "- " + bannedUser.Key;
          if (bannedUser.Value != null)
            message = $"- {bannedUser.Value} ({bannedUser.Key})";
          chat.addInfoMessage(message);
        }
      }
    }

    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void UnbanAll(string[] command, ChatBox chat)
    {
      if (Game1.bannedUsers.Count == 0)
      {
        chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Unban_NoPlayersBanned"));
      }
      else
      {
        chat.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_UnbanAll_Done"));
        Game1.bannedUsers.Clear();
      }
    }

    /// <summary>Unlink a farmer from its player, so it can be claimed by anyone.</summary>
    /// <inheritdoc cref="T:StardewValley.Delegates.ChatCommandHandlerDelegate" />
    public static void UnlinkPlayer(string[] command, ChatBox chat)
    {
      int matchingIndex = 0;
      Farmer matchingFarmer = chat.findMatchingFarmer(command, ref matchingIndex, true, false);
      if (matchingFarmer != null)
      {
        matchingFarmer.userID.Value = string.Empty;
        Game1.log.Info($"Unlinked {(matchingFarmer.isActive() ? "active" : "inactive")} player {matchingFarmer.uniqueMultiplayerID} ('{matchingFarmer.Name}').");
      }
      else
      {
        chat.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_NoSuchPlayer"));
        chat.listPlayers(true, false);
      }
    }
  }
}
