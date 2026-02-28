#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PropertyChanged.SourceGenerator;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData.Pets;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;

namespace PetAdopter;

/// <summary>Represents a single pet type + breed combo that the player can browse.</summary>
internal class PetOption
{
    public string PetType { get; }
    public string BreedId { get; }
    public string DisplayName { get; }

    /// <summary>The actual pet sprite sheet (e.g. Animals/Cat0) for a full appearance preview.</summary>
    public Texture2D SpriteTexture { get; }

    /// <summary>Source rect for one frame of the pet sprite (32x32, sitting pose).</summary>
    public Rectangle SpriteSourceRect { get; }

    public PetOption(string petType, string breedId, string displayName, Texture2D spriteTexture, Rectangle spriteSourceRect)
    {
        PetType = petType;
        BreedId = breedId;
        DisplayName = displayName;
        SpriteTexture = spriteTexture;
        SpriteSourceRect = spriteSourceRect;
    }
}

/// <summary>ViewModel for the Pet Adopter StardewUI menu.</summary>
internal partial class PetAdopterViewModel : INotifyPropertyChanged
{
    // ── Browse state ──
    private readonly List<PetOption> petOptions = new();
    private int currentIndex;

    // ── Bound properties ──

    /// <summary>The pet sprite to display (texture + source rect tuple for StardewUI).</summary>
    [Notify] private Tuple<Texture2D, Rectangle>? petSprite;

    /// <summary>Display label, e.g. "Dog 1".</summary>
    [Notify] private string petLabel = "";

    /// <summary>Status text shown below the adopt button.</summary>
    [Notify] private string statusText = "";

    /// <summary>Whether the adopt button should be visible/enabled.</summary>
    [Notify] private bool canAdopt = true;

    /// <summary>Counter text like "2 / 6".</summary>
    [Notify] private string counterText = "";

    /// <summary>Localized adopt button text.</summary>
    [Notify] private string adoptButtonText = "Adopt";

    /// <summary>Localized menu title.</summary>
    [Notify] private string menuTitle = "Pet Adopter";

    // ── Constructor ──
    public PetAdopterViewModel()
    {
        // Load i18n strings
        var i18n = ModEntry.ModHelper?.Translation;
        if (i18n != null)
        {
            AdoptButtonText = i18n.Get("menu.adopt");
            MenuTitle = i18n.Get("menu.title");
        }

        LoadPetOptions();
        if (petOptions.Count > 0)
            UpdateDisplay();
        else
        {
            PetLabel = i18n?.Get("status.no-pets-found") ?? "No pet types found!";
            CanAdopt = false;
        }

        RefreshStatus();
    }

    // ── Public commands (bound from StarML) ──

    /// <summary>Navigate to previous pet.</summary>
    public void OnPrevious()
    {
        if (petOptions.Count == 0) return;
        currentIndex--;
        if (currentIndex < 0) currentIndex = petOptions.Count - 1;
        UpdateDisplay();
        Game1.playSound("shwip");
    }

    /// <summary>Navigate to next pet.</summary>
    public void OnNext()
    {
        if (petOptions.Count == 0) return;
        currentIndex++;
        if (currentIndex >= petOptions.Count) currentIndex = 0;
        UpdateDisplay();
        Game1.playSound("shwip");
    }

    /// <summary>Adopt the currently displayed pet.</summary>
    public void OnAdopt()
    {
        if (!CanAdopt || petOptions.Count == 0) return;

        // Check if there are available pet bowls.
        int availableBowls = CountAvailableBowls();
        if (availableBowls <= 0)
        {
            var i18n = ModEntry.ModHelper?.Translation;
            StatusText = i18n?.Get("status.no-bowls-short") ?? "No pet bowls available! Build one first.";
            CanAdopt = false;
            Game1.playSound("cancel");
            return;
        }

        var selected = petOptions[currentIndex];

        // Close the adoption menu and open the naming menu.
        Game1.exitActiveMenu();
        Game1.activeClickableMenu = new NamingMenu(
            new NamingMenu.doneNamingBehavior(name => FinishAdoption(name, selected)),
            Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"),
            Dialogue.randomName()
        );

        Game1.playSound("purchaseClick");
    }

    // ── Internal helpers ──

