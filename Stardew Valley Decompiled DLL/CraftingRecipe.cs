// Decompiled with JetBrains decompiler
// Type: StardewValley.CraftingRecipe
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley;

public class CraftingRecipe
{
  public const int wild_seed_special_category = -777;
  /// <summary>The index in <c>Data/CookingRecipes</c> or <c>Data/CraftingRecipes</c> for the ingredient list.</summary>
  public const int index_ingredients = 0;
  /// <summary>The index in <c>Data/CookingRecipes</c> or <c>Data/CraftingRecipes</c> for the produced items.</summary>
  public const int index_output = 2;
  /// <summary>The index in <c>Data/CookingRecipes</c> for the unlock conditions.</summary>
  public const int index_cookingUnlockConditions = 3;
  /// <summary>The index in <c>Data/CookingRecipes</c> for the optional translated recipe name. If omitted, the display name for the first output item is used.</summary>
  public const int index_cookingDisplayName = 4;
  /// <summary>The index in <c>Data/CraftingRecipes</c> for whether it produces a <see cref="F:StardewValley.ItemRegistry.type_bigCraftable" /> item.</summary>
  public const int index_craftingBigCraftable = 3;
  /// <summary>The index in <c>Data/CraftingRecipes</c> for the unlock conditions.</summary>
  public const int index_craftingUnlockConditions = 4;
  /// <summary>The index in <c>Data/CraftingRecipes</c> for the optional translated recipe name. If omitted, the display name for the first output item is used.</summary>
  public const int index_craftingDisplayName = 5;
  /// <summary>The recipe key in <c>Data/CookingRecipes</c> or <c>Data/CraftingRecipes</c>.</summary>
  public string name;
  /// <summary>The translated display name for this recipe.</summary>
  public string DisplayName;
  /// <summary>The translated description for the item produced by recipe.</summary>
  public string description;
  /// <summary>The cached crafting recipe data loaded from <c>Data/CraftingRecipes</c>.</summary>
  public static Dictionary<string, string> craftingRecipes;
  /// <summary>The cached cooking recipe data loaded from <c>Data/CookingRecipes</c>.</summary>
  public static Dictionary<string, string> cookingRecipes;
  /// <summary>The ingredients needed by this recipe, indexed by unqualified item ID or category number.</summary>
  public Dictionary<string, int> recipeList = new Dictionary<string, int>();
  /// <summary>The unqualified item IDs produced by this recipe. If there are multiple items, one is chosen at random each time.</summary>
  public List<string> itemToProduce = new List<string>();
  /// <summary>Whether this recipe produces a <see cref="F:StardewValley.ItemRegistry.type_bigCraftable" /> item, instead of an <see cref="F:StardewValley.ItemRegistry.type_object" /> item.</summary>
  public bool bigCraftable;
  /// <summary>Whether this is a recipe in <c>Data/CookingRecipes</c> (true) or <c>Data/CraftingRecipes</c> (false).</summary>
  public bool isCookingRecipe;
  /// <summary>The number of times this recipe has been crafted by the player.</summary>
  public int timesCrafted;
  /// <summary>The number of the selected item in <see cref="F:StardewValley.CraftingRecipe.itemToProduce" /> to produce.</summary>
  public int numberProducedPerCraft;

