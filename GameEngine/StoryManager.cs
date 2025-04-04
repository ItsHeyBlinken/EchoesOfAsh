using System;
using System.Collections.Generic;
using test_project.Models;

namespace test_project.GameEngine
{
    /// <summary>
    /// Manages the story progression and quest system
    /// </summary>
    public class StoryManager
    {
        private readonly GameState _gameState;
        private readonly Random _random = new Random();

        public StoryManager(GameState gameState)
        {
            _gameState = gameState;
        }

        public void InitializeStory()
        {
            // Set up the player first
            Player player = new Player("Survivor", "A survivor of the nuclear apocalypse, searching for others.");

            // Create the game world
            CreateLocations();

            // Initialize the player with the starting location
            Location? startingLocation = _gameState.GetLocation("bunker");
            if (startingLocation == null)
            {
                throw new InvalidOperationException("Starting location 'bunker' not found");
            }
            _gameState.Initialize(player, startingLocation);

            // Now create NPCs and quests after player is initialized
            CreateNPCs();
            CreateQuests();

            // Give the player some starting items
            player.AddItem(new Consumable("Stale Bread", "A piece of stale bread. Better than nothing.", 0.2, ItemType.Food, 2, hungerEffect: 15));
            player.AddItem(new Consumable("Water Flask", "A small flask of clean water.", 0.5, ItemType.Water, 3, thirstEffect: 20));
            player.AddItem(new Weapon("Rusty Knife", "A rusty knife. Not much, but it's something.", 0.5, 5, 15, 5));
            player.EquipWeapon((Weapon)player.FindItem("Rusty Knife"));
        }

