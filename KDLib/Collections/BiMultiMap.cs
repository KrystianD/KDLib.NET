using System.Collections.Generic;

namespace KDLib.Collections
{
  public class BiMultiMap<TKey, TValue> where TKey : notnull where TValue : notnull
  {
    private readonly Dictionary<TKey, HashSet<TValue>> _mapForward = new Dictionary<TKey, HashSet<TValue>>();
    private readonly Dictionary<TValue, HashSet<TKey>> _mapReverse = new Dictionary<TValue, HashSet<TKey>>();

    private static readonly HashSet<TKey> EmptyKeysSet = new HashSet<TKey>();
    private static readonly HashSet<TValue> EmptyValuesSet = new HashSet<TValue>();

    public void Add(TKey key, TValue value)
    {
      if (_mapForward.TryGetValue(key, out var set1))
        set1.Add(value);
      else
        _mapForward[key] = new HashSet<TValue>() { value };

      if (_mapReverse.TryGetValue(value, out var set2))
        set2.Add(key);
      else
        _mapReverse[value] = new HashSet<TKey>() { key };
    }

    public void DeleteByKey(TKey key, bool missingOk = false)
    {
      HashSet<TValue> valuesSet;

      if (!_mapForward.TryGetValue(key, out valuesSet)) {
        if (!missingOk)
          throw new KeyNotFoundException();
        else
          return;
      }

      _mapForward.Remove(key);

      foreach (var value in valuesSet) {
        var set = _mapReverse[value];
        set.Remove(key);
        if (set.Count == 0)
          _mapReverse.Remove(value);
      }
    }

    public void DeleteByValue(TValue value, bool missingOk = false)
    {
      HashSet<TKey> keysSet;

      if (!_mapReverse.TryGetValue(value, out keysSet)) {
        if (!missingOk)
          throw new KeyNotFoundException();
        else
          return;
      }

      _mapReverse.Remove(value);

      foreach (var key in keysSet) {
        var set = _mapForward[key];
        set.Remove(value);
        if (set.Count == 0)
          _mapForward.Remove(key);
      }
    }

    public IReadOnlyCollection<TValue> GetForward(TKey key)
    {
      return _mapForward.TryGetValue(key, out var valuesSet) ? valuesSet : EmptyValuesSet;
    }

    public IReadOnlyCollection<TKey> GetReverse(TValue value)
    {
      return _mapReverse.TryGetValue(value, out var keysSet) ? keysSet : EmptyKeysSet;
    }

    public bool ContainsKey(TKey key) => _mapForward.ContainsKey(key);
    public bool ContainsValue(TValue value) => _mapReverse.ContainsKey(value);

    internal int KeysMapCount => _mapForward.Count;
    internal int ValuesMapCount => _mapReverse.Count;
  }
}