using VoidStrike.Models;

namespace VoidStrike.Services;

public class PhysicsEngine
{
    private static bool CirclesOverlap(float x1, float y1, float r1, float x2, float y2, float r2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return dx * dx + dy * dy < (r1 + r2) * (r1 + r2);
    }

    public List<(int bulletIndex, int asteroidIndex)> GetBulletAsteroidCollisions(
        List<Bullet> bullets, List<Asteroid> asteroids)
    {
        var result = new List<(int, int)>();
        for (int b = 0; b < bullets.Count; b++)
        {
            var bullet = bullets[b];
            if (!bullet.IsActive) continue;
            for (int a = 0; a < asteroids.Count; a++)
            {
                var asteroid = asteroids[a];
                if (!asteroid.IsActive) continue;
                if (CirclesOverlap(bullet.X, bullet.Y, bullet.Radius, asteroid.X, asteroid.Y, asteroid.Radius))
                    result.Add((b, a));
            }
        }
        return result;
    }

    public bool CheckPlayerAsteroidCollision(Player player, List<Asteroid> asteroids)
    {
        if (!player.IsActive || player.IsInvincible) return false;
        foreach (var a in asteroids)
        {
            if (a.IsActive && CirclesOverlap(player.X, player.Y, player.Radius, a.X, a.Y, a.Radius))
                return true;
        }
        return false;
    }
}
