// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ServerConnectionDialog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.Menus;

public class ServerConnectionDialog : ConfirmationDialog
{
  public ServerConnectionDialog(
    ConfirmationDialog.behavior onConfirm = null,
    ConfirmationDialog.behavior onCancel = null)
    : base(Game1.content.LoadString("Strings\\UI:CoopMenu_Connecting"), onConfirm, onCancel)
  {
    this.okButton.visible = false;
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    if (Game1.server == null || !Game1.server.connected())
      return;
    this.confirm();
  }
}
