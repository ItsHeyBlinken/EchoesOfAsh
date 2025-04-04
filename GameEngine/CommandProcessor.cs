using System;
using System.Collections.Generic;
using System.Linq;
using test_project.Models;

namespace test_project.GameEngine
{
    /// <summary>
    /// Processes player commands and executes the corresponding actions
    /// </summary>
    public class CommandProcessor
    {
        private readonly GameState _gameState;
        private readonly Dictionary<string, Action<string[]>> _commands;

        public CommandProcessor(GameState gameState)
        {
            _gameState = gameState;
            _commands = new Dictionary<string, Action<string[]>>
            {
                { "help", ShowHelp },
                { "look", Look },
                { "go", Go },
                { "take", Take },
                { "drop", Drop },
                { "inventory", ShowInventory },
                { "status", ShowStatus },
                { "use", Use },
                { "eat", Eat },
                { "drink", Drink },
                { "talk", Talk },
                { "quests", ShowQuests },
                { "time", ShowTime },
                { "wait", Wait },
                { "examine", Examine },
                { "equip", Equip },
                { "attack", Attack },
                { "search", Search }
            };
        }

        public string ProcessCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please enter a command.";

            string[] parts = input.Trim().ToLower().Split(' ', 2);
            string command = parts[0];
            string[] args = parts.Length > 1 ? new[] { parts[1] } : Array.Empty<string>();

            if (_commands.TryGetValue(command, out Action<string[]> action))
            {
                try
                {
                    action(args);
                    return string.Empty; // The action methods will handle output
                }
                catch (Exception ex)
                {
                    return $"Error executing command: {ex.Message}";
                }
            }
            else
            {
                return $"I don't understand '{command}'. Type 'help' for a list of commands.";
            }
        }

