// Decompiled with JetBrains decompiler
// Type: StardewValley.NPCDialogueResponse
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley;

public class NPCDialogueResponse : Response
{
  public int friendshipChange;
  public string id;
  public string extraArgument;

  public NPCDialogueResponse(
    string id,
    int friendshipChange,
    string keyToNPCresponse,
    string responseText,
    string extraArgument = null)
    : base(keyToNPCresponse, responseText)
  {
    this.friendshipChange = friendshipChange;
    this.id = id;
    this.extraArgument = extraArgument;
  }

  public NPCDialogueResponse(NPCDialogueResponse other)
    : this(other.id, other.friendshipChange, other.responseKey, other.responseText, other.extraArgument)
  {
    if (other.hotkey == Keys.None)
      return;
    this.SetHotKey(other.hotkey);
  }
}
