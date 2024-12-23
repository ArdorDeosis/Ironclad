namespace Ironclad.Collections;

public interface IMultiValueDictionary<TKey, TValue> where TKey : notnull
{
  public IReadOnlyValueCollection<TValue> this[TKey key] { get; }

  public bool Add(TKey key, TValue value);
  
  public bool Remove(TKey key, TValue value);

  public void Clear(TKey key);

  public void Clear();
}

public interface IReadOnlyValueCollection<T> : IReadOnlyCollection<T>
{
  public bool Contains(T value);
}

public static class MultiValueDictionaryExtensions
{
  public static bool HasValues<TKey, TValue>(this IMultiValueDictionary<TKey, TValue> dictionary, TKey key)
    where TKey : notnull =>
    dictionary[key].Count != 0;
}