    private void LoadPetOptions()
    {
        if (Game1.petData == null) return;

        foreach (var kvp in Game1.petData)
        {
            string petType = kvp.Key;
            PetData data = kvp.Value;
            string typeDisplayName = NormalizePetTypeName(TokenParser.ParseText(data.DisplayName) ?? petType);

            foreach (PetBreed breed in data.Breeds)
            {
                try
                {
                    // Load the actual pet sprite sheet (e.g. "Animals/Cat0") for a real appearance preview.
                    string spriteSheetPath = breed.Texture;
                    var spriteTexture = Game1.content.Load<Texture2D>(spriteSheetPath);
                    // Frame 0 = sitting/facing-down pose, 32x32 per frame.
                    var srcRect = new Rectangle(0, 0, 32, 32);
                    // Display as "Dog 1", "Cat 2" etc. (1-indexed for humans).
                    int displayNumber = int.TryParse(breed.Id, out int breedNum) ? breedNum + 1 : petOptions.Count + 1;
                    string label = $"{typeDisplayName} {displayNumber}";
                    petOptions.Add(new PetOption(petType, breed.Id, label, spriteTexture, srcRect));
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor?.Log(
                        ModEntry.ModHelper?.Translation.Get("log.sprite-fail", new { petType, breedId = breed.Id, error = ex.Message })
                        ?? $"Failed to load sprite for {petType}/{breed.Id}: {ex.Message}",
                        LogLevel.Warn);
                }
            }
        }
    }

    private static string NormalizePetTypeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        if (name.Equals("dog", StringComparison.OrdinalIgnoreCase))
            return "Dog";
        if (name.Equals("cat", StringComparison.OrdinalIgnoreCase))
            return "Cat";

        return name;
    }

    private void UpdateDisplay()
    {
        if (petOptions.Count == 0) return;
        var opt = petOptions[currentIndex];
        PetSprite = Tuple.Create(opt.SpriteTexture, opt.SpriteSourceRect);
        PetLabel = opt.DisplayName;
        CounterText = $"{currentIndex + 1} / {petOptions.Count}";
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        int bowls = CountAvailableBowls();
        int totalPets = Utility.getAllPets().Count;
        var i18n = ModEntry.ModHelper?.Translation;

        if (bowls <= 0)
        {
            StatusText = i18n?.Get("status.no-bowls", new { totalPets }) ?? $"No empty pet bowls! ({totalPets} pet(s) on farm)";
            CanAdopt = false;
        }
        else
        {
            StatusText = i18n?.Get("status.available", new { bowls, totalPets }) ?? $"{bowls} bowl(s) available  ·  {totalPets} pet(s) on farm";
            CanAdopt = true;
        }
    }

    private static int CountAvailableBowls()
    {
        int count = 0;
        Farm? farm = Game1.getFarm();
        if (farm == null) return 0;

        foreach (Building building in farm.buildings)
        {
            if (building is PetBowl petBowl && !petBowl.HasPet())
                count++;
        }
        return count;
    }

    /// <summary>Called after the player types a name. Spawns the pet.</summary>
    private static void FinishAdoption(string name, PetOption selected)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Game1.exitActiveMenu();
            return;
        }

        FarmHouse home = Utility.getHomeOfFarmer(Game1.player);

        // Pick spawn position based on house upgrade level (same as vanilla PetLicense).
        Point spawnTile = home.upgradeLevel switch
        {
            1 => new Point(9, 7),
            >= 2 => new Point(27, 26),
            _ => new Point(3, 7)
        };

        // Offset spawn randomly by up to ±3 tiles to avoid stacking.
        Random rng = Game1.random;
        int offsetX = rng.Next(-3, 4);
        int offsetY = rng.Next(-3, 4);
        int finalX = Math.Max(1, spawnTile.X + offsetX);
        int finalY = Math.Max(1, spawnTile.Y + offsetY);

        // Create the pet.
        Pet pet = new Pet(finalX, finalY, selected.BreedId, selected.PetType);
        pet.currentLocation = home;
        home.characters.Add(pet);
        pet.warpToFarmHouse(Game1.player);
        pet.Name = name;
        pet.displayName = pet.Name;

        // Assign to first available pet bowl.
        Farm? farm = Game1.getFarm();
        if (farm != null)
        {
            foreach (Building building in farm.buildings)
            {
                if (building is PetBowl petBowl && !petBowl.HasPet())
                {
                    petBowl.AssignPet(pet);
                    break;
                }
            }
        }

        // Fire the "gotPet" event for all farmers (same as vanilla).
        foreach (Farmer farmer in Game1.getAllFarmers())
            farmer.autoGenerateActiveDialogueEvent("gotPet");

        Game1.exitActiveMenu();
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:AdoptedPet", name));

        ModEntry.ModMonitor?.Log(
            ModEntry.ModHelper?.Translation.Get("log.adopted", new { petType = selected.PetType, breedId = selected.BreedId, petName = name })
            ?? $"[Pet Adopter] Adopted {selected.PetType} (breed {selected.BreedId}) named \"{name}\".",
            LogLevel.Info);
    }
}