        private void ShowHelp(string[] args)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  help - Show this help message");
            Console.WriteLine("  look - Look around your current location");
            Console.WriteLine("  go [direction] - Move in a direction (north, south, east, west)");
            Console.WriteLine("  take [item] - Pick up an item");
            Console.WriteLine("  drop [item] - Drop an item from your inventory");
            Console.WriteLine("  inventory - Show your inventory");
            Console.WriteLine("  status - Show your current status");
            Console.WriteLine("  use [item] - Use an item from your inventory");
            Console.WriteLine("  eat [food] - Eat food from your inventory");
            Console.WriteLine("  drink [water] - Drink water from your inventory");
            Console.WriteLine("  talk [character] - Talk to a character");
            Console.WriteLine("  quests - Show your active quests");
            Console.WriteLine("  time - Show the current time and day");
            Console.WriteLine("  wait - Wait for time to pass");
            Console.WriteLine("  examine [item/character] - Examine an item or character");
            Console.WriteLine("  equip [weapon] - Equip a weapon");
            Console.WriteLine("  attack [character] - Attack a character");
            Console.WriteLine("  search - Search the area for items");
        }

        private void Look(string[] args)
        {
            Console.WriteLine(_gameState.Player.CurrentLocation.GetFullDescription());
            _gameState.Player.CurrentLocation.IsExplored = true;
        }

        private void Go(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Go where? Please specify a direction.");
                return;
            }

            string direction = args[0].ToLower();
            Location nextLocation = _gameState.Player.CurrentLocation.GetExit(direction);

            if (nextLocation == null)
            {
                Console.WriteLine($"You can't go {direction} from here.");
                return;
            }

            _gameState.Player.MoveTo(nextLocation);
            Console.WriteLine($"You go {direction} to {nextLocation.Name}.");
            
            // Apply radiation damage if the location is irradiated
            if (nextLocation.RadiationLevel > 0)
            {
                int radiationDamage = nextLocation.RadiationLevel * 2;
                _gameState.Player.Radiation += radiationDamage;
                Console.WriteLine($"You are exposed to radiation. Radiation +{radiationDamage}");
            }

            // Advance time when moving
            _gameState.AdvanceTime();
            
            // Show the new location
            Look(Array.Empty<string>());
        }

        private void Take(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Take what? Please specify an item.");
                return;
            }

            string itemName = args[0];
            Item item = _gameState.Player.CurrentLocation.FindItem(itemName);

            if (item == null)
            {
                Console.WriteLine($"There is no {itemName} here.");
                return;
            }

            _gameState.Player.AddItem(item);
            _gameState.Player.CurrentLocation.RemoveItem(item);
            Console.WriteLine($"You take the {item.Name}.");
        }

        private void Drop(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Drop what? Please specify an item.");
                return;
            }

            string itemName = args[0];
            Item item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                Console.WriteLine($"You don't have a {itemName}.");
                return;
            }

            _gameState.Player.RemoveItem(item);
            _gameState.Player.CurrentLocation.AddItem(item);
            Console.WriteLine($"You drop the {item.Name}.");
        }

        private void ShowInventory(string[] args)
        {
            if (_gameState.Player.Inventory.Count == 0)
            {
                Console.WriteLine("Your inventory is empty.");
                return;
            }

            Console.WriteLine("Inventory:");
            foreach (var item in _gameState.Player.Inventory)
            {
                Console.WriteLine($"- {item.GetDescription()}");
            }

            if (_gameState.Player.EquippedWeapon != null)
            {
                Console.WriteLine($"\nEquipped Weapon: {_gameState.Player.EquippedWeapon.GetDescription()}");
            }
        }

        private void ShowStatus(string[] args)
        {
            Console.WriteLine(_gameState.Player.GetStatus());
        }

        private void Use(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Use what? Please specify an item.");
                return;
            }

            string itemName = args[0];
            Item item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                Console.WriteLine($"You don't have a {itemName}.");
                return;
            }

            if (item is Consumable consumable)
            {
                if (consumable.Type == ItemType.Medicine)
                {
                    _gameState.Player.UseMedicine(consumable);
                    Console.WriteLine($"You use the {consumable.Name}.");
                    Console.WriteLine($"Health: {(consumable.HealthEffect > 0 ? "+" : "")}{consumable.HealthEffect}");
                    Console.WriteLine($"Radiation: {(consumable.RadiationEffect < 0 ? "" : "+")}{consumable.RadiationEffect}");
                }
                else
                {
                    Console.WriteLine($"You can't use {consumable.Name} like that. Try 'eat' or 'drink' instead.");
                }
            }
            else
            {
                Console.WriteLine($"You can't use {item.Name}.");
            }
        }

        private void Eat(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Eat what? Please specify a food item.");
                return;
            }

            string itemName = args[0];
            Item item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                Console.WriteLine($"You don't have a {itemName}.");
                return;
            }

            if (item is Consumable food && food.Type == ItemType.Food)
            {
                _gameState.Player.Eat(food);
                Console.WriteLine($"You eat the {food.Name}.");
                Console.WriteLine($"Hunger: {(food.HungerEffect > 0 ? "-" : "+")}{Math.Abs(food.HungerEffect)}");
                if (food.HealthEffect != 0)
                    Console.WriteLine($"Health: {(food.HealthEffect > 0 ? "+" : "")}{food.HealthEffect}");
                if (food.RadiationEffect != 0)
                    Console.WriteLine($"Radiation: {(food.RadiationEffect > 0 ? "+" : "")}{food.RadiationEffect}");
            }
            else
            {
                Console.WriteLine($"You can't eat {item.Name}.");
            }
        }

        private void Drink(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Drink what? Please specify a water item.");
                return;
            }

            string itemName = args[0];
            Item item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                Console.WriteLine($"You don't have a {itemName}.");
                return;
            }

            if (item is Consumable water && water.Type == ItemType.Water)
            {
                _gameState.Player.Drink(water);
                Console.WriteLine($"You drink the {water.Name}.");
                Console.WriteLine($"Thirst: {(water.ThirstEffect > 0 ? "-" : "+")}{Math.Abs(water.ThirstEffect)}");
                if (water.HealthEffect != 0)
                    Console.WriteLine($"Health: {(water.HealthEffect > 0 ? "+" : "")}{water.HealthEffect}");
                if (water.RadiationEffect != 0)
                    Console.WriteLine($"Radiation: {(water.RadiationEffect > 0 ? "+" : "")}{water.RadiationEffect}");
            }
            else
            {
                Console.WriteLine($"You can't drink {item.Name}.");
            }
        }

        private void Talk(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Talk to whom? Please specify a character.");
                return;
            }

            string[] parts = args[0].Split(' ', 2);
            string characterName = parts[0];
            string topic = parts.Length > 1 ? parts[1] : string.Empty;

            NPC character = _gameState.Player.CurrentLocation.FindCharacter(characterName);

            if (character == null)
            {
                Console.WriteLine($"There is no {characterName} here.");
                return;
            }

            Console.WriteLine($"{character.Name}: {character.GetDialogue(topic)}");

            // If this is a survivor and they haven't been counted yet, count them
            if (character.IsSurvivor && !character.Name.StartsWith("Rescued:"))
            {
                _gameState.Player.FindSurvivor();
                character.Name = $"Rescued: {character.Name}";
                Console.WriteLine($"You've found a survivor! Total survivors found: {_gameState.Player.SurvivorsFound}/{_gameState.TotalSurvivors}");
                
                // Check if all survivors have been found
                _gameState.CheckVictoryCondition();
            }
        }

        private void ShowQuests(string[] args)
        {
            Console.WriteLine(_gameState.GetActiveQuestsList());
        }

        private void ShowTime(string[] args)
        {
            Console.WriteLine(_gameState.GetTimeInfo());
        }

        private void Wait(string[] args)
        {
            _gameState.AdvanceTime();
            Console.WriteLine($"Time passes... It is now {_gameState.GetTimeInfo()}");
            Console.WriteLine(_gameState.Player.GetStatus());
        }

        private void Examine(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Examine what? Please specify an item or character.");
                return;
            }

            string name = args[0];
            
            // Check if it's an item in inventory
            Item inventoryItem = _gameState.Player.FindItem(name);
            if (inventoryItem != null)
            {
                Console.WriteLine(inventoryItem.GetDescription());
                return;
            }
            
            // Check if it's an item in the location
            Item locationItem = _gameState.Player.CurrentLocation.FindItem(name);
            if (locationItem != null)
            {
                Console.WriteLine(locationItem.GetDescription());
                return;
            }
            
            // Check if it's a character
            NPC character = _gameState.Player.CurrentLocation.FindCharacter(name);
            if (character != null)
            {
                Console.WriteLine($"{character.Name}: {character.Description}");
                return;
            }
            
            Console.WriteLine($"You don't see a {name} here.");
        }

        private void Equip(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Equip what? Please specify a weapon.");
                return;
            }

            string weaponName = args[0];
            Item item = _gameState.Player.FindItem(weaponName);

            if (item == null)
            {
                Console.WriteLine($"You don't have a {weaponName}.");
                return;
            }

            if (item is Weapon weapon)
            {
                _gameState.Player.EquipWeapon(weapon);
                Console.WriteLine($"You equip the {weapon.Name}.");
            }
            else
            {
                Console.WriteLine($"{item.Name} is not a weapon.");
            }
        }

        private void Attack(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Attack whom? Please specify a character.");
                return;
            }

            string characterName = args[0];
            NPC character = _gameState.Player.CurrentLocation.FindCharacter(characterName);

            if (character == null)
            {
                Console.WriteLine($"There is no {characterName} here.");
                return;
            }

            if (character.IsFriendly)
            {
                Console.WriteLine($"{character.Name} is friendly. Are you sure you want to attack them?");
                return;
            }

            // Combat logic
            int playerDamage = _gameState.Player.Strength;
            if (_gameState.Player.EquippedWeapon != null)
            {
                playerDamage += _gameState.Player.EquippedWeapon.Damage;
                
                // Reduce weapon durability
                _gameState.Player.EquippedWeapon.Durability--;
                if (_gameState.Player.EquippedWeapon.Durability <= 0)
                {
                    Console.WriteLine($"Your {_gameState.Player.EquippedWeapon.Name} breaks!");
                    _gameState.Player.EquippedWeapon = null;
                }
            }

            character.TakeDamage(playerDamage);
            Console.WriteLine($"You attack {character.Name} for {playerDamage} damage!");

            if (!character.IsAlive)
            {
                Console.WriteLine($"{character.Name} is defeated!");
                
                // Transfer any items from the character to the location
                foreach (var item in character.Inventory)
                {
                    _gameState.Player.CurrentLocation.AddItem(item);
                }
                
                _gameState.Player.CurrentLocation.RemoveCharacter(character);
            }
            else
            {
                // Character counterattacks
                int characterDamage = character.Strength;
                _gameState.Player.TakeDamage(characterDamage);
                Console.WriteLine($"{character.Name} attacks you for {characterDamage} damage!");
                
                if (!_gameState.Player.IsAlive)
                {
                    _gameState.IsGameOver = true;
                    _gameState.GameOverReason = $"You were killed by {character.Name}.";
                }
            }
        }

        private void Search(string[] args)
        {
            Random random = new Random();
            bool foundSomething = random.Next(100) < 40; // 40% chance to find something
            
            if (foundSomething)
            {
                // Create a random item based on location
                Item newItem = null;
                int itemType = random.Next(100);
                
                if (itemType < 30) // 30% chance for food
                {
                    newItem = new Consumable(
                        "Canned Food", 
                        "A dusty can of preserved food. Still edible... probably.", 
                        0.5, 
                        ItemType.Food, 
                        5, 
                        healthEffect: 5, 
                        hungerEffect: 30, 
                        radiationEffect: random.Next(5)
                    );
                }
                else if (itemType < 60) // 30% chance for water
                {
                    newItem = new Consumable(
                        "Water Bottle", 
                        "A bottle of somewhat clean water.", 
                        1.0, 
                        ItemType.Water, 
                        5, 
                        healthEffect: 0, 
                        thirstEffect: 40, 
                        radiationEffect: random.Next(3)
                    );
                }
                else if (itemType < 75) // 15% chance for medicine
                {
                    newItem = new Consumable(
                        "Med Kit", 
                        "A small medical kit with basic supplies.", 
                        0.3, 
                        ItemType.Medicine, 
                        10, 
                        healthEffect: 30, 
                        radiationEffect: -10
                    );
                }
                else if (itemType < 90) // 15% chance for weapon
                {
                    newItem = new Weapon(
                        "Rusty Pipe", 
                        "A rusty metal pipe that can be used as a weapon.", 
                        2.0, 
                        10, 
                        20, 
                        8
                    );
                }
                else // 10% chance for valuable item
                {
                    newItem = new Item(
                        "Valuable Trinket", 
                        "A small valuable item that might be worth something to the right person.", 
                        0.1, 
                        ItemType.Miscellaneous, 
                        false, 
                        15
                    );
                }
                
                if (newItem != null)
                {
                    _gameState.Player.CurrentLocation.AddItem(newItem);
                    Console.WriteLine($"You found something! A {newItem.Name} has been added to this location.");
                }
            }
            else
            {
                Console.WriteLine("You search the area but find nothing of value.");
            }
            
            // Searching takes time
            _gameState.AdvanceTime();
        }
    }
}
