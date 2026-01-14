# **Overview**:
A mod that makes your pet helpful by making it clear debris around the farm. If configured in the GMCM, it'll even chop trees and break down boulders.

# **Detailed Description**:
***How it feels in-game***:
Instead of your pet just being a cute obstacle that naps in the worst possible place, it becomes a little farm helper you can actually “assign” work to. You don’t have to craft tools for it or babysit it — you just talk to your pet like normal and tell it what you want.

***Talking to your pet (the menu)*:**
Whenever you interact with your pet, you get a small choice menu with friendly options (so it feels like you’re giving it instructions, not flipping a switch).

***The options you can pick*:**
* ***Follow me / That’s enough following for now***: toggles your pet’s follow mode.
* ***Ask [pet name] to help around the farm / Let [pet name] rest***: toggles work mode.
* ***Check what [pet name] has found (shows the number of items if it has any)***: opens your pet’s “pouch”.
* ***Give [pet name] some love***: pets them (with the usual once-per-day limit).
* ***Give [pet name] a new name***: a one-time rename option.
* **Never mind**: closes the menu.

***Little greetings (dialogue flavor)***:
The first line you see changes depending on what your pet is doing:
* If it’s following you: it’s described as happily following.
* If it’s working: it pauses, looks at you, and seems eager.
* If it’s idle: it looks up at you expectantly.

***Work mode: what your pet actually does***:
When work mode is on, your pet will repeatedly look around for chores and then walk over to the nearest thing that matches what you allowed in the settings.

***What it can clear (based on your settings)***:
* **Debris**: weeds, small stones, and twigs.
* **Stumps & big logs**: regular stumps plus the big “hardwood” ones.
* **Fully grown trees**: it can chop down mature trees.
* **Big boulders**: it can break the large rocks that block areas.

***How it decides what to do first:***
You can choose the “personality” of its work in the config menu:
* **Priority mode**: you assign an order (like “always do weeds first, then stumps, then trees…”), and it will try that order each time it looks for something to do.
* **Nearest-target mode**: it ignores priority and just goes for the closest allowed target.

***How far it will roam, and how often it searches:***
You can tune how far your pet is willing to wander from where it currently is, and how often it does a new “scan” for chores. If you set it tight, it’ll stay close and tidy up nearby. If you set it wide, it’ll patrol a big chunk of the farm.

***What it looks/sounds like when it works:***
It doesn’t silently delete things. It uses the game’s normal vibes:
* Weeds get sliced with that familiar cut sound and green “poof”.
* Twigs get chopped with axe sounds and wood bits.
* Stones get smashed with hammer sounds and stone chips.
* Trees and stumps take multiple hits, so it feels like it’s actually working instead of instantly nuking the whole farm.

***What happens to the drops:***
Instead of spraying items everywhere and cluttering your screen, your pet collects what it earns into its own little inventory (its “pouch”).

***Checking the pouch:***
When you choose the pouch option, you get a chest-style menu showing what your pet has picked up. If it’s empty, it still lets you check it (because sometimes you just wanna confirm it didn’t steal your stuff).

***If the pouch fills up:***
If there’s no room left, your pet won’t delete items — it drops the extra on the ground near where the pet is.

***Overnight scavenging (bonus helper behavior):***
If you left your pet in “helping” mode, it can come back in the morning with extra finds — up to a handful per night. The game pops a message like “[pet name] found X new items overnight!” and those items go straight into the pouch.

***The kinds of things it can bring home:***
It’s a mix of “pet logic” categories — like a hunter bringing something meaty, a digger bringing something from the dirt, or a little trash gremlin bringing… well, trash.

***Follow mode (separate from work mode):***
Follow mode is exactly what it sounds like: your pet tries to keep up with you and trail along behind. If it gets stuck on the terrain for too long, it “cheats” a little by popping closer so it doesn’t fall miles behind.

***Farm boundaries:***
By default, your pet is a farm helper — if you leave the farm while it’s following, it stops following (so it doesn’t get weird outside). There’s also an experimental setting that lets it follow you outside the farm if you want.

***Rest rules (so it doesn’t feel cursed):***
If your pet is sleeping/resting, the mod lets it be. It won’t drag your pet out of bed just to go punch rocks.

***Petting and bonding (still matters):***
The “Give some love” option behaves like normal pet affection:
* If you already did it today, it’ll basically say “yes yes, you already loved me today” and act happy.
* If you haven’t, it gives the proper “heart” moment and counts as real attention.

***Renaming (one-time):***
You get one rename option through the pet menu. After you rename your pet once, that rename choice won’t show up there again.

# **Conceptualization**:
My cat is blocking the door when I finally realized how useless it is. Also the Infinite Stamina is useless if I am still the one to do all the goddamn work.

Then I thought, hmmm... this is gonna be sick. If I actually make the cat work.

# **Concerns**:
I only ever use this mod to a single pet and according to my research there's a thing called pet license and there are other type of pets like turtles and dogs. I ain't sure how this mod will work with multiple pets and different types at that.

# **Update Plans**:
1. Add a full fledge follow mechanics around town. Add a foraging mechanics so the player can just walk around and the pet will pick it up for the player.
2. Daily summary report for what item was collected during the day.
3. Emotes when working (💧 when watering, ⚡ when chopping, ❤️ when finding good items). Trail particles when moving to targets. Speech bubbles showing what they're collecting. [DOUBTS]
4. Watering Crops - Pet waters unwatered crops when work mode is on.
5. Pet collects from Kegs, Preserve Jars, Furnaces, etc. Or even crazier, allow them to put things in them from chests.
6. Pet automatically harvests ready crops (configurable on/off). Respects crop quality, adds to inventory. Would be HUGELY useful for large farms

# **Progress**:
version 1.2.0
- Multi-pet support! Each pet now works independently (Pet Licenses compatible).
- Target reservation system prevents pets from targeting the same debris.
- Collision avoidance keeps pets from stacking on each other.
- Pets now pause and stop moving when you talk to them.
- After 3 failed warp attempts, pets destroy target remotely (fixes infinite stuck loops).
- Trees at all growth stages (1-5) can now be removed, not just fully grown ones.

version 1.1.0
* Added proper destruction visuals/FX/SFX when pets clear debris/wood/stone (no more silent disappearing).
- Adjusted wood-related destruction to use a consistent twig-style particle/poof effect.
- Fixed cases where pets could get stuck while doing work by adding a warp fallback near the target.
- Improved follow behavior with catch-up speed scaling and a stuck warp fallback near the player.