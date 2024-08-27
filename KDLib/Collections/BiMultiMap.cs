using System.Collections.Generic;

namespace KDLib.Collections
{
  public class BiMultiMap<TKey, TValue> where TKey : notnull where TValue : notnull
  {
    private readonly Dictionary<TKey, HashSet<TValue>> _mapForward = new();
    private readonly Dictionary<TValue, HashSet<TKey>> _mapReverse = new();

    private static readonly HashSet<TKey> EmptyKeysSet = [];
    private static readonly HashSet<TValue> EmptyValuesSet = [];

    public void Add(TKey key, TValue value)
    {
      if (_mapForward.TryGetValue(key, out var valuesSet))
        valuesSet.Add(value);
      else
        _mapForward[key] = [value];

      if (_mapReverse.TryGetValue(value, out var keysSet))
        keysSet.Add(key);
      else
        _mapReverse[value] = [key];
    }

    public void DeleteByKey(TKey key, bool missingOk = false)
    {
      HashSet<TValue>? valuesSet;

      if (!_mapForward.Remove(key, out valuesSet)) {
        if (!missingOk)
          throw new KeyNotFoundException();
        else
          return;
      }

      foreach (var value in valuesSet) {
        var keysSet = _mapReverse[value];
        keysSet.Remove(key);
        if (keysSet.Count == 0)
          _mapReverse.Remove(value);
      }
    }

    public void DeleteByValue(TValue value, bool missingOk = false)
    {
      HashSet<TKey>? keysSet;

      if (!_mapReverse.Remove(value, out keysSet)) {
        if (!missingOk)
          throw new KeyNotFoundException();
        else
          return;
      }

      foreach (var key in keysSet) {
        var valuesSet = _mapForward[key];
        valuesSet.Remove(value);
        if (valuesSet.Count == 0)
          _mapForward.Remove(key);
      }
    }

    public IReadOnlyCollection<TValue> GetForward(TKey key) => _mapForward.GetValueOrDefault(key, EmptyValuesSet);
    public IReadOnlyCollection<TKey> GetReverse(TValue value) => _mapReverse.GetValueOrDefault(value, EmptyKeysSet);

    public bool ContainsKey(TKey key) => _mapForward.ContainsKey(key);
    public bool ContainsValue(TValue value) => _mapReverse.ContainsKey(value);

    public IReadOnlyCollection<TKey> Keys => _mapForward.Keys;
    public IReadOnlyCollection<TValue> Values => _mapReverse.Keys;

    internal int KeysMapCount => _mapForward.Count;
    internal int ValuesMapCount => _mapReverse.Count;
  }
}