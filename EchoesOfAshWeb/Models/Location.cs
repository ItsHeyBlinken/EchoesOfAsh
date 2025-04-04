using System;
using System.Collections.Generic;

namespace EchoesOfAshWeb.Models
{
    /// <summary>
    /// Represents a location in the game world
    /// </summary>
    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DetailedDescription { get; set; }
        public bool IsExplored { get; set; }
        public bool IsSafe { get; set; }
        public int RadiationLevel { get; set; }
        public Dictionary<string, Location> Exits { get; set; }
        public List<Item> Items { get; set; }
        public List<NPC> Characters { get; set; }
        public bool HasWater { get; set; }
        public bool HasFood { get; set; }
        public bool HasShelter { get; set; }

        public Location(string name, string description, string detailedDescription, bool isSafe, int radiationLevel)
        {
            Name = name;
            Description = description;
            DetailedDescription = detailedDescription;
            IsExplored = false;
            IsSafe = isSafe;
            RadiationLevel = radiationLevel;
            Exits = new Dictionary<string, Location>();
            Items = new List<Item>();
            Characters = new List<NPC>();
            HasWater = false;
            HasFood = false;
            HasShelter = false;
        }

        public void AddExit(string direction, Location location)
        {
            Exits[direction.ToLower()] = location;
        }

        public Location? GetExit(string direction)
        {
            if (Exits.TryGetValue(direction.ToLower(), out Location location))
                return location;
            
            return null;
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public Item? FindItem(string itemName)
        {
            return Items.Find(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        public bool RemoveItem(Item item)
        {
            return Items.Remove(item);
        }

        public void AddCharacter(NPC character)
        {
            Characters.Add(character);
        }

        public NPC? FindCharacter(string characterName)
        {
            return Characters.Find(c => c.Name.Equals(characterName, StringComparison.OrdinalIgnoreCase));
        }

        public bool RemoveCharacter(NPC character)
        {
            return Characters.Remove(character);
        }

        public string GetExitsList()
        {
            if (Exits.Count == 0)
                return "There are no visible exits.";

            string exitList = "Exits: ";
            foreach (var exit in Exits)
            {
                exitList += $"{exit.Key} ";
            }
            return exitList;
        }

        public string GetItemsList()
        {
            if (Items.Count == 0)
                return "There are no items here.";

            string itemList = "Items: \n";
            foreach (var item in Items)
            {
                itemList += $"- {item.Name}\n";
            }
            return itemList;
        }

        public string GetCharactersList()
        {
            if (Characters.Count == 0)
                return "There is no one here.";

            string characterList = "Characters: \n";
            foreach (var character in Characters)
            {
                characterList += $"- {character.Name}\n";
            }
            return characterList;
        }

        public string GetFullDescription()
        {
            string description = $"{Name}\n";
            description += $"{(IsExplored ? DetailedDescription : Description)}\n";
            
            if (RadiationLevel > 0)
                description += $"Radiation Level: {GetRadiationDescription()}\n";
            
            description += GetExitsList() + "\n";
            description += GetItemsList() + "\n";
            description += GetCharactersList();
            
            return description;
        }

        private string GetRadiationDescription()
        {
            return RadiationLevel switch
            {
                0 => "None",
                1 => "Low",
                2 => "Moderate",
                3 => "High",
                4 => "Very High",
                _ => "Deadly"
            };
        }
    }
}
