using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Pathfinding;
using StardewValley.TerrainFeatures;
using StardewValley.Characters;
using StardewValley.BellsAndWhistles;

namespace WorkingPets.Behaviors
{
    /// <summary>Work type categories for priority system.</summary>
    public enum WorkType
    {
        Debris,
        StumpsAndLogs,
        Trees,
        Boulders
    }

    /// <summary>Manages pet work behavior.</summary>
    public class PetWorkManager
    {
        /*********
        ** Constants
        *********/
        private const int TREE_HEALTH = 10;
        private const int STUMP_HEALTH = 5;
        private const int BOULDER_HEALTH = 8;
        private const int FORAGE_DETECTION_RADIUS = 20; // tiles around pet to check for forageable items
        
        // ===== FOLLOWING BEHAVIOR CONSTANTS =====
        // Distance thresholds (in pixels, 64px = 1 tile)
        private const float FOLLOW_STOP_DISTANCE = 80f;       // Stop when this close to player (~1.25 tiles)
        private const float FOLLOW_START_DISTANCE = 160f;     // Start following when farther than this (~2.5 tiles)
        private const float FORAGE_LEASH_DISTANCE = 640f;     // Max distance from player to forage (10 tiles)
        
        // Speed settings
        private const float BASE_FOLLOW_SPEED = 5f;           // Base following speed (slightly faster)
        private const float BASE_FORAGE_SPEED = 5.5f;         // Speed when going to forageables
        private const float WALL_PASS_SPEED = 50f;            // EXTREME BLINK speed - phases through EVERYTHING
        private const float WALL_PASS_LERP_IN = 0.8f;         // Almost instant acceleration
        private const float WALL_PASS_LERP_OUT = 0.25f;       // Decelerate out of wall-pass
        
        // Stuck detection thresholds
        private const int STUCK_THRESHOLD_TICKS = 30;         // ~0.5 seconds before activating wall-pass
        private const int WALL_PASS_MAX_TICKS = 240;          // Max ~4 seconds in wall-pass mode (for long distances like beach)
        private const float STUCK_MOVEMENT_THRESHOLD = 1.5f;  // Min pixels moved to not be "stuck"
        private const int FORAGE_STUCK_THRESHOLD = 30;        // 0.5 second before activating wall-pass for forage
        
        // Idle roaming (when close to stationary player)
        private const float IDLE_ROAM_CHANCE = 0.02f;         // 2% chance per tick to start roaming
        private const float IDLE_ROAM_RADIUS = 48f;           // Max 0.75 tiles roam distance
        private const int IDLE_ROAM_MIN_TICKS = 30;           // Min ticks between roams
        private const float PLAYER_STATIONARY_THRESHOLD = 2f; // Player considered stationary if moved less than this
        
        // Direction locking
        private const int DIRECTION_LOCK_TICKS = 6;           // Min ticks before changing direction
        private const float DIRECTION_CHANGE_THRESHOLD = 0.4f; // How different direction must be to change
        
        // Scanning intervals
        private const int FORAGE_SCAN_INTERVAL = 12;          // Ticks between forage scans
        private const int UNREACHABLE_CLEAR_INTERVAL = 600;   // Clear unreachable list every 10 seconds
        
        // ===== EXPLORE MODE CONSTANTS =====
        private const int EXPLORE_SCAN_RADIUS = 50;           // Scan 50 tiles for forageables
        private const int EXPLORE_AREA_DONE_DELAY = 120;      // 2 seconds before moving to next area
        private const float EXPLORE_FORAGE_SPEED = 6f;        // Speed when exploring
        private const int EXPLORE_TRANSITION_TIMEOUT = 900;   // 15 seconds before fallback teleport
        
        // Valid exploration areas (no Ginger Island, no Desert, no special areas)
        private static readonly string[] EXPLORE_AREAS = new[]
        {
            "Mountain", "Forest", "Town", "Beach"
        };

        /*********
        ** Fields
        *********/
        private Pet? _pet;
        private bool _isWorking;
        private bool _isFollowing;
        private bool _isExploring;  // NEW: Autonomous explore mode
        private int _tickCounter;
        private bool _isPausedForDialogue;
        
        // Movement & Action State (for work mode)
        private Vector2? _targetTile;
        private Action? _pendingAction;
        private readonly float _moveSpeed = 3f;
        private float _currentSpeed = 3f;
        private bool _isInFastMode = false;
        
        // Track unreachable targets to avoid trying them repeatedly
        private readonly HashSet<Vector2> _unreachableTiles = new();
        private int _unreachableClearTimer;
        
        // Track when we've notified about no work available
        private bool _noWorkNotificationShown = false;

        // Stuck detection for work mode
        private float _lastDistanceToTarget = float.MaxValue;
        private int _noProgressTicks;
        private Vector2? _lastTargetTile;
        private int _consecutiveFastAttempts;
        private const int MAX_FAST_ATTEMPTS = 3;

        // ===== FOLLOW MODE STATE =====
        // Direction locking to prevent flickering
        private int _currentDirection = 2; // 0=up, 1=right, 2=down, 3=left
        private int _directionLockTimer = 0;
        
        // Follow state machine
        private enum FollowState { Idle, FollowingPlayer, Foraging, WallPassing }
        private FollowState _followState = FollowState.Idle;
        
        // Movement
        private Vector2 _velocity = Vector2.Zero;
        private float _currentFollowSpeed = BASE_FOLLOW_SPEED;
        
        // Stuck detection for follow mode
        private Vector2 _lastPosition = Vector2.Zero;
        private int _stuckTicks = 0;
        private int _wallPassTicks = 0;
        private Vector2 _wallPassDirection = Vector2.Zero; // Lock direction during wall-pass
        
        // Foraging state
        private Vector2? _foragingTarget;
        private int _forageScanTimer = 0;
        private int _forageStuckTicks = 0;
        private HashSet<Vector2> _unreachableForageTiles = new();
        private int _unreachableForageClearTimer = 0;
        
        // Wall-pass target (can be player OR forage target)
        private Vector2? _wallPassTarget = null;
        private bool _wallPassingToForage = false; // True if wall-passing to forage, false if to player
        
        // Idle roaming state
        private Vector2? _roamTarget = null;
        private int _idleRoamCooldown = 0;
        private Vector2 _lastPlayerPosition = Vector2.Zero;
        private int _playerStationaryTicks = 0;
        
        // Animation state
        private bool _wasMoving = false;
        // ===== END FOLLOW MODE STATE =====
        
        // ===== EXPLORE MODE STATE =====
        private Vector2? _exploreTarget;
        private int _exploreScanTimer = 0;
        private int _exploreStuckTicks = 0;
        private int _exploreAreaDoneTimer = 0;
        private bool _exploreAreaComplete = false;
        private HashSet<Vector2> _exploreUnreachableTiles = new();
        private int _currentExploreAreaIndex = 0;
        private string? _exploreStartArea;
        private bool _exploreHasLeftStartArea = false;
        private bool _exploreEndWhenArriveAtStart = false;
        private bool _exploreInTransit = false;
        private readonly Queue<string> _exploreTravelPath = new();
        private string? _exploreTransitionTargetLocation;
        private Vector2? _exploreTransitionWarpTile;
        private Vector2? _exploreTransitionTargetTile;
        private int _exploreTransitionTicks = 0;
        // ===== END EXPLORE MODE STATE =====
        
        // ===== AUTO-EXPLORE STATE =====
        private bool _hasExploredToday = false;
        private int _idleTicks = 0;
        private const int AUTO_EXPLORE_IDLE_THRESHOLD = 1800; // 30 seconds of idle before auto-explore
        // ===== END AUTO-EXPLORE STATE =====

        private readonly Random _random = new();

        // Track damage for multi-hit objects
        private readonly Dictionary<Vector2, int> _objectDamage = new();
        
        // Current reserved target (for multi-pet coordination)
        private Vector2? _reservedTarget;

        /*********
        ** Properties
        *********/
        public bool IsWorking => _isWorking;
        public bool IsFollowing => _isFollowing;
        public bool IsExploring => _isExploring;
        public Pet? Pet => _pet;
        
        /// <summary>Get the current explore location name for display.</summary>
        public string? CurrentExploreLocation => _isExploring ? _pet?.currentLocation?.Name : null;

        /*********
        ** Public methods
        *********/
        public void Initialize(Pet pet)
        {
            _pet = pet;
            LoadState(pet);
        }
        
        /// <summary>Reset daily flags (call at start of new day).</summary>
        public void ResetDailyFlags()
        {
            _hasExploredToday = false;
            _idleTicks = 0;
        }
        
        /// <summary>Check if inventory is full and stop work/explore if needed.</summary>
        private bool CheckInventoryFull()
        {
            if (!ModEntry.InventoryManager.IsFull)
                return false;
            
            // Inventory is full - stop all collecting activities
            if (_isExploring)
            {
                Game1.addHUDMessage(new HUDMessage($"{_pet?.Name}'s inventory is full!", HUDMessage.error_type));
                StopExploringReturnToFarm();
            }
            else if (_isWorking)
            {
                Game1.addHUDMessage(new HUDMessage($"{_pet?.Name}'s inventory is full!", HUDMessage.error_type));
                _isWorking = false;
                StopWorkImmediately();
            }
            else if (_foragingTarget != null)
            {
                // Only show notification once when stopping forage during follow
                Game1.addHUDMessage(new HUDMessage($"{_pet?.Name}'s inventory is full!", HUDMessage.error_type));
            }
            
            // Clear forage target if following
            _foragingTarget = null;
            _exploreTarget = null;
            
            return true;
        }
        
        /// <summary>Stop exploring and return to farm (not player).</summary>
        private void StopExploringReturnToFarm()
        {
            if (_isExploring)
            {
                _isExploring = false;
                ResetExploreState();
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = false;
                    _pet.Halt();
                    _pet.controller = null;
                    
                    // Safely warp to farm
                    try
                    {
                        var farm = Game1.getFarm();
                        if (farm != null)
                        {
                            Game1.warpCharacter(_pet, farm, new Vector2(64, 15));
                        }
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Instance.Monitor.Log($"[WorkingPets] Error warping pet to farm: {ex.Message}", LogLevel.Warn);
                    }
                }
            }
        }

        public void ToggleWork()
        {
            _isWorking = !_isWorking;
            _tickCounter = 0;
            _targetTile = null;
            _pendingAction = null;
            _noWorkNotificationShown = false; // Reset notification flag when toggling

            if (_isWorking)
            {


            }
            else
            {

                StopWorkImmediately();
            }
        }

