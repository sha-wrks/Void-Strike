# Void Strike

Classic arcade shooter built in **C# / Blazor WebAssembly** — runs entirely in the browser with zero server dependencies.

## Controls

| Key | Action |
|-----|--------|
| `W` | Thrust forward |
| `A` / `D` | Rotate left / right |
| `Space` | Shoot |
| `P` | Pause / Resume |
| `R` | Restart (after Game Over) |

## Scoring

| Asteroid | Points |
|----------|--------|
| Large | 20 |
| Medium | 50 |
| Small | 100 |

## Tech Stack

- **.NET 9** Blazor WebAssembly
- **Canvas 2D API** via JavaScript interop
- Immutable game state (C# records)
- Clean service architecture (GameEngine / PhysicsEngine / InputManager)
- GitHub Actions CI/CD → GitHub Pages

## Run locally

```bash
cd VoidStrike
dotnet run
```

Open `https://localhost:5001`

## Build for production

```bash
cd VoidStrike
dotnet publish -c Release
```
