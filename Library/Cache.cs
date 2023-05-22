namespace Hef.Math;

internal class Cache<TKey, TValue> where TKey : notnull
{
    public delegate TValue InitializeValue(TKey key);

    private readonly Dictionary<TKey, CachedValue<TValue>> _cache = new();

    private readonly int _maxValues;

    public Cache(int maxValues = int.MaxValue)
    {
        _maxValues = maxValues;
    }

    public int Count {
        get
        {
            lock (_cache)
            {
                return _cache.Count;
            }
        }
    }

    public TValue GetOrInitializeValue(TKey key, InitializeValue initializeValue)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                return cachedValue.Value;
            }

            var value = initializeValue.Invoke(key);
            _cache.Add(key, new CachedValue<TValue>(value, _nextId ++));

            var minId = int.MaxValue;
            if (_cache.Count <= _maxValues) return value;
            TKey? keyToRemove = default;
            foreach (var kvp in _cache.Where(kvp => kvp.Value.Id < minId))
            {
                minId = kvp.Value.Id;
                keyToRemove = kvp.Key;
            }
            if (keyToRemove is not null) 
                _cache.Remove(keyToRemove);
            return value;
        }
    }

    public void Clear()
    {
        lock (_cache)
        {
            _cache.Clear();
        }
    }

    internal string Dump()
    {
        var dump = new System.Text.StringBuilder();

        lock (_cache)
        {
            foreach (var kvp in _cache)
            {
                dump.Append($"[{kvp.Value.Id}] {kvp.Key} => {kvp.Value.Value}\n");
            }
        }

        return dump.ToString();
    }

    private int _nextId;
}
    
internal struct CachedValue<TValue>
{
    public readonly int Id;
    public readonly TValue Value;

    public CachedValue(TValue value, int nextId)
    {
        Id = nextId;
        Value = value;
    }
}