  public static void InitShared()
  {
    CraftingRecipe.craftingRecipes = DataLoader.CraftingRecipes(Game1.content);
    CraftingRecipe.cookingRecipes = DataLoader.CookingRecipes(Game1.content);
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="name">The recipe key in <c>Data/CookingRecipes</c> or <c>Data/CraftingRecipes</c>.</param>
  public CraftingRecipe(string name)
    : this(name, CraftingRecipe.cookingRecipes.ContainsKey(name))
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="name">The recipe key in <c>Data/CookingRecipes</c> or <c>Data/CraftingRecipes</c>.</param>
  /// <param name="isCookingRecipe">Whether this is a recipe in <c>Data/CookingRecipes</c> (true) or <c>Data/CraftingRecipes</c> (false).</param>
  public CraftingRecipe(string name, bool isCookingRecipe)
  {
    this.isCookingRecipe = isCookingRecipe;
    this.name = name;
    string str1;
    string rawData;
    if (isCookingRecipe && CraftingRecipe.cookingRecipes.TryGetValue(name, out str1))
      rawData = str1;
    else if (CraftingRecipe.craftingRecipes.TryGetValue(name, out str1))
    {
      rawData = str1;
    }
    else
    {
      this.name = name = "Torch";
      rawData = CraftingRecipe.craftingRecipes[name];
    }
    string[] array1 = rawData.Split('/');
    string str2;
    string error;
    if (!ArgUtility.TryGet(array1, 0, out str2, out error, false, "string rawIngredients"))
    {
      str2 = "";
      this.LogParseError(rawData, error);
    }
    string str3;
    if (!ArgUtility.TryGet(array1, 2, out str3, out error, false, "string rawOutputItems"))
    {
      str3 = "";
      this.LogParseError(rawData, error);
    }
    string text;
    if (!ArgUtility.TryGetOptional(array1, isCookingRecipe ? 4 : 5, out text, out error, name: "string tokenizableDisplayName"))
      this.LogParseError(rawData, error);
    this.bigCraftable = !isCookingRecipe && ArgUtility.GetBool(array1, 3);
    string[] array2 = ArgUtility.SplitBySpace(str2);
    for (int index = 0; index < array2.Length; index += 2)
      this.recipeList.Add(array2[index], ArgUtility.GetInt(array2, index + 1, 1));
    string[] array3 = ArgUtility.SplitBySpace(str3);
    for (int index = 0; index < array3.Length; index += 2)
    {
      this.itemToProduce.Add(array3[index]);
      this.numberProducedPerCraft = ArgUtility.GetInt(array3, index + 1, 1);
    }
    ParsedItemData itemData = this.GetItemData(true);
    this.DisplayName = !string.IsNullOrWhiteSpace(text) ? TokenParser.ParseText(text) : itemData?.DisplayName ?? str3;
    this.description = itemData?.Description ?? "";
    if (!Game1.player.craftingRecipes.TryGetValue(name, out this.timesCrafted))
      this.timesCrafted = 0;
    if (!name.Equals("Crab Pot") || !Game1.player.professions.Contains(7))
      return;
    this.recipeList = new Dictionary<string, int>()
    {
      ["388"] = 25,
      ["334"] = 2
    };
  }

  public virtual string getIndexOfMenuView()
  {
    return this.itemToProduce.Count <= 0 ? "-1" : this.itemToProduce[0];
  }

  public virtual bool doesFarmerHaveIngredientsInInventory(IList<Item> extraToCheck = null)
  {
    foreach (KeyValuePair<string, int> recipe in this.recipeList)
    {
      int num = recipe.Value - Game1.player.getItemCount(recipe.Key);
      if (num > 0 && (extraToCheck == null || num - Game1.player.getItemCountInList(extraToCheck, recipe.Key) > 0))
        return false;
    }
    return true;
  }

  public virtual void drawMenuView(SpriteBatch b, int x, int y, float layerDepth = 0.88f, bool shadow = true)
  {
    ParsedItemData itemData = this.GetItemData(true);
    Texture2D texture = itemData.GetTexture();
    Rectangle sourceRect = itemData.GetSourceRect();
    Utility.drawWithShadow(b, texture, new Vector2((float) x, (float) y), sourceRect, Color.White, 0.0f, Vector2.Zero, 4f, layerDepth: layerDepth);
  }

  /// <summary>Get the item data to produce when this recipe is crafted.</summary>
  /// <param name="useFirst">If this recipe has multiple possible outputs, whether to use the first one instead of a random one.</param>
  public virtual ParsedItemData GetItemData(bool useFirst = false)
  {
    string itemId = useFirst ? this.itemToProduce.FirstOrDefault<string>() : Game1.random.ChooseFrom<string>((IList<string>) this.itemToProduce);
    if (this.bigCraftable)
      itemId = ItemRegistry.ManuallyQualifyItemId(itemId, "(BC)");
    return ItemRegistry.GetDataOrErrorItem(itemId);
  }

  public virtual Item createItem()
  {
    Item obj = ItemRegistry.Create(this.GetItemData().QualifiedItemId, this.numberProducedPerCraft);
    if (this.isCookingRecipe && obj is Object @object && Game1.player.team.SpecialOrderRuleActive("QI_COOKING"))
    {
      @object.orderData.Value = "QI_COOKING";
      @object.MarkContextTagsDirty();
    }
    return obj;
  }

  /// <summary>Try to parse the skill level requirement from the raw recipe data entry, if applicable.</summary>
  /// <param name="id">The recipe ID.</param>
  /// <param name="rawData">The raw recipe data from <see cref="F:StardewValley.CraftingRecipe.cookingRecipes" /> or <see cref="F:StardewValley.CraftingRecipe.craftingRecipes" />.</param>
  /// <param name="isCooking">Whether this is a cooking recipe (true) or crafting recipe (false).</param>
  /// <param name="skillNumber">The skill number required to learn this recipe (as returned by <see cref="M:StardewValley.Farmer.getSkillNumberFromName(System.String)" />).</param>
  /// <param name="minLevel">The minimum level of the skill needed to learn the recipe.</param>
  /// <param name="logErrors">Whether to log a warning if the recipe has an invalid skill level condition.</param>
  /// <returns>Returns whether the recipe has a skill requirement which was successfully parsed.</returns>
  public static bool TryParseLevelRequirement(
    string id,
    string rawData,
    bool isCooking,
    out int skillNumber,
    out int minLevel,
    bool logErrors = true)
  {
    int index1 = isCooking ? 3 : 4;
    string conditions = ArgUtility.Get(rawData?.Split('/'), index1);
    string[] array = conditions?.Split(' ');
    int index2 = 1;
    string lower = ArgUtility.Get(array, 0)?.ToLower();
    if (lower != null)
    {
      switch (lower.Length)
      {
        case 1:
          if (lower == "s")
            break;
          goto label_17;
        case 4:
          if (lower == "luck")
            goto label_16;
          goto label_17;
        case 6:
          switch (lower[0])
          {
            case 'c':
              if (lower == "combat")
                goto label_16;
              goto label_17;
            case 'm':
              if (lower == "mining")
                goto label_16;
              goto label_17;
            default:
              goto label_17;
          }
        case 7:
          switch (lower[1])
          {
            case 'a':
              if (lower == "farming")
                goto label_16;
              goto label_17;
            case 'i':
              if (lower == "fishing")
                goto label_16;
              goto label_17;
            default:
              goto label_17;
          }
        case 8:
          if (lower == "foraging")
            goto label_16;
          goto label_17;
        default:
          goto label_17;
      }
label_11:
      string name;
      string error;
      if (ArgUtility.TryGet(array, index2, out name, out error, name: "string skillId") && ArgUtility.TryGetInt(array, index2 + 1, out minLevel, out error, nameof (minLevel)))
      {
        skillNumber = Farmer.getSkillNumberFromName(name);
        if (skillNumber > -1)
          return true;
        LogFormatWarning($"no skill found matching ID '{name}'.");
        goto label_17;
      }
      LogFormatWarning(error);
      goto label_17;
label_16:
      index2 = 0;
      goto label_11;
    }
label_17:
    skillNumber = -1;
    minLevel = -1;
    return false;

    void LogFormatWarning(string error)
    {
      Game1.log.Warn($"{(isCooking ? "Cooking" : "Crafting")} recipe '{id}' has invalid skill level condition '{conditions}': {error}");
    }
  }

  public static bool isThereSpecialIngredientRule(
    Item potentialIngredient,
    string requiredIngredient)
  {
    return requiredIngredient == -777.ToString() && (potentialIngredient.QualifiedItemId == "(O)495" || potentialIngredient.QualifiedItemId == "(O)496" || potentialIngredient.QualifiedItemId == "(O)497" || potentialIngredient.QualifiedItemId == "(O)498");
  }

  public virtual void consumeIngredients(List<IInventory> additionalMaterials)
  {
    foreach (KeyValuePair<string, int> recipe in this.recipeList)
    {
      string key = recipe.Key;
      int val1 = recipe.Value;
      bool flag1 = false;
      for (int index = Game1.player.Items.Count - 1; index >= 0; --index)
      {
        if (CraftingRecipe.ItemMatchesForCrafting(Game1.player.Items[index], key))
        {
          int amount = val1;
          val1 -= Game1.player.Items[index].Stack;
          Game1.player.Items[index] = Game1.player.Items[index].ConsumeStack(amount);
          if (val1 <= 0)
          {
            flag1 = true;
            break;
          }
        }
      }
      if (additionalMaterials != null && !flag1)
      {
        for (int index1 = 0; index1 < additionalMaterials.Count; ++index1)
        {
          IInventory additionalMaterial = additionalMaterials[index1];
          if (additionalMaterial != null)
          {
            bool flag2 = false;
            for (int index2 = additionalMaterial.Count - 1; index2 >= 0; --index2)
            {
              if (CraftingRecipe.ItemMatchesForCrafting(additionalMaterial[index2], key))
              {
                int amount = Math.Min(val1, additionalMaterial[index2].Stack);
                val1 -= amount;
                additionalMaterial[index2] = additionalMaterial[index2].ConsumeStack(amount);
                if (additionalMaterial[index2] == null)
                  flag2 = true;
                if (val1 <= 0)
                  break;
              }
            }
            if (flag2)
              additionalMaterial.RemoveEmptySlots();
            if (val1 <= 0)
              break;
          }
        }
      }
    }
  }

  public static bool DoesFarmerHaveAdditionalIngredientsInInventory(
    List<KeyValuePair<string, int>> additional_recipe_items,
    IList<Item> extraToCheck = null)
  {
    foreach (KeyValuePair<string, int> additionalRecipeItem in additional_recipe_items)
    {
      int num = additionalRecipeItem.Value - Game1.player.getItemCount(additionalRecipeItem.Key);
      if (num > 0 && (extraToCheck == null || num - Game1.player.getItemCountInList(extraToCheck, additionalRecipeItem.Key) > 0))
        return false;
    }
    return true;
  }

  public static bool ItemMatchesForCrafting(Item item, string item_id)
  {
    if (item == null)
      return false;
    if (item.Category.ToString() == item_id || CraftingRecipe.isThereSpecialIngredientRule(item, item_id))
      return true;
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item_id);
    return item.QualifiedItemId == dataOrErrorItem.QualifiedItemId;
  }

