using System;
using System.Collections.Generic;

namespace EchoesOfAshWeb.Models
{
    /// <summary>
    /// Base class for all characters in the game
    /// </summary>
    public abstract class Character
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Agility { get; set; }
        public List<Item> Inventory { get; set; }

        public Character(string name, string description, int health, int strength, int intelligence, int agility)
        {
            Name = name;
            Description = description;
            Health = health;
            MaxHealth = health;
            Strength = strength;
            Intelligence = intelligence;
            Agility = agility;
            Inventory = new List<Item>();
        }

        public virtual bool IsAlive => Health > 0;

        public virtual void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public virtual void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public virtual void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public virtual bool RemoveItem(Item item)
        {
            return Inventory.Remove(item);
        }

        public virtual Item? FindItem(string itemName)
        {
            return Inventory.Find(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
