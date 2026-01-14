using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using System.Collections.Generic;

namespace WorkingPets.Integrations;

/// <summary>
/// Integration with NPC Map Locations mod to show pets on the map.
/// </summary>
public class NpcMapLocationsIntegration
{
    private readonly IModHelper Helper;
    private readonly IMonitor Monitor;
    private readonly string NPCMapLocationsId = "Bouhm.NPCMapLocations";
    
    private bool IsAvailable = false;
    private bool Initialized = false;
    
    public NpcMapLocationsIntegration(IModHelper helper, IMonitor monitor)
    {
        Helper = helper;
        Monitor = monitor;
    }
    
    /// <summary>
    /// Initialize the integration if NPC Map Locations is installed.
    /// </summary>
    public void Initialize()
    {
        if (Initialized)
            return;
            
        Initialized = true;
        
        var modInfo = Helper.ModRegistry.Get(NPCMapLocationsId);
        if (modInfo != null)
        {
            IsAvailable = true;
            Monitor.Log($"NPC Map Locations detected! Pets will appear on the map with their custom names.", LogLevel.Info);
        }
    }
    
    /// <summary>
    /// Get all pets with their custom names for map tracking.
    /// </summary>
    public Dictionary<Pet, string> GetPetsWithNames()
    {
        var pets = new Dictionary<Pet, string>();
        
        if (!IsAvailable || !Context.IsWorldReady)
            return pets;
        
        // Get all pets in the world
        Utility.ForEachLocation(location =>
        {
            foreach (var character in location.characters)
            {
                if (character is Pet pet)
                {
                    // Use the pet's display name (custom name given by player)
                    string displayName = string.IsNullOrWhiteSpace(pet.displayName) 
                        ? pet.Name 
                        : pet.displayName;
                    
                    pets[pet] = displayName;
                }
            }
            return true;
        });
        
        return pets;
    }
    
    /// <summary>
    /// Check if a pet should be shown on the map based on settings.
    /// </summary>
    public bool ShouldShowPet(Pet pet)
    {
        if (!IsAvailable)
            return false;
            
        return true;
    }
}
