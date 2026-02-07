#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace StardewUI.Framework;

/// <summary>
/// Public API for StardewUI, abstracting away all implementation details of views and trees.
/// </summary>
public interface IViewEngine
{
    /// <summary>
    /// Creates a drawable (non-interactive HUD element) from a StarML asset (.sml file).
    /// </summary>
    /// <param name="assetName">The asset name for the view, e.g. Mods/MyMod/Views/MyView.</param>
    /// <returns>A drawable view that can be rendered to SpriteBatch.</returns>
    IViewDrawable CreateDrawableFromAsset(string assetName);

    /// <summary>
    /// Creates a drawable from inline StarML markup. Mainly for testing.
    /// </summary>
    IViewDrawable CreateDrawableFromMarkup(string markup);

    /// <summary>
    /// Creates a menu controller with full customization from a StarML asset.
    /// </summary>
    IMenuController CreateMenuControllerFromAsset(string assetName, object? context = null);

    /// <summary>
    /// Creates a menu controller from inline StarML markup.
    /// </summary>
    IMenuController CreateMenuControllerFromMarkup(string markup, object? context = null);

    /// <summary>
    /// Creates a full-screen clickable menu from a StarML asset.
    /// </summary>
    IClickableMenu CreateMenuFromAsset(string assetName, object? context = null);

    /// <summary>
    /// Creates a full-screen clickable menu from inline StarML markup.
    /// </summary>
    IClickableMenu CreateMenuFromMarkup(string markup, object? context = null);

    /// <summary>
    /// Enables hot-reloading of views for development.
    /// </summary>
    void EnableHotReloading(string? sourceDirectory = null);

    /// <summary>
    /// Preloads all registered view and sprite assets in the background.
    /// </summary>
    void PreloadAssets();

    /// <summary>
    /// Pre-reflects the specified model types to speed up first menu open.
    /// </summary>
    void PreloadModels(params Type[] types);

    /// <summary>
    /// Registers a directory of custom data assets.
    /// </summary>
    void RegisterCustomData(string assetPrefix, string modDirectory);

    /// <summary>
    /// Registers a directory of sprite assets.
    /// </summary>
    void RegisterSprites(string assetPrefix, string modDirectory);

    /// <summary>
    /// Registers a directory of StarML view assets.
    /// </summary>
    void RegisterViews(string assetPrefix, string modDirectory);
}

/// <summary>
/// A drawable view for rendering to SpriteBatch (HUD elements, non-interactive).
/// </summary>
public interface IViewDrawable : IDisposable
{
    Vector2 ActualSize { get; }
    object? Context { get; set; }
    Vector2? MaxSize { get; set; }
    void Draw(SpriteBatch b, Vector2 position);
}

/// <summary>
/// Controller for a StardewUI menu with advanced customization options.
/// </summary>
public interface IMenuController : IDisposable
{
    event Action Closed;
    event Action Closing;

    Func<bool>? CanClose { get; set; }
    Action? CloseAction { get; set; }
    Vector2 CloseButtonOffset { get; set; }
    bool CloseOnOutsideClick { get; set; }
    string CloseSound { get; set; }
    float DimmingAmount { get; set; }
    IClickableMenu Menu { get; }
    Func<Point>? PositionSelector { get; set; }

    void ClearCursorAttachment();
    void Close();
    void EnableCloseButton(Texture2D? texture = null, Rectangle? sourceRect = null, float scale = 4f);
    void SetCursorAttachment(Texture2D texture, Rectangle? sourceRect = null, Point? size = null, Point? offset = null, Color? tint = null);
    void SetGutters(int left, int top, int right = -1, int bottom = -1);
}
