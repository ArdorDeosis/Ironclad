using System.Collections;
using System.Collections.Immutable;

namespace Ironclad.Relations.Playground;

public class RelationCollection<T> : ICollection<T>
{
  private readonly ImmutableHashSet<T> set = [];

  public int Count => set.Count;
  public bool IsReadOnly => false;

  public void Add(T item)
  {
    throw new NotImplementedException(); 
  }

  public bool Remove(T item)
  {
    throw new NotImplementedException();
  }

  public void Clear()
  {
    throw new NotImplementedException();
  }

  public bool Contains(T item) => set.Contains(item);
  
  public void CopyTo(T[] array, int arrayIndex)
  {
    foreach (var element in set)
      array[arrayIndex++] = element;
  }

  public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}