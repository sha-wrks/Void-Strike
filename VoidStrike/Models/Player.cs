namespace VoidStrike.Models;

public record Player : GameEntity
{
    public float ThrustMagnitude { get; init; }
    public int Lives { get; init; } = 3;
    public float ShootCooldown { get; init; }
    public float InvincibilityTimer { get; init; }

    public bool IsThrusting => ThrustMagnitude > 0;
    public bool IsInvincible => InvincibilityTimer > 0;

    public static Player Create(float x, float y) => new()
    {
        X = x, Y = y,
        Radius = 10f,
        IsActive = true,
        Lives = 3
    };

    public Player Rotate(float deltaRotation) =>
        this with { Rotation = Rotation + deltaRotation };

    // Accumulates thrust in the direction the ship is pointing
    public Player ApplyThrust(float acceleration, float deltaTime)
    {
        const float maxSpeed = 400f;
        var vx = Math.Clamp(VelocityX + MathF.Sin(Rotation) * acceleration * deltaTime, -maxSpeed, maxSpeed);
        var vy = Math.Clamp(VelocityY - MathF.Cos(Rotation) * acceleration * deltaTime, -maxSpeed, maxSpeed);
        return this with { VelocityX = vx, VelocityY = vy, ThrustMagnitude = acceleration };
    }

    public Player ApplyDrag(float deltaTime) =>
        this with { VelocityX = VelocityX * (1f - 0.8f * deltaTime), VelocityY = VelocityY * (1f - 0.8f * deltaTime), ThrustMagnitude = 0 };

    public Player UpdateCooldowns(float deltaTime) =>
        this with
        {
            ShootCooldown = MathF.Max(0, ShootCooldown - deltaTime),
            InvincibilityTimer = MathF.Max(0, InvincibilityTimer - deltaTime)
        };

    public (Player updatedPlayer, Bullet? bullet) TryShoot()
    {
        if (ShootCooldown > 0) return (this, null);

        var bullet = new Bullet
        {
            X = X + MathF.Sin(Rotation) * 15f,
            Y = Y - MathF.Cos(Rotation) * 15f,
            VelocityX = MathF.Sin(Rotation) * 500f + VelocityX,
            VelocityY = -MathF.Cos(Rotation) * 500f + VelocityY,
            Radius = 3f,
            IsActive = true,
            Lifetime = 1.5f
        };

        return (this with { ShootCooldown = 0.2f }, bullet);
    }

    public Player TakeDamage() =>
        this with { Lives = Lives - 1, X = 512, Y = 384, VelocityX = 0, VelocityY = 0, InvincibilityTimer = 2f };
}
