// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BuffsDisplay
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buffs;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace StardewValley.Menus;

public class BuffsDisplay : IClickableMenu
{
  /// <summary>The buff attributes shown for buffs which don't have their own icon or description.</summary>
  /// <remarks>For example, a food buff which adds +2 fishing and +1 luck will show two buff icons using this data. A buff which has its own icon but no description will show a single icon with a combined description based on this data.</remarks>
  public static readonly 
  #nullable disable
  List<BuffAttributeDisplay> displayAttributes = new List<BuffAttributeDisplay>()
  {
    new BuffAttributeDisplay(0, (Func<BuffEffects, NetFloat>) (buff => buff.FarmingLevel), "Strings\\StringsFromCSFiles:Buff.cs.480"),
    new BuffAttributeDisplay(1, (Func<BuffEffects, NetFloat>) (buff => buff.FishingLevel), "Strings\\StringsFromCSFiles:Buff.cs.483"),
    new BuffAttributeDisplay(2, (Func<BuffEffects, NetFloat>) (buff => buff.MiningLevel), "Strings\\StringsFromCSFiles:Buff.cs.486"),
    new BuffAttributeDisplay(4, (Func<BuffEffects, NetFloat>) (buff => buff.LuckLevel), "Strings\\StringsFromCSFiles:Buff.cs.489"),
    new BuffAttributeDisplay(5, (Func<BuffEffects, NetFloat>) (buff => buff.ForagingLevel), "Strings\\StringsFromCSFiles:Buff.cs.492"),
    new BuffAttributeDisplay(16 /*0x10*/, (Func<BuffEffects, NetFloat>) (buff => buff.MaxStamina), "Strings\\StringsFromCSFiles:Buff.cs.495"),
    new BuffAttributeDisplay(11, (Func<BuffEffects, NetFloat>) (buff => buff.Attack), "Strings\\StringsFromCSFiles:Buff.cs.504"),
    new BuffAttributeDisplay(8, (Func<BuffEffects, NetFloat>) (buff => buff.MagneticRadius), "Strings\\StringsFromCSFiles:Buff.cs.498"),
    new BuffAttributeDisplay(10, (Func<BuffEffects, NetFloat>) (buff => buff.Defense), "Strings\\StringsFromCSFiles:Buff.cs.501"),
    new BuffAttributeDisplay(9, (Func<BuffEffects, NetFloat>) (buff => buff.Speed), "Strings\\StringsFromCSFiles:Buff.cs.507")
  };
  private readonly Dictionary<ClickableTextureComponent, Buff> buffs = new Dictionary<ClickableTextureComponent, Buff>();
  /// <summary>The buff IDs added or renewed since the last icon render.</summary>
  public readonly HashSet<string> updatedIDs = new HashSet<string>();
  public bool dirty;
  public string hoverText = "";

  public BuffsDisplay() => this.updatePosition();

  private void updatePosition()
  {
    Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
    int num1 = 288;
    int num2 = 64 /*0x40*/;
    int num3 = titleSafeArea.Right - 300 - this.width;
    int num4 = titleSafeArea.Top + 8;
    if (num3 == this.xPositionOnScreen && num4 == this.yPositionOnScreen && num1 == this.width && num2 == this.height)
      return;
    this.xPositionOnScreen = num3;
    this.yPositionOnScreen = num4;
    this.width = num1;
    this.height = num2;
    this.resetIcons();
  }

  public override bool isWithinBounds(int x, int y)
  {
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
    {
      if (buff.Key.containsPoint(x, y))
        return true;
    }
    return false;
  }

  public int getNumBuffs() => this.buffs == null ? 0 : this.buffs.Count;

  /// <inheritdoc />
  public override void performHoverAction(int x, int y)
  {
    this.hoverText = "";
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
    {
      if (buff.Key.containsPoint(x, y))
      {
        this.hoverText = buff.Key.hoverText + (buff.Value.millisecondsDuration != -2 ? Environment.NewLine + buff.Value.getTimeLeft() : "");
        this.hoverText = string.Format(this.hoverText, (object[]) this.getBuffDescriptionTextReplacement(buff.Value.id));
        buff.Key.scale = Math.Min(buff.Key.baseScale + 0.1f, buff.Key.scale + 0.02f);
        break;
      }
    }
  }

  public string[] getBuffDescriptionTextReplacement(string buffName)
  {
    if (!(buffName == "statue_of_blessings_3"))
      return LegacyShims.EmptyArray<string>();
    return new string[1]
    {
      Game1.player.stats.Get("blessingOfWaters").ToString()
    };
  }

  public void arrangeTheseComponentsInThisRectangle(
    int rectangleX,
    int rectangleY,
    int rectangleWidthInComponentWidthUnits,
    int componentWidth,
    int componentHeight,
    int buffer,
    bool rightToLeft)
  {
    int num1 = 0;
    int num2 = 0;
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
    {
      ClickableTextureComponent key = buff.Key;
      if (rightToLeft)
        key.bounds = new Rectangle(rectangleX + rectangleWidthInComponentWidthUnits * componentWidth - (num1 + 1) * (componentWidth + buffer), rectangleY + num2 * (componentHeight + buffer), componentWidth, componentHeight);
      else
        key.bounds = new Rectangle(rectangleX + num1 * (componentWidth + buffer), rectangleY + num2 * (componentHeight + buffer), componentWidth, componentHeight);
      ++num1;
      if (num1 > rectangleWidthInComponentWidthUnits)
      {
        ++num2;
        num1 = 0;
      }
    }
  }

