// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ChatBox
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Extensions;
using StardewValley.Logging;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Menus;

public class ChatBox : IClickableMenu
{
  public const int chatMessage = 0;
  public const int errorMessage = 1;
  public const int userNotificationMessage = 2;
  public const int privateMessage = 3;
  public const int defaultMaxMessages = 10;
  public const int timeToDisplayMessages = 600;
  public const int chatboxWidth = 896;
  public const int chatboxHeight = 56;
  public const int region_chatBox = 101;
  public const int region_emojiButton = 102;
  public ChatTextBox chatBox;
  public ClickableComponent chatBoxCC;
  /// <summary>A logger which copies messages to the chat box, used when entering commands through the chat.</summary>
  private readonly IGameLogger CheatCommandChatLogger;
  public List<ChatMessage> messages = new List<ChatMessage>();
  private KeyboardState oldKBState;
  private List<string> cheatHistory = new List<string>();
  private int cheatHistoryPosition = -1;
  public int maxMessages = 10;
  public static Texture2D emojiTexture;
  public ClickableTextureComponent emojiMenuIcon;
  public EmojiMenu emojiMenu;
  public bool choosingEmoji;
  private long lastReceivedPrivateMessagePlayerId;

  public ChatBox()
  {
    this.CheatCommandChatLogger = (IGameLogger) new StardewValley.Logging.CheatCommandChatLogger(this);
    Texture2D texture2D = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
    this.chatBox = new ChatTextBox(texture2D, (Texture2D) null, Game1.smallFont, Color.White);
    this.chatBox.OnEnterPressed += new TextBoxEvent(this.textBoxEnter);
    this.chatBox.TitleText = "Chat";
    this.chatBoxCC = new ClickableComponent(new Rectangle(this.chatBox.X, this.chatBox.Y, this.chatBox.Width, this.chatBox.Height), "")
    {
      myID = 101
    };
    Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber) this.chatBox;
    ChatBox.emojiTexture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
    ClickableTextureComponent textureComponent = new ClickableTextureComponent(new Rectangle(0, 0, 40, 36), ChatBox.emojiTexture, new Rectangle(0, 0, 9, 9), 4f);
    textureComponent.myID = 102;
    textureComponent.leftNeighborID = 101;
    this.emojiMenuIcon = textureComponent;
    this.emojiMenu = new EmojiMenu(this, ChatBox.emojiTexture, texture2D);
    this.chatBoxCC.rightNeighborID = 102;
    this.updatePosition();
    this.chatBox.Selected = false;
  }

  public override void snapToDefaultClickableComponent()
  {
    this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  private void updatePosition()
  {
    this.chatBox.Width = 896;
    this.chatBox.Height = 56;
    this.width = this.chatBox.Width;
    this.height = this.chatBox.Height;
    this.xPositionOnScreen = 0;
    this.yPositionOnScreen = Game1.uiViewport.Height - this.chatBox.Height;
    Utility.makeSafe(ref this.xPositionOnScreen, ref this.yPositionOnScreen, this.chatBox.Width, this.chatBox.Height);
    this.chatBox.X = this.xPositionOnScreen;
    this.chatBox.Y = this.yPositionOnScreen;
    this.chatBoxCC.bounds = new Rectangle(this.chatBox.X, this.chatBox.Y, this.chatBox.Width, this.chatBox.Height);
    this.emojiMenuIcon.bounds.Y = this.chatBox.Y + 8;
    this.emojiMenuIcon.bounds.X = this.chatBox.Width - this.emojiMenuIcon.bounds.Width - 8;
    if (this.emojiMenu == null)
      return;
    this.emojiMenu.xPositionOnScreen = this.emojiMenuIcon.bounds.Center.X - 146;
    this.emojiMenu.yPositionOnScreen = this.emojiMenuIcon.bounds.Y - 248;
  }

  public virtual void textBoxEnter(string text_to_send)
  {
    if (text_to_send.Length < 1)
      return;
    if (text_to_send[0] == '/')
    {
      string str = ArgUtility.SplitBySpaceAndGet(text_to_send, 0);
      if ((str != null ? (str.Length > 1 ? 1 : 0) : 0) != 0)
      {
        this.runCommand(text_to_send.Substring(1));
        return;
      }
    }
    text_to_send = Program.sdk.FilterDirtyWords(text_to_send);
    Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, text_to_send, Multiplayer.AllPlayers);
    this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 0, LocalizedContentManager.CurrentLanguageCode, text_to_send);
  }

  public virtual void textBoxEnter(TextBox sender)
  {
    if (sender is ChatTextBox chatTextBox)
    {
      if (chatTextBox.finalText.Count > 0)
      {
        bool include_color_information = true;
        string message1 = chatTextBox.finalText[0].message;
        if ((message1 != null ? (message1.StartsWith('/') ? 1 : 0) : 0) != 0)
        {
          string str = ArgUtility.SplitBySpaceAndGet(chatTextBox.finalText[0].message, 0);
          if ((str != null ? (str.Length > 1 ? 1 : 0) : 0) != 0)
            include_color_information = false;
        }
        if (chatTextBox.finalText.Count == 1)
        {
          if (chatTextBox.finalText[0].message != null || chatTextBox.finalText[0].emojiIndex != -1)
          {
            string message2 = chatTextBox.finalText[0].message;
            if ((message2 != null ? (message2.Trim().Length == 0 ? 1 : 0) : 0) != 0)
              goto label_9;
          }
          else
            goto label_9;
        }
        this.textBoxEnter(ChatMessage.makeMessagePlaintext(chatTextBox.finalText, include_color_information));
      }
label_9:
      chatTextBox.reset();
      this.cheatHistoryPosition = -1;
    }
    sender.Text = "";
    this.clickAway();
  }

  public virtual void addInfoMessage(string message)
  {
    this.receiveChatMessage(0L, 2, LocalizedContentManager.CurrentLanguageCode, message);
  }

  public virtual void globalInfoMessage(string messageKey, params string[] args)
  {
    if (Game1.IsMultiplayer)
      Game1.multiplayer.globalChatInfoMessage(messageKey, args);
    else
      this.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, (object[]) args));
  }

  public virtual void addErrorMessage(string message)
  {
    this.receiveChatMessage(0L, 1, LocalizedContentManager.CurrentLanguageCode, message);
  }

  public virtual void listPlayers(bool otherPlayersOnly = false, bool onlineOnly = true)
  {
    this.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_ListOnlinePlayers"));
    foreach (Farmer farmer in onlineOnly ? (IEnumerable<Farmer>) Game1.getOnlineFarmers() : Game1.getAllFarmers())
    {
      if (!otherPlayersOnly || farmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
        this.addInfoMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_ListOnlinePlayersEntry", (object) ChatBox.formattedUserNameLong(farmer)));
    }
  }

  protected virtual void runCommand(string commandText)
  {
    if (ChatCommands.TryHandle(ArgUtility.SplitBySpace(commandText), this) || !ChatCommands.AllowCheats && !Game1.isRunningMacro)
      return;
    this.cheat(commandText);
  }

  public virtual void cheat(string command, bool isDebug = false)
  {
    string str = (isDebug ? "debug " : "") + command;
    Game1.debugOutput = (string) null;
    this.addInfoMessage("/" + str);
    if (!Game1.isRunningMacro)
      this.cheatHistory.Insert(0, "/" + str);
    if (Game1.game1.parseDebugInput(command, this.CheatCommandChatLogger))
    {
      if (string.IsNullOrEmpty(Game1.debugOutput))
        return;
      this.addInfoMessage(Game1.debugOutput);
    }
    else if (!string.IsNullOrEmpty(Game1.debugOutput))
      this.addErrorMessage(Game1.debugOutput);
    else
      this.addErrorMessage($"{Game1.content.LoadString("Strings\\StringsFromCSFiles:ChatBox.cs.10261")} {ArgUtility.SplitBySpaceAndGet(command, 0)}");
  }

  public void replyPrivateMessage(string[] command)
  {
    if (!Game1.IsMultiplayer)
      return;
    if (this.lastReceivedPrivateMessagePlayerId == 0L)
    {
      this.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Reply_NoMessageFound"));
    }
    else
    {
      Farmer farmer;
      if (!Game1.otherFarmers.TryGetValue(this.lastReceivedPrivateMessagePlayerId, out farmer) || !farmer.isActive())
      {
        this.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Reply_Failed"));
      }
      else
      {
        if (command.Length <= 1)
          return;
        string words = "";
        for (int index = 1; index < command.Length; ++index)
        {
          words += command[index];
          if (index < command.Length - 1)
            words += " ";
        }
        string message = Program.sdk.FilterDirtyWords(words);
        Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, this.lastReceivedPrivateMessagePlayerId);
        this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 3, LocalizedContentManager.CurrentLanguageCode, message);
      }
    }
  }

  public Farmer findMatchingFarmer(
    string[] command,
    ref int matchingIndex,
    bool allowMatchingByUserName = false,
    bool onlineOnly = true)
  {
    Farmer matchingFarmer = (Farmer) null;
    foreach (Farmer farmer in onlineOnly ? (IEnumerable<Farmer>) Game1.otherFarmers.Values : Game1.getAllFarmers())
    {
      string[] strArray1 = ArgUtility.SplitBySpace(farmer.displayName);
      bool flag1 = true;
      int index1;
      for (index1 = 0; index1 < strArray1.Length; ++index1)
      {
        if (command.Length > index1 + 1)
        {
          if (!command[index1 + 1].EqualsIgnoreCase(strArray1[index1]))
          {
            flag1 = false;
            break;
          }
        }
        else
        {
          flag1 = false;
          break;
        }
      }
      if (flag1)
      {
        matchingFarmer = farmer;
        matchingIndex = index1;
        break;
      }
      if (allowMatchingByUserName)
      {
        bool flag2 = true;
        string[] strArray2 = ArgUtility.SplitBySpace(Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID));
        if (strArray2.Length != 0)
        {
          int index2;
          for (index2 = 0; index2 < strArray2.Length; ++index2)
          {
            if (command.Length > index2 + 1)
            {
              if (!command[index2 + 1].EqualsIgnoreCase(strArray2[index2]))
              {
                flag2 = false;
                break;
              }
            }
            else
            {
              flag2 = false;
              break;
            }
          }
          if (flag2)
          {
            matchingFarmer = farmer;
            matchingIndex = index2;
            break;
          }
        }
      }
    }
    return matchingFarmer;
  }

  public void sendPrivateMessage(string[] command)
  {
    if (!Game1.IsMultiplayer)
      return;
    int matchingIndex = 0;
    Farmer matchingFarmer = this.findMatchingFarmer(command, ref matchingIndex);
    if (matchingFarmer == null)
    {
      this.addErrorMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_NoSuchOnlinePlayer"));
    }
    else
    {
      string words = "";
      for (int index = matchingIndex + 1; index < command.Length; ++index)
      {
        words += command[index];
        if (index < command.Length - 1)
          words += " ";
      }
      string message = Program.sdk.FilterDirtyWords(words);
      Game1.multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, message, matchingFarmer.UniqueMultiplayerID);
      this.receiveChatMessage(Game1.player.UniqueMultiplayerID, 3, LocalizedContentManager.CurrentLanguageCode, message);
    }
  }

  public bool isActive() => this.chatBox.Selected;

  public void activate()
  {
    this.chatBox.Selected = true;
    this.setText("");
  }

  public override void clickAway()
  {
    base.clickAway();
    if (this.choosingEmoji && this.emojiMenu.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) && !Game1.input.GetKeyboardState().IsKeyDown(Keys.Escape))
      return;
    int num = this.chatBox.Selected ? 1 : 0;
    this.chatBox.Selected = false;
    this.choosingEmoji = false;
    this.setText("");
    this.cheatHistoryPosition = -1;
    if (num == 0)
      return;
    Game1.oldKBState = Game1.GetKeyboardState();
  }

  public override bool isWithinBounds(int x, int y)
  {
    if (x - this.xPositionOnScreen < this.width && x - this.xPositionOnScreen >= 0 && y - this.yPositionOnScreen < this.height && y - this.yPositionOnScreen >= -this.getOldMessagesBoxHeight())
      return true;
    return this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y);
  }

  public virtual void setText(string text) => this.chatBox.setText(text);

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
    switch (key)
    {
      case Keys.Up:
        if (this.cheatHistoryPosition < this.cheatHistory.Count - 1)
        {
          ++this.cheatHistoryPosition;
          this.chatBox.setText(this.cheatHistory[this.cheatHistoryPosition]);
          break;
        }
        break;
      case Keys.Down:
        if (this.cheatHistoryPosition > 0)
        {
          --this.cheatHistoryPosition;
          this.chatBox.setText(this.cheatHistory[this.cheatHistoryPosition]);
          break;
        }
        break;
    }
    if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key) || Game1.options.doesInputListContain(Game1.options.moveRightButton, key) || Game1.options.doesInputListContain(Game1.options.moveDownButton, key) || Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
      return;
    base.receiveKeyPress(key);
  }

  public override bool readyToClose() => false;

  /// <inheritdoc />
  public override void receiveGamePadButton(Buttons button)
  {
  }

  public bool isHoveringOverClickable(int x, int y)
  {
    return this.emojiMenuIcon.containsPoint(x, y) || this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y);
  }

  /// <inheritdoc />
  public override void receiveLeftClick(int x, int y, bool playSound = true)
  {
    if (!this.chatBox.Selected)
      return;
    if (this.emojiMenuIcon.containsPoint(x, y))
    {
      this.choosingEmoji = !this.choosingEmoji;
      Game1.playSound("shwip");
      this.emojiMenuIcon.scale = 4f;
    }
    else if (this.choosingEmoji && this.emojiMenu.isWithinBounds(x, y))
    {
      this.emojiMenu.leftClick(x, y, this);
    }
    else
    {
      this.chatBox.Update();
      if (this.choosingEmoji)
      {
        this.choosingEmoji = false;
        this.emojiMenuIcon.scale = 4f;
      }
      if (!this.isWithinBounds(x, y))
        return;
      this.chatBox.Selected = true;
    }
  }

  public static string formattedUserName(Farmer farmer)
  {
    string words = farmer.Name;
    if (string.IsNullOrWhiteSpace(words))
      words = Game1.content.LoadString("Strings\\UI:Chat_PlayerJoinedNewName");
    return Program.sdk.FilterDirtyWords(words);
  }

  public static string formattedUserNameLong(Farmer farmer)
  {
    string sub1 = ChatBox.formattedUserName(farmer);
    string userName = Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID);
    return string.IsNullOrWhiteSpace(userName) ? sub1 : Game1.content.LoadString("Strings\\UI:Chat_PlayerName", (object) sub1, (object) userName);
  }

  public string formatMessage(long sourceFarmer, int chatKind, string message)
  {
    string sub1 = Game1.content.LoadString("Strings\\UI:Chat_UnknownUserName");
    Farmer farmer;
    if (sourceFarmer == Game1.player.UniqueMultiplayerID)
      farmer = Game1.player;
    else if (!Game1.otherFarmers.TryGetValue(sourceFarmer, out farmer))
      farmer = (Farmer) null;
    if (farmer != null)
      sub1 = ChatBox.formattedUserName(farmer);
    switch (chatKind)
    {
      case 0:
        return Game1.content.LoadString("Strings\\UI:Chat_ChatMessageFormat", (object) sub1, (object) message);
      case 2:
        return Game1.content.LoadString("Strings\\UI:Chat_UserNotificationMessageFormat", (object) message);
      case 3:
        return Game1.content.LoadString("Strings\\UI:Chat_PrivateMessageFormat", (object) sub1, (object) message);
      default:
        return Game1.content.LoadString("Strings\\UI:Chat_ErrorMessageFormat", (object) message);
    }
  }

  public virtual Color messageColor(int chatKind)
  {
    switch (chatKind)
    {
      case 0:
        return this.chatBox.TextColor;
      case 2:
        return Color.Yellow;
      case 3:
        return Color.DarkCyan;
      default:
        return Color.Red;
    }
  }

  public virtual void receiveChatMessage(
    long sourceFarmer,
    int chatKind,
    LocalizedContentManager.LanguageCode language,
    string message)
  {
    string text1 = this.formatMessage(sourceFarmer, chatKind, message);
    ChatMessage chatMessage = new ChatMessage();
    SpriteFont font = this.chatBox.Font;
    int width = this.chatBox.Width - 16 /*0x10*/;
    string text2 = Game1.parseText(text1, font, width);
    chatMessage.timeLeftToDisplay = 600;
    chatMessage.verticalSize = (int) this.chatBox.Font.MeasureString(text2).Y + 4;
    chatMessage.color = this.messageColor(chatKind);
    chatMessage.language = language;
    chatMessage.parseMessageForEmoji(text2);
    this.messages.Add(chatMessage);
    if (this.messages.Count > this.maxMessages)
      this.messages.RemoveAt(0);
    if (chatKind != 3 || sourceFarmer == Game1.player.UniqueMultiplayerID)
      return;
    this.lastReceivedPrivateMessagePlayerId = sourceFarmer;
  }

  public virtual void addMessage(string message, Color color)
  {
    ChatMessage chatMessage = new ChatMessage();
    string text = Game1.parseText(message, this.chatBox.Font, this.chatBox.Width - 8);
    chatMessage.timeLeftToDisplay = 600;
    chatMessage.verticalSize = (int) this.chatBox.Font.MeasureString(text).Y + 4;
    chatMessage.color = color;
    chatMessage.language = LocalizedContentManager.CurrentLanguageCode;
    chatMessage.parseMessageForEmoji(text);
    this.messages.Add(chatMessage);
    if (this.messages.Count <= this.maxMessages)
      return;
    this.messages.RemoveAt(0);
  }

  /// <summary>Add a "ConcernedApe: Nice try..." Easter egg message to the chat box.</summary>
  public void addNiceTryEasterEggMessage()
  {
    this.addMessage(Game1.content.LoadString("Strings\\UI:ChatCommands_Error_NiceTry"), new Color(104, 214, (int) byte.MaxValue));
  }

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.emojiMenuIcon.tryHover(x, y, 1f);
    this.emojiMenuIcon.tryHover(x, y, 1f);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    KeyboardState keyboardState = Game1.input.GetKeyboardState();
    foreach (Keys pressedKey in keyboardState.GetPressedKeys())
    {
      if (!this.oldKBState.IsKeyDown(pressedKey))
        this.receiveKeyPress(pressedKey);
    }
    this.oldKBState = keyboardState;
    for (int index = 0; index < this.messages.Count; ++index)
    {
      if (this.messages[index].timeLeftToDisplay > 0)
        --this.messages[index].timeLeftToDisplay;
      if (this.messages[index].timeLeftToDisplay < 75)
        this.messages[index].alpha = (float) this.messages[index].timeLeftToDisplay / 75f;
    }
    if (this.chatBox.Selected)
    {
      foreach (ChatMessage message in this.messages)
        message.alpha = 1f;
    }
    this.emojiMenuIcon.tryHover(0, 0, 1f);
  }

  /// <inheritdoc />
  public override void receiveScrollWheelAction(int direction)
  {
    if (!this.choosingEmoji)
      return;
    this.emojiMenu.receiveScrollWheelAction(direction);
  }

  /// <inheritdoc />
  public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
  {
    this.updatePosition();
  }

  public static SpriteFont messageFont(LocalizedContentManager.LanguageCode language)
  {
    return Game1.content.Load<SpriteFont>("Fonts\\SmallFont", language);
  }

  public int getOldMessagesBoxHeight()
  {
    int messagesBoxHeight = 20;
    for (int index = this.messages.Count - 1; index >= 0; --index)
    {
      ChatMessage message = this.messages[index];
      if (this.chatBox.Selected || (double) message.alpha > 0.0099999997764825821)
        messagesBoxHeight += message.verticalSize;
    }
    return messagesBoxHeight;
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    int num1 = 0;
    bool flag = false;
    for (int index = this.messages.Count - 1; index >= 0; --index)
    {
      ChatMessage message = this.messages[index];
      if (this.chatBox.Selected || (double) message.alpha > 0.0099999997764825821)
      {
        num1 += message.verticalSize;
        flag = true;
      }
    }
    if (flag)
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(301, 288, 15, 15), this.xPositionOnScreen, this.yPositionOnScreen - num1 - 20 + (this.chatBox.Selected ? 0 : this.chatBox.Height), this.chatBox.Width, num1 + 20, Color.White, 4f, false);
    int num2 = 0;
    for (int index = this.messages.Count - 1; index >= 0; --index)
    {
      ChatMessage message = this.messages[index];
      num2 += message.verticalSize;
      message.draw(b, this.xPositionOnScreen + 12, this.yPositionOnScreen - num2 - 8 + (this.chatBox.Selected ? 0 : this.chatBox.Height));
    }
    if (!this.chatBox.Selected)
      return;
    this.chatBox.Draw(b, false);
    this.emojiMenuIcon.draw(b, Color.White, 0.99f);
    if (this.choosingEmoji)
      this.emojiMenu.draw(b);
    if (!this.isWithinBounds(Game1.getMouseX(), Game1.getMouseY()) || Game1.options.hardwareCursor)
      return;
    Game1.mouseCursor = Game1.options.gamepadControls ? Game1.cursor_gamepad_pointer : Game1.cursor_default;
  }
}