        private void CreateLocations()
        {
            // Create all game locations

            // Starting area
            Location bunker = new Location(
                "Underground Bunker",
                "A small underground bunker that has protected you from the worst of the radiation.",
                "This cramped underground bunker has been your home since the bombs fell. It's stocked with basic supplies, but they're running low. The air filtration system is failing, and you know you can't stay here much longer. A ladder leads up to the surface.",
                true,
                0
            );
            bunker.HasShelter = true;
            bunker.HasWater = true;
            bunker.HasFood = true;
            _gameState.AddLocation("bunker", bunker);

            Location wasteland = new Location(
                "Desolate Wasteland",
                "A barren wasteland stretches out before you, the ground scorched and lifeless.",
                "The landscape is unrecognizable from what it once was. The ground is cracked and scorched, with no vegetation in sight. The air is thick with dust, and you can see the ruins of buildings in the distance. The sky has a sickly yellow tint to it.",
                false,
                2
            );
            _gameState.AddLocation("wasteland", wasteland);

            Location ruinedCity = new Location(
                "Ruined City",
                "The skeletal remains of a once-thriving city rise from the wasteland.",
                "Crumbling skyscrapers and collapsed buildings create a maze of concrete and steel. The streets are littered with abandoned vehicles and debris. Occasionally, you hear strange noises echoing through the empty streets. This place feels dangerous, but might contain valuable supplies.",
                false,
                3
            );
            _gameState.AddLocation("ruinedCity", ruinedCity);

            Location settlement = new Location(
                "Survivor Settlement",
                "A small settlement of survivors who have banded together.",
                "This makeshift settlement is built from scavenged materials and fortified against the dangers of the wasteland. A few dozen people live here, trading goods and information. They look at you with a mixture of suspicion and hope. Guards patrol the perimeter, armed with improvised weapons.",
                true,
                1
            );
            settlement.HasShelter = true;
            settlement.HasWater = true;
            settlement.HasFood = true;
            _gameState.AddLocation("settlement", settlement);

            Location hospital = new Location(
                "Abandoned Hospital",
                "A dilapidated hospital that might contain valuable medical supplies.",
                "The hospital is in a state of disrepair, with collapsed sections and debris everywhere. Medical equipment lies scattered about, most of it broken or useless. The air smells of antiseptic and decay. Dark corridors lead deeper into the building, where who knows what might lurk.",
                false,
                2
            );
            _gameState.AddLocation("hospital", hospital);

            Location forest = new Location(
                "Mutated Forest",
                "A forest of twisted, mutated trees and strange vegetation.",
                "The trees here have been warped by radiation, growing in impossible shapes with glowing fungus clinging to their trunks. Strange sounds come from deep within the forest, and you occasionally glimpse movement between the trees. Despite the danger, there might be edible plants or clean water here.",
                false,
                4
            );
            forest.HasWater = true;
            _gameState.AddLocation("forest", forest);

            Location militaryBase = new Location(
                "Military Bunker",
                "A sealed military bunker that might contain weapons and supplies.",
                "This reinforced bunker was designed to withstand nuclear attacks. The entrance is heavily secured, but someone has managed to force it open. Inside, you can see military-grade equipment and supplies. This place would have been a treasure trove right after the war, but it's been picked over by scavengers.",
                false,
                1
            );
            militaryBase.HasShelter = true;
            _gameState.AddLocation("militaryBase", militaryBase);

            Location radioactiveCrater = new Location(
                "Radioactive Crater",
                "A highly radioactive crater where a nuclear bomb detonated.",
                "The ground here is glass-like, fused by the intense heat of a nuclear explosion. Your Geiger counter is clicking frantically, warning of the extreme radiation levels. Strange, mutated creatures can be seen in the distance, warped beyond recognition. Only the desperate or foolish would linger here.",
                false,
                5
            );
            _gameState.AddLocation("radioactiveCrater", radioactiveCrater);

            Location undergroundLab = new Location(
                "Underground Laboratory",
                "A hidden laboratory that was conducting secret experiments.",
                "This high-tech facility is surprisingly intact, protected deep underground. Computer terminals still flicker with power, and scientific equipment lines the walls. Documents scattered around hint at experimental research that might have contributed to the current state of the world. There might be valuable technology or information here.",
                false,
                2
            );
            undergroundLab.HasShelter = true;
            _gameState.AddLocation("undergroundLab", undergroundLab);

            Location coastline = new Location(
                "Toxic Coastline",
                "A coastline where toxic waste has polluted the water.",
                "The ocean has turned a sickly green color, with dead fish washing up on the shore. The beach is littered with industrial waste and debris. Despite the contamination, you can see signs that people have been living here, perhaps fishing or scavenging from the washed-up debris.",
                false,
                3
            );
            _gameState.AddLocation("coastline", coastline);

            // Connect locations
            bunker.AddExit("up", wasteland);
            wasteland.AddExit("down", bunker);
            wasteland.AddExit("north", ruinedCity);
            wasteland.AddExit("east", settlement);
            wasteland.AddExit("south", forest);
            wasteland.AddExit("west", radioactiveCrater);
            ruinedCity.AddExit("south", wasteland);
            ruinedCity.AddExit("east", hospital);
            ruinedCity.AddExit("north", undergroundLab);
            settlement.AddExit("west", wasteland);
            settlement.AddExit("north", hospital);
            hospital.AddExit("west", ruinedCity);
            hospital.AddExit("south", settlement);
            forest.AddExit("north", wasteland);
            forest.AddExit("east", coastline);
            radioactiveCrater.AddExit("east", wasteland);
            undergroundLab.AddExit("south", ruinedCity);
            coastline.AddExit("west", forest);
            militaryBase.AddExit("east", radioactiveCrater);
            radioactiveCrater.AddExit("west", militaryBase);

            // Add some items to locations
            wasteland.AddItem(new Consumable("Dirty Water", "A puddle of contaminated water. Drinking it would be risky.", 1.0, ItemType.Water, 1, thirstEffect: 15, radiationEffect: 10));
            ruinedCity.AddItem(new Weapon("Metal Pipe", "A sturdy metal pipe that could be used as a weapon.", 2.0, 8, 25, 3));
            hospital.AddItem(new Consumable("Antibiotics", "A bottle of antibiotics that could treat infections.", 0.1, ItemType.Medicine, 15, healthEffect: 20, radiationEffect: -5));
            forest.AddItem(new Consumable("Wild Berries", "Berries growing in the mutated forest. They look edible... maybe.", 0.2, ItemType.Food, 2, hungerEffect: 10, radiationEffect: 5));
            militaryBase.AddItem(new Weapon("Military Knife", "A well-maintained military combat knife.", 1.0, 15, 50, 20));
            undergroundLab.AddItem(new Consumable("Rad-X", "An experimental anti-radiation drug.", 0.1, ItemType.Medicine, 25, radiationEffect: -30));
            coastline.AddItem(new Consumable("Mutated Fish", "A strange fish with multiple eyes. Probably not safe to eat.", 1.0, ItemType.Food, 5, hungerEffect: 25, healthEffect: -5, radiationEffect: 15));
        }

