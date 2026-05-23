using VoidStrike.Models;

namespace VoidStrike.Services;

public class GameEngine
{
    private const int ScreenWidth = 1024;
    private const int ScreenHeight = 768;
    private const float RotationSpeed = 3f;
    private const float ThrustAcceleration = 300f;

    private readonly PhysicsEngine _physics;
    private readonly InputManager _input;
    private readonly Random _rng = new();

    public GameEngine(PhysicsEngine physics, InputManager input)
    {
        _physics = physics;
        _input = input;
    }

    public GameState Update(GameState state, float deltaTime)
    {
        deltaTime = Math.Min(deltaTime, 0.05f); // cap at 50ms to avoid spiral of death

        var next = state.Status switch
        {
            GameStatus.Menu => HandleMenu(state, deltaTime),
            GameStatus.Playing => HandlePlaying(state, deltaTime),
            GameStatus.Paused => HandlePaused(state, deltaTime),
            GameStatus.LevelComplete => HandleLevelComplete(state, deltaTime),
            GameStatus.GameOver => HandleGameOver(state, deltaTime),
            _ => state
        };

        _input.EndFrame();
        return next with { FPS = deltaTime > 0 ? 1f / deltaTime : 60f };
    }

    private GameState HandleMenu(GameState state, float deltaTime)
    {
        if (_input.WasKeyJustPressed(" "))
        {
            return state with
            {
                Status = GameStatus.Playing,
                Player = Player.Create(ScreenWidth / 2f, ScreenHeight / 2f),
                Asteroids = SpawnAsteroids(5, 1),
                Bullets = [],
                Particles = [],
                Score = 0,
                Level = 1
            };
        }
        return state;
    }

    private GameState HandlePlaying(GameState state, float deltaTime)
    {
        if (_input.WasKeyJustPressed("p"))
            return state with { Status = GameStatus.Paused };

        // --- Player input ---
        var player = state.Player;

        if (_input.IsKeyPressed("a")) player = player.Rotate(-RotationSpeed * deltaTime);
        if (_input.IsKeyPressed("d")) player = player.Rotate(RotationSpeed * deltaTime);

        if (_input.IsKeyPressed("w"))
            player = player.ApplyThrust(ThrustAcceleration, deltaTime);
        else
            player = player.ApplyDrag(deltaTime);

        player = player.UpdateCooldowns(deltaTime);

        var bullets = new List<Bullet>(state.Bullets);
        if (_input.IsKeyPressed(" "))
        {
            var (p, bullet) = player.TryShoot();
            player = p;
            if (bullet is not null) bullets.Add(bullet);
        }

        // --- Move everything ---
        player = (Player)player.Move(deltaTime, ScreenWidth, ScreenHeight);
        var asteroids = state.Asteroids.Select(a => (Asteroid)a.Spin(deltaTime).Move(deltaTime, ScreenWidth, ScreenHeight)).ToList();
        bullets = bullets.Select(b => (Bullet)b.UpdateLifetime(deltaTime).Move(deltaTime, ScreenWidth, ScreenHeight)).Where(b => b.IsActive).ToList();
        var particles = state.Particles.Select(p => (Particle)p.UpdateLifetime(deltaTime).Move(deltaTime, ScreenWidth, ScreenHeight)).Where(p => p.IsActive).ToList();

        // --- Bullet-asteroid collisions ---
        var hits = _physics.GetBulletAsteroidCollisions(bullets, asteroids);
        var hitBullets = new HashSet<int>();
        var hitAsteroids = new HashSet<int>();
        var score = state.Score;
        var newAsteroids = new List<Asteroid>();

        foreach (var (bi, ai) in hits)
        {
            hitBullets.Add(bi);
            if (hitAsteroids.Add(ai))
            {
                score += asteroids[ai].GetPoints();
                newAsteroids.AddRange(asteroids[ai].Split(_rng));
                SpawnExplosionParticles(particles, asteroids[ai]);
            }
        }

        bullets = bullets.Where((_, i) => !hitBullets.Contains(i)).ToList();
        asteroids = asteroids.Where((_, i) => !hitAsteroids.Contains(i)).Concat(newAsteroids).ToList();

        // --- Player-asteroid collision ---
        if (_physics.CheckPlayerAsteroidCollision(player, asteroids))
        {
            player = player.TakeDamage();
            SpawnExplosionParticles(particles, player, "#FF006E");
            if (player.Lives <= 0)
                return state with { Status = GameStatus.GameOver, Score = score, Player = player, Particles = particles };
        }

        // --- Level complete ---
        if (asteroids.Count == 0)
            return state with { Status = GameStatus.LevelComplete, Score = score, Level = state.Level + 1, Player = player, Bullets = [], Particles = particles, LevelCompleteTimer = 0 };

        return state with
        {
            Player = player,
            Asteroids = asteroids,
            Bullets = bullets,
            Particles = particles,
            Score = score,
            ElapsedTime = state.ElapsedTime + deltaTime
        };
    }

