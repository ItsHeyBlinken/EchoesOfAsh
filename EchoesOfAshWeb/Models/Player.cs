using System;
using System.Collections.Generic;

namespace EchoesOfAshWeb.Models
{
    /// <summary>
    /// Represents the player character
    /// </summary>
    public class Player : Character
    {
        public int Hunger { get; set; }
        public int Thirst { get; set; }
        public int Radiation { get; set; }
        public int MaxHunger { get; set; }
        public int MaxThirst { get; set; }
        public int MaxRadiation { get; set; }
        public Weapon? EquippedWeapon { get; set; }
        public Location? CurrentLocation { get; set; }
        public List<string> CompletedQuests { get; set; }
        public List<string> ActiveQuests { get; set; }
        public int SurvivorsFound { get; set; }

        public Player(string name, string description)
            : base(name, description, 100, 10, 10, 10)
        {
            Hunger = 0;
            Thirst = 0;
            Radiation = 0;
            MaxHunger = 100;
            MaxThirst = 100;
            MaxRadiation = 100;
            CompletedQuests = new List<string>();
            ActiveQuests = new List<string>();
            SurvivorsFound = 0;
        }

        public void Eat(Consumable food)
        {
            if (food.Type != ItemType.Food)
                return;

            Heal(food.HealthEffect);
            Hunger = Math.Max(0, Math.Min(MaxHunger, Hunger - food.HungerEffect));
            Radiation += food.RadiationEffect;
            RemoveItem(food);
        }

        public void Drink(Consumable drink)
        {
            if (drink.Type != ItemType.Water)
                return;

            Heal(drink.HealthEffect);
            Thirst = Math.Max(0, Math.Min(MaxThirst, Thirst - drink.ThirstEffect));
            Radiation += drink.RadiationEffect;
            RemoveItem(drink);
        }

        public void UseMedicine(Consumable medicine)
        {
            if (medicine.Type != ItemType.Medicine)
                return;

            Heal(medicine.HealthEffect);
            Radiation = Math.Max(0, Radiation - medicine.RadiationEffect);
            RemoveItem(medicine);
        }

        public void EquipWeapon(Weapon weapon)
        {
            EquippedWeapon = weapon;
        }

        public void UpdateVitals(int hungerRate, int thirstRate)
        {
            Hunger = Math.Min(MaxHunger, Hunger + hungerRate);
            Thirst = Math.Min(MaxThirst, Thirst + thirstRate);

            // Critical conditions damage health
            if (Hunger >= MaxHunger)
                TakeDamage(5);
            
            if (Thirst >= MaxThirst)
                TakeDamage(10);
            
            if (Radiation >= MaxRadiation / 2)
                TakeDamage(Radiation / 10);
        }

        public void MoveTo(Location newLocation)
        {
            CurrentLocation = newLocation;
        }

        public void AddQuest(string questName)
        {
            if (!ActiveQuests.Contains(questName) && !CompletedQuests.Contains(questName))
                ActiveQuests.Add(questName);
        }

        public void CompleteQuest(string questName)
        {
            if (ActiveQuests.Contains(questName))
            {
                ActiveQuests.Remove(questName);
                CompletedQuests.Add(questName);
            }
        }

        public void FindSurvivor()
        {
            SurvivorsFound++;
        }

        public string GetStatus()
        {
            string healthStatus = Health >= 75 ? "Good" : Health >= 50 ? "Injured" : Health >= 25 ? "Badly Injured" : "Critical";
            string hungerStatus = Hunger <= 25 ? "Well Fed" : Hunger <= 50 ? "Hungry" : Hunger <= 75 ? "Very Hungry" : "Starving";
            string thirstStatus = Thirst <= 25 ? "Hydrated" : Thirst <= 50 ? "Thirsty" : Thirst <= 75 ? "Very Thirsty" : "Dehydrated";
            string radiationStatus = Radiation <= 25 ? "Safe" : Radiation <= 50 ? "Irradiated" : Radiation <= 75 ? "Highly Irradiated" : "Critically Irradiated";

            return $"Health: {Health}/{MaxHealth} ({healthStatus})\n" +
                   $"Hunger: {Hunger}/{MaxHunger} ({hungerStatus})\n" +
                   $"Thirst: {Thirst}/{MaxThirst} ({thirstStatus})\n" +
                   $"Radiation: {Radiation}/{MaxRadiation} ({radiationStatus})\n" +
                   $"Survivors Found: {SurvivorsFound}";
        }
    }
}
