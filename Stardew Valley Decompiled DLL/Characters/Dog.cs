// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Dog
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley.Characters;

/// <summary>Obsolete. This is only kept to preserve data from old save files. All dogs now use the <see cref="T:StardewValley.Characters.Pet" /> class instead.</summary>
[Obsolete("All dogs now use the Pet class.")]
public class Dog : Pet
{
  public Dog()
  {
    this.Sprite = new AnimatedSprite(this.getPetTextureName(), 0, 32 /*0x20*/, 32 /*0x20*/);
    this.HideShadow = true;
    this.Breather = false;
    this.willDestroyObjectsUnderfoot = false;
  }
}
