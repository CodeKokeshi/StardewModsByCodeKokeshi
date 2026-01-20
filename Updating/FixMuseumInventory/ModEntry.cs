using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FixMuseumInventory;

public class ModEntry : Mod
{
    private const int MoveButtonComponentId = 19512001;
    private const int StationaryTicksBeforeShowingMoveTooltip = 6;

    // Move button bounds
    private Rectangle _moveButtonBounds;

    private ClickableComponent? _moveButtonComponent;
    
    // Toggle move mode: click once to start moving, click again to place
    private bool _isMoving;
    private Point _mouseOffset; // Offset from mouse to menu top-left

    // Used to suppress tooltip while the menu is actively moving.
    private Point? _lastMenuPosition;
    private int _ticksSinceMenuMoved = int.MaxValue;
    
    // Saved position for the session
    private Point? _savedMenuOffset;

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        _isMoving = false;
        _moveButtonComponent = null;
        _lastMenuPosition = null;
        _ticksSinceMenuMoved = int.MaxValue;

        // When museum menu opens, apply saved offset if we have one
        if (e.NewMenu is MuseumMenu menu && _savedMenuOffset.HasValue)
        {
            // Precompute move button bounds from okButton so clamping includes side buttons.
            if (menu.okButton != null)
            {
                _moveButtonBounds = new Rectangle(
                    menu.okButton.bounds.X,
                    menu.okButton.bounds.Y - 80,
                    64,
                    64
                );
            }

            Point clamped = ClampMenuPosition(menu, _savedMenuOffset.Value);
            int targetX = clamped.X;
            int targetY = clamped.Y;

            int dx = targetX - menu.xPositionOnScreen;
            int dy = targetY - menu.yPositionOnScreen;
            
            if (dx != 0 || dy != 0)
                menu.movePosition(dx, dy);
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
        {
            _isMoving = false;
            _lastMenuPosition = null;
            _ticksSinceMenuMoved = int.MaxValue;
            return;
        }

        // Don't allow interaction during fades
        bool isFading = menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f;
        if (isFading)
        {
            _isMoving = false;
            return;
        }

        Point positionBefore = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);

        // Position the move button relative to okButton
        if (menu.okButton != null)
        {
            _moveButtonBounds = new Rectangle(
                menu.okButton.bounds.X,
                menu.okButton.bounds.Y - 80,
                64,
                64
            );
        }

        // Add the move button into the menu's snappy navigation so controllers can reach it.
        TryAddMoveButtonToSnappyMenu(menu);

        // If in move mode, menu follows the mouse
        if (_isMoving)
        {
            if (Game1.options.gamepadControls)
            {
                MoveMenuWithController(menu);
            }
            else
            {
                int mouseX = Game1.getMouseX();
                int mouseY = Game1.getMouseY();

                // Calculate target menu position (mouse pos minus offset)
                int targetX = mouseX - _mouseOffset.X;
                int targetY = mouseY - _mouseOffset.Y;

                // Clamp to keep menu + side buttons fully on screen
                Point clamped = ClampMenuPosition(menu, new Point(targetX, targetY));
                targetX = clamped.X;
                targetY = clamped.Y;

                int dx = targetX - menu.xPositionOnScreen;
                int dy = targetY - menu.yPositionOnScreen;

                if (dx != 0 || dy != 0)
                    menu.movePosition(dx, dy);
            }
        }