  public static void ConsumeAdditionalIngredients(
    List<KeyValuePair<string, int>> additionalRecipeItems,
    List<IInventory> additionalMaterials)
  {
    for (int index1 = additionalRecipeItems.Count - 1; index1 >= 0; --index1)
    {
      KeyValuePair<string, int> additionalRecipeItem = additionalRecipeItems[index1];
      string key = additionalRecipeItem.Key;
      additionalRecipeItem = additionalRecipeItems[index1];
      int val1 = additionalRecipeItem.Value;
      bool flag1 = false;
      for (int index2 = Game1.player.Items.Count - 1; index2 >= 0; --index2)
      {
        Item obj = Game1.player.Items[index2];
        if (CraftingRecipe.ItemMatchesForCrafting(obj, key))
        {
          int amount = Math.Min(val1, obj.Stack);
          val1 -= amount;
          Game1.player.Items[index2] = obj.ConsumeStack(amount);
          if (val1 <= 0)
          {
            flag1 = true;
            break;
          }
        }
      }
      if (additionalMaterials != null && !flag1)
      {
        for (int index3 = 0; index3 < additionalMaterials.Count; ++index3)
        {
          IInventory additionalMaterial = additionalMaterials[index3];
          if (additionalMaterial != null)
          {
            bool flag2 = false;
            for (int index4 = additionalMaterial.Count - 1; index4 >= 0; --index4)
            {
              Item obj = additionalMaterial[index4];
              if (CraftingRecipe.ItemMatchesForCrafting(obj, key))
              {
                int amount = Math.Min(val1, obj.Stack);
                val1 -= amount;
                additionalMaterial[index4] = obj.ConsumeStack(amount);
                if (additionalMaterial[index4] == null)
                  flag2 = true;
                if (val1 <= 0)
                  break;
              }
            }
            if (flag2)
              additionalMaterial.RemoveEmptySlots();
            if (val1 <= 0)
              break;
          }
        }
      }
    }
  }