        private void CreateNPCs()
        {
            // Create NPCs for the game

            // Friendly NPCs
            NPC oldSurvivor = new NPC(
                "Old Man",
                "An elderly survivor with a weathered face and knowing eyes.",
                50,
                5,
                15,
                5,
                true,
                true
            );
            oldSurvivor.DefaultDialogue = "I've been surviving out here since the bombs fell. Not many of us left now.";
            oldSurvivor.AddDialogueOption("survivors", "There's a settlement to the east where some folks have gathered. Be careful though, not everyone out here is friendly.");
            oldSurvivor.AddDialogueOption("radiation", "The radiation's worst in the crater to the west. Stay away from there if you value your life.");
            oldSurvivor.AddDialogueOption("food", "You can find some edible plants in the forest to the south, but be careful what you eat.");
            oldSurvivor.AddDialogueOption("water", "Clean water is hard to come by. The settlement has a purification system, but they don't share freely.");
            oldSurvivor.AddDialogueOption("help", "I'm too old to travel, but if you find medical supplies, I could use some for my joints.");
            if (_gameState.Player != null)
            {
                _gameState.Player.AddQuest("Help the Old Man");
            }
            _gameState.AddQuest("Help the Old Man", "Find medicine for the Old Man's joint pain.");
            _gameState.GetLocation("wasteland").AddCharacter(oldSurvivor);

            NPC trader = new NPC(
                "Trader",
                "A shrewd-looking person with a backpack full of scavenged goods.",
                70,
                8,
                12,
                10,
                true,
                true
            );
            trader.DefaultDialogue = "Looking to trade? I've got supplies if you've got something valuable.";
            trader.AddDialogueOption("trade", "I can offer you clean water, food, or medicine in exchange for weapons or valuable items.");
            trader.AddDialogueOption("survivors", "I travel between survivor groups. There are pockets of people all over, trying to rebuild.");
            trader.AddDialogueOption("information", "Information is valuable in this world. I might know something that could help you, for the right price.");
            trader.AddTradeItem(new Consumable("Purified Water", "Clean, radiation-free water.", 1.0, ItemType.Water, 10, thirstEffect: 50));
            trader.AddTradeItem(new Consumable("Canned Beans", "A preserved can of beans. A rare find.", 0.5, ItemType.Food, 8, hungerEffect: 40));
            trader.AddTradeItem(new Consumable("First Aid Kit", "A well-stocked first aid kit.", 1.0, ItemType.Medicine, 15, healthEffect: 50, radiationEffect: -10));
            _gameState.GetLocation("settlement").AddCharacter(trader);

            NPC doctor = new NPC(
                "Doctor",
                "A tired-looking woman wearing a stained lab coat.",
                60,
                6,
                18,
                7,
                true,
                true
            );
            doctor.DefaultDialogue = "I'm trying to help people with limited supplies. What do you need?";
            doctor.AddDialogueOption("radiation", "I'm working on better treatments for radiation sickness. If you find any medical research in the underground lab, bring it to me.");
            doctor.AddDialogueOption("medicine", "I can treat your injuries or radiation sickness if you have something to trade.");
            doctor.AddDialogueOption("survivors", "I've treated several survivors who came from the coast. They mentioned others hiding in caves there.");
            if (_gameState.Player != null)
            {
                _gameState.Player.AddQuest("Medical Research");
            }
            _gameState.AddQuest("Medical Research", "Find medical research documents in the underground lab for the Doctor.");
            _gameState.GetLocation("hospital").AddCharacter(doctor);

            NPC childSurvivor = new NPC(
                "Child",
                "A young child, dirty and thin, but with a resilient spirit.",
                30,
                3,
                8,
                12,
                true,
                true
            );
            childSurvivor.DefaultDialogue = "Are you going to help us? My parents are missing.";
            childSurvivor.AddDialogueOption("parents", "They went to the city to find medicine and never came back. Can you look for them?");
            childSurvivor.AddDialogueOption("survivors", "There are other kids at the settlement. The adults try to protect us.");
            if (_gameState.Player != null)
            {
                _gameState.Player.AddQuest("Find Missing Parents");
            }
            _gameState.AddQuest("Find Missing Parents", "Look for the Child's missing parents in the ruined city.");
            _gameState.GetLocation("settlement").AddCharacter(childSurvivor);

            // Create more survivors scattered around the world
            NPC scientist = new NPC(
                "Scientist",
                "A gaunt man in a tattered lab coat, clutching a notebook.",
                45,
                4,
                20,
                6,
                true,
                true
            );
            scientist.DefaultDialogue = "My research... I need to complete my research. It could help everyone.";
            scientist.AddDialogueOption("research", "I was working on a way to neutralize radiation in the soil. My notes are still in the lab.");
            scientist.AddDialogueOption("lab", "The underground laboratory has everything I need, but it's overrun with those... things.");
            _gameState.GetLocation("undergroundLab").AddCharacter(scientist);

            NPC militaryVeteran = new NPC(
                "Veteran",
                "A stern-looking person in tattered military fatigues.",
                80,
                15,
                10,
                12,
                true,
                true
            );
            militaryVeteran.DefaultDialogue = "State your business. This area is under my protection.";
            militaryVeteran.AddDialogueOption("military", "I was stationed at the base when the bombs fell. Been guarding the weapons cache ever since.");
            militaryVeteran.AddDialogueOption("weapons", "I've got a stockpile inside. Could spare some if you help me secure the perimeter.");
            if (_gameState.Player != null)
            {
                _gameState.Player.AddQuest("Secure the Perimeter");
            }
            _gameState.AddQuest("Secure the Perimeter", "Help the Veteran secure the military base perimeter.");
            _gameState.GetLocation("militaryBase").AddCharacter(militaryVeteran);

            NPC fisherman = new NPC(
                "Fisherman",
                "A weathered individual with a makeshift fishing rod.",
                65,
                9,
                7,
                10,
                true,
                true
            );
            fisherman.DefaultDialogue = "The waters are poisoned, but sometimes you can catch something edible.";
            fisherman.AddDialogueOption("fish", "Most are mutated, but some are safe to eat if you know what to look for.");
            fisherman.AddDialogueOption("coast", "There's a group of survivors living in caves further down the coast. They're cautious of strangers.");
            _gameState.GetLocation("coastline").AddCharacter(fisherman);

            NPC hermit = new NPC(
                "Forest Hermit",
                "A wild-looking person who seems to have adapted to life in the mutated forest.",
                70,
                10,
                9,
                15,
                true,
                true
            );
            hermit.DefaultDialogue = "The forest provides, if you know its secrets.";
            hermit.AddDialogueOption("forest", "The radiation changed everything, but new life always finds a way. Some plants here have healing properties.");
            hermit.AddDialogueOption("healing", "Bring me some clean water, and I'll show you how to make medicine from the forest plants.");
            if (_gameState.Player != null)
            {
                _gameState.Player.AddQuest("Forest Medicine");
            }
            _gameState.AddQuest("Forest Medicine", "Bring clean water to the Forest Hermit to learn about medicinal plants.");
            _gameState.GetLocation("forest").AddCharacter(hermit);

            // Hostile NPCs
            NPC raider = new NPC(
                "Raider",
                "A vicious-looking person with makeshift armor and weapons.",
                60,
                12,
                6,
                8,
                false,
                false
            );
            raider.DefaultDialogue = "Hand over your supplies and maybe I'll let you live!";
            _gameState.GetLocation("ruinedCity").AddCharacter(raider);

            NPC mutant = new NPC(
                "Mutant",
                "A heavily deformed humanoid, affected by extreme radiation.",
                100,
                15,
                3,
                5,
                false,
                false
            );
            mutant.DefaultDialogue = "Grrraaahhhh!";
            _gameState.GetLocation("radioactiveCrater").AddCharacter(mutant);
        }

