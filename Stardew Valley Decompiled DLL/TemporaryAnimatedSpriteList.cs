// Decompiled with JetBrains decompiler
// Type: StardewValley.TemporaryAnimatedSpriteList
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class TemporaryAnimatedSpriteList : 
  IList<TemporaryAnimatedSprite>,
  ICollection<TemporaryAnimatedSprite>,
  IEnumerable<TemporaryAnimatedSprite>,
  IEnumerable
{
  public List<TemporaryAnimatedSprite> AnimatedSprites = new List<TemporaryAnimatedSprite>();

  public TemporaryAnimatedSprite this[int index]
  {
    get => this.AnimatedSprites[index];
    set => this.AnimatedSprites[index] = value;
  }

  public void AddRange(IEnumerable<TemporaryAnimatedSprite> values)
  {
    this.AnimatedSprites.AddRange(values);
  }

  public int Count => this.AnimatedSprites.Count;

  public bool IsReadOnly => false;

  public void Add(TemporaryAnimatedSprite item) => this.AnimatedSprites.Add(item);

  public void Clear()
  {
    foreach (TemporaryAnimatedSprite animatedSprite in this.AnimatedSprites)
    {
      if (animatedSprite.Pooled)
        animatedSprite.Pool();
    }
    this.AnimatedSprites.Clear();
  }

  public bool Contains(TemporaryAnimatedSprite item) => this.AnimatedSprites.Contains(item);

  public void CopyTo(TemporaryAnimatedSprite[] array, int index)
  {
    this.AnimatedSprites.CopyTo(array, index);
  }

  public IEnumerator<TemporaryAnimatedSprite> GetEnumerator()
  {
    return (IEnumerator<TemporaryAnimatedSprite>) this.AnimatedSprites.GetEnumerator();
  }

  public int IndexOf(TemporaryAnimatedSprite item) => this.AnimatedSprites.IndexOf(item);

  public void Insert(int index, TemporaryAnimatedSprite item)
  {
    this.AnimatedSprites.Insert(index, item);
  }

  public bool Remove(TemporaryAnimatedSprite item)
  {
    if (!this.AnimatedSprites.Remove(item))
      return false;
    if (item.Pooled)
      item.Pool();
    return true;
  }

  public void RemoveAt(int index)
  {
    TemporaryAnimatedSprite animatedSprite = this.AnimatedSprites[index];
    this.AnimatedSprites.RemoveAt(index);
    if (!animatedSprite.Pooled)
      return;
    animatedSprite.Pool();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
