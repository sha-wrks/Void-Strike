using VoidStrike.Models;

namespace VoidStrike.Services;

public record GameState
{
    public Player Player { get; init; } = Player.Create(512, 384);
    public List<Asteroid> Asteroids { get; init; } = [];
    public List<Bullet> Bullets { get; init; } = [];
    public List<Particle> Particles { get; init; } = [];
    public int Score { get; init; }
    public int Level { get; init; } = 1;
    public GameStatus Status { get; init; } = GameStatus.Menu;
    public float FPS { get; init; } = 60f;
    public float ElapsedTime { get; init; }
    public float LevelCompleteTimer { get; init; }

    public static GameState CreateInitial() => new()
    {
        Player = Player.Create(512, 384),
        Score = 0,
        Level = 1,
        Status = GameStatus.Menu,
        FPS = 60f,
        ElapsedTime = 0
    };
}