  public virtual int getCraftableCount(IList<Chest> additional_material_chests)
  {
    List<Item> additional_materials = new List<Item>();
    if (additional_material_chests != null)
    {
      for (int index = 0; index < additional_material_chests.Count; ++index)
        additional_materials.AddRange((IEnumerable<Item>) additional_material_chests[index].Items);
    }
    return this.getCraftableCount((IList<Item>) additional_materials);
  }

  public virtual int getCraftableCount(IList<Item> additional_materials)
  {
    int craftableCount = -1;
    foreach (KeyValuePair<string, int> recipe in this.recipeList)
    {
      int num1 = 0;
      string requiredIngredient = recipe.Key;
      int num2 = recipe.Value;
      if (!requiredIngredient.StartsWith("(") && !requiredIngredient.StartsWith("-"))
        requiredIngredient = "(O)" + requiredIngredient;
      for (int index = Game1.player.Items.Count - 1; index >= 0; --index)
      {
        if (Game1.player.Items[index] is Object potentialIngredient && (potentialIngredient.QualifiedItemId == requiredIngredient || potentialIngredient.Category.ToString() == requiredIngredient || CraftingRecipe.isThereSpecialIngredientRule((Item) potentialIngredient, requiredIngredient)))
          num1 += potentialIngredient.Stack;
      }
      if (additional_materials != null)
      {
        for (int index = 0; index < additional_materials.Count; ++index)
        {
          if (additional_materials[index] is Object additionalMaterial && (additionalMaterial.QualifiedItemId == requiredIngredient || additionalMaterial.Category.ToString() == requiredIngredient || CraftingRecipe.isThereSpecialIngredientRule((Item) additionalMaterial, requiredIngredient)))
            num1 += additionalMaterial.Stack;
        }
      }
      int num3 = num1 / num2;
      if (num3 < craftableCount || craftableCount == -1)
        craftableCount = num3;
    }
    return craftableCount;
  }

