using System;
using System.Collections.Generic;
using System.Linq;
using EchoesOfAshWeb.Models;

namespace EchoesOfAshWeb.GameEngine
{
    /// <summary>
    /// Processes player commands and executes the corresponding actions
    /// </summary>
    public class CommandProcessor
    {
        private readonly GameState _gameState;
        private readonly Dictionary<string, Func<string[], string>> _commands;

        public CommandProcessor(GameState gameState)
        {
            _gameState = gameState;
            _commands = new Dictionary<string, Func<string[], string>>
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

            if (_commands.TryGetValue(command, out Func<string[], string> action))
            {
                try
                {
                    return action(args);
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

        private string ShowHelp(string[] args)
        {
            return "Available commands:\n" +
                   "  help - Show this help message\n" +
                   "  look - Look around your current location\n" +
                   "  go [direction] - Move in a direction (north, south, east, west)\n" +
                   "  take [item] - Pick up an item\n" +
                   "  drop [item] - Drop an item from your inventory\n" +
                   "  inventory - Show your inventory\n" +
                   "  status - Show your current status\n" +
                   "  use [item] - Use an item from your inventory\n" +
                   "  eat [food] - Eat food from your inventory\n" +
                   "  drink [water] - Drink water from your inventory\n" +
                   "  talk [character] - Talk to a character\n" +
                   "  quests - Show your active quests\n" +
                   "  time - Show the current time and day\n" +
                   "  wait - Wait for time to pass\n" +
                   "  examine [item/character] - Examine an item or character\n" +
                   "  equip [weapon] - Equip a weapon\n" +
                   "  attack [character] - Attack a character\n" +
                   "  search - Search the area for items";
        }

        private string Look(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            _gameState.Player.CurrentLocation.IsExplored = true;
            return _gameState.Player.CurrentLocation.GetFullDescription();
        }

        private string Go(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Go where? Please specify a direction.";
            }

            string direction = args[0].ToLower();
            Location? nextLocation = _gameState.Player.CurrentLocation.GetExit(direction);

            if (nextLocation == null)
            {
                return $"You can't go {direction} from here.";
            }

            _gameState.Player.MoveTo(nextLocation);
            string result = $"You go {direction} to {nextLocation.Name}.\n";
            
            // Apply radiation damage if the location is irradiated
            if (nextLocation.RadiationLevel > 0)
            {
                int radiationDamage = nextLocation.RadiationLevel * 2;
                _gameState.Player.Radiation += radiationDamage;
                result += $"You are exposed to radiation. Radiation +{radiationDamage}\n";
            }

            // Advance time when moving
            _gameState.AdvanceTime();
            
            // Show the new location
            result += nextLocation.GetFullDescription();
            return result;
        }

        private string Take(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Take what? Please specify an item.";
            }

            string itemName = args[0];
            Item? item = _gameState.Player.CurrentLocation.FindItem(itemName);

            if (item == null)
            {
                return $"There is no {itemName} here.";
            }

            _gameState.Player.AddItem(item);
            _gameState.Player.CurrentLocation.RemoveItem(item);
            return $"You take the {item.Name}.";
        }

        private string Drop(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Drop what? Please specify an item.";
            }

            string itemName = args[0];
            Item? item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                return $"You don't have a {itemName}.";
            }

            _gameState.Player.RemoveItem(item);
            _gameState.Player.CurrentLocation.AddItem(item);
            return $"You drop the {item.Name}.";
        }

        private string ShowInventory(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            if (_gameState.Player.Inventory.Count == 0)
            {
                return "Your inventory is empty.";
            }

            string result = "Inventory:\n";
            foreach (var item in _gameState.Player.Inventory)
            {
                result += $"- {item.GetDescription()}\n";
            }

            if (_gameState.Player.EquippedWeapon != null)
            {
                result += $"\nEquipped Weapon: {_gameState.Player.EquippedWeapon.GetDescription()}";
            }
            
            return result;
        }

        private string ShowStatus(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            return _gameState.Player.GetStatus();
        }

        private string Use(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Use what? Please specify an item.";
            }

