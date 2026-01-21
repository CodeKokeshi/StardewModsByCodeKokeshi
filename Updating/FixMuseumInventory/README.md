# Fix Museum Inventory

A complete rewrite of the Stardew Valley museum donation system with two distinct modes:

## Features

### Donation Mode (Talk to Gunther)
- Shows **ONLY donatable items** in a compact, draggable inventory panel
- No clutter from your entire inventory
- Clean, focused UI for donations
- Drag the panel by the title bar to reposition it

### Arrange Mode (Inspect the Book)
- **NO inventory displayed at all**
- Pure rearrangement mode - pick up and move museum pieces
- No input interference from invisible inventory components
- Clean, distraction-free interface

## How It Works

This mod **completely replaces** the vanilla `MuseumMenu` with a custom implementation that:
- Inherits directly from `IClickableMenu` (NOT `MenuWithInventory`)
- Has full control over what's rendered and what receives input
- **Donation mode**: Triggered when talking to Gunther - creates a minimal inventory showing only donatable items
- **Arrange mode**: Triggered when inspecting the book - literally doesn't create any inventory components at all

## Controls

- **Talk to Gunther**: Opens Donation Mode with compact inventory
- **Inspect the Book**: Opens Arrange Mode with NO inventory
- **Left Click**: Pick up/place items
- **Right Click**: Return held item
- **Escape**: Close menu (must not be holding an item)
- **Drag Title Bar**: Reposition the donation inventory panel (donation mode only)

## Technical Details

The vanilla museum menu had fundamental issues:
1. Inherited from `MenuWithInventory` which auto-manages inventory
2. Inventory was always present in memory, even when "hidden"
3. Received input events and gamepad navigation even when invisible
4. Impossible to truly disable without fighting the base class

This mod solves that by:
- Intercepting `LibraryMuseum.OpenDonationMenu()` and `LibraryMuseum.OpenRearrangeMenu()`
- Creating the appropriate custom menu based on which method was called
- Building everything from scratch with complete control

## Architecture

- **CustomMuseumMenu.cs**: The entire museum UI, built on `IClickableMenu`
- **ModEntry.cs**: Harmony patches to intercept the two menu-opening methods
- No complex patches, no fighting vanilla behavior
- Clean separation: donation mode has inventory, arrange mode doesn't
