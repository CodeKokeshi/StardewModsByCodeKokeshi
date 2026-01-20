// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.Stable
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Characters;
using StardewValley.Util;
using System;

#nullable disable
namespace StardewValley.Buildings;

public class Stable : Building
{
  public Guid HorseId
  {
    get => this.id.Value;
    set => this.id.Value = value;
  }

  public Stable()
    : this(Vector2.Zero)
  {
  }

  public Stable(Vector2 tileLocation)
    : this(tileLocation, GuidHelper.NewGuid())
  {
  }

  public Stable(Vector2 tileLocation, Guid horseId)
    : base(nameof (Stable), tileLocation)
  {
    this.HorseId = horseId;
  }

  public override Rectangle? getSourceRectForMenu()
  {
    return new Rectangle?(new Rectangle(0, 0, this.texture.Value.Bounds.Width, this.texture.Value.Bounds.Height));
  }

  public Horse getStableHorse() => Utility.findHorse(this.HorseId);

  /// <summary>Get the default tile position for a horse in this stable.</summary>
  public Point GetDefaultHorseTile() => new Point(this.tileX.Value + 1, this.tileY.Value + 1);

  public virtual void grabHorse()
  {
    if (this.daysOfConstructionLeft.Value > 0)
      return;
    Horse character = Utility.findHorse(this.HorseId);
    Point defaultHorseTile = this.GetDefaultHorseTile();
    if (character == null)
    {
      character = new Horse(this.HorseId, defaultHorseTile.X, defaultHorseTile.Y);
      this.GetParentLocation().characters.Add((NPC) character);
    }
    else
      Game1.warpCharacter((NPC) character, this.parentLocationName.Value, defaultHorseTile);
    character.ownerId.Value = this.owner.Value;
  }

  public virtual void updateHorseOwnership()
  {
    if (this.daysOfConstructionLeft.Value > 0)
      return;
    Horse horse = Utility.findHorse(this.HorseId);
    if (horse == null)
      return;
    horse.ownerId.Value = this.owner.Value;
    if (horse.getOwner() == null)
      return;
    if (horse.getOwner().horseName.Value != null)
    {
      horse.name.Value = horse.getOwner().horseName.Value;
      horse.displayName = horse.getOwner().horseName.Value;
    }
    else
    {
      horse.name.Value = "";
      horse.displayName = "";
    }
  }

  public override void dayUpdate(int dayOfMonth)
  {
    base.dayUpdate(dayOfMonth);
    this.grabHorse();
  }

  /// <inheritdoc />
  public override void performActionOnDemolition(GameLocation location)
  {
    base.performActionOnDemolition(location);
    Horse stableHorse = this.getStableHorse();
    stableHorse?.currentLocation?.characters.Remove((NPC) stableHorse);
  }
}
