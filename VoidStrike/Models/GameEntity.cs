namespace VoidStrike.Models;

public abstract record GameEntity
{
    public float X { get; init; }
    public float Y { get; init; }
    public float VelocityX { get; init; }
    public float VelocityY { get; init; }
    public float Rotation { get; init; }
    public float Radius { get; init; }
    public bool IsActive { get; init; }

    public GameEntity Move(float deltaTime, int screenWidth, int screenHeight)
    {
        var newX = X + VelocityX * deltaTime;
        var newY = Y + VelocityY * deltaTime;

        // Toroidal wrapping
        newX = newX < -Radius ? screenWidth + Radius : newX > screenWidth + Radius ? -Radius : newX;
        newY = newY < -Radius ? screenHeight + Radius : newY > screenHeight + Radius ? -Radius : newY;

        return this with { X = newX, Y = newY };
    }
}
