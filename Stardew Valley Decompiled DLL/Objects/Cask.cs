// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Cask
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData.Machines;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class Cask : StardewValley.Object
{
  public const int defaultDaysToMature = 56;
  [XmlElement("agingRate")]
  public readonly NetFloat agingRate = new NetFloat();
  [XmlElement("daysToMature")]
  public readonly NetFloat daysToMature = new NetFloat();

  /// <inheritdoc />
  public override string TypeDefinitionId => "(BC)";

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.agingRate, "agingRate").AddField((INetSerializable) this.daysToMature, "daysToMature");
  }

  public Cask()
  {
  }

  public Cask(Vector2 v)
    : base(v, "163")
  {
  }

  public override bool performToolAction(Tool t)
  {
    if (t == null || !t.isHeavyHitter() || t is MeleeWeapon)
      return base.performToolAction(t);
    if (this.heldObject.Value != null)
      Game1.createItemDebris((Item) this.heldObject.Value, this.tileLocation.Value * 64f, -1);
    this.playNearbySoundAll("woodWhack");
    if (this.heldObject.Value == null)
      return true;
    this.heldObject.Value = (StardewValley.Object) null;
    this.readyForHarvest.Value = false;
    this.minutesUntilReady.Value = -1;
    return false;
  }

  public virtual bool IsValidCaskLocation()
  {
    GameLocation location = this.Location;
    if (location == null)
      return false;
    return location is Cellar || location.HasMapPropertyWithValue("CanCaskHere");
  }

  /// <summary>Get the output item to produce for a cask.</summary>
  /// <inheritdoc cref="T:StardewValley.Delegates.MachineOutputDelegate" />
  public static Item OutputCask(
    StardewValley.Object machine,
    Item inputItem,
    bool probe,
    MachineItemOutput outputData,
    Farmer player,
    out int? overrideMinutesUntilReady)
  {
    overrideMinutesUntilReady = new int?();
    if (!(machine is Cask cask))
      return (Item) null;
    if (!cask.IsValidCaskLocation())
    {
      if (StardewValley.Object.autoLoadFrom == null && !probe)
        Game1.showRedMessageUsingLoadString("Strings\\Objects:CaskNoCellar");
      return (Item) null;
    }
    if (cask.quality.Value >= 4)
      return (Item) null;
    if (inputItem.Quality >= 4)
      return (Item) null;
    float result = 1f;
    string s;
    if (outputData?.CustomData != null && outputData.CustomData.TryGetValue("AgingMultiplier", out s) && (!float.TryParse(s, out result) || (double) result <= 0.0))
    {
      Game1.log.Error($"Failed to parse cask aging multiplier '{s}' for trigger rule. This must be a positive float value.");
      return (Item) null;
    }
    if ((double) result <= 0.0)
      return (Item) null;
    StardewValley.Object one = (StardewValley.Object) inputItem.getOne();
    if (probe)
      return (Item) one;
    cask.agingRate.Value = result;
    cask.daysToMature.Value = cask.GetDaysForQuality(one.Quality);
    overrideMinutesUntilReady = new int?(one.Quality >= 4 ? 1 : 999999);
    return (Item) one;
  }

  /// <inheritdoc />
  public override bool TryApplyFairyDust(bool probe = false)
  {
    if (this.heldObject.Value == null || this.heldObject.Value.Quality == 4)
      return false;
    if (!probe)
    {
      Utility.addSprinklesToLocation(this.Location, (int) this.tileLocation.X, (int) this.tileLocation.Y, 1, 2, 400, 40, Color.White);
      Game1.playSound("yoba");
      this.daysToMature.Value = this.GetDaysForQuality(this.GetNextQuality(this.heldObject.Value.Quality));
      this.checkForMaturity();
    }
    return true;
  }

  public override void DayUpdate()
  {
    base.DayUpdate();
    if (this.heldObject.Value == null)
      return;
    this.minutesUntilReady.Value = 999999;
    this.daysToMature.Value -= this.agingRate.Value;
    this.checkForMaturity();
  }

  public float GetDaysForQuality(int quality)
  {
    switch (quality)
    {
      case 1:
        return 42f;
      case 2:
        return 28f;
      case 4:
        return 0.0f;
      default:
        return 56f;
    }
  }

  public int GetNextQuality(int quality)
  {
    switch (quality)
    {
      case 1:
        return 2;
      case 2:
      case 4:
        return 4;
      default:
        return 1;
    }
  }

  public void checkForMaturity()
  {
    if ((double) this.daysToMature.Value > (double) this.GetDaysForQuality(this.GetNextQuality(this.heldObject.Value.quality.Value)))
      return;
    this.heldObject.Value.quality.Value = this.GetNextQuality(this.heldObject.Value.quality.Value);
    if (this.heldObject.Value.Quality != 4)
      return;
    this.minutesUntilReady.Value = 1;
  }

  public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
  {
    base.draw(spriteBatch, x, y, alpha);
    StardewValley.Object @object = this.heldObject.Value;
    if ((@object != null ? (@object.quality.Value > 0 ? 1 : 0) : 0) == 0)
      return;
    Vector2 vector2 = (this.MinutesUntilReady > 0 ? new Vector2(Math.Abs(this.scale.X - 5f), Math.Abs(this.scale.Y - 5f)) : Vector2.Zero) * 4f;
    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/ - 64 /*0x40*/)));
    Rectangle destinationRectangle = new Rectangle((int) ((double) local.X + 32.0 - 8.0 - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y + 64.0 + 8.0 - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (16.0 + (double) vector2.X), (int) (16.0 + (double) vector2.Y / 2.0));
    spriteBatch.Draw(Game1.mouseCursors, destinationRectangle, new Rectangle?(this.heldObject.Value.quality.Value < 4 ? new Rectangle(338 + (this.heldObject.Value.quality.Value - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8)), Color.White * 0.95f, 0.0f, Vector2.Zero, SpriteEffects.None, (float) ((y + 1) * 64 /*0x40*/) / 10000f);
  }
}
