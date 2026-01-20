// Decompiled with JetBrains decompiler
// Type: StardewValley.ItemTypeDefinitions.ObjectDataDefinition
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.ItemTypeDefinitions;

/// <summary>Manages the data for object items.</summary>
public class ObjectDataDefinition : BaseItemDataDefinition
{
  /// <inheritdoc />
  public override string Identifier => "(O)";

  /// <inheritdoc />
  public override string StandardDescriptor => "O";

  /// <inheritdoc />
  public override IEnumerable<string> GetAllIds() => (IEnumerable<string>) Game1.objectData.Keys;

  /// <inheritdoc />
  public override bool Exists(string itemId)
  {
    return itemId != null && Game1.objectData.ContainsKey(itemId);
  }

  /// <inheritdoc />
  public override ParsedItemData GetData(string itemId)
  {
    ObjectData rawData = this.GetRawData(itemId);
    if (rawData == null)
      return (ParsedItemData) null;
    int category = rawData.Category;
    if (category == 0 && rawData.Type == "Ring")
      category = -96;
    return new ParsedItemData((IItemDataDefinition) this, itemId, rawData.SpriteIndex, rawData.Texture ?? "Maps\\springobjects", rawData.Name, TokenParser.ParseText(rawData.DisplayName), TokenParser.ParseText(rawData.Description), category, rawData.Type, (object) rawData, excludeFromRandomSale: rawData.ExcludeFromRandomSale);
  }

  /// <inheritdoc />
  public override Rectangle GetSourceRect(ParsedItemData data, Texture2D texture, int spriteIndex)
  {
    if (data == null)
      throw new ArgumentNullException(nameof (data));
    return texture != null ? Game1.getSourceRectForStandardTileSheet(texture, spriteIndex, 16 /*0x10*/, 16 /*0x10*/) : throw new ArgumentNullException(nameof (texture));
  }

  /// <inheritdoc />
  public override Item CreateItem(ParsedItemData data)
  {
    string itemId = data != null ? data.ItemId : throw new ArgumentNullException(nameof (data));
    HashSet<string> baseContextTags = ItemContextTagManager.GetBaseContextTags(itemId);
    if (baseContextTags.Contains("torch_item"))
      return (Item) new Torch(1, itemId);
    if (itemId == "812")
      return (Item) new ColoredObject(itemId, 1, Color.Orange);
    if (!baseContextTags.Contains("item_type_ring") && !(itemId == "801"))
      return (Item) new StardewValley.Object(itemId, 1);
    return !(itemId == "880") ? (Item) new Ring(itemId) : (Item) new CombinedRing();
  }

  /// <summary>Get whether an object has an explicit category set in <c>Data/Objects</c>, regardless of whether a category is dynamically assigned after it's loaded.</summary>
  /// <param name="data">The parsed item data to check.</param>
  public static bool HasExplicitCategory(ParsedItemData data)
  {
    return data.HasTypeObject() && data.RawData is ObjectData rawData && rawData.Category < 0;
  }

  /// <summary>Get the raw price field set in <c>Data/Objects</c>.</summary>
  /// <param name="data">The parsed item data to check.</param>
  public static int GetRawPrice(ParsedItemData data)
  {
    return !data.HasTypeObject() || !(data.RawData is ObjectData rawData) ? 0 : rawData.Price;
  }

  /// <summary>Get whether an item is a fish that can produce roe.</summary>
  /// <param name="fish">The potential fish item.</param>
  public bool CanHaveRoe(Item fish)
  {
    return fish is StardewValley.Object @object && ItemContextTagManager.HasBaseTag(@object.QualifiedItemId, "fish_has_roe");
  }

