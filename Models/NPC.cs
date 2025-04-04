using System;
using System.Collections.Generic;

namespace test_project.Models
{
    /// <summary>
    /// Represents a non-player character in the game
    /// </summary>
    public class NPC : Character
    {
        public bool IsFriendly { get; set; }
        public bool IsSurvivor { get; set; }
        public Dictionary<string, string> DialogueOptions { get; set; }
        public string DefaultDialogue { get; set; } = "Hello there.";
        public List<Item> Trades { get; set; }

        public NPC(string name, string description, int health, int strength, int intelligence, int agility, bool isFriendly, bool isSurvivor)
            : base(name, description, health, strength, intelligence, agility)
        {
            IsFriendly = isFriendly;
            IsSurvivor = isSurvivor;
            DialogueOptions = new Dictionary<string, string>();
            Trades = new List<Item>();
        }

        public void AddDialogueOption(string keyword, string response)
        {
            DialogueOptions[keyword.ToLower()] = response;
        }

        public string GetDialogue(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return DefaultDialogue;

            if (DialogueOptions.TryGetValue(keyword.ToLower(), out string response))
                return response;

            return "I don't know anything about that.";
        }

        public void AddTradeItem(Item item)
        {
            Trades.Add(item);
        }

        public Item GetTradeItem(string itemName)
        {
            return Trades.Find(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        public bool RemoveTradeItem(Item item)
        {
            return Trades.Remove(item);
        }

        public string GetTradeList()
        {
            if (Trades.Count == 0)
                return "I have nothing to trade.";

            string tradeList = "I have these items to trade:\n";
            foreach (var item in Trades)
            {
                tradeList += $"- {item.Name} (Value: {item.Value})\n";
            }
            return tradeList;
        }
    }
}