  protected virtual void resetIcons()
  {
    this.buffs.Clear();
    if (Game1.player == null)
      return;
    IDictionary<string, float> dictionary = (IDictionary<string, float>) new Dictionary<string, float>();
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
      dictionary[buff.Value.id] = buff.Key.scale;
    foreach (Buff sortedBuff in this.GetSortedBuffs())
    {
      if (sortedBuff.visible)
      {
        bool flag = this.updatedIDs.Contains(sortedBuff.id);
        foreach (ClickableTextureComponent clickableComponent in this.getClickableComponents(sortedBuff))
        {
          if (flag)
          {
            clickableComponent.scale = clickableComponent.baseScale + 0.2f;
          }
          else
          {
            float val2;
            if (dictionary.TryGetValue(sortedBuff.id, out val2))
              clickableComponent.scale = Math.Max(clickableComponent.baseScale, val2);
          }
          this.buffs.Add(clickableComponent, sortedBuff);
        }
      }
    }
    this.updatedIDs.Clear();
    this.arrangeTheseComponentsInThisRectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width / 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/, 8, true);
  }

  public new void update(GameTime time)
  {
    if (this.dirty)
    {
      this.resetIcons();
      this.dirty = false;
    }
    if (!Game1.wasMouseVisibleThisFrame)
      this.hoverText = "";
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff1 in this.buffs)
    {
      ClickableTextureComponent key = buff1.Key;
      Buff buff2 = buff1.Value;
      key.scale = Math.Max(key.baseScale, key.scale - 0.01f);
      if (!buff2.alreadyUpdatedIconAlpha && (double) buff2.millisecondsDuration < (double) Math.Min(10000f, (float) buff2.totalMillisecondsDuration / 10f) && buff2.millisecondsDuration != -2)
      {
        buff2.displayAlphaTimer += (float) (Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / ((double) buff2.millisecondsDuration < (double) Math.Min(2000f, (float) buff2.totalMillisecondsDuration / 20f) ? 1.0 : 2.0));
        buff2.alreadyUpdatedIconAlpha = true;
      }
    }
  }

  /// <inheritdoc />
  public override void draw(SpriteBatch b)
  {
    this.updatePosition();
    foreach (KeyValuePair<ClickableTextureComponent, Buff> buff in this.buffs)
    {
      buff.Key.draw(b, Color.White * ((double) buff.Value.displayAlphaTimer > 0.0 ? (float) ((Math.Cos((double) buff.Value.displayAlphaTimer / 100.0) + 3.0) / 4.0) : 1f), 0.8f);
      buff.Value.alreadyUpdatedIconAlpha = false;
    }
    if (this.hoverText.Length == 0 || !this.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
      return;
    this.performHoverAction(Game1.getOldMouseX(), Game1.getOldMouseY());
    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
  }

  public IEnumerable<Buff> GetSortedBuffs()
  {
    return (IEnumerable<Buff>) Game1.player.buffs.AppliedBuffs.Values.OrderByDescending<Buff, bool>((Func<Buff, bool>) (p => p.id == "food")).ThenByDescending<Buff, bool>((Func<Buff, bool>) (p => p.id == "drink"));
  }

  protected virtual string getDescription(Buff buff)
  {
    StringBuilder stringBuilder = new StringBuilder();
    string displayName = buff.displayName;
    if ((displayName != null ? (displayName.Length > 1 ? 1 : 0) : 0) != 0)
    {
      stringBuilder.AppendLine(buff.displayName);
      stringBuilder.AppendLine("[line]");
    }
    string description1 = buff.description;
    if ((description1 != null ? (description1.Length > 1 ? 1 : 0) : 0) != 0)
      stringBuilder.AppendLine(buff.description);
    foreach (BuffAttributeDisplay displayAttribute in BuffsDisplay.displayAttributes)
    {
      string description2 = this.getDescription(buff, displayAttribute, false);
      if (description2 != null)
        stringBuilder.AppendLine(description2);
    }
    string sourceLine = this.getSourceLine(buff);
    if (sourceLine != null)
      stringBuilder.AppendLine(sourceLine);
    return stringBuilder.ToString().TrimEnd();
  }

  protected virtual string getDescription(
    Buff buff,
    BuffAttributeDisplay attribute,
    bool withSource)
  {
    float num = attribute.Value(buff);
    if ((double) num == 0.0)
      return (string) null;
    string description = attribute.Description(num);
    if (withSource)
    {
      string sourceLine = this.getSourceLine(buff);
      if (sourceLine != null)
        description = $"{description}\n{sourceLine}";
    }
    return description;
  }

  protected virtual string getSourceLine(Buff buff)
  {
    string str = buff.displaySource ?? buff.source;
    return string.IsNullOrWhiteSpace(str) ? (string) null : Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.508") + str;
  }

  public virtual IEnumerable<ClickableTextureComponent> getClickableComponents(Buff buff)
  {
    if (buff.visible)
    {
      if (buff.iconTexture != null)
      {
        Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(buff.iconTexture, buff.iconSheetIndex, 16 /*0x10*/, 16 /*0x10*/);
        yield return new ClickableTextureComponent("", Rectangle.Empty, (string) null, this.getDescription(buff), buff.iconTexture, standardTileSheet, 4f);
      }
      else
      {
        foreach (BuffAttributeDisplay displayAttribute in BuffsDisplay.displayAttributes)
        {
          string description = this.getDescription(buff, displayAttribute, true);
          if (description != null)
          {
            Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(displayAttribute.Texture(), displayAttribute.SpriteIndex, 16 /*0x10*/, 16 /*0x10*/);
            yield return new ClickableTextureComponent("", Rectangle.Empty, (string) null, description, displayAttribute.Texture(), standardTileSheet, 4f);
          }
        }
      }
    }
  }
}
