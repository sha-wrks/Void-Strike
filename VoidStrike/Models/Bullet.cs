namespace VoidStrike.Models;

public record Bullet : GameEntity
{
    public float Lifetime { get; init; } = 3f;

    public Bullet UpdateLifetime(float deltaTime)
    {
        var newLifetime = Lifetime - deltaTime;
        return this with { Lifetime = newLifetime, IsActive = newLifetime > 0 };
    }
}