        public void ToggleFollow()
        {
            _isFollowing = !_isFollowing;
            
            // Stop exploring if we start following (same as whistle behavior)
            if (_isFollowing && _isExploring)
            {
                _isExploring = false;
                ResetExploreState();
            }
            
            // Stop working if we start following
            if (_isFollowing)
            {
                _isWorking = false;
                _targetTile = null;
                _pendingAction = null;
                ResetFollowState(); // Initialize follow state
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = true; // Make passable when following
                    _pet.controller = null; // Clear any vanilla pathfinding
                    _pet.Halt();
                }

            }
            else
            {
                ResetFollowState(); // Clear follow state
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = false; // Restore collision when not following
                    _pet.Halt();
                }

            }
        }

        public void StopFollowing()
        {
            if (_isFollowing)
            {
                _isFollowing = false;
                ResetFollowState();
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = false; // Restore collision
                    _pet.Halt();
                }

            }
        }

        /// <summary>Toggle autonomous explore mode - pet forages across the valley on its own.</summary>
        public void ToggleExplore()
        {
            if (_isExploring)
            {
                _isExploring = false;
                ResetExploreState();
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = false;
                    _pet.Halt();
                    _pet.IsInvisible = false; // Ensure visible when stopping
                }
                return;
            }

            if (_hasExploredToday)
            {
                return; // Only explore once per day
            }

            if (_pet?.currentLocation == null || !IsValidExploreArea(_pet.currentLocation.Name))
            {
                return; // Only start exploring in allowed exterior areas
            }

            _isExploring = true;
            _hasExploredToday = true;
            
            if (_isExploring)
            {
                // Stop other modes
                _isWorking = false;
                _isFollowing = false;
                _targetTile = null;
                _pendingAction = null;
                ResetFollowState();
                ResetExploreAreaState();
                _exploreStartArea = _pet.currentLocation.Name;
                _exploreHasLeftStartArea = false;
                _exploreEndWhenArriveAtStart = false;
                _currentExploreAreaIndex = Array.IndexOf(EXPLORE_AREAS, _exploreStartArea);
                if (_currentExploreAreaIndex < 0)
                    _currentExploreAreaIndex = 0;
                
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = true;
                    _pet.controller = null;
                    _pet.Halt();
                }
            }
        }
        
        /// <summary>Stop exploring and return to player.</summary>
        public void StopExploring()
        {
            if (_isExploring)
            {
                _isExploring = false;
                ResetExploreState();
                if (_pet != null)
                {
                    _pet.farmerPassesThrough = false;
                    _pet.Halt();
                    _pet.controller = null;
                    _pet.IsInvisible = false; // Ensure visible when stopping;
                    
                    // Safely warp back to player
                    try
                    {
                        var player = Game1.player;
                        if (player?.currentLocation != null && !Game1.eventUp && !Game1.isFestival())
                        {
                            // Use DelayedAction to warp after menu closes
                            DelayedAction.functionAfterDelay(() =>
                            {
                                if (_pet != null && Game1.player?.currentLocation != null)
                                {
                                    Game1.warpCharacter(_pet, Game1.player.currentLocation, Game1.player.Tile);
                                }
                            }, 100);
                        }
                        else
                        {
                            // Fallback: warp to farm
                            var farm = Game1.getFarm();
                            if (farm != null)
                            {
                                Game1.warpCharacter(_pet, farm, new Vector2(64, 15));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModEntry.Instance.Monitor.Log($"[WorkingPets] Error warping pet: {ex.Message}", LogLevel.Warn);
                    }
                }
            }
        }

        public void PauseForDialogue()
        {
            _isPausedForDialogue = true;
            if (_pet != null)
            {
                _pet.Halt();
                _pet.controller = null;
            }
        }

        public void ResumeFromDialogue()
        {
            _isPausedForDialogue = false;
        }

        public void Update()
        {
            if (_pet == null)
            {
                return;
            }

            // Pause all movement and actions while dialogue is active
            if (_isPausedForDialogue)
            {
                return;
            }
            
            // === CHECK IF INVENTORY IS FULL ===
            if (CheckInventoryFull())
            {
                return; // Stop all activities if inventory full
            }

            // Handle explore mode - autonomous foraging across the valley
            if (_isExploring)
            {
                _idleTicks = 0; // Not idle while exploring
                UpdateExplore();
                return;
            }

            // Handle following mode - following overrides resting state
            // The pet should follow even if it was sleeping
            if (_isFollowing)
            {
                _idleTicks = 0; // Not idle while following
                UpdateFollow();
                return;
            }

            // Handle working mode
            if (_isWorking)
            {
                _idleTicks = 0; // Not idle while working
            }

            // Let the pet rest if it's sleeping / showing the Zzz emote.
            // This avoids the mod force-moving the pet while the game wants it to be asleep.
            // (Only applies to work mode, not follow mode)
            if (IsPetResting())
            {
                return;
            }

            if (!_isWorking)
            {
                // === AUTO-EXPLORE ONCE PER DAY ===
                // If pet is idle for too long and hasn't explored today, start exploring
                if (!_hasExploredToday && ModEntry.InventoryManager.HasSpace)
                {
                    _idleTicks++;
                    if (_idleTicks >= AUTO_EXPLORE_IDLE_THRESHOLD)
                    {
                        _idleTicks = 0;
                        
                        // Start exploring autonomously (silent - no debug log)
                        ToggleExplore();
                    }
                }
                return;
            }

            Farm? farm = Game1.getFarm();
            if (farm == null)
            {
                return;
            }

            // Ensure pet is on farm when working
            if (_pet.currentLocation?.Name != "Farm")
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Pet not on farm, warping. Current location: {_pet.currentLocation?.Name ?? "null"}", LogLevel.Debug);
                Game1.warpCharacter(_pet, "Farm", new Vector2(64, 15));
                return;
            }

            // If we have a target, move towards it
            if (_targetTile.HasValue)
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Moving to target: {_targetTile.Value}, HasController: {_pet.controller != null}", LogLevel.Trace);
                MoveTowardTarget();
                return;
            }

            // Otherwise, scan for work periodically
            _tickCounter++;
            if (_tickCounter < ModEntry.Config.TicksBetweenActions)
                return;
            

            _tickCounter = 0;

            ScanForWork(farm);
        }

        public void SaveState(Pet pet)
        {
            if (pet == null) return;
            pet.modData["WorkingPets.IsWorking"] = _isWorking.ToString();
        }

        public void LoadState(Pet pet)
        {
            if (pet == null) return;
            if (pet.modData.TryGetValue("WorkingPets.IsWorking", out string? value))
            {
                _isWorking = bool.TryParse(value, out bool result) && result;
            }
        }

        /*********
        ** Private methods
        *********/
        
        private void StopWorkImmediately()
        {
            if (_pet == null) return;

            _pet.Halt();
            _pet.controller = null;
            
            // Release any reserved target
            if (_reservedTarget.HasValue)
            {
                MultiPetManager.ReleaseTarget(_pet, _reservedTarget.Value);
                _reservedTarget = null;
            }
            
            _targetTile = null;
            _pendingAction = null;
        }

        private bool IsPetResting()
        {
            if (_pet == null)
            {
                return false;
            }

            // Bed sleeping state.
            if (_pet.isSleepingOnFarmerBed?.Value == true)
            {
                return true;
            }

            // Emotes: Zzz (sleep) and (sometimes) sad.
            // Note: the backing fields are protected; use the public properties.
            if (_pet.IsEmoting && (_pet.CurrentEmote == Character.sleepEmote || _pet.CurrentEmote == Character.sadEmote))
            {
                return true;
            }

            // General sleep behavior.
            if (string.Equals(_pet.CurrentBehavior, Pet.behavior_Sleep, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private void MoveTowardTarget()
        {
            if (_pet == null || !_targetTile.HasValue || _pet.currentLocation == null) return;

            Vector2 targetPos = _targetTile.Value * 64f + new Vector2(32, 32);
            Vector2 currentPos = _pet.Position + new Vector2(32, 32);
            float distance = Vector2.Distance(currentPos, targetPos);

            // Detect if we're not making progress toward the target.
            // If we get "stuck", activate fast mode to bypass obstacles.
            if (!_lastTargetTile.HasValue || _lastTargetTile.Value != _targetTile.Value)
            {
                _lastTargetTile = _targetTile.Value;
                _lastDistanceToTarget = distance;
                _noProgressTicks = 0;
                _consecutiveFastAttempts = 0; // Reset fast counter for new target
                _isInFastMode = false;
                _currentSpeed = _moveSpeed;
            }
            else
            {
                // Consider it progress only if we got meaningfully closer.
                if (distance < _lastDistanceToTarget - 1f)
                {
                    _lastDistanceToTarget = distance;
                    _noProgressTicks = 0;
                    // DO NOT reset _consecutiveFastAttempts here - only reset on actual arrival
                }
                else
                {
                    _noProgressTicks++;
                }

                // After ~2 seconds with no progress, activate fast speed mode to bypass obstacles.
                if (_noProgressTicks >= 120 && !_isInFastMode)
                {
                    _consecutiveFastAttempts++;
                    _isInFastMode = true;
                    _currentSpeed = _moveSpeed * 6f; // Ultra-fast speed to pass through walls
                    
                    ModEntry.Instance.Monitor.Log($"[WorkingPets] Stuck detected; activating fast-speed bypass (attempt {_consecutiveFastAttempts}/{MAX_FAST_ATTEMPTS}) for target {_targetTile.Value}", LogLevel.Debug);
                    
                    // After 3 fast attempts, give up on pathfinding and just destroy the target
                    if (_consecutiveFastAttempts >= MAX_FAST_ATTEMPTS)
                    {
                        ModEntry.Instance.Monitor.Log($"[WorkingPets] Failed to reach target after {MAX_FAST_ATTEMPTS} fast-speed attempts; destroying target directly at {_targetTile.Value}", LogLevel.Warn);
                        
                        // Execute the action without moving (destroy target from distance)
                        _pet.Halt();
                        _pendingAction?.Invoke();
                        
                        // Release the reserved target
                        if (_reservedTarget.HasValue)
                        {
                            MultiPetManager.ReleaseTarget(_pet, _reservedTarget.Value);
                            _reservedTarget = null;
                        }
                        
                        _targetTile = null;
                        _pendingAction = null;
                        _consecutiveFastAttempts = 0;
                        _lastTargetTile = null;
                        _lastDistanceToTarget = float.MaxValue;
                        _noProgressTicks = 0;
                        _isInFastMode = false;
                        _currentSpeed = _moveSpeed;
                        return;
                    }
                }
            }

            // Arrived!
            if (distance < 55f)
            {

                _pet.Halt();
                _pendingAction?.Invoke();
                
                // Release the reserved target
                if (_reservedTarget.HasValue)
                {
                    MultiPetManager.ReleaseTarget(_pet, _reservedTarget.Value);
                    _reservedTarget = null;
                }
                
                _targetTile = null;
                _pendingAction = null;

                _lastTargetTile = null;
                _lastDistanceToTarget = float.MaxValue;
                _noProgressTicks = 0;
                _consecutiveFastAttempts = 0;
                _isInFastMode = false;
                _currentSpeed = _moveSpeed;
                return;
            }

            // Calculate next position
            Vector2 direction = targetPos - currentPos;
            direction.Normalize();
            Vector2 nextPos = _pet.Position + direction * _currentSpeed;
            
            // When in fast mode, ignore water/obstacles (pass through walls)
            // Otherwise check for water/obstacles
            var location = _pet.currentLocation;
            if (!_isInFastMode && IsNearWaterOrBlocked(location, nextPos))
            {
                // Not in fast mode and blocked - activate fast mode to bypass
                _noProgressTicks++;
                if (_noProgressTicks >= 60)  // Shorter threshold in normal mode
                {
                    _consecutiveFastAttempts++;
                    _isInFastMode = true;
                    _currentSpeed = _moveSpeed * 6f;
                    ModEntry.Instance.Monitor.Log($"[WorkingPets] Obstacle detected; activating fast-speed bypass for target {_targetTile.Value}", LogLevel.Debug);
                }
                return;
            }

            // Move the pet
            // Add collision avoidance nudge to prevent pets from stacking on each other (but skip if in fast mode)
            Vector2 finalPos = nextPos;
            if (!_isInFastMode)
            {
                var allPets = MultiPetManager.GetAllPets();
                Vector2 avoidanceNudge = MultiPetManager.GetCollisionAvoidanceNudge(_pet, allPets);
                finalPos = nextPos + avoidanceNudge;
            }
            
            _pet.Position = finalPos;

            // Face the right direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                _pet.faceDirection(direction.X > 0 ? 1 : 3);
            else
                _pet.faceDirection(direction.Y > 0 ? 2 : 0);

            _pet.animateInFacingDirection(Game1.currentGameTime);
        }

        private bool IsNearWaterOrBlocked(GameLocation location, Vector2 pixelPos)
        {
            // Check the corners and center of the pet's bounding box
            // Pet sprite is roughly 64x64, so check points around the position
            float[] offsetsX = { -24, 0, 24 };
            float[] offsetsY = { -24, 0, 24 };

            foreach (float ox in offsetsX)
            {
                foreach (float oy in offsetsY)
                {
                    int tileX = (int)((pixelPos.X + ox) / 64);
                    int tileY = (int)((pixelPos.Y + oy) / 64);

                    if (location.isWaterTile(tileX, tileY))
                        return true;
                }
            }

            // Also check if the main tile is impassable
            int mainTileX = (int)(pixelPos.X / 64);
            int mainTileY = (int)(pixelPos.Y / 64);
            if (!location.isTilePassable(new xTile.Dimensions.Location(mainTileX, mainTileY), Game1.viewport))
                return true;

            return false;
        }

        private void UpdateFollow()
        {
            if (_pet == null) return;

            Farmer player = Game1.player;
            var petLocation = _pet.currentLocation;
            
            // === TAKE FULL CONTROL OF THE PET ===
            _pet.farmerPassesThrough = true;
            _pet.controller = null;
            
            // Force pet out of sleep behavior while following
            if (_pet.CurrentBehavior == Pet.behavior_Sleep || _pet.CurrentBehavior == "SitDown")
            {
                _pet.CurrentBehavior = "Walk";
            }
            
            // === HANDLE LOCATION CHANGES ===
            if (player.currentLocation?.Name != "Farm" && !ModEntry.Config.FollowOutsideFarm)
            {
                _isFollowing = false;
                _pet.farmerPassesThrough = false;
                _followState = FollowState.Idle;
                ExitWallPassMode();
                return;
            }
            
            // Warp pet to player's location if different
            if (_pet.currentLocation != player.currentLocation)
            {
                if (player.currentLocation?.Name == "Farm" || ModEntry.Config.FollowOutsideFarm)
                {
                    Game1.warpCharacter(_pet, player.currentLocation, player.Tile);
                    ResetFollowState();
                }
                return;
            }

            // === CLEAR UNREACHABLE FORAGE PERIODICALLY ===
            _unreachableForageClearTimer++;
            if (_unreachableForageClearTimer > UNREACHABLE_CLEAR_INTERVAL)
            {
                _unreachableForageTiles.Clear();
                _unreachableForageClearTimer = 0;
            }

            Vector2 petPos = _pet.Position;
            Vector2 playerPos = player.Position;
            float distanceToPlayer = Vector2.Distance(petPos, playerPos);
            
            // === TRACK PLAYER MOVEMENT FOR IDLE ROAMING ===
            float playerMoved = Vector2.Distance(playerPos, _lastPlayerPosition);
            if (playerMoved > PLAYER_STATIONARY_THRESHOLD)
            {
                _playerStationaryTicks = 0;
                _roamTarget = null; // Cancel roaming if player moves
            }
            else
            {
                _playerStationaryTicks++;
            }
            _lastPlayerPosition = playerPos;
            
            if (_idleRoamCooldown > 0) _idleRoamCooldown--;

            // === WALL-PASS MODE HANDLING (WORKS FOR BOTH PLAYER AND FORAGE) ===
            if (_followState == FollowState.WallPassing && _wallPassTarget.HasValue)
            {
                _wallPassTicks++;
                
                Vector2 target = _wallPassTarget.Value;
                float distToTarget = Vector2.Distance(petPos, target);
                float stopDist = _wallPassingToForage ? 64f : FOLLOW_STOP_DISTANCE;
                
                // Check if forage target still valid
                if (_wallPassingToForage && _foragingTarget.HasValue && petLocation != null)
                {
                    if (!IsForageTargetStillValid(petLocation, _foragingTarget.Value))
                    {
                        // Forage gone, exit wall-pass
                        _foragingTarget = null;
                        ExitWallPassMode();
                        _followState = FollowState.Idle;
                        _lastPosition = petPos;
                        return;
                    }
                }
                
                // Check exit conditions
                bool reachedTarget = distToTarget <= stopDist;
                bool timedOut = _wallPassTicks >= WALL_PASS_MAX_TICKS;
                
                if (reachedTarget)
                {
                    // Successfully reached target
                    if (_wallPassingToForage && _foragingTarget.HasValue && petLocation != null)
                    {
                        // Pick up the forage
                        TryPickupForageableAt(petLocation, _foragingTarget.Value);
                        _foragingTarget = null;
                    }
                    ExitWallPassMode();
                    SyncDirectionToVelocity(); // FIX MOONWALKING - sync direction after arrival
                    StopMovingSmooth();
                    _lastPosition = petPos;
                    return;
                }
                
                if (timedOut)
                {
                    // Wall-pass took too long
                    if (_wallPassingToForage && _foragingTarget.HasValue)
                    {
                        // Mark as unreachable
                        _unreachableForageTiles.Add(_foragingTarget.Value);
                        _foragingTarget = null;
                    }
                    ExitWallPassMode();
                    _stuckTicks = 0;
                    _forageStuckTicks = 0;
                }
                else
                {
                    // Continue wall-passing toward target (BLINK SPEED!)
                    MoveWithWallPass(target);
                    _lastPosition = petPos;
                    return;
                }
            }

            // === SCAN FOR FORAGEABLES (TAKES PRIORITY) ===
            if (ModEntry.Config.ForageWhileFollowing && petLocation != null && _followState != FollowState.WallPassing)
            {
                _forageScanTimer++;
                if (_forageScanTimer >= FORAGE_SCAN_INTERVAL)
                {
                    _forageScanTimer = 0;
                    
                    if (distanceToPlayer <= FORAGE_LEASH_DISTANCE && !_foragingTarget.HasValue)
                    {
                        var nearestForage = FindNearbyForageableSmooth(petLocation, _pet.Tile);
                        if (nearestForage.HasValue)
                        {
                            _foragingTarget = nearestForage.Value;
                            _followState = FollowState.Foraging;
                            _forageStuckTicks = 0;
                            _stuckTicks = 0;
                            SyncDirectionToTarget(_foragingTarget.Value * 64f + new Vector2(32f, 32f)); // Face new target
                        }
                    }
                }
            }

            // === FORAGING TAKES ABSOLUTE PRIORITY ===
            if (_foragingTarget.HasValue && ModEntry.Config.ForageWhileFollowing && petLocation != null)
            {
                if (!IsForageTargetStillValid(petLocation, _foragingTarget.Value))
                {
                    // Target gone, immediately scan for another forage
                    _foragingTarget = null;
                    _forageStuckTicks = 0;
                    
                    // IMMEDIATELY look for next forageable before returning to player
                    var nextForage = FindNearbyForageableSmooth(petLocation, _pet.Tile);
                    if (nextForage.HasValue)
                    {
                        _foragingTarget = nextForage.Value;
                        _followState = FollowState.Foraging;
                        SyncDirectionToTarget(_foragingTarget.Value * 64f + new Vector2(32f, 32f));
                    }
                    else
                    {
                        _followState = FollowState.Idle;
                        SyncDirectionToTarget(playerPos);
                    }
                }
                else if (Vector2.Distance(_pet.Tile, _foragingTarget.Value) < 1.5f)
                {
                    // Close enough to pick up
                    TryPickupForageableAt(petLocation, _foragingTarget.Value);
                    _foragingTarget = null;
                    _forageStuckTicks = 0;
                    
                    // IMMEDIATELY look for next forageable - DON'T go back to player yet!
                    var nextForage = FindNearbyForageableSmooth(petLocation, _pet.Tile);
                    if (nextForage.HasValue)
                    {
                        _foragingTarget = nextForage.Value;
                        _followState = FollowState.Foraging;
                        _forageScanTimer = 0;
                        SyncDirectionToTarget(_foragingTarget.Value * 64f + new Vector2(32f, 32f));
                        // Don't stop moving - keep going!
                        return;
                    }
                    else
                    {
                        // No more forageables, NOW go back to player
                        _followState = FollowState.Idle;
                        _forageScanTimer = 0;
                        SyncDirectionToTarget(playerPos);
                        StopMovingSmooth();
                    }
                }
                else
                {
                    // Move to forage - PET IGNORES PLAYER COMPLETELY
                    _followState = FollowState.Foraging;
                    Vector2 targetPos = _foragingTarget.Value * 64f + new Vector2(32f, 32f);
                    bool moved = MoveTowardTarget(targetPos, BASE_FORAGE_SPEED, false);
                    
                    // Forage stuck detection - activate WALL-PASS if stuck
                    float movedDist = Vector2.Distance(petPos, _lastPosition);
                    if (!moved || movedDist < STUCK_MOVEMENT_THRESHOLD)
                    {
                        _forageStuckTicks++;
                        if (_forageStuckTicks >= FORAGE_STUCK_THRESHOLD)
                        {
                            // ACTIVATE WALL-PASS TO REACH FORAGE
                            EnterWallPassModeForForage(targetPos);
                        }
                    }
                    else
                    {
                        _forageStuckTicks = 0;
                    }
                    
                    _lastPosition = petPos;
                    return;
                }
            }

            // === FOLLOWING THE PLAYER ===
            _followState = FollowState.FollowingPlayer;
            
            // If close enough, stop and maybe do idle roaming
            if (distanceToPlayer <= FOLLOW_STOP_DISTANCE)
            {
                StopMovingSmooth();
                _stuckTicks = 0;
                _lastPosition = petPos;
                
                // === IDLE ROAMING ===
                TryIdleRoam(petLocation, playerPos);
                return;
            }
            
            // If within comfortable range and player stationary, allow idle roaming
            if (distanceToPlayer <= FOLLOW_START_DISTANCE)
            {
                StopMovingSmooth();
                _stuckTicks = 0;
                _lastPosition = petPos;
                
                // === IDLE ROAMING ===
                TryIdleRoam(petLocation, playerPos);
                return;
            }
            
            // Cancel roaming if we need to follow
            _roamTarget = null;
            
            // === MOVE TOWARD PLAYER ===
            float speed = CalculateFollowSpeed(distanceToPlayer);
            bool successfulMove = MoveTowardTarget(playerPos, speed, false);
            
            // Always animate while trying to follow (even if stuck)
            if (!successfulMove)
            {
                // Update direction to face player even when stuck
                Vector2 direction = playerPos - petPos;
                if (direction.LengthSquared() > 0)
                {
                    direction.Normalize();
                    UpdateDirectionSmooth(direction);
                }
                AnimatePetMovement(); // Still animate to show trying to move
            }
            
            // === STUCK DETECTION - TRIGGER WALL-PASS ===
            float movedDistance = Vector2.Distance(petPos, _lastPosition);
            if (movedDistance < STUCK_MOVEMENT_THRESHOLD)
            {
                _stuckTicks++;
                if (_stuckTicks >= STUCK_THRESHOLD_TICKS)
                {
                    // ACTIVATE WALL-PASS MODE
                    EnterWallPassMode(playerPos);
                }
            }
            else
            {
                _stuckTicks = 0;
            }
            
            _lastPosition = petPos;
        }

        /// <summary>Calculate follow speed based on distance to player.</summary>
        private float CalculateFollowSpeed(float distanceToPlayer)
        {
            float speed = BASE_FOLLOW_SPEED;
            if (distanceToPlayer > 512f) speed *= 2.2f;       // 8+ tiles: sprint hard
            else if (distanceToPlayer > 384f) speed *= 1.8f;  // 6+ tiles: sprint
            else if (distanceToPlayer > 256f) speed *= 1.4f;  // 4+ tiles: jog
            else if (distanceToPlayer > 160f) speed *= 1.1f;  // 2.5+ tiles: walk fast
            return speed;
        }
        
        /// <summary>Try to do idle roaming when player is stationary and pet is close.</summary>
        private void TryIdleRoam(GameLocation? location, Vector2 playerPos)
        {
            if (_pet == null || location == null) return;
            
            // If we have an active roam target, move toward it
            if (_roamTarget.HasValue)
            {
                Vector2 roamTargetPos = _roamTarget.Value;
                float distToRoam = Vector2.Distance(_pet.Position, roamTargetPos);
                
                if (distToRoam < 8f)
                {
                    // Reached roam target
                    _roamTarget = null;
                    _idleRoamCooldown = IDLE_ROAM_MIN_TICKS;
                    StopMovingSmooth();
                    return;
                }
                
                // Move slowly toward roam target
                MoveTowardTarget(roamTargetPos, 1.5f, false);
                return;
            }
            
            // Only roam if player has been stationary for a while
            if (_playerStationaryTicks < 60) return; // Wait 1 second of stationary
            if (_idleRoamCooldown > 0) return;
            
            // Random chance to start roaming
            if (_random.NextDouble() > IDLE_ROAM_CHANCE) return;
            
            // Pick a random nearby position
            float angle = (float)(_random.NextDouble() * Math.PI * 2);
            float distance = (float)(_random.NextDouble() * IDLE_ROAM_RADIUS);
            Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * distance;
            Vector2 potentialTarget = _pet.Position + offset;
            
            // Make sure it's walkable and doesn't go too far from player
            if (IsPositionWalkable(location, potentialTarget) && 
                Vector2.Distance(potentialTarget, playerPos) <= FOLLOW_START_DISTANCE + IDLE_ROAM_RADIUS)
            {
                _roamTarget = potentialTarget;
            }
        }
        
        /// <summary>Sync facing direction to current velocity (fixes moonwalking).</summary>
        private void SyncDirectionToVelocity()
        {
            if (_pet == null || _velocity.LengthSquared() < 0.01f) return;
            
            Vector2 dir = _velocity;
            dir.Normalize();
            
            int newDir = _currentDirection;
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
                newDir = dir.X > 0 ? 1 : 3;
            else
                newDir = dir.Y > 0 ? 2 : 0;
            
            _currentDirection = newDir;
            _pet.faceDirection(_currentDirection);
            _directionLockTimer = DIRECTION_LOCK_TICKS;
        }
        
        /// <summary>Sync facing direction to a target position (fixes moonwalking on state change).</summary>
        private void SyncDirectionToTarget(Vector2 targetPos)
        {
            if (_pet == null) return;
            
            Vector2 dir = targetPos - _pet.Position;
            if (dir.LengthSquared() < 1f) return;
            dir.Normalize();
            
            int newDir = _currentDirection;
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
                newDir = dir.X > 0 ? 1 : 3;
            else
                newDir = dir.Y > 0 ? 2 : 0;
            
            _currentDirection = newDir;
            _pet.faceDirection(_currentDirection);
            _directionLockTimer = DIRECTION_LOCK_TICKS;
        }

        /// <summary>Enter wall-pass mode to bypass obstacles (toward PLAYER).</summary>
        private void EnterWallPassMode(Vector2 targetPos)
        {
            if (_pet == null) return;
            
            float dist = Vector2.Distance(_pet.Position, targetPos);

            
            _followState = FollowState.WallPassing;
            _wallPassTicks = 0;
            _stuckTicks = 0;
            _wallPassTarget = targetPos;
            _wallPassingToForage = false;
            
            // Lock direction toward target
            Vector2 direction = targetPos - _pet.Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                _wallPassDirection = direction;
            }
            
            // Sync direction immediately
            SyncDirectionToTarget(targetPos);
            
            // Start accelerating toward wall-pass speed
            _currentFollowSpeed = Math.Max(_velocity.Length(), BASE_FOLLOW_SPEED);
        }
        
        /// <summary>Enter wall-pass mode to bypass obstacles (toward FORAGE).</summary>
        private void EnterWallPassModeForForage(Vector2 targetPos)
        {
            if (_pet == null) return;
            
            float dist = Vector2.Distance(_pet.Position, targetPos);

            
            _followState = FollowState.WallPassing;
            _wallPassTicks = 0;
            _forageStuckTicks = 0;
            _wallPassTarget = targetPos;
            _wallPassingToForage = true;
            
            // Lock direction toward target
            Vector2 direction = targetPos - _pet.Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                _wallPassDirection = direction;
            }
            
            // Sync direction immediately
            SyncDirectionToTarget(targetPos);
            
            // Start accelerating toward wall-pass speed
            _currentFollowSpeed = Math.Max(_velocity.Length(), BASE_FORAGE_SPEED);
        }

        /// <summary>Exit wall-pass mode.</summary>
        private void ExitWallPassMode()
        {
            if (_followState == FollowState.WallPassing)
            {
                string reason = _wallPassingToForage ? "(reached forage)" : "(reached player)";

            }
            
            _followState = FollowState.Idle;
            _wallPassTicks = 0;
            _wallPassDirection = Vector2.Zero;
            _wallPassTarget = null;
            _wallPassingToForage = false;
            _currentFollowSpeed = BASE_FOLLOW_SPEED;
        }

        /// <summary>Move with wall-pass enabled (ignores obstacles except water).</summary>
        private void MoveWithWallPass(Vector2 targetPos)
        {
            if (_pet == null || _pet.currentLocation == null) return;

            Vector2 petPos = _pet.Position + new Vector2(32f, 32f);
            Vector2 direction = targetPos - petPos;
            
            if (direction.LengthSquared() < 1f)
            {
                ExitWallPassMode();
                return;
            }
            
            direction.Normalize();
            
            // Use locked direction to prevent wobbling during wall-pass
            if (_wallPassDirection != Vector2.Zero)
            {
                // Blend toward current target but mostly keep locked direction
                direction = Vector2.Lerp(_wallPassDirection, direction, 0.1f);
                direction.Normalize();
                _wallPassDirection = direction;
            }
            
            // Smoothly accelerate to wall-pass speed
            _currentFollowSpeed = MathHelper.Lerp(_currentFollowSpeed, WALL_PASS_SPEED, WALL_PASS_LERP_IN);
            
            Vector2 velocity = direction * _currentFollowSpeed;
            Vector2 nextPos = _pet.Position + velocity;
            
            // === PHASE THROUGH EVERYTHING - NO COLLISION CHECKS ===
            // In wall-pass mode, pets ignore water, walls, buildings, fences, EVERYTHING
            _pet.Position = nextPos;
            
            // Log for debugging
            if (_wallPassTicks % 20 == 0)
            {
                float distToTarget = Vector2.Distance(nextPos, targetPos);

            }
            
            // Update facing direction (locked during wall-pass to prevent flickering)
            UpdateDirectionSmooth(direction);
            AnimatePetMovement();
        }

        /// <summary>Move toward a target position with obstacle handling.</summary>
        /// <returns>True if movement was successful.</returns>
        private bool MoveTowardTarget(Vector2 targetPos, float speed, bool allowWallPass)
        {
            if (_pet == null || _pet.currentLocation == null) return false;

            Vector2 petPos = _pet.Position + new Vector2(32f, 32f);
            Vector2 direction = targetPos - petPos;
            float distance = direction.Length();
            
            if (distance < 1f)
            {
                StopMovingSmooth();
                return true;
            }
            
            direction.Normalize();
            
            // Smooth acceleration
            Vector2 desiredVelocity = direction * speed;
            _velocity = Vector2.Lerp(_velocity, desiredVelocity, 0.2f);
            
            Vector2 nextPos = _pet.Position + _velocity;
            
            // Try direct movement first
            if (IsPositionWalkable(_pet.currentLocation, nextPos))
            {
                _pet.Position = nextPos;
                UpdateDirectionSmooth(direction);
                AnimatePetMovement();
                return true;
            }
            
            // Try angled approaches
            float[] angles = { 25f, -25f, 45f, -45f, 70f, -70f };
            foreach (float angle in angles)
            {
                Vector2 rotatedDir = RotateVector(direction, angle);
                Vector2 altPos = _pet.Position + rotatedDir * (speed * 0.85f);
                
                if (IsPositionWalkable(_pet.currentLocation, altPos))
                {
                    _pet.Position = altPos;
                    UpdateDirectionSmooth(rotatedDir);
                    AnimatePetMovement();
                    return true;
                }
            }
            
            // Try sliding along walls
            if (TrySlideMovement(_pet.currentLocation, direction, speed))
            {
                UpdateDirectionSmooth(direction);
                AnimatePetMovement();
                return true;
            }
            
            // Movement failed
            return false;
        }

        /// <summary>Try to slide along obstacles.</summary>
        private bool TrySlideMovement(GameLocation location, Vector2 direction, float speed)
        {
            if (_pet == null) return false;

            float slideSpeed = speed * 0.6f;
            
            // Try horizontal slide
            if (Math.Abs(direction.X) > 0.1f)
            {
                Vector2 hPos = _pet.Position + new Vector2(Math.Sign(direction.X) * slideSpeed, 0);
                if (IsPositionWalkable(location, hPos))
                {
                    _pet.Position = hPos;
                    return true;
                }
            }
            
            // Try vertical slide
            if (Math.Abs(direction.Y) > 0.1f)
            {
                Vector2 vPos = _pet.Position + new Vector2(0, Math.Sign(direction.Y) * slideSpeed);
                if (IsPositionWalkable(location, vPos))
                {
                    _pet.Position = vPos;
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>Reset all follow-related state.</summary>
        private void ResetFollowState()
        {
            _velocity = Vector2.Zero;
            _stuckTicks = 0;
            _wallPassTicks = 0;
            _wallPassDirection = Vector2.Zero;
            _wallPassTarget = null;
            _wallPassingToForage = false;
            _currentFollowSpeed = BASE_FOLLOW_SPEED;
            _foragingTarget = null;
            _forageStuckTicks = 0;
            _followState = FollowState.Idle;
            _directionLockTimer = 0;
            _unreachableForageTiles.Clear();
            _roamTarget = null;
            _idleRoamCooldown = 0;
            _playerStationaryTicks = 0;
            _lastPlayerPosition = Vector2.Zero;
        }
        
        /// <summary>Reset explore state for the current area (keeps daily/loop tracking).</summary>
        private void ResetExploreAreaState()
        {
            _exploreTarget = null;
            _exploreScanTimer = 0;
            _exploreStuckTicks = 0;
            _exploreAreaDoneTimer = 0;
            _exploreAreaComplete = false;
            _exploreUnreachableTiles.Clear();
            _velocity = Vector2.Zero;
            _stuckTicks = 0;
            _wallPassTicks = 0;
            _wallPassDirection = Vector2.Zero;
            _wallPassTarget = null;
            _followState = FollowState.Idle;
        }

        /// <summary>Reset all explore-related state.</summary>
        private void ResetExploreState()
        {
            ResetExploreAreaState();
            _exploreTravelPath.Clear();
            _exploreInTransit = false;
            _exploreTransitionTargetLocation = null;
            _exploreTransitionWarpTile = null;
            _exploreTransitionTargetTile = null;
            _exploreTransitionTicks = 0;
            _exploreStartArea = null;
            _exploreHasLeftStartArea = false;
            _exploreEndWhenArriveAtStart = false;
            _currentExploreAreaIndex = 0;
            // Note: _hasExploredToday is reset daily
        }
        
        /// <summary>Update autonomous explore mode.</summary>
        private void UpdateExplore()
        {
            if (_pet == null) return;

            // === HANDLE AREA TRANSITIONS ===
            if (_exploreInTransit)
            {
                UpdateExploreTransition();
                return;
            }
            
            var location = _pet.currentLocation;
            if (location == null) return;
            
            // === TAKE FULL CONTROL ===
            _pet.farmerPassesThrough = true;
            _pet.controller = null;
            
            // Force out of sleep
            if (_pet.CurrentBehavior == Pet.behavior_Sleep || _pet.CurrentBehavior == "SitDown")
            {
                _pet.CurrentBehavior = "Walk";
            }
            
            // === CHECK IF IN VALID AREA ===
            if (!IsValidExploreArea(location.Name))
            {
                StopExploringReturnToFarm();
                return;
            }
            
            Vector2 petPos = _pet.Position;
            
            // === WALL-PASS MODE (same as follow mode) ===
            if (_followState == FollowState.WallPassing && _wallPassTarget.HasValue)
            {
                _wallPassTicks++;
                
                float distToTarget = Vector2.Distance(petPos, _wallPassTarget.Value);
                
                if (distToTarget <= 64f || _wallPassTicks >= WALL_PASS_MAX_TICKS)
                {
                    ExitWallPassMode();
                    _exploreStuckTicks = 0;
                    
                    // Check if we reached the forage (2 tiles is enough to pick up)
                    if (_exploreTarget.HasValue && Vector2.Distance(_pet.Tile, _exploreTarget.Value) < 2.0f)
                    {
                        TryPickupForageableAt(location, _exploreTarget.Value);
                        _exploreTarget = null;
                    }
                }
                else
                {
                    MoveWithWallPass(_wallPassTarget.Value);
                    _lastPosition = petPos;
                    return;
                }
            }
            
            // === IF AREA IS COMPLETE, WAIT THEN TELEPORT ===
            if (_exploreAreaComplete)
            {
                _exploreAreaDoneTimer++;
                
                // Do a little idle animation
                StopMovingSmooth();
                
                if (_exploreAreaDoneTimer >= EXPLORE_AREA_DONE_DELAY)
                {
                    TryAdvanceExploreArea();
                }
                return;
            }
            
            // === SCAN FOR FORAGEABLES ===
            _exploreScanTimer++;
            if (_exploreScanTimer >= FORAGE_SCAN_INTERVAL && !_exploreTarget.HasValue)
            {
                _exploreScanTimer = 0;
                
                var nearestForage = ScanAreaForForageable(location);
                if (nearestForage.HasValue)
                {
                    _exploreTarget = nearestForage.Value;
                    _exploreStuckTicks = 0;
                    SyncDirectionToTarget(_exploreTarget.Value * 64f + new Vector2(32f, 32f));

                }
                else
                {
                    // No more forageables in this area!
                    _exploreAreaComplete = true;
                    _exploreAreaDoneTimer = 0;

                }
            }
            
            // === MOVE TO FORAGE TARGET ===
            if (_exploreTarget.HasValue)
            {
                // Check if target still valid
                if (!IsForageTargetStillValid(location, _exploreTarget.Value))
                {
                    _exploreTarget = null;
                    return;
                }
                
                // Check if reached (2 tiles is enough to pick up)
                if (Vector2.Distance(_pet.Tile, _exploreTarget.Value) < 2.0f)
                {
                    TryPickupForageableAt(location, _exploreTarget.Value);
                    _exploreTarget = null;
                    _exploreStuckTicks = 0;
                    return;
                }
                
                // Move toward forage
                Vector2 targetPos = _exploreTarget.Value * 64f + new Vector2(32f, 32f);
                bool moved = MoveTowardTarget(targetPos, EXPLORE_FORAGE_SPEED, false);
                
                // Always animate while moving toward target (even if stuck)
                if (!moved)
                {
                    // Update direction to face target even when stuck
                    Vector2 direction = targetPos - petPos;
                    if (direction.LengthSquared() > 0)
                    {
                        direction.Normalize();
                        UpdateDirectionSmooth(direction);
                    }
                    AnimatePetMovement(); // Still animate to show trying to move
                }
                
                // Stuck detection
                float movedDist = Vector2.Distance(petPos, _lastPosition);
                if (!moved || movedDist < STUCK_MOVEMENT_THRESHOLD)
                {
                    _exploreStuckTicks++;
                    if (_exploreStuckTicks >= STUCK_THRESHOLD_TICKS)
                    {
                        // Activate wall-pass
                        _followState = FollowState.WallPassing;
                        _wallPassTicks = 0;
                        _wallPassTarget = targetPos;
                        _wallPassDirection = targetPos - petPos;
                        if (_wallPassDirection != Vector2.Zero)
                            _wallPassDirection.Normalize();
                        _currentFollowSpeed = EXPLORE_FORAGE_SPEED;
                        

                    }
                }
                else
                {
                    _exploreStuckTicks = 0;
                }
                
                _lastPosition = petPos;
            }
        }
        
        /// <summary>Scan the entire area for forageables.</summary>
        private Vector2? ScanAreaForForageable(GameLocation location)
        {
            if (location == null || _pet == null) return null;
            
            float nearestDist = float.MaxValue;
            Vector2? nearestForage = null;
            Vector2 petTile = _pet.Tile;
            
            foreach (var pair in location.Objects.Pairs)
            {
                if (!IsForageable(pair.Value))
                    continue;
                
                if (_exploreUnreachableTiles.Contains(pair.Key))
                    continue;
                
                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestForage = pair.Key;
                }
            }
            
            return nearestForage;
        }

        /// <summary>Advance to the next explore area or finish if the loop completed.</summary>
        private void TryAdvanceExploreArea()
        {
            if (_pet?.currentLocation == null)
                return;

            string nextArea = GetNextExploreArea();

            if (_exploreStartArea != null && !string.Equals(nextArea, _exploreStartArea, StringComparison.OrdinalIgnoreCase))
            {
                _exploreHasLeftStartArea = true;
            }

            if (_exploreStartArea != null && string.Equals(nextArea, _exploreStartArea, StringComparison.OrdinalIgnoreCase) && _exploreHasLeftStartArea)
            {
                _exploreEndWhenArriveAtStart = true;
            }

            BeginExploreTravelToArea(nextArea);
        }

        /// <summary>Begin natural travel to the next explore area using map warps.</summary>
        private void BeginExploreTravelToArea(string targetArea)
        {
            if (_pet?.currentLocation == null)
                return;

            string currentArea = _pet.currentLocation.Name;
            if (string.Equals(currentArea, targetArea, StringComparison.OrdinalIgnoreCase))
            {
                ResetExploreAreaState();
                return;
            }

            ResetExploreAreaState();

            var path = GetExploreTravelPath(currentArea, targetArea);
            if (path == null || path.Count == 0)
            {
                TeleportToExploreArea(targetArea);
                return;
            }

            _exploreTravelPath.Clear();
            foreach (var area in path)
            {
                _exploreTravelPath.Enqueue(area);
            }

            _exploreInTransit = true;
            BeginExploreTravelHop();
        }

        /// <summary>Update natural travel between areas.</summary>
        private void UpdateExploreTransition()
        {
            if (_pet == null || _pet.currentLocation == null)
                return;

            if (_exploreTransitionTargetLocation == null)
            {
                _exploreInTransit = false;
                return;
            }

            _exploreTransitionTicks++;

            if (string.Equals(_pet.currentLocation.Name, _exploreTransitionTargetLocation, StringComparison.OrdinalIgnoreCase))
            {
                _exploreTransitionTicks = 0;
                _exploreTransitionWarpTile = null;
                _exploreTransitionTargetTile = null;
                _pet.controller = null;

                // Arrived at this hop
                if (_exploreTravelPath.Count > 0)
                    _exploreTravelPath.Dequeue();

                if (_exploreTravelPath.Count == 0)
                {
                    if (_exploreEndWhenArriveAtStart && _exploreStartArea != null &&
                        string.Equals(_pet.currentLocation.Name, _exploreStartArea, StringComparison.OrdinalIgnoreCase))
                    {
                        StopExploringReturnToFarm();
                        return;
                    }

                    _exploreInTransit = false;
                    ResetExploreAreaState();
                }
                else
                {
                    BeginExploreTravelHop();
                }
                return;
            }

            if (_exploreTransitionWarpTile.HasValue)
            {
                float dist = Vector2.Distance(_pet.Tile, _exploreTransitionWarpTile.Value);
                if (dist <= 0.5f)
                {
                    var targetLocation = Game1.getLocationFromName(_exploreTransitionTargetLocation);
                    if (targetLocation != null)
                    {
                        Vector2 preferred = _exploreTransitionTargetTile ?? FindSafeSpawnTile(targetLocation);
                        Vector2 arrival = GetSafeArrivalTile(targetLocation, preferred);
                        Game1.warpCharacter(_pet, targetLocation, arrival);
                    }
                }
            }

            if (_exploreTransitionTicks >= EXPLORE_TRANSITION_TIMEOUT)
            {
                TeleportToExploreArea(_exploreTransitionTargetLocation);
                _exploreTransitionTicks = 0;
            }
        }

        /// <summary>Start a single hop in the travel path.</summary>
        private void BeginExploreTravelHop()
        {
            if (_pet?.currentLocation == null || _exploreTravelPath.Count == 0)
                return;

            string nextArea = _exploreTravelPath.Peek();
            _exploreTransitionTargetLocation = nextArea;
            _exploreTransitionTicks = 0;

            var warp = FindWarpToLocation(_pet.currentLocation, nextArea);
            if (warp == null)
            {
                TeleportToExploreArea(nextArea);
                return;
            }

            _exploreTransitionWarpTile = new Vector2(warp.X, warp.Y);
            _exploreTransitionTargetTile = new Vector2(warp.TargetX, warp.TargetY);

            _pet.controller = new PathFindController(_pet, _pet.currentLocation, new Point(warp.X, warp.Y), 0);
        }

        /// <summary>Get a natural travel path between explore areas.</summary>
        private List<string>? GetExploreTravelPath(string fromArea, string toArea)
        {
            if (string.Equals(fromArea, toArea, StringComparison.OrdinalIgnoreCase))
                return new List<string>();

            var graph = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Town", new List<string> { "Mountain", "Forest", "Beach" } },
                { "Mountain", new List<string> { "Town" } },
                { "Forest", new List<string> { "Town" } },
                { "Beach", new List<string> { "Town" } },
            };

            if (!graph.ContainsKey(fromArea) || !graph.ContainsKey(toArea))
                return null;

            var queue = new Queue<string>();
            var cameFrom = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            queue.Enqueue(fromArea);
            cameFrom[fromArea] = null;

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                if (string.Equals(current, toArea, StringComparison.OrdinalIgnoreCase))
                    break;

                foreach (string next in graph[current])
                {
                    if (cameFrom.ContainsKey(next))
                        continue;
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }

            if (!cameFrom.ContainsKey(toArea))
                return null;

            var path = new List<string>();
            string? step = toArea;
            while (step != null && !string.Equals(step, fromArea, StringComparison.OrdinalIgnoreCase))
            {
                path.Add(step);
                step = cameFrom[step];
            }
            path.Reverse();
            return path;
        }

        /// <summary>Find a warp from the current location to the target location.</summary>
        private Warp? FindWarpToLocation(GameLocation location, string targetLocationName)
        {
            if (location?.warps == null)
                return null;

            foreach (var warp in location.warps)
            {
                if (string.Equals(warp.TargetName, targetLocationName, StringComparison.OrdinalIgnoreCase))
                    return warp;
            }

            return null;
        }
        
        /// <summary>Check if an area is valid for exploration.</summary>
        private bool IsValidExploreArea(string locationName)
        {
            return EXPLORE_AREAS.Any(area => string.Equals(area, locationName, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>Get the next area to explore.</summary>
        private string GetNextExploreArea()
        {
            // Rotate through explore areas
            _currentExploreAreaIndex = (_currentExploreAreaIndex + 1) % EXPLORE_AREAS.Length;
            return EXPLORE_AREAS[_currentExploreAreaIndex];
        }
        
        /// <summary>Teleport pet to a specific exploration area (fallback only).</summary>
        private void TeleportToExploreArea(string targetArea)
        {
            if (_pet == null) return;

            GameLocation? targetLocation = Game1.getLocationFromName(targetArea);
            if (targetLocation != null)
            {
                Vector2 spawnTile = FindSafeSpawnTile(targetLocation);
                Vector2 arrival = GetSafeArrivalTile(targetLocation, spawnTile);
                Game1.warpCharacter(_pet, targetLocation, arrival);
                ResetExploreAreaState();
                _isExploring = true;
            }
            else
            {
                StopExploringReturnToFarm();
            }
        }
        
        /// <summary>Find a safe spawn tile in a location.</summary>
        private Vector2 FindSafeSpawnTile(GameLocation location)
        {
            // Try common spawn points
            Vector2[] commonSpawns = new[]
            {
                new Vector2(10, 10),
                new Vector2(15, 15),
                new Vector2(20, 20),
                new Vector2(25, 25),
                new Vector2(30, 30),
            };
            
            foreach (var tile in commonSpawns)
            {
                if (IsTileWalkable(location, tile) && !IsTileOccupiedByPet(location, tile))
                    return tile;
            }
            
            // Random search
            for (int i = 0; i < 50; i++)
            {
                int x = _random.Next(5, Math.Min(50, location.Map?.Layers[0]?.LayerWidth ?? 50));
                int y = _random.Next(5, Math.Min(50, location.Map?.Layers[0]?.LayerHeight ?? 50));
                Vector2 tile = new Vector2(x, y);
                
                if (IsTileWalkable(location, tile) && !IsTileOccupiedByPet(location, tile))
                    return tile;
            }
            
            // Fallback
            return new Vector2(10, 10);
        }

        /// <summary>Pick a safe arrival tile near a preferred tile, avoiding other pets.</summary>
        private Vector2 GetSafeArrivalTile(GameLocation location, Vector2 preferredTile)
        {
            if (IsTileWalkable(location, preferredTile) && !IsTileOccupiedByPet(location, preferredTile))
                return preferredTile;

            for (int radius = 1; radius <= 4; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        Vector2 tile = preferredTile + new Vector2(dx, dy);
                        if (IsTileWalkable(location, tile) && !IsTileOccupiedByPet(location, tile))
                            return tile;
                    }
                }
            }

            return FindSafeSpawnTile(location);
        }

        /// <summary>Check if a tile is already occupied by another pet.</summary>
        private bool IsTileOccupiedByPet(GameLocation location, Vector2 tile)
        {
            var pets = MultiPetManager.GetAllPets();
            foreach (var pet in pets)
            {
                if (pet == null || pet.currentLocation != location)
                    continue;
                if (Vector2.Distance(pet.Tile, tile) < 0.5f)
                    return true;
            }

            return false;
        }

        /// <summary>Stop the pet's movement smoothly.</summary>
        private void StopMovingSmooth()
        {
            // Decelerate smoothly instead of instant stop
            _velocity = Vector2.Lerp(_velocity, Vector2.Zero, 0.3f);
            
            if (_velocity.LengthSquared() < 0.1f)
            {
                _velocity = Vector2.Zero;
                if (_pet != null && _wasMoving)
                {
                    _pet.Sprite.StopAnimation();
                    _wasMoving = false;
                }
            }
        }

        /// <summary>Update facing direction with hysteresis.</summary>
        private void UpdateDirectionSmooth(Vector2 direction)
        {
            if (_pet == null) return;
            
            // Don't change direction during wall-pass (prevents flickering)
            if (_followState == FollowState.WallPassing && _directionLockTimer > 0)
            {
                _directionLockTimer--;
                return;
            }
            
            if (_directionLockTimer > 0)
            {
                _directionLockTimer--;
                return;
            }
            
            int newDirection = _currentDirection;
            float absX = Math.Abs(direction.X);
            float absY = Math.Abs(direction.Y);
            
            if (absX > absY + DIRECTION_CHANGE_THRESHOLD)
            {
                newDirection = direction.X > 0 ? 1 : 3;
            }
            else if (absY > absX + DIRECTION_CHANGE_THRESHOLD)
            {
                newDirection = direction.Y > 0 ? 2 : 0;
            }
            
            if (newDirection != _currentDirection)
            {
                _currentDirection = newDirection;
                _directionLockTimer = _followState == FollowState.WallPassing ? DIRECTION_LOCK_TICKS * 2 : DIRECTION_LOCK_TICKS;
                _pet.faceDirection(_currentDirection);
            }
        }

        /// <summary>Animate pet movement.</summary>
        private void AnimatePetMovement()
        {
            if (_pet == null) return;
            _wasMoving = true;
            _pet.animateInFacingDirection(Game1.currentGameTime);
        }

        /// <summary>Check if a position is walkable.</summary>
        private bool IsPositionWalkable(GameLocation location, Vector2 pixelPos)
        {
            if (location == null) return false;
            
            int tileX = (int)(pixelPos.X / 64f);
            int tileY = (int)(pixelPos.Y / 64f);
            
            // Always block water
            if (location.isWaterTile(tileX, tileY))
                return false;
            
            // Check passability
            if (!location.isTilePassable(new xTile.Dimensions.Location(tileX, tileY), Game1.viewport))
                return false;
            
            // Check blocking objects - but allow debris/forage items since pet can walk through them
            Vector2 tile = new Vector2(tileX, tileY);
            if (location.Objects.TryGetValue(tile, out var obj))
            {
                // Allow walking through forageables and debris (weeds, twigs, stones)
                if (obj.isDebrisOrForage())
                    return true;
                    
                // Block non-passable objects (placed items, machines, etc.)
                if (!obj.isPassable())
                    return false;
            }
            
            return true;
        }

        /// <summary>Check if a tile is walkable.</summary>
        private bool IsTileWalkable(GameLocation location, Vector2 tile)
        {
            return IsPositionWalkable(location, tile * 64f + new Vector2(32f, 32f));
        }

        /// <summary>Find nearby forageable.</summary>
        private Vector2? FindNearbyForageableSmooth(GameLocation location, Vector2 petTile)
        {
            if (location == null) return null;
            
            float nearestDist = float.MaxValue;
            Vector2? nearestForage = null;
            
            foreach (var pair in location.Objects.Pairs)
            {
                if (!IsForageable(pair.Value))
                    continue;
                
                if (_unreachableForageTiles.Contains(pair.Key))
                    continue;
                
                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist <= FORAGE_DETECTION_RADIUS && dist < nearestDist)
                {
                    if (IsTileWalkable(location, pair.Key) || HasWalkableAdjacent(location, pair.Key))
                    {
                        nearestDist = dist;
                        nearestForage = pair.Key;
                    }
                    else
                    {
                        _unreachableForageTiles.Add(pair.Key);
                    }
                }
            }
            
            return nearestForage;
        }

        /// <summary>Check if any adjacent tile is walkable.</summary>
        private bool HasWalkableAdjacent(GameLocation location, Vector2 tile)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (IsTileWalkable(location, tile + new Vector2(dx, dy)))
                        return true;
                }
            }
            return false;
        }

        /// <summary>Rotate a vector by degrees.</summary>
        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * MathF.PI / 180f;
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);
            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        /// <summary>Get a random adjacent tile offset.</summary>
        private Vector2 GetRandomAdjacentTile()
        {
            int[] offsets = { -1, 0, 1 };
            return new Vector2(offsets[_random.Next(3)], offsets[_random.Next(3)]);
        }

        private bool IsForageTargetStillValid(GameLocation location, Vector2 tile)
        {
            if (location == null)
                return false;
            if (!location.Objects.TryGetValue(tile, out StardewValley.Object obj))
                return false;
            return IsForageable(obj);
        }

        /// <summary>Find a nearby forageable item within detection radius.</summary>
        private Vector2? FindNearbyForageable(GameLocation location, Vector2 petTile)
        {
            if (location == null) return null;
            
            float nearestDist = float.MaxValue;
            Vector2? nearestForage = null;
            
            // Check objects in the location for forageable items
            foreach (var pair in location.Objects.Pairs)
            {
                if (!IsForageable(pair.Value))
                    continue;
                    
                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist <= FORAGE_DETECTION_RADIUS && dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestForage = pair.Key;
                }
            }
            
            return nearestForage;
        }
        
        /// <summary>Check if an object is a forageable item.</summary>
        private bool IsForageable(StardewValley.Object obj)
        {
            if (obj == null) return false;
            
            // Check if it's a forage item (category -81 and -80 are forage categories)
            // Also check isForage flag
            if (obj.isForage())
                return true;
                
            // Additional forageable categories
            if (obj.Category == StardewValley.Object.GreensCategory || 
                obj.Category == StardewValley.Object.flowersCategory)
                return true;
                
            return false;
        }
        
        /// <summary>Try to pick up a forageable item at the specified location.</summary>
        private void TryPickupForageableAt(GameLocation location, Vector2 tile)
        {
            if (_pet == null || location == null) return;
            
            // Check if inventory has space before even trying
            if (!ModEntry.InventoryManager.HasSpace)
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] {_pet.Name}'s inventory is full, skipping pickup", LogLevel.Debug);
                return;
            }
            
            // Check if there's still an object at this location
            if (!location.Objects.TryGetValue(tile, out StardewValley.Object obj))
                return;
                
            if (!IsForageable(obj))
                return;
                
            // Create a copy of the item to add to inventory
            Item itemToAdd = obj.getOne();
            itemToAdd.Stack = obj.Stack;
            
            // Try to add to pet inventory
            if (ModEntry.InventoryManager.AddItem(itemToAdd))
            {
                // Remove the object from the world
                location.Objects.Remove(tile);
                
                // Play pickup sound
                Game1.playSound("pickUpItem");
                
                // Show notification (respects config)
                ShowForagePickupNotification(obj.DisplayName);
            }
        }
        
        /// <summary>Show a HUD notification when the pet picks up a forageable item.</summary>
        private void ShowForagePickupNotification(string itemName)
        {
            if (_pet == null) return;
            
            // Check if forage notifications are enabled
            if (!ModEntry.Config.ShowForageNotifications) return;
            
            // Use HUDMessage with an icon-like display
            string message = $"{_pet.Name} found: {itemName}";
            HUDMessage hudMessage = new HUDMessage(message, HUDMessage.newQuest_type);
            Game1.addHUDMessage(hudMessage);
        }

        private void ScanForWork(Farm farm)
        {
            Vector2 petTile = _pet!.Tile;
            var config = ModEntry.Config;

            // Clear unreachable tiles periodically (every ~10 seconds)
            _unreachableClearTimer++;
            if (_unreachableClearTimer > 600)
            {
                _unreachableTiles.Clear();
                _unreachableClearTimer = 0;
            }

            bool foundWork;
            if (config.IgnorePriority)
            {
                foundWork = ScanForNearestTarget(farm, petTile);
            }
            else
            {
                foundWork = ScanByPriority(farm, petTile);
            }
            
            // If work was found, reset the notification flag so we can notify again later
            if (foundWork)
            {
                _noWorkNotificationShown = false;
            }
            // Only show "no work" notification once per work session
            else if (!_noWorkNotificationShown && config.ShowWorkingMessages)
            {
                _noWorkNotificationShown = true;
                string petName = _pet?.Name ?? "Your pet";
                Game1.addHUDMessage(new HUDMessage($"{petName} found nothing to do.", HUDMessage.newQuest_type));
            }
        }

        private bool ScanByPriority(Farm farm, Vector2 petTile)
        {
            // Get work types sorted by priority (lower number = higher priority)
            var enabledTypes = GetEnabledWorkTypesByPriority();

            foreach (var workType in enabledTypes)
            {
                bool found = workType switch
                {
                    WorkType.Debris => TryClearDebris(farm, petTile),
                    WorkType.StumpsAndLogs => TryRemoveStump(farm, petTile),
                    WorkType.Trees => TryChopTree(farm, petTile),
                    WorkType.Boulders => TryBreakBoulder(farm, petTile),
                    _ => false
                };

                if (found) return true;
            }
            
            return false;
        }

        private bool ScanForNearestTarget(Farm farm, Vector2 petTile)
        {
            var config = ModEntry.Config;
            var candidates = new List<(Vector2 tile, float distance, Action action)>();
            int workRadius = config.WorkRadius;

            // Debris
            if (config.ClearDebris)
            {
                foreach (var pair in farm.objects.Pairs)
                {
                    if (IsDebris(pair.Value))
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            var obj = pair.Value;
                            candidates.Add((tile, dist, () => ClearDebrisAt(farm, tile, obj)));
                        }
                    }
                }
            }

            // Trees (includes seeds at stage 0)
            if (config.ChopTrees)
            {
                foreach (var pair in farm.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree && tree.growthStage.Value >= 0 && !tree.stump.Value) // Stage 0 = seed, 1-4 = growing, 5 = full
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            candidates.Add((tile, dist, () => ChopTreeAt(farm, tile, tree)));
                        }
                    }
                }
            }

            // Stumps
            if (config.ClearStumpsAndLogs)
            {
                foreach (var pair in farm.terrainFeatures.Pairs)
                {
                    if (pair.Value is Tree tree && tree.stump.Value)
                    {
                        float dist = Vector2.Distance(petTile, pair.Key);
                        if (dist <= workRadius)
                        {
                            var tile = pair.Key;
                            candidates.Add((tile, dist, () => RemoveStumpAt(farm, tile, tree)));
                        }
                    }
                }

                // Large stumps/logs
                foreach (var clump in farm.resourceClumps)
                {
                    if (clump.parentSheetIndex.Value == 600 || clump.parentSheetIndex.Value == 602)
                    {
                        float dist = Vector2.Distance(petTile, clump.Tile);
                        if (dist <= workRadius)
                        {
                            var targetClump = clump;
                            candidates.Add((clump.Tile, dist, () => RemoveLargeStump(farm, targetClump)));
                        }
                    }
                }
            }

            // Boulders
            if (config.BreakBoulders)
            {
                foreach (var clump in farm.resourceClumps)
                {
                    if (IsBoulderClump(clump.parentSheetIndex.Value))
                    {
                        float dist = Vector2.Distance(petTile, clump.Tile);
                        if (dist <= workRadius)
                        {
                            var targetClump = clump;
                            candidates.Add((clump.Tile, dist, () => BreakBoulder(farm, targetClump)));
                        }
                    }
                }
            }

            // Pick the nearest
            if (candidates.Count > 0)
            {
                var nearest = candidates.OrderBy(c => c.distance).First();
                SetJob(nearest.tile, nearest.action);
                return true;
            }
            
            return false;
        }

        private List<WorkType> GetEnabledWorkTypesByPriority()
        {
            var config = ModEntry.Config;
            var types = new List<(WorkType type, int priority, bool enabled)>
            {
                (WorkType.Debris, config.DebrisPriority, config.ClearDebris),
                (WorkType.StumpsAndLogs, config.StumpsAndLogsPriority, config.ClearStumpsAndLogs),
                (WorkType.Trees, config.TreesPriority, config.ChopTrees),
                (WorkType.Boulders, config.BouldersPriority, config.BreakBoulders)
            };

            return types
                .Where(t => t.enabled)
                .OrderBy(t => t.priority)
                .Select(t => t.type)
                .ToList();
        }

        private bool IsDebris(StardewValley.Object obj)
        {
            return obj.Name.Contains("Weed") || 
                   obj.Name.Contains("Stone") || 
                   obj.Name.Contains("Twig") || 
                   obj.ParentSheetIndex == 294 || 
                   obj.ParentSheetIndex == 295 ||
                   obj.Category == StardewValley.Object.litterCategory;
        }

        private bool IsBoulderClump(int parentSheetIndex)
        {
            return parentSheetIndex == 672 || 
                   parentSheetIndex == 752 || 
                   parentSheetIndex == 754 || 
                   parentSheetIndex == 756 || 
                   parentSheetIndex == 758;
        }

        private bool TryClearDebris(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ClearDebris) return false;

            KeyValuePair<Vector2, StardewValley.Object>? target = null;
            float nearestDist = float.MaxValue;

            foreach (var pair in farm.objects.Pairs)
            {
                if (!IsDebris(pair.Value)) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius || dist >= nearestDist)
                    continue;

                target = pair;
                nearestDist = dist;
            }

            if (target.HasValue)
            {
                SetJob(target.Value.Key, () => ClearDebrisAt(farm, target.Value.Key, target.Value.Value));
                return true;
            }
            return false;
        }

        private bool TryChopTree(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ChopTrees) return false;

            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree) continue;
                if (tree.growthStage.Value < 1 || tree.stump.Value) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius) continue;

                SetJob(pair.Key, () => ChopTreeAt(farm, pair.Key, tree));
                return true;
            }
            return false;
        }

        private bool TryRemoveStump(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.ClearStumpsAndLogs) return false;

            // Normal stumps
            foreach (var pair in farm.terrainFeatures.Pairs)
            {
                if (pair.Value is not Tree tree) continue;
                if (!tree.stump.Value) continue;

                float dist = Vector2.Distance(petTile, pair.Key);
                if (dist > ModEntry.Config.WorkRadius) continue;

                SetJob(pair.Key, () => RemoveStumpAt(farm, pair.Key, tree));
                return true;
            }

            // Large Stumps/Logs (ResourceClumps)
            foreach (var clump in farm.resourceClumps)
            {
                if (clump.parentSheetIndex.Value != 600 && clump.parentSheetIndex.Value != 602)
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                ResourceClump targetClump = clump;
                SetJob(clump.Tile, () => RemoveLargeStump(farm, targetClump));
                return true;
            }

            return false;
        }

        private bool TryBreakBoulder(Farm farm, Vector2 petTile)
        {
            if (!ModEntry.Config.BreakBoulders) return false;

            foreach (var clump in farm.resourceClumps)
            {
                if (!IsBoulderClump(clump.parentSheetIndex.Value))
                    continue;

                float dist = Vector2.Distance(petTile, clump.Tile);
                if (dist > ModEntry.Config.WorkRadius)
                    continue;

                ResourceClump targetClump = clump;
                SetJob(clump.Tile, () => BreakBoulder(farm, targetClump));
                return true;
            }

            return false;
        }

        private void SetJob(Vector2 tile, Action action)
        {
            // Skip if we already know this tile is unreachable
            if (_unreachableTiles.Contains(tile))
            {
                return; // Silently skip, don't spam logs
            }
            
            // Skip if another pet already reserved this target
            if (_pet != null && MultiPetManager.IsTargetReservedByOther(_pet, tile))
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] {_pet.Name}: Target {tile} already reserved by another pet, skipping.", LogLevel.Trace);
                return;
            }
            
            // Try to reserve this target
            if (_pet != null && !MultiPetManager.TryReserveTarget(_pet, tile))
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] {_pet.Name}: Failed to reserve target {tile}.", LogLevel.Trace);
                return;
            }
            
            // Release previous reservation if we had one
            if (_reservedTarget.HasValue && _pet != null)
            {
                MultiPetManager.ReleaseTarget(_pet, _reservedTarget.Value);
            }
            _reservedTarget = tile;

            ModEntry.Instance.Monitor.Log($"[WorkingPets] SetJob: {_pet?.Name} moving to tile {tile}", LogLevel.Debug);
            
            _targetTile = tile;
            _pendingAction = action;

            _lastTargetTile = tile;
            _lastDistanceToTarget = float.MaxValue;
            _noProgressTicks = 0;
        }

        private bool TryWarpPetNearTarget(GameLocation location, Vector2 targetTile)
        {
            if (_pet == null)
            {
                return false;
            }

            // Try the 8 neighboring tiles around the target (1 tile away).
            // Prefer tiles that are on-map, passable, and not water.
            var candidates = new List<Vector2>(8);
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    candidates.Add(targetTile + new Vector2(dx, dy));
                }
            }

            // Prefer closer-to-pet tiles.
            candidates = candidates
                .Where(t => location.isTileOnMap(t))
                .OrderBy(t => Vector2.Distance(_pet.Tile, t))
                .ToList();

            foreach (var tile in candidates)
            {
                int x = (int)tile.X;
                int y = (int)tile.Y;

                if (location.isWaterTile(x, y))
                    continue;

                if (!location.isTilePassable(new xTile.Dimensions.Location(x, y), Game1.viewport))
                    continue;

                ModEntry.Instance.Monitor.Log($"[WorkingPets] Stuck detected; warping {_pet.Name} near target {targetTile} -> {tile}", LogLevel.Debug);
                Game1.warpCharacter(_pet, location.NameOrUniqueName, tile);
                _pet.Halt();
                _pet.controller = null;
                return true;
            }

            return false;
        }

        private void ClearDebrisAt(Farm farm, Vector2 tile, StardewValley.Object obj)
        {
            ModEntry.Instance.Monitor.Log($"[WorkingPets] ClearDebrisAt called: tile={tile}, object={obj.Name}", LogLevel.Info);
            
            if (!farm.objects.ContainsKey(tile))
            {
                ModEntry.Instance.Monitor.Log($"[WorkingPets] Object no longer exists at {tile}!", LogLevel.Warn);
                return;
            }

            // Play appropriate sound and create destruction animation
            if (obj.Name.Contains("Stone"))
            {
                // Stone destruction - hammer sound + stone debris
                farm.localSound("hammer");
                Game1.createRadialDebris(farm, 14, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
                // Stone break animation
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(47, tile * 64f, Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f));
            }
            else if (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295)
            {
                // Twig destruction - axe chop + wood debris animation
                farm.localSound("axchop");
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(4, 10), resource: false);
                // Wood break animation (sprite 12 = wood poof)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
            }
            else if (obj.Name.Contains("Weed"))
            {
                // Weed cutting - cut sound + grass/weed animation
                farm.localSound("weed_cut");
                // Weed destruction animation (sprite 50 = green poof for weeds)
                Color weedColor = Color.Green;
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f, weedColor));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), weedColor * 0.75f)
                {
                    scale = 0.75f,
                    flipped = true
                });
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(50, tile * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), weedColor * 0.75f)
                {
                    scale = 0.75f,
                    delayBeforeAnimationStart = 50
                });
            }
            else
            {
                // Generic debris - cut sound
                farm.localSound("cut");
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(2, 5), resource: false);
            }

            var drops = GetDropsForObject(obj);
            farm.objects.Remove(tile);
            
            ModEntry.Instance.Monitor.Log($"[WorkingPets] Removed {obj.Name} at {tile}, got {drops.Count} drops", LogLevel.Info);

            foreach (var item in drops) ModEntry.InventoryManager.AddItem(item);
        }

        private void ChopTreeAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Determine tree size based on growth stage
            int growthStage = tree.growthStage.Value;
            bool isSmallTree = growthStage < 5; // Stages 0-4 are seeds/small trees/saplings (stage 0 = seed, 1-4 = growing)
            
            // Play axe chop sound
            farm.localSound("axchop");
            
            if (isSmallTree)
            {
                // Small tree/sapling: lighter twig debris
                Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f + 32f);
                farm.debris.Add(new Debris(12, Game1.random.Next(1, 2), debrisOrigin, debrisOrigin, 0));
            }
            else
            {
                // Fully grown tree: normal wood chip debris
                Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f);
                farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));
                
                // Shake the tree
                tree.shake(tile, doEvenIfStillShaking: true);
            }

            // Small trees die in fewer hits
            int hitsNeeded = isSmallTree ? 2 : TREE_HEALTH;
            
            if (_objectDamage[tile] >= hitsNeeded)
            {
                if (isSmallTree)
                {
                    // Small tree/sapling: simple twig poof, remove completely
                    farm.localSound("leafrustle");
                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f + new Vector2(32f, 32f), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f)
                    {
                        scale = 0.75f
                    });
                    
                    // Remove the small tree completely (no stump)
                    farm.terrainFeatures.Remove(tile);
                    
                    // Small trees give less wood
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(1, 3)));
                }
                else
                {
                    // Fully grown tree: burst effect and leave stump
                    Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(12, 18), resource: false);
                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));

                    // Force a clean non-falling state and leave a stump behind
                    tree.falling.Value = false;
                    tree.shakeRotation = 0f;
                    tree.maxShake = 0f;
                    tree.stump.Value = true;
                    tree.health.Value = 5f;

                    // Add drops to pet inventory
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(8, 15)));
                    if (_random.NextDouble() < 0.5)
                        ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)92", _random.Next(1, 3)));
                }
                
                // Reset our hit tracking
                _objectDamage.Remove(tile);
            }
        }

        private void RemoveStumpAt(Farm farm, Vector2 tile, Tree tree)
        {
            if (!farm.terrainFeatures.ContainsKey(tile)) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play axe chop sound and create wood debris
            farm.localSound("axchop");
            Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f);
            farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));

            if (_objectDamage[tile] >= STUMP_HEALTH)
            {
                // Final destruction - use the same VFX as breaking twigs/wood (universal wood break look)
                farm.localSound("stumpCrack");

                // Wood chips burst
                Game1.createRadialDebris(farm, 12, (int)tile.X, (int)tile.Y, Game1.random.Next(10, 16), resource: false);

                // Wood poof (same as twig break)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                
                farm.terrainFeatures.Remove(tile);
                _objectDamage.Remove(tile);

                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)388", _random.Next(3, 6)));
            }
        }

        private void RemoveLargeStump(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play axe chop sound and create wood debris on each hit
            farm.localSound("axchop");
            Vector2 debrisOrigin = new Vector2(tile.X * 64f + 32f, tile.Y * 64f + 32f);
            farm.debris.Add(new Debris(12, Game1.random.Next(1, 3), debrisOrigin, debrisOrigin, 0));

            if (_objectDamage[tile] >= STUMP_HEALTH * 2)
            {
                // Final destruction - use the same VFX as breaking twigs/wood (universal wood break look)
                farm.localSound("stumpCrack");

                // Wood chips burst across the clump footprint
                int bursts = 3;
                for (int i = 0; i < bursts; i++)
                {
                    int x = (int)tile.X + Game1.random.Next(clump.width.Value);
                    int y = (int)tile.Y + Game1.random.Next(clump.height.Value);
                    Game1.createRadialDebris(farm, 12, x, y, Game1.random.Next(8, 14), resource: false);
                }

                // A couple wood poofs to sell the breakup (covers 2x2-ish area)
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, tile * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, (tile + new Vector2(1f, 0f)) * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 60f));
                farm.temporarySprites.Add(new TemporaryAnimatedSprite(12, (tile + new Vector2(0f, 1f)) * 64f, Color.White, 8, Game1.random.NextDouble() < 0.5, 70f));
                
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);

                // Hardwood drops (game gives 2-8 for stump, 8-10 for log)
                int hardwoodAmount = (clump.parentSheetIndex.Value == 602) ? _random.Next(6, 10) : _random.Next(2, 4);
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)709", hardwoodAmount));
            }
        }

        private void BreakBoulder(Farm farm, ResourceClump clump)
        {
            Vector2 tile = clump.Tile;
            int index = farm.resourceClumps.IndexOf(clump);
            if (index == -1) return;

            if (!_objectDamage.ContainsKey(tile)) _objectDamage[tile] = 0;
            _objectDamage[tile]++;

            // Play hammer sound and create stone debris on each hit
            farm.localSound("hammer");
            
            // Stone chip debris flying from impact (14 = stoneDebris)
            Game1.createRadialDebris(farm, 14, (int)tile.X + Game1.random.Next(clump.width.Value / 2 + 1), 
                (int)tile.Y + Game1.random.Next(clump.height.Value / 2 + 1), Game1.random.Next(4, 9), resource: false);

            if (_objectDamage[tile] >= BOULDER_HEALTH)
            {
                // Final destruction - use the normal stone break VFX, just bigger/more of it
                farm.localSound("hammer");

                // Big burst of regular stone debris across the clump footprint (14 = stoneDebris)
                int bursts = 4;
                for (int i = 0; i < bursts; i++)
                {
                    int x = (int)tile.X + Game1.random.Next(clump.width.Value);
                    int y = (int)tile.Y + Game1.random.Next(clump.height.Value);
                    Game1.createRadialDebris(farm, 14, x, y, Game1.random.Next(6, 12), resource: false);
                }

                // Extra stone break sprites (47) to sell the impact
                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = (tile + new Vector2(Game1.random.Next(clump.width.Value), Game1.random.Next(clump.height.Value))) * 64f;
                    farm.temporarySprites.Add(new TemporaryAnimatedSprite(47, pos, Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f)
                    {
                        scale = 1.25f
                    });
                }
                
                farm.resourceClumps.RemoveAt(index);
                _objectDamage.Remove(tile);

                // Drop stone - game gives 15 for regular boulder, 10 for mine boulders
                int stoneAmount = (clump.parentSheetIndex.Value == 672) ? _random.Next(12, 18) : _random.Next(8, 12);
                ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)390", stoneAmount));
                
                // Chance for ores
                if (_random.NextDouble() < 0.25)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)380", _random.Next(1, 3))); // Copper
                if (_random.NextDouble() < 0.1)
                    ModEntry.InventoryManager.AddItem(ItemRegistry.Create("(O)384", _random.Next(1, 2))); // Gold
            }
        }

        private List<Item> GetDropsForObject(StardewValley.Object obj)
        {
            var drops = new List<Item>();
            if (obj.Name.Contains("Stone"))
            {
                drops.Add(ItemRegistry.Create("(O)390", 1));
                if (_random.NextDouble() < 0.05) drops.Add(ItemRegistry.Create("(O)382", 1));
            }
            else if (obj.Name.Contains("Twig") || obj.ParentSheetIndex == 294 || obj.ParentSheetIndex == 295)
            {
                drops.Add(ItemRegistry.Create("(O)388", 1));
            }
            else if (obj.Name.Contains("Weed"))
            {
                if (_random.NextDouble() < 0.5) drops.Add(ItemRegistry.Create("(O)771", 1));
                if (_random.NextDouble() < 0.05) drops.Add(ItemRegistry.Create("(O)770", 1));
            }
            return drops;
        }
    }
}