        Point positionAfter = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
        if (_lastMenuPosition == null)
        {
            _lastMenuPosition = positionAfter;
            _ticksSinceMenuMoved = int.MaxValue;
        }
        else if (positionAfter != positionBefore)
        {
            _lastMenuPosition = positionAfter;
            _ticksSinceMenuMoved = 0;
        }
        else
        {
            if (_ticksSinceMenuMoved < int.MaxValue)
                _ticksSinceMenuMoved++;
        }
    }

    private void TryAddMoveButtonToSnappyMenu(MuseumMenu menu)
    {
        if (!Game1.options.SnappyMenus)
            return;

        bool shouldShowButton = menu.heldItem == null && Game1.player?.CursorSlotItem == null;
        if (!shouldShowButton)
        {
            if (_moveButtonComponent != null)
                _moveButtonComponent.visible = false;
            return;
        }

        if (menu.allClickableComponents == null)
            menu.populateClickableComponentList();
        menu.allClickableComponents ??= new();

        if (_moveButtonComponent == null)
        {
            _moveButtonComponent = new ClickableComponent(_moveButtonBounds, "Move UI")
            {
                myID = MoveButtonComponentId,
                downNeighborID = menu.okButton?.myID ?? 106,
                leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
                region = menu.okButton?.region ?? 0
            };
        }
        else
        {
            _moveButtonComponent.bounds = _moveButtonBounds;
            _moveButtonComponent.visible = true;
            _moveButtonComponent.downNeighborID = menu.okButton?.myID ?? 106;
            _moveButtonComponent.region = menu.okButton?.region ?? 0;
        }

        bool alreadyAdded = false;
        for (int i = 0; i < menu.allClickableComponents.Count; i++)
        {
            if (menu.allClickableComponents[i]?.myID == MoveButtonComponentId)
            {
                alreadyAdded = true;
                menu.allClickableComponents[i] = _moveButtonComponent;
                break;
            }
        }
        if (!alreadyAdded)
            menu.allClickableComponents.Add(_moveButtonComponent);

        // Make it reachable from the OK button.
        if (menu.okButton != null)
            menu.okButton.upNeighborID = MoveButtonComponentId;
    }

    private static void MoveMenuWithController(MuseumMenu menu)
    {
        GamePadState state = GamePad.GetState(PlayerIndex.One);

        int speed = state.IsButtonDown(Buttons.LeftShoulder) || state.IsButtonDown(Buttons.RightShoulder)
            ? 12
            : 6;

        int dx = 0;
        int dy = 0;

        Vector2 stick = state.ThumbSticks.Left;
        if (Math.Abs(stick.X) > 0.2f)
            dx += (int)Math.Round(stick.X * speed);
        if (Math.Abs(stick.Y) > 0.2f)
            dy += (int)Math.Round(-stick.Y * speed);

        if (state.IsButtonDown(Buttons.DPadLeft)) dx -= speed;
        if (state.IsButtonDown(Buttons.DPadRight)) dx += speed;
        if (state.IsButtonDown(Buttons.DPadUp)) dy -= speed;
        if (state.IsButtonDown(Buttons.DPadDown)) dy += speed;

        if (dx == 0 && dy == 0)
            return;

        Point clamped = ClampMenuPosition(menu, new Point(menu.xPositionOnScreen + dx, menu.yPositionOnScreen + dy));
        int targetX = clamped.X;
        int targetY = clamped.Y;

        int actualDx = targetX - menu.xPositionOnScreen;
        int actualDy = targetY - menu.yPositionOnScreen;

        if (actualDx != 0 || actualDy != 0)
            menu.movePosition(actualDx, actualDy);
    }

    private static Point ClampMenuPosition(MuseumMenu menu, Point desiredTopLeft)
    {
        Rectangle union = new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);

        if (menu.okButton != null)
            union = Rectangle.Union(union, menu.okButton.bounds);

        // The move button is owned by the mod; if it exists, it will be registered as a clickable component.
        // When it doesn't exist yet, clamping will still be correct based on the OK button.
        ClickableComponent? modMoveComponent = null;
        if (menu.allClickableComponents != null)
        {
            for (int i = 0; i < menu.allClickableComponents.Count; i++)
            {
                ClickableComponent c = menu.allClickableComponents[i];
                if (c != null && c.myID == MoveButtonComponentId)
                {
                    modMoveComponent = c;
                    break;
                }
            }
        }
        if (modMoveComponent != null)
            union = Rectangle.Union(union, modMoveComponent.bounds);

        int leftOffset = union.Left - menu.xPositionOnScreen;
        int rightOffset = union.Right - menu.xPositionOnScreen;
        int topOffset = union.Top - menu.yPositionOnScreen;
        int bottomOffset = union.Bottom - menu.yPositionOnScreen;

        int minX = -leftOffset;
        int maxX = Game1.uiViewport.Width - rightOffset;
        int minY = -topOffset;
        int maxY = Game1.uiViewport.Height - bottomOffset;

        int x = Math.Clamp(desiredTopLeft.X, minX, maxX);
        int y = Math.Clamp(desiredTopLeft.Y, minY, maxY);
        return new Point(x, y);
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't render during fades
        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        // Don't show button while holding an item
        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        int mx = Game1.getMouseX();
        int my = Game1.getMouseY();
        bool isHoveringMoveButton = _moveButtonBounds.Contains(mx, my);

        SpriteBatch b = e.SpriteBatch;

        // Draw button background (use menu texture for a less basic look)
        Color bgColor = _isMoving
            ? new Color(180, 255, 180)
            : (isHoveringMoveButton ? new Color(255, 255, 220) : Color.White);
        IClickableMenu.drawTextureBox(
            b,
            Game1.menuTexture,
            new Rectangle(0, 256, 60, 60),
            _moveButtonBounds.X,
            _moveButtonBounds.Y,
            _moveButtonBounds.Width,
            _moveButtonBounds.Height,
            bgColor,
            1f,
            drawShadow: false
        );

        // Draw a single vanilla icon (same icon used by the chest "Organize" button)
        Vector2 center = new Vector2(
            _moveButtonBounds.X + _moveButtonBounds.Width / 2f,
            _moveButtonBounds.Y + _moveButtonBounds.Height / 2f
        );

        Rectangle iconSource = new Rectangle(162, 440, 16, 16);
        float iconScale = isHoveringMoveButton ? 4.25f : 4f;
        Color iconColor = _isMoving ? Color.DarkGreen : Color.White;
        Vector2 iconPos = new Vector2(
            center.X - iconSource.Width * iconScale / 2f,
            center.Y - iconSource.Height * iconScale / 2f
        );
        Utility.drawWithShadow(b, Game1.mouseCursors, iconPos, iconSource, iconColor, 0f, Vector2.Zero, iconScale);

        // Tooltip
        if (isHoveringMoveButton && !_isMoving)
        {
            IClickableMenu.drawHoverText(b, "Move this UI", Game1.smallFont);
        }
        else if (_isMoving && _ticksSinceMenuMoved >= StationaryTicksBeforeShowingMoveTooltip)
        {
            IClickableMenu.drawHoverText(b, "Click anywhere (or press A) to place", Game1.smallFont);
        }

        // Always redraw cursor on top
        menu.drawMouse(b);
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        bool isConfirm = e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA;
        bool isCancel = e.Button == SButton.ControllerB;
        if (!isConfirm && !isCancel)
            return;

        if (Game1.activeClickableMenu is not MuseumMenu menu)
            return;

        // Don't allow during fades
        if (menu.fadeTimer > 0 || menu.blackFadeAlpha > 0f)
            return;

        // Don't allow while holding an item
        if (menu.heldItem != null || Game1.player?.CursorSlotItem != null)
            return;

        int mouseX = Game1.getMouseX();
        int mouseY = Game1.getMouseY();

        if (_isMoving && isCancel)
        {
            _isMoving = false;
            Game1.playSound("cancel");
            Helper.Input.Suppress(e.Button);
            return;
        }

        if (!isConfirm)
            return;

        if (_isMoving)
        {
            // Currently moving → click anywhere to place it
            _isMoving = false;
            _savedMenuOffset = new Point(menu.xPositionOnScreen, menu.yPositionOnScreen);
            Game1.playSound("stoneStep");
            Helper.Input.Suppress(e.Button);
        }
        else if (_moveButtonBounds.Width > 0 && _moveButtonBounds.Contains(mouseX, mouseY))
        {
            // Click on button → start moving
            _isMoving = true;
            _ticksSinceMenuMoved = 0;
            // Store offset for mouse-drag mode.
            _mouseOffset = new Point(mouseX - menu.xPositionOnScreen, mouseY - menu.yPositionOnScreen);
            Game1.playSound("bigSelect");
            Helper.Input.Suppress(e.Button);
        }
    }
}
