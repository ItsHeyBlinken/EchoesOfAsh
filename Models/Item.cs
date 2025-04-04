using System;

namespace test_project.Models
{
    /// <summary>
    /// Represents an item that can be found, carried, and used in the game
    /// </summary>
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public ItemType Type { get; set; }
        public bool IsConsumable { get; set; }
        public int Value { get; set; }

        public Item(string name, string description, double weight, ItemType type, bool isConsumable, int value)
        {
            Name = name;
            Description = description;
            Weight = weight;
            Type = type;
            IsConsumable = isConsumable;
            Value = value;
        }

        public virtual string GetDescription()
        {
            return $"{Name}: {Description} (Weight: {Weight}kg, Value: {Value})";
        }
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Food,
        Water,
        Medicine,
        Tool,
        Resource,
        Quest,
        Miscellaneous
    }

    public class Weapon : Item
    {
        public int Damage { get; set; }
        public int Durability { get; set; }
        public int MaxDurability { get; set; }

        public Weapon(string name, string description, double weight, int damage, int durability, int value)
            : base(name, description, weight, ItemType.Weapon, false, value)
        {
            Damage = damage;
            Durability = durability;
            MaxDurability = durability;
        }

        public override string GetDescription()
        {
            return $"{Name}: {Description} (Damage: {Damage}, Durability: {Durability}/{MaxDurability}, Weight: {Weight}kg)";
        }
    }

    public class Consumable : Item
    {
        public int HealthEffect { get; set; }
        public int HungerEffect { get; set; }
        public int ThirstEffect { get; set; }
        public int RadiationEffect { get; set; }

        public Consumable(string name, string description, double weight, ItemType type, int value, 
                         int healthEffect = 0, int hungerEffect = 0, int thirstEffect = 0, int radiationEffect = 0)
            : base(name, description, weight, type, true, value)
        {
            HealthEffect = healthEffect;
            HungerEffect = hungerEffect;
            ThirstEffect = thirstEffect;
            RadiationEffect = radiationEffect;
        }

        public override string GetDescription()
        {
            string effects = "";
            if (HealthEffect != 0) effects += $" Health: {(HealthEffect > 0 ? "+" : "")}{HealthEffect}";
            if (HungerEffect != 0) effects += $" Hunger: {(HungerEffect > 0 ? "+" : "")}{HungerEffect}";
            if (ThirstEffect != 0) effects += $" Thirst: {(ThirstEffect > 0 ? "+" : "")}{ThirstEffect}";
            if (RadiationEffect != 0) effects += $" Radiation: {(RadiationEffect > 0 ? "+" : "")}{RadiationEffect}";
            
            return $"{Name}: {Description} ({effects}, Weight: {Weight}kg)";
        }
    }
}
