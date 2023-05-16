using System.Collections;

namespace Ironclad.Collections;

// TODO: add equality comparers
public sealed class MultiValueDictionary<TKey, TValue> where TKey : notnull
{
  private readonly Dictionary<TKey, HashSet<TValue>> dictionary = new();

  public ValueSet this[TKey key] => new(this, key);

  public bool Add(TKey key, TValue value)
  {
    dictionary.Keys
    if (!dictionary.ContainsKey(key))
      dictionary.Add(key, new HashSet<TValue>());
    return dictionary[key].Add(value);
  }
  
  public bool Remove (TKey key)
  {
    if (dictionary.TryGetValue(key, out var set))
      set.Clear();
    return dictionary.Remove(key);
  }

  public void Clear() => dictionary.Clear();

  public readonly struct EmptyValueSet : IReadOnlyCollection<TValue>
  {
    public EmptyValueSet()
    { }

    public IEnumerator<TValue> GetEnumerator() => new EmptyEnumerator<TValue>();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => 0;
  }
  
  public readonly struct ValueSet : IReadOnlyCollection<TValue>
  {
    private readonly MultiValueDictionary<TKey, TValue> parent;
    private readonly TKey key;

    internal ValueSet(MultiValueDictionary<TKey, TValue> parent, TKey key)
    {
      this.parent = parent;
      this.key = key;
    }

    public IEnumerator<TValue> GetEnumerator() =>
      parent.dictionary.ContainsKey(key)
        ? parent[key].GetEnumerator()
        : new EmptyEnumerator<TValue>();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Add(TValue item)
    {
      if (!parent.dictionary.ContainsKey(key))
        parent.dictionary.Add(key, new HashSet<TValue>());
      return parent.dictionary[key].Add(item);
    }

    public void Clear() => parent.Remove(key);

    public bool Contains(TValue item) => parent.dictionary.TryGetValue(key, out var set) && set.Contains(item);

    public bool Remove(TValue item) => parent.dictionary.TryGetValue(key, out var set) && set.Remove(item);

    public int Count => parent.dictionary.TryGetValue(key, out var set) ? set.Count : 0;
  }
}

public readonly struct KeySetPair<TKey, TValue>
{
  
}