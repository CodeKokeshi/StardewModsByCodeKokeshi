// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.PetLicense
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Locations;
using StardewValley.Menus;

#nullable disable
namespace StardewValley.Objects;

public class PetLicense : Object
{
  /// <summary>The delimiter between the pet ID and breed ID in the <see cref="P:StardewValley.Object.Name" /> field.</summary>
  public const char Delimiter = '|';

  public PetLicense()
    : base(nameof (PetLicense), 1)
  {
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
    this.AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
    if (drawShadow && !this.bigCraftable.Value && this.QualifiedItemId != "(O)590" && this.QualifiedItemId != "(O)SeedSpot")
      spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
    ItemRegistry.GetDataOrErrorItem(this.QualifiedItemId);
    float num = scaleSize;
    if (this.bigCraftable.Value && (double) num > 0.20000000298023224)
      num /= 2f;
    string[] strArray = this.Name.Split('|');
    PetData petData;
    if (Game1.petData.TryGetValue(strArray[0], out petData))
    {
      PetBreed breedById = petData.GetBreedById(strArray[1]);
      if (breedById != null)
      {
        Rectangle iconSourceRect = breedById.IconSourceRect;
        spriteBatch.Draw(Game1.content.Load<Texture2D>(breedById.IconTexture), location + new Vector2(32f, 32f), new Rectangle?(iconSourceRect), color * transparency, 0.0f, new Vector2((float) (iconSourceRect.Width / 2), (float) (iconSourceRect.Height / 2)), 4f * num, SpriteEffects.None, layerDepth);
      }
    }
    this.DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
  }

  public override bool actionWhenPurchased(string shopId)
  {
    Game1.exitActiveMenu();
    Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.namePet), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), Dialogue.randomName());
    Game1.playSound("purchaseClick");
    return true;
  }

  private void namePet(string name)
  {
    string[] strArray = this.Name.Split('|');
    FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
    Point point = new Point(3, 7);
    if (homeOfFarmer.upgradeLevel == 1)
      point = new Point(9, 7);
    else if (homeOfFarmer.upgradeLevel >= 2)
      point = new Point(27, 26);
    Pet pet = new Pet(point.X, point.Y, strArray[1], strArray[0]);
    pet.currentLocation = (GameLocation) homeOfFarmer;
    homeOfFarmer.characters.Add((NPC) pet);
    pet.warpToFarmHouse(Game1.player);
    pet.Name = name;
    pet.displayName = pet.name.Value;
    foreach (Building building in Game1.getFarm().buildings)
    {
      if (building is PetBowl petBowl && !petBowl.HasPet())
      {
        petBowl.AssignPet(pet);
        break;
      }
    }
    foreach (Farmer allFarmer in Game1.getAllFarmers())
      allFarmer.autoGenerateActiveDialogueEvent("gotPet");
    Game1.exitActiveMenu();
    if (Game1.currentLocation.getCharacterFromName("Marnie") != null)
      Game1.DrawDialogue(Game1.currentLocation.getCharacterFromName("Marnie"), "Strings\\1_6_Strings:AdoptedPet_Marnie", (object) name);
    else
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:AdoptedPet", (object) name));
  }
}