  public virtual string getCraftCountText()
  {
    if (this.isCookingRecipe)
    {
      int sub1;
      if (Game1.player.recipesCooked.TryGetValue(this.getIndexOfMenuView(), out sub1) && sub1 > 0)
        return Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", (object) sub1);
    }
    else
    {
      int sub1;
      if (Game1.player.craftingRecipes.TryGetValue(this.name, out sub1) && sub1 > 0)
        return Game1.content.LoadString("Strings\\UI:Crafting_NumberCrafted", (object) sub1);
    }
    return (string) null;
  }

  public virtual int getDescriptionHeight(int width)
  {
    return (int) ((double) Game1.smallFont.MeasureString(Game1.parseText(this.description, Game1.smallFont, width)).Y + (double) (this.getNumberOfIngredients() * 36) + (double) (int) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567")).Y + 21.0);
  }

  public virtual void drawRecipeDescription(
    SpriteBatch b,
    Vector2 position,
    int width,
    IList<Item> additional_crafting_items)
  {
    int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 8 : 0;
    b.Draw(Game1.staminaRect, new Rectangle((int) ((double) position.X + 8.0), (int) ((double) position.Y + 32.0 + (double) Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int) ((double) num1 * 1.5), width - 32 /*0x20*/, 2), Game1.textColor * 0.35f);
    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f);
    int num2 = -1;
    foreach (KeyValuePair<string, int> recipe in this.recipeList)
    {
      ++num2;
      int toDraw = recipe.Value;
      string key = recipe.Key;
      int itemCount = Game1.player.getItemCount(key);
      int num3 = 0;
      int num4 = toDraw - itemCount;
      if (additional_crafting_items != null)
      {
        num3 = Game1.player.getItemCountInList(additional_crafting_items, key);
        if (num4 > 0)
          num4 -= num3;
      }
      string nameFromIndex = this.getNameFromIndex(key);
      Color color1 = num4 <= 0 ? Game1.textColor : Color.Red;
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.getSpriteIndexFromRawIndex(key));
      Texture2D texture = dataOrErrorItem.GetTexture();
      Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
      float scale = 2f;
      if (sourceRect.Width > 0 || sourceRect.Height > 0)
        scale *= 16f / (float) Math.Max(sourceRect.Width, sourceRect.Height);
      b.Draw(texture, new Vector2(position.X + 16f, (float) ((double) position.Y + 64.0 + (double) (num2 * 64 /*0x40*/ / 2) + (double) (num2 * 4) + 16.0)), new Rectangle?(sourceRect), Color.White, 0.0f, new Vector2((float) (sourceRect.Width / 2), (float) (sourceRect.Height / 2)), scale, SpriteEffects.None, 0.86f);
      Utility.drawTinyDigits(toDraw, b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(toDraw.ToString() ?? "").X, (float) ((double) position.Y + 64.0 + (double) (num2 * 64 /*0x40*/ / 2) + (double) (num2 * 4) + 21.0)), 2f, 0.87f, Color.AntiqueWhite);
      Vector2 position1 = new Vector2((float) ((double) position.X + 32.0 + 8.0), (float) ((double) position.Y + 64.0 + (double) (num2 * 64 /*0x40*/ / 2) + (double) (num2 * 4) + 4.0));
      Utility.drawTextWithShadow(b, nameFromIndex, Game1.smallFont, position1, color1);
      if (Game1.options.showAdvancedCraftingInformation)
      {
        position1.X = (float) ((double) position.X + (double) width - 40.0);
        b.Draw(Game1.mouseCursors, new Rectangle((int) position1.X, (int) position1.Y + 2, 22, 26), new Rectangle?(new Rectangle(268, 1436, 11, 13)), Color.White);
        SpriteBatch b1 = b;
        int num5 = itemCount + num3;
        string text1 = num5.ToString() ?? "";
        SpriteFont smallFont1 = Game1.smallFont;
        Vector2 vector2_1 = position1;
        SpriteFont smallFont2 = Game1.smallFont;
        num5 = itemCount + num3;
        string text2 = num5.ToString() + " ";
        Vector2 vector2_2 = new Vector2(smallFont2.MeasureString(text2).X, 0.0f);
        Vector2 position2 = vector2_1 - vector2_2;
        Color color2 = color1;
        Utility.drawTextWithShadow(b1, text1, smallFont1, position2, color2);
      }
    }
    b.Draw(Game1.staminaRect, new Rectangle((int) position.X + 8, (int) position.Y + num1 + 64 /*0x40*/ + 4 + this.recipeList.Count * 36, width - 32 /*0x20*/, 2), Game1.textColor * 0.35f);
    Utility.drawTextWithShadow(b, Game1.parseText(this.description, Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(0.0f, (float) (76 + this.recipeList.Count * 36 + num1)), Game1.textColor * 0.75f);
  }

  public virtual int getNumberOfIngredients() => this.recipeList.Count;

  public virtual string getSpriteIndexFromRawIndex(string item_id)
  {
    switch (item_id)
    {
      case "-1":
        return "(O)20";
      case "-2":
        return "(O)80";
      case "-3":
        return "(O)24";
      case "-4":
        return "(O)145";
      case "-5":
        return "(O)176";
      case "-6":
        return "(O)184";
      default:
        return item_id == -777.ToString() ? "(O)495" : item_id;
    }
  }

  public virtual string getNameFromIndex(string item_id)
  {
    if (item_id != null && item_id.StartsWith('-'))
    {
      switch (item_id)
      {
        case "-1":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.568");
        case "-2":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
        case "-3":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
        case "-4":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
        case "-5":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
        case "-6":
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
        default:
          return item_id == -777.ToString() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.574") : "???";
      }
    }
    else
    {
      ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(item_id);
      return dataOrErrorItem != null ? dataOrErrorItem.DisplayName : ItemRegistry.GetErrorItemName();
    }
  }

  /// <summary>Log a message indicating the underlying crafting data is invalid.</summary>
  /// <param name="rawData">The raw data being parsed.</param>
  /// <param name="message">The error message indicating why parsing failed.</param>
  private void LogParseError(string rawData, string message)
  {
    Game1.log.Error($"Failed parsing raw recipe data '{rawData}' for {(this.isCookingRecipe ? "cooking" : "crafting")} recipe '{this.name}': {message}");
  }
}
