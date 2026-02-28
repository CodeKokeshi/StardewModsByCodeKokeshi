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
    public Texture2D IconTexture { get; }
    public Rectangle IconSourceRect { get; }

    public PetOption(string petType, string breedId, string displayName, Texture2D iconTexture, Rectangle iconSourceRect)
    {
        PetType = petType;
        BreedId = breedId;
        DisplayName = displayName;
        IconTexture = iconTexture;
        IconSourceRect = iconSourceRect;
    }
}

/// <summary>ViewModel for the Pet Adopter StardewUI menu.</summary>
internal partial class PetAdopterViewModel : INotifyPropertyChanged
{
    // ── Browse state ──
    private readonly List<PetOption> petOptions = new();
    private int currentIndex;

    // ── Bound properties ──

    /// <summary>The icon sprite to display (texture + source rect tuple for StardewUI).</summary>
    [Notify] private Tuple<Texture2D, Rectangle>? petSprite;

    /// <summary>Display label, e.g. "Cat — Breed 1".</summary>
    [Notify] private string petLabel = "";

    /// <summary>Status text shown below the adopt button.</summary>
    [Notify] private string statusText = "";

    /// <summary>Whether the adopt button should be visible/enabled.</summary>
    [Notify] private bool canAdopt = true;

    /// <summary>Counter text like "2 / 6".</summary>
    [Notify] private string counterText = "";

    // ── Constructor ──
    public PetAdopterViewModel()
    {
        LoadPetOptions();
        if (petOptions.Count > 0)
            UpdateDisplay();
        else
        {
            PetLabel = "No pet types found!";
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
            StatusText = "No pet bowls available! Build one first.";
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
            string typeDisplayName = TokenParser.ParseText(data.DisplayName) ?? petType;

            foreach (PetBreed breed in data.Breeds)
            {
                try
                {
                    var texture = Game1.content.Load<Texture2D>(breed.IconTexture);
                    var srcRect = breed.IconSourceRect;
                    string label = $"{typeDisplayName}  —  Breed {breed.Id}";
                    petOptions.Add(new PetOption(petType, breed.Id, label, texture, srcRect));
                }
                catch (Exception ex)
                {
                    ModEntry.ModMonitor?.Log($"Failed to load icon for {petType}/{breed.Id}: {ex.Message}", LogLevel.Warn);
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        if (petOptions.Count == 0) return;
        var opt = petOptions[currentIndex];
        PetSprite = Tuple.Create(opt.IconTexture, opt.IconSourceRect);
        PetLabel = opt.DisplayName;
        CounterText = $"{currentIndex + 1} / {petOptions.Count}";
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        int bowls = CountAvailableBowls();
        int totalPets = Utility.getAllPets().Count;

        if (bowls <= 0)
        {
            StatusText = $"No empty pet bowls! ({totalPets} pet(s) on farm)";
            CanAdopt = false;
        }
        else
        {
            StatusText = $"{bowls} bowl(s) available  ·  {totalPets} pet(s) on farm";
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

        ModEntry.ModMonitor?.Log($"[Pet Adopter] Adopted {selected.PetType} (breed {selected.BreedId}) named \"{name}\".", LogLevel.Info);
    }
}