            string itemName = args[0];
            Item? item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                return $"You don't have a {itemName}.";
            }

            if (item is Consumable consumable)
            {
                if (consumable.Type == ItemType.Medicine)
                {
                    _gameState.Player.UseMedicine(consumable);
                    return $"You use the {consumable.Name}.\n" +
                           $"Health: {(consumable.HealthEffect > 0 ? "+" : "")}{consumable.HealthEffect}\n" +
                           $"Radiation: {(consumable.RadiationEffect < 0 ? "" : "+")}{consumable.RadiationEffect}";
                }
                else
                {
                    return $"You can't use {consumable.Name} like that. Try 'eat' or 'drink' instead.";
                }
            }
            else
            {
                return $"You can't use {item.Name}.";
            }
        }

        private string Eat(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Eat what? Please specify a food item.";
            }

            string itemName = args[0];
            Item? item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                return $"You don't have a {itemName}.";
            }

            if (item is Consumable food && food.Type == ItemType.Food)
            {
                _gameState.Player.Eat(food);
                string result = $"You eat the {food.Name}.\n" +
                                $"Hunger: {(food.HungerEffect > 0 ? "-" : "+")}{Math.Abs(food.HungerEffect)}";
                
                if (food.HealthEffect != 0)
                    result += $"\nHealth: {(food.HealthEffect > 0 ? "+" : "")}{food.HealthEffect}";
                    
                if (food.RadiationEffect != 0)
                    result += $"\nRadiation: {(food.RadiationEffect > 0 ? "+" : "")}{food.RadiationEffect}";
                    
                return result;
            }
            else
            {
                return $"You can't eat {item.Name}.";
            }
        }

        private string Drink(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Drink what? Please specify a water item.";
            }

            string itemName = args[0];
            Item? item = _gameState.Player.FindItem(itemName);

            if (item == null)
            {
                return $"You don't have a {itemName}.";
            }

            if (item is Consumable water && water.Type == ItemType.Water)
            {
                _gameState.Player.Drink(water);
                string result = $"You drink the {water.Name}.\n" +
                                $"Thirst: {(water.ThirstEffect > 0 ? "-" : "+")}{Math.Abs(water.ThirstEffect)}";
                
                if (water.HealthEffect != 0)
                    result += $"\nHealth: {(water.HealthEffect > 0 ? "+" : "")}{water.HealthEffect}";
                    
                if (water.RadiationEffect != 0)
                    result += $"\nRadiation: {(water.RadiationEffect > 0 ? "+" : "")}{water.RadiationEffect}";
                    
                return result;
            }
            else
            {
                return $"You can't drink {item.Name}.";
            }
        }

        private string Talk(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Talk to whom? Please specify a character.";
            }

            string[] parts = args[0].Split(' ', 2);
            string characterName = parts[0];
            string topic = parts.Length > 1 ? parts[1] : string.Empty;

            NPC? character = _gameState.Player.CurrentLocation.FindCharacter(characterName);

            if (character == null)
            {
                return $"There is no {characterName} here.";
            }

            string result = $"{character.Name}: {character.GetDialogue(topic)}";

            // If this is a survivor and they haven't been counted yet, count them
            if (character.IsSurvivor && !character.Name.StartsWith("Rescued:"))
            {
                _gameState.Player.FindSurvivor();
                character.Name = $"Rescued: {character.Name}";
                result += $"\n\nYou've found a survivor! Total survivors found: {_gameState.Player.SurvivorsFound}/{_gameState.TotalSurvivors}";
                
                // Check if all survivors have been found
                _gameState.CheckVictoryCondition();
            }
            
            return result;
        }

        private string ShowQuests(string[] args)
        {
            return _gameState.GetActiveQuestsList();
        }

        private string ShowTime(string[] args)
        {
            return _gameState.GetTimeInfo();
        }

        private string Wait(string[] args)
        {
            _gameState.AdvanceTime();
            return $"Time passes... It is now {_gameState.GetTimeInfo()}\n" +
                   (_gameState.Player != null ? _gameState.Player.GetStatus() : "");
        }

        private string Examine(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Examine what? Please specify an item or character.";
            }

            string name = args[0];
            
            // Check if it's an item in inventory
            Item? inventoryItem = _gameState.Player.FindItem(name);
            if (inventoryItem != null)
            {
                return inventoryItem.GetDescription();
            }
            
            // Check if it's an item in the location
            Item? locationItem = _gameState.Player.CurrentLocation.FindItem(name);
            if (locationItem != null)
            {
                return locationItem.GetDescription();
            }
            
            // Check if it's a character
            NPC? character = _gameState.Player.CurrentLocation.FindCharacter(name);
            if (character != null)
            {
                return $"{character.Name}: {character.Description}";
            }
            
            return $"You don't see a {name} here.";
        }

        private string Equip(string[] args)
        {
            if (_gameState.Player == null)
                return "You don't exist.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Equip what? Please specify a weapon.";
            }

            string weaponName = args[0];
            Item? item = _gameState.Player.FindItem(weaponName);

            if (item == null)
            {
                return $"You don't have a {weaponName}.";
            }

            if (item is Weapon weapon)
            {
                _gameState.Player.EquipWeapon(weapon);
                return $"You equip the {weapon.Name}.";
            }
            else
            {
                return $"{item.Name} is not a weapon.";
            }
        }

        private string Attack(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                return "Attack whom? Please specify a character.";
            }

            string characterName = args[0];
            NPC? character = _gameState.Player.CurrentLocation.FindCharacter(characterName);

            if (character == null)
            {
                return $"There is no {characterName} here.";
            }

            if (character.IsFriendly)
            {
                return $"{character.Name} is friendly. Are you sure you want to attack them?";
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
                    string result = $"Your {_gameState.Player.EquippedWeapon.Name} breaks!";
                    _gameState.Player.EquippedWeapon = null;
                    return result;
                }
            }

            character.TakeDamage(playerDamage);
            string output = $"You attack {character.Name} for {playerDamage} damage!\n";

            if (!character.IsAlive)
            {
                output += $"{character.Name} is defeated!\n";
                
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
                output += $"{character.Name} attacks you for {characterDamage} damage!\n";
                
                if (!_gameState.Player.IsAlive)
                {
                    _gameState.IsGameOver = true;
                    _gameState.GameOverReason = $"You were killed by {character.Name}.";
                }
            }
            
            return output;
        }

        private string Search(string[] args)
        {
            if (_gameState.Player?.CurrentLocation == null)
                return "You are nowhere.";
                
            Random random = new Random();
            bool foundSomething = random.Next(100) < 40; // 40% chance to find something
            
            if (foundSomething)
            {
                // Create a random item based on location
                Item? newItem = null;
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
                    string result = $"You found something! A {newItem.Name} has been added to this location.";
                    
                    // Searching takes time
                    _gameState.AdvanceTime();
                    
                    return result;
                }
            }
            
            // Searching takes time
            _gameState.AdvanceTime();
            
            return "You search the area but find nothing of value.";
        }
    }
}
