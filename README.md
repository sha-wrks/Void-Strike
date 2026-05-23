<div align="center">

# Void Strike

A classic arcade shooter built with **C# and Blazor WebAssembly**, running entirely in the browser with no server-side dependencies. Inspired by the Asteroids genre, Void Strike demonstrates that C# can power real-time interactive applications client-side through WebAssembly.

![License](https://img.shields.io/github/license/sha-wrks/Void-Strike)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![Platform](https://img.shields.io/badge/platform-WebAssembly-blue)

</div>

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running Locally](#running-locally)
- [How to Play](#how-to-play)
  - [Controls](#controls)
  - [Scoring](#scoring)
  - [Game States](#game-states)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Building for Production](#building-for-production)
- [Contributing](#contributing)
- [Security](#security)
- [License](#license)

---

## Overview

Void Strike is a 2D space shooter where the player pilots a ship through waves of asteroids. The game runs entirely in the browser via Blazor WebAssembly, with game logic written in C# and rendering handled through the Canvas 2D API via JavaScript interop.

The project was built as a portfolio piece to showcase:

- C# compiled to WebAssembly running at 60 FPS in a browser
- Clean service-based architecture with immutable state management
- Real-time game loop without any game framework dependency
- Static deployment to GitHub Pages with zero server cost

---

## Features

- 60 FPS gameplay with smooth physics and collision detection
- Asteroid splitting: large asteroids break into medium, medium into small
- Particle explosion effects on asteroid destruction
- Progressive difficulty across levels
- Player thrust with momentum and drag physics
- Toroidal world wrapping (ship and projectiles wrap around screen edges)
- Invincibility frames on player damage with center respawn
- Menu, paused, level complete, and game over states
- Live FPS counter
- Responsive canvas that scales to any screen size

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 13 / .NET 9 |
| Framework | Blazor WebAssembly |
| Rendering | Canvas 2D API via JS interop |
| State management | Immutable C# records |
| Hosting | GitHub Pages |
| CI/CD | GitHub Actions |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A modern browser (Chrome, Firefox, Edge, Safari)

### Installation

Clone the repository:

```bash
git clone https://github.com/sha-wrks/Void-Strike.git
cd Void-Strike
```

Restore dependencies:

```bash
cd VoidStrike
dotnet restore
```

### Running Locally

```bash
dotnet run
```

Open your browser and navigate to `https://localhost:5001`.

---

## How to Play

### Controls

| Key | Action |
|---|---|
| `W` | Thrust forward |
| `A` | Rotate left |
| `D` | Rotate right |
| `Space` | Shoot |
| `P` | Pause / Resume |
| `R` | Restart after game over |

### Scoring

| Asteroid Size | Points |
|---|---|
| Large | 20 |
| Medium | 50 |
| Small | 100 |

Destroying a large asteroid splits it into two medium asteroids. Destroying a medium asteroid splits it into two small asteroids. Small asteroids are destroyed completely.

### Game States

| State | Description |
|---|---|
| Menu | Press `Space` to begin |
| Playing | Active gameplay |
| Paused | Press `P` to resume |
| Level Complete | All asteroids cleared, next wave begins after 2 seconds |
| Game Over | Lives depleted, press `R` to restart |

---

## Project Structure

```
Void-Strike/
├── VoidStrike/
│   ├── Models/
│   │   ├── Enums.cs            # GameStatus, AsteroidSize
│   │   ├── GameEntity.cs       # Abstract base record for all entities
│   │   ├── Player.cs           # Player ship with movement and shooting
│   │   ├── Asteroid.cs         # Asteroid with spin and split behavior
│   │   ├── Bullet.cs           # Projectile with lifetime
│   │   └── Particle.cs         # Visual particle for explosions
│   ├── Services/
│   │   ├── GameState.cs        # Immutable per-frame game snapshot
│   │   ├── GameEngine.cs       # Main game loop and state transitions
│   │   ├── PhysicsEngine.cs    # Collision detection
│   │   └── InputManager.cs     # Keyboard input tracking
│   ├── Pages/
│   │   └── GamePage.razor      # Root Blazor component and game loop
│   ├── wwwroot/
│   │   ├── js/canvas.js        # Canvas 2D rendering
│   │   ├── css/game.css        # Neon dark theme
│   │   └── index.html          # Entry point
│   └── VoidStrike.csproj
├── .github/
│   ├── ISSUE_TEMPLATE/
│   └── FUNDING.yml
├── CODE_OF_CONDUCT.md
├── CONTRIBUTING.md
├── LICENSE
└── SECURITY.md
```

---

## Architecture

The game follows an immutable state pattern. Each frame produces a new `GameState` record rather than mutating existing state.

```
GamePage.razor  (Blazor component)
    |
    |-- async game loop (Task.Delay ~16ms)
    |
    +-- GameEngine.Update(state, deltaTime)
    |       |
    |       +-- InputManager    reads keyboard state
    |       +-- Player          applies thrust, rotation, shooting
    |       +-- PhysicsEngine   detects collisions
    |       +-- Asteroid        splits on hit, spawns particles
    |       +-- new GameState   returned as immutable snapshot
    |
    +-- JS interop: vsGame.render(state)
            |
            +-- canvas.js draws player, asteroids, bullets, particles, HUD
```

Key design decisions:

- **Immutable records**: `GameState` and all entities are C# records using `with` expressions for updates. This prevents accidental mutations and simplifies reasoning about frame state.
- **No game framework**: All game loop, physics, and rendering logic is written from scratch to keep the bundle lean and demonstrate the capability of raw Blazor WASM.
- **JS interop boundary**: C# handles all game logic. JavaScript only handles Canvas 2D drawing, keeping the JS surface minimal.

---

## Building for Production

```bash
cd VoidStrike
dotnet publish -c Release
```

The compiled output is placed in `VoidStrike/bin/Release/net9.0/publish/wwwroot/`.

To serve it locally for testing:

```bash
cd VoidStrike/bin/Release/net9.0/publish/wwwroot
npx serve .
```

---

## Contributing

Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request, and follow the [Code of Conduct](CODE_OF_CONDUCT.md).

To report a bug or request a feature, open an issue using the appropriate template.

---

## Security

To report a security vulnerability, please follow the process described in [SECURITY.md](SECURITY.md). Do not open a public issue for security concerns.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