        private void CreateQuests()
        {
            // Main quest
            _gameState.AddQuest("Find Survivors", "Search for survivors across the wasteland and bring them to safety.");

            // Side quests
            _gameState.AddQuest("Clean Water", "Find a way to purify irradiated water for the settlement.");
            _gameState.AddQuest("Secure Supplies", "Gather essential supplies (food, medicine, tools) for long-term survival.");
            _gameState.AddQuest("Neutralize Radiation", "Find a way to reduce radiation in a small area to allow for farming.");
        }

        public void CheckQuestProgress(Player player)
        {
            // Check for quest completion conditions

            // Help the Old Man quest
            if (player.ActiveQuests.Contains("Help the Old Man"))
            {
                Item medicine = player.FindItem("Antibiotics") ?? player.FindItem("First Aid Kit") ?? player.FindItem("Med Kit");
                NPC oldMan = _gameState.GetNPC("Old Man");

                if (medicine != null && player.CurrentLocation.FindCharacter("Old Man") != null)
                {
                    Console.WriteLine("You give the medicine to the Old Man.");
                    Console.WriteLine("Old Man: Thank you, my joints have been giving me trouble. Here, take this. I found it but have no use for it.");

                    player.RemoveItem(medicine);
                    player.CompleteQuest("Help the Old Man");

                    // Reward
                    Item reward = new Item("Map Fragment", "A torn piece of a map showing the location of a hidden bunker.", 0.1, ItemType.Quest, false, 20);
                    player.AddItem(reward);
                    Console.WriteLine($"You received: {reward.Name}");
                }
            }

            // Medical Research quest
            if (player.ActiveQuests.Contains("Medical Research") && player.CurrentLocation.Name == "Underground Laboratory")
            {
                // Add a research document item to the player's inventory when they reach the lab
                if (!player.Inventory.Exists(i => i.Name == "Research Documents"))
                {
                    Item documents = new Item("Research Documents", "Medical research documents about radiation treatment.", 0.2, ItemType.Quest, false, 30);
                    player.AddItem(documents);
                    Console.WriteLine("You found research documents that might help the Doctor with her radiation treatment research.");
                }
            }

            // Complete the Medical Research quest if the player has the documents and talks to the Doctor
            if (player.ActiveQuests.Contains("Medical Research"))
            {
                Item documents = player.FindItem("Research Documents");
                NPC doctor = _gameState.GetNPC("Doctor");

                if (documents != null && player.CurrentLocation.FindCharacter("Doctor") != null)
                {
                    Console.WriteLine("You give the research documents to the Doctor.");
                    Console.WriteLine("Doctor: This is exactly what I needed! With this research, I can develop better radiation treatments. Here, take this as thanks.");

                    player.RemoveItem(documents);
                    player.CompleteQuest("Medical Research");

                    // Reward
                    Item reward = new Consumable("Advanced Rad-Away", "A powerful anti-radiation medicine developed by the Doctor.", 0.2, ItemType.Medicine, 40, radiationEffect: -50);
                    player.AddItem(reward);
                    Console.WriteLine($"You received: {reward.Name}");
                }
            }

            // Find Missing Parents quest - add a sad outcome when the player reaches the ruined city
            if (player.ActiveQuests.Contains("Find Missing Parents") && player.CurrentLocation.Name == "Ruined City")
            {
                if (!player.Inventory.Exists(i => i.Name == "Family Locket"))
                {
                    Item locket = new Item("Family Locket", "A locket with a family photo inside. It belongs to the missing parents.", 0.1, ItemType.Quest, false, 10);
                    player.AddItem(locket);
                    Console.WriteLine("Among the ruins, you find a locket with a family photo. You recognize the Child's parents from the picture. There are signs of a struggle nearby. It doesn't look good.");
                }
            }

            // Complete the Find Missing Parents quest with a bittersweet ending
            if (player.ActiveQuests.Contains("Find Missing Parents"))
            {
                Item locket = player.FindItem("Family Locket");
                NPC child = _gameState.GetNPC("Child");

                if (locket != null && player.CurrentLocation.FindCharacter("Child") != null)
                {
                    Console.WriteLine("With a heavy heart, you show the locket to the Child.");
                    Console.WriteLine("Child: That's... that's my mom's. They're not coming back, are they?");
                    Console.WriteLine("The Child takes the locket with trembling hands. After a moment of silence, they look up at you with determination.");
                    Console.WriteLine("Child: I want to be strong, like you. Can you teach me how to survive out here?");

                    player.RemoveItem(locket);
                    player.CompleteQuest("Find Missing Parents");

                    // The child becomes a companion/survivor
                    Console.WriteLine("You've gained the Child's trust. They will now help at the settlement, and you've effectively saved another survivor.");
                    player.FindSurvivor();
                }
            }

            // Forest Medicine quest
            if (player.ActiveQuests.Contains("Forest Medicine"))
            {
                Item water = player.FindItem("Purified Water") ?? player.FindItem("Clean Water");
                NPC hermit = _gameState.GetNPC("Forest Hermit");

                if (water != null && player.CurrentLocation.FindCharacter("Forest Hermit") != null)
                {
                    Console.WriteLine("You give the clean water to the Forest Hermit.");
                    Console.WriteLine("Forest Hermit: Good, good. Now watch carefully as I prepare this medicine. The forest provides everything we need, if we respect it.");

                    player.RemoveItem(water);
                    player.CompleteQuest("Forest Medicine");

                    // Reward
                    Item reward = new Consumable("Herbal Medicine", "A potent medicine made from forest plants. Heals and reduces radiation.", 0.3, ItemType.Medicine, 25, healthEffect: 30, radiationEffect: -20);
                    player.AddItem(reward);
                    Console.WriteLine($"You received: {reward.Name} and knowledge of how to make more.");
                }
            }

            // Secure the Perimeter quest
            if (player.ActiveQuests.Contains("Secure the Perimeter") && player.CurrentLocation.Name == "Military Bunker")
            {
                // This quest requires the player to defeat enemies around the military base
                // We'll simulate this by checking if the player has a strong enough weapon and good health

                if (player.EquippedWeapon != null && player.EquippedWeapon.Damage >= 10 && player.Health >= 50)
                {
                    Console.WriteLine("You help the Veteran secure the perimeter, fighting off several hostile mutants.");
                    Console.WriteLine("Veteran: Good work, soldier. The perimeter is secure for now. As promised, here's something from my stockpile.");

                    player.CompleteQuest("Secure the Perimeter");

                    // Reward
                    Item reward = new Weapon("Military Rifle", "A well-maintained military rifle with a scope.", 4.0, 25, 40, 50);
                    player.AddItem(reward);
                    Console.WriteLine($"You received: {reward.Name}");

                    // The veteran becomes a counted survivor
                    Console.WriteLine("The Veteran agrees to help coordinate defense for any survivors you find. You've effectively recruited another survivor to your cause.");
                    player.FindSurvivor();
                }
            }

            // Check for main quest progress - finding survivors
            if (player.SurvivorsFound >= _gameState.TotalSurvivors)
            {
                player.CompleteQuest("Find Survivors");
                _gameState.CheckVictoryCondition();
            }
        }
    }
}
