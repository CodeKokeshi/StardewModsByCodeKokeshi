using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace WorkingPets.Behaviors
{
    /// <summary>Manages multiple pets and coordinates their work to prevent conflicts.</summary>
    public class MultiPetManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>Individual work managers for each pet, keyed by pet's unique GUID.</summary>
        private readonly Dictionary<Guid, PetWorkManager> _petManagers = new();
        
        /// <summary>Global set of tiles currently being targeted by any pet (prevents double-targeting).</summary>
        private static readonly HashSet<Vector2> _globalReservedTargets = new();
        
        /// <summary>Maps reserved tile -> pet GUID that reserved it.</summary>
        private static readonly Dictionary<Vector2, Guid> _targetReservations = new();

        /*********
        ** Properties
        *********/
        /// <summary>Gets all active pet work managers.</summary>
        public IReadOnlyDictionary<Guid, PetWorkManager> PetManagers => _petManagers;

        /// <summary>Gets the set of globally reserved targets.</summary>
        public static IReadOnlySet<Vector2> ReservedTargets => _globalReservedTargets;

        /*********
        ** Public methods
        *********/
        
        /// <summary>Initialize all pets in the game.</summary>
        public void InitializeAllPets()
        {
            _petManagers.Clear();
            _globalReservedTargets.Clear();
            _targetReservations.Clear();

            var allPets = GetAllPets();
            foreach (var pet in allPets)
            {
                InitializePet(pet);
            }

            ModEntry.Instance.Monitor.Log($"[MultiPetManager] Initialized {_petManagers.Count} pet(s).", LogLevel.Info);
        }

        /// <summary>Initialize a single pet (adds to manager if not already tracked).</summary>
        public void InitializePet(Pet pet)
        {
            if (pet == null) return;

            Guid petGuid = pet.petId.Value;
            
            if (!_petManagers.ContainsKey(petGuid))
            {
                var manager = new PetWorkManager();
                manager.Initialize(pet);
                _petManagers[petGuid] = manager;
                ModEntry.Instance.Monitor.Log($"[MultiPetManager] Added pet: {pet.Name} (ID: {petGuid})", LogLevel.Debug);
            }
        }

        /// <summary>Get the work manager for a specific pet.</summary>
        public PetWorkManager? GetManagerForPet(Pet pet)
        {
            if (pet == null) return null;
            
            Guid petGuid = pet.petId.Value;
            
            // Auto-initialize if not tracked yet
            if (!_petManagers.ContainsKey(petGuid))
            {
                InitializePet(pet);
            }

            return _petManagers.TryGetValue(petGuid, out var manager) ? manager : null;
        }

        /// <summary>Update all pets.</summary>
        public void UpdateAll()
        {
            // Clean up stale reservations periodically
            CleanupStaleReservations();

            foreach (var kvp in _petManagers)
            {
                kvp.Value.Update();
            }
        }

        /// <summary>Pause all pets for dialogue.</summary>
        public void PauseAllForDialogue()
        {
            foreach (var manager in _petManagers.Values)
            {
                manager.PauseForDialogue();
            }
        }

        /// <summary>Resume all pets from dialogue.</summary>
        public void ResumeAllFromDialogue()
        {
            foreach (var manager in _petManagers.Values)
            {
                manager.ResumeFromDialogue();
            }
        }

        /// <summary>Save state for all pets.</summary>
        public void SaveAllStates()
        {
            foreach (var kvp in _petManagers)
            {
                var pet = kvp.Value.Pet;
                if (pet != null)
                {
                    kvp.Value.SaveState(pet);
                }
            }
        }

        /// <summary>Load state for all pets.</summary>
        public void LoadAllStates()
        {
            foreach (var kvp in _petManagers)
            {
                var pet = kvp.Value.Pet;
                if (pet != null)
                {
                    kvp.Value.LoadState(pet);
                }
            }
        }

        /// <summary>Try to reserve a target tile for a specific pet.</summary>
        /// <returns>True if reservation successful, false if already reserved by another pet.</returns>
        public static bool TryReserveTarget(Pet pet, Vector2 tile)
        {
            if (pet == null) return false;
            
            Guid petGuid = pet.petId.Value;
            
            // Already reserved by this pet? That's fine
            if (_targetReservations.TryGetValue(tile, out Guid existingOwner) && existingOwner == petGuid)
            {
                return true;
            }

            // Reserved by another pet? Can't take it
            if (_globalReservedTargets.Contains(tile))
            {
                return false;
            }

            // Reserve it
            _globalReservedTargets.Add(tile);
            _targetReservations[tile] = petGuid;
            return true;
        }

        /// <summary>Release a target reservation for a pet.</summary>
        public static void ReleaseTarget(Pet pet, Vector2 tile)
        {
            if (pet == null) return;
            
            Guid petGuid = pet.petId.Value;

            // Only release if this pet owns the reservation
            if (_targetReservations.TryGetValue(tile, out Guid owner) && owner == petGuid)
            {
                _globalReservedTargets.Remove(tile);
                _targetReservations.Remove(tile);
            }
        }

        /// <summary>Release all reservations for a specific pet.</summary>
        public static void ReleaseAllTargetsForPet(Pet pet)
        {
            if (pet == null) return;
            
            Guid petGuid = pet.petId.Value;
            
            var tilesToRelease = _targetReservations
                .Where(kvp => kvp.Value == petGuid)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tile in tilesToRelease)
            {
                _globalReservedTargets.Remove(tile);
                _targetReservations.Remove(tile);
            }
        }

        /// <summary>Check if a tile is reserved by another pet (not this one).</summary>
        public static bool IsTargetReservedByOther(Pet pet, Vector2 tile)
        {
            if (pet == null) return _globalReservedTargets.Contains(tile);
            
            Guid petGuid = pet.petId.Value;
            
            if (_targetReservations.TryGetValue(tile, out Guid owner))
            {
                return owner != petGuid;
            }
            
            return false;
        }

        /// <summary>Check if two pets are too close (for collision avoidance).</summary>
        public static bool ArePetsTooClose(Pet pet1, Pet pet2, float minDistance = 48f)
        {
            if (pet1 == null || pet2 == null) return false;
            if (pet1.petId.Value == pet2.petId.Value) return false; // Same pet
            if (pet1.currentLocation != pet2.currentLocation) return false; // Different locations

            float distance = Vector2.Distance(pet1.Position, pet2.Position);
            return distance < minDistance;
        }

        /// <summary>Get a nudge direction to avoid collision with other pets.</summary>
        public static Vector2 GetCollisionAvoidanceNudge(Pet pet, List<Pet> allPets, float avoidanceStrength = 2f)
        {
            if (pet == null) return Vector2.Zero;

            Vector2 nudge = Vector2.Zero;

            foreach (var otherPet in allPets)
            {
                if (otherPet == null || otherPet.petId.Value == pet.petId.Value) continue;
                if (otherPet.currentLocation != pet.currentLocation) continue;

                float distance = Vector2.Distance(pet.Position, otherPet.Position);
                if (distance < 64f && distance > 0.1f) // Too close but not at exact same spot
                {
                    Vector2 awayDir = pet.Position - otherPet.Position;
                    awayDir.Normalize();
                    nudge += awayDir * (avoidanceStrength * (64f - distance) / 64f);
                }
            }

            return nudge;
        }

        /*********
        ** Private methods
        *********/
        
        /// <summary>Get all pets in the game.</summary>
        public static List<Pet> GetAllPets()
        {
            var pets = new List<Pet>();
            
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc is Pet pet)
                {
                    pets.Add(pet);
                }
            }
            
            return pets;
        }

        /// <summary>Clean up reservations for targets that no longer exist or pets that are gone.</summary>
        private void CleanupStaleReservations()
        {
            var allPetGuids = _petManagers.Keys.ToHashSet();
            var staleTiles = new List<Vector2>();

            foreach (var kvp in _targetReservations)
            {
                // If the pet that reserved this is no longer tracked, release it
                if (!allPetGuids.Contains(kvp.Value))
                {
                    staleTiles.Add(kvp.Key);
                }
            }

            foreach (var tile in staleTiles)
            {
                _globalReservedTargets.Remove(tile);
                _targetReservations.Remove(tile);
            }
        }

        /// <summary>Remove a pet from tracking (e.g., if pet is removed from game).</summary>
        public void RemovePet(Pet pet)
        {
            if (pet == null) return;
            
            Guid petGuid = pet.petId.Value;
            
            ReleaseAllTargetsForPet(pet);
            _petManagers.Remove(petGuid);
            
            ModEntry.Instance.Monitor.Log($"[MultiPetManager] Removed pet: {pet.Name}", LogLevel.Debug);
        }
    }
}
