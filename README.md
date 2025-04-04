# Echoes of Ash

A post-apocalyptic text adventure game built with Blazor WebAssembly.

## About the Game

The world as you knew it ended 5 years ago in a flash of nuclear fire. You've survived in your small bunker, but supplies are running low, and the air filtration system is failing. It's time to venture out into the wasteland, to find other survivors and perhaps, a new beginning for humanity.

## Play the Game

You can play the game online at: https://itsheyblinkn.github.io/EchoesOfAsh/

## Features

- Text-based adventure gameplay
- Post-apocalyptic setting
- Inventory management
- Character interactions
- Combat system
- Quest system

## Technologies Used

- C#
- Blazor WebAssembly
- .NET 9.0
- HTML/CSS
- GitHub Pages for hosting

## Development

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 or later (recommended)

### Running Locally

1. Clone the repository
   ```
   git clone https://github.com/ItsHeyBlinken/EchoesOfAsh.git
   cd EchoesOfAsh
   ```

2. Run the Blazor WebAssembly project
   ```
   cd EchoesOfAshWeb
   dotnet run
   ```

3. Open your browser and navigate to `https://localhost:7284` or `http://localhost:5229`

### Publishing to GitHub Pages

1. Run the publish script
   ```
   .\publish.ps1
   ```

2. The script will:
   - Build the Blazor WebAssembly project in Release mode
   - Create necessary GitHub Pages files
   - Update the base href for GitHub Pages
   - Output the files to the `publish/wwwroot` directory

3. The contents of the `publish/wwwroot` directory can be deployed to GitHub Pages

## License

[MIT License](LICENSE)
