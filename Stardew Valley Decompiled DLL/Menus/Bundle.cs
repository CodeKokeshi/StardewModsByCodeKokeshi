// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Bundle
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.Internal;
using StardewValley.Locations;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley.Menus;

public class Bundle : ClickableComponent
{
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the internal name.</summary>
  public const int NameIndex = 0;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the reward data.</summary>
  public const int RewardIndex = 1;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the items needed to complete the bundle.</summary>
  public const int IngredientsIndex = 2;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the bundle color.</summary>
  public const int ColorIndex = 3;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the optional number of slots to fill.</summary>
  public const int NumberOfSlotsIndex = 4;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the optional override texture name and sprite index.</summary>
  public const int SpriteIndex = 5;
  /// <summary>The index in the raw <c>Data/Bundles</c> data for the display name.</summary>
  public const int DisplayNameIndex = 6;
  /// <summary>The number of slash-delimited fields in the raw <c>Data/Bundles</c> data.</summary>
  public const int FieldCount = 7;
  public const float shakeRate = 0.0157079641f;
  public const float shakeDecayRate = 0.00306796166f;
  public const int Color_Green = 0;
  public const int Color_Purple = 1;
  public const int Color_Orange = 2;
  public const int Color_Yellow = 3;
  public const int Color_Red = 4;
  public const int Color_Blue = 5;
  public const int Color_Teal = 6;
  public const float DefaultShakeForce = 0.07363108f;
  public string rewardDescription;
  public List<BundleIngredientDescription> ingredients;
  public int bundleColor;
  public int numberOfIngredientSlots;
  public int bundleIndex;
  public int completionTimer;
  public bool complete;
  public bool depositsAllowed = true;
  public Texture2D bundleTextureOverride;
  public int bundleTextureIndexOverride = -1;
  public TemporaryAnimatedSprite sprite;
  private float maxShake;
  private bool shakeLeft;

  public Bundle(
    string name,
    string displayName,
    List<BundleIngredientDescription> ingredients,
    bool[] completedIngredientsList,
    string rewardListString = "")
    : base(new Rectangle(0, 0, 64 /*0x40*/, 64 /*0x40*/), "")
  {
    this.name = name;
    this.label = displayName;
    this.rewardDescription = rewardListString;
    this.numberOfIngredientSlots = ingredients.Count;
    this.ingredients = ingredients;
  }