  /// <summary>Create a flavored Aged Roe item (like 'Aged Tuna Roe').</summary>
  /// <param name="ingredient">The roe to age, or the fish whose aged roe to create.</param>
  public virtual ColoredObject CreateFlavoredAgedRoe(StardewValley.Object ingredient)
  {
    if (ingredient == null)
      throw new ArgumentNullException(nameof (ingredient));
    if (ingredient.QualifiedItemId != "(O)812")
      ingredient = (StardewValley.Object) this.CreateFlavoredRoe(ingredient);
    ColoredObject flavoredAgedRoe = new ColoredObject("447", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange);
    flavoredAgedRoe.Name = "Aged " + ingredient.Name;
    flavoredAgedRoe.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.AgedRoe);
    flavoredAgedRoe.preservedParentSheetIndex.Value = ingredient.preservedParentSheetIndex.Value;
    flavoredAgedRoe.Price = ingredient.Price * 2;
    return flavoredAgedRoe;
  }

  /// <summary>Create a flavored honey item (like 'Poppy Honey').</summary>
  /// <param name="ingredient">The item for which to create a honey, or <c>null</c> for a Wild Honey.</param>
  public virtual StardewValley.Object CreateFlavoredHoney(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredHoney = new StardewValley.Object("340", 1);
    if (ingredient == null || ingredient.Name == null || ingredient.Name == "Error Item" || ingredient.ItemId == "-1")
      ingredient = (StardewValley.Object) null;
    if (ingredient == null)
    {
      flavoredHoney.Name = "Wild Honey";
    }
    else
    {
      flavoredHoney.Name = ingredient.Name + " Honey";
      flavoredHoney.Price += ingredient.Price * 2;
    }
    flavoredHoney.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Honey);
    flavoredHoney.preservedParentSheetIndex.Value = ingredient?.ItemId ?? "-1";
    return flavoredHoney;
  }

  /// <summary>Create a flavored jelly item (like 'Apple Jelly').</summary>
  /// <param name="ingredient">The item to jelly.</param>
  public virtual StardewValley.Object CreateFlavoredJelly(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredJelly = ingredient != null ? (StardewValley.Object) new ColoredObject("344", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Red) : throw new ArgumentNullException(nameof (ingredient));
    flavoredJelly.Name = ingredient.Name + " Jelly";
    flavoredJelly.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Jelly);
    flavoredJelly.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredJelly.Price = ingredient.Price * 2 + 50;
    flavoredJelly.Edibility = ingredient.Edibility <= 0 ? (ingredient.Edibility != -300 ? ingredient.Edibility : (int) ((double) ingredient.Price * 0.20000000298023224)) : (int) ((double) ingredient.Edibility * 2.0);
    return flavoredJelly;
  }

  /// <summary>Create a flavored juice item (like 'Apple Juice').</summary>
  /// <param name="ingredient">The item for which to create a juice.</param>
  public virtual StardewValley.Object CreateFlavoredJuice(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredJuice = ingredient != null ? (StardewValley.Object) new ColoredObject("350", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Green) : throw new ArgumentNullException(nameof (ingredient));
    flavoredJuice.Name = ingredient.Name + " Juice";
    flavoredJuice.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Juice);
    flavoredJuice.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredJuice.Price = (int) ((double) ingredient.Price * 2.25);
    flavoredJuice.Edibility = ingredient.Edibility <= 0 ? (ingredient.Edibility != -300 ? ingredient.Edibility : (int) ((double) ingredient.Price * 0.40000000596046448)) : (int) ((double) ingredient.Edibility * 2.0);
    return flavoredJuice;
  }

  /// <summary>Create a pickled item (like 'Pickled Beet').</summary>
  /// <param name="ingredient">The item to pickle.</param>
  public virtual StardewValley.Object CreateFlavoredPickle(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredPickle = ingredient != null ? (StardewValley.Object) new ColoredObject("342", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Green) : throw new ArgumentNullException(nameof (ingredient));
    flavoredPickle.Name = "Pickled " + ingredient.Name;
    flavoredPickle.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Pickle);
    flavoredPickle.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredPickle.Price = ingredient.Price * 2 + 50;
    flavoredPickle.Edibility = ingredient.Edibility <= 0 ? (ingredient.Edibility != -300 ? ingredient.Edibility : (int) ((double) ingredient.Price * 0.25)) : (int) ((double) ingredient.Edibility * 1.75);
    return flavoredPickle;
  }

  /// <summary>Create a flavored Roe item (like 'Tuna Roe').</summary>
  /// <param name="ingredient">The fish whose roe to create.</param>
  public virtual ColoredObject CreateFlavoredRoe(StardewValley.Object ingredient)
  {
    if (ingredient == null)
      throw new ArgumentNullException(nameof (ingredient));
    ColoredObject flavoredRoe = new ColoredObject("812", 1, ingredient.QualifiedItemId == "(O)698" ? new Color(61, 55, 42) : TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange);
    flavoredRoe.Name = ingredient.Name + " Roe";
    flavoredRoe.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Roe);
    flavoredRoe.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredRoe.Price += ingredient.Price / 2;
    return flavoredRoe;
  }

  /// <summary>Create a flavored wine item (like 'Apple Wine').</summary>
  /// <param name="ingredient">The item for which to create a wine.</param>
  public virtual StardewValley.Object CreateFlavoredWine(StardewValley.Object ingredient)
  {
    ColoredObject flavoredWine = ingredient != null ? new ColoredObject("348", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Purple) : throw new ArgumentNullException(nameof (ingredient));
    flavoredWine.Name = ingredient.Name + " Wine";
    flavoredWine.Price = ingredient.Price * 3;
    flavoredWine.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Wine);
    flavoredWine.preservedParentSheetIndex.Value = ingredient.ItemId;
    if (ingredient.Edibility > 0)
      flavoredWine.Edibility = (int) ((double) ingredient.Edibility * 1.75);
    else if (ingredient.Edibility == -300)
      flavoredWine.Edibility = (int) ((double) ingredient.Price * 0.10000000149011612);
    else
      flavoredWine.Edibility = ingredient.Edibility;
    return (StardewValley.Object) flavoredWine;
  }

  /// <summary>Create a flavored bait item (like 'Squid Bait').</summary>
  /// <param name="ingredient">The item for which to create a bait.</param>
  public virtual StardewValley.Object CreateFlavoredBait(StardewValley.Object ingredient)
  {
    ColoredObject flavoredBait = ingredient != null ? new ColoredObject("SpecificBait", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange) : throw new ArgumentNullException(nameof (ingredient));
    flavoredBait.Name = ingredient.Name + " Bait";
    flavoredBait.Price = Math.Max(1, (int) ((double) ingredient.Price * 0.10000000149011612));
    flavoredBait.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Bait);
    flavoredBait.preservedParentSheetIndex.Value = ingredient.ItemId;
    return (StardewValley.Object) flavoredBait;
  }

  /// <summary>Create a flavored dried fruit item (like 'Dried Apple').</summary>
  /// <param name="ingredient">The item for which to create a wine.</param>
  public virtual StardewValley.Object CreateFlavoredDriedFruit(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredDriedFruit = ingredient != null ? (StardewValley.Object) new ColoredObject("DriedFruit", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange) : throw new ArgumentNullException(nameof (ingredient));
    flavoredDriedFruit.Name = Lexicon.makePlural("Dried " + ingredient.Name);
    flavoredDriedFruit.Price = (int) ((double) (ingredient.Price * 5) * 1.5) + 25;
    flavoredDriedFruit.Quality = ingredient.Quality;
    flavoredDriedFruit.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.DriedFruit);
    flavoredDriedFruit.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredDriedFruit.Edibility = ingredient.Edibility * 3;
    flavoredDriedFruit.Edibility = ingredient.Edibility <= 0 ? (ingredient.Edibility != -300 ? ingredient.Edibility : (int) ((double) ingredient.Price * 0.5)) : (int) ((double) ingredient.Edibility * 3.0);
    return flavoredDriedFruit;
  }

  /// <summary>Create a flavored dried fruit item (like 'Dried Apple').</summary>
  /// <param name="ingredient">The item for which to create a wine.</param>
  public virtual StardewValley.Object CreateFlavoredDriedMushroom(StardewValley.Object ingredient)
  {
    ColoredObject flavoredDriedMushroom = ingredient != null ? new ColoredObject("DriedMushrooms", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange) : throw new ArgumentNullException(nameof (ingredient));
    flavoredDriedMushroom.Name = Lexicon.makePlural("Dried " + ingredient.Name);
    flavoredDriedMushroom.Price = (int) ((double) (ingredient.Price * 5) * 1.5) + 25;
    flavoredDriedMushroom.Quality = ingredient.Quality;
    flavoredDriedMushroom.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.DriedMushroom);
    flavoredDriedMushroom.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredDriedMushroom.Edibility = ingredient.Edibility * 3;
    return (StardewValley.Object) flavoredDriedMushroom;
  }

  /// <summary>Create a flavored dried fruit item (like 'Dried Apple').</summary>
  /// <param name="ingredient">The item for which to create a wine.</param>
  public virtual StardewValley.Object CreateFlavoredSmokedFish(StardewValley.Object ingredient)
  {
    StardewValley.Object flavoredSmokedFish = ingredient != null ? (StardewValley.Object) new ColoredObject("SmokedFish", 1, TailoringMenu.GetDyeColor((Item) ingredient) ?? Color.Orange) : throw new ArgumentNullException(nameof (ingredient));
    flavoredSmokedFish.Name = "Smoked " + ingredient.Name;
    flavoredSmokedFish.Price = ingredient.Price * 2;
    flavoredSmokedFish.Quality = ingredient.Quality;
    flavoredSmokedFish.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.SmokedFish);
    flavoredSmokedFish.preservedParentSheetIndex.Value = ingredient.ItemId;
    flavoredSmokedFish.Edibility = ingredient.Edibility <= 0 ? (ingredient.Edibility != -300 ? ingredient.Edibility : (int) ((double) ingredient.Price * 0.30000001192092896)) : (int) ((double) ingredient.Edibility * 1.5);
    return flavoredSmokedFish;
  }

  /// <summary>Create a flavored item (like 'Apple Juice').</summary>
  /// <param name="preserveType">The flavored item type to create.</param>
  /// <param name="ingredient">The ingredient to apply to the flavored item (like apple for Apple Juice).</param>
  public virtual StardewValley.Object CreateFlavoredItem(
    StardewValley.Object.PreserveType preserveType,
    StardewValley.Object ingredient)
  {
    switch (preserveType)
    {
      case StardewValley.Object.PreserveType.Wine:
        return this.CreateFlavoredWine(ingredient);
      case StardewValley.Object.PreserveType.Jelly:
        return this.CreateFlavoredJelly(ingredient);
      case StardewValley.Object.PreserveType.Pickle:
        return this.CreateFlavoredPickle(ingredient);
      case StardewValley.Object.PreserveType.Juice:
        return this.CreateFlavoredJuice(ingredient);
      case StardewValley.Object.PreserveType.Roe:
        return (StardewValley.Object) this.CreateFlavoredRoe(ingredient);
      case StardewValley.Object.PreserveType.AgedRoe:
        return (StardewValley.Object) this.CreateFlavoredAgedRoe(ingredient);
      case StardewValley.Object.PreserveType.Honey:
        return this.CreateFlavoredHoney(ingredient);
      case StardewValley.Object.PreserveType.Bait:
        return this.CreateFlavoredBait(ingredient);
      case StardewValley.Object.PreserveType.DriedFruit:
        return this.CreateFlavoredDriedFruit(ingredient);
      case StardewValley.Object.PreserveType.DriedMushroom:
        return this.CreateFlavoredDriedMushroom(ingredient);
      case StardewValley.Object.PreserveType.SmokedFish:
        return this.CreateFlavoredSmokedFish(ingredient);
      default:
        return (StardewValley.Object) null;
    }
  }

  /// <summary>Get the item ID which will be created by <see cref="M:StardewValley.ItemTypeDefinitions.ObjectDataDefinition.CreateFlavoredItem(StardewValley.Object.PreserveType,StardewValley.Object)" /> for a given preserve type.</summary>
  /// <param name="preserveType">The preserve type.</param>
  /// <param name="ingredientItemId">The item ID for the preserved flavor.</param>
  public string GetBaseItemIdForFlavoredItem(
    StardewValley.Object.PreserveType preserveType,
    string ingredientItemId)
  {
    switch (preserveType)
    {
      case StardewValley.Object.PreserveType.Wine:
        return "(O)348";
      case StardewValley.Object.PreserveType.Jelly:
        return "(O)344";
      case StardewValley.Object.PreserveType.Pickle:
        return "(O)342";
      case StardewValley.Object.PreserveType.Juice:
        return "(O)350";
      case StardewValley.Object.PreserveType.Roe:
        return "(O)812";
      case StardewValley.Object.PreserveType.AgedRoe:
        return "(O)447";
      case StardewValley.Object.PreserveType.Honey:
        return "(O)340";
      case StardewValley.Object.PreserveType.Bait:
        return "(O)SpecificBait";
      case StardewValley.Object.PreserveType.DriedFruit:
        return "(O)DriedFruit";
      case StardewValley.Object.PreserveType.DriedMushroom:
        return "(O)DriedMushrooms";
      case StardewValley.Object.PreserveType.SmokedFish:
        return "(O)SmokedFish";
      default:
        return (string) null;
    }
  }

  /// <summary>Get the raw data fields for an item.</summary>
  /// <param name="itemId">The unqualified item ID.</param>
  protected ObjectData GetRawData(string itemId)
  {
    ObjectData objectData;
    return itemId == null || !Game1.objectData.TryGetValue(itemId, out objectData) ? (ObjectData) null : objectData;
  }
}
