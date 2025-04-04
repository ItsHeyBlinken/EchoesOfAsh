using System;
using System.Threading;
using test_project.GameEngine;

namespace test_project.UI
{
    /// <summary>
    /// Handles the console user interface for the game
    /// </summary>
    public class ConsoleUI
    {
        private readonly GameState _gameState;
        private readonly CommandProcessor _commandProcessor;
        private readonly StoryManager _storyManager;

        public ConsoleUI(GameState gameState, CommandProcessor commandProcessor, StoryManager storyManager)
        {
            _gameState = gameState;
            _commandProcessor = commandProcessor;
            _storyManager = storyManager;
        }

        public void Start()
        {
            DisplayIntro();
            GameLoop();
        }

        private void DisplayIntro()
        {
            Console.Clear();
            SlowPrint(@"
█▀▀ █▀▀ █░█ █▀█ █▀▀ █▀   █▀█ █▀▀   ▄▀█ █▀ █░█
█▀▀ █▄▄ █▀█ █▄█ ██▄ ▄█   █▄█ █▀░   █▀█ ▄█ █▀█
", 10);

            SlowPrint("\nA post-apocalyptic text adventure\n", 30);
            Thread.Sleep(1000);

            SlowPrint("\n-----------------------------------------\n", 5);

            SlowPrint("\nThe world as you knew it ended 5 years ago in a flash of nuclear fire.\n", 40);
            SlowPrint("You've survived in your small bunker, but supplies are running low, and the air filtration system is failing.\n", 40);
            SlowPrint("It's time to venture out into the wasteland, to find other survivors and perhaps, a new beginning for humanity.\n", 40);
            SlowPrint("\nYour mission: Find survivors and bring them together. Humanity must endure.\n", 40);

            SlowPrint("\n-----------------------------------------\n", 5);
            SlowPrint("\nType 'help' for a list of commands.\n", 30);

            Console.WriteLine("\nPress any key to begin your journey...");
            Console.ReadKey(true);
            Console.Clear();
        }

        private void GameLoop()
        {
            // Initial look at starting location
            _commandProcessor.ProcessCommand("look");

            while (!_gameState.IsGameOver)
            {
                Console.WriteLine("\n" + _gameState.GetTimeInfo());
                Console.Write("\n> ");
                string? input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLower() == "quit" || input.ToLower() == "exit")
                {
                    if (ConfirmQuit())
                        break;
                    else
                        continue;
                }

                Console.WriteLine();
                _commandProcessor.ProcessCommand(input);

                // Check for quest progress after each command
                _storyManager.CheckQuestProgress(_gameState.Player);

                // Check if the game is over
                if (_gameState.IsGameOver)
                {
                    DisplayGameOver();
                }
            }
        }

        private bool ConfirmQuit()
        {
            Console.Write("\nAre you sure you want to quit? (y/n): ");
            string? response = Console.ReadLine();
            return response != null && (response.ToLower() == "y" || response.ToLower() == "yes");
        }

        private void DisplayGameOver()
        {
            Console.WriteLine("\n-----------------------------------------\n");
            SlowPrint("GAME OVER\n", 100);
            SlowPrint(_gameState.GameOverReason + "\n", 40);

            // Display statistics
            Console.WriteLine("\nSurvival Statistics:");
            Console.WriteLine($"Days Survived: {_gameState.Day}");
            Console.WriteLine($"Survivors Found: {_gameState.Player.SurvivorsFound}/{_gameState.TotalSurvivors}");
            Console.WriteLine($"Quests Completed: {_gameState.Player.CompletedQuests.Count}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }

        private void SlowPrint(string text, int delay)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
        }
    }
}
