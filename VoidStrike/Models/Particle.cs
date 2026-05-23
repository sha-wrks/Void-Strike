namespace VoidStrike.Models;

public record Particle : GameEntity
{
    public float Lifetime { get; init; }
    public float MaxLifetime { get; init; }
    public string Color { get; init; } = "#00D9FF";

    public float GetAlpha() => MaxLifetime > 0 ? Lifetime / MaxLifetime : 0;

    public Particle UpdateLifetime(float deltaTime)
    {
        var newLifetime = Lifetime - deltaTime;
        return this with { Lifetime = newLifetime, IsActive = newLifetime > 0 };
    }
}
