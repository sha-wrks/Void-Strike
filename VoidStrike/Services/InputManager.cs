namespace VoidStrike.Services;

public class InputManager
{
    private readonly Dictionary<string, bool> _keys = new();
    private Dictionary<string, bool> _lastKeys = new();

    public void OnKeyDown(string key) => _keys[key.ToLower()] = true;
    public void OnKeyUp(string key) => _keys[key.ToLower()] = false;

    public bool IsKeyPressed(string key) => _keys.GetValueOrDefault(key.ToLower(), false);

    public bool WasKeyJustPressed(string key)
    {
        var k = key.ToLower();
        return _keys.GetValueOrDefault(k, false) && !_lastKeys.GetValueOrDefault(k, false);
    }

    public void EndFrame() => _lastKeys = new Dictionary<string, bool>(_keys);
}
