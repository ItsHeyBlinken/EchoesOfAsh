﻿using System;
using test_project.GameEngine;
using test_project.Models;
using test_project.UI;

namespace test_project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Echoes of Ash - A Post-Apocalyptic Text Adventure";

            // Initialize game components
            GameState gameState = new GameState();
            CommandProcessor commandProcessor = new CommandProcessor(gameState);
            StoryManager storyManager = new StoryManager(gameState);
            ConsoleUI ui = new ConsoleUI(gameState, commandProcessor, storyManager);

            // Initialize the game world and story
            storyManager.InitializeStory();

            // Start the game
            ui.Start();
        }
    }
}