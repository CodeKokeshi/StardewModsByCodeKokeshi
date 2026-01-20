// Decompiled with JetBrains decompiler
// Type: StardewValley.Buffs.BuffAttributeDisplay
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;

#nullable disable
namespace StardewValley.Buffs;

/// <summary>Display info for a buff attribute shown when a buff doesn't have its own dedicated icon.</summary>
public class BuffAttributeDisplay
{
  /// <summary>The icon texture to draw.</summary>
  public readonly Func<Texture2D> Texture;
  /// <summary>The icon's sprite index within the <see cref="F:StardewValley.Buffs.BuffAttributeDisplay.Texture" />.</summary>
  public readonly int SpriteIndex;
  /// <summary>The attribute's current value.</summary>
  public readonly Func<Buff, float> Value;
  /// <summary>The attribute's translated display name.</summary>
  public readonly Func<float, string> Description;

  /// <summary>Construct an instance for a custom buff attribute.</summary>
  /// <param name="texture">The icon texture to draw.</param>
  /// <param name="spriteIndex">The icon's sprite index within the <paramref name="texture" />.</param>
  /// <param name="value">The attribute's current value.</param>
  /// <param name="description">The attribute's translated display name.</param>
  public BuffAttributeDisplay(
    Func<Texture2D> texture,
    int spriteIndex,
    Func<Buff, float> value,
    Func<float, string> description)
  {
    this.Texture = texture;
    this.SpriteIndex = spriteIndex;
    this.Value = value;
    this.Description = description;
  }

  /// <summary>Construct an instance for a standard buff attribute.</summary>
  /// <param name="spriteIndex">The icon's sprite index within <see cref="F:StardewValley.Game1.buffsIcons" />.</param>
  /// <param name="value">The attribute's current value.</param>
  /// <param name="descriptionKey">The translation key for the attribute's display name.</param>
  public BuffAttributeDisplay(
    int spriteIndex,
    Func<BuffEffects, NetFloat> value,
    string descriptionKey)
  {
    this.Texture = (Func<Texture2D>) (() => Game1.buffsIcons);
    this.SpriteIndex = spriteIndex;
    this.Value = (Func<Buff, float>) (buff => value(buff.effects).Value);
    this.Description = (Func<float, string>) (buffValue =>
    {
      string str1 = (double) buffValue > 0.0 ? "+" + buffValue.ToString() : buffValue.ToString() ?? "";
      string str2 = Game1.content.LoadString(descriptionKey);
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
        case LocalizedContentManager.LanguageCode.es:
        case LocalizedContentManager.LanguageCode.ko:
          return str2 + str1;
        default:
          return str1 + str2;
      }
    });
  }
}
