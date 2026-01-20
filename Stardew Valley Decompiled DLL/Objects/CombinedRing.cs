// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.CombinedRing
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Monsters;

#nullable disable
namespace StardewValley.Objects;

public class CombinedRing : Ring
{
  public NetList<Ring, NetRef<Ring>> combinedRings = new NetList<Ring, NetRef<Ring>>();

  public CombinedRing()
    : base("880")
  {
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.combinedRings, "combinedRings");
    this.combinedRings.OnElementChanged += (NetList<Ring, NetRef<Ring>>.ElementChangedEvent) ((_param1, _param2, _param3, _param4) => this.OnCombinedRingsChanged());
    this.combinedRings.OnArrayReplaced += (NetList<Ring, NetRef<Ring>>.ArrayReplacedEvent) ((_param1, _param2, _param3) => this.OnCombinedRingsChanged());
  }

  protected override bool loadDisplayFields()
  {
    base.loadDisplayFields();
    this.description = "";
    foreach (Ring combinedRing in this.combinedRings)
    {
      combinedRing.getDescription();
      this.description = $"{this.description}{combinedRing.description}\n\n";
    }
    this.description = this.description.Trim();
    return true;
  }

  public override bool GetsEffectOfRing(string ringId)
  {
    foreach (Ring combinedRing in this.combinedRings)
    {
      if (combinedRing.GetsEffectOfRing(ringId))
        return true;
    }
    return base.GetsEffectOfRing(ringId);
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new CombinedRing();

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is CombinedRing combinedRing1))
      return;
    this.combinedRings.Clear();
    foreach (Item combinedRing2 in combinedRing1.combinedRings)
      this.combinedRings.Add((Ring) combinedRing2.getOne());
  }

  public override int GetEffectsOfRingMultiplier(string ringId)
  {
    int ofRingMultiplier = 0;
    foreach (Ring combinedRing in this.combinedRings)
      ofRingMultiplier += combinedRing.GetEffectsOfRingMultiplier(ringId);
    return ofRingMultiplier;
  }

  /// <inheritdoc />
  public override void onEquip(Farmer who)
  {
    foreach (Item combinedRing in this.combinedRings)
      combinedRing.onEquip(who);
    base.onEquip(who);
  }

  /// <inheritdoc />
  public override void onUnequip(Farmer who)
  {
    foreach (Item combinedRing in this.combinedRings)
      combinedRing.onUnequip(who);
    base.onUnequip(who);
  }

  public override void AddEquipmentEffects(BuffEffects effects)
  {
    base.AddEquipmentEffects(effects);
    foreach (Item combinedRing in this.combinedRings)
      combinedRing.AddEquipmentEffects(effects);
  }

  public override void onLeaveLocation(Farmer who, GameLocation environment)
  {
    foreach (Ring combinedRing in this.combinedRings)
      combinedRing.onLeaveLocation(who, environment);
    base.onLeaveLocation(who, environment);
  }

  /// <inheritdoc />
  public override void onMonsterSlay(Monster m, GameLocation location, Farmer who)
  {
    foreach (Ring combinedRing in this.combinedRings)
      combinedRing.onMonsterSlay(m, location, who);
    base.onMonsterSlay(m, location, who);
  }

  public override void onNewLocation(Farmer who, GameLocation environment)
  {
    foreach (Ring combinedRing in this.combinedRings)
      combinedRing.onNewLocation(who, environment);
    base.onNewLocation(who, environment);
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
    if (this.combinedRings.Count >= 2)
    {
      this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
      float num = scaleSize;
      scaleSize = 1f;
      location.Y -= (float) (((double) num - 1.0) * 32.0);
      ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(this.combinedRings[0].QualifiedItemId);
      Texture2D texture1 = dataOrErrorItem1.GetTexture();
      Rectangle rectangle1 = dataOrErrorItem1.GetSourceRect().Clone();
      rectangle1.X += 5;
      rectangle1.Y += 7;
      rectangle1.Width = 4;
      rectangle1.Height = 6;
      spriteBatch.Draw(texture1, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-12f, 8f) * scaleSize, new Rectangle?(rectangle1), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      ++rectangle1.X;
      rectangle1.Y += 4;
      rectangle1.Width = 3;
      rectangle1.Height = 1;
      spriteBatch.Draw(texture1, location + new Vector2(51f, 51f) * scaleSize + new Vector2(-8f, 4f) * scaleSize, new Rectangle?(rectangle1), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem(this.combinedRings[1].QualifiedItemId);
      Texture2D texture2 = dataOrErrorItem2.GetTexture();
      Rectangle rectangle2 = dataOrErrorItem2.GetSourceRect().Clone();
      rectangle2.X += 9;
      rectangle2.Y += 7;
      rectangle2.Width = 4;
      rectangle2.Height = 6;
      spriteBatch.Draw(texture2, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 8f) * scaleSize, new Rectangle?(rectangle2), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      rectangle2.Y += 4;
      rectangle2.Width = 3;
      rectangle2.Height = 1;
      spriteBatch.Draw(texture2, location + new Vector2(51f, 51f) * scaleSize + new Vector2(4f, 4f) * scaleSize, new Rectangle?(rectangle2), color * transparency, 0.0f, new Vector2(1.5f, 2f) * 4f * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      Color? dyeColor1 = TailoringMenu.GetDyeColor((Item) this.combinedRings[0]);
      Color? dyeColor2 = TailoringMenu.GetDyeColor((Item) this.combinedRings[1]);
      Color red = Color.Red;
      Color blue = Color.Blue;
      if (dyeColor1.HasValue)
        red = dyeColor1.Value;
      if (dyeColor2.HasValue)
        blue = dyeColor2.Value;
      base.drawInMenu(spriteBatch, location + new Vector2(-5f, -1f), scaleSize, transparency, layerDepth, drawStackNumber, Utility.Get2PhaseColor(red, blue), drawShadow);
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(13f, 35f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 1125f) * transparency, -1.57079637f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(49f, 35f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 375f) * transparency, 1.57079637f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(31f, 53f) * scaleSize, new Rectangle?(new Rectangle(263, 579, 4, 2)), Utility.Get2PhaseColor(red, blue, timeOffset: 750f) * transparency, 3.14159274f, new Vector2(2f, 1.5f) * scaleSize, scaleSize * 4f, SpriteEffects.None, layerDepth);
      this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
    }
    else
      base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
  }

  public override void update(GameTime time, GameLocation environment, Farmer who)
  {
    foreach (Ring combinedRing in this.combinedRings)
      combinedRing.update(time, environment, who);
    base.update(time, environment, who);
  }

  /// <summary>Update data when the <see cref="F:StardewValley.Objects.CombinedRing.combinedRings" /> list changes.</summary>
  protected virtual void OnCombinedRingsChanged() => this.description = (string) null;
}
