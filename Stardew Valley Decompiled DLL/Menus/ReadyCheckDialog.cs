// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ReadyCheckDialog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace StardewValley.Menus;

public class ReadyCheckDialog : ConfirmationDialog
{
  public string checkName;
  private bool allowCancel;

  public ReadyCheckDialog(
    string checkName,
    bool allowCancel,
    ConfirmationDialog.behavior onConfirm = null,
    ConfirmationDialog.behavior onCancel = null)
    : base(Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) "N", (object) "M"), onConfirm, onCancel)
  {
    this.checkName = checkName;
    this.allowCancel = allowCancel;
    this.okButton.visible = false;
    this.cancelButton.visible = this.isCancelable();
    this.updateMessage();
    this.exitFunction = (IClickableMenu.onExit) (() => this.closeDialog(Game1.player));
    if (!Game1.options.SnappyMenus)
      return;
    this.populateClickableComponentList();
    this.snapToDefaultClickableComponent();
  }

  public bool isCancelable()
  {
    return this.allowCancel && Game1.netReady.IsReadyCheckCancelable(this.checkName);
  }

  public override bool readyToClose() => this.isCancelable();

  public override void closeDialog(Farmer who)
  {
    base.closeDialog(who);
    Game1.displayFarmer = true;
    if (!this.isCancelable())
      return;
    Game1.netReady.SetLocalReady(this.checkName, false);
  }

  /// <inheritdoc />
  public override void receiveKeyPress(Keys key)
  {
  }

  private void updateMessage()
  {
    int numberReady = Game1.netReady.GetNumberReady(this.checkName);
    int numberRequired = Game1.netReady.GetNumberRequired(this.checkName);
    this.message = Game1.content.LoadString("Strings\\UI:ReadyCheck", (object) numberReady, (object) numberRequired);
  }

  /// <inheritdoc />
  public override void update(GameTime time)
  {
    base.update(time);
    this.cancelButton.visible = this.isCancelable();
    this.updateMessage();
    Game1.netReady.SetLocalReady(this.checkName, true);
    if (!Game1.netReady.IsReady(this.checkName))
      return;
    this.confirm();
  }
}