    private GameState HandlePaused(GameState state, float deltaTime)
    {
        if (_input.WasKeyJustPressed("p"))
            return state with { Status = GameStatus.Playing };
        return state;
    }

    private GameState HandleLevelComplete(GameState state, float deltaTime)
    {
        var timer = state.LevelCompleteTimer + deltaTime;
        if (timer >= 2f)
        {
            return state with
            {
                Status = GameStatus.Playing,
                Asteroids = SpawnAsteroids(4 + state.Level, state.Level),
                Bullets = [],
                Player = Player.Create(ScreenWidth / 2f, ScreenHeight / 2f) with { Lives = state.Player.Lives },
                LevelCompleteTimer = 0
            };
        }
        return state with { LevelCompleteTimer = timer, ElapsedTime = state.ElapsedTime + deltaTime };
    }

    private GameState HandleGameOver(GameState state, float deltaTime)
    {
        if (_input.WasKeyJustPressed("r"))
            return GameState.CreateInitial();
        return state;
    }

    private List<Asteroid> SpawnAsteroids(int count, int level)
    {
        var result = new List<Asteroid>(count);
        for (int i = 0; i < count; i++)
        {
            float x, y;
            do
            {
                x = _rng.NextSingle() * (ScreenWidth - 200) + 100;
                y = _rng.NextSingle() * (ScreenHeight - 200) + 100;
            }
            while (MathF.Pow(x - ScreenWidth / 2f, 2) + MathF.Pow(y - ScreenHeight / 2f, 2) < 160f * 160f);

            var angle = _rng.NextSingle() * MathF.PI * 2f;
            var speed = 40f + _rng.NextSingle() * (60f + level * 15f);

            result.Add(new Asteroid
            {
                X = x, Y = y,
                VelocityX = MathF.Cos(angle) * speed,
                VelocityY = MathF.Sin(angle) * speed,
                Size = AsteroidSize.Large,
                Radius = (float)AsteroidSize.Large / 2f,
                AngularVelocity = (_rng.NextSingle() - 0.5f) * 2f,
                IsActive = true
            });
        }
        return result;
    }

    private static void SpawnExplosionParticles(List<Particle> particles, GameEntity source, string color = "#00D9FF")
    {
        var rng = Random.Shared;
        int count = source is Asteroid a ? (a.Size == AsteroidSize.Large ? 12 : a.Size == AsteroidSize.Medium ? 8 : 5) : 8;
        for (int i = 0; i < count; i++)
        {
            var angle = rng.NextSingle() * MathF.PI * 2f;
            var speed = 60f + rng.NextSingle() * 180f;
            var life = 0.4f + rng.NextSingle() * 0.4f;
            particles.Add(new Particle
            {
                X = source.X, Y = source.Y,
                VelocityX = MathF.Cos(angle) * speed,
                VelocityY = MathF.Sin(angle) * speed,
                Lifetime = life,
                MaxLifetime = life,
                Radius = 1.5f,
                IsActive = true,
                Color = color
            });
        }
    }
}
