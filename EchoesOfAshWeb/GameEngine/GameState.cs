using System;
using System.Collections.Generic;
using EchoesOfAshWeb.Models;

namespace EchoesOfAshWeb.GameEngine
{
    /// <summary>
    /// Manages the overall state of the game
    /// </summary>
    public class GameState
    {
        public Player? Player { get; set; }
        public int Day { get; set; }
        public TimeOfDay CurrentTime { get; set; }
        public bool IsGameOver { get; set; }
        public string GameOverReason { get; set; } = string.Empty;
        public Dictionary<string, Location> AllLocations { get; set; }
        public Dictionary<string, NPC> AllNPCs { get; set; }
        public Dictionary<string, string> QuestDescriptions { get; set; }
        public int TotalSurvivors { get; set; }

        public GameState()
        {
            Day = 1;
            CurrentTime = TimeOfDay.Morning;
            IsGameOver = false;
            AllLocations = new Dictionary<string, Location>();
            AllNPCs = new Dictionary<string, NPC>();
            QuestDescriptions = new Dictionary<string, string>();
            TotalSurvivors = 12; // Total survivors to find in the game
        }

        public void Initialize(Player player, Location startingLocation)
        {
            Player = player;
            Player.CurrentLocation = startingLocation;
        }

        public void AdvanceTime()
        {
            switch (CurrentTime)
            {
                case TimeOfDay.Morning:
                    CurrentTime = TimeOfDay.Afternoon;
                    break;
                case TimeOfDay.Afternoon:
                    CurrentTime = TimeOfDay.Evening;
                    break;
                case TimeOfDay.Evening:
                    CurrentTime = TimeOfDay.Night;
                    break;
                case TimeOfDay.Night:
                    CurrentTime = TimeOfDay.Morning;
                    Day++;
                    break;
            }

            // Update player's vitals based on time of day
            if (Player != null)
            {
                int hungerRate = CurrentTime == TimeOfDay.Night ? 1 : 3;
                int thirstRate = CurrentTime == TimeOfDay.Afternoon ? 5 : 2;
                
                Player.UpdateVitals(hungerRate, thirstRate);

                // Check if player is dead
                if (!Player.IsAlive)
                {
                    IsGameOver = true;
                    GameOverReason = "You have died. Your journey ends here.";
                }
            }
        }

        public void AddLocation(string key, Location location)
        {
            AllLocations[key] = location;
        }

        public Location? GetLocation(string key)
        {
            if (AllLocations.TryGetValue(key, out Location location))
                return location;
            
            return null;
        }

        public void AddNPC(string key, NPC npc)
        {
            AllNPCs[key] = npc;
        }

        public NPC? GetNPC(string key)
        {
            if (AllNPCs.TryGetValue(key, out NPC npc))
                return npc;
            
            return null;
        }

        public void AddQuest(string questName, string description)
        {
            QuestDescriptions[questName] = description;
        }

        public string GetQuestDescription(string questName)
        {
            if (QuestDescriptions.TryGetValue(questName, out string description))
                return description;
            
            return "No description available.";
        }

        public string GetActiveQuestsList()
        {
            if (Player == null || Player.ActiveQuests.Count == 0)
                return "You have no active quests.";

            string questList = "Active Quests:\n";
            foreach (var quest in Player.ActiveQuests)
            {
                questList += $"- {quest}: {GetQuestDescription(quest)}\n";
            }
            return questList;
        }

        public string GetTimeInfo()
        {
            return $"Day {Day}, {CurrentTime}";
        }

        public bool CheckVictoryCondition()
        {
            if (Player != null && Player.SurvivorsFound >= TotalSurvivors)
            {
                IsGameOver = true;
                GameOverReason = $"Congratulations! You have found all {TotalSurvivors} survivors and ensured the future of humanity.";
                return true;
            }
            return false;
        }
    }

    public enum TimeOfDay
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }
}
