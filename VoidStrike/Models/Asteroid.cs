namespace VoidStrike.Models;

public record Asteroid : GameEntity
{
    public AsteroidSize Size { get; init; }
    public float AngularVelocity { get; init; }

    public int GetPoints() => Size switch
    {
        AsteroidSize.Large => 20,
        AsteroidSize.Medium => 50,
        AsteroidSize.Small => 100,
        _ => 0
    };

    public Asteroid Spin(float deltaTime) =>
        this with { Rotation = Rotation + AngularVelocity * deltaTime };

    public List<Asteroid> Split(Random rng)
    {
        if (Size == AsteroidSize.Small) return [];

        var nextSize = Size == AsteroidSize.Large ? AsteroidSize.Medium : AsteroidSize.Small;
        var nextRadius = (float)nextSize / 2f;
        var result = new List<Asteroid>(2);

        for (int i = 0; i < 2; i++)
        {
            var angle = rng.NextSingle() * MathF.PI * 2f;
            var speed = 80f + rng.Next(120);
            result.Add(new Asteroid
            {
                X = X, Y = Y,
                VelocityX = MathF.Cos(angle) * speed,
                VelocityY = MathF.Sin(angle) * speed,
                Size = nextSize,
                Radius = nextRadius,
                AngularVelocity = (rng.NextSingle() - 0.5f) * 4f,
                IsActive = true
            });
        }
        return result;
    }
}