  public Bundle(
    int bundleIndex,
    string rawBundleInfo,
    bool[] completedIngredientsList,
    Point position,
    string textureName,
    JunimoNoteMenu menu)
    : base(new Rectangle(position.X, position.Y, 64 /*0x40*/, 64 /*0x40*/), "")
  {
    if (menu != null && menu.fromGameMenu)
      this.depositsAllowed = false;
    this.bundleIndex = bundleIndex;
    string[] array = rawBundleInfo.Split('/');
    this.name = array[0];
    this.label = array[6];
    this.rewardDescription = array[1];
    if (!string.IsNullOrWhiteSpace(array[5]))
    {
      try
      {
        string[] strArray = array[5].Split(':', 2);
        if (strArray.Length == 2)
        {
          this.bundleTextureOverride = Game1.content.Load<Texture2D>(strArray[0]);
          this.bundleTextureIndexOverride = int.Parse(strArray[1]);
        }
        else
          this.bundleTextureIndexOverride = int.Parse(array[5]);
      }
      catch
      {
        this.bundleTextureOverride = (Texture2D) null;
        this.bundleTextureIndexOverride = -1;
      }
    }
    string[] strArray1 = ArgUtility.SplitBySpace(array[2]);
    this.complete = true;
    this.ingredients = new List<BundleIngredientDescription>();
    int num = 0;
    for (int index = 0; index < strArray1.Length; index += 3)
    {
      this.ingredients.Add(new BundleIngredientDescription(strArray1[index], Convert.ToInt32(strArray1[index + 1]), Convert.ToInt32(strArray1[index + 2]), completedIngredientsList[index / 3]));
      if (!completedIngredientsList[index / 3])
        this.complete = false;
      else
        ++num;
    }
    this.bundleColor = Convert.ToInt32(array[3]);
    this.numberOfIngredientSlots = ArgUtility.GetInt(array, 4, this.ingredients.Count);
    if (num >= this.numberOfIngredientSlots)
      this.complete = true;
    this.sprite = new TemporaryAnimatedSprite(textureName, new Rectangle(this.bundleColor * 256 /*0x0100*/ % 512 /*0x0200*/, 244 + this.bundleColor * 256 /*0x0100*/ / 512 /*0x0200*/ * 16 /*0x10*/, 16 /*0x10*/, 16 /*0x10*/), 70f, 3, 99999, new Vector2((float) this.bounds.X, (float) this.bounds.Y), false, false, 0.8f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
    {
      pingPong = true
    };
    this.sprite.paused = true;
    this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
    if (this.name.ContainsIgnoreCase(Game1.currentSeason) && !this.complete)
      this.shake();
    if (!this.complete)
      return;
    this.completionAnimation(menu, false);
  }

  public Item getReward()
  {
    return Utility.getItemFromStandardTextDescription(this.rewardDescription, Game1.player);
  }

  public void shake(float force = 0.07363108f)
  {
    if (!this.sprite.paused)
      return;
    this.maxShake = force;
  }

  public void shake(int extraInfo)
  {
    this.maxShake = 3f * (float) Math.PI / 128f;
    if (extraInfo != 1)
      return;
    Game1.playSound("leafrustle");
    TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite(50, this.sprite.position, Bundle.getColorFromColorIndex(this.bundleColor))
    {
      motion = new Vector2(-1f, 0.5f),
      acceleration = new Vector2(0.0f, 0.02f)
    };
    ++temporaryAnimatedSprite1.sourceRect.Y;
    --temporaryAnimatedSprite1.sourceRect.Height;
    JunimoNoteMenu.tempSprites.Add(temporaryAnimatedSprite1);
    TemporaryAnimatedSprite temporaryAnimatedSprite2 = new TemporaryAnimatedSprite(50, this.sprite.position, Bundle.getColorFromColorIndex(this.bundleColor))
    {
      motion = new Vector2(1f, 0.5f),
      acceleration = new Vector2(0.0f, 0.02f),
      flipped = true,
      delayBeforeAnimationStart = 50
    };
    ++temporaryAnimatedSprite2.sourceRect.Y;
    --temporaryAnimatedSprite2.sourceRect.Height;
    JunimoNoteMenu.tempSprites.Add(temporaryAnimatedSprite2);
  }

  public void tryHoverAction(int x, int y)
  {
    if (this.bounds.Contains(x, y) && !this.complete)
    {
      this.sprite.paused = false;
      JunimoNoteMenu.hoverText = Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", (object) this.label);
    }
    else
    {
      if (this.complete)
        return;
      this.sprite.reset();
      this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
      this.sprite.paused = true;
    }
  }

  public bool IsValidItemForThisIngredientDescription(
    Item item,
    BundleIngredientDescription ingredient)
  {
    if (item == null || ingredient.completed || ingredient.quality > item.Quality)
      return false;
    if (ingredient.preservesId != null)
    {
      ItemQueryContext context = new ItemQueryContext(Game1.currentLocation, Game1.player, Game1.random, "query 'FLAVORED_ITEM'");
      return ((IEnumerable<ItemQueryResult>) ItemQueryResolver.TryResolve($"FLAVORED_ITEM {ingredient.id} {ingredient.preservesId}", context)).FirstOrDefault<ItemQueryResult>()?.Item is StardewValley.Object object1 && item is StardewValley.Object object2 && object2.preservedParentSheetIndex.Value != null && item.QualifiedItemId == object1.QualifiedItemId && object2.preservedParentSheetIndex.Value.Contains(ingredient.preservesId);
    }
    if (!ingredient.category.HasValue)
      return ItemRegistry.HasItemId(item, ingredient.id);
    if (item.QualifiedItemId == "(O)107" && ingredient.category.GetValueOrDefault() == -5)
      return true;
    int category1 = item.Category;
    int? category2 = ingredient.category;
    int valueOrDefault = category2.GetValueOrDefault();
    return category1 == valueOrDefault & category2.HasValue;
  }

  public int GetBundleIngredientDescriptionIndexForItem(Item item)
  {
    for (int index = 0; index < this.ingredients.Count; ++index)
    {
      if (this.IsValidItemForThisIngredientDescription(item, this.ingredients[index]))
        return index;
    }
    return -1;
  }

  public bool canAcceptThisItem(Item item, ClickableTextureComponent slot)
  {
    return this.canAcceptThisItem(item, slot, false);
  }

  public bool canAcceptThisItem(Item item, ClickableTextureComponent slot, bool ignore_stack_count = false)
  {
    if (!this.depositsAllowed)
      return false;
    for (int index = 0; index < this.ingredients.Count; ++index)
    {
      if (this.IsValidItemForThisIngredientDescription(item, this.ingredients[index]) && (ignore_stack_count || this.ingredients[index].stack <= item.Stack) && slot?.item == null)
        return true;
    }
    return false;
  }

  public Item tryToDepositThisItem(
    Item item,
    ClickableTextureComponent slot,
    string noteTextureName,
    JunimoNoteMenu parentMenu)
  {
    if (!this.depositsAllowed)
    {
      if (Game1.player.hasCompletedCommunityCenter())
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtAJM"));
      else
        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtCC"));
      return item;
    }
    CommunityCenter communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
    for (int index1 = 0; index1 < this.ingredients.Count; ++index1)
    {
      BundleIngredientDescription ingredientDescription1 = this.ingredients[index1];
      if (this.IsValidItemForThisIngredientDescription(item, ingredientDescription1) && slot.item == null)
      {
        item = item.ConsumeStack(ingredientDescription1.stack);
        List<BundleIngredientDescription> ingredients = this.ingredients;
        int index2 = index1;
        ingredientDescription1 = new BundleIngredientDescription(ingredientDescription1, true);
        BundleIngredientDescription ingredientDescription2 = ingredientDescription1;
        ingredients[index2] = ingredientDescription2;
        this.ingredientDepositAnimation(slot, noteTextureName);
        string representativeItemId = JunimoNoteMenu.GetRepresentativeItemId(ingredientDescription1);
        if (ingredientDescription1.preservesId != null)
          slot.item = Utility.CreateFlavoredItem(ingredientDescription1.id, ingredientDescription1.preservesId, ingredientDescription1.quality, ingredientDescription1.stack);
        else
          slot.item = ItemRegistry.Create(representativeItemId, ingredientDescription1.stack, ingredientDescription1.quality);
        Game1.playSound("newArtifact");
        slot.sourceRect.X = 512 /*0x0200*/;
        slot.sourceRect.Y = 244;
        if (parentMenu.onIngredientDeposit != null)
        {
          parentMenu.onIngredientDeposit(index1);
          break;
        }
        communityCenter.bundles.FieldDict[this.bundleIndex][index1] = true;
        Game1.multiplayer.globalChatInfoMessage("BundleDonate", Game1.player.displayName, TokenStringBuilder.ItemNameFor(slot.item));
        break;
      }
    }
    return item;
  }

  public void ingredientDepositAnimation(
    ClickableTextureComponent slot,
    string noteTextureName,
    bool skipAnimation = false)
  {
    TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(noteTextureName, new Rectangle(530, 244, 18, 18), 50f, 6, 1, new Vector2((float) slot.bounds.X, (float) slot.bounds.Y), false, false, 0.88f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
    {
      holdLastFrame = true,
      endSound = "cowboy_monsterhit"
    };
    if (skipAnimation)
    {
      temporaryAnimatedSprite.sourceRect.Offset(temporaryAnimatedSprite.sourceRect.Width * 5, 0);
      temporaryAnimatedSprite.sourceRectStartingPos = new Vector2((float) temporaryAnimatedSprite.sourceRect.X, (float) temporaryAnimatedSprite.sourceRect.Y);
      temporaryAnimatedSprite.animationLength = 1;
    }
    JunimoNoteMenu.tempSprites.Add(temporaryAnimatedSprite);
  }

  public bool canBeClicked() => !this.complete;

  public void completionAnimation(JunimoNoteMenu menu, bool playSound = true, int delay = 0)
  {
    if (delay <= 0)
      this.completionAnimation(playSound);
    else
      this.completionTimer = delay;
  }

  private void completionAnimation(bool playSound = true)
  {
    if (Game1.activeClickableMenu is JunimoNoteMenu activeClickableMenu)
      activeClickableMenu.takeDownBundleSpecificPage();
    this.sprite.pingPong = false;
    this.sprite.paused = false;
    this.sprite.sourceRect.X = (int) this.sprite.sourceRectStartingPos.X;
    this.sprite.sourceRect.X += this.sprite.sourceRect.Width;
    this.sprite.animationLength = 15;
    this.sprite.interval = 50f;
    this.sprite.totalNumberOfLoops = 0;
    this.sprite.holdLastFrame = true;
    this.sprite.endFunction = new TemporaryAnimatedSprite.endBehavior(this.shake);
    this.sprite.extraInfoForEndBehavior = 1;
    if (this.complete)
    {
      this.sprite.sourceRect.X += this.sprite.sourceRect.Width * 14;
      this.sprite.sourceRectStartingPos = new Vector2((float) this.sprite.sourceRect.X, (float) this.sprite.sourceRect.Y);
      this.sprite.currentParentTileIndex = 14;
      this.sprite.interval = 0.0f;
      this.sprite.animationLength = 1;
      this.sprite.extraInfoForEndBehavior = 0;
    }
    else
    {
      if (playSound)
        Game1.playSound("dwop");
      this.bounds.Inflate(64 /*0x40*/, 64 /*0x40*/);
      JunimoNoteMenu.tempSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(this.bounds, 8, Bundle.getColorFromColorIndex(this.bundleColor) * 0.5f));
      this.bounds.Inflate(-64, -64);
    }
    this.complete = true;
  }

  public void update(GameTime time)
  {
    this.sprite.update(time);
    if (this.completionTimer > 0 && JunimoNoteMenu.screenSwipe == null)
    {
      this.completionTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.completionTimer <= 0)
        this.completionAnimation();
    }
    if (Game1.random.NextDouble() < 0.005 && (this.complete || this.name.ContainsIgnoreCase(Game1.currentSeason)))
      this.shake();
    if ((double) this.maxShake > 0.0)
    {
      if (this.shakeLeft)
      {
        this.sprite.rotation -= (float) Math.PI / 200f;
        if ((double) this.sprite.rotation <= -(double) this.maxShake)
          this.shakeLeft = false;
      }
      else
      {
        this.sprite.rotation += (float) Math.PI / 200f;
        if ((double) this.sprite.rotation >= (double) this.maxShake)
          this.shakeLeft = true;
      }
    }
    if ((double) this.maxShake <= 0.0)
      return;
    this.maxShake = Math.Max(0.0f, this.maxShake - 0.0007669904f);
  }

  public void draw(SpriteBatch b) => this.sprite.draw(b, true);

  public static Color getColorFromColorIndex(int color)
  {
    switch (color)
    {
      case 0:
        return Color.Lime;
      case 1:
        return Color.DeepPink;
      case 2:
        return Color.Orange;
      case 3:
        return Color.Orange;
      case 4:
        return Color.Red;
      case 5:
        return Color.LightBlue;
      case 6:
        return Color.Cyan;
      default:
        return Color.Lime;
    }
  }
}
