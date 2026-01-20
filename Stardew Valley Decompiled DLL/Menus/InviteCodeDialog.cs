// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.InviteCodeDialog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Menus;

public class InviteCodeDialog : ConfirmationDialog
{
  private string code;

  public InviteCodeDialog(string code, ConfirmationDialog.behavior onClose)
    : base(Game1.content.LoadString("Strings\\UI:Server_InviteCode", (object) code), onClose, onClose)
  {
    this.code = code;
    this.onCancel = new ConfirmationDialog.behavior(this.copyCode);
    ClickableTextureComponent textureComponent = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64 /*0x40*/, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 21, 64 /*0x40*/, 64 /*0x40*/), (string) null, (string) null, Game1.mouseCursors, new Rectangle(274, 284, 16 /*0x10*/, 16 /*0x10*/), 4f);
    textureComponent.myID = 102;
    textureComponent.leftNeighborID = 101;
    this.cancelButton = textureComponent;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.currentlySnappedComponent = this.getComponentWithID(101);
    this.snapCursorToCurrentSnappedComponent();
  }

  protected void copyCode(Farmer who)
  {
    if (DesktopClipboard.SetText(this.code))
      Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\UI:Server_InviteCode_Copied")));
    else
      Game1.showRedMessageUsingLoadString("Strings\\UI:Server_InviteCode_CopyFailed");
  }
